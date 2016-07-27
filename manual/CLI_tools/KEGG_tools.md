---
title: KEGG_tools
tags: [maunal, tools]
date: 7/27/2016 6:40:18 PM
---
# GCModeller [version 3.0.854.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/KEGG_tools.exe
**Root namespace**: KEGG_tools.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/16S_rRNA||
|/blastn|Blastn analysis of your DNA sequence on KEGG server for the functional analysis.|
|/Compile.Model|KEGG pathway model compiler|
|/Download.Ortholog|Downloads the KEGG gene ortholog annotation data from the web server.|
|/Download.Pathway.Maps||
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

**Prototype**: KEGG_tools.CLI::Int32 Download16SRNA(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe /16s_rna [/out <outDIR>]
  Example:      KEGG_tools /16S_rRNA 
```

##### Help for command '/blastn':

**Prototype**: KEGG_tools.CLI::Int32 Blastn(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Blastn analysis of your DNA sequence on KEGG server for the functional analysis.
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe /blastn /query <query.fasta> [/out <outDIR>]
  Example:      KEGG_tools /blastn 
```

##### Help for command '/Compile.Model':

**Prototype**: KEGG_tools.CLI::Int32 Compile(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  KEGG pathway model compiler
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe /Compile.Model /pathway <pathwayDIR> /mods <modulesDIR> /sp <sp_code> [/out <out.Xml>]
  Example:      KEGG_tools /Compile.Model 
```

##### Help for command '/Download.Ortholog':

**Prototype**: KEGG_tools.CLI::Int32 DownloadOrthologs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Downloads the KEGG gene ortholog annotation data from the web server.
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe /Download.Ortholog -i <gene_list_file.txt/gbk> -export <exportedDIR> [/gbk /sp <KEGG.sp>]
  Example:      KEGG_tools /Download.Ortholog 
```

##### Help for command '/Download.Pathway.Maps':

**Prototype**: KEGG_tools.CLI::Int32 DownloadPathwayMaps(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe /Download.Pathway.Maps /sp <kegg.sp_code> [/out <EXPORT_DIR>]
  Example:      KEGG_tools /Download.Pathway.Maps 
```

##### Help for command '/Dump.sp':

**Prototype**: KEGG_tools.CLI::Int32 DumpOrganisms(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe /Dump.sp [/res sp.html /out <out.csv>]
  Example:      KEGG_tools /Dump.sp 
```

##### Help for command '/Fasta.By.Sp':

**Prototype**: KEGG_tools.CLI::Int32 GetFastaBySp(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe /Fasta.By.Sp /in <KEGG.fasta> /sp <sp.list> [/out <out.fasta>]
  Example:      KEGG_tools /Fasta.By.Sp 
```

##### Help for command '/Get.prot_motif':

**Prototype**: KEGG_tools.CLI::Int32 ProteinMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe /Get.prot_motif /query <sp:locus> [/out out.json]
  Example:      KEGG_tools /Get.prot_motif 
```

##### Help for command '/Gets.prot_motif':

**Prototype**: KEGG_tools.CLI::Int32 GetsProteinMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe /Gets.prot_motif /query <query.txt/genome.PTT> [/PTT /sp <kegg-sp> /out <out.json> /update]
  Example:      KEGG_tools /Gets.prot_motif 
```

##### Help for command '/Imports.SSDB':

**Prototype**: KEGG_tools.CLI::Int32 ImportsDb(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe /Imports.SSDB /in <source.DIR> [/out <ssdb.csv>]
  Example:      KEGG_tools /Imports.SSDB 
```

##### Help for command '/Pathways.Downloads.All':

**Prototype**: KEGG_tools.CLI::Int32 DownloadsAllPathways(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe /Pathways.Downloads.All [/out <outDIR>]
  Example:      KEGG_tools /Pathways.Downloads.All 
```

##### Help for command '/Pull.Seq':

**Prototype**: KEGG_tools.CLI::Int32 PullSequence(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Downloads the missing sequence in the local KEGG database from the KEGG database server.
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe 
  Example:      KEGG_tools /Pull.Seq 
```

##### Help for command '/Query.KO':

**Prototype**: KEGG_tools.CLI::Int32 QueryKOAnno(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe /Query.KO /in <blastnhits.csv> [/out <out.csv> /evalue 1e-5 /batch]
  Example:      KEGG_tools /Query.KO 
```

##### Help for command '/Views.mod_stat':

**Prototype**: KEGG_tools.CLI::Int32 Stats(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe /Views.mod_stat /in <KEGG_Modules/Pathways_DIR> /locus <in.csv> [/locus_map Gene /pathway /out <out.csv>]
  Example:      KEGG_tools /Views.mod_stat 
```

##### Help for command '-Build.KO':

**Prototype**: KEGG_tools.CLI::Int32 BuildKEGGOrthology(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Download data from KEGG database to local server.
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe -Build.KO [/fill-missing]
  Example:      KEGG_tools -Build.KO 
```

##### Help for command 'Download.Sequence':

**Prototype**: KEGG_tools.CLI::Int32 DownloadSequence(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe Download.Sequence /query <querySource.txt> [/out <outDIR> /source <existsDIR>]
  Example:      KEGG_tools Download.Sequence 
```

##### Help for command '--Dump.Db':

**Prototype**: KEGG_tools.CLI::Int32 DumpDb(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe --Dump.Db /KEGG.Pathways <DIR> /KEGG.Modules <DIR> /KEGG.Reactions <DIR> /sp <sp.Code> /out <out.Xml>
  Example:      KEGG_tools --Dump.Db 
```

##### Help for command '--Export.KO':

**Prototype**: KEGG_tools.CLI::Int32 ExportKO(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe 
  Example:      KEGG_tools --Export.KO 
```

##### Help for command '-function.association.analysis':

**Prototype**: KEGG_tools.CLI::Int32 FunctionAnalysis(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe -function.association.analysis -i <matrix_csv>
  Example:      KEGG_tools -function.association.analysis 
```

##### Help for command '--Get.KO':

**Prototype**: KEGG_tools.CLI::Int32 GetKOAnnotation(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe --Get.KO /in <KASS-query.txt>
  Example:      KEGG_tools --Get.KO 
```

##### Help for command '--part.from':

**Prototype**: KEGG_tools.CLI::Int32 GetSource(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  source and ref should be in KEGG annotation format.
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe --part.from /source <source.fasta> /ref <referenceFrom.fasta> [/out <out.fasta> /brief]
  Example:      KEGG_tools --part.from 
```

##### Help for command '-query':

**Prototype**: KEGG_tools.CLI::Int32 QueryGenes(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Query the KEGG database for nucleotide sequence and protein sequence by using a keywork.
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe -query -keyword <keyword> -o <out_dir>
  Example:      KEGG_tools -query 
```

##### Help for command '-query.orthology':

**Prototype**: KEGG_tools.CLI::Int32 QueryOrthology(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe -query.orthology -keyword <gene_name> -o <output_csv>
  Example:      KEGG_tools -query.orthology 
```

##### Help for command '-query.ref.map':

**Prototype**: KEGG_tools.CLI::Int32 DownloadReferenceMap(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe -query.ref.map -id <id> -o <out_dir>
  Example:      KEGG_tools -query.ref.map 
```

##### Help for command '-ref.map.download':

**Prototype**: KEGG_tools.CLI::Int32 DownloadReferenceMapDatabase(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe -ref.map.download -o <out_dir>
  Example:      KEGG_tools -ref.map.download 
```

##### Help for command '-Table.Create':

**Prototype**: KEGG_tools.CLI::Int32 CreateTABLE(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\KEGG_tools.exe -table.create -i <input_dir> -o <out_csv>
  Example:      KEGG_tools -Table.Create 
```



  Parameters information:
```
    -i
    Description:  This parameter specific the source directory input of the download data.

    Example:      -i ""


```

#### Accepted Types
##### -i
