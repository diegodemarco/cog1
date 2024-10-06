@echo off
rd ..\shared\admin /s /q
md ..\shared\admin
call ng build --base-href=/admin/ --output-path=..\shared\admin
move ..\shared\admin\browser ..\shared\admin-browser
rd ..\shared\admin /s /q
move ..\shared\admin-browser ..\shared\admin
