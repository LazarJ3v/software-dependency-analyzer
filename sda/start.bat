@echo off

echo Starting Backend...
start "Backend - ASP.NET Core" cmd /k "cd /d %~dp0 && dotnet watch run"

echo Starting Frontend...
start "Frontend - Vite" cmd /k "cd /d %~dp0frontend && npm run dev"

timeout /t 5 /nobreak > nul
start http://localhost:5173

exit