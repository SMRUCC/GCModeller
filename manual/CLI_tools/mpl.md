---
title: mpl
tags: [maunal, tools]
date: 7/27/2016 6:40:20 PM
---
# GCModeller [version 1.0.0.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/mpl.exe
**Root namespace**: xMPAlignment.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/Build.Db.CDD|Install NCBI CDD database into the GCModeller repository database for the MPAlignment analysis.|
|/Build.Db.Family|Build protein family database from KEGG database dump data and using for the protein family annotation by MPAlignment.|
|/Build.Db.Family.Manual-Build||
|/Build.Db.Ortholog|Build protein functional orthology database from KEGG orthology or NCBI COG database.|
|/Build.Db.PPI|Build protein interaction seeds database from string-db.|
|/Build.PPI.Signature||
|/KEGG.Family||
|/Motif.Density||
|/Pfam.Align|Align your proteins with selected protein domain structure database by using blast+ program.|
|/Pfam.Sub||
|/Pfam-String.Dump|Dump the pfam-String domain Structure composition information from the blastp alignment result.|
|/Select.Pfam-String||
|--align|MPAlignment on your own dataset.|
|--align.Family|Protein family annotation by using MPAlignment algorithm.|
|--align.Family_test||
|--align.Function|Protein function annotation by using MPAlignment algorithm.|
|--align.PPI|Protein-Protein interaction network annotation by using MPAlignment algorithm.|
|--align.PPI_test||
|--align.String|MPAlignment test, pfam-string value must be in format as  <locusId>:<length>:<pfam-string>|
|--blast.allhits||
|--View.Alignment||

## Commands
--------------------------
##### Help for command '/Build.Db.CDD':

