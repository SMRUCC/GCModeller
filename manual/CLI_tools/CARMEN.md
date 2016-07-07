---
title: CARMEN
tags: [maunal, tools]
date: 7/7/2016 6:51:25 PM
---
# GCModeller [version 1.0.0.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/CARMEN.exe
**Root namespace**: CARMENTools.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/Export.Anno||
|--lstId.Downloads|Download the Avaliable organism name And available pathways' name.|
|--Reconstruct.KEGG.Online||

## Commands
--------------------------
##### Help for command '/Export.Anno':

**Prototype**: CARMENTools.CLI::Int32 ExportAnno(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\CARMEN.exe /Export.Anno /in <inDIR> [/out <out.Csv>]
  Example:      CARMEN /Export.Anno 
```

##### Help for command '--lstId.Downloads':

**Prototype**: CARMENTools.CLI::Int32 DownloadList(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Download the Avaliable organism name And available pathways' name.
  Usage:        G:\GCModeller\manual\bin\CARMEN.exe --lstId.Downloads [/o <out.DIR>]
  Example:      CARMEN --lstId.Downloads 
```

##### Help for command '--Reconstruct.KEGG.Online':

**Prototype**: CARMENTools.CLI::Int32 Reconstruct(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\CARMEN.exe --Reconstruct.KEGG.Online /sp <organism> [/pathway <KEGG.pathwayId> /out <outDIR>]
  Example:      CARMEN --Reconstruct.KEGG.Online 
```

