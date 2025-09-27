@echo off
echo ================================
echo Avvio stack con Docker Compose
echo ================================

docker-compose down
docker-compose up -d --build

echo ================================
echo Servizi avviati
echo API disponibile su: https://api.portfolio.dev
echo ================================
pause
