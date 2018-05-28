---
title: Settings
tags: [maunal, tools]
date: 5/28/2018 9:05:10 PM
---
# GCModeller [version 1.0.0.0]
> GCModeller configuration console.

<!--more-->

**SMRUCC genomics GCModeller Programs Profiles Manager**<br/>
_SMRUCC genomics GCModeller Programs Profiles Manager_<br/>
Copyright © SMRUCC genomics. 2014

**Module AssemblyName**: Settings<br/>
**Root namespace**: ``GCModeller.Configuration.CLI``<br/>

------------------------------------------------------------
If you are having trouble debugging this Error, first read the best practices tutorial for helpful tips that address many common problems:
> http://docs.gcmodeller.org


The debugging facility Is helpful To figure out what's happening under the hood:
> https://github.com/SMRUCC/GCModeller/wiki


If you're still stumped, you can try get help from author directly from E-mail:
> xie.guigang@gcmodeller.org



All of the command that available in this program has been list below:

##### 1. GCModeller configuration CLI tool


|Function API|Info|
|------------|----|
|[/set.mysql](#/set.mysql)|Setting up the mysql connection parameters|
|[Set](#Set)|Setting up the configuration data node.|
|[var](#var)|Gets the settings value.|


##### 2. GCModeller development helper CLI


|Function API|Info|
|------------|----|
|[/dev](#/dev)|Generates Apps CLI visualbasic reference source code.|

## CLI API list
--------------------------
<h3 id="/dev"> 1. /dev</h3>

Generates Apps CLI visualbasic reference source code.
**Prototype**: ``GCModeller.Configuration.CLI::Int32 CLIDevelopment(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Settings /dev [/out <DIR>]
```


#### Arguments
##### [/out]
The generated VisualBasic source file output directory location.

###### Example
```bash
/out <file/directory>
```
##### Accepted Types
###### /out
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

<h3 id="/set.mysql"> 2. /set.mysql</h3>

Setting up the mysql connection parameters
**Prototype**: ``GCModeller.Configuration.CLI::Int32 SetMySQL(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Settings /set.mysql /test
```


#### Arguments
##### [/test]
If this boolean argument is set, then the program will testing for the mysqli connection before write the configuration file. If the connection test failure, then the configuration file will not be updated!

###### Example
```bash
/test
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /test
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="Set"> 3. Set</h3>

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


#### Arguments
##### <varName>
The variable name in the GCModeller configuration file.

###### Example
```bash
<varName> <term_string>
```
##### Accepted Types
###### <varName>
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

<h3 id="var"> 4. var</h3>

Gets the settings value.
**Prototype**: ``GCModeller.Configuration.CLI::Int32 ViewVar(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Settings var [varName] [/value]
```


#### Arguments
##### [[VarName]]
If this value is null, then the program will prints all of the variables in the gcmodeller config file or when the variable is presents in the database, only the config value of the specific variable will be display.

###### Example
```bash
[VarName] <term_string>
```
##### [/value]
If this argument is presented, then this settings program will only output the variable value, otherwise will output data in format: key = value

###### Example
```bash
/value
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /value
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

