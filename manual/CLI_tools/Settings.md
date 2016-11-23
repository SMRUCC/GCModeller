---
title: Settings
tags: [maunal, tools]
date: 11/24/2016 2:54:22 AM
---
# Settings [version 1.0.0.0]
> GCModeller configuration console.

<!--more-->

**SMRUCC genomics GCModeller Programs Profiles Manager**<br/>
_SMRUCC genomics GCModeller Programs Profiles Manager_<br/>
Copyright © SMRUCC genomics. 2014

**Module AssemblyName**: file:///G:/GCModeller/GCModeller/bin/Settings.exe<br/>
**Root namespace**: ``GCModeller.Configuration.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[Set](#Set)|Setting up the configuration data node.|
|[var](#var)|Gets the settings value.|

## CLI API list
--------------------------
<h3 id="Set"> 1. Set</h3>

Setting up the configuration data node.
**Prototype**: ``GCModeller.Configuration.CLI::Int32 Set(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Settings Set <varName> <value>
```
###### Example
```bash
Settings Set java /usr/lib/java/java.bin
```
<h3 id="var"> 2. var</h3>

Gets the settings value.
**Prototype**: ``GCModeller.Configuration.CLI::Int32 Var(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Settings var [varName]
```


#### Arguments
##### [[VarName]]
If this value is null, then the program will prints all of the variables in the gcmodeller config file or when the variable is presents in the database, only the config value of the specific variable will be display.

###### Example
```bash
[VarName] <term_string>
```
