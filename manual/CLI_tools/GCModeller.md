---
title: GCModeller
tags: [maunal, tools]
date: 5/28/2018 9:30:22 PM
---
# GCModeller [version 1.0.2.3]
> 

<!--more-->

**GCModeller administer cli console**<br/>
_GCModeller administer cli console_<br/>
Copyright © 蓝思生物信息工程师工作站 2013

**Module AssemblyName**: GCModeller<br/>
**Root namespace**: ``xGCModeller.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Data.Copy](#/Data.Copy)||
|[/download.chebi](#/download.chebi)||
|[/Export.Basys](#/Export.Basys)||
|[/Intersect](#/Intersect)||
|[/kmeans](#/kmeans)||
|[/plot.png.corp_blank](#/plot.png.corp_blank)||
|[/Scan.templates](#/Scan.templates)||
|[/Search.Fasta](#/Search.Fasta)||
|[/seqdiff](#/seqdiff)||
|[/Strip.Null.Columns](#/Strip.Null.Columns)||
|[export](#export)|Export the calculation data from a specific data table in the mysql database server.|
|[--Gendist.From.Self.Overviews](#--Gendist.From.Self.Overviews)||
|[--Gendist.From.SelfMPAlignment](#--Gendist.From.SelfMPAlignment)||
|[--Get.Subset.lstID](#--Get.Subset.lstID)||
|[help](#help)|Show help information about this program.|
|[--install.MYSQL](#--install.MYSQL)||
|[--Interpro.Build](#--Interpro.Build)||
|[--user.create](#--user.create)||


##### 1. Biological Data Visualization Tools


|Function API|Info|
|------------|----|
|[/Draw.Comparative](#/Draw.Comparative)||
|[/Plot.GC](#/Plot.GC)||
|[--Drawing.ClustalW](#--Drawing.ClustalW)||


##### 2. GCModeller Application Utilities


|Function API|Info|
|------------|----|
|[/init.manuals](#/init.manuals)||
|[/Located.AppData](#/Located.AppData)||
|[/Merge.Files](#/Merge.Files)|Tools that works on the text files merge operation. This tool is usually used for merge of the fasta files into a larger fasta file.|
|[--ls](#--ls)|Listing all of the available GCModeller CLI tools commands.|


##### 3. GCModeller repository database tools


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


##### 4. Localblast analysis tools


|Function API|Info|
|------------|----|
|[/Map.Hits](#/Map.Hits)||
|[/Map.Hits.Taxonomy](#/Map.Hits.Taxonomy)||

## CLI API list
--------------------------
<h3 id="/Data.Copy"> 1. /Data.Copy</h3>



**Prototype**: ``xGCModeller.CLI::Int32 CopyFiles(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /Data.Copy /imports <DIR> [/ext <*.*> /copy2 <CopyTo> /overrides]
```
<h3 id="/download.chebi"> 2. /download.chebi</h3>



**Prototype**: ``xGCModeller.CLI::Int32 DownloadChebi(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /download.chebi [/max 80000 /save <directory>]
```
<h3 id="/Draw.Comparative"> 3. /Draw.Comparative</h3>



**Prototype**: ``xGCModeller.CLI::Int32 DrawMultipleAlignments(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /Draw.Comparative /in <meta.Xml> /PTT <PTT_DIR> [/out <outDIR>]
```
<h3 id="/Export.Basys"> 4. /Export.Basys</h3>



**Prototype**: ``xGCModeller.CLI::Int32 ExportBaSys(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /Export.Basys /in <in.DIR> [/out <out.DIR>]
```
<h3 id="/init.manuals"> 5. /init.manuals</h3>



**Prototype**: ``xGCModeller.CLI::Int32 InitManuals(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /init.manuals [/out <directory, default=./>]
```
<h3 id="/Install.genbank"> 6. /Install.genbank</h3>



**Prototype**: ``xGCModeller.CLI::Int32 InstallGenbank(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /Install.genbank /imports <all_genbanks.DIR> [/refresh]
```
<h3 id="/Intersect"> 7. /Intersect</h3>



**Prototype**: ``xGCModeller.CLI::Int32 Intersect(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /Intersect /a <list.txt> /b <list.txt> [/out <list.txt>]
```
<h3 id="/kmeans"> 8. /kmeans</h3>



**Prototype**: ``xGCModeller.CLI::Int32 Kmeans(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /kmeans /in <matrix.csv> [/cluster.n <default=6> /out <out.csv>]
```
<h3 id="/Located.AppData"> 9. /Located.AppData</h3>



**Prototype**: ``xGCModeller.CLI::Int32 LocatedAppData(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller
```
<h3 id="/Map.Hits"> 10. /Map.Hits</h3>



**Prototype**: ``xGCModeller.CLI::Int32 MapHits(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /Map.Hits /in <query.csv> /mapping <blastnMapping.csv> [/split.Samples /sample.Name <filedName,default:=track> /out <out.csv>]
```
<h3 id="/Map.Hits.Taxonomy"> 11. /Map.Hits.Taxonomy</h3>



**Prototype**: ``xGCModeller.CLI::Int32 MapHitsTaxonomy(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /Map.Hits.Taxonomy /in <query.csv> /mapping <blastnMapping.csv/DIR> /tax <taxonomy.DIR:name/nodes> [/out <out.csv>]
```


#### Arguments
##### [/mapping]
Data frame should have a ``taxid`` field.

###### Example
```bash
/mapping <term_string>
```
##### Accepted Types
###### /mapping
**Decalre**:  _SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BlastnMapping_

Example: 
```json
null
```

<h3 id="/Merge.Files"> 12. /Merge.Files</h3>

Tools that works on the text files merge operation. This tool is usually used for merge of the fasta files into a larger fasta file.

**Prototype**: ``xGCModeller.CLI::Int32 FileMerges(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /Merge.Files /in <in.DIR> [/trim /ext <*.txt> /encoding <ascii> /out <out.txt>]
```


#### Arguments
##### [/encoding]
Specific the output text file encoding value, default is ASCII encoding. Fasta file merge must be ASCII encoding output

###### Example
```bash
/encoding <term_string>
```
##### Accepted Types
###### /encoding
**Decalre**:  _Microsoft.VisualBasic.Text.Encodings_

Example: 
```json
0
```

<h3 id="/Name.match.hits"> 13. /Name.match.hits</h3>



**Prototype**: ``xGCModeller.CLI::Int32 MatchHits(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /Name.match.hits /in <list.csv> /titles <*.txt/DIR> [/out <out.csv>]
```
<h3 id="/nt.repository.query"> 14. /nt.repository.query</h3>



**Prototype**: ``xGCModeller.CLI::Int32 ntRepositoryExports(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /nt.repository.query /query <arguments.csv> /DATA <DATA_dir> [/out <out_DIR>]
```


#### Arguments
##### /query

###### Example
```bash
/query <term_string>
```
##### Accepted Types
###### /query
**Decalre**:  _Microsoft.VisualBasic.Data.IO.SearchEngine.QueryArgument_

Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Expression": "System.String",
    "Name": "System.String"
}
```

<h3 id="/nt.scan"> 15. /nt.scan</h3>



**Prototype**: ``xGCModeller.CLI::Int32 NtScaner(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /nt.scan /query <expression.csv> /DATA <nt.DIR> [/break 60 /out <out_DIR>]
```
<h3 id="/Plot.GC"> 16. /Plot.GC</h3>



**Prototype**: ``xGCModeller.CLI::Int32 PlotGC(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /Plot.GC /in <mal.fasta> [/plot <gcskew/gccontent> /colors <Jet> /out <out.png>]
```
<h3 id="/plot.png.corp_blank"> 17. /plot.png.corp_blank</h3>



**Prototype**: ``xGCModeller.CLI::Int32 PlotStripBlank(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /plot.png.corp_blank /in <plot.png> [/margin <30px> /out <plot.png>]
```
<h3 id="/Scan.templates"> 18. /Scan.templates</h3>



**Prototype**: ``xGCModeller.CLI::Int32 ScanTableTemplates(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /Scan.templates
```
<h3 id="/Search.Fasta"> 19. /Search.Fasta</h3>



**Prototype**: ``xGCModeller.CLI::Int32 SearchFasta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /Search.Fasta /in <fasta.fasta/DIR> /query <query_arguments.csv> [/out <out_DIR>]
```


#### Arguments
##### /query

###### Example
```bash
/query <term_string>
```
##### Accepted Types
###### /query
**Decalre**:  _Microsoft.VisualBasic.Data.IO.SearchEngine.QueryArgument_

Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Expression": "System.String",
    "Name": "System.String"
}
```

<h3 id="/seqdiff"> 20. /seqdiff</h3>



**Prototype**: ``xGCModeller.CLI::Int32 SeqDiffCLI(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /seqdiff /in <mla.fasta> [/toplog <file-list.txt> /winsize 250 /steps 50 /slides 5 /out <out.csv>]
```


#### Arguments
##### [/toplog]
Put these directory path in the item order of:
+ hairpinks
+ perfects palindrome
+ repeats view
+ rev-repeats view

###### Example
```bash
/toplog <term_string>
```
##### Accepted Types
###### /toplog
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.string[](v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.String[]</a>_

Example: 
```json
[
    "System.String"
]
```

<h3 id="/Strip.Null.Columns"> 21. /Strip.Null.Columns</h3>



**Prototype**: ``xGCModeller.CLI::Int32 StripNullColumns(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /Strip.Null.Columns /in <table.csv> [/out <out.csv>]
```
<h3 id="/title.uniques"> 22. /title.uniques</h3>



**Prototype**: ``xGCModeller.CLI::Int32 UniqueTitle(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller /title.uniques /in <*.txt/DIR> [/simple /tokens 3 /n -1 /out <out.csv>]
```
<h3 id="--Drawing.ClustalW"> 23. --Drawing.ClustalW</h3>



**Prototype**: ``xGCModeller.CLI::Int32 DrawClustalW(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller --Drawing.ClustalW /in <align.fasta> [/out <out.png> /dot.Size 10]
```
<h3 id="export"> 24. export</h3>

Export the calculation data from a specific data table in the mysql database server.

**Prototype**: ``xGCModeller.CLI::Int32 ExportData(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller export -mysql <mysql_connection_string> [-o <output_save_file/dir> -t <table_name>]
```
###### Example
```bash
GCModeller export -t all -o ~/Desktop/ -mysql "http://localhost:8080/client?user=username%password=password%database=database"
```


#### Arguments
##### -mysql
The mysql connection string for gc program connect to a specific mysql database server.

###### Example
```bash
-mysql <term_string>
```
##### [-t]
Optional, The target table name for export the data set, there is a option value for this switch: all.
<name> - export the data in the specific name of the table;
all - Default, export all of the table in the database, and at the mean time the -o switch value will be stand for the output directory of the exported csv files.

###### Example
```bash
-t <term_string>
```
##### [-o]
Optional, The path of the export csv file save, it can be a directory or a file path, depend on the value of the -t switch value.
Default is desktop directory and table name combination

###### Example
```bash
-o <term_string>
```
<h3 id="--Gendist.From.Self.Overviews"> 25. --Gendist.From.Self.Overviews</h3>



**Prototype**: ``xGCModeller.CLI::Int32 SelfOverviewAsMAT(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller --Gendist.From.Self.Overviews /blast_out <blast_out.txt>
```
<h3 id="--Gendist.From.SelfMPAlignment"> 26. --Gendist.From.SelfMPAlignment</h3>



**Prototype**: ``xGCModeller.CLI::Int32 SelfMPAlignmentAsMAT(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller --Gendist.From.SelfMPAlignment /aln <mpalignment.csv>
```
<h3 id="--Get.Subset.lstID"> 27. --Get.Subset.lstID</h3>



**Prototype**: ``xGCModeller.CLI::Int32 GetSubsetID(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller --Get.Subset.lstID /subset <lstID.txt> /lstID <lstID.csv>
```
<h3 id="help"> 28. help</h3>

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
<h3 id="--install.MYSQL"> 29. --install.MYSQL</h3>



**Prototype**: ``xGCModeller.CLI::Int32 InstallMySQL(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller --install.MYSQL /user <userName> /pass <password> /repository <host_ipAddress> [/port 3306 /database <GCModeller>]
```
<h3 id="--install.ncbi_nt"> 30. --install.ncbi_nt</h3>



**Prototype**: ``xGCModeller.CLI::Int32 Install_NCBI_nt(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller --install.ncbi_nt /nt <nt.fasta/DIR> [/EXPORT <DATA_dir>]
```
<h3 id="--install-CDD"> 31. --install-CDD</h3>



**Prototype**: ``xGCModeller.CLI::Int32 InstallCDD(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller --install-CDD /cdd <cdd.DIR>
```
<h3 id="--install-COGs"> 32. --install-COGs</h3>

Install the COGs database into the GCModeller database.

**Prototype**: ``xGCModeller.CLI::Int32 InstallCOGs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller --install-COGs /COGs <Dir.COGs>
```
<h3 id="--Interpro.Build"> 33. --Interpro.Build</h3>



**Prototype**: ``xGCModeller.CLI::Int32 BuildFamilies(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller --Interpro.Build /xml <interpro.xml>
```
<h3 id="--ls"> 34. --ls</h3>

Listing all of the available GCModeller CLI tools commands.

**Prototype**: ``xGCModeller.CLI::Int32 List(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller
```
<h3 id="--user.create"> 35. --user.create</h3>



**Prototype**: ``xGCModeller.CLI::Int32 Register(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
GCModeller
```
