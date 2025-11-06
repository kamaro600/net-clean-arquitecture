Write-Host "Iniciando RabbitMQ para desarrollo local..." -ForegroundColor Green

# Detener cualquier contenedor previo
Write-Host "Deteniendo contenedores previos..." -ForegroundColor Yellow
docker-compose -f docker-compose.rabbitmq.yml down

# Levantar solo RabbitMQ
Write-Host "Iniciando RabbitMQ..." -ForegroundColor Yellow
docker-compose -f docker-compose.rabbitmq.yml up -d

# Esperar que RabbitMQ esté listo
Write-Host "Esperando que RabbitMQ este listo..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

# Verificar estado
Write-Host "Verificando estado de RabbitMQ..." -ForegroundColor Yellow
docker-compose -f docker-compose.rabbitmq.yml ps

Write-Host ""
Write-Host "RabbitMQ listo para desarrollo!" -ForegroundColor Green
Write-Host ""
Write-Host "RabbitMQ Management UI: http://localhost:15672" -ForegroundColor Cyan
Write-Host "Usuario: guest" -ForegroundColor Cyan
Write-Host "Password: guest" -ForegroundColor Cyan
Write-Host "Puerto AMQP: 5672" -ForegroundColor Cyan
Write-Host ""
Write-Host "Para detener RabbitMQ:" -ForegroundColor White
Write-Host "  docker-compose -f docker-compose.rabbitmq.yml down" -ForegroundColor Gray