@echo off
cd /d %~d0%~p0
setlocal enableextensions enabledelayedexpansion
set RETURNCODE=0
@rem checking for admin right here. If not, print out a warning for the user
@rem and exit out.
net session >nul 2>&1
if %errorlevel% NEQ 0 (
echo ...Administrator Privilege is required to install the driver
echo ...Please consult the README.txt for install/uninstall instruction
set RETURNCODE=%errorlevel%
goto DISP
)
@call preinstchk.bat
if "%vcredist%"=="no" ( goto VCREDIST)
if %continueSetup%==no (goto WARN)
@pushd .
@cd /d %SYSTEMTYPE%
@msiexec /i "%msifile%" TRANSFORMS=%msitransform%
set RETURNCODE=%errorlevel%
@popd
goto END
:VCREDIST
echo ------ Microsoft Visual C++ 2010 Redistributable Package is required to be installed on target computer
echo ------ Please consult the README.txt for installation instruction, then try again.
goto DISP
:WARN
echo ======== WARNING WARNING: Cannot install this driver =========================================
echo There is a different version of MSI driver package, ver=%foundVer% is currently installed
echo Please first remove the current MSI installed driver in 'Control Panel\Add and Remove program'
echo Then try to install this driver again.
echo ==============================================================================================
@rem make sure that user can see the above warning message.
:DISP
@pause
:END
exit /B !RETURNCODE!

