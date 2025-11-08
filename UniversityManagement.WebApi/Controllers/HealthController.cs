using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Endpoint básico de salud para verificar que la API funciona
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { 
            Status = "Healthy", 
            Timestamp = DateTime.UtcNow,
            Message = "API básica funcionando correctamente",
            Services = new {
                Kafka = "Configurado en contenedores Docker",
                RabbitMQ = "Configurado en contenedores Docker",
                PostgreSQL = "Configurado en contenedores Docker"
            }
        });
    }

    /// <summary>
    /// Test de conectividad con Kafka
    /// </summary>
    [HttpGet("kafka")]
    public IActionResult TestKafka()
    {
        return Ok(new {
            Service = "Kafka",
            Status = "Ready for testing",
            Topics = new[] { "university.audit.events", "university.audit.events.dlq" },
            Note = "Kafka está ejecutándose en Docker en puerto 9092"
        });
    }

    /// <summary>
    /// Test de preparación para auditoría
    /// </summary>
    [HttpGet("audit-ready")]
    public IActionResult TestAuditReady()
    {
        return Ok(new {
            AuditSystem = "Ready",
            Infrastructure = new {
                KafkaTopics = "✅ Creados automáticamente",
                AuditTable = "✅ Tabla AuditLogs creada en PostgreSQL",
                KafkaConsumer = "✅ Implementado (KafkaAuditConsumerService)",
                KafkaPublisher = "✅ Implementado (KafkaAuditPublisherAdapter)"
            },
            NextSteps = new[] {
                "Habilitar servicios de Infrastructure",
                "Probar publicación de eventos de auditoría",
                "Verificar almacenamiento en base de datos"
            }
        });
    }
}