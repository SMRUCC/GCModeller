---
title: Circos
tags: [maunal, tools]
date: 7/7/2016 6:51:27 PM
---
# GCModeller [version 1.0.0.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/Circos.exe
**Root namespace**: Circos_CLITools.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/Alignment.Dumps||
|/DOOR.COGs||
|/MGA2Myva||
|/NT.Variation||
|/Regulons.Dumps||
|--AT.Percent||
|--GC.Skew||

## Commands
--------------------------
##### Help for command '/Alignment.Dumps':

**Prototype**: Circos_CLITools.CLI::Int32 AlignmentTableDump(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Circos.exe /Alignment.Dumps /in <inDIR> [/out <out.Xml>]
  Example:      Circos /Alignment.Dumps 
```

##### Help for command '/DOOR.COGs':

**Prototype**: Circos_CLITools.CLI::Int32 DOOR_COGs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Circos.exe /DOOR.COGs /DOOR <genome.opr> [/out <out.COGs.Csv>]
  Example:      Circos /DOOR.COGs 
```

##### Help for command '/MGA2Myva':

**Prototype**: Circos_CLITools.CLI::Int32 MGA2Myva(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Circos.exe /MGA2Myva /in <mga_cog.csv> [/out <myva_cog.csv> /map <genome.gb>]
  Example:      Circos /MGA2Myva 
```

##### Help for command '/NT.Variation':

**Prototype**: Circos_CLITools.CLI::Int32 NTVariation(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Circos.exe /NT.Variation /mla <fasta.fa> [/ref <index/fasta.fa, 0> /out <out.txt> /cut 0.75]
  Example:      Circos /NT.Variation 
```

##### Help for command '/Regulons.Dumps':

**Prototype**: Circos_CLITools.CLI::Int32 DumpNames(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Circos.exe /Regulons.Dumps /in <genomes.bbh.DIR> /ptt <genome.ptt> [/out <out.Csv>]
  Example:      Circos /Regulons.Dumps 
```

##### Help for command '--AT.Percent':

**Prototype**: Circos_CLITools.CLI::Int32 ATContent(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Circos.exe --AT.Percent /in <in.fasta> [/win_size <200> /step <25> /out <out.txt>]
  Example:      Circos --AT.Percent 
```

##### Help for command '--GC.Skew':

**Prototype**: Circos_CLITools.CLI::Int32 GCSkew(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Circos.exe --GC.Skew /in <in.fasta> [/win_size <200> /step <25> /out <out.txt>]
  Example:      Circos --GC.Skew 
```

