---
title: Closure
---

# Closure
_namespace: [RDotNET](N-RDotNET.html)_

A closure.



### Methods

#### #ctor
```csharp
RDotNET.Closure.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a closure object.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|pointer|The pointer.|


#### Invoke
```csharp
RDotNET.Closure.Invoke(System.Collections.Generic.IDictionary{System.String,RDotNET.SymbolicExpression})
```
Invoke this function, using named arguments provided as key-value pairs

|Parameter Name|Remarks|
|--------------|-------|
|args|the representation of named arguments, as a dictionary|

_returns: The result of the evaluation_


### Properties

#### Arguments
Gets the arguments list.
#### Body
Gets the body.
#### Environment
Gets the environment.
