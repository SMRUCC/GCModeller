---
title: KEGG_tools
tags: [maunal, tools]
date: 2016/10/22 12:30:12
---
# GCModeller [version 3.0.854.0]
> KEGG web services API tools.

<!--more-->

**tools utilis for KEGG database DBGET API**
_tools utilis for KEGG database DBGET API_
Copyright ? xie.guigang@gmail.com 2014

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/KEGG_tools.exe
**Root namespace**: ``KEGG_tools.CLI``


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/16S_rRNA](#/16S_rRNA)||
|[/blastn](#/blastn)|Blastn analysis of your DNA sequence on KEGG server for the functional analysis.|
|[/Compile.Model](#/Compile.Model)|KEGG pathway model compiler|
|[/Download.Ortholog](#/Download.Ortholog)|Downloads the KEGG gene ortholog annotation data from the web server.|
|[/Download.Pathway.Maps](#/Download.Pathway.Maps)||
|[/Dump.sp](#/Dump.sp)||
|[/Fasta.By.Sp](#/Fasta.By.Sp)||
|[/Get.prot_motif](#/Get.prot_motif)||
|[/Gets.prot_motif](#/Gets.prot_motif)||
|[/Imports.SSDB](#/Imports.SSDB)||
|[/Pathways.Downloads.All](#/Pathways.Downloads.All)||
|[/Pull.Seq](#/Pull.Seq)|Downloads the missing sequence in the local KEGG database from the KEGG database server.|
|[/Query.KO](#/Query.KO)||
|[/Views.mod_stat](#/Views.mod_stat)||
|[-Build.KO](#-Build.KO)|Download data from KEGG database to local server.|
|[Download.Sequence](#Download.Sequence)||
|[--Dump.Db](#--Dump.Db)||
|[--Export.KO](#--Export.KO)||
|[-function.association.analysis](#-function.association.analysis)||
|[--Get.KO](#--Get.KO)||
|[--part.from](#--part.from)|source and ref should be in KEGG annotation format.|
|[-query](#-query)|Query the KEGG database for nucleotide sequence and protein sequence by using a keywork.|
|[-query.orthology](#-query.orthology)||
|[-query.ref.map](#-query.ref.map)||
|[-ref.map.download](#-ref.map.download)||
|[-Table.Create](#-Table.Create)||

## CLI API list
--------------------------
<h3 id="/16S_rRNA"> 1. /16S_rRNA</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 Download16SRNA(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /16s_rna [/out <outDIR>]
```
<h3 id="/blastn"> 2. /blastn</h3>

Blastn analysis of your DNA sequence on KEGG server for the functional analysis.
**Prototype**: ``KEGG_tools.CLI::Int32 Blastn(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /blastn /query <query.fasta> [/out <outDIR>]
```
<h3 id="/Compile.Model"> 3. /Compile.Model</h3>

KEGG pathway model compiler
**Prototype**: ``KEGG_tools.CLI::Int32 Compile(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Compile.Model /pathway <pathwayDIR> /mods <modulesDIR> /sp <sp_code> [/out <out.Xml>]
```
<h3 id="/Download.Ortholog"> 4. /Download.Ortholog</h3>

Downloads the KEGG gene ortholog annotation data from the web server.
**Prototype**: ``KEGG_tools.CLI::Int32 DownloadOrthologs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Download.Ortholog -i <gene_list_file.txt/gbk> -export <exportedDIR> [/gbk /sp <KEGG.sp>]
```
<h3 id="/Download.Pathway.Maps"> 5. /Download.Pathway.Maps</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DownloadPathwayMaps(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Download.Pathway.Maps /sp <kegg.sp_code> [/out <EXPORT_DIR>]
```
<h3 id="/Dump.sp"> 6. /Dump.sp</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DumpOrganisms(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Dump.sp [/res sp.html /out <out.csv>]
```
<h3 id="/Fasta.By.Sp"> 7. /Fasta.By.Sp</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 GetFastaBySp(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Fasta.By.Sp /in <KEGG.fasta> /sp <sp.list> [/out <out.fasta>]
```
<h3 id="/Get.prot_motif"> 8. /Get.prot_motif</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 ProteinMotifs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Get.prot_motif /query <sp:locus> [/out out.json]
```
<h3 id="/Gets.prot_motif"> 9. /Gets.prot_motif</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 GetsProteinMotifs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Gets.prot_motif /query <query.txt/genome.PTT> [/PTT /sp <kegg-sp> /out <out.json> /update]
```
<h3 id="/Imports.SSDB"> 10. /Imports.SSDB</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 ImportsDb(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Imports.SSDB /in <source.DIR> [/out <ssdb.csv>]
```
<h3 id="/Pathways.Downloads.All"> 11. /Pathways.Downloads.All</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DownloadsAllPathways(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Pathways.Downloads.All [/out <outDIR>]
```
<h3 id="/Pull.Seq"> 12. /Pull.Seq</h3>

Downloads the missing sequence in the local KEGG database from the KEGG database server.
**Prototype**: ``KEGG_tools.CLI::Int32 PullSequence(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools
```
<h3 id="/Query.KO"> 13. /Query.KO</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 QueryKOAnno(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Query.KO /in <blastnhits.csv> [/out <out.csv> /evalue 1e-5 /batch]
```
<h3 id="/Views.mod_stat"> 14. /Views.mod_stat</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 Stats(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Views.mod_stat /in <KEGG_Modules/Pathways_DIR> /locus <in.csv> [/locus_map Gene /pathway /out <out.csv>]
```
<h3 id="-Build.KO"> 15. -Build.KO</h3>

Download data from KEGG database to local server.
**Prototype**: ``KEGG_tools.CLI::Int32 BuildKEGGOrthology(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools -Build.KO [/fill-missing]
```
<h3 id="Download.Sequence"> 16. Download.Sequence</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DownloadSequence(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools Download.Sequence /query <querySource.txt> [/out <outDIR> /source <existsDIR>]
```
<h3 id="--Dump.Db"> 17. --Dump.Db</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DumpDb(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools --Dump.Db /KEGG.Pathways <DIR> /KEGG.Modules <DIR> /KEGG.Reactions <DIR> /sp <sp.Code> /out <out.Xml>
```
<h3 id="--Export.KO"> 18. --Export.KO</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 ExportKO(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools
```
<h3 id="-function.association.analysis"> 19. -function.association.analysis</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 FunctionAnalysis(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools -function.association.analysis -i <matrix_csv>
```
<h3 id="--Get.KO"> 20. --Get.KO</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 GetKOAnnotation(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools --Get.KO /in <KASS-query.txt>
```
<h3 id="--part.from"> 21. --part.from</h3>

source and ref should be in KEGG annotation format.
**Prototype**: ``KEGG_tools.CLI::Int32 GetSource(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools --part.from /source <source.fasta> /ref <referenceFrom.fasta> [/out <out.fasta> /brief]
```
<h3 id="-query"> 22. -query</h3>

Query the KEGG database for nucleotide sequence and protein sequence by using a keywork.
**Prototype**: ``KEGG_tools.CLI::Int32 QueryGenes(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools -query -keyword <keyword> -o <out_dir>
```
<h3 id="-query.orthology"> 23. -query.orthology</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 QueryOrthology(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools -query.orthology -keyword <gene_name> -o <output_csv>
```
<h3 id="-query.ref.map"> 24. -query.ref.map</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DownloadReferenceMap(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools -query.ref.map -id <id> -o <out_dir>
```
<h3 id="-ref.map.download"> 25. -ref.map.download</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DownloadReferenceMapDatabase(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools -ref.map.download -o <out_dir>
```
<h3 id="-Table.Create"> 26. -Table.Create</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 CreateTABLE(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools -table.create -i <input_dir> -o <out_csv>
```


#### Arguments
##### -i
This parameter specific the source directory input of the download data.

###### Example
```bash
-i <term_string>
```
