---
title: KEGG
tags: [maunal, tools]
date: 7/7/2016 6:51:37 PM
---
# GCModeller [version 3.0.854.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/KEGG.exe
**Root namespace**: KEGG.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/16S_rRNA||
|/blastn|Blastn analysis of your DNA sequence on KEGG server for the functional analysis.|
|/Compile.Model|KEGG pathway model compiler|
|/Download.Ortholog|Downloads the KEGG gene ortholog annotation data from the web server.|
|/Dump.sp||
|/Fasta.By.Sp||
|/Get.prot_motif||
|/Gets.prot_motif||
|/Imports.SSDB||
|/Pathways.Downloads.All||
|/Pull.Seq|Downloads the missing sequence in the local KEGG database from the KEGG database server.|
|/Query.KO||
|/Views.mod_stat||
|-Build.KO|Download data from KEGG database to local server.|
|Download.Sequence||
|--Dump.Db||
|--Export.KO||
|-function.association.analysis||
|--Get.KO||
|--part.from|source and ref should be in KEGG annotation format.|
|-query|Query the KEGG database for nucleotide sequence and protein sequence by using a keywork.|
|-query.orthology||
|-query.ref.map||
|-ref.map.download||
|-Table.Create||

## Commands
--------------------------
##### Help for command '/16S_rRNA':

