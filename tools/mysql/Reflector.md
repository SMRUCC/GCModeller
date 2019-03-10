﻿# Reflector [version 1.0.0.0]
> Tools for convert the mysql schema dump sql script into VisualBasic classes source code.

<!--more-->

**Reflector**
__
Copyright ©  2015

**Module AssemblyName**: file:///G:/GCModeller/tools/mysql/Reflector.exe
**Root namespace**: ``Reflector.CLIProgram``


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[--export.dump](#--export.dump)||
|[--reflects](#--reflects)|Automatically generates visualbasic source code from the MySQL database schema dump.|

## CLI API list
--------------------------
<h3 id="--export.dump"> 1. --export.dump</h3>


**Prototype**: ``Reflector.CLIProgram::Int32 ExportDumpDir(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Reflector --export.dump [-o <out_dir> /namespace <namespace> --dir <source_dir>]
```
<h3 id="--reflects"> 2. --reflects</h3>

Automatically generates visualbasic source code from the MySQL database schema dump.
**Prototype**: ``Reflector.CLIProgram::Int32 ReflectsConvert(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Reflector --reflects /sql <sql_path/std_in> [-o <output_path> /namespace <namespace> /split]
```
###### Example
```bash
Reflector --reflects /sql ./test.sql /split /namespace ExampleNamespace
```


#### Arguments
##### /sql
The file path of the MySQL database schema dump file.

###### Example
```bash
/sql <term_string>
```
##### [-o]
The output file path of the generated visual basic source code file from the SQL dump file "/sql"

###### Example
```bash
-o <term_string>
```
##### [/namespace]
The namespace value will be insert into the generated source code if this parameter is not null.

###### Example
```bash
/namespace <term_string>
```
##### [/split]
Split the source code into sevral files and named by table name?

###### Example
```bash
/split <term_string>
```
##### Accepted Types
###### /sql
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

###### -o
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

###### /namespace
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

###### /split
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

