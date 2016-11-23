---
title: localblast
tags: [maunal, tools]
date: 11/24/2016 2:54:10 AM
---
# GCModeller [version 1.0.0.0]
> Wrapper tools for the ncbi blast+ program and the blast output data analysis program.

<!--more-->

**NCBI localblast wrapper tools**<br/>
_NCBI localblast_<br/>
Copyright © GPL3 2015

**Module AssemblyName**: file:///G:/GCModeller/GCModeller/bin/localblast.exe<br/>
**Root namespace**: ``NCBI.localblast.CLI``<br/>

------------------------------------------------------------
If you are having trouble debugging this Error, first read the best practices tutorial for helpful tips that address many common problems:
> http://docs.gcmodeller.org


The debugging facility Is helpful To figure out what's happening under the hood:
> https://github.com/SMRUCC/GCModeller/wiki


If you're still stumped, you can try get help from author directly from E-mail:
> xie.guigang@gcmodeller.org



All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Bash.Venn](#/Bash.Venn)||
|[/BestHits.Filtering](#/BestHits.Filtering)||
|[/Copys](#/Copys)||
|[/export.prot](#/export.prot)||
|[/Fasta.Filters](#/Fasta.Filters)||
|[/Identities.Matrix](#/Identities.Matrix)||
|[/MAT.evalue](#/MAT.evalue)||
|[/Paralog](#/Paralog)||
|[/Reads.OTU.Taxonomy](#/Reads.OTU.Taxonomy)||
|[/SSBH2BH_LDM](#/SSBH2BH_LDM)||
|[/SSDB.Export](#/SSDB.Export)||
|[/Taxonomy.efetch](#/Taxonomy.efetch)||
|[/Taxonomy.efetch.Merge](#/Taxonomy.efetch.Merge)||
|[/Venn.Single](#/Venn.Single)||
|[--bbh.export](#--bbh.export)|Batch export bbh result data from a directory.|
|[blast](#blast)|In order to draw as venn diagram for a specific set of genome and study the diferrence and consists between these genomes, you should do the blast operation from the protein amino aciad sequence first. The blastp operation can be performenced by the blast+ program which you can download from the NCBI website, this command is a interop service for the NCBI blast program, you should install the blast+ program at first.|
|[--blast.self](#--blast.self)|Query fasta query against itself for paralogs.|
|[-copy](#-copy)||
|[--Export.Fasta](#--Export.Fasta)||
|[--Export.Overviews](#--Export.Overviews)||
|[--Export.SBH](#--Export.SBH)||
|[-export_besthit](#-export_besthit)||
|[grep](#grep)|The gene id in the blast output log file are not well format for reading and program processing, so before you generate the venn diagram you should call this command to parse the gene id from the log file. You can also done this id parsing job using other tools.|
|[logs_analysis](#logs_analysis)|Parsing the xml format blast log into a csv data file that use for venn diagram drawing.|
|[merge](#merge)|This program can not use the blast parsing result for the venn diagram drawing operation, and this command is using for generate the drawing data for the venn diagram drawing command, this command merge the blast log parsing result and then using the parsing result for drawing a venn diagram.|
|[-merge_besthit](#-merge_besthit)||
|[--Xml2Excel](#--Xml2Excel)||
|[--Xml2Excel.Batch](#--Xml2Excel.Batch)||


##### 1. NCBI genbank tools


|Function API|Info|
|------------|----|
|[/add.locus_tag](#/add.locus_tag)|Add locus_tag qualifier into the feature slot.|
|[/add.names](#/add.names)||
|[/Copy.PTT](#/Copy.PTT)||
|[/Export.BlastX](#/Export.BlastX)||
|[/Export.gb](#/Export.gb)|Export the *.fna, *.faa, *.ptt file from the gbk file.|
|[/Export.gpff](#/Export.gpff)||
|[/Export.gpffs](#/Export.gpffs)||
|[/Merge.faa](#/Merge.faa)||
|[/Print](#/Print)||


##### 2. NCBI Web Blast Tools


|Function API|Info|
|------------|----|
|[/AlignmentTable.TopBest](#/AlignmentTable.TopBest)||
|[/Export.AlignmentTable](#/Export.AlignmentTable)||
|[/Export.AlignmentTable.giList](#/Export.AlignmentTable.giList)||


##### 3. Blastp BBH tools


|Function API|Info|
|------------|----|
|[/bbh.Export](#/bbh.Export)||
|[/BBH.Merge](#/BBH.Merge)||
|[/Blastp.BBH.Query](#/Blastp.BBH.Query)|Using query fasta invoke blastp against the fasta files in a directory.
               * This command tools required of NCBI blast+ suite, you must config the blast bin path by using ``settings.exe`` before running this command.|
|[/Export.Locus](#/Export.Locus)||
|[/locus.Selects](#/locus.Selects)||
|[/SBH.BBH.Batch](#/SBH.BBH.Batch)||
|[/SBH.Export.Large](#/SBH.Export.Large)|Using this command for export the sbh result of your blastp raw data.|
|[/SBH.Trim](#/SBH.Trim)||
|[/sbh2bbh](#/sbh2bbh)||
|[/Select.Meta](#/Select.Meta)||
|[/venn.BBH](#/venn.BBH)|2. Build venn table And bbh data from the blastp result out Or sbh data cache.|
|[/venn.BlastAll](#/venn.BlastAll)|Completely paired combos blastp bbh operations for the venn diagram Or network builder.|
|[/venn.cache](#/venn.cache)|1. [SBH_Batch] Creates the sbh cache data for the downstream bbh analysis. 
               And this batch function is suitable with any scale of the blastp sbh data output.|
|[/venn.sbh.thread](#/venn.sbh.thread)||


##### 4. Blastn alignment tools


|Function API|Info|
|------------|----|
|[/Blastn.Maps.Taxid](#/Blastn.Maps.Taxid)||
|[/blastn.Query](#/blastn.Query)||
|[/blastn.Query.All](#/blastn.Query.All)||
|[/BlastnMaps.Match.Taxid](#/BlastnMaps.Match.Taxid)||
|[/BlastnMaps.Select](#/BlastnMaps.Select)||
|[/BlastnMaps.Select.Top](#/BlastnMaps.Select.Top)||
|[/BlastnMaps.Summery](#/BlastnMaps.Summery)||
|[/Chromosomes.Export](#/Chromosomes.Export)||
|[/Export.Blastn](#/Export.Blastn)||
|[/Export.blastnMaps](#/Export.blastnMaps)||
|[/Export.blastnMaps.Batch](#/Export.blastnMaps.Batch)|Multiple processor task.|
|[/Export.blastnMaps.littles](#/Export.blastnMaps.littles)||
|[/Export.blastnMaps.Write](#/Export.blastnMaps.Write)|Exports large amount of blastn output files and write all data into a specific csv file.|


##### 5. COG annotation tools


|Function API|Info|
|------------|----|
|[/COG.Statics](#/COG.Statics)||
|[/EXPORT.COGs.from.DOOR](#/EXPORT.COGs.from.DOOR)||
|[/install.cog2003-2014](#/install.cog2003-2014)|Config the ``prot2003-2014.fasta`` database for GCModeller localblast tools. This database will be using for the COG annotation. 
               This command required of the blast+ install first.|
|[/query.cog2003-2014](#/query.cog2003-2014)|Protein COG annotation by using NCBI cog2003-2014.fasta database.|


##### 6. NCBI taxonomy tools


|Function API|Info|
|------------|----|
|[/ref.acc.list](#/ref.acc.list)||
|[/ref.gi.list](#/ref.gi.list)||

## CLI API list
--------------------------
<h3 id="/add.locus_tag"> 1. /add.locus_tag</h3>

Add locus_tag qualifier into the feature slot.
**Prototype**: ``NCBI.localblast.CLI::Int32 AddLocusTag(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /add.locus_tag /gb <gb.gbk> /prefix <prefix> [/add.gene /out <out.gb>]
```


#### Arguments
##### [/add.gene]
Add gene features?

###### Example
```bash
/add.gene <term_string>
```
<h3 id="/add.names"> 2. /add.names</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 AddNames(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /add.names /anno <anno.csv> /gb <genbank.gbk> [/out <out.gbk> /tag <overrides_name>]
```
<h3 id="/AlignmentTable.TopBest"> 3. /AlignmentTable.TopBest</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 AlignmentTableTopBest(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /AlignmentTable.TopBest /in <table.csv> [/out <out.csv>]
```
<h3 id="/Bash.Venn"> 4. /Bash.Venn</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BashShell(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Bash.Venn /blast <blastDIR> /inDIR <fasta.DIR> /inRef <inRefAs.DIR> [/out <outDIR> /evalue <evalue:10>]
```
<h3 id="/bbh.Export"> 5. /bbh.Export</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BBHExportFile(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /bbh.Export /query <query.blastp_out> /subject <subject.blast_out> [/trim /out <bbh.csv> /evalue 1e-3 /coverage 0.85 /identities 0.3]
```
<h3 id="/BBH.Merge"> 6. /BBH.Merge</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 MergeBBH(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /BBH.Merge /in <inDIR> [/out <out.csv>]
```
<h3 id="/BestHits.Filtering"> 7. /BestHits.Filtering</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BestHitFiltering(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /BestHits.Filtering /in <besthit.xml> /sp <table.txt> [/out <out.Xml>]
```
<h3 id="/Blastn.Maps.Taxid"> 8. /Blastn.Maps.Taxid</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BlastnMapsTaxonomy(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Blastn.Maps.Taxid /in <blastnMapping.csv> /2taxid <acc2taxid.tsv/gi2taxid.dmp> [/gi2taxid /trim /tax <NCBI_taxonomy:nodes/names> /out <out.csv>]
```


#### Arguments
##### [/gi2taxid]
The 2taxid data source is comes from gi2taxid, by default is acc2taxid.

###### Example
```bash
/gi2taxid <term_string>
```
##### Accepted Types
###### /gi2taxid
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="/blastn.Query"> 9. /blastn.Query</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BlastnQuery(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /blastn.Query /query <query.fna> /db <db.DIR> [/thread /evalue 1e-5 /word_size <-1> /out <out.DIR>]
```
<h3 id="/blastn.Query.All"> 10. /blastn.Query.All</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BlastnQueryAll(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /blastn.Query.All /query <query.fasta.DIR> /db <db.DIR> [/skip-format /evalue 10 /word_size <-1> /out <out.DIR> /parallel /penalty <penalty> /reward <reward>]
```
<h3 id="/BlastnMaps.Match.Taxid"> 11. /BlastnMaps.Match.Taxid</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 MatchTaxid(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /BlastnMaps.Match.Taxid /in <maps.csv> /acc2taxid <acc2taxid.DIR> [/out <out.tsv>]
```
<h3 id="/BlastnMaps.Select"> 12. /BlastnMaps.Select</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SelectMaps(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /BlastnMaps.Select /in <reads.id.list.txt> /data <blastn.maps.csv> [/out <out.csv>]
```
<h3 id="/BlastnMaps.Select.Top"> 13. /BlastnMaps.Select.Top</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 TopBlastnMapReads(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /BlastnMaps.Select.Top /in <maps.csv> [/out <out.csv>]
```
<h3 id="/BlastnMaps.Summery"> 14. /BlastnMaps.Summery</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BlastnMapsSummery(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /BlastnMaps.Summery /in <in.DIR> [/split "-" /out <out.csv>]
```
<h3 id="/Blastp.BBH.Query"> 15. /Blastp.BBH.Query</h3>

Using query fasta invoke blastp against the fasta files in a directory.
* This command tools required of NCBI blast+ suite, you must config the blast bin path by using ``settings.exe`` before running this command.
**Prototype**: ``NCBI.localblast.CLI::Int32 BlastpBBHQuery(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Blastp.BBH.Query /query <query.fasta> /hit <hit.source> [/out <outDIR> /overrides /num_threads <-1>]
```


#### Arguments
##### /query
The protein query fasta file.

###### Example
```bash
/query <file/directory>
```
##### /hit
A directory contains the protein sequence fasta files which will be using for bbh search.

###### Example
```bash
/hit <file/directory>
```
##### Accepted Types
###### /query
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

###### /hit
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

<h3 id="/Chromosomes.Export"> 16. /Chromosomes.Export</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ChromosomesBlastnResult(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Chromosomes.Export /reads <reads.fasta/DIR> /maps <blastnMappings.Csv/DIR> [/out <outDIR>]
```
<h3 id="/COG.Statics"> 17. /COG.Statics</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 COGStatics(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /COG.Statics /in <myva_cogs.csv> [/locus <locus.txt/csv> /locuMap <Gene> /out <out.csv>]
```
<h3 id="/Copy.PTT"> 18. /Copy.PTT</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 CopyPTT(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Copy.PTT /in <inDIR> [/out <outDIR>]
```
<h3 id="/Copys"> 19. /Copys</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 Copys(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Copys /imports <DIR> [/out <outDIR>]
```
<h3 id="/Export.AlignmentTable"> 20. /Export.AlignmentTable</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportWebAlignmentTable(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.AlignmentTable /in <alignment.txt> [/split /header.split /out <outDIR/file>]
```
<h3 id="/Export.AlignmentTable.giList"> 21. /Export.AlignmentTable.giList</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ParseAlignmentTableGIlist(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.AlignmentTable.giList /in <table.csv> [/out <gi.txt>]
```
<h3 id="/Export.Blastn"> 22. /Export.Blastn</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastn(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.Blastn /in <in.txt> [/out <out.csv>]
```
<h3 id="/Export.blastnMaps"> 23. /Export.blastnMaps</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastnMaps(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.blastnMaps /in <blastn.txt> [/best /out <out.csv>]
```


#### Arguments
##### [/best]
Only output the first hit result for each query as best?

###### Example
```bash
/best <term_string>
```
##### Accepted Types
###### /best
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="/Export.blastnMaps.Batch"> 24. /Export.blastnMaps.Batch</h3>

Multiple processor task.
**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastnMapsBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.blastnMaps.Batch /in <blastn_out.DIR> [/best /out <out.DIR> /num_threads <-1>]
```
<h3 id="/Export.blastnMaps.littles"> 25. /Export.blastnMaps.littles</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastnMapsSmall(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.blastnMaps.littles /in <blastn.txt.DIR> [/out <out.csv.DIR>]
```
<h3 id="/Export.blastnMaps.Write"> 26. /Export.blastnMaps.Write</h3>

Exports large amount of blastn output files and write all data into a specific csv file.
**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastnMapsBatchWrite(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.blastnMaps.Write /in <blastn_out.DIR> [/best /out <write.csv>]
```


#### Arguments
##### /in
The directory path that contains the blastn output data.

###### Example
```bash
/in <file/directory>
# (This argument can accept the std_out from upstream app as input)
```
##### [/best]
Only export the top best blastn alignment hit?

###### Example
```bash
/best
#(bool flag does not require of argument value)
```
##### [/out]
Blastn alignment maps data.

###### Example
```bash
/out <file/directory>
```
##### Accepted Types
###### /in
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

###### /best
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

###### /out
**Decalre**:  _SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BlastnMapping_
Example: 
```json
null
```

<h3 id="/Export.BlastX"> 27. /Export.BlastX</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastX(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.BlastX /in <blastx.txt> [/out <out.csv>]
```
<h3 id="/EXPORT.COGs.from.DOOR"> 28. /EXPORT.COGs.from.DOOR</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportDOORCogs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /EXPORT.COGs.from.DOOR /in <DOOR.opr> [/out <out.csv>]
```
<h3 id="/Export.gb"> 29. /Export.gb</h3>

Export the *.fna, *.faa, *.ptt file from the gbk file.
**Prototype**: ``NCBI.localblast.CLI::Int32 ExportPTTDb(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.gb /gb <genbank.gb/DIR> [/out <outDIR> /simple /batch]
```


#### Arguments
##### [/simple]
Fasta sequence short title, which is just only contains locus_tag

###### Example
```bash
/simple <term_string>
```
##### Accepted Types
###### /simple
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="/Export.gpff"> 30. /Export.gpff</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 EXPORTgpff(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.gpff /in <genome.gpff> /gff <genome.gff> [/out <out.PTT>]
```
<h3 id="/Export.gpffs"> 31. /Export.gpffs</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 EXPORTgpffs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.gpffs [/in <inDIR>]
```
<h3 id="/Export.Locus"> 32. /Export.Locus</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportLocus(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.Locus /in <sbh/bbh_DIR> [/hit /out <out.txt>]
```
<h3 id="/export.prot"> 33. /export.prot</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportProt(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /export.prot /gb <genome.gbk> [/out <out.fasta>]
```
<h3 id="/Fasta.Filters"> 34. /Fasta.Filters</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 Filter(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Fasta.Filters /in <nt.fasta> /key <regex/list.txt> [/tokens /out <out.fasta> /p]
```


#### Arguments
##### [/p]
Using the parallel edition?? If GCModeller running in a 32bit environment, do not use this option. This option only works in single key mode.

###### Example
```bash
/p <term_string>
```
##### Accepted Types
###### /p
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="/Identities.Matrix"> 35. /Identities.Matrix</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 IdentitiesMAT(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Identities.Matrix /hit <sbh/bbh.csv> [/out <out.csv> /cut 0.65]
```
<h3 id="/install.cog2003-2014"> 36. /install.cog2003-2014</h3>

Config the ``prot2003-2014.fasta`` database for GCModeller localblast tools. This database will be using for the COG annotation.
This command required of the blast+ install first.
**Prototype**: ``NCBI.localblast.CLI::Int32 InstallCOGDatabase(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /install.cog2003-2014 /db <prot2003-2014.fasta>
```
###### Example
```bash
localblast /install.cog2003-2014 /db /data/fasta/prot2003-2014.fasta
```


#### Arguments
##### /db
The fasta database using for COG annotation, which can be download from NCBI ftp:
> ftp://ftp.ncbi.nlm.nih.gov/pub/COG/COG2014/data/prot2003-2014.fa.gz

###### Example
```bash
/db <file/directory>
```
##### Accepted Types
###### /db
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

<h3 id="/locus.Selects"> 37. /locus.Selects</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 LocusSelects(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /locus.Selects /locus <locus.txt> /bh <bbhindex.csv> [/out <out.csv>]
```
<h3 id="/MAT.evalue"> 38. /MAT.evalue</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 EvalueMatrix(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /MAT.evalue /in <sbh.csv> [/out <mat.csv> /flip]
```
<h3 id="/Merge.faa"> 39. /Merge.faa</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 MergeFaa(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Merge.faa /in <DIR> /out <out.fasta>
```
<h3 id="/Paralog"> 40. /Paralog</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportParalog(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Paralog /blastp <blastp.txt> [/coverage 0.5 /identities 0.3 /out <out.csv>]
```
<h3 id="/Print"> 41. /Print</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 Print(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Print /in <inDIR> [/ext <ext> /out <out.Csv>]
```
<h3 id="/query.cog2003-2014"> 42. /query.cog2003-2014</h3>

Protein COG annotation by using NCBI cog2003-2014.fasta database.
**Prototype**: ``NCBI.localblast.CLI::Int32 COG2003_2014(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /query.cog2003-2014 /query <query.fasta> [/evalue 1e-5 /coverage 0.65 /identities 0.85 /all /out <out.DIR> /db <cog2003-2014.fasta> /blast+ <blast+/bin>]
```


#### Arguments
##### [/db]
The file path to the database fasta file.
If you have config the cog2003-2014 database previously, then this argument can be omitted.

###### Example
```bash
/db <file/directory>
```
##### [/blast+]
The directory to the NCBI blast+ suite ``bin`` directory. If you have config this path before, then this argument can be omitted.

###### Example
```bash
/blast+ <file/directory>
```
##### [/all]
For export the bbh result, export all match or only the top best? default is only top best.

###### Example
```bash
/all
#(bool flag does not require of argument value)
```
##### [/evalue]
blastp e-value cutoff.

###### Example
```bash
/evalue <float>
```
##### [/out]
The output directory for the work files.

###### Example
```bash
/out <file/directory>
```
##### Accepted Types
###### /db
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

###### /blast+
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

###### /all
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

###### /evalue
**Decalre**:  _System.Double_
Example: 
```json
0
```

###### /out
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

<h3 id="/Reads.OTU.Taxonomy"> 43. /Reads.OTU.Taxonomy</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ReadsOTU_Taxonomy(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Reads.OTU.Taxonomy /in <blastnMaps.csv> /OTU <OTU_data.csv> /tax <taxonomy:nodes/names> [/out <out.csv>]
```
<h3 id="/ref.acc.list"> 44. /ref.acc.list</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 AccessionList(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /ref.acc.list /in <blastnMaps.csv/DIR> [/out <out.csv>]
```
<h3 id="/ref.gi.list"> 45. /ref.gi.list</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 GiList(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /ref.gi.list /in <blastnMaps.csv/DIR> [/out <out.csv>]
```
<h3 id="/SBH.BBH.Batch"> 46. /SBH.BBH.Batch</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SBH_BBH_Batch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SBH.BBH.Batch /in <sbh.DIR> [/identities <-1> /coverage <-1> /all /out <bbh.DIR> /num_threads <-1>]
```
<h3 id="/SBH.Export.Large"> 47. /SBH.Export.Large</h3>

Using this command for export the sbh result of your blastp raw data.
**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBBHLarge(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SBH.Export.Large /in <blastp_out.txt> [/trim-kegg /out <sbh.csv> /identities 0.15 /coverage 0.5]
```


#### Arguments
##### /in
The blastp raw result input file path.

###### Example
```bash
/in <file/directory>
```
##### [/trim-KEGG]
If the fasta sequence source is comes from the KEGG database, and you want to removes the kegg species brief code for the locus_tag, then enable this option.

###### Example
```bash
/trim-KEGG
#(bool flag does not require of argument value)
```
##### [/out]
The sbh result output csv file location.

###### Example
```bash
/out <file/directory>
```
##### Accepted Types
###### /in
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

###### /trim-KEGG
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

###### /out
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

<h3 id="/SBH.Trim"> 48. /SBH.Trim</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SBHTrim(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SBH.Trim /in <sbh.csv> /evalue <evalue> [/identities 0.15 /coverage 0.5 /out <out.csv>]
```
<h3 id="/sbh2bbh"> 49. /sbh2bbh</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BBHExport2(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /sbh2bbh /qvs <qvs.sbh.csv> /svq <svq.sbh.csv> [/identities <-1> /coverage <-1> /all /out <bbh.csv>]
```


#### Arguments
##### [/identities]
Makes a further filtering on the bbh by using this option, default value is -1, so that this means no filter.

###### Example
```bash
/identities <term_string>
```
##### [/coverage]
Makes a further filtering on the bbh by using this option, default value is -1, so that this means no filter.

###### Example
```bash
/coverage <term_string>
```
<h3 id="/Select.Meta"> 50. /Select.Meta</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SelectsMeta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Select.Meta /in <meta.Xml> /bbh <bbh.csv> [/out <out.csv>]
```
<h3 id="/SSBH2BH_LDM"> 51. /SSBH2BH_LDM</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 KEGGSSOrtholog2Bh(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SSBH2BH_LDM /in <ssbh.csv> [/xml /coverage 0.8 /identities 0.3 /out <out.xml>]
```
<h3 id="/SSDB.Export"> 52. /SSDB.Export</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 KEGGSSDBExport(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SSDB.Export /in <inDIR> [/coverage 0.8 /identities 0.3 /out <out.Xml>]
```
<h3 id="/Taxonomy.efetch"> 53. /Taxonomy.efetch</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 FetchTaxnData(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Taxonomy.efetch /in <nt.fasta> [/out <out.DIR>]
```
<h3 id="/Taxonomy.efetch.Merge"> 54. /Taxonomy.efetch.Merge</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 MergeFetchTaxonData(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Taxonomy.efetch.Merge /in <in.DIR> [/out <out.Csv>]
```
<h3 id="/venn.BBH"> 55. /venn.BBH</h3>

2. Build venn table And bbh data from the blastp result out Or sbh data cache.
**Prototype**: ``NCBI.localblast.CLI::Int32 VennBBH(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /venn.BBH /imports <blastp_out.DIR> [/skip-load /query <queryName> /all /coverage <0.6> /identities <0.3> /out <outDIR>]
```


#### Arguments
##### [/skip-load]
If the data source in the imports directory Is already the sbh data source, then using this parameter to skip the blastp file parsing.

###### Example
```bash
/skip-load <term_string>
```
<h3 id="/venn.BlastAll"> 56. /venn.BlastAll</h3>

Completely paired combos blastp bbh operations for the venn diagram Or network builder.
**Prototype**: ``NCBI.localblast.CLI::Int32 vennBlastAll(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /venn.BlastAll /query <queryDIR> /out <outDIR> [/num_threads <-1> /evalue 10 /overrides /all /coverage <0.8> /identities <0.3>]
```


#### Arguments
##### /query
Recommended format of the fasta title Is that the fasta title only contains gene locus_tag.

###### Example
```bash
/query <term_string>
```
##### [/num_threads]
The number of the parallel blast task in this command, default value Is -1 which means the number of the blast threads Is determined by system automatically.

###### Example
```bash
/num_threads <term_string>
```
##### [/all]
If this parameter Is represent, then all of the paired best hit will be export, otherwise only the top best will be export.

###### Example
```bash
/all <term_string>
```
<h3 id="/venn.cache"> 57. /venn.cache</h3>

1. [SBH_Batch] Creates the sbh cache data for the downstream bbh analysis.
And this batch function is suitable with any scale of the blastp sbh data output.
**Prototype**: ``NCBI.localblast.CLI::Int32 VennCache(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /venn.cache /imports <blastp.DIR> [/out <sbh.out.DIR> /coverage <0.6> /identities <0.3> /num_threads <-1> /overrides]
```


#### Arguments
##### [/num_threads]
The number of the sub process thread. -1 value is stands for auto config by the system.

###### Example
```bash
/num_threads <term_string>
```
<h3 id="/venn.sbh.thread"> 58. /venn.sbh.thread</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SBHThread(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /venn.sbh.thread /in <blastp.txt> [/out <out.sbh.csv> /coverage <0.6> /identities <0.3> /overrides]
```
<h3 id="/Venn.Single"> 59. /Venn.Single</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 VennSingle(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Venn.Single /in <besthits.Xml> [/out <out.csv>]
```
<h3 id="--bbh.export"> 60. --bbh.export</h3>

Batch export bbh result data from a directory.
**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBBH(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --bbh.export /in <blast_out.DIR> [/all /out <out.DIR> /single-query <queryName> /coverage <0.5> /identities 0.15]
```


#### Arguments
##### [/all]
If this all Boolean value is specific, then the program will export all hits for the bbh not the top 1 best.

###### Example
```bash
/all <term_string>
```
<h3 id="blast"> 61. blast</h3>

In order to draw as venn diagram for a specific set of genome and study the diferrence and consists between these genomes, you should do the blast operation from the protein amino aciad sequence first. The blastp operation can be performenced by the blast+ program which you can download from the NCBI website, this command is a interop service for the NCBI blast program, you should install the blast+ program at first.
**Prototype**: ``NCBI.localblast.CLI::Int32 BLASTA(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast blast -i <file_directory> -blast_bin <BLAST_program_directory> -program <program_type_name> [-ld <log_dir> -xld <xml_log_dir>]
```
###### Example
```bash
localblast blast -i /home/xieguigang/GCModeller/examples/blast_test/ -ld ~/Desktop/logs -xld ~/Desktop/xmls -blast_bin ~/BLAST/bin
```


#### Arguments
##### -i
The input data directory which is contains the FASTA format protein amino acid sequence data file.

###### Example
```bash
-i ~/Desktop/8004
```
##### -blast_bin
The localtion for the blast+ program, you should specific this switch value or this program will throw an exception.

###### Example
```bash
-blast_bin ~/BLAST/bin
```
##### -program
The program type name for the NCBI local blast executable assembly.

###### Example
```bash
-program blast+
```
##### [-xld]
Optional, the parsed and well organized blast log file output directory, if this switch value is not specific by the user then the user desktop directoy will be used as default.

###### Example
```bash
-xld ~/Desktop/xml_logs
```
##### [-ld]
Optional, the blastp log file output directory for the NCBI blast+ program. If this switch value is not specific by the user then the user desktop directory will be specific for the logs file output as default.

###### Example
```bash
-ld ~/Desktop/logs/
```
<h3 id="--blast.self"> 62. --blast.self</h3>

Query fasta query against itself for paralogs.
**Prototype**: ``NCBI.localblast.CLI::Int32 SelfBlast(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --blast.self /query <query.fasta> [/blast <blast_HOME> /out <out.csv>]
```
<h3 id="-copy"> 63. -copy</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 Copy(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast -copy -i <index_file> -os <output_saved> [-osidx <id_column_index> -os_skip_first <T/F>]
```
<h3 id="--Export.Fasta"> 64. --Export.Fasta</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportFasta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Export.Fasta /hits <query-hits.csv> /query <query.fasta> /subject <subject.fasta>
```
<h3 id="--Export.Overviews"> 65. --Export.Overviews</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportOverviews(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Export.Overviews /blast <blastout.txt> [/out <overview.csv>]
```
<h3 id="--Export.SBH"> 66. --Export.SBH</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportSBH(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Export.SBH /in <in.DIR> /prefix <queryName> /out <out.csv> [/txt]
```
<h3 id="-export_besthit"> 67. -export_besthit</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBestHit(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast -export_besthit -i <input_csv_file> -o <output_saved_csv>
```
<h3 id="grep"> 68. grep</h3>

The gene id in the blast output log file are not well format for reading and program processing, so before you generate the venn diagram you should call this command to parse the gene id from the log file. You can also done this id parsing job using other tools.
**Prototype**: ``NCBI.localblast.CLI::Int32 Grep(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast grep -i <xml_log_file> -q <script_statements> -h <script_statements>
```
###### Example
```bash
localblast grep -i C:\Users\WORKGROUP\Desktop\blast_xml_logs\1__8004_ecoli_prot.log.xml -q "tokens | 4" -h "'tokens | 2';'tokens ' ' 0'"
```


#### Arguments
##### -q
The parsing script for parsing the gene_id from the blast log file, this switch value is consist of sevral operation tokens, and each token is separate by the ';' character and the token unit in each script token should seperate by the ' character.
There are two basic operation in this parsing script:
tokens - Split the query or hit name string into sevral piece of string by the specific delimiter character and           get the specifc location unit in the return string array.
Usage:   tokens <delimiter> <position>
Example: tokens | 3 match - match a gene id using a specific pattern regular expression.
usage:   match <regular_expression>
Example: match .+[-]\d{5}

###### Example
```bash
-q "'tokens | 5';'match .+[-].+'"
```
##### -h
The parsing script for parsing the gene_id from the blast log file, this switch value is consist of sevral operation tokens, and each token is separate by the ';' character and the token unit in each script token should seperate by the ' character.
There are two basic operation in this parsing script:
tokens - Split the query or hit name string into sevral piece of string by the specific delimiter character and           get the specifc location unit in the return string array.
Usage:   tokens <delimiter> <position>
Example: tokens | 3 match - match a gene id using a specific pattern regular expression.
usage:   match <regular_expression>
Example: match .+[-]\d{5}

###### Example
```bash
-h "'tokens | 5';'match .+[-].+'"
```
<h3 id="logs_analysis"> 69. logs_analysis</h3>

Parsing the xml format blast log into a csv data file that use for venn diagram drawing.
**Prototype**: ``NCBI.localblast.CLI::Int32 bLogAnalysis(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast logs_analysis -d <xml_logs_directory> -export <export_csv_file>
```
###### Example
```bash
localblast logs_analysis -d ~/xml_logs -export ~/Desktop/result.csv
```


#### Arguments
##### -d
The data directory which contains the xml format blast log file, those xml format log file were generated from the 'venn -> blast' command.

###### Example
```bash
-d ~/xml_logs
```
##### -export
The save file path for the venn diagram drawing data csv file.

###### Example
```bash
-export ~/Documents/8004_venn.csv
```
<h3 id="merge"> 70. merge</h3>

This program can not use the blast parsing result for the venn diagram drawing operation, and this command is using for generate the drawing data for the venn diagram drawing command, this command merge the blast log parsing result and then using the parsing result for drawing a venn diagram.
**Prototype**: ``NCBI.localblast.CLI::Int32 Merge(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast merge -d <directory> -o <output_file>
```
###### Example
```bash
localblast merge -d ~/blast_besthit/ -o ~/Desktop/compared.csv
```


#### Arguments
##### -d
The directory that contains some blast log parsing csv data file.

###### Example
```bash
-d ~/Desktop/blast/result/
```
##### -o
The save file name for the output result, the program willl save the merge result in the csv format

###### Example
```bash
-o ~/Desktop/8004_venn.csv
```
<h3 id="-merge_besthit"> 71. -merge_besthit</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 MergeBestHits(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast -merge_besthit -i <input_file_list> -o <output_file> -os <original_idlist_sequence_file> [-osidx <id_column_index> -os_skip_first <T/F>]
```


#### Arguments
##### -i
Each file path in the filelist should be separated by a "|" character.

###### Example
```bash
-i <term_string>
```
<h3 id="--Xml2Excel"> 72. --Xml2Excel</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 XmlToExcel(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Xml2Excel /in <in.xml> [/out <out.csv>]
```
<h3 id="--Xml2Excel.Batch"> 73. --Xml2Excel.Batch</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 XmlToExcelBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Xml2Excel.Batch /in <inDIR> [/out <outDIR> /Merge]
```
