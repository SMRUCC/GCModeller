---
title: Circos
tags: [maunal, tools]
date: 11/24/2016 2:54:04 AM
---
# GCModeller [version 1.0.0.0]
> Tools for generates the circos drawing model file for the circos perl script.

<!--more-->

**Circos CLI interface Utility**<br/>
_Circos CLI interface Utility_<br/>
Copyright ©  2015

**Module AssemblyName**: file:///G:/GCModeller/GCModeller/bin/Circos.exe<br/>
**Root namespace**: ``Circos_CLITools.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Alignment.Dumps](#/Alignment.Dumps)||
|[/Compare](#/Compare)||
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
<h3 id="/Compare"> 2. /Compare</h3>


**Prototype**: ``Circos_CLITools.CLI::Int32 Compare(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Circos /Compare /locis <list.txt> /vectors <vectors.txt.DIR> [/winSize 100 /steps 1 /out <out.csv>]
```
<h3 id="/DOOR.COGs"> 3. /DOOR.COGs</h3>


**Prototype**: ``Circos_CLITools.CLI::Int32 DOOR_COGs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Circos /DOOR.COGs /DOOR <genome.opr> [/out <out.COGs.Csv>]
```
<h3 id="/MGA2Myva"> 4. /MGA2Myva</h3>


**Prototype**: ``Circos_CLITools.CLI::Int32 MGA2Myva(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Circos /MGA2Myva /in <mga_cog.csv> [/out <myva_cog.csv> /map <genome.gb>]
```
<h3 id="/NT.Variation"> 5. /NT.Variation</h3>


**Prototype**: ``Circos_CLITools.CLI::Int32 NTVariation(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Circos /NT.Variation /mla <fasta.fa> [/ref <index/fasta.fa, 0> /out <out.txt> /cut 0.75]
```
<h3 id="/Regulons.Dumps"> 6. /Regulons.Dumps</h3>


**Prototype**: ``Circos_CLITools.CLI::Int32 DumpNames(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Circos /Regulons.Dumps /in <genomes.bbh.DIR> /ptt <genome.ptt> [/out <out.Csv>]
```
<h3 id="--AT.Percent"> 7. --AT.Percent</h3>


**Prototype**: ``Circos_CLITools.CLI::Int32 ATContent(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Circos --AT.Percent /in <in.fasta> [/win_size <200> /step <25> /out <out.txt>]
```
<h3 id="--GC.Skew"> 8. --GC.Skew</h3>


**Prototype**: ``Circos_CLITools.CLI::Int32 GCSkew(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Circos --GC.Skew /in <in.fasta> [/win_size <200> /step <25> /out <out.txt>]
```
