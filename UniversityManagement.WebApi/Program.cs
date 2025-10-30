using Microsoft.EntityFrameworkCore;
using UniversityManagement.Application.Services;
using UniversityManagement.Application.Ports.In;
using UniversityManagement.Infrastructure.Data;
using UniversityManagement.Infrastructure;
using UniversityManagement.WebApi.Middleware;

// Configurar comportamiento de DateTime para PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Universidad Management API",
        Version = "v1",
        Description = "API para gestión universitaria - Clean Architecture Implementation",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Universidad Management System",
            Email = "admin@universidad.edu"
        }
    });
    
    // Incluir comentarios XML si existen
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configurar base de datos
builder.Services.AddDbContext<UniversityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar servicios de Infrastructure usando el extension method
builder.Services.AddInfrastructure(builder.Configuration);

// Registrar casos de uso de Application
builder.Services.AddScoped<IStudentUseCase, StudentUseCase>();
builder.Services.AddScoped<ICareerUseCase, CareerUseCase>();
builder.Services.AddScoped<IFacultyUseCase, FacultyUseCase>();
builder.Services.AddScoped<IProfessorUseCase, ProfessorUseCase>();

// Configurar CORS si es necesario
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Middleware de manejo de excepciones (debe ser el primero para capturar todas las excepciones)
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    // Habilitar Swagger UI en desarrollo
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Universidad Management API v1");
        c.RoutePrefix = "swagger"; // Swagger UI será accesible en /swagger
        c.DocumentTitle = "Universidad Management API";
        c.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseRouting();

app.MapControllers();

app.Run();
