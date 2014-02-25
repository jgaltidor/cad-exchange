@echo off
setlocal
set PROE=C:\Program Files\PTC\Creo 2.0\Parametric\bin
set PRO_COMM_MSG_EXE=C:\Program Files\PTC\Creo 2.0\Common Files\F001\x86e_win64\obj\pro_comm_msg.exe
proexporter "%PROE%\parametric.exe" %*
endlocal
@echo on
