---
title: NullCharacterDevice
---

# NullCharacterDevice
_namespace: [RDotNET.Devices](N-RDotNET.Devices.html)_

A sink with (almost) no effect, similar in purpose to /dev/null



### Methods

#### AddHistory
```csharp
RDotNET.Devices.NullCharacterDevice.AddHistory(RDotNET.Language,RDotNET.SymbolicExpression,RDotNET.Pairlist,RDotNET.REnvironment)
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
RDotNET.Devices.NullCharacterDevice.Ask(System.String)
```
Always return the default value of the YesNoCancel enum (yes?)

|Parameter Name|Remarks|
|--------------|-------|
|question|-|


#### Busy
```csharp
RDotNET.Devices.NullCharacterDevice.Busy(RDotNET.Internals.BusyType)
```
This implementation has no effect

|Parameter Name|Remarks|
|--------------|-------|
|which|-|


#### Callback
```csharp
RDotNET.Devices.NullCharacterDevice.Callback
```
This implementation has no effect

#### ChooseFile
```csharp
RDotNET.Devices.NullCharacterDevice.ChooseFile(System.Boolean)
```
Always returns null; no other side effect

|Parameter Name|Remarks|
|--------------|-------|
|create|ignored|

_returns: null_

#### CleanUp
```csharp
RDotNET.Devices.NullCharacterDevice.CleanUp(RDotNET.Internals.StartupSaveAction,System.Int32,System.Boolean)
```
Clean up action; exit the process with a specified status

|Parameter Name|Remarks|
|--------------|-------|
|saveAction|Ignored|
|status|-|
|runLast|Ignored|


#### ClearErrorConsole
```csharp
RDotNET.Devices.NullCharacterDevice.ClearErrorConsole
```
This implementation has no effect

#### EditFile
```csharp
RDotNET.Devices.NullCharacterDevice.EditFile(System.String)
```
No effect

|Parameter Name|Remarks|
|--------------|-------|
|file|-|


#### FlushConsole
```csharp
RDotNET.Devices.NullCharacterDevice.FlushConsole
```
This implementation has no effect

#### LoadHistory
```csharp
RDotNET.Devices.NullCharacterDevice.LoadHistory(RDotNET.Language,RDotNET.SymbolicExpression,RDotNET.Pairlist,RDotNET.REnvironment)
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
RDotNET.Devices.NullCharacterDevice.ReadConsole(System.String,System.Int32,System.Boolean)
```
Read input from console.

|Parameter Name|Remarks|
|--------------|-------|
|prompt|The prompt message.|
|capacity|The buffer's capacity in byte.|
|history|Whether the input should be added to any command history.|

_returns: A null reference_

#### ResetConsole
```csharp
RDotNET.Devices.NullCharacterDevice.ResetConsole
```
This implementation has no effect

#### SaveHistory
```csharp
RDotNET.Devices.NullCharacterDevice.SaveHistory(RDotNET.Language,RDotNET.SymbolicExpression,RDotNET.Pairlist,RDotNET.REnvironment)
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
RDotNET.Devices.NullCharacterDevice.ShowFiles(System.String[],System.String[],System.String,System.Boolean,System.String)
```
Always returns false, no other side effect

|Parameter Name|Remarks|
|--------------|-------|
|files|-|
|headers|-|
|title|-|
|delete|-|
|pager|-|

_returns: Returns false_

#### ShowMessage
```csharp
RDotNET.Devices.NullCharacterDevice.ShowMessage(System.String)
```
This implementation has no effect

|Parameter Name|Remarks|
|--------------|-------|
|message|The message.|


#### Suicide
```csharp
RDotNET.Devices.NullCharacterDevice.Suicide(System.String)
```
Ignores the message, but triggers a CleanUp, a termination with no action.

|Parameter Name|Remarks|
|--------------|-------|
|message|-|


#### WriteConsole
```csharp
RDotNET.Devices.NullCharacterDevice.WriteConsole(System.String,System.Int32,RDotNET.Internals.ConsoleOutputType)
```
This implementation has no effect

|Parameter Name|Remarks|
|--------------|-------|
|output|The output message|
|length|The output's length in byte.|
|outputType|The output type.|



