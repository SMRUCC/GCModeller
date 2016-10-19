---
title: Xfam
tags: [maunal, tools]
date: 2016/10/19 16:38:40
---
# GCModeller [version 1.0.0.0]
> Xfam Tools (Pfam, Rfam, iPfam)

<!--more-->

**Xfam Tools (Pfam, Rfam, iPfam)**
_Xfam Tools (Pfam, Rfam, iPfam)_
Copyright ? xie.guigang@gcmodeller.org 2015

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/Xfam.exe
**Root namespace**: ``Xfam.CLI``


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Export.Blastn](#/Export.Blastn)||
|[/Export.Blastn.Batch](#/Export.Blastn.Batch)||
|[/Export.hmmscan](#/Export.hmmscan)||
|[/Export.hmmsearch](#/Export.hmmsearch)||
|[/Export.Pfam.UltraLarge](#/Export.Pfam.UltraLarge)||
|[/Load.cmscan](#/Load.cmscan)||
|[/Load.cmsearch](#/Load.cmsearch)||
|[/Rfam](#/Rfam)||
|[/Rfam.Align](#/Rfam.Align)||
|[/Rfam.GenomicsContext](#/Rfam.GenomicsContext)||
|[/Rfam.Regulatory](#/Rfam.Regulatory)||
|[/Rfam.Regulons](#/Rfam.Regulons)||
|[/Rfam.SeedsDb.Dump](#/Rfam.SeedsDb.Dump)||
|[/Rfam.Sites.seq](#/Rfam.Sites.seq)||
|[--Install.Rfam](#--Install.Rfam)||




## CLI API list
--------------------------
<h3 id="/Export.Blastn"> 1. /Export.Blastn</h3>


**Prototype**: ``Xfam.CLI::Int32 ExportBlastn(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam /Export.Blastn /in <blastout.txt> [/out <blastn.Csv>]
```
###### Example
```bash
Xfam
```
<h3 id="/Export.Blastn.Batch"> 2. /Export.Blastn.Batch</h3>


**Prototype**: ``Xfam.CLI::Int32 ExportBlastns(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam /Export.Blastn.Batch /in <blastout.DIR> [/out outDIR /large /num_threads <-1> /no_parallel]
```
###### Example
```bash
Xfam
```
<h3 id="/Export.hmmscan"> 3. /Export.hmmscan</h3>


**Prototype**: ``Xfam.CLI::Int32 ExportHMMScan(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam /Export.hmmscan /in <input_hmmscan.txt> [/evalue 1e-5 /out <pfam.csv>]
```
###### Example
```bash
Xfam
```
<h3 id="/Export.hmmsearch"> 4. /Export.hmmsearch</h3>


**Prototype**: ``Xfam.CLI::Int32 ExportHMMSearch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam /Export.hmmsearch /in <input_hmmsearch.txt> [/prot <query.fasta> /out <pfam.csv>]
```
###### Example
```bash
Xfam
```
<h3 id="/Export.Pfam.UltraLarge"> 5. /Export.Pfam.UltraLarge</h3>


**Prototype**: ``Xfam.CLI::Int32 ExportUltraLarge(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam /Export.Pfam.UltraLarge /in <blastOUT.txt> [/out <out.csv> /evalue <0.00001> /coverage <0.85> /offset <0.1>]
```
###### Example
```bash
Xfam
```
<h3 id="/Load.cmscan"> 6. /Load.cmscan</h3>


**Prototype**: ``Xfam.CLI::Int32 LoadDoc(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam /Load.cmscan /in <stdout.txt> [/out <out.Xml>]
```
###### Example
```bash
Xfam
```
<h3 id="/Load.cmsearch"> 7. /Load.cmsearch</h3>


**Prototype**: ``Xfam.CLI::Int32 LoadCMSearch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam /Load.cmsearch /in <stdio.txt> /out <out.Xml>
```
###### Example
```bash
Xfam
```
<h3 id="/Rfam"> 8. /Rfam</h3>


**Prototype**: ``Xfam.CLI::Int32 RfamAnalysis(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam /Rfam /in <blastMappings.Csv.DIR> /PTT <pttDIR> [/prefix <sp_prefix> /out <out.Rfam.csv> /offset 10 /non-directed]
```
###### Example
```bash
Xfam
```



#### Parameters information:
##### [/prefix]
Optional for the custom RNA id, is this parameter value is nothing, then the id prefix will be parsed from the PTT file automaticslly.

###### Example
```bash

```
##### Accepted Types
###### /prefix
<h3 id="/Rfam.Align"> 9. /Rfam.Align</h3>


**Prototype**: ``Xfam.CLI::Int32 RfamAlignment(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam /Rfam.Align /query <sequence.fasta> [/rfam <DIR> /out <outDIR> /num_threads -1 /ticks 1000]
```
###### Example
```bash
Xfam
```



#### Parameters information:
##### [/formatdb]
If the /rfam directory parameter is specific and the database is not formatted, then this value should be TRUE for local blast.
If /rfam parameter is not specific, then the program will using the system database if it is exists, and the database is already be formatted as the installation of the database is includes this formation process.

###### Example
```bash

```
##### Accepted Types
###### /formatdb
<h3 id="/Rfam.GenomicsContext"> 10. /Rfam.GenomicsContext</h3>


**Prototype**: ``Xfam.CLI::Int32 RfamGenomicsContext(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam /Rfam.GenomicsContext /in <scan_sites.Csv> /PTT <genome.PTT> [/dist 500 /out <out.csv>]
```
###### Example
```bash
Xfam
```
<h3 id="/Rfam.Regulatory"> 11. /Rfam.Regulatory</h3>


**Prototype**: ``Xfam.CLI::Int32 RfamRegulatory(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam /Rfam.Regulatory /query <RfamilyMappings.csv> /mast <mastsites.csv> [/out <out.csv>]
```
###### Example
```bash
Xfam
```
<h3 id="/Rfam.Regulons"> 12. /Rfam.Regulons</h3>


**Prototype**: ``Xfam.CLI::Int32 RFamRegulons(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam /Rfam.Regulons /in <cmsearch.hits.csv> /regulons <regprecise.regulons.hits.csv> [/out <out.csv>]
```
###### Example
```bash
Xfam
```
<h3 id="/Rfam.SeedsDb.Dump"> 13. /Rfam.SeedsDb.Dump</h3>


**Prototype**: ``Xfam.CLI::Int32 DumpSeedsDb(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam /Rfam.SeedsDb.Dump /in <rfam.seed> [/out <rfam.csv>]
```
###### Example
```bash
Xfam
```
<h3 id="/Rfam.Sites.seq"> 14. /Rfam.Sites.seq</h3>


**Prototype**: ``Xfam.CLI::Int32 RfamSites(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam /Rfam.Sites.Seq /nt <nt.fasta> /sites <sites.csv> [/out out.fasta]
```
###### Example
```bash
Xfam
```
<h3 id="--Install.Rfam"> 15. --Install.Rfam</h3>


**Prototype**: ``Xfam.CLI::Int32 InstallRfam(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Xfam --Install.Rfam /seed <rfam.seed>
```
###### Example
```bash
Xfam
```
