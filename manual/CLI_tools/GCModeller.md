---
title: GCModeller
tags: [maunal, tools]
date: 2016/10/19 16:38:32
---
# GCModeller [version 1.0.2.3]
> 

<!--more-->

**GCModeller administer cli console**
_GCModeller administer cli console_
Copyright ? 蓝思生物信息工程师工作站 2013

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/GCModeller.exe
**Root namespace**: ``xGCModeller.CLI``


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Draw.Comparative](#/Draw.Comparative)||
|[/Export.Basys](#/Export.Basys)||
|[/Merge.Table](#/Merge.Table)||
|[/Search.Fasta](#/Search.Fasta)||
|[/seqdiff](#/seqdiff)||
|[/Visual.BBH](#/Visual.BBH)||
|[--Drawing.ChromosomeMap](#--Drawing.ChromosomeMap)|Drawing the chromosomes map from the PTT object as the basically genome information source.|
|[--Drawing.ClustalW](#--Drawing.ClustalW)||
|[export](#export)|Export the calculation data from a specific data table in the mysql database server.|
|[--Gendist.From.Self.Overviews](#--Gendist.From.Self.Overviews)||
|[--Gendist.From.SelfMPAlignment](#--Gendist.From.SelfMPAlignment)||
|[--Get.Subset.lstID](#--Get.Subset.lstID)||
|[help](#help)|Show help information about this program.|
|[--install.MYSQL](#--install.MYSQL)||
|[--Interpro.Build](#--Interpro.Build)||
|[--user.create](#--user.create)||


##### 1. GCModeller Application Utilities


|Function API|Info|
|------------|----|
|[/init.manuals](#/init.manuals)||
|[/Located.AppData](#/Located.AppData)||
|[--ls](#--ls)|Listing all of the available GCModeller CLI tools commands.|


##### 2. GCModeller repository database tools


|Function API|Info|
|------------|----|
|[/Install.genbank](#/Install.genbank)||
|[/Name.match.hits](#/Name.match.hits)||
|[/nt.repository.query](#/nt.repository.query)||
|[/nt.scan](#/nt.scan)||
|[/title.uniques](#/title.uniques)||
|[--install.ncbi_nt](#--install.ncbi_nt)||
|[--install-CDD](#--install-CDD)||
|[--install-COGs](#--install-COGs)|Install the COGs database into the GCModeller database.|


##### 3. Localblast analysis tools


|Function API|Info|
|------------|----|
|[/Map.Hits](#/Map.Hits)||
|[/Map.Hits.Taxonomy](#/Map.Hits.Taxonomy)||




## CLI API list
--------------------------
<h3 id="/Draw.Comparative"> 1. /Draw.Comparative</h3>


**Prototype**: ``xGCModeller.CLI::Int32 DrawMultipleAlignments(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller /Draw.Comparative /in <meta.Xml> /PTT <PTT_DIR> [/out <outDIR>]
```
###### Example
```bash
GCModeller
```
<h3 id="/Export.Basys"> 2. /Export.Basys</h3>


**Prototype**: ``xGCModeller.CLI::Int32 ExportBaSys(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller /Export.Basys /in <in.DIR> [/out <out.DIR>]
```
###### Example
```bash
GCModeller
```
<h3 id="/init.manuals"> 3. /init.manuals</h3>


**Prototype**: ``xGCModeller.CLI::Int32 InitManuals(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller /init.manuals
```
###### Example
```bash
GCModeller
```
<h3 id="/Install.genbank"> 4. /Install.genbank</h3>


**Prototype**: ``xGCModeller.CLI::Int32 InstallGenbank(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller /Install.genbank /imports <all_genbanks.DIR> [/refresh]
```
###### Example
```bash
GCModeller
```
<h3 id="/Located.AppData"> 5. /Located.AppData</h3>


**Prototype**: ``xGCModeller.CLI::Int32 LocatedAppData(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller
```
###### Example
```bash
GCModeller
```
<h3 id="/Map.Hits"> 6. /Map.Hits</h3>


**Prototype**: ``xGCModeller.CLI::Int32 MapHits(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller /Map.Hits /in <query.csv> /mapping <blastnMapping.csv> [/out <out.csv>]
```
###### Example
```bash
GCModeller
```
<h3 id="/Map.Hits.Taxonomy"> 7. /Map.Hits.Taxonomy</h3>


**Prototype**: ``xGCModeller.CLI::Int32 MapHitsTaxonomy(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller /Map.Hits.Taxonomy /in <query.csv> /mapping <blastnMapping.csv> /tax <taxonomy.DIR:name/nodes>[/out <out.csv>]
```
###### Example
```bash
GCModeller
```



#### Parameters information:
##### [/mapping]
Data frame should have a ``taxid`` field.

###### Example
```bash

```
##### Accepted Types
###### /mapping
**Decalre**:  _SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BlastnMapping_
Example: 
```json
null
```

<h3 id="/Merge.Table"> 8. /Merge.Table</h3>


**Prototype**: ``xGCModeller.CLI::Int32 MergeTable(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller /Merge.Table /in <*.csv.DIR> [/out <EXPORT.csv>]
```
###### Example
```bash
GCModeller
```
<h3 id="/Name.match.hits"> 9. /Name.match.hits</h3>


**Prototype**: ``xGCModeller.CLI::Int32 MatchHits(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller /Name.match.hits /in <list.csv> /titles <*.txt/DIR> [/out <out.csv>]
```
###### Example
```bash
GCModeller
```
<h3 id="/nt.repository.query"> 10. /nt.repository.query</h3>


**Prototype**: ``xGCModeller.CLI::Int32 ntRepositoryExports(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller /nt.repository.query /query <arguments.csv> /DATA <DATA_dir> [/out <out_DIR>]
```
###### Example
```bash
GCModeller
```



#### Parameters information:
##### /query

###### Example
```bash

```
##### Accepted Types
###### /query
**Decalre**:  _Microsoft.VisualBasic.Data.IO.SearchEngine.QueryArgument_
Example: 
```json
{
    "Data": {
        
    },
    "Expression": "System.String",
    "Name": "System.String"
}
```

<h3 id="/nt.scan"> 11. /nt.scan</h3>


**Prototype**: ``xGCModeller.CLI::Int32 NtScaner(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller /nt.scan /query <expression.csv> /DATA <nt.DIR> [/break 60 /out <out_DIR>]
```
###### Example
```bash
GCModeller
```
<h3 id="/Search.Fasta"> 12. /Search.Fasta</h3>


**Prototype**: ``xGCModeller.CLI::Int32 SearchFasta(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller /Search.Fasta /in <fasta.fasta/DIR> /query <query_arguments.csv> [/out <out_DIR>]
```
###### Example
```bash
GCModeller
```



#### Parameters information:
##### /query

###### Example
```bash

```
##### Accepted Types
###### /query
**Decalre**:  _Microsoft.VisualBasic.Data.IO.SearchEngine.QueryArgument_
Example: 
```json
{
    "Data": {
        
    },
    "Expression": "System.String",
    "Name": "System.String"
}
```

<h3 id="/seqdiff"> 13. /seqdiff</h3>


**Prototype**: ``xGCModeller.CLI::Int32 SeqDiffCLI(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller /seqdiff /in <mla.fasta> [/toplog <file-list.txt> /winsize 250 /steps 50 /slides 5 /out <out.csv>]
```
###### Example
```bash
GCModeller
```



#### Parameters information:
##### [/toplog]
Put these directory path in the item order of:
+ hairpinks
+ perfects palindrome
+ repeats view
+ rev-repeats view

###### Example
```bash

```
##### Accepted Types
###### /toplog
**Decalre**:  _System.String[]_
Example: 
```json
[
    "System.String"
]
```

<h3 id="/title.uniques"> 14. /title.uniques</h3>


**Prototype**: ``xGCModeller.CLI::Int32 UniqueTitle(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller /title.uniques /in <*.txt/DIR> [/simple /tokens 3 /n -1 /out <out.csv>]
```
###### Example
```bash
GCModeller
```
<h3 id="/Visual.BBH"> 15. /Visual.BBH</h3>


**Prototype**: ``xGCModeller.CLI::Int32 BBHVisual(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller /Visual.BBH /in <bbh.Xml> /PTT <genome.PTT> /density <genomes.density.DIR> [/limits <sp-list.txt> /out <image.png>]
```
###### Example
```bash
GCModeller
```



#### Parameters information:
##### /PTT
A directory which contains all of the information data files for the reference genome,
this directory would includes *.gb, *.ptt, *.gff, *.fna, *.faa, etc.

###### Example
```bash

```
##### Accepted Types
###### /PTT
<h3 id="--Drawing.ChromosomeMap"> 16. --Drawing.ChromosomeMap</h3>

Drawing the chromosomes map from the PTT object as the basically genome information source.
**Prototype**: ``xGCModeller.CLI::Int32 DrawingChrMap(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller --Drawing.ChromosomeMap /ptt <genome.ptt> [/conf <config.inf> /out <dir.export> /COG <cog.csv>]
```
###### Example
```bash
GCModeller
```
<h3 id="--Drawing.ClustalW"> 17. --Drawing.ClustalW</h3>


**Prototype**: ``xGCModeller.CLI::Int32 DrawClustalW(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller --Drawing.ClustalW /in <align.fasta> [/out <out.png> /dot.Size 10]
```
###### Example
```bash
GCModeller
```
<h3 id="export"> 18. export</h3>

Export the calculation data from a specific data table in the mysql database server.
**Prototype**: ``xGCModeller.CLI::Int32 ExportData(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller export -mysql <mysql_connection_string> [-o <output_save_file/dir> -t <table_name>]
```
###### Example
```bash
GCModeller export -t all -o ~/Desktop/ -mysql "http://localhost:8080/client?user=username%password=password%database=database"
```



#### Parameters information:
##### -mysql
The mysql connection string for gc program connect to a specific mysql database server.

###### Example
```bash
http://localhost:8080/client?user=username%password=password%database=database
```
##### [-t]
Optional, The target table name for export the data set, there is a option value for this switch: all.
<name> - export the data in the specific name of the table;
all - Default, export all of the table in the database, and at the mean time the -o switch value will be stand for the output directory of the exported csv files.

###### Example
```bash
all
```
##### [-o]
Optional, The path of the export csv file save, it can be a directory or a file path, depend on the value of the -t switch value.
Default is desktop directory and table name combination

###### Example
```bash
~/Desktop/
```
##### Accepted Types
###### -mysql
###### -t
###### -o
<h3 id="--Gendist.From.Self.Overviews"> 19. --Gendist.From.Self.Overviews</h3>


**Prototype**: ``xGCModeller.CLI::Int32 SelfOverviewAsMAT(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller --Gendist.From.Self.Overviews /blast_out <blast_out.txt>
```
###### Example
```bash
GCModeller
```
<h3 id="--Gendist.From.SelfMPAlignment"> 20. --Gendist.From.SelfMPAlignment</h3>


**Prototype**: ``xGCModeller.CLI::Int32 SelfMPAlignmentAsMAT(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller --Gendist.From.SelfMPAlignment /aln <mpalignment.csv>
```
###### Example
```bash
GCModeller
```
<h3 id="--Get.Subset.lstID"> 21. --Get.Subset.lstID</h3>


**Prototype**: ``xGCModeller.CLI::Int32 GetSubsetID(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller --Get.Subset.lstID /subset <lstID.txt> /lstID <lstID.csv>
```
###### Example
```bash
GCModeller
```
<h3 id="help"> 22. help</h3>

Show help information about this program.
**Prototype**: ``xGCModeller.CLI::Int32 About()``

###### Usage
```bash
GCModeller gc help
```
###### Example
```bash
GCModeller gc help
```
<h3 id="--install.MYSQL"> 23. --install.MYSQL</h3>


**Prototype**: ``xGCModeller.CLI::Int32 InstallMySQL(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller --install.MYSQL /user <userName> /pass <password> /repository <host_ipAddress> [/port 3306 /database <GCModeller>]
```
###### Example
```bash
GCModeller
```
<h3 id="--install.ncbi_nt"> 24. --install.ncbi_nt</h3>


**Prototype**: ``xGCModeller.CLI::Int32 Install_NCBI_nt(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller --install.ncbi_nt /nt <nt.fasta/DIR> [/EXPORT <DATA_dir>]
```
###### Example
```bash
GCModeller
```
<h3 id="--install-CDD"> 25. --install-CDD</h3>


**Prototype**: ``xGCModeller.CLI::Int32 InstallCDD(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller --install-CDD /cdd <cdd.DIR>
```
###### Example
```bash
GCModeller
```
<h3 id="--install-COGs"> 26. --install-COGs</h3>

Install the COGs database into the GCModeller database.
**Prototype**: ``xGCModeller.CLI::Int32 InstallCOGs(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller --install-COGs /COGs <Dir.COGs>
```
###### Example
```bash
GCModeller
```
<h3 id="--Interpro.Build"> 27. --Interpro.Build</h3>


**Prototype**: ``xGCModeller.CLI::Int32 BuildFamilies(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller --Interpro.Build /xml <interpro.xml>
```
###### Example
```bash
GCModeller
```
<h3 id="--ls"> 28. --ls</h3>

Listing all of the available GCModeller CLI tools commands.
**Prototype**: ``xGCModeller.CLI::Int32 List(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller
```
###### Example
```bash
GCModeller
```
<h3 id="--user.create"> 29. --user.create</h3>


**Prototype**: ``xGCModeller.CLI::Int32 Register(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
GCModeller
```
###### Example
```bash
GCModeller
```
