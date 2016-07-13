---
title: PLAS
tags: [maunal, tools]
date: 7/7/2016 6:51:49 PM
---
# GCModeller [version 1.0.0.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/PLAS.exe
**Root namespace**: PLAS.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|Compile|Compile a script file or sbml file into the plas model file.|
|Run|run a model file of the biochemical network system.|

## Commands
--------------------------
##### Help for command 'Compile':

**Prototype**: PLAS.CLI::Int32 Compile(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Compile a script file or sbml file into the plas model file.
  Usage:        G:\GCModeller\manual\bin\PLAS.exe compile -i <file> -f <script/sbml> -o <output_file> [/auto-fix]
  Example:      PLAS Compile compile -i "/home/xieguigang/proj/metacyc/xcc8004/17.0/data/metabolic-reactions.sbml" -f sbml -o "/home/xieguigang/Desktop/xcc8004.xml"
```

##### Help for command 'Run':

**Prototype**: PLAS.CLI::Int32 Run(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  run a model file of the biochemical network system.
  Usage:        G:\GCModeller\manual\bin\PLAS.exe run -i <model_file> -f <script/model/sbml> -chart <T/F> -o <output_csv>
  Example:      PLAS Run run -i "/home/xieguigang/proj/xcc8004.sbml" -f sbml -chart T -o "/home/xieguigang/Desktop/xcc8004.csv"
```



  Parameters information:
```
    -i
    Description:  The file path of the input model file that will be run on the PLAS program.

    Example:      -i "/home/xieguigang/proj/xcc8004.sbml"

-o
    Description:  The file path of the output data file for the calculation.

    Example:      -o "/home/xieguigang/Desktop/xcc8004.csv"

   [-f]
    Description:  This parameter specific that the file format of the model file which will be run on the PLAS program.
 script - The input file that specific by the switch parameter "-i" is a PLAS script file,
 model - The input file is a compiled PLAS model, run it directly,
 sbml - The input file is a sbml model file, it needs to be compiled to a PLAS model first.

    Example:      -f "model"

   [-chart]
    Description:  Optional, This switch specific that PLAS displaying a chart windows after the calculation or not, default is F for not displaying.
 T - (True) display a chart window after the calculation,
 F - (False) not display a chart window after the calculation.

    Example:      -chart "/home/xieguigang/proj/xcc8004.sbml"


```

#### Accepted Types
##### -i
##### -o
##### -f
##### -chart
