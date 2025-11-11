using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UniversityManagement.Application.DTOs.Messages;
using UniversityManagement.Application.Ports.Out;
using UniversityManagement.Infrastructure.Configuration;
using UniversityManagement.Infrastructure.Services;

namespace UniversityManagement.Infrastructure.Services;

/// <summary>
/// Servicio consumidor de RabbitMQ que procesa mensajes de notificación
/// </summary>
public class RabbitMQConsumerService : BackgroundService
{
    private readonly RabbitMQConnectionService _connectionService;
    private readonly RabbitMQSettings _settings;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RabbitMQConsumerService> _logger;
    private IModel? _channel;

    public RabbitMQConsumerService(
        RabbitMQConnectionService connectionService,
        IOptions<RabbitMQSettings> settings,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<RabbitMQConsumerService> logger)
    {
        _connectionService = connectionService;
        _settings = settings.Value;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Servicio Consumidor de RabbitMQ iniciado");

        try
        {
            _channel = _connectionService.GetChannel();
            
            // Configurar el consumidor para matrículas
            var enrollmentConsumer = new EventingBasicConsumer(_channel);
            enrollmentConsumer.Received += async (model, ea) =>
            {
                await ProcessEnrollmentMessage(ea);
            };

            // Configurar el consumidor para desmatrículas
            var unenrollmentConsumer = new EventingBasicConsumer(_channel);
            unenrollmentConsumer.Received += async (model, ea) =>
            {
                await ProcessUnenrollmentMessage(ea);
            };

            // Iniciar el consumo
            _channel.BasicConsume(
                queue: _settings.EnrollmentQueueName,
                autoAck: false,
                consumer: enrollmentConsumer
            );

            _channel.BasicConsume(
                queue: _settings.UnenrollmentQueueName,
                autoAck: false,
                consumer: unenrollmentConsumer
            );

            _logger.LogInformation("El consumidor esta escuchando");

            // Mantener el servicio corriendo
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("El servicio de RabbitMQ fue cancelado");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en servicio consumidor de RabbitMQ");
        }
    }

    private async Task ProcessEnrollmentMessage(BasicDeliverEventArgs ea)
    {
        try
        {
            var messageBody = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(messageBody);
            
            var notificationMessage = JsonSerializer.Deserialize<EnrollmentNotificationMessage>(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (notificationMessage != null)
            {
                _logger.LogInformation("Procesando notificacion de la matricula {StudentEmail}", notificationMessage.StudentEmail);

                // Crear un scope para usar servicios scoped
                using var scope = _serviceScopeFactory.CreateScope();
                var emailNotificationPort = scope.ServiceProvider.GetRequiredService<IEmailNotificationPort>();

                // También enviar confirmación de matrícula usando el método correcto
                await emailNotificationPort.SendEnrollmentConfirmation(
                    notificationMessage.StudentEmail,
                    notificationMessage.StudentName,
                    notificationMessage.CareerName,
                    notificationMessage.EnrollmentDate.ToString("dd/MM/yyyy")
                );

                _logger.LogInformation("Notificacion de matricula enviada a {StudentEmail}", notificationMessage.StudentEmail);
            }

            // Confirmar que el mensaje fue procesado
            _channel?.BasicAck(ea.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en lanotificacion de matricula");
            
            // Rechazar el mensaje y no requeued (evitar loops infinitos)
            _channel?.BasicNack(ea.DeliveryTag, false, false);
        }
    }

    private async Task ProcessUnenrollmentMessage(BasicDeliverEventArgs ea)
    {
        try
        {
            var messageBody = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(messageBody);
            
            var notificationMessage = JsonSerializer.Deserialize<EnrollmentNotificationMessage>(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (notificationMessage != null)
            {
                _logger.LogInformation("Proceso de desmatricula notificacion a {StudentEmail}", notificationMessage.StudentEmail);

                // Crear un scope para usar servicios scoped
                using var scope = _serviceScopeFactory.CreateScope();
                var emailNotificationPort = scope.ServiceProvider.GetRequiredService<IEmailNotificationPort>();

                // Enviar email de confirmación de desmatrícula usando el método correcto
                await emailNotificationPort.SendEnrollmentCancellation(
                    notificationMessage.StudentEmail,
                    notificationMessage.StudentName,
                    notificationMessage.CareerName,
                    notificationMessage.EnrollmentDate.ToString("dd/MM/yyyy")
                );

                _logger.LogInformation("Notificaicon de desmatricula enviada a {StudentEmail}", notificationMessage.StudentEmail);
            }

            // Confirmar que el mensaje fue procesado
            _channel?.BasicAck(ea.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en la notificaicon de desmatricula");
            
            // Rechazar el mensaje y no requeued
            _channel?.BasicNack(ea.DeliveryTag, false, false);
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        base.Dispose();
    }
}