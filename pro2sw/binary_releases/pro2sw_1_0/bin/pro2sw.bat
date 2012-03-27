@echo off
set PROTOSW=%~dp0..
set PATH=%PROTOSW%\3rdparty\proexporter;%PATH%
set PATH=%PROTOSW%\3rdparty\psconv\bin;%PATH%
set PATH=%PROTOSW%\3rdparty\swimporter\bin;%PATH%
set CLASSPATH=%PROTOSW%;%CLASSPATH%
java protosw.ProToSW %*
@echo on
