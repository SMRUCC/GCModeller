@echo off

REM install mysql to vb.net source code into the data project

REM install go.obo mysql ORM adapter
SET dir="../../src/GCModeller/data/GO_gene-ontology/GO_mysql/kb_go"
reflector --reflects /sql ./kb_go.sql /namespace "kb_go" /split /auto_increment.disable
RD /S /Q %dir%
mkdir %dir%
xcopy "./kb_go/*.*" %dir% /s /h /d /y /e /f /i
RD /S /Q "./kb_go/"


REM install uniprot.XML database mysql ORM adapter
SET dir="../../src/repository/DataMySql/UniprotSprot/MySQL"
reflector --reflects /sql ./kb_UniProtKB.sql /namespace "kb_UniProtKB" /split /auto_increment.disable
RD /S /Q %dir%
mkdir %dir%
xcopy "./kb_UniProtKB/*.*" %dir% /s /h /d /y /e /f /i
RD /S /Q "./kb_UniProtKB/"


REM generates the mysql development sdk documents
reflector /MySQL.Markdown /sql ./kb_UniProtKB.sql
reflector /MySQL.Markdown /sql ./kb_go.sql