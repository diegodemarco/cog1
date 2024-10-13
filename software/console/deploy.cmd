@echo off
rd ..\shared\console /s /q
md ..\shared\console
call ng build --base-href=/console/ --output-path=..\shared\console
move ..\shared\console\browser ..\shared\console-browser
rd ..\shared\console /s /q
move ..\shared\console-browser ..\shared\console
pause