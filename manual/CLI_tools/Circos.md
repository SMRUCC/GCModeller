---
title: Circos
tags: [maunal, tools]
date: 2016/10/22 12:30:08
---
# GCModeller [version 1.0.0.0]
> Tools for generates the circos drawing model file for the circos perl script.

<!--more-->

**Circos CLI interface Utility**
_Circos CLI interface Utility_
Copyright ?  2015

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/Circos.exe
**Root namespace**: ``Circos_CLITools.CLI``


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Alignment.Dumps](#/Alignment.Dumps)||
|[/DOOR.COGs](#/DOOR.COGs)||
|[/MGA2Myva](#/MGA2Myva)||
|[/NT.Variation](#/NT.Variation)||
|[/Regulons.Dumps](#/Regulons.Dumps)||
|[--AT.Percent](#--AT.Percent)||
|[--GC.Skew](#--GC.Skew)||

## CLI API list
--------------------------
<h3 id="/Alignment.Dumps"> 1. /Alignment.Dumps</h3>


**Prototype**: ``Circos_CLITools.CLI::Int32 AlignmentTableDump(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Circos /Alignment.Dumps /in <inDIR> [/out <out.Xml>]
```
<h3 id="/DOOR.COGs"> 2. /DOOR.COGs</h3>


**Prototype**: ``Circos_CLITools.CLI::Int32 DOOR_COGs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Circos /DOOR.COGs /DOOR <genome.opr> [/out <out.COGs.Csv>]
```
<h3 id="/MGA2Myva"> 3. /MGA2Myva</h3>


**Prototype**: ``Circos_CLITools.CLI::Int32 MGA2Myva(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Circos /MGA2Myva /in <mga_cog.csv> [/out <myva_cog.csv> /map <genome.gb>]
```
<h3 id="/NT.Variation"> 4. /NT.Variation</h3>


**Prototype**: ``Circos_CLITools.CLI::Int32 NTVariation(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Circos /NT.Variation /mla <fasta.fa> [/ref <index/fasta.fa, 0> /out <out.txt> /cut 0.75]
```
<h3 id="/Regulons.Dumps"> 5. /Regulons.Dumps</h3>


**Prototype**: ``Circos_CLITools.CLI::Int32 DumpNames(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Circos /Regulons.Dumps /in <genomes.bbh.DIR> /ptt <genome.ptt> [/out <out.Csv>]
```
<h3 id="--AT.Percent"> 6. --AT.Percent</h3>


**Prototype**: ``Circos_CLITools.CLI::Int32 ATContent(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Circos --AT.Percent /in <in.fasta> [/win_size <200> /step <25> /out <out.txt>]
```
<h3 id="--GC.Skew"> 7. --GC.Skew</h3>


**Prototype**: ``Circos_CLITools.CLI::Int32 GCSkew(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Circos --GC.Skew /in <in.fasta> [/win_size <200> /step <25> /out <out.txt>]
```
