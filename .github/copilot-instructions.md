# Universidad Management System - Clean Architecture

Este proyecto implementa un sistema de gestión universitaria utilizando .NET 8 con Clean Architecture.

## Estructura del Proyecto
- **Domain**: 
  - **Models**: Entidades de negocio (Student, Professor, Faculty, Career)
    - **Events**: Eventos de dominio
    - **ValueObjects**: Objetos de valor con validaciones
  - **Exceptions**: Excepciones específicas del dominio
  - **Repositories**: Interfaces de repositorios
  - **Services**: Servicios de dominio
- **Application**: Casos de uso, servicios e interfaces de aplicación
- **Infrastructure**: Repositorios, Entity Framework, configuraciones
- **Presentation**: Web API controllers

## Entidades principales
- Estudiante: información personal y académica
- Profesor: datos del docente y especialidad
- Facultad: organización académica
- Carrera: programas de estudio vinculados a facultades

## Tecnologías
- .NET 8
- Entity Framework Core
- Clean Architecture
- Domain-Driven Design (DDD)
- Repository Pattern
- Domain Events
- Value Objects