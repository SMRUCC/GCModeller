REM converts the mysql dataabase schema source code to visualbasic source code
"Reflector" --reflects -o "./KEGG_Schema/" /namespace "mysql" /split --language=visualbasic /sql "./jp_kegg2.sql"
"Reflector" --reflects -o "./ExplorEnz/" /namespace "ExplorEnz.MySQL" /split --language=visualbasic /sql "./ExplorEnz.sql"

REM copy ORM source code to project source code directory
RD /S /Q "../src/GCModeller/data/KEGG/LocalMySQL/"
xcopy "./KEGG_Schema/*.*" "../src/GCModeller/data/KEGG/LocalMySQL/" /s /h /d /y /e /f /i

