---
title: Xfam
tags: [maunal, tools]
date: 7/7/2016 6:52:20 PM
---
# GCModeller [version 1.0.0.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/Xfam.exe
**Root namespace**: Xfam.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/Export.Blastn||
|/Export.Blastn.Batch||
|/Export.hmmscan||
|/Export.hmmsearch||
|/Export.Pfam.UltraLarge||
|/Load.cmscan||
|/Load.cmsearch||
|/Rfam||
|/Rfam.Align||
|/Rfam.GenomicsContext||
|/Rfam.Regulatory||
|/Rfam.Regulons||
|/Rfam.SeedsDb.Dump||
|/Rfam.Sites.seq||
|--Install.Rfam||

## Commands
--------------------------
##### Help for command '/Export.Blastn':

**Prototype**: Xfam.CLI::Int32 ExportBlastn(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe /Export.Blastn /in <blastout.txt> [/out <blastn.Csv>]
  Example:      Xfam /Export.Blastn 
```

##### Help for command '/Export.Blastn.Batch':

**Prototype**: Xfam.CLI::Int32 ExportBlastns(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe /Export.Blastn.Batch /in <blastout.DIR> [/out outDIR /large /num_threads <-1> /no_parallel]
  Example:      Xfam /Export.Blastn.Batch 
```

##### Help for command '/Export.hmmscan':

**Prototype**: Xfam.CLI::Int32 ExportHMMScan(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe /Export.hmmscan /in <input_hmmscan.txt> [/evalue 1e-5 /out <pfam.csv>]
  Example:      Xfam /Export.hmmscan 
```

##### Help for command '/Export.hmmsearch':

**Prototype**: Xfam.CLI::Int32 ExportHMMSearch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe /Export.hmmsearch /in <input_hmmsearch.txt> [/prot <query.fasta> /out <pfam.csv>]
  Example:      Xfam /Export.hmmsearch 
```

##### Help for command '/Export.Pfam.UltraLarge':

**Prototype**: Xfam.CLI::Int32 ExportUltraLarge(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe /Export.Pfam.UltraLarge /in <blastOUT.txt> [/out <out.csv> /evalue <0.00001> /coverage <0.85> /offset <0.1>]
  Example:      Xfam /Export.Pfam.UltraLarge 
```

##### Help for command '/Load.cmscan':

**Prototype**: Xfam.CLI::Int32 LoadDoc(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe /Load.cmscan /in <stdout.txt> [/out <out.Xml>]
  Example:      Xfam /Load.cmscan 
```

##### Help for command '/Load.cmsearch':

**Prototype**: Xfam.CLI::Int32 LoadCMSearch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe /Load.cmsearch /in <stdio.txt> /out <out.Xml>
  Example:      Xfam /Load.cmsearch 
```

##### Help for command '/Rfam':

**Prototype**: Xfam.CLI::Int32 RfamAnalysis(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe /Rfam /in <blastMappings.Csv.DIR> /PTT <pttDIR> [/prefix <sp_prefix> /out <out.Rfam.csv> /offset 10 /non-directed]
  Example:      Xfam /Rfam 
```



  Parameters information:
```
       [/prefix]
    Description:  Optional for the custom RNA id, is this parameter value is nothing, then the id prefix will be parsed from the PTT file automaticslly.

    Example:      /prefix ""


```

#### Accepted Types
##### /prefix
##### Help for command '/Rfam.Align':

**Prototype**: Xfam.CLI::Int32 RfamAlignment(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe /Rfam.Align /query <sequence.fasta> [/rfam <DIR> /out <outDIR> /num_threads -1 /ticks 1000]
  Example:      Xfam /Rfam.Align 
```



  Parameters information:
```
       [/formatdb]
    Description:  If the /rfam directory parameter is specific and the database is not formatted, then this value should be TRUE for local blast. 
                   If /rfam parameter is not specific, then the program will using the system database if it is exists, and the database is already be formatted as the installation of the database is includes this formation process.

    Example:      /formatdb ""


```

#### Accepted Types
##### /formatdb
##### Help for command '/Rfam.GenomicsContext':

**Prototype**: Xfam.CLI::Int32 RfamGenomicsContext(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe /Rfam.GenomicsContext /in <scan_sites.Csv> /PTT <genome.PTT> [/dist 500 /out <out.csv>]
  Example:      Xfam /Rfam.GenomicsContext 
```

##### Help for command '/Rfam.Regulatory':

**Prototype**: Xfam.CLI::Int32 RfamRegulatory(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe /Rfam.Regulatory /query <RfamilyMappings.csv> /mast <mastsites.csv> [/out <out.csv>]
  Example:      Xfam /Rfam.Regulatory 
```

##### Help for command '/Rfam.Regulons':

**Prototype**: Xfam.CLI::Int32 RFamRegulons(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe /Rfam.Regulons /in <cmsearch.hits.csv> /regulons <regprecise.regulons.hits.csv> [/out <out.csv>]
  Example:      Xfam /Rfam.Regulons 
```

##### Help for command '/Rfam.SeedsDb.Dump':

**Prototype**: Xfam.CLI::Int32 DumpSeedsDb(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe /Rfam.SeedsDb.Dump /in <rfam.seed> [/out <rfam.csv>]
  Example:      Xfam /Rfam.SeedsDb.Dump 
```

##### Help for command '/Rfam.Sites.seq':

**Prototype**: Xfam.CLI::Int32 RfamSites(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe /Rfam.Sites.Seq /nt <nt.fasta> /sites <sites.csv> [/out out.fasta]
  Example:      Xfam /Rfam.Sites.seq 
```

##### Help for command '--Install.Rfam':

**Prototype**: Xfam.CLI::Int32 InstallRfam(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Xfam.exe --Install.Rfam /seed <rfam.seed>
  Example:      Xfam --Install.Rfam 
```

