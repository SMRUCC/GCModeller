---
title: gcc
tags: [maunal, tools]
date: 7/27/2016 6:40:17 PM
---
# GCModeller [version 1.0.0.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/gcc.exe
**Root namespace**: gcc.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|-add_replacement||
|-add_rule||
|compile_metacyc|compile a metacyc database into a gcml(genetic clock markup language) model file.|

## Commands
--------------------------
##### Help for command '-add_replacement':

**Prototype**: gcc.CLI::Int32 AddNewPair(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\gcc.exe -add_replacement -old <old_value> -new <new_value>
  Example:      gcc -add_replacement 
```

##### Help for command '-add_rule':

**Prototype**: gcc.CLI::Int32 AddRule(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\gcc.exe -add_rule -rulefile <path> -db <datadir> -model <path> [-grep <scriptText>]
  Example:      gcc -add_rule 
```



  Parameters information:
```
    -rulefile
    Description:  a file contains some protein interaction rules

    Example:      -rulefile ""

-db
    Description:  original database for the target compiled model

    Example:      -db ""

-model
    Description:  Target model file for adding some new rules

    Example:      -model ""

   [-grep]
    Description:  If null then the system will using the MeatCyc database unique-id parsing method as default.

    Example:      -grep ""


```

#### Accepted Types
##### -rulefile
##### -db
##### -model
##### -grep
##### Help for command 'compile_metacyc':

**Prototype**: gcc.CLI::Int32 CompileMetaCyc(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  compile a metacyc database into a gcml(genetic clock markup language) model file.
  Usage:        G:\GCModeller\manual\bin\gcc.exe compile_metacyc -i <data_dir> -o <output_file>
  Example:      gcc compile_metacyc compile_metacyc -i ~/Documents/ecoli/ -o ~/Desktop/ecoli.xml
```



  Parameters information:
```
    -i
    Description:  

    Example:      -i ""

-o
    Description:  

    Example:      -o ""


```

#### Accepted Types
##### -i
##### -o
