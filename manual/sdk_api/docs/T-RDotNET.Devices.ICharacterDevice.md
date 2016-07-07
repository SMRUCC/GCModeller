---
title: ICharacterDevice
---

# ICharacterDevice
_namespace: [RDotNET.Devices](N-RDotNET.Devices.html)_

A console class handles user's inputs and outputs.



### Methods

#### AddHistory
```csharp
RDotNET.Devices.ICharacterDevice.AddHistory(RDotNET.Language,RDotNET.SymbolicExpression,RDotNET.Pairlist,RDotNET.REnvironment)
```

> 
>  Only Unix.
>  

#### Ask
```csharp
RDotNET.Devices.ICharacterDevice.Ask(System.String)
```
Asks user's decision.

|Parameter Name|Remarks|
|--------------|-------|
|question|The question.|

_returns: User's decision._

#### Busy
```csharp
RDotNET.Devices.ICharacterDevice.Busy(RDotNET.Internals.BusyType)
```
Invokes actions.

|Parameter Name|Remarks|
|--------------|-------|
|which|The state.|


#### Callback
```csharp
RDotNET.Devices.ICharacterDevice.Callback
```
Callback function.

#### ChooseFile
```csharp
RDotNET.Devices.ICharacterDevice.ChooseFile(System.Boolean)
```
Chooses a file.

|Parameter Name|Remarks|
|--------------|-------|
|create|To be created.|

_returns: The length of input._
> 
>  Only Unix.
>  

#### CleanUp
```csharp
RDotNET.Devices.ICharacterDevice.CleanUp(RDotNET.Internals.StartupSaveAction,System.Int32,System.Boolean)
```
Invokes any actions which occur at system termination.

|Parameter Name|Remarks|
|--------------|-------|
|saveAction|The save type.|
|status|Exit code.|
|runLast|Whether R should execute '.Last'.|

> 
>  Only Unix.
>  

#### ClearErrorConsole
```csharp
RDotNET.Devices.ICharacterDevice.ClearErrorConsole
```
Clear the error console.
> 
>  Only Unix.
>  

#### EditFile
```csharp
RDotNET.Devices.ICharacterDevice.EditFile(System.String)
```

> 
>  Only Unix.
>  

#### FlushConsole
```csharp
RDotNET.Devices.ICharacterDevice.FlushConsole
```
Flush the console.
> 
>  Only Unix.
>  

#### LoadHistory
```csharp
RDotNET.Devices.ICharacterDevice.LoadHistory(RDotNET.Language,RDotNET.SymbolicExpression,RDotNET.Pairlist,RDotNET.REnvironment)
```

> 
>  Only Unix.
>  

#### ReadConsole
```csharp
RDotNET.Devices.ICharacterDevice.ReadConsole(System.String,System.Int32,System.Boolean)
```
Read input from console.

|Parameter Name|Remarks|
|--------------|-------|
|prompt|The prompt message.|
|capacity|The buffer's capacity in byte.|
|history|Whether the input should be added to any command history.|

_returns: The input._

#### ResetConsole
```csharp
RDotNET.Devices.ICharacterDevice.ResetConsole
```
Clear the console.
> 
>  Only Unix.
>  

#### SaveHistory
```csharp
RDotNET.Devices.ICharacterDevice.SaveHistory(RDotNET.Language,RDotNET.SymbolicExpression,RDotNET.Pairlist,RDotNET.REnvironment)
```

> 
>  Only Unix.
>  

#### ShowFiles
```csharp
RDotNET.Devices.ICharacterDevice.ShowFiles(System.String[],System.String[],System.String,System.Boolean,System.String)
```
Displays the contents of files.

|Parameter Name|Remarks|
|--------------|-------|
|files|The file paths.|
|headers|The header before the contents is printed.|
|title|The window title.|
|delete|Whether the file will be deleted.|
|pager|The pager used.|

> 
>  Only Unix.
>  

#### ShowMessage
```csharp
RDotNET.Devices.ICharacterDevice.ShowMessage(System.String)
```
Displays the message.

|Parameter Name|Remarks|
|--------------|-------|
|message|The message.|

> 
>  It should be brought to the user's attention immediately.
>  

#### Suicide
```csharp
RDotNET.Devices.ICharacterDevice.Suicide(System.String)
```
Abort R environment itself as soon as possible.

|Parameter Name|Remarks|
|--------------|-------|
|message|The message.|

> 
>  Only Unix.
>  

#### WriteConsole
```csharp
RDotNET.Devices.ICharacterDevice.WriteConsole(System.String,System.Int32,RDotNET.Internals.ConsoleOutputType)
```
Write output on console.

|Parameter Name|Remarks|
|--------------|-------|
|output|The output message|
|length|The output's length in byte.|
|outputType|The output type.|