**Prototype**: KEGG.CLI::Int32 Download16SRNA(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe /16s_rna [/out <outDIR>]
  Example:      KEGG /16S_rRNA 
```

##### Help for command '/blastn':

**Prototype**: KEGG.CLI::Int32 Blastn(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Blastn analysis of your DNA sequence on KEGG server for the functional analysis.
  Usage:        G:\GCModeller\manual\bin\KEGG.exe /blastn /query <query.fasta> [/out <outDIR>]
  Example:      KEGG /blastn 
```

##### Help for command '/Compile.Model':

**Prototype**: KEGG.CLI::Int32 Compile(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  KEGG pathway model compiler
  Usage:        G:\GCModeller\manual\bin\KEGG.exe /Compile.Model /pathway <pathwayDIR> /mods <modulesDIR> /sp <sp_code> [/out <out.Xml>]
  Example:      KEGG /Compile.Model 
```

##### Help for command '/Download.Ortholog':

**Prototype**: KEGG.CLI::Int32 DownloadOrthologs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Downloads the KEGG gene ortholog annotation data from the web server.
  Usage:        G:\GCModeller\manual\bin\KEGG.exe /Download.Ortholog -i <gene_list_file.txt/gbk> -export <exportedDIR> [/gbk /sp <KEGG.sp>]
  Example:      KEGG /Download.Ortholog 
```

##### Help for command '/Dump.sp':

**Prototype**: KEGG.CLI::Int32 DumpOrganisms(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe /Dump.sp [/res sp.html /out <out.csv>]
  Example:      KEGG /Dump.sp 
```

##### Help for command '/Fasta.By.Sp':

**Prototype**: KEGG.CLI::Int32 GetFastaBySp(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe /Fasta.By.Sp /in <KEGG.fasta> /sp <sp.list> [/out <out.fasta>]
  Example:      KEGG /Fasta.By.Sp 
```

##### Help for command '/Get.prot_motif':

**Prototype**: KEGG.CLI::Int32 ProteinMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe /Get.prot_motif /query <sp:locus> [/out out.json]
  Example:      KEGG /Get.prot_motif 
```

##### Help for command '/Gets.prot_motif':

**Prototype**: KEGG.CLI::Int32 GetsProteinMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe /Gets.prot_motif /query <query.txt/genome.PTT> [/PTT /sp <kegg-sp> /out <out.json> /update]
  Example:      KEGG /Gets.prot_motif 
```

##### Help for command '/Imports.SSDB':

**Prototype**: KEGG.CLI::Int32 ImportsDb(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe /Imports.SSDB /in <source.DIR> [/out <ssdb.csv>]
  Example:      KEGG /Imports.SSDB 
```

##### Help for command '/Pathways.Downloads.All':

**Prototype**: KEGG.CLI::Int32 DownloadsAllPathways(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe /Pathways.Downloads.All [/out <outDIR>]
  Example:      KEGG /Pathways.Downloads.All 
```

##### Help for command '/Pull.Seq':

**Prototype**: KEGG.CLI::Int32 PullSequence(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Downloads the missing sequence in the local KEGG database from the KEGG database server.
  Usage:        G:\GCModeller\manual\bin\KEGG.exe 
  Example:      KEGG /Pull.Seq 
```

##### Help for command '/Query.KO':

**Prototype**: KEGG.CLI::Int32 QueryKOAnno(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe /Query.KO /in <blastnhits.csv> [/out <out.csv> /evalue 1e-5 /batch]
  Example:      KEGG /Query.KO 
```

##### Help for command '/Views.mod_stat':

**Prototype**: KEGG.CLI::Int32 Stats(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe /Views.mod_stat /in <KEGG_Modules/Pathways_DIR> /locus <in.csv> [/locus_map Gene /pathway /out <out.csv>]
  Example:      KEGG /Views.mod_stat 
```

##### Help for command '-Build.KO':

**Prototype**: KEGG.CLI::Int32 BuildKEGGOrthology(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Download data from KEGG database to local server.
  Usage:        G:\GCModeller\manual\bin\KEGG.exe -Build.KO [/fill-missing]
  Example:      KEGG -Build.KO 
```

##### Help for command 'Download.Sequence':

**Prototype**: KEGG.CLI::Int32 DownloadSequence(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe Download.Sequence /query <querySource.txt> [/out <outDIR> /source <existsDIR>]
  Example:      KEGG Download.Sequence 
```

##### Help for command '--Dump.Db':

**Prototype**: KEGG.CLI::Int32 DumpDb(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe --Dump.Db /KEGG.Pathways <DIR> /KEGG.Modules <DIR> /KEGG.Reactions <DIR> /sp <sp.Code> /out <out.Xml>
  Example:      KEGG --Dump.Db 
```

##### Help for command '--Export.KO':

**Prototype**: KEGG.CLI::Int32 ExportKO(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe 
  Example:      KEGG --Export.KO 
```

##### Help for command '-function.association.analysis':

**Prototype**: KEGG.CLI::Int32 FunctionAnalysis(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe -function.association.analysis -i <matrix_csv>
  Example:      KEGG -function.association.analysis 
```

##### Help for command '--Get.KO':

**Prototype**: KEGG.CLI::Int32 GetKOAnnotation(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe --Get.KO /in <KASS-query.txt>
  Example:      KEGG --Get.KO 
```

##### Help for command '--part.from':

**Prototype**: KEGG.CLI::Int32 GetSource(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  source and ref should be in KEGG annotation format.
  Usage:        G:\GCModeller\manual\bin\KEGG.exe --part.from /source <source.fasta> /ref <referenceFrom.fasta> [/out <out.fasta> /brief]
  Example:      KEGG --part.from 
```

##### Help for command '-query':

**Prototype**: KEGG.CLI::Int32 QueryGenes(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Query the KEGG database for nucleotide sequence and protein sequence by using a keywork.
  Usage:        G:\GCModeller\manual\bin\KEGG.exe -query -keyword <keyword> -o <out_dir>
  Example:      KEGG -query 
```

##### Help for command '-query.orthology':

**Prototype**: KEGG.CLI::Int32 QueryOrthology(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe -query.orthology -keyword <gene_name> -o <output_csv>
  Example:      KEGG -query.orthology 
```

##### Help for command '-query.ref.map':

**Prototype**: KEGG.CLI::Int32 DownloadReferenceMap(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe -query.ref.map -id <id> -o <out_dir>
  Example:      KEGG -query.ref.map 
```

##### Help for command '-ref.map.download':

**Prototype**: KEGG.CLI::Int32 DownloadReferenceMapDatabase(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe -ref.map.download -o <out_dir>
  Example:      KEGG -ref.map.download 
```

##### Help for command '-Table.Create':

**Prototype**: KEGG.CLI::Int32 CreateTABLE(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG.exe -table.create -i <input_dir> -o <out_csv>
  Example:      KEGG -Table.Create 
```



  Parameters information:
```
    -i
    Description:  This parameter specific the source directory input of the download data.

    Example:      -i ""


```

#### Accepted Types
##### -i
