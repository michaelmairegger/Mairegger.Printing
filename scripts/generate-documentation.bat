@ECHO OFF
PUSHD %~dp0
PowerShell -NoProfile -ExecutionPolicy Bypass -Command ".\generate-documentation.ps1 %*; exit $LastExitCode;"
POPD
