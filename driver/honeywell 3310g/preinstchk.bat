@echo off
@rem.
@rem =====================================================================
@rem Name: preinstchk.bat 
@rem version: 1.5
@rem Description: This utility invokes before any installation take place. 
@rem              It is called by setup.bat.
@rem              The goal is to do all the pre checking activities:
@rem              1/ check to see if the user run as admin.
@rem              2/ check to see if there is any installed MSI driver.
@rem                 If there is an installed MSI driver AND its version is NOT the same as the new MSI version
@rem                   then we need to exit
@rem                 Else 
@rem 
@rem Usage:
@rem checkmsi
@rem Revision History:
@rem  Version       Author      Description
@rem   1.0          KL          Initial Release
@rem   1.1          KL          Do not check for honeywell_enum.sys
@rem   1.2          KL	        reg.exe 3.0, the earlier version on POSReady 2009. 
@rem                            It outputs the utility header information hence messed up
@rem                            the output format. So, we need to deal with this.
@rem   1.3          KL          Windows XP is using the same reg.exe as that of POSReady 2009.
@rem                            So, we need to fix for XP
@rem   1.4          KL          Figure out if MS Visual C++ Redistributable package is installed
@rem                            We cheat, assuming that findUsbPkgVer.exe fails mean the vcredist is not installed.
@rem   1.5          KL          Fix %USERPROFILE%, make sure that this can handle special character.
@rem   1.6          KL          Figure out the correct msi file and use that instead of setup.exe
@rem =====================================================================
@rem.
@rem.
@set POSREADY2009=yes
@set MSIINSTALLEXIST=no
@set TARGETOS="Windows POSReady 2009"
set CHECKMSI_VER=1.6
set foundVer=0
set installVer=0
set msifile=
set continueSetup=yes
set vcredist=yes
set msitransform=
set REGOUTPRE="%USERPROFILE%\regout-pre.txt"
set REGOUT="%USERPROFILE%\regout.txt"
set INSTALLMSIFILE="%USERPROFILE%\installmsifile.txt"
echo CHECKMSI_VER=%CHECKMSI_VER%
if exist %REGOUTPRE% (
@del /F /Q %REGOUTPRE% > nul 2>&1
)
if exist %REGOUT% (
@del /F /Q %REGOUT% > nul 2>&1
)
if exist %INSTALLMSIFILE% (
@del /F /Q %INSTALLMSIFILE% > nul 2>&1
)
@rem
if %PROCESSOR_ARCHITECTURE%==x86 (
dir x86-ForInternalUseOnly\*.msi /A:H /B > %INSTALLMSIFILE%
set SYSTEMTYPE=x86-ForInternalUseOnly
set msitransform=usb-serial-32-transform.mst
) else (
dir x64-ForInternalUseOnly\*.msi /A:H /B > %INSTALLMSIFILE%
set SYSTEMTYPE=x64-ForInternalUseOnly
set msitransform=usb-serial-64-transform.mst
)
@rem
set /p msifile=<%INSTALLMSIFILE%
@rem
if exist %SystemRoot%\System32\findstr.exe (
set FINDSTR="%SystemRoot%\System32\findstr.exe"
set POSREADY2009=no
) else (
echo POSREADY2009=%POSREADY2009%
set FINDSTR=
set FINDUSBVER="%cd%\findUsbPkgVer.exe"
)
if exist %SystemRoot%\System32\reg.exe (
set REG="%SystemRoot%\System32\reg.exe"
) else (
echo ...reg.exe does not exist... bail ...
goto END
)
@rem.
@rem move on ....
if "%POSREADY2009%"=="yes" (goto DONEOS)
@rem If not, find out the OS version ...
ver | %FINDSTR% /C:"5.0" > null
if %ERRORLEVEL%==0 goto WIN2K
ver | %FINDSTR% /C:"5.1" > null
if %ERRORLEVEL%==0 goto WINXP
ver | %FINDSTR% /C:"5.2" > null
if %ERRORLEVEL%==0 goto WINXP_64
ver | %FINDSTR% /C:"6.0" > null
if %ERRORLEVEL%==0 goto WINVISTA
ver | %FINDSTR% /C:"6.1" > null
if %ERRORLEVEL%==0 goto WIN7
ver | %FINDSTR% /C:"6.2" > null
if %ERRORLEVEL%==0 goto WIN8
goto DONEOS

:WIN2K
@set TARGETOS="Windows 2000"
echo Unsupported OS: %TARGETOS% ...
goto END

:WINXP
@set TARGETOS="Windows XP"
@set DRIVERSTOREPATH="%SystemRoot%\System32\DRVSTORE"
@set FINDUSBVER="%cd%\findUsbPkgVer.exe"
goto DONEOS

:WINXP_64
@set TARGETOS="Windows XP 64"
@set DRIVERSTOREPATH="%SystemRoot%\System32\DRVSTORE"
goto DONEOS

:WINVISTA
@set TARGETOS="Windows Vista"
@set DRIVERSTOREPATH="%SystemRoot%\System32\DriverStore\FileRepository"
goto DONEOS

:WIN7
@set TARGETOS="Windows 7"
@set DRIVERSTOREPATH="%SystemRoot%\System32\DriverStore\FileRepository"
goto DONEOS

:WIN8
@set TARGETOS="Windows 8"
@set DRIVERSTOREPATH="%SystemRoot%\System32\DriverStore\FileRepository"
goto DONEOS

:DONEOS
@rem query and surppress all output from reg.exe
%REG% query HKLM\SYSTEM\CurrentControlSet\Services\Honeywell_enum\Parameters /v UsbDriverVer > nul 2>&1
if %ERRORLEVEL%==1 (goto GETINVER)
@rem we are here, this means that the registry "UsbDriverVer" exists. Get it.
@rem, depending on what OS, we need to use different method to get the UsbDriverVer from registry
@rem.
@pushd .
cd /d "%USERPROFILE%"
if "%POSREADY2009%"=="yes" (goto POSR)
if %TARGETOS%=="Windows XP" (goto POSR)
@rem this is for non POSReady 2009
%REG% query HKLM\SYSTEM\CurrentControlSet\Services\Honeywell_enum\Parameters /v UsbDriverVer > %REGOUT%
for /f "tokens=3 delims= " %%A in ('@type %REGOUT%') do @set foundVer=%%A
goto NPOSR
:POSR
@rem, first we need to save this into a file, so that we can later process with findString1.exe
%REG% query HKLM\SYSTEM\CurrentControlSet\Services\Honeywell_enum\Parameters /v UsbDriverVer > %REGOUTPRE%
%FINDUSBVER% REG_SZ %REGOUTPRE% > %REGOUT%
@rem, if there is an error, this would mean that there is no MS C++ 2010 redistributable installed.
@rem, get out and return
if not %ERRORLEVEL%==0 (
set vcredist=no
@popd
goto END
)
for /f "tokens=1 delims= " %%A in ('@type %REGOUT%') do @set foundVer=%%A
:NPOSR
@popd
@rem.
if %ERRORLEVEL%==0 ( goto GETINVER )
goto END
:GETINVER
@rem now, we need to check the installed version
set /p installVer=<.\driver_package_reg_ver.txt
if not %installVer%==%foundVer% ( goto CHECKMORE )
goto END
:CHECKMORE
@rem check to see if foundVer is non zero
if not %foundVer%==0 ( set continueSetup=no )
:END
if exist null del /F /Q null
echo ... foundVer=%foundVer%
echo ... installVer=%installVer%
echo ... continueSetup=%continueSetup%
echo preinstchk completes ....
