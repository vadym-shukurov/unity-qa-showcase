@echo off
REM Run Unity tests from command line. Requires Unity 2022.3+ installed.
REM Usage: run-tests.bat [editmode|playmode|all]
REM Set UNITY_PATH if Unity is not in default location.

setlocal
set MODE=%~1
if "%MODE%"=="" set MODE=all

set "PROJECT_PATH=%~dp0.."
cd /d "%PROJECT_PATH%"

REM Unity path: env var or default Hub location
if defined UNITY_PATH (
  set "UNITY=%UNITY_PATH%"
) else (
  for /d %%d in ("%ProgramFiles%\Unity\Hub\Editor\*") do set "UNITY=%%d\Editor\Unity.exe"
  if not exist "%UNITY%" (
    for /d %%d in ("%ProgramFiles(x86)%\Unity\Editor\*") do set "UNITY=%%d\Unity.exe"
  )
)

if not exist "%UNITY%" (
  echo Unity not found. Set UNITY_PATH or install Unity 2022.3.
  echo   Example: set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2022.3.0f1\Editor\Unity.exe"
  exit /b 1
)

if not exist "reports" mkdir reports

if "%MODE%"=="editmode" goto editmode
if "%MODE%"=="playmode" goto playmode
if "%MODE%"=="all" goto all
echo Usage: %~nx0 [editmode^|playmode^|all]
exit /b 1

:editmode
echo Running Edit Mode tests...
"%UNITY%" -batchmode -nographics -projectPath "%PROJECT_PATH%" -runTests -testPlatform EditMode -testResults "%PROJECT_PATH%\reports\editmode-results.xml" -logFile "%PROJECT_PATH%\reports\editmode.log"
goto end

:playmode
echo Running Play Mode tests...
"%UNITY%" -batchmode -nographics -projectPath "%PROJECT_PATH%" -runTests -testPlatform PlayMode -testResults "%PROJECT_PATH%\reports\playmode-results.xml" -logFile "%PROJECT_PATH%\reports\playmode.log"
goto end

:all
echo Running Edit Mode tests...
"%UNITY%" -batchmode -nographics -projectPath "%PROJECT_PATH%" -runTests -testPlatform EditMode -testResults "%PROJECT_PATH%\reports\editmode-results.xml" -logFile "%PROJECT_PATH%\reports\editmode.log"
echo Running Play Mode tests...
"%UNITY%" -batchmode -nographics -projectPath "%PROJECT_PATH%" -runTests -testPlatform PlayMode -testResults "%PROJECT_PATH%\reports\playmode-results.xml" -logFile "%PROJECT_PATH%\reports\playmode.log"
goto end

:end
echo Tests complete. Results in %PROJECT_PATH%\reports\
endlocal
