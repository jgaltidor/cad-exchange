@echo off
setlocal
set PROE=C:\Program Files\proeWildfire 4.0
set MACHINE=i486_nt
rem PRO_COMM_MSG_EXE must be set for asynchronous mode
rem Refer to p. 337 of tkuse.pdf (ProToolkit User's Guide) for more info
set PRO_COMM_MSG_EXE=%PROE%\%MACHINE%\obj\pro_comm_msg.exe
proexporter "%PROE%\bin\proe.exe" %*
endlocal
@echo on
