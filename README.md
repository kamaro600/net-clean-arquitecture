# 🎓 Sistema universidad - Clean Architecture

> Sistema de gestión universitaria implementado con .NET 8, Clean Architecture y Value Objects.

## 🚀 Inicio Rápido

### ⚡ Configuración en 3 pasos

```bash
# 1. Clonar y navegar al proyecto
git clone https://github.com/kamaro600/net-clean-arquitecture.git
cd net-clean-arquitecture

# 2. Configurar base de datos PostgreSQL
# Actualizar ConnectionString en appsettings.json
# Ejecutar script: database-schema.sql

# 3. Ejecutar aplicación
dotnet run --project UniversityManagement.WebApi
```

🌐 **API disponible en:** `http://localhost:5064/swagger`

### 📋 Requisitos Previos

- **.NET 8 SDK** o superior
- **PostgreSQL 12+** 
- **Visual Studio 2022** o **VS Code** (recomendado)

## 🏢 Estructura Detallada del Proyecto

```
UniversityManagement/
├── 🌐 UniversityManagement.WebApi/         # Capa de Presentación
│   ├── Controllers/                         # Controladores REST API
│   │   ├── StudentsController.cs           # 👨‍🎓 CRUD Estudiantes
│   │   ├── ProfessorsController.cs         # 👨‍🏫 CRUD Profesores  
│   │   ├── CareersController.cs            # 📚 CRUD Carreras
│   │   └── FacultiesController.cs          # 🏛️ CRUD Facultades
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs  # Manejo global de excepciones
│   └── Program.cs                          # Configuración DI y servicios
│
├── 📱 UniversityManagement.Application/     # Capa de Aplicación
│   ├── Services/                           # Casos de uso (Use Cases)
│   │   ├── StudentUseCase.cs               # Lógica de negocio estudiantes
│   │   ├── ProfessorUseCase.cs             # Lógica de negocio profesores
│   │   ├── CareerUseCase.cs                # Lógica de negocio carreras
│   │   └── FacultyUseCase.cs               # Lógica de negocio facultades
│   ├── Ports/In/                           # Interfaces de entrada
│   │   └── I*UseCase.cs                    # Contratos de casos de uso
│   ├── Ports/Out/                          # Interfaces de salida  
│   │   └── I*NotificationPort.cs           # Contratos notificaciones
│   ├── DTOs/                               # Objetos de transferencia
│   │   ├── Commands/                       # DTOs para comandos (escritura)
│   │   ├── Queries/                        # DTOs para consultas (lectura)
│   │   └── Responses/                      # DTOs de respuesta
│   └── Mappers/                            # Conversión Entity ↔ DTO
│
├── 💼 UniversityManagement.Domain/          # Capa de Dominio (Core)
│   ├── Models/                             # Entidades de dominio
│   │   ├── Student.cs                      # 👨‍🎓 Entidad Estudiante + Value Objects
│   │   ├── Professor.cs                    # 👨‍🏫 Entidad Profesor
│   │   ├── Career.cs                       # 📚 Entidad Carrera
│   │   ├── Faculty.cs                      # 🏛️ Entidad Facultad
│   │   ├── StudentCareer.cs                # Relación N:M Estudiante-Carrera
│   │   ├── ProfessorCareer.cs              # Relación N:M Profesor-Carrera
│   │   └── ValueObjects/                   # Objetos de valor con validaciones
│   │       ├── FullName.cs                 # Nombre completo validado
│   │       ├── Dni.cs                      # DNI con validación formato
│   │       ├── Email.cs                    # Email con validación regex
│   │       ├── Phone.cs                    # Teléfono con validación formato
│   │       └── Address.cs                  # Dirección completa
│   ├── Services/                           # Servicios de dominio
│   │   ├── StudentDomainService.cs         # Validaciones complejas estudiantes
│   │   └── ProfessorDomainService.cs       # Validaciones complejas profesores
│   ├── Repositories/                       # Interfaces de repositorios
│   ├── Exceptions/                         # Excepciones específicas del dominio
│   └── Common/                             # Clases base y comunes
│
└── 🔧 UniversityManagement.Infrastructure/  # Capa de Infraestructura
    ├── Data/                               # Contexto Entity Framework
    │   ├── UniversityDbContext.cs          # DbContext principal
    │   └── Configurations/                 # Configuraciones EF Core
    │       ├── StudentConfiguration.cs     # Mapeo entidad Student
    │       ├── ProfessorConfiguration.cs   # Mapeo entidad Professor
    │       ├── CareerConfiguration.cs      # Mapeo entidad Career
    │       └── FacultyConfiguration.cs     # Mapeo entidad Faculty
    ├── Persistence/Repositories/           # Implementaciones repositorios
    │   ├── StudentRepository.cs            # Acceso datos estudiantes
    │   ├── ProfessorRepository.cs          # Acceso datos profesores  
    │   ├── CareerRepository.cs             # Acceso datos carreras
    │   └── FacultyRepository.cs            # Acceso datos facultades
    ├── Adapters/Out/                       # Adaptadores servicios externos
    │   ├── EmailNotificationAdapter.cs     # Envío emails
    │   └── SmsNotificationAdapter.cs       # Envío SMS
    └── DependencyInjection.cs             # Registro servicios infrastructure
```

## 🛠️ Tecnologías y Herramientas

### 🔧 Backend
- **.NET 8** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core 9** - ORM
- **PostgreSQL** - Base de datos
- **Swagger/OpenAPI** - Documentación API


## 💾 Base de Datos

### 🐘 PostgreSQL Setup

```sql
-- Crear base de datos
CREATE DATABASE universidad_management;

-- Ejecutar script completo
\i database-schema.sql
```

### 📋 Script de Inicialización

El archivo `database-schema.sql` incluye:
- ✅ Creación de todas las tablas
- ✅ Índices y constrains
- ✅ Datos de prueba
- ✅ Relaciones foreign keys

---

### 🔍 Pruebas Rápidas

```bash
# Crear un estudiante
curl -X POST "http://localhost:5064/api/students" \
     -H "Content-Type: application/json" \
     -d '{
       "nombre": "Juan",
       "apellido": "Pérez", 
       "dni": "12345678",
       "email": "juan@email.com",
       "fechaNacimiento": "1995-05-15"
     }'

# Listar estudiantes  
curl "http://localhost:5064/api/students"
```