**Prototype**: xMPAlignment.CLI::Int32 InstallCDD(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Install NCBI CDD database into the GCModeller repository database for the MPAlignment analysis.
  Usage:        G:\GCModeller\manual\bin\mpl.exe /Build.Db.CDD /source <source.DIR>
  Example:      mpl /Build.Db.CDD 
```

##### Help for command '/Build.Db.Family':

**Prototype**: xMPAlignment.CLI::Int32 BuildFamily(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Build protein family database from KEGG database dump data and using for the protein family annotation by MPAlignment.
  Usage:        G:\GCModeller\manual\bin\mpl.exe /Build.Db.Family /source <source.KEGG.fasta> /pfam <pfam-string.csv>
  Example:      mpl /Build.Db.Family 
```

##### Help for command '/Build.Db.Family.Manual-Build':

**Prototype**: xMPAlignment.CLI::Int32 ManualBuild(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\mpl.exe /Build.Db.Family.Manual-Build /pfam-string <pfam-string.csv> /name <familyName>
  Example:      mpl /Build.Db.Family.Manual-Build 
```

##### Help for command '/Build.Db.Ortholog':

**Prototype**: xMPAlignment.CLI::Int32 BuildOrthologDb(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Build protein functional orthology database from KEGG orthology or NCBI COG database.
  Usage:        G:\GCModeller\manual\bin\mpl.exe /Build.Db.Ortholog [/COG <cogDIR> /KO]
  Example:      mpl /Build.Db.Ortholog 
```

##### Help for command '/Build.Db.PPI':

**Prototype**: xMPAlignment.CLI::Int32 BuildPPIDb(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Build protein interaction seeds database from string-db.
  Usage:        G:\GCModeller\manual\bin\mpl.exe 
  Example:      mpl /Build.Db.PPI 
```

##### Help for command '/Build.PPI.Signature':

**Prototype**: xMPAlignment.CLI::Int32 BuildSignature(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\mpl.exe /Build.PPI.Signature /in <clustalW.fasta> [/level <5> /out <out.xml>]
  Example:      mpl /Build.PPI.Signature 
```



  Parameters information:
```
       [/level]
    Description:  It is not recommended to modify this value. The greater of this value, the more strict of the interaction scoring. level 5 is enough.

    Example:      /level ""


```

#### Accepted Types
##### /level
##### Help for command '/KEGG.Family':

**Prototype**: xMPAlignment.CLI::Int32 KEGGFamilys(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\mpl.exe /KEGG.Family /in <inDIR> /pfam <pfam-string.csv> [/out <out.csv>]
  Example:      mpl /KEGG.Family 
```

##### Help for command '/Motif.Density':

**Prototype**: xMPAlignment.CLI::Int32 MotifDensity(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\mpl.exe /Motif.Density /in <pfam-string.csv> [/out <out.csv>]
  Example:      mpl /Motif.Density 
```

##### Help for command '/Pfam.Align':

**Prototype**: xMPAlignment.CLI::Int32 AlignPfam(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Align your proteins with selected protein domain structure database by using blast+ program.
  Usage:        G:\GCModeller\manual\bin\mpl.exe /Pfam.Align /query <query.fasta> [/db <name/path> /out <blastOut.txt>]
  Example:      mpl /Pfam.Align 
```

##### Help for command '/Pfam.Sub':

**Prototype**: xMPAlignment.CLI::Int32 SubPfam(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\mpl.exe /Pfam.Sub /index <bbh_index.csv> /pfam <pfam-string> [/out <sub-out.csv>]
  Example:      mpl /Pfam.Sub 
```

##### Help for command '/Pfam-String.Dump':

**Prototype**: xMPAlignment.CLI::Int32 DumpPfamString(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Dump the pfam-String domain Structure composition information from the blastp alignment result.
  Usage:        G:\GCModeller\manual\bin\mpl.exe /Pfam-String.Dump /In <blastp_out.txt> [/out <pfam-String.csv> /evalue <0.001> /identities <0.2> /coverage <0.85>]
  Example:      mpl /Pfam-String.Dump 
```



  Parameters information:
```
    /In
    Description:  The blastp output For the protein alignment, which the aligned database can be selected from Pfam-A Or NCBI CDD. And blast+ program Is recommended For used For the domain alignment.

    Example:      /In ""

   [/out]
    Description:  The output Excel .csv data file path For the dumped pfam-String data Of your annotated protein. If this parameter Is empty, Then the file will saved On the same location With your blastp input file.

    Example:      /out ""


```

#### Accepted Types
##### /In
##### /out
##### Help for command '/Select.Pfam-String':

**Prototype**: xMPAlignment.CLI::Int32 SelectPfams(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\mpl.exe /Select.Pfam-String /in <pfam-string.csv> /hits <bbh/sbh.csv> [/hit_name /out <out.csv>]
  Example:      mpl /Select.Pfam-String 
```

##### Help for command '--align':

**Prototype**: xMPAlignment.CLI::Int32 MPAlignment(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  MPAlignment on your own dataset.
  Usage:        G:\GCModeller\manual\bin\mpl.exe --align /query <pfam-string.csv> /subject <pfam-string.csv> [/hits <query_vs_sbj.blastp.csv> /flip-bbh /out <alignment_out.csv> /mp <cutoff:=0.65> /swap /parts]
  Example:      mpl --align 
```



  Parameters information:
```
       [/swap]
    Description:  Swap the location of query and subject in the output result set.

    Example:      /swap ""

   [/parts]
    Description:  Does the domain motif equals function determine the domain positioning equals just if one side in the high scoring then thoese two domain its position is equals? 
Default is not, default checks right side and left side.

    Example:      /parts ""

   [/flip-bbh]
    Description:  Swap the direction of the query_name/hit_name in the hits?

    Example:      /flip-bbh ""


```

#### Accepted Types
##### /swap
##### /parts
##### /flip-bbh
##### Help for command '--align.Family':

**Prototype**: xMPAlignment.CLI::Int32 FamilyClassified(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Protein family annotation by using MPAlignment algorithm.
  Usage:        G:\GCModeller\manual\bin\mpl.exe --align.Family /query <pfam-string.csv> [/out <out.csv> /threshold 0.5 /mp 0.6 /Name <null>]
  Example:      mpl --align.Family 
```



  Parameters information:
```
       [/Name]
    Description:  The database name of the aligned subject, if this value is empty or not exists in the source, then the entired Family database will be used.

    Example:      /Name ""


```

#### Accepted Types
##### /Name
##### Help for command '--align.Family_test':

**Prototype**: xMPAlignment.CLI::Int32 FamilyAlignmentTest(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\mpl.exe --align.Family_test /query <pfam-string> /name <dbName/Path> [/threshold <0.65> /mpCut <0.65> /accept <10>]
  Example:      mpl --align.Family_test 
```

##### Help for command '--align.Function':

**Prototype**: xMPAlignment.CLI::Int32 AlignFunction(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Protein function annotation by using MPAlignment algorithm.
  Usage:        G:\GCModeller\manual\bin\mpl.exe 
  Example:      mpl --align.Function 
```

##### Help for command '--align.PPI':

**Prototype**: xMPAlignment.CLI::Int32 MplPPI(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Protein-Protein interaction network annotation by using MPAlignment algorithm.
  Usage:        G:\GCModeller\manual\bin\mpl.exe 
  Example:      mpl --align.PPI 
```

##### Help for command '--align.PPI_test':

**Prototype**: xMPAlignment.CLI::Int32 StructureAlign(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\mpl.exe --align.PPI_test /query <contacts.fasta> /db <ppi_signature.Xml> [/mp <cutoff:=0.9> /out <outDIR>]
  Example:      mpl --align.PPI_test 
```

##### Help for command '--align.String':

**Prototype**: xMPAlignment.CLI::Int32 MPAlignment2(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  MPAlignment test, pfam-string value must be in format as  <locusId>:<length>:<pfam-string>
  Usage:        G:\GCModeller\manual\bin\mpl.exe --align.String /query <pfam-string> /subject <pfam-string> [/mp <cutoff:=0.65> /out <outDIR> /parts]
  Example:      mpl --align.String 
```

##### Help for command '--blast.allhits':

**Prototype**: xMPAlignment.CLI::Int32 ExportAppSBH(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\mpl.exe --blast.allhits /blast <blastp.txt> [/out <sbh.csv> /coverage <0.5> /identities 0.15]
  Example:      mpl --blast.allhits 
```

##### Help for command '--View.Alignment':

**Prototype**: xMPAlignment.CLI::Int32 ViewAlignment(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\mpl.exe --View.Alignment /blast <blastp.txt> /name <queryName> [/out <out.png>]
  Example:      mpl --View.Alignment 
```

