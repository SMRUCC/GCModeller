---
title: CARMEN
tags: [maunal, tools]
date: 11/24/2016 2:54:03 AM
---
# GCModeller [version 1.0.0.0]
> 

<!--more-->

**CARMEN**<br/>
__<br/>
Copyright ©  2015

**Module AssemblyName**: file:///G:/GCModeller/GCModeller/bin/CARMEN.exe<br/>
**Root namespace**: ``CARMENTools.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Export.Anno](#/Export.Anno)||
|[--lstId.Downloads](#--lstId.Downloads)|Download the Avaliable organism name And available pathways' name.|
|[--Reconstruct.KEGG.Online](#--Reconstruct.KEGG.Online)||

## CLI API list
--------------------------
<h3 id="/Export.Anno"> 1. /Export.Anno</h3>


**Prototype**: ``CARMENTools.CLI::Int32 ExportAnno(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
CARMEN /Export.Anno /in <inDIR> [/out <out.Csv>]
```
<h3 id="--lstId.Downloads"> 2. --lstId.Downloads</h3>

Download the Avaliable organism name And available pathways' name.
**Prototype**: ``CARMENTools.CLI::Int32 DownloadList(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
CARMEN --lstId.Downloads [/o <out.DIR>]
```
<h3 id="--Reconstruct.KEGG.Online"> 3. --Reconstruct.KEGG.Online</h3>


**Prototype**: ``CARMENTools.CLI::Int32 Reconstruct(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
CARMEN --Reconstruct.KEGG.Online /sp <organism> [/pathway <KEGG.pathwayId> /out <outDIR>]
```
