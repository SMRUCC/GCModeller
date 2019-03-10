---
title: Xfam
tags: [maunal, tools]
date: 5/28/2018 9:30:30 PM
---
# GCModeller [version 1.0.0.0]
> Xfam Tools (Pfam, Rfam, iPfam)

<!--more-->

**Xfam Tools (Pfam, Rfam, iPfam)**<br/>
_Xfam Tools (Pfam, Rfam, iPfam)_<br/>
Copyright © xie.guigang@gcmodeller.org 2015

**Module AssemblyName**: Xfam<br/>
**Root namespace**: ``Xfam.CLI``<br/>


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



**Prototype**: ``Xfam.CLI::Int32 ExportBlastn(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam /Export.Blastn /in <blastout.txt> [/out <blastn.Csv>]
```
<h3 id="/Export.Blastn.Batch"> 2. /Export.Blastn.Batch</h3>



**Prototype**: ``Xfam.CLI::Int32 ExportBlastns(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam /Export.Blastn.Batch /in <blastout.DIR> [/out outDIR /large /num_threads <-1> /no_parallel]
```
<h3 id="/Export.hmmscan"> 3. /Export.hmmscan</h3>



**Prototype**: ``Xfam.CLI::Int32 ExportHMMScan(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam /Export.hmmscan /in <input_hmmscan.txt> [/evalue 1e-5 /out <pfam.csv>]
```
<h3 id="/Export.hmmsearch"> 4. /Export.hmmsearch</h3>



**Prototype**: ``Xfam.CLI::Int32 ExportHMMSearch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam /Export.hmmsearch /in <input_hmmsearch.txt> [/prot <query.fasta> /out <pfam.csv>]
```
<h3 id="/Export.Pfam.UltraLarge"> 5. /Export.Pfam.UltraLarge</h3>



**Prototype**: ``Xfam.CLI::Int32 ExportUltraLarge(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam /Export.Pfam.UltraLarge /in <blastOUT.txt> [/out <out.csv> /evalue <0.00001> /coverage <0.85> /offset <0.1>]
```
<h3 id="/Load.cmscan"> 6. /Load.cmscan</h3>



**Prototype**: ``Xfam.CLI::Int32 LoadDoc(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam /Load.cmscan /in <stdout.txt> [/out <out.Xml>]
```
<h3 id="/Load.cmsearch"> 7. /Load.cmsearch</h3>



**Prototype**: ``Xfam.CLI::Int32 LoadCMSearch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam /Load.cmsearch /in <stdio.txt> /out <out.Xml>
```
<h3 id="/Rfam"> 8. /Rfam</h3>



**Prototype**: ``Xfam.CLI::Int32 RfamAnalysis(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam /Rfam /in <blastMappings.Csv.DIR> /PTT <pttDIR> [/prefix <sp_prefix> /out <out.Rfam.csv> /offset 10 /non-directed]
```


#### Arguments
##### [/prefix]
Optional for the custom RNA id, is this parameter value is nothing, then the id prefix will be parsed from the PTT file automaticslly.

###### Example
```bash
/prefix <term_string>
```
<h3 id="/Rfam.Align"> 9. /Rfam.Align</h3>



**Prototype**: ``Xfam.CLI::Int32 RfamAlignment(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam /Rfam.Align /query <sequence.fasta> [/rfam <DIR> /out <outDIR> /num_threads -1 /ticks 1000]
```


#### Arguments
##### [/formatdb]
If the /rfam directory parameter is specific and the database is not formatted, then this value should be TRUE for local blast.
If /rfam parameter is not specific, then the program will using the system database if it is exists, and the database is already be formatted as the installation of the database is includes this formation process.

###### Example
```bash
/formatdb <term_string>
```
<h3 id="/Rfam.GenomicsContext"> 10. /Rfam.GenomicsContext</h3>



**Prototype**: ``Xfam.CLI::Int32 RfamGenomicsContext(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam /Rfam.GenomicsContext /in <scan_sites.Csv> /PTT <genome.PTT> [/dist 500 /out <out.csv>]
```
<h3 id="/Rfam.Regulatory"> 11. /Rfam.Regulatory</h3>



**Prototype**: ``Xfam.CLI::Int32 RfamRegulatory(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam /Rfam.Regulatory /query <RfamilyMappings.csv> /mast <mastsites.csv> [/out <out.csv>]
```
<h3 id="/Rfam.Regulons"> 12. /Rfam.Regulons</h3>



**Prototype**: ``Xfam.CLI::Int32 RFamRegulons(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam /Rfam.Regulons /in <cmsearch.hits.csv> /regulons <regprecise.regulons.hits.csv> [/out <out.csv>]
```
<h3 id="/Rfam.SeedsDb.Dump"> 13. /Rfam.SeedsDb.Dump</h3>



**Prototype**: ``Xfam.CLI::Int32 DumpSeedsDb(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam /Rfam.SeedsDb.Dump /in <rfam.seed> [/out <rfam.csv>]
```
<h3 id="/Rfam.Sites.seq"> 14. /Rfam.Sites.seq</h3>



**Prototype**: ``Xfam.CLI::Int32 RfamSites(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam /Rfam.Sites.Seq /nt <nt.fasta> /sites <sites.csv> [/out out.fasta]
```
<h3 id="--Install.Rfam"> 15. --Install.Rfam</h3>



**Prototype**: ``Xfam.CLI::Int32 InstallRfam(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
Xfam --Install.Rfam /seed <rfam.seed>
```
