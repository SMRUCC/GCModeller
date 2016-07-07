---
title: ConsoleDevice
---

# ConsoleDevice
_namespace: [RDotNET.Devices](N-RDotNET.Devices.html)_

The default IO device, using the System.Console



### Methods

#### AddHistory
```csharp
RDotNET.Devices.ConsoleDevice.AddHistory(RDotNET.Language,RDotNET.SymbolicExpression,RDotNET.Pairlist,RDotNET.REnvironment)
```
Return the NULL SEXP; no other effect

|Parameter Name|Remarks|
|--------------|-------|
|call|-|
|operation|-|
|args|-|
|environment|-|


#### Ask
```csharp
RDotNET.Devices.ConsoleDevice.Ask(System.String)
```
Ask a question to the user with three choices.

|Parameter Name|Remarks|
|--------------|-------|
|question|The question to write to the console|


#### Busy
```csharp
RDotNET.Devices.ConsoleDevice.Busy(RDotNET.Internals.BusyType)
```
This implementation has no effect

|Parameter Name|Remarks|
|--------------|-------|
|which|-|


#### Callback
```csharp
RDotNET.Devices.ConsoleDevice.Callback
```
This implementation has no effect

#### ChooseFile
```csharp
RDotNET.Devices.ConsoleDevice.ChooseFile(System.Boolean)
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
RDotNET.Devices.ConsoleDevice.CleanUp(RDotNET.Internals.StartupSaveAction,System.Int32,System.Boolean)
```
Terminate the process with the given status

|Parameter Name|Remarks|
|--------------|-------|
|saveAction|Parameter is ignored|
|status|The status code on exit|
|runLast|Parameter is ignored|


#### ClearErrorConsole
```csharp
RDotNET.Devices.ConsoleDevice.ClearErrorConsole
```
Clears the System.Console

#### EditFile
```csharp
RDotNET.Devices.ConsoleDevice.EditFile(System.String)
```
This implementation does nothing

|Parameter Name|Remarks|
|--------------|-------|
|file|-|


#### FlushConsole
```csharp
RDotNET.Devices.ConsoleDevice.FlushConsole
```
Flush the System.Console

#### LoadHistory
```csharp
RDotNET.Devices.ConsoleDevice.LoadHistory(RDotNET.Language,RDotNET.SymbolicExpression,RDotNET.Pairlist,RDotNET.REnvironment)
```
Return the NULL SEXP; no other effect

|Parameter Name|Remarks|
|--------------|-------|
|call|-|
|operation|-|
|args|-|
|environment|-|


#### ReadConsole
```csharp
RDotNET.Devices.ConsoleDevice.ReadConsole(System.String,System.Int32,System.Boolean)
```
Read input from console.

|Parameter Name|Remarks|
|--------------|-------|
|prompt|The prompt message.|
|capacity|Parameter is ignored|
|history|Parameter is ignored|

_returns: The input._

#### ResetConsole
```csharp
RDotNET.Devices.ConsoleDevice.ResetConsole
```
Clears the System.Console

#### SaveHistory
```csharp
RDotNET.Devices.ConsoleDevice.SaveHistory(RDotNET.Language,RDotNET.SymbolicExpression,RDotNET.Pairlist,RDotNET.REnvironment)
```
Return the NULL SEXP; no other effect

|Parameter Name|Remarks|
|--------------|-------|
|call|-|
|operation|-|
|args|-|
|environment|-|


#### ShowFiles
```csharp
RDotNET.Devices.ConsoleDevice.ShowFiles(System.String[],System.String[],System.String,System.Boolean,System.String)
```
Displays the contents of files.

|Parameter Name|Remarks|
|--------------|-------|
|files|The file paths.|
|headers|The header before the contents is printed.|
|title|Ignored by this implementation|
|delete|Whether the file will be deleted.|
|pager|Ignored by this implementation|

_returns: true on successful completion, false if an IOException was caught_
> 
>  Only Unix.
>  

#### ShowMessage
```csharp
RDotNET.Devices.ConsoleDevice.ShowMessage(System.String)
```
Displays the message to the System.Console.

|Parameter Name|Remarks|
|--------------|-------|
|message|The message.|


#### Suicide
```csharp
RDotNET.Devices.ConsoleDevice.Suicide(System.String)
```
Write the message to standard error output stream.

|Parameter Name|Remarks|
|--------------|-------|
|message|-|


#### WriteConsole
```csharp
RDotNET.Devices.ConsoleDevice.WriteConsole(System.String,System.Int32,RDotNET.Internals.ConsoleOutputType)
```
Write output on console.

|Parameter Name|Remarks|
|--------------|-------|
|output|The output message|
|length|Parameter is ignored|
|outputType|Parameter is ignored|



