REM converts the mysql dataabase schema source code to visualbasic source code
"Reflector" --reflects -o "./KEGG_Schema/" /namespace "mysql" /split --language=visualbasic /sql "./jp_kegg2.sql"
"Reflector" --reflects -o "./ExplorEnz/" /namespace "ExplorEnz.MySQL" /split --language=visualbasic /sql "./ExplorEnz.sql"
"Reflector" --reflects -o "./interpro/" /namespace "Interpro.Tables" /split --language=visualbasic /sql "./interpro.sql"
"Reflector" --reflects -o "./uniprot/" /namespace "UniprotKB.MySQL.Tables" /split --language=visualbasic /sql "./uniprot.sql"
"Reflector" --reflects -o "../src/GCModeller/data/ExternalDBSource/ChEBI/Tables/" /namespace "ChEBI.Tables" /split --language=visualbasic /sql "../src/GCModeller/data/ExternalDBSource/ChEBI/chebi.sql"
"Reflector" --reflects -o "../src/GCModeller/data/ExternalDBSource/MetaCyc/MySQL/" /namespace "MetaCyc.MySQL" /split --language=visualbasic /sql "../src/GCModeller/data/ExternalDBSource/MetaCyc/bio_warehouse.sql"
"Reflector" --reflects -o "../src/GCModeller/data/GO_gene-ontology/GO_mysql/MySQL/Tables" /namespace "MySQL.Tables" /split --language=visualbasic /sql "../src/GCModeller/data/GO_gene-ontology/GO_mysql/MySQL/go.sql"

REM copy ORM source code to project source code directory
RD /S /Q "../src/GCModeller/data/KEGG/LocalMySQL/"
RD /S /Q "../src/GCModeller/data/ExternalDBSource/ExplorEnz/MySQL"

xcopy "./KEGG_Schema/*.*" "../src/GCModeller/data/KEGG/LocalMySQL/" /s /h /d /y /e /f /i
xcopy "./ExplorEnz/*.*" "../src/GCModeller/data/ExternalDBSource/ExplorEnz/MySQL/" /s /h /d /y /e /f /i
