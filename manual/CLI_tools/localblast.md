---
title: localblast
tags: [maunal, tools]
date: 2016/10/22 12:30:12
---
# GCModeller [version 1.0.0.0]
> Wrapper tools for the ncbi blast+ program and the blast output data analysis program.

<!--more-->

**NCBI localblast wrapper tools**
_NCBI localblast_
Copyright ? GPL3 2015

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/localblast.exe
**Root namespace**: ``NCBI.localblast.CLI``

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
|[/Ref.Gi.list](#/Ref.Gi.list)||
|[/SBH.Export.Large](#/SBH.Export.Large)||
|[/SSBH2BH_LDM](#/SSBH2BH_LDM)||
|[/SSDB.Export](#/SSDB.Export)||
|[/Taxonomy.efetch](#/Taxonomy.efetch)||
|[/Taxonomy.efetch.Merge](#/Taxonomy.efetch.Merge)||
|[/Venn.Single](#/Venn.Single)||
|[--bbh.export](#--bbh.export)|Batch export bbh result data from a directory.|
|[blast](#blast)|In order to draw as venn diagram for a specific set of genome and study the diferrence and consists between these genomes, you should do the blast operation from the protein amino aciad sequence first. The blastp operation can be performenced by the blast+ program which you can download from the NCBI website, this command is a interop service for the NCBI blast program£¬ you should install the blast+ program at first.|
|[--blast.self](#--blast.self)||
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
|[/Blastp.BBH.Query](#/Blastp.BBH.Query)||
|[/Export.Locus](#/Export.Locus)||
|[/locus.Selects](#/locus.Selects)||
|[/SBH.BBH.Batch](#/SBH.BBH.Batch)||
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
|[/Export.blastnMaps.Batch](#/Export.blastnMaps.Batch)||
|[/Export.blastnMaps.littles](#/Export.blastnMaps.littles)||


##### 5. COG annotation tools


|Function API|Info|
|------------|----|
|[/COG.Statics](#/COG.Statics)||
|[/EXPORT.COGs.from.DOOR](#/EXPORT.COGs.from.DOOR)||

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


**Prototype**: ``NCBI.localblast.CLI::Int32 BlastpBBHQuery(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Blastp.BBH.Query /query <query.fasta> /hit <hit.source> [/out <outDIR> /overrides /num_threads <-1>]
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
<h3 id="/Export.BlastX"> 26. /Export.BlastX</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastX(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.BlastX /in <blastx.txt> [/out <out.csv>]
```
<h3 id="/EXPORT.COGs.from.DOOR"> 27. /EXPORT.COGs.from.DOOR</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportDOORCogs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /EXPORT.COGs.from.DOOR /in <DOOR.opr> [/out <out.csv>]
```
<h3 id="/Export.gb"> 28. /Export.gb</h3>

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

<h3 id="/Export.gpff"> 29. /Export.gpff</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 EXPORTgpff(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.gpff /in <genome.gpff> /gff <genome.gff> [/out <out.PTT>]
```
<h3 id="/Export.gpffs"> 30. /Export.gpffs</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 EXPORTgpffs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.gpffs [/in <inDIR>]
```
<h3 id="/Export.Locus"> 31. /Export.Locus</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportLocus(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.Locus /in <sbh/bbh_DIR> [/hit /out <out.txt>]
```
<h3 id="/export.prot"> 32. /export.prot</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportProt(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /export.prot /gb <genome.gbk> [/out <out.fasta>]
```
<h3 id="/Fasta.Filters"> 33. /Fasta.Filters</h3>


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

<h3 id="/Identities.Matrix"> 34. /Identities.Matrix</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 IdentitiesMAT(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Identities.Matrix /hit <sbh/bbh.csv> [/out <out.csv> /cut 0.65]
```
<h3 id="/locus.Selects"> 35. /locus.Selects</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 LocusSelects(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /locus.Selects /locus <locus.txt> /bh <bbhindex.csv> [/out <out.csv>]
```
<h3 id="/MAT.evalue"> 36. /MAT.evalue</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 EvalueMatrix(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /MAT.evalue /in <sbh.csv> [/out <mat.csv> /flip]
```
<h3 id="/Merge.faa"> 37. /Merge.faa</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 MergeFaa(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Merge.faa /in <DIR> /out <out.fasta>
```
<h3 id="/Paralog"> 38. /Paralog</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportParalog(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Paralog /blastp <blastp.txt> [/coverage 0.5 /identities 0.3 /out <out.csv>]
```
<h3 id="/Print"> 39. /Print</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 Print(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Print /in <inDIR> [/ext <ext> /out <out.Csv>]
```
<h3 id="/Reads.OTU.Taxonomy"> 40. /Reads.OTU.Taxonomy</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ReadsOTU_Taxonomy(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Reads.OTU.Taxonomy /in <blastnMaps.csv> /OTU <OTU_data.csv> /tax <taxonomy:nodes/names> [/out <out.csv>]
```
<h3 id="/Ref.Gi.list"> 41. /Ref.Gi.list</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 GiList(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Ref.Gi.list /in <blastnMaps.csv> [/out <out.csv>]
```
<h3 id="/SBH.BBH.Batch"> 42. /SBH.BBH.Batch</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SBH_BBH_Batch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SBH.BBH.Batch /in <sbh.DIR> [/identities <-1> /coverage <-1> /all /out <bbh.DIR> /num_threads <-1>]
```
<h3 id="/SBH.Export.Large"> 43. /SBH.Export.Large</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBBHLarge(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SBH.Export.Large /in <blast_out.txt> [/trim-kegg /out <bbh.csv> /identities 0.15 /coverage 0.5]
```


#### Arguments
##### [/trim-KEGG]
If the fasta sequence source is comes from the KEGG database, and you want to removes the kegg species brief code for the locus_tag, then enable this option.

###### Example
```bash
/trim-KEGG <term_string>
```
<h3 id="/SBH.Trim"> 44. /SBH.Trim</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SBHTrim(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SBH.Trim /in <sbh.csv> /evalue <evalue> [/identities 0.15 /coverage 0.5 /out <out.csv>]
```
<h3 id="/sbh2bbh"> 45. /sbh2bbh</h3>


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
<h3 id="/Select.Meta"> 46. /Select.Meta</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SelectsMeta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Select.Meta /in <meta.Xml> /bbh <bbh.csv> [/out <out.csv>]
```
<h3 id="/SSBH2BH_LDM"> 47. /SSBH2BH_LDM</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 KEGGSSOrtholog2Bh(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SSBH2BH_LDM /in <ssbh.csv> [/xml /coverage 0.8 /identities 0.3 /out <out.xml>]
```
<h3 id="/SSDB.Export"> 48. /SSDB.Export</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 KEGGSSDBExport(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SSDB.Export /in <inDIR> [/coverage 0.8 /identities 0.3 /out <out.Xml>]
```
<h3 id="/Taxonomy.efetch"> 49. /Taxonomy.efetch</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 FetchTaxnData(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Taxonomy.efetch /in <nt.fasta> [/out <out.DIR>]
```
<h3 id="/Taxonomy.efetch.Merge"> 50. /Taxonomy.efetch.Merge</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 MergeFetchTaxonData(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Taxonomy.efetch.Merge /in <in.DIR> [/out <out.Csv>]
```
<h3 id="/venn.BBH"> 51. /venn.BBH</h3>

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
<h3 id="/venn.BlastAll"> 52. /venn.BlastAll</h3>

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
<h3 id="/venn.cache"> 53. /venn.cache</h3>

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
<h3 id="/venn.sbh.thread"> 54. /venn.sbh.thread</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SBHThread(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /venn.sbh.thread /in <blastp.txt> [/out <out.sbh.csv> /coverage <0.6> /identities <0.3> /overrides]
```
<h3 id="/Venn.Single"> 55. /Venn.Single</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 VennSingle(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Venn.Single /in <besthits.Xml> [/out <out.csv>]
```
<h3 id="--bbh.export"> 56. --bbh.export</h3>

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
<h3 id="blast"> 57. blast</h3>

In order to draw as venn diagram for a specific set of genome and study the diferrence and consists between these genomes, you should do the blast operation from the protein amino aciad sequence first. The blastp operation can be performenced by the blast+ program which you can download from the NCBI website, this command is a interop service for the NCBI blast program£¬ you should install the blast+ program at first.
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
<h3 id="--blast.self"> 58. --blast.self</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SelfBlast(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --blast.self /query <query.fasta> [/blast <blast_HOME> /out <out.csv>]
```
<h3 id="-copy"> 59. -copy</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 Copy(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast -copy -i <index_file> -os <output_saved> [-osidx <id_column_index> -os_skip_first <T/F>]
```
<h3 id="--Export.Fasta"> 60. --Export.Fasta</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportFasta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Export.Fasta /hits <query-hits.csv> /query <query.fasta> /subject <subject.fasta>
```
<h3 id="--Export.Overviews"> 61. --Export.Overviews</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportOverviews(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Export.Overviews /blast <blastout.txt> [/out <overview.csv>]
```
<h3 id="--Export.SBH"> 62. --Export.SBH</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportSBH(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Export.SBH /in <in.DIR> /prefix <queryName> /out <out.csv> [/txt]
```
<h3 id="-export_besthit"> 63. -export_besthit</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBestHit(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast -export_besthit -i <input_csv_file> -o <output_saved_csv>
```
<h3 id="grep"> 64. grep</h3>

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
<h3 id="logs_analysis"> 65. logs_analysis</h3>

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
<h3 id="merge"> 66. merge</h3>

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
<h3 id="-merge_besthit"> 67. -merge_besthit</h3>


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
<h3 id="--Xml2Excel"> 68. --Xml2Excel</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 XmlToExcel(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Xml2Excel /in <in.xml> [/out <out.csv>]
```
<h3 id="--Xml2Excel.Batch"> 69. --Xml2Excel.Batch</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 XmlToExcelBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Xml2Excel.Batch /in <inDIR> [/out <outDIR> /Merge]
```
