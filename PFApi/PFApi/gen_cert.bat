@echo off
setlocal enabledelayedexpansion

REM Ottieni path corrente e convertilo
set CURDIR=%cd%
set CURDIR=/c/%CURDIR:C:\=%
set CURDIR=%CURDIR:\=/%

echo Usando path: %CURDIR%

docker run --rm ^
  -v %CURDIR%/certbot:/etc/letsencrypt ^
  -v %CURDIR%/html:/var/www/html ^
  certbot/certbot certonly --webroot -w /var/www/html ^
  -d api.portfolio.dev --agree-tos -m tuaemail@mail.com --non-interactive

pause
