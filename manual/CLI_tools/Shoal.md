---
title: Shoal
tags: [maunal, tools]
date: 7/7/2016 6:52:04 PM
---
# ShoalShell [version 1.2.258.2033]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/Shoal.exe
**Root namespace**: Microsoft.VisualBasic.Shoal.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/debug|Start the shoal shell in debug output mode.|
|::|Execute one script line, this command is useful for the shoal API development and debugging.|
|~|Start the shoal shell in the current directory, not using the directory in the profile data.|
|--logs.show||
|-register_modules|Register the shellscript API module assembly DLL or assembly exe file to the shellscript type registry.|
|-scan.plugins|Scanning all of the avaliable shoal plugin modules in the specific directory and install all of them into the shoal registry.|
|set|Setting up the shoal environment variables, you can using var command to view all of the avaliable variable in the shoal shell.|
|-start|Start the shoal shell using the user custom data.|
|var|Get the environment variable value in the shoal shell, if a variable name is not specific, then the shoal will list all of the variable value in shoal.|
|--version|Print the version of the shoal shell in the console.|

## Commands
--------------------------
##### Help for command '/debug':

**Prototype**: Microsoft.VisualBasic.Shoal.CLI::Int32 DEBUG(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Start the shoal shell in debug output mode.
  Usage:        G:\GCModeller\manual\bin\Shoal.exe /debug listener_port <listen_port> [-work <working_Dir>]
  Example:      Shoal /debug 
```

##### Help for command '::':

**Prototype**: Microsoft.VisualBasic.Shoal.CLI::Int32 Shell(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Execute one script line, this command is useful for the shoal API development and debugging.
  Usage:        G:\GCModeller\manual\bin\Shoal.exe :: <scriptline>
  Example:      Shoal :: shoal :: "hello world!" -> msgbox title "This is a hello world tesing example!"
```

##### Help for command '~':

**Prototype**: Microsoft.VisualBasic.Shoal.CLI::Int32 Start()

```
  Information:  Start the shoal shell in the current directory, not using the directory in the profile data.
  Usage:        G:\GCModeller\manual\bin\Shoal.exe 
  Example:      Shoal ~ 
```

##### Help for command '--logs.show':

**Prototype**: Microsoft.VisualBasic.Shoal.CLI::Int32 ShowLogs()

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Shoal.exe 
  Example:      Shoal --logs.show 
```

##### Help for command '-register_modules':

**Prototype**: Microsoft.VisualBasic.Shoal.CLI::Int32 RegisterModule(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Register the shellscript API module assembly DLL or assembly exe file to the shellscript type registry.
  Usage:        G:\GCModeller\manual\bin\Shoal.exe -register_modules -path <assemnly_dll_file> [-module_name <string_name>]
  Example:      Shoal -register_modules 
```



  Parameters information:
```
    -path
    Description:  the assembly file path of the API module that you are going to register in the shellscript type library

    Example:      -path ""

   [-module_name]
    Description:  The module name for the register type namespace, if the target assembly just have one shellscript namespace, then this switch value will override the namespace attribute value if the value of this switch is not null, when there are more than one shellscript namespace was declared in the module, then this switch opetion will be disabled.

    Example:      -module_name ""


```

#### Accepted Types
##### -path
##### -module_name
##### Help for command '-scan.plugins':

**Prototype**: Microsoft.VisualBasic.Shoal.CLI::Int32 ScanPlugins(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Scanning all of the avaliable shoal plugin modules in the specific directory and install all of them into the shoal registry.
  Usage:        G:\GCModeller\manual\bin\Shoal.exe -scan.plugins -dir <dir>[ -ext *.*/*.dll/*.exe/*.lib /top_only /clean]
  Example:      Shoal -scan.plugins -scan.plugins -dir ./ -ext *.dll
```

##### Help for command 'set':

**Prototype**: Microsoft.VisualBasic.Shoal.CLI::Int32 SetValue(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Setting up the shoal environment variables, you can using var command to view all of the avaliable variable in the shoal shell.
  Usage:        G:\GCModeller\manual\bin\Shoal.exe set <var_Name> <string_value>
  Example:      Shoal set set lastdirasinit true
```

##### Help for command '-start':

**Prototype**: Microsoft.VisualBasic.Shoal.CLI::Int32 Start(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Start the shoal shell using the user custom data.
  Usage:        G:\GCModeller\manual\bin\Shoal.exe -start -init_dir <inits_dir> -registry <regustry_xml> -imports <dll_paths>
  Example:      Shoal -start 
```

##### Help for command 'var':

**Prototype**: Microsoft.VisualBasic.Shoal.CLI::Int32 GetValue(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Get the environment variable value in the shoal shell, if a variable name is not specific, then the shoal will list all of the variable value in shoal.
  Usage:        G:\GCModeller\manual\bin\Shoal.exe var [<var_Name>]
  Example:      Shoal var var registry_location
```

##### Help for command '--version':

**Prototype**: Microsoft.VisualBasic.Shoal.CLI::Int32 Version()

```
  Information:  Print the version of the shoal shell in the console.
  Usage:        G:\GCModeller\manual\bin\Shoal.exe 
  Example:      Shoal --version 
```

