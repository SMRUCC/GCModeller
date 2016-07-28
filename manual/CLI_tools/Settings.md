---
title: Settings
tags: [maunal, tools]
date: 7/27/2016 6:40:25 PM
---
# Settings [version 1.0.0.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/Settings.exe
**Root namespace**: GCModeller.Configuration.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|Set|Setting up the configuration data node.|
|var|Gets the settings value.|

## Commands
--------------------------
##### Help for command 'Set':

**Prototype**: GCModeller.Configuration.CLI::Int32 Set(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Setting up the configuration data node.
  Usage:        G:\GCModeller\manual\bin\Settings.exe Set <varName> <value>
  Example:      Settings Set Set java /usr/lib/java/java.bin
```

##### Help for command 'var':

**Prototype**: GCModeller.Configuration.CLI::Int32 Var(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Gets the settings value.
  Usage:        G:\GCModeller\manual\bin\Settings.exe var [varName]
  Example:      Settings var 
```



  Parameters information:
```
       [[VarName]]
    Description:  If this value is null, then the program will prints all of the variables in the gcmodeller config file or when the variable is presents in the database, only the config value of the specific variable will be display.

    Example:      [VarName] ""


```

#### Accepted Types
##### [VarName]
