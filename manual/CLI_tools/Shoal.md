---
title: Shoal
tags: [maunal, tools]
date: 11/24/2016 2:54:24 AM
---
# ShoalShell [version 1.2.258.2033]
> This module define the shoal commandlines for the command line interpreter.

<!--more-->

**ShoalShell Command Line Interpreter**<br/>
_*.shl_<br/>
Copyright © xie.guigang@gmail.com 2014

**Module AssemblyName**: file:///G:/GCModeller/GCModeller/bin/Shoal.exe<br/>
**Root namespace**: ``Microsoft.VisualBasic.Shoal.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/debug](#/debug)|Start the shoal shell in debug output mode.|
|[::](#::)|Execute one script line, this command is useful for the shoal API development and debugging.|
|[~](#~)|Start the shoal shell in the current directory, not using the directory in the profile data.|
|[--logs.show](#--logs.show)||
|[-register_modules](#-register_modules)|Register the shellscript API module assembly DLL or assembly exe file to the shellscript type registry.|
|[-scan.plugins](#-scan.plugins)|Scanning all of the avaliable shoal plugin modules in the specific directory and install all of them into the shoal registry.|
|[set](#set)|Setting up the shoal environment variables, you can using var command to view all of the avaliable variable in the shoal shell.|
|[-start](#-start)|Start the shoal shell using the user custom data.|
|[var](#var)|Get the environment variable value in the shoal shell, if a variable name is not specific, then the shoal will list all of the variable value in shoal.|
|[--version](#--version)|Print the version of the shoal shell in the console.|

## CLI API list
--------------------------
<h3 id="/debug"> 1. /debug</h3>

Start the shoal shell in debug output mode.
**Prototype**: ``Microsoft.VisualBasic.Shoal.CLI::Int32 DEBUG(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Shoal /debug listener_port <listen_port> [-work <working_Dir>]
```
<h3 id="::"> 2. ::</h3>

Execute one script line, this command is useful for the shoal API development and debugging.
**Prototype**: ``Microsoft.VisualBasic.Shoal.CLI::Int32 Shell(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Shoal :: <scriptline>
```
###### Example
```bash
Shoal shoal :: "hello world!" -> msgbox title "This is a hello world tesing example!"
```
<h3 id="~"> 3. ~</h3>

Start the shoal shell in the current directory, not using the directory in the profile data.
**Prototype**: ``Microsoft.VisualBasic.Shoal.CLI::Int32 Start()``

###### Usage
```bash
Shoal
```
<h3 id="--logs.show"> 4. --logs.show</h3>


**Prototype**: ``Microsoft.VisualBasic.Shoal.CLI::Int32 ShowLogs()``

###### Usage
```bash
Shoal
```
<h3 id="-register_modules"> 5. -register_modules</h3>

Register the shellscript API module assembly DLL or assembly exe file to the shellscript type registry.
**Prototype**: ``Microsoft.VisualBasic.Shoal.CLI::Int32 RegisterModule(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Shoal -register_modules -path <assemnly_dll_file> [-module_name <string_name>]
```


#### Arguments
##### -path
the assembly file path of the API module that you are going to register in the shellscript type library

###### Example
```bash
-path <term_string>
```
##### [-module_name]
The module name for the register type namespace, if the target assembly just have one shellscript namespace, then this switch value will override the namespace attribute value if the value of this switch is not null, when there are more than one shellscript namespace was declared in the module, then this switch opetion will be disabled.

###### Example
```bash
-module_name <term_string>
```
<h3 id="-scan.plugins"> 6. -scan.plugins</h3>

Scanning all of the avaliable shoal plugin modules in the specific directory and install all of them into the shoal registry.
**Prototype**: ``Microsoft.VisualBasic.Shoal.CLI::Int32 ScanPlugins(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Shoal -scan.plugins -dir <dir>[ -ext *.*/*.dll/*.exe/*.lib /top_only /clean]
```
###### Example
```bash
Shoal -scan.plugins -dir ./ -ext *.dll
```
<h3 id="set"> 7. set</h3>

Setting up the shoal environment variables, you can using var command to view all of the avaliable variable in the shoal shell.
**Prototype**: ``Microsoft.VisualBasic.Shoal.CLI::Int32 SetValue(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Shoal set <var_Name> <string_value>
```
###### Example
```bash
Shoal set lastdirasinit true
```
<h3 id="-start"> 8. -start</h3>

Start the shoal shell using the user custom data.
**Prototype**: ``Microsoft.VisualBasic.Shoal.CLI::Int32 Start(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Shoal -start -init_dir <inits_dir> -registry <regustry_xml> -imports <dll_paths>
```
<h3 id="var"> 9. var</h3>

Get the environment variable value in the shoal shell, if a variable name is not specific, then the shoal will list all of the variable value in shoal.
**Prototype**: ``Microsoft.VisualBasic.Shoal.CLI::Int32 GetValue(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Shoal var [<var_Name>]
```
###### Example
```bash
Shoal var registry_location
```
<h3 id="--version"> 10. --version</h3>

Print the version of the shoal shell in the console.
**Prototype**: ``Microsoft.VisualBasic.Shoal.CLI::Int32 Version()``

###### Usage
```bash
Shoal
```
