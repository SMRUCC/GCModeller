---
title: localblast
tags: [maunal, tools]
date: 5/28/2018 8:37:21 PM
---
# GCModeller [version 1.0.0.0]
> Wrapper tools for the ncbi blast+ program and the blast output data analysis program. 
>                   For running a large scale parallel alignment task, using ``/venn.BlastAll`` command for ``blastp`` and ``/blastn.Query.All`` command for ``blastn``.

<!--more-->

**NCBI localblast wrapper tools**<br/>
_NCBI localblast_<br/>
Copyright © GPL3 2015

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/localblast.exe<br/>
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
|[/bbh.topbest](#/bbh.topbest)||
|[/COG.myva](#/COG.myva)|COG myva annotation using blastp raw output or exports sbh/bbh table result.|
|[/COG2014.result](#/COG2014.result)||
|[/Copy.Fasta](#/Copy.Fasta)|Copy target type files from different sub directory into a directory.|
|[/hits.ID.list](#/hits.ID.list)||
|[/Identities.Matrix](#/Identities.Matrix)||
|[/MAT.evalue](#/MAT.evalue)||
|[/Paralog](#/Paralog)||
|[/SBH.tophits](#/SBH.tophits)|Filtering the sbh result with top SBH Score|
|[/to.kobas](#/to.kobas)||
|[/UniProt.bbh.mappings](#/UniProt.bbh.mappings)||
|[/Whog.XML](#/Whog.XML)|Converts the whog text file into a XML data file.|
|[--bbh.export](#--bbh.export)|Batch export bbh result data from a directory.|
|[--blast.self](#--blast.self)|Query fasta query against itself for paralogs.|
|[--Export.Fasta](#--Export.Fasta)||
|[--Export.Overviews](#--Export.Overviews)||
|[--Export.SBH](#--Export.SBH)||
|[--Xml2Excel](#--Xml2Excel)||
|[--Xml2Excel.Batch](#--Xml2Excel.Batch)||


##### 1. Blastn alignment tools


|Function API|Info|
|------------|----|
|[/Blastn.Maps.Taxid](#/Blastn.Maps.Taxid)||
|[/blastn.Query](#/blastn.Query)|Using target fasta sequence query against all of the fasta sequence in target direcotry. This function is single thread.|
|[/blastn.Query.All](#/blastn.Query.All)|Using the fasta sequence in a directory query against all of the sequence in another directory.|
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


##### 2. Blastp BBH tools


|Function API|Info|
|------------|----|
|[/bbh.EXPORT](#/bbh.EXPORT)|Export bbh mapping result from the blastp raw output.|
|[/BBH.Merge](#/BBH.Merge)||
|[/Blastp.BBH.Query](#/Blastp.BBH.Query)|Using query fasta invoke blastp against the fasta files in a directory.
               * This command tools required of NCBI blast+ suite, you must config the blast bin path by using ``settings.exe`` before running this command.|
|[/Export.Locus](#/Export.Locus)||
|[/Fasta.Filters](#/Fasta.Filters)|Filter the fasta sequence subset from a larger fasta database by using the regexp for match on the fasta title.|
|[/locus.Selects](#/locus.Selects)||
|[/SBH.BBH.Batch](#/SBH.BBH.Batch)||
|[/SBH.Export.Large](#/SBH.Export.Large)|Using this command for export the sbh result of your blastp raw data.|
|[/SBH.Trim](#/SBH.Trim)||
|[/sbh2bbh](#/sbh2bbh)|Export bbh result from the sbh pairs.|
|[/Select.Meta](#/Select.Meta)||
|[/venn.BBH](#/venn.BBH)|2. Build venn table And bbh data from the blastp result out Or sbh data cache.|
|[/venn.BlastAll](#/venn.BlastAll)|Completely paired combos blastp bbh operations for the venn diagram Or network builder.|
|[/venn.cache](#/venn.cache)|1. [SBH_Batch] Creates the sbh cache data for the downstream bbh analysis. 
    And this batch function is suitable with any scale of the blastp sbh data output.|
|[/venn.sbh.thread](#/venn.sbh.thread)||


##### 3. COG annotation tools


|Function API|Info|
|------------|----|
|[/COG.Statics](#/COG.Statics)|Statics the COG profiling in your analysised genome.|
|[/EXPORT.COGs.from.DOOR](#/EXPORT.COGs.from.DOOR)||
|[/install.cog2003-2014](#/install.cog2003-2014)|Config the ``prot2003-2014.fasta`` database for GCModeller localblast tools. This database will be using for the COG annotation. 
               This command required of the blast+ install first.|
|[/query.cog2003-2014](#/query.cog2003-2014)|Protein COG annotation by using NCBI cog2003-2014.fasta database.|


##### 4. NCBI genbank tools


|Function API|Info|
|------------|----|
|[/add.locus_tag](#/add.locus_tag)|Add locus_tag qualifier into the feature slot.|
|[/add.names](#/add.names)||
|[/Copy.PTT](#/Copy.PTT)||
|[/Copys](#/Copys)||
|[/Export.BlastX](#/Export.BlastX)|Export the blastx alignment result into a csv table.|
|[/Export.gb](#/Export.gb)|Export the *.fna, *.faa, *.ptt file from the gbk file.|
|[/Export.gb.genes](#/Export.gb.genes)||
|[/Export.gpff](#/Export.gpff)||
|[/Export.gpffs](#/Export.gpffs)||
|[/Export.Protein](#/Export.Protein)|Export all of the protein sequence from the genbank database file.|
|[/Merge.faa](#/Merge.faa)||
|[/Print](#/Print)||


##### 5. NCBI taxonomy tools


|Function API|Info|
|------------|----|
|[/Reads.OTU.Taxonomy](#/Reads.OTU.Taxonomy)|If the blastnmapping data have the duplicated OTU tags, then this function will makes a copy of the duplicated OTU tag data. top-best data will not.|
|[/ref.acc.list](#/ref.acc.list)||
|[/ref.gi.list](#/ref.gi.list)||


##### 6. NCBI Web Blast Tools


|Function API|Info|
|------------|----|
|[/AlignmentTable.TopBest](#/AlignmentTable.TopBest)||
|[/Export.AlignmentTable](#/Export.AlignmentTable)||
|[/Export.AlignmentTable.giList](#/Export.AlignmentTable.giList)||
|[/Taxonomy.efetch](#/Taxonomy.efetch)|Fetch the taxonomy information of the fasta sequence from NCBI web server.|
|[/Taxonomy.efetch.Merge](#/Taxonomy.efetch.Merge)||

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
<h3 id="/bbh.EXPORT"> 5. /bbh.EXPORT</h3>

Export bbh mapping result from the blastp raw output.
**Prototype**: ``NCBI.localblast.CLI::Int32 BBHExportFile(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /bbh.EXPORT /query <query.blastp_out> /subject <subject.blast_out> [/trim /out <bbh.csv> /evalue 1e-3 /coverage 0.85 /identities 0.3]
```
<h3 id="/BBH.Merge"> 6. /BBH.Merge</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 MergeBBH(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /BBH.Merge /in <inDIR> [/out <out.csv>]
```
<h3 id="/bbh.topbest"> 7. /bbh.topbest</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 BBHTopBest(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /bbh.topbest /in <bbh.csv> [/out <out.csv>]
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

Using target fasta sequence query against all of the fasta sequence in target direcotry. This function is single thread.
**Prototype**: ``NCBI.localblast.CLI::Int32 BlastnQuery(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /blastn.Query /query <query.fna/faa> /db <db.DIR> [/thread /evalue 1e-5 /word_size <-1> /out <out.DIR>]
```


#### Arguments
##### [/thread]
Is this CLI api running in one of the processor in thread mode for a caller API ``/blastn.Query.All``

###### Example
```bash
/thread
#(boolean flag does not require of argument value)
```
<h3 id="/blastn.Query.All"> 10. /blastn.Query.All</h3>

Using the fasta sequence in a directory query against all of the sequence in another directory.
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
<h3 id="/COG.myva"> 17. /COG.myva</h3>

COG myva annotation using blastp raw output or exports sbh/bbh table result.
**Prototype**: ``NCBI.localblast.CLI::Int32 COG_myva(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /COG.myva /blastp <blastp.myva.txt/sbh.csv> /whog <whog.XML> [/simple /out <out.csv/txt>]
```
<h3 id="/COG.Statics"> 18. /COG.Statics</h3>

Statics the COG profiling in your analysised genome.
**Prototype**: ``NCBI.localblast.CLI::Int32 COGStatics(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /COG.Statics /in <myva_cogs.csv> [/locus <locus.txt/csv> /locuMap <Gene> /out <out.csv>]
```
<h3 id="/COG2014.result"> 19. /COG2014.result</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 COG2014_result(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /COG2014.result /sbh <query-vs-COG2003-2014.fasta> /cog <cog2003-2014.csv> [/cog.names <cognames2003-2014.tab> /out <out.myva_cog.csv>]
```
<h3 id="/Copy.Fasta"> 20. /Copy.Fasta</h3>

Copy target type files from different sub directory into a directory.
**Prototype**: ``NCBI.localblast.CLI::Int32 CopyFasta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Copy.Fasta /imports <DIR> [/type <faa,fna,ffn,fasta,...., default:=faa> /out <DIR>]
```
<h3 id="/Copy.PTT"> 21. /Copy.PTT</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 CopyPTT(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Copy.PTT /in <inDIR> [/out <outDIR>]
```
<h3 id="/Copys"> 22. /Copys</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 Copys(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Copys /imports <DIR> [/out <outDIR>]
```
<h3 id="/Export.AlignmentTable"> 23. /Export.AlignmentTable</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportWebAlignmentTable(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.AlignmentTable /in <alignment.txt> [/split /header.split /out <outDIR/file>]
```
<h3 id="/Export.AlignmentTable.giList"> 24. /Export.AlignmentTable.giList</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ParseAlignmentTableGIlist(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.AlignmentTable.giList /in <table.csv> [/out <gi.txt>]
```
<h3 id="/Export.Blastn"> 25. /Export.Blastn</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastn(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.Blastn /in <in.txt> [/out <out.csv>]
```
<h3 id="/Export.blastnMaps"> 26. /Export.blastnMaps</h3>


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

<h3 id="/Export.blastnMaps.Batch"> 27. /Export.blastnMaps.Batch</h3>

Multiple processor task.
**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastnMapsBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.blastnMaps.Batch /in <blastn_out.DIR> [/best /out <out.DIR> /num_threads <-1>]
```
<h3 id="/Export.blastnMaps.littles"> 28. /Export.blastnMaps.littles</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastnMapsSmall(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.blastnMaps.littles /in <blastn.txt.DIR> [/out <out.csv.DIR>]
```
<h3 id="/Export.blastnMaps.Write"> 29. /Export.blastnMaps.Write</h3>

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
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### [/out]
Blastn alignment maps data.

###### Example
```bash
/out <file/directory>
```
##### [/best]
Only export the top best blastn alignment hit?

###### Example
```bash
/best
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /in
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

###### /out
**Decalre**:  _SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BlastnMapping_
Example: 
```json
null
```

###### /best
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="/Export.BlastX"> 30. /Export.BlastX</h3>

Export the blastx alignment result into a csv table.
**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBlastX(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.BlastX /in <blastx.txt> [/top /Uncharacterized.exclude /out <out.csv>]
```


#### Arguments
##### /in
The text file content output from the blastx command in NCBI blast+ suite.

###### Example
```bash
/in <file, *.txt>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### [/top]
Only output the top first alignment result? Default is not.

###### Example
```bash
/top
#(boolean flag does not require of argument value)
```
<h3 id="/EXPORT.COGs.from.DOOR"> 31. /EXPORT.COGs.from.DOOR</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportDOORCogs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /EXPORT.COGs.from.DOOR /in <DOOR.opr> [/out <out.csv>]
```
<h3 id="/Export.gb"> 32. /Export.gb</h3>

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

<h3 id="/Export.gb.genes"> 33. /Export.gb.genes</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportGenesFasta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.gb.genes /gb <genbank.gb> [/geneName /out <out.fasta>]
```


#### Arguments
##### [/geneName]
If this parameter is specific as True, then this function will try using geneName as the fasta sequence title, or using locus_tag value as default.

###### Example
```bash
/geneName
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /geneName
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="/Export.gpff"> 34. /Export.gpff</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 EXPORTgpff(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.gpff /in <genome.gpff> /gff <genome.gff> [/out <out.PTT>]
```
<h3 id="/Export.gpffs"> 35. /Export.gpffs</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 EXPORTgpffs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.gpffs [/in <inDIR>]
```
<h3 id="/Export.Locus"> 36. /Export.Locus</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportLocus(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.Locus /in <sbh/bbh_DIR> [/hit /out <out.txt>]
```
<h3 id="/Export.Protein"> 37. /Export.Protein</h3>

Export all of the protein sequence from the genbank database file.
**Prototype**: ``NCBI.localblast.CLI::Int32 ExportProt(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Export.Protein /gb <genome.gb> [/out <out.fasta>]
```
<h3 id="/Fasta.Filters"> 38. /Fasta.Filters</h3>

Filter the fasta sequence subset from a larger fasta database by using the regexp for match on the fasta title.
**Prototype**: ``NCBI.localblast.CLI::Int32 Filter(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Fasta.Filters /in <nt.fasta> /key <regex/list.txt> [/tokens /out <out.fasta> /p]
```


#### Arguments
##### /key
A regexp string term that will be using for title search or file path of a text file contains lines of regexp.

###### Example
```bash
/key <file/directory>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### [/p]
Using the parallel edition?? If GCModeller running in a 32bit environment, do not use this option. This option only works in single key mode.

###### Example
```bash
/p <term_string>
```
##### Accepted Types
###### /key
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

**Decalre**:  _System.String[]_
Example: 
```json
[
    "System.String"
]
```

###### /p
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="/hits.ID.list"> 39. /hits.ID.list</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 HitsIDList(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /hits.ID.list /in <bbhindex.csv> [/out <out.txt>]
```
<h3 id="/Identities.Matrix"> 40. /Identities.Matrix</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 IdentitiesMAT(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Identities.Matrix /hit <sbh/bbh.csv> [/out <out.csv> /cut 0.65]
```
<h3 id="/install.cog2003-2014"> 41. /install.cog2003-2014</h3>

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

<h3 id="/locus.Selects"> 42. /locus.Selects</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 LocusSelects(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /locus.Selects /locus <locus.txt> /bh <bbhindex.csv> [/out <out.csv>]
```
<h3 id="/MAT.evalue"> 43. /MAT.evalue</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 EvalueMatrix(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /MAT.evalue /in <sbh.csv> [/out <mat.csv> /flip]
```
<h3 id="/Merge.faa"> 44. /Merge.faa</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 MergeFaa(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Merge.faa /in <DIR> /out <out.fasta>
```
<h3 id="/Paralog"> 45. /Paralog</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportParalog(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Paralog /blastp <blastp.txt> [/coverage 0.5 /identities 0.3 /out <out.csv>]
```
<h3 id="/Print"> 46. /Print</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 Print(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Print /in <inDIR> [/ext <ext> /out <out.Csv>]
```
<h3 id="/query.cog2003-2014"> 47. /query.cog2003-2014</h3>

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
##### [/out]
The output directory for the work files.

###### Example
```bash
/out <file/directory>
```
##### [/evalue]
blastp e-value cutoff.

###### Example
```bash
/evalue <float>
```
##### [/all]
For export the bbh result, export all match or only the top best? default is only top best.

###### Example
```bash
/all
#(boolean flag does not require of argument value)
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

###### /out
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

###### /evalue
**Decalre**:  _System.Double_
Example: 
```json
0
```

###### /all
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="/Reads.OTU.Taxonomy"> 48. /Reads.OTU.Taxonomy</h3>

If the blastnmapping data have the duplicated OTU tags, then this function will makes a copy of the duplicated OTU tag data. top-best data will not.
**Prototype**: ``NCBI.localblast.CLI::Int32 ReadsOTU_Taxonomy(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Reads.OTU.Taxonomy /in <blastnMaps.csv> /OTU <OTU_data.csv> /tax <taxonomy:nodes/names> [/fill.empty /out <out.csv>]
```


#### Arguments
##### /in
This input data should have a column named ``taxid`` for the taxonomy information.

###### Example
```bash
/in <file/directory>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### /OTU

###### Example
```bash
/OTU <term_string>
```
##### [/fill.empty]
If this options is true, then this function will only fill the rows which have an empty ``Taxonomy`` field column.

###### Example
```bash
/fill.empty <term_string>
```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BlastnMapping_
Example: 
```json
null
```

###### /OTU
**Decalre**:  _SMRUCC.genomics.Metagenomics.OTUData_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "OTU": "System.String",
    "Taxonomy": "System.String"
}
```

###### /fill.empty
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="/ref.acc.list"> 49. /ref.acc.list</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 AccessionList(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /ref.acc.list /in <blastnMaps.csv/DIR> [/out <out.csv>]
```
<h3 id="/ref.gi.list"> 50. /ref.gi.list</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 GiList(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /ref.gi.list /in <blastnMaps.csv/DIR> [/out <out.csv>]
```
<h3 id="/SBH.BBH.Batch"> 51. /SBH.BBH.Batch</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SBH_BBH_Batch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SBH.BBH.Batch /in <sbh.DIR> [/identities <-1> /coverage <-1> /all /out <bbh.DIR> /num_threads <-1>]
```
<h3 id="/SBH.Export.Large"> 52. /SBH.Export.Large</h3>

Using this command for export the sbh result of your blastp raw data.
**Prototype**: ``NCBI.localblast.CLI::Int32 ExportBBHLarge(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SBH.Export.Large /in <blastp_out.txt> [/trim-kegg /out <sbh.csv> /s.pattern <default=-> /q.pattern <default=-> /identities 0.15 /coverage 0.5]
```


#### Arguments
##### /in
The blastp raw result input file path.

###### Example
```bash
/in <file/directory>
```
##### [/out]
The sbh result output csv file location.

###### Example
```bash
/out <file/directory>
```
##### [/trim-KEGG]
If the fasta sequence source is comes from the KEGG database, and you want to removes the kegg species brief code for the locus_tag, then enable this option.

###### Example
```bash
/trim-KEGG
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /in
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

###### /out
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

<h3 id="/SBH.tophits"> 53. /SBH.tophits</h3>

Filtering the sbh result with top SBH Score
**Prototype**: ``NCBI.localblast.CLI::Int32 SBH_topHits(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SBH.tophits /in <sbh.csv> [/uniprotKB /out <out.csv>]
```
<h3 id="/SBH.Trim"> 54. /SBH.Trim</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SBHTrim(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /SBH.Trim /in <sbh.csv> /evalue <evalue> [/identities 0.15 /coverage 0.5 /out <out.csv>]
```
<h3 id="/sbh2bbh"> 55. /sbh2bbh</h3>

Export bbh result from the sbh pairs.
**Prototype**: ``NCBI.localblast.CLI::Int32 BBHExport2(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /sbh2bbh /qvs <qvs.sbh.csv> /svq <svq.sbh.csv> [/trim /identities <-1> /coverage <-1> /all /out <bbh.csv>]
```


#### Arguments
##### [/identities]
Makes a further filtering on the bbh by using this option, default value is -1, so that this means no filter.

###### Example
```bash
/identities <float>
```
##### [/coverage]
Makes a further filtering on the bbh by using this option, default value is -1, so that this means no filter.

###### Example
```bash
/coverage <float>
```
##### [/trim]
If this option was enabled, then the queryName and hitname will be trimed by using space and the first token was taken as the name ID.

###### Example
```bash
/trim
#(boolean flag does not require of argument value)
```
<h3 id="/Select.Meta"> 56. /Select.Meta</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SelectsMeta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Select.Meta /in <meta.Xml> /bbh <bbh.csv> [/out <out.csv>]
```
<h3 id="/Taxonomy.efetch"> 57. /Taxonomy.efetch</h3>

Fetch the taxonomy information of the fasta sequence from NCBI web server.
**Prototype**: ``NCBI.localblast.CLI::Int32 FetchTaxnData(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Taxonomy.efetch /in <nt.fasta> [/out <out.DIR>]
```
<h3 id="/Taxonomy.efetch.Merge"> 58. /Taxonomy.efetch.Merge</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 MergeFetchTaxonData(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Taxonomy.efetch.Merge /in <in.DIR> [/out <out.Csv>]
```
<h3 id="/to.kobas"> 59. /to.kobas</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 _2_KOBASOutput(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /to.kobas /in <sbh.csv> [/out <kobas.tsv>]
```
<h3 id="/UniProt.bbh.mappings"> 60. /UniProt.bbh.mappings</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 UniProtBBHMapTable(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /UniProt.bbh.mappings /in <bbh.csv> [/reverse /out <mappings.txt>]
```
<h3 id="/venn.BBH"> 61. /venn.BBH</h3>

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
<h3 id="/venn.BlastAll"> 62. /venn.BlastAll</h3>

Completely paired combos blastp bbh operations for the venn diagram Or network builder.
**Prototype**: ``NCBI.localblast.CLI::Int32 vennBlastAll(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /venn.BlastAll /query <queryDIR> [/out <outDIR> /num_threads <-1> /evalue 10 /overrides /all /coverage <0.8> /identities <0.3>]
```


#### Arguments
##### /query
Recommended format of the fasta title Is that the fasta title only contains gene locus_tag.

###### Example
```bash
/query <term_string>
```
##### [/num_threads]
The number of the parallel blast task in this command, set this argument ZERO for single thread. default value Is -1 which means the number of the blast threads Is determined by system automatically.

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
<h3 id="/venn.cache"> 63. /venn.cache</h3>

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
<h3 id="/venn.sbh.thread"> 64. /venn.sbh.thread</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 SBHThread(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /venn.sbh.thread /in <blastp.txt> [/out <out.sbh.csv> /coverage <0.6> /identities <0.3> /overrides]
```
<h3 id="/Whog.XML"> 65. /Whog.XML</h3>

Converts the whog text file into a XML data file.
**Prototype**: ``NCBI.localblast.CLI::Int32 WhogXML(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast /Whog.XML /in <whog> [/out <whog.XML>]
```
<h3 id="--bbh.export"> 66. --bbh.export</h3>

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
<h3 id="--blast.self"> 67. --blast.self</h3>

Query fasta query against itself for paralogs.
**Prototype**: ``NCBI.localblast.CLI::Int32 SelfBlast(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --blast.self /query <query.fasta> [/blast <blast_HOME> /out <out.csv>]
```
<h3 id="--Export.Fasta"> 68. --Export.Fasta</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportFasta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Export.Fasta /hits <query-hits.csv> /query <query.fasta> /subject <subject.fasta>
```
<h3 id="--Export.Overviews"> 69. --Export.Overviews</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportOverviews(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Export.Overviews /blast <blastout.txt> [/out <overview.csv>]
```
<h3 id="--Export.SBH"> 70. --Export.SBH</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 ExportSBH(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Export.SBH /in <in.DIR> /prefix <queryName> /out <out.csv> [/txt]
```
<h3 id="--Xml2Excel"> 71. --Xml2Excel</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 XmlToExcel(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Xml2Excel /in <in.xml> [/out <out.csv>]
```
<h3 id="--Xml2Excel.Batch"> 72. --Xml2Excel.Batch</h3>


**Prototype**: ``NCBI.localblast.CLI::Int32 XmlToExcelBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
localblast --Xml2Excel.Batch /in <inDIR> [/out <outDIR> /Merge]
```
