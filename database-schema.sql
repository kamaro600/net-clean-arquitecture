
-- =============================================
-- TABLAS PRINCIPALES
-- =============================================

-- 1. Tabla: facultad
CREATE TABLE IF NOT EXISTS facultad (
    facultad_id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    ubicacion VARCHAR(100),
    decano VARCHAR(100),
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    activo BOOLEAN DEFAULT true,
    CONSTRAINT uk_facultad_nombre UNIQUE (nombre)
);

-- 2. Tabla: carrera
CREATE TABLE IF NOT EXISTS carrera (
    carrera_id SERIAL PRIMARY KEY,
    facultad_id INTEGER NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    duracion_semestres INTEGER NOT NULL,
    titulo_otorgado VARCHAR(100),
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    activo BOOLEAN DEFAULT true,
    CONSTRAINT fk_facultad FOREIGN KEY (facultad_id) REFERENCES facultad(facultad_id) ON DELETE RESTRICT,
    CONSTRAINT uk_carrera_nombre UNIQUE (nombre)
);

-- 3. Tabla: estudiante
CREATE TABLE IF NOT EXISTS estudiante (
    estudiante_id SERIAL PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    apellido VARCHAR(50) NOT NULL,
    dni VARCHAR(20) NOT NULL,
    email VARCHAR(100) NOT NULL,
    telefono VARCHAR(20),
    fecha_nacimiento DATE NOT NULL,
    direccion VARCHAR(200),
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    activo BOOLEAN DEFAULT true,
    CONSTRAINT uk_estudiante_dni UNIQUE (dni),
    CONSTRAINT uk_estudiante_email UNIQUE (email)
);

-- 4. Tabla: profesor
CREATE TABLE IF NOT EXISTS profesor (
    profesor_id SERIAL PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    apellido VARCHAR(50) NOT NULL,
    dni VARCHAR(20) NOT NULL,
    email VARCHAR(100) NOT NULL,
    telefono VARCHAR(20),
    especialidad VARCHAR(100),
    titulo_academico VARCHAR(100),
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    activo BOOLEAN DEFAULT true,
    CONSTRAINT uk_profesor_dni UNIQUE (dni),
    CONSTRAINT uk_profesor_email UNIQUE (email)
);

-- =============================================
-- TABLAS DE RELACIÓN (Many-to-Many)
-- =============================================

-- 5. Tabla: estudiante_carrera
CREATE TABLE IF NOT EXISTS estudiante_carrera (
    estudiante_id INTEGER NOT NULL,
    carrera_id INTEGER NOT NULL,
    fecha_inscripcion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    activo BOOLEAN DEFAULT true,
    PRIMARY KEY (estudiante_id, carrera_id),
    CONSTRAINT fk_estudiante_carrera_estudiante FOREIGN KEY (estudiante_id) REFERENCES estudiante(estudiante_id) ON DELETE CASCADE,
    CONSTRAINT fk_estudiante_carrera_carrera FOREIGN KEY (carrera_id) REFERENCES carrera(carrera_id) ON DELETE CASCADE
);

-- 6. Tabla: profesor_carrera
CREATE TABLE IF NOT EXISTS profesor_carrera (
    profesor_id INTEGER NOT NULL,
    carrera_id INTEGER NOT NULL,
    fecha_asignacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    activo BOOLEAN DEFAULT true,
    PRIMARY KEY (profesor_id, carrera_id),
    CONSTRAINT fk_profesor_carrera_profesor FOREIGN KEY (profesor_id) REFERENCES profesor(profesor_id) ON DELETE CASCADE,
    CONSTRAINT fk_profesor_carrera_carrera FOREIGN KEY (carrera_id) REFERENCES carrera(carrera_id) ON DELETE CASCADE
);

-- =============================================
-- ÍNDICES ADICIONALES PARA PERFORMANCE
-- =============================================

-- Índices para búsquedas frecuentes
CREATE INDEX idx_carrera_facultad ON carrera(facultad_id);
CREATE INDEX idx_estudiante_nombre ON estudiante(nombre, apellido);
CREATE INDEX idx_profesor_nombre ON profesor(nombre, apellido);
CREATE INDEX idx_estudiante_carrera_estudiante ON estudiante_carrera(estudiante_id);
CREATE INDEX idx_estudiante_carrera_carrera ON estudiante_carrera(carrera_id);
CREATE INDEX idx_profesor_carrera_profesor ON profesor_carrera(profesor_id);
CREATE INDEX idx_profesor_carrera_carrera ON profesor_carrera(carrera_id);

-- =============================================
-- DATOS DE EJEMPLO (OPCIONAL)
-- =============================================

-- Insertar facultades de ejemplo
INSERT INTO facultad (nombre, descripcion, ubicacion, decano) VALUES
('Facultad de Ingeniería', 'Facultad dedicada a las carreras de ingeniería', 'Edificio A', 'Dr. Juan Pérez'),
('Facultad de Ciencias', 'Facultad de ciencias exactas y naturales', 'Edificio B', 'Dra. María González'),
('Facultad de Humanidades', 'Facultad de ciencias humanas y sociales', 'Edificio C', 'Dr. Carlos Rodríguez');

