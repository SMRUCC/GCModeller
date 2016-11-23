---
title: gcc
tags: [maunal, tools]
date: 11/24/2016 2:54:06 AM
---
# GCModeller [version 1.0.0.0]
> gcc=GCModeller Compiler; Compiler program for the GCModeller virtual cell system model

<!--more-->

**GCModeller Modelling console and model compiler**<br/>
_GCModeller Modelling console and model compiler (GCModeller????????)_<br/>
Copyright © ???????????? 2013

**Module AssemblyName**: file:///G:/GCModeller/GCModeller/bin/gcc.exe<br/>
**Root namespace**: ``gcc.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[-add_replacement](#-add_replacement)||
|[-add_rule](#-add_rule)||
|[compile_metacyc](#compile_metacyc)|compile a metacyc database into a gcml(genetic clock markup language) model file.|

## CLI API list
--------------------------
<h3 id="-add_replacement"> 1. -add_replacement</h3>


**Prototype**: ``gcc.CLI::Int32 AddNewPair(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
gcc -add_replacement -old <old_value> -new <new_value>
```
<h3 id="-add_rule"> 2. -add_rule</h3>


**Prototype**: ``gcc.CLI::Int32 AddRule(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
gcc -add_rule -rulefile <path> -db <datadir> -model <path> [-grep <scriptText>]
```


#### Arguments
##### -rulefile
a file contains some protein interaction rules

###### Example
```bash
-rulefile <term_string>
```
##### -db
original database for the target compiled model

###### Example
```bash
-db <term_string>
```
##### -model
Target model file for adding some new rules

###### Example
```bash
-model <term_string>
```
##### [-grep]
If null then the system will using the MeatCyc database unique-id parsing method as default.

###### Example
```bash
-grep <term_string>
```
<h3 id="compile_metacyc"> 3. compile_metacyc</h3>

compile a metacyc database into a gcml(genetic clock markup language) model file.
**Prototype**: ``gcc.CLI::Int32 CompileMetaCyc(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
gcc compile_metacyc -i <data_dir> -o <output_file>
```
###### Example
```bash
gcc compile_metacyc -i ~/Documents/ecoli/ -o ~/Desktop/ecoli.xml
```


#### Arguments
##### -i


###### Example
```bash
-i <term_string>
```
##### -o


###### Example
```bash
-o <term_string>
```
