Write-Host "🛑 Deteniendo RabbitMQ de desarrollo..." -ForegroundColor Red

docker-compose -f docker-compose.rabbitmq.yml down

Write-Host ""
Write-Host "✅ RabbitMQ detenido correctamente" -ForegroundColor Green
Write-Host ""
Write-Host "🔄 Para reiniciar:" -ForegroundColor White
Write-Host "  .\start-rabbitmq.ps1" -ForegroundColor Gray