-- Insertar carreras de ejemplo
INSERT INTO carrera (facultad_id, nombre, descripcion, duracion_semestres, titulo_otorgado) VALUES
(1, 'Ingeniería en Sistemas', 'Carrera de ingeniería en sistemas computacionales', 10, 'Ingeniero en Sistemas'),
(1, 'Ingeniería Civil', 'Carrera de ingeniería civil', 12, 'Ingeniero Civil'),
(2, 'Licenciatura en Matemáticas', 'Carrera de matemáticas puras y aplicadas', 8, 'Licenciado en Matemáticas'),
(2, 'Licenciatura en Física', 'Carrera de física teórica y experimental', 8, 'Licenciado en Física'),
(3, 'Licenciatura en Historia', 'Carrera de historia universal y nacional', 8, 'Licenciado en Historia');

-- Insertar estudiantes de ejemplo
INSERT INTO estudiante (nombre, apellido, dni, email, telefono, fecha_nacimiento, direccion) VALUES
('Ana', 'García', '12345678', 'ana.garcia@email.com', '555-0001', '2000-05-15', 'Calle Principal 123'),
('Luis', 'Martínez', '87654321', 'luis.martinez@email.com', '555-0002', '1999-08-22', 'Av. Central 456'),
('Sofia', 'López', '11223344', 'sofia.lopez@email.com', '555-0003', '2001-03-10', 'Plaza Mayor 789');

-- Insertar profesores de ejemplo
INSERT INTO profesor (nombre, apellido, dni, email, telefono, especialidad, titulo_academico) VALUES
('Roberto', 'Silva', '99887766', 'roberto.silva@universidad.edu', '555-1001', 'Programación', 'Master en Ciencias de la Computación'),
('Elena', 'Vargas', '55443322', 'elena.vargas@universidad.edu', '555-1002', 'Matemáticas', 'PhD en Matemáticas'),
('Diego', 'Morales', '77889900', 'diego.morales@universidad.edu', '555-1003', 'Historia', 'PhD en Historia');

-- Insertar relaciones estudiante-carrera
INSERT INTO estudiante_carrera (estudiante_id, carrera_id) VALUES
(1, 1),  -- Ana en Ingeniería en Sistemas
(2, 1),  -- Luis en Ingeniería en Sistemas
(3, 3);  -- Sofia en Matemáticas

-- Insertar relaciones profesor-carrera
INSERT INTO profesor_carrera (profesor_id, carrera_id) VALUES
(1, 1),  -- Roberto enseña Ingeniería en Sistemas
(2, 3),  -- Elena enseña Matemáticas
(2, 4),  -- Elena también enseña Física
(3, 5);  -- Diego enseña Historia

-- =============================================
-- TABLA DE AUDITORÍA PARA KAFKA
-- =============================================

-- Tabla: audit_logs (Para sistema de auditoría con Kafka)
CREATE TABLE IF NOT EXISTS audit_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    event_type VARCHAR(50) NOT NULL,
    entity_name VARCHAR(100) NOT NULL,
    entity_id VARCHAR(50) NOT NULL,
    action VARCHAR(50) NOT NULL,
    user_id VARCHAR(50) NOT NULL DEFAULT 'System',
    user_name VARCHAR(100) NOT NULL DEFAULT 'System',
    timestamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    old_values TEXT,
    new_values TEXT,
    additional_data TEXT,
    ip_address VARCHAR(45),
    user_agent TEXT
);

-- Índices para optimizar consultas de auditoría
CREATE INDEX idx_audit_logs_timestamp ON audit_logs (timestamp DESC);
CREATE INDEX idx_audit_logs_event_type ON audit_logs (event_type);
CREATE INDEX idx_audit_logs_entity ON audit_logs (entity_name, entity_id);
CREATE INDEX idx_audit_logs_user ON audit_logs (user_id);
CREATE INDEX idx_audit_logs_action ON audit_logs (action);

-- =============================================
-- VERIFICACIÓN DE LA CREACIÓN
-- =============================================

-- Consultar información de las tablas creadas
SELECT 
    schemaname,
    tablename,
    tableowner
FROM pg_tables 
WHERE schemaname = 'public'
ORDER BY tablename;

-- Consultar datos insertados
SELECT 'Facultades' as tabla, count(*) as registros FROM facultad
UNION ALL
SELECT 'Carreras', count(*) FROM carrera
UNION ALL
SELECT 'Estudiantes', count(*) FROM estudiante
UNION ALL
SELECT 'Profesores', count(*) FROM profesor
UNION ALL
SELECT 'Estudiante-Carrera', count(*) FROM estudiante_carrera
UNION ALL
SELECT 'Audit-Logs', count(*) FROM audit_logs
UNION ALL
SELECT 'Profesor-Carrera', count(*) FROM profesor_carrera;

-- =============================================
-- SCRIPT COMPLETADO
-- Base de datos Universidad lista para usar
-- =============================================