REM converts the mysql dataabase schema source code to visualbasic source code

"Reflector" --reflects -o "../src/repository/DataMySql/UniprotSprot/MySQL" /namespace "UniprotKB.MySQL.Tables" /split --language=visualbasic /sql "../src/repository/DataMySql/UniprotSprot/uniprotKB.sql"
REM "Reflector" --reflects -o "../src/repository/DataMySql/Xfam/Rfam/Tables" /namespace "Xfam.Rfam.MySQL.Tables" /split --language=visualbasic /sql "./xfam/rfam_12_2.sql"

REM "Reflector" --reflects -o "../src/repository/Model_Repository/MySQL/" /namespace "MySQL.Tables" /split --language=visualbasic /sql "../src/repository/Model_Repository/MySQL/correlations.sql"

REM "Reflector" --reflects -o "../src/repository/nt/mysql/NCBI" /namespace "mysql.NCBI" /split --language=visualbasic /sql "../src/repository/ncbi.sql"
REM "Reflector" --reflects -o "../src/GCModeller/data/RegulonDatabase/RegulonDB/MySQL/" /namespace "RegulonDB.Tables" /split --language=visualbasic /sql "./regulonDB_93.sql"
REM "Reflector" --reflects -o "./KEGG_Schema/" /namespace "LocalMySQL" /split --language=visualbasic /sql "./jp_kegg2.sql"
REM "Reflector" --reflects -o "./ExplorEnz/" /namespace "ExplorEnz.MySQL" /split --language=visualbasic /sql "./ExplorEnz.sql"
REM "Reflector" --reflects -o "../src/repository/DataMySql/Interpro/Tables/" /namespace "Interpro.Tables" /split --language=visualbasic /sql "../src/repository/DataMySql/Interpro/interpro.sql"
REM "Reflector" --reflects -o "../src/GCModeller/data/RegulonDatabase/Regtransbase/MySQL" /namespace "Regtransbase.MySQL" /split --language=visualbasic /sql "./dbregulation_update.sql"
REM "Reflector" --reflects -o "../src/repository/DataMySql/CEG/MySQL" /namespace "CEG.MySQL" /split --language=visualbasic /sql "../src/repository/DataMySql/CEG/CEG.sql"
REM "Reflector" --reflects -o "./uniprot/" /namespace "UniprotKB.MySQL.Tables" /split --language=visualbasic /sql "./uniprot.sql"
REM "Reflector" --reflects -o "../src/GCModeller/data/ExternalDBSource/ChEBI/Tables/" /namespace "ChEBI.Tables" /split --language=visualbasic /sql "../src/GCModeller/data/ExternalDBSource/ChEBI/chebi.sql"
REM "Reflector" --reflects -o "../src/GCModeller/data/ExternalDBSource/MetaCyc/MySQL/" /namespace "MetaCyc.MySQL" /split --language=visualbasic /sql "../src/GCModeller/data/ExternalDBSource/MetaCyc/bio_warehouse.sql"
REM "Reflector" --reflects -o "../src/GCModeller/data/GO_gene-ontology/GO_mysql/MySQL/Tables" /namespace "MySQL.Tables" /split --language=visualbasic /sql "../src/GCModeller/data/GO_gene-ontology/GO_mysql/MySQL/go.sql"

REM "Reflector" --reflects -o "../src/GCModeller/data/Reactome/LocalMySQL/gk_current" /namespace "LocalMySQL.Tables.gk_current" /split --language=visualbasic /sql "./reactome/gk_current.sql"
REM "Reflector" --reflects -o "../src/GCModeller/data/Reactome/LocalMySQL/gk_current_dn" /namespace "LocalMySQL.Tables.gk_current_dn" /split --language=visualbasic /sql "./reactome/gk_current_dn.sql"
REM "Reflector" --reflects -o "../src/GCModeller/data/Reactome/LocalMySQL/gk_stable_ids" /namespace "LocalMySQL.Tables.gk_stable_ids" /split --language=visualbasic /sql "./reactome/gk_stable_ids.sql"

REM copy ORM source code to project source code directory

REM RD /S /Q "../src/GCModeller/data/KEGG/LocalMySQL/"
REM RD /S /Q "../src/GCModeller/data/ExternalDBSource/ExplorEnz/MySQL"

xcopy "./KEGG_Schema/*.*" "../src/GCModeller/data/KEGG/LocalMySQL/" /s /h /d /y /e /f /i
xcopy "./ExplorEnz/*.*" "../src/GCModeller/data/ExternalDBSource/ExplorEnz/MySQL/" /s /h /d /y /e /f /i
