---
title: localblast
tags: [maunal, tools]
date: 2016/10/19 16:38:32
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
|[/add.locus_tag](#/add.locus_tag)|Add locus_tag qualifier into the feature slot.|
|[/add.names](#/add.names)||
|[/Bash.Venn](#/Bash.Venn)||
|[/bbh.Export](#/bbh.Export)||
|[/BBH.Merge](#/BBH.Merge)||
|[/BestHits.Filtering](#/BestHits.Filtering)||
|[/Blastn.Maps.Taxid](#/Blastn.Maps.Taxid)||
|[/blastn.Query](#/blastn.Query)||
|[/blastn.Query.All](#/blastn.Query.All)||
|[/BlastnMaps.Select](#/BlastnMaps.Select)||
|[/BlastnMaps.Select.Top](#/BlastnMaps.Select.Top)||
|[/Blastp.BBH.Query](#/Blastp.BBH.Query)||
|[/Chromosomes.Export](#/Chromosomes.Export)||
|[/COG.Statics](#/COG.Statics)||
|[/Copy.PTT](#/Copy.PTT)||
|[/Copys](#/Copys)||
|[/Export.Blastn](#/Export.Blastn)||
|[/Export.blastnMaps](#/Export.blastnMaps)||
|[/Export.blastnMaps.Batch](#/Export.blastnMaps.Batch)||
|[/Export.blastnMaps.littles](#/Export.blastnMaps.littles)||
|[/Export.BlastX](#/Export.BlastX)||
|[/EXPORT.COGs.from.DOOR](#/EXPORT.COGs.from.DOOR)||
|[/Export.gb](#/Export.gb)|Export the *.fna, *.faa, *.ptt file from the gbk file.|
|[/Export.gpff](#/Export.gpff)||
|[/Export.gpffs](#/Export.gpffs)||
|[/Export.Locus](#/Export.Locus)||
|[/export.prot](#/export.prot)||
|[/Fasta.Filters](#/Fasta.Filters)||
|[/Identities.Matrix](#/Identities.Matrix)||
|[/locus.Selects](#/locus.Selects)||
|[/MAT.evalue](#/MAT.evalue)||
|[/Merge.faa](#/Merge.faa)||
|[/Paralog](#/Paralog)||
|[/Print](#/Print)||
|[/Reads.OTU.Taxonomy](#/Reads.OTU.Taxonomy)||
|[/Ref.Gi.list](#/Ref.Gi.list)||
|[/SBH.BBH.Batch](#/SBH.BBH.Batch)||
|[/SBH.Export.Large](#/SBH.Export.Large)||
|[/SBH.Trim](#/SBH.Trim)||
|[/sbh2bbh](#/sbh2bbh)||
|[/Select.Meta](#/Select.Meta)||
|[/SSBH2BH_LDM](#/SSBH2BH_LDM)||
|[/SSDB.Export](#/SSDB.Export)||
|[/Taxonomy.efetch](#/Taxonomy.efetch)||
|[/Taxonomy.efetch.Merge](#/Taxonomy.efetch.Merge)||
|[/venn.BBH](#/venn.BBH)|2. Build venn table And bbh data from the blastp result out Or sbh data cache.|
|[/venn.BlastAll](#/venn.BlastAll)|Completely paired combos blastp bbh operations for the venn diagram Or network builder.|
|[/venn.cache](#/venn.cache)|1. [SBH_Batch] Creates the sbh cache data for the downstream bbh analysis. 
               And this batch function is suitable with any scale of the blastp sbh data output.|
|[/venn.sbh.thread](#/venn.sbh.thread)||
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


##### 1. NCBI Web Blast Tools


|Function API|Info|
|------------|----|
|[/AlignmentTable.TopBest](#/AlignmentTable.TopBest)||
|[/Export.AlignmentTable](#/Export.AlignmentTable)||
|[/Export.AlignmentTable.giList](#/Export.AlignmentTable.giList)||




## CLI API list
--------------------------
<h3 id="/add.locus_tag"> 1. /add.locus_tag</h3>

Add locus_tag qualifier into the feature slot.
**Prototype**: ``NCBI.localblast.CLI::Int32 AddLocusTag(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /add.locus_tag /gb <gb.gbk> /prefix <prefix> [/add.gene /out <out.gb>]
```
###### Example
```bash
localblast
```



#### Parameters information:
##### [/add.gene]
Add gene features?

###### Example
```bash

```
##### Accepted Types
###### /add.gene
<h3 id="/add.names"> 2. /add.names</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 AddNames(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /add.names /anno <anno.csv> /gb <genbank.gbk> [/out <out.gbk> /tag <overrides_name>]
```
###### Example
```bash
localblast
```
<h3 id="/AlignmentTable.TopBest"> 3. /AlignmentTable.TopBest</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 AlignmentTableTopBest(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /AlignmentTable.TopBest /in <table.csv> [/out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/Bash.Venn"> 4. /Bash.Venn</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BashShell(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Bash.Venn /blast <blastDIR> /inDIR <fasta.DIR> /inRef <inRefAs.DIR> [/out <outDIR> /evalue <evalue:10>]
```
###### Example
```bash
localblast
```
<h3 id="/bbh.Export"> 5. /bbh.Export</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BBHExportFile(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /bbh.Export /query <query.blastp_out> /subject <subject.blast_out> [/trim /out <bbh.csv> /evalue 1e-3 /coverage 0.85 /identities 0.3]
```
###### Example
```bash
localblast
```
<h3 id="/BBH.Merge"> 6. /BBH.Merge</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 MergeBBH(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /BBH.Merge /in <inDIR> [/out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/BestHits.Filtering"> 7. /BestHits.Filtering</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BestHitFiltering(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /BestHits.Filtering /in <besthit.xml> /sp <table.txt> [/out <out.Xml>]
```
###### Example
```bash
localblast
```
<h3 id="/Blastn.Maps.Taxid"> 8. /Blastn.Maps.Taxid</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BlastnMapsTaxonomy(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Blastn.Maps.Taxid /in <blastnMapping.csv> /gi2taxid <gi2taxid.dmp> [/trim /tax <NCBI_taxonomy:nodes/names> /out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/blastn.Query"> 9. /blastn.Query</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BlastnQuery(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /blastn.Query /query <query.fna> /db <db.DIR> [/thread /evalue 1e-5 /word_size <-1> /out <out.DIR>]
```
###### Example
```bash
localblast
```
<h3 id="/blastn.Query.All"> 10. /blastn.Query.All</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BlastnQueryAll(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /blastn.Query.All /query <query.fasta.DIR> /db <db.DIR> [/skip-format /evalue 10 /word_size <-1> /out <out.DIR> /parallel /penalty <penalty> /reward <reward>]
```
###### Example
```bash
localblast
```
<h3 id="/BlastnMaps.Select"> 11. /BlastnMaps.Select</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SelectMaps(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /BlastnMaps.Select /in <reads.id.list.txt> /data <blastn.maps.csv> [/out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/BlastnMaps.Select.Top"> 12. /BlastnMaps.Select.Top</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 TopBlastnMapReads(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /BlastnMaps.Select.Top /in <maps.csv> [/out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/Blastp.BBH.Query"> 13. /Blastp.BBH.Query</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BlastpBBHQuery(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Blastp.BBH.Query /query <query.fasta> /hit <hit.source> [/out <outDIR> /overrides /num_threads <-1>]
```
###### Example
```bash
localblast
```
<h3 id="/Chromosomes.Export"> 14. /Chromosomes.Export</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ChromosomesBlastnResult(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Chromosomes.Export /reads <reads.fasta/DIR> /maps <blastnMappings.Csv/DIR> [/out <outDIR>]
```
###### Example
```bash
localblast
```
<h3 id="/COG.Statics"> 15. /COG.Statics</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 COGStatics(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /COG.Statics /in <myva_cogs.csv> [/locus <locus.txt/csv> /locuMap <Gene> /out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/Copy.PTT"> 16. /Copy.PTT</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 CopyPTT(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Copy.PTT /in <inDIR> [/out <outDIR>]
```
###### Example
```bash
localblast
```
<h3 id="/Copys"> 17. /Copys</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 Copys(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Copys /imports <DIR> [/out <outDIR>]
```
###### Example
```bash
localblast
```
<h3 id="/Export.AlignmentTable"> 18. /Export.AlignmentTable</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportWebAlignmentTable(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.AlignmentTable /in <alignment.txt> [/split /header.split /out <outDIR/file>]
```
###### Example
```bash
localblast
```
<h3 id="/Export.AlignmentTable.giList"> 19. /Export.AlignmentTable.giList</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ParseAlignmentTableGIlist(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.AlignmentTable.giList /in <table.csv> [/out <gi.txt>]
```
###### Example
```bash
localblast
```
<h3 id="/Export.Blastn"> 20. /Export.Blastn</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastn(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.Blastn /in <in.txt> [/out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/Export.blastnMaps"> 21. /Export.blastnMaps</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastnMaps(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.blastnMaps /in <blastn.txt> [/best /out <out.csv>]
```
###### Example
```bash
localblast
```



#### Parameters information:
##### [/best]
Only output the first hit result for each query as best?

###### Example
```bash

```
##### Accepted Types
###### /best
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="/Export.blastnMaps.Batch"> 22. /Export.blastnMaps.Batch</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastnMapsBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.blastnMaps.Batch /in <blastn_out.DIR> [/out <out.DIR> /num_threads <-1>]
```
###### Example
```bash
localblast
```
<h3 id="/Export.blastnMaps.littles"> 23. /Export.blastnMaps.littles</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastnMapsSmall(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.blastnMaps.littles /in <blastn.txt.DIR> [/out <out.csv.DIR>]
```
###### Example
```bash
localblast
```
<h3 id="/Export.BlastX"> 24. /Export.BlastX</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastX(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.BlastX /in <blastx.txt> [/out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/EXPORT.COGs.from.DOOR"> 25. /EXPORT.COGs.from.DOOR</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportDOORCogs(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /EXPORT.COGs.from.DOOR /in <DOOR.opr> [/out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/Export.gb"> 26. /Export.gb</h3>

Export the *.fna, *.faa, *.ptt file from the gbk file.
**Prototype**: ``NCBI.localblast.CLI::Int32 ExportPTTDb(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.gb /gb <genbank.gb/DIR> [/out <outDIR> /simple /batch]
```
###### Example
```bash
localblast
```



#### Parameters information:
##### [/simple]
Fasta sequence short title, which is just only contains locus_tag

###### Example
```bash

```
##### Accepted Types
###### /simple
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="/Export.gpff"> 27. /Export.gpff</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 EXPORTgpff(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.gpff /in <genome.gpff> /gff <genome.gff> [/out <out.PTT>]
```
###### Example
```bash
localblast
```
<h3 id="/Export.gpffs"> 28. /Export.gpffs</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 EXPORTgpffs(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.gpffs [/in <inDIR>]
```
###### Example
```bash
localblast
```
<h3 id="/Export.Locus"> 29. /Export.Locus</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportLocus(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.Locus /in <sbh/bbh_DIR> [/hit /out <out.txt>]
```
###### Example
```bash
localblast
```
<h3 id="/export.prot"> 30. /export.prot</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportProt(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /export.prot /gb <genome.gbk> [/out <out.fasta>]
```
###### Example
```bash
localblast
```
<h3 id="/Fasta.Filters"> 31. /Fasta.Filters</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 Filter(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Fasta.Filters /in <nt.fasta> /key <regex/list.txt> [/tokens /out <out.fasta> /p]
```
###### Example
```bash
localblast
```



#### Parameters information:
##### [/p]
Using the parallel edition?? If GCModeller running in a 32bit environment, do not use this option. This option only works in single key mode.

###### Example
```bash

```
##### Accepted Types
###### /p
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="/Identities.Matrix"> 32. /Identities.Matrix</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 IdentitiesMAT(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Identities.Matrix /hit <sbh/bbh.csv> [/out <out.csv> /cut 0.65]
```
###### Example
```bash
localblast
```
<h3 id="/locus.Selects"> 33. /locus.Selects</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 LocusSelects(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /locus.Selects /locus <locus.txt> /bh <bbhindex.csv> [/out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/MAT.evalue"> 34. /MAT.evalue</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 EvalueMatrix(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /MAT.evalue /in <sbh.csv> [/out <mat.csv> /flip]
```
###### Example
```bash
localblast
```
<h3 id="/Merge.faa"> 35. /Merge.faa</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 MergeFaa(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Merge.faa /in <DIR> /out <out.fasta>
```
###### Example
```bash
localblast
```
<h3 id="/Paralog"> 36. /Paralog</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportParalog(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Paralog /blastp <blastp.txt> [/coverage 0.5 /identities 0.3 /out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/Print"> 37. /Print</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 Print(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Print /in <inDIR> [/ext <ext> /out <out.Csv>]
```
###### Example
```bash
localblast
```
<h3 id="/Reads.OTU.Taxonomy"> 38. /Reads.OTU.Taxonomy</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ReadsOTU_Taxonomy(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Reads.OTU.Taxonomy /in <blastnMaps.csv> /OTU <OTU_data.csv> /tax <taxonomy:nodes/names> [/out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/Ref.Gi.list"> 39. /Ref.Gi.list</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 GiList(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Ref.Gi.list /in <blastnMaps.csv> [/out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/SBH.BBH.Batch"> 40. /SBH.BBH.Batch</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SBH_BBH_Batch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SBH.BBH.Batch /in <sbh.DIR> [/identities <-1> /coverage <-1> /all /out <bbh.DIR> /num_threads <-1>]
```
###### Example
```bash
localblast
```
<h3 id="/SBH.Export.Large"> 41. /SBH.Export.Large</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBBHLarge(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SBH.Export.Large /in <blast_out.txt> [/trim-kegg /out <bbh.csv> /identities 0.15 /coverage 0.5]
```
###### Example
```bash
localblast
```



#### Parameters information:
##### [/trim-KEGG]
If the fasta sequence source is comes from the KEGG database, and you want to removes the kegg species brief code for the locus_tag, then enable this option.

###### Example
```bash

```
##### Accepted Types
###### /trim-KEGG
<h3 id="/SBH.Trim"> 42. /SBH.Trim</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SBHTrim(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SBH.Trim /in <sbh.csv> /evalue <evalue> [/identities 0.15 /coverage 0.5 /out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/sbh2bbh"> 43. /sbh2bbh</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BBHExport2(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /sbh2bbh /qvs <qvs.sbh.csv> /svq <svq.sbh.csv> [/identities <-1> /coverage <-1> /all /out <bbh.csv>]
```
###### Example
```bash
localblast
```



#### Parameters information:
##### [/identities]
Makes a further filtering on the bbh by using this option, default value is -1, so that this means no filter.

###### Example
```bash

```
##### [/coverage]
Makes a further filtering on the bbh by using this option, default value is -1, so that this means no filter.

###### Example
```bash

```
##### Accepted Types
###### /identities
###### /coverage
<h3 id="/Select.Meta"> 44. /Select.Meta</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SelectsMeta(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Select.Meta /in <meta.Xml> /bbh <bbh.csv> [/out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="/SSBH2BH_LDM"> 45. /SSBH2BH_LDM</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 KEGGSSOrtholog2Bh(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SSBH2BH_LDM /in <ssbh.csv> [/xml /coverage 0.8 /identities 0.3 /out <out.xml>]
```
###### Example
```bash
localblast
```
<h3 id="/SSDB.Export"> 46. /SSDB.Export</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 KEGGSSDBExport(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SSDB.Export /in <inDIR> [/coverage 0.8 /identities 0.3 /out <out.Xml>]
```
###### Example
```bash
localblast
```
<h3 id="/Taxonomy.efetch"> 47. /Taxonomy.efetch</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 FetchTaxnData(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Taxonomy.efetch /in <nt.fasta> [/out <out.DIR>]
```
###### Example
```bash
localblast
```
<h3 id="/Taxonomy.efetch.Merge"> 48. /Taxonomy.efetch.Merge</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 MergeFetchTaxonData(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Taxonomy.efetch.Merge /in <in.DIR> [/out <out.Csv>]
```
###### Example
```bash
localblast
```
<h3 id="/venn.BBH"> 49. /venn.BBH</h3>

2. Build venn table And bbh data from the blastp result out Or sbh data cache.
**Prototype**: ``NCBI.localblast.CLI::Int32 VennBBH(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /venn.BBH /imports <blastp_out.DIR> [/skip-load /query <queryName> /all /coverage <0.6> /identities <0.3> /out <outDIR>]
```
###### Example
```bash
localblast
```



#### Parameters information:
##### [/skip-load]
If the data source in the imports directory Is already the sbh data source, then using this parameter to skip the blastp file parsing.

###### Example
```bash

```
##### Accepted Types
###### /skip-load
<h3 id="/venn.BlastAll"> 50. /venn.BlastAll</h3>

Completely paired combos blastp bbh operations for the venn diagram Or network builder.
**Prototype**: ``NCBI.localblast.CLI::Int32 vennBlastAll(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /venn.BlastAll /query <queryDIR> /out <outDIR> [/num_threads <-1> /evalue 10 /overrides /all /coverage <0.8> /identities <0.3>]
```
###### Example
```bash
localblast
```



#### Parameters information:
##### /query
Recommended format of the fasta title Is that the fasta title only contains gene locus_tag.

###### Example
```bash

```
##### [/num_threads]
The number of the parallel blast task in this command, default value Is -1 which means the number of the blast threads Is determined by system automatically.

###### Example
```bash

```
##### [/all]
If this parameter Is represent, then all of the paired best hit will be export, otherwise only the top best will be export.

###### Example
```bash

```
##### Accepted Types
###### /query
###### /num_threads
###### /all
<h3 id="/venn.cache"> 51. /venn.cache</h3>

1. [SBH_Batch] Creates the sbh cache data for the downstream bbh analysis.
And this batch function is suitable with any scale of the blastp sbh data output.
**Prototype**: ``NCBI.localblast.CLI::Int32 VennCache(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /venn.cache /imports <blastp.DIR> [/out <sbh.out.DIR> /coverage <0.6> /identities <0.3> /num_threads <-1> /overrides]
```
###### Example
```bash
localblast
```



#### Parameters information:
##### [/num_threads]
The number of the sub process thread. -1 value is stands for auto config by the system.

###### Example
```bash

```
##### Accepted Types
###### /num_threads
<h3 id="/venn.sbh.thread"> 52. /venn.sbh.thread</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SBHThread(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /venn.sbh.thread /in <blastp.txt> [/out <out.sbh.csv> /coverage <0.6> /identities <0.3> /overrides]
```
###### Example
```bash
localblast
```
<h3 id="/Venn.Single"> 53. /Venn.Single</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 VennSingle(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Venn.Single /in <besthits.Xml> [/out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="--bbh.export"> 54. --bbh.export</h3>

Batch export bbh result data from a directory.
**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBBH(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --bbh.export /in <blast_out.DIR> [/all /out <out.DIR> /single-query <queryName> /coverage <0.5> /identities 0.15]
```
###### Example
```bash
localblast
```



#### Parameters information:
##### [/all]
If this all Boolean value is specific, then the program will export all hits for the bbh not the top 1 best.

###### Example
```bash

```
##### Accepted Types
###### /all
<h3 id="blast"> 55. blast</h3>

In order to draw as venn diagram for a specific set of genome and study the diferrence and consists between these genomes, you should do the blast operation from the protein amino aciad sequence first. The blastp operation can be performenced by the blast+ program which you can download from the NCBI website, this command is a interop service for the NCBI blast program£¬ you should install the blast+ program at first.
**Prototype**: ``NCBI.localblast.CLI::Int32 BLASTA(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast blast -i <file_directory> -blast_bin <BLAST_program_directory> -program <program_type_name> [-ld <log_dir> -xld <xml_log_dir>]
```
###### Example
```bash
localblast blast -i /home/xieguigang/GCModeller/examples/blast_test/ -ld ~/Desktop/logs -xld ~/Desktop/xmls -blast_bin ~/BLAST/bin
```



#### Parameters information:
##### -i
The input data directory which is contains the FASTA format protein amino acid sequence data file.

###### Example
```bash
~/Desktop/8004
```
##### -blast_bin
The localtion for the blast+ program, you should specific this switch value or this program will throw an exception.

###### Example
```bash
~/BLAST/bin
```
##### -program
The program type name for the NCBI local blast executable assembly.

###### Example
```bash
blast+
```
##### [-xld]
Optional, the parsed and well organized blast log file output directory, if this switch value is not specific by the user then the user desktop directoy will be used as default.

###### Example
```bash
~/Desktop/xml_logs
```
##### [-ld]
Optional, the blastp log file output directory for the NCBI blast+ program. If this switch value is not specific by the user then the user desktop directory will be specific for the logs file output as default.

###### Example
```bash
~/Desktop/logs/
```
##### Accepted Types
###### -i
###### -blast_bin
###### -program
###### -xld
###### -ld
<h3 id="--blast.self"> 56. --blast.self</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SelfBlast(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --blast.self /query <query.fasta> [/blast <blast_HOME> /out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="-copy"> 57. -copy</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 Copy(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast -copy -i <index_file> -os <output_saved> [-osidx <id_column_index> -os_skip_first <T/F>]
```
###### Example
```bash
localblast
```
<h3 id="--Export.Fasta"> 58. --Export.Fasta</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportFasta(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Export.Fasta /hits <query-hits.csv> /query <query.fasta> /subject <subject.fasta>
```
###### Example
```bash
localblast
```
<h3 id="--Export.Overviews"> 59. --Export.Overviews</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportOverviews(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Export.Overviews /blast <blastout.txt> [/out <overview.csv>]
```
###### Example
```bash
localblast
```
<h3 id="--Export.SBH"> 60. --Export.SBH</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportSBH(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Export.SBH /in <in.DIR> /prefix <queryName> /out <out.csv> [/txt]
```
###### Example
```bash
localblast
```
<h3 id="-export_besthit"> 61. -export_besthit</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBestHit(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast -export_besthit -i <input_csv_file> -o <output_saved_csv>
```
###### Example
```bash
localblast
```
<h3 id="grep"> 62. grep</h3>

The gene id in the blast output log file are not well format for reading and program processing, so before you generate the venn diagram you should call this command to parse the gene id from the log file. You can also done this id parsing job using other tools.
**Prototype**: ``NCBI.localblast.CLI::Int32 Grep(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast grep -i <xml_log_file> -q <script_statements> -h <script_statements>
```
###### Example
```bash
localblast grep -i C:\Users\WORKGROUP\Desktop\blast_xml_logs\1__8004_ecoli_prot.log.xml -q "tokens | 4" -h "'tokens | 2';'tokens ' ' 0'"
```



#### Parameters information:
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
'tokens | 5';'match .+[-].+'
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
'tokens | 5';'match .+[-].+'
```
##### Accepted Types
###### -q
###### -h
<h3 id="logs_analysis"> 63. logs_analysis</h3>

Parsing the xml format blast log into a csv data file that use for venn diagram drawing.
**Prototype**: ``NCBI.localblast.CLI::Int32 bLogAnalysis(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast logs_analysis -d <xml_logs_directory> -export <export_csv_file>
```
###### Example
```bash
localblast logs_analysis -d ~/xml_logs -export ~/Desktop/result.csv
```



#### Parameters information:
##### -d
The data directory which contains the xml format blast log file, those xml format log file were generated from the 'venn -> blast' command.

###### Example
```bash
~/xml_logs
```
##### -export
The save file path for the venn diagram drawing data csv file.

###### Example
```bash
~/Documents/8004_venn.csv
```
##### Accepted Types
###### -d
###### -export
<h3 id="merge"> 64. merge</h3>

This program can not use the blast parsing result for the venn diagram drawing operation, and this command is using for generate the drawing data for the venn diagram drawing command, this command merge the blast log parsing result and then using the parsing result for drawing a venn diagram.
**Prototype**: ``NCBI.localblast.CLI::Int32 Merge(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast merge -d <directory> -o <output_file>
```
###### Example
```bash
localblast merge -d ~/blast_besthit/ -o ~/Desktop/compared.csv
```



#### Parameters information:
##### -d
The directory that contains some blast log parsing csv data file.

###### Example
```bash
~/Desktop/blast/result/
```
##### -o
The save file name for the output result, the program willl save the merge result in the csv format

###### Example
```bash
~/Desktop/8004_venn.csv
```
##### Accepted Types
###### -d
###### -o
<h3 id="-merge_besthit"> 65. -merge_besthit</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 MergeBestHits(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast -merge_besthit -i <input_file_list> -o <output_file> -os <original_idlist_sequence_file> [-osidx <id_column_index> -os_skip_first <T/F>]
```
###### Example
```bash
localblast
```



#### Parameters information:
##### -i
Each file path in the filelist should be separated by a "|" character.

###### Example
```bash

```
##### Accepted Types
###### -i
<h3 id="--Xml2Excel"> 66. --Xml2Excel</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 XmlToExcel(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Xml2Excel /in <in.xml> [/out <out.csv>]
```
###### Example
```bash
localblast
```
<h3 id="--Xml2Excel.Batch"> 67. --Xml2Excel.Batch</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 XmlToExcelBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Xml2Excel.Batch /in <inDIR> [/out <outDIR> /Merge]
```
###### Example
```bash
localblast
```
