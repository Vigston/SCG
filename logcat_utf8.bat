@echo off
:: Set code page to UTF-8
chcp 65001 >nul

:: Ensure PowerShell uses UTF-8 for output
powershell -Command "$OutputEncoding = [Console]::OutputEncoding = [System.Text.Encoding]::UTF8"

:: Run adb logcat for Unity logs
adb logcat -v time Unity:D *:S

pause
