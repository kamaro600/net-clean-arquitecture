# 🏗️ Infrastructure Layer - Estructura Mejorada

Esta capa implementa los detalles técnicos y se conecta con el mundo exterior, siguiendo **Clean Architecture** y **Hexagonal Architecture**.

## 📁 Estructura Recomendada

```
UniversityManagement.Infrastructure/
├── 🔌 Adapters/                     # Patrón Adapter de Hexagonal Architecture
│   ├── In/                          # Adapters de entrada (Controllers externos, CLI, etc.)
│   └── Out/                         # Adapters de salida (Email, SMS, APIs externas)
│       ├── EmailNotificationAdapter.cs
│       └── SmsNotificationAdapter.cs
│
├── 💾 Data/                         # Todo lo relacionado con persistencia
│   ├── Configurations/              # Entity Framework configurations
│   │   ├── StudentConfiguration.cs
│   │   ├── CareerConfiguration.cs
│   │   ├── FacultyConfiguration.cs
│   │   ├── ProfessorConfiguration.cs
│   │   ├── StudentCareerConfiguration.cs
│   │   └── ProfessorCareerConfiguration.cs
│   └── UniversityDbContext.cs       # DbContext principal
│
├── 🌐 External/                     # Servicios externos (APIs de terceros)
│   └── [Para futuras integraciones con APIs externas]
│
├── 📦 Persistence/                  # Implementaciones de persistencia
│   └── Repositories/                # Implementaciones de repositorios del Domain
│       ├── StudentRepository.cs
│       ├── CareerRepository.cs
│       ├── FacultyRepository.cs
│       └── ProfessorRepository.cs
│
├── ⚙️ Services/                     # Servicios de Infrastructure
│   └── DomainEventDispatcher.cs     # Dispatcher de eventos de dominio
│
└── 🔧 DependencyInjection.cs        # Configuración de inyección de dependencias
```

## 🎯 Responsabilidades por Carpeta

### 🔌 **Adapters/**
- **In/**: Adapters que reciben información del exterior (no WebAPI, eso va en Presentation)
- **Out/**: Adapters que envían información al exterior (Email, SMS, APIs)
- **Principio**: Implementan interfaces definidas en Application (Ports)

### 💾 **Data/**
- **Configurations/**: Configuraciones de Entity Framework para cada entidad
- **DbContext**: Contexto de base de datos
- **Migrations**: Migraciones de base de datos (auto-generadas)

### 🌐 **External/**
- Clientes para APIs externas
- Wrappers de servicios de terceros
- Integraciones con sistemas externos

### 📦 **Persistence/**
- **Repositories/**: Implementaciones concretas de las interfaces de repositorios del Domain
- Acceso a datos específico para cada entidad

### ⚙️ **Services/**
- Servicios técnicos de Infrastructure
- Event dispatchers, loggers, cache, etc.
- NO servicios de negocio (esos van en Domain)

## 🔄 Flujo de Dependencias

```
Domain ←── Application ←── Infrastructure ←── WebApi
```

- **Infrastructure** puede referenciar **Domain** y **Application**
- **Infrastructure** implementa interfaces de **Application** (Ports)
- **Infrastructure** implementa interfaces de **Domain** (Repositories)

## ✅ Principios Aplicados

1. **Dependency Inversion**: Infrastructure implementa interfaces, no las define
2. **Single Responsibility**: Cada carpeta tiene una responsabilidad específica
3. **Hexagonal Architecture**: Separación clara entre Adapters In/Out
4. **Clean Architecture**: Infrastructure es un detalle de implementación

## 🚀 Ventajas de esta Estructura

- ✅ **Claridad**: Fácil ubicar componentes por responsabilidad
- ✅ **Escalabilidad**: Fácil agregar nuevos adapters o servicios
- ✅ **Testabilidad**: Separación clara facilita testing
- ✅ **Mantenibilidad**: Estructura predecible y bien organizada
- ✅ **Cumplimiento**: Sigue patrones de Clean/Hexagonal Architecture