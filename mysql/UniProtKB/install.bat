@echo off

REM install mysql to vb.net source code into the data project

reflector --reflects /sql ./kb_go.sql /namespace "kb_go" /split /auto_increment.disable
xcopy "./kb_go/*.*" "../../src/GCModeller/data/GO_gene-ontology/GO_mysql/kb_go" /s /h /d /y /e /f /i
RD /S /Q "./kb_go/"