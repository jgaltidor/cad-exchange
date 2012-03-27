rem This script must be ran from its parent directory to have
rem Pro/E read the protk.dat in this directory
setlocal
set PROE=C:\Program Files\proeWildfire 4.0
"%PROE%\bin\proe.exe" example_draft.prt.1 +EXAMPLE_DRAFT +xml_EXAMPLE_DRAFT
endlocal
