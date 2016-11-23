# RExtensionInvoke
_namespace: [RDotNET.Extensions.VisualBasic](./index.md)_

Wrapper for R engine script invoke.



### Methods

#### __call
```csharp
RDotNET.Extensions.VisualBasic.RExtensionInvoke.__call(System.String)
```
Evaluates a R statement in the given string.

|Parameter Name|Remarks|
|--------------|-------|
|expr|-|


#### function
```csharp
RDotNET.Extensions.VisualBasic.RExtensionInvoke.function(System.Collections.Generic.IEnumerable{System.String},System.String)
```
Declaring a function object in the R language

|Parameter Name|Remarks|
|--------------|-------|
|args|-|
|def|-|


#### q
```csharp
RDotNET.Extensions.VisualBasic.RExtensionInvoke.q(RDotNET.REngine)
```
Quite the R system.

#### WriteLine
```csharp
RDotNET.Extensions.VisualBasic.RExtensionInvoke.WriteLine(RDotNET.REngine,RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract.IRProvider)
```
获取来自于R服务器的输出，而不将结果打印于终端之上

|Parameter Name|Remarks|
|--------------|-------|
|script|-|



