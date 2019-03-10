---
title: PLAS
tags: [maunal, tools]
date: 5/28/2018 9:30:26 PM
---
# GCModeller [version 1.0.0.0]
> 

<!--more-->

**PLAS**<br/>
__<br/>
Copyright © LANS Engineering Workstation 2013

**Module AssemblyName**: PLAS<br/>
**Root namespace**: ``PLAS.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[Compile](#Compile)|Compile a script file or sbml file into the plas model file.|
|[Run](#Run)|run a model file of the biochemical network system.|

## CLI API list
--------------------------
<h3 id="Compile"> 1. Compile</h3>

Compile a script file or sbml file into the plas model file.

**Prototype**: ``PLAS.CLI::Int32 Compile(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
PLAS compile -i <file> -f <script/sbml> -o <output_file> [/auto-fix]
```
###### Example
```bash
PLAS compile -i "/home/xieguigang/proj/metacyc/xcc8004/17.0/data/metabolic-reactions.sbml" -f sbml -o "/home/xieguigang/Desktop/xcc8004.xml"
```
<h3 id="Run"> 2. Run</h3>

run a model file of the biochemical network system.

**Prototype**: ``PLAS.CLI::Int32 Run(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
PLAS run -i <model_file> -f <script/model/sbml> [-o <output_csv> /time <-1> /ODEs]
```
###### Example
```bash
PLAS run -i "/home/xieguigang/proj/xcc8004.sbml" -f sbml -chart T -o "/home/xieguigang/Desktop/xcc8004.csv"
```


#### Arguments
##### -i
The file path of the input model file that will be run on the PLAS program.

###### Example
```bash
-i <term_string>
```
##### -o
The file path of the output data file for the calculation.

###### Example
```bash
-o <term_string>
```
##### [-f]
This parameter specific that the file format of the model file which will be run on the PLAS program.
script - The input file that specific by the switch parameter "-i" is a PLAS script file,
model - The input file is a compiled PLAS model, run it directly,
sbml - The input file is a sbml model file, it needs to be compiled to a PLAS model first.

###### Example
```bash
-f <term_string>
```
##### [-chart]
Optional, This switch specific that PLAS displaying a chart windows after the calculation or not, default is F for not displaying.
T - (True) display a chart window after the calculation,
F - (False) not display a chart window after the calculation.

###### Example
```bash
-chart <term_string>
```
