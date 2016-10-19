---
title: gcc
tags: [maunal, tools]
date: 2016/10/19 16:38:31
---
# GCModeller [version 1.0.0.0]
> gcc=GCModeller Compiler; Compiler program for the GCModeller virtual cell system model

<!--more-->

**GCModeller Modelling console and model compiler**
_GCModeller Modelling console and model compiler (GCModeller模型文件编译工具)_
Copyright ? 蓝思生物信息工程师工作站 2013

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/gcc.exe
**Root namespace**: ``gcc.CLI``


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


**Prototype**: ``gcc.CLI::Int32 AddNewPair(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
gcc -add_replacement -old <old_value> -new <new_value>
```
###### Example
```bash
gcc
```
<h3 id="-add_rule"> 2. -add_rule</h3>


**Prototype**: ``gcc.CLI::Int32 AddRule(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
gcc -add_rule -rulefile <path> -db <datadir> -model <path> [-grep <scriptText>]
```
###### Example
```bash
gcc
```



#### Parameters information:
##### -rulefile
a file contains some protein interaction rules

###### Example
```bash

```
##### -db
original database for the target compiled model

###### Example
```bash

```
##### -model
Target model file for adding some new rules

###### Example
```bash

```
##### [-grep]
If null then the system will using the MeatCyc database unique-id parsing method as default.

###### Example
```bash

```
##### Accepted Types
###### -rulefile
###### -db
###### -model
###### -grep
<h3 id="compile_metacyc"> 3. compile_metacyc</h3>

compile a metacyc database into a gcml(genetic clock markup language) model file.
**Prototype**: ``gcc.CLI::Int32 CompileMetaCyc(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
gcc compile_metacyc -i <data_dir> -o <output_file>
```
###### Example
```bash
gcc compile_metacyc -i ~/Documents/ecoli/ -o ~/Desktop/ecoli.xml
```



#### Parameters information:
##### -i


###### Example
```bash

```
##### -o


###### Example
```bash

```
##### Accepted Types
###### -i
###### -o
