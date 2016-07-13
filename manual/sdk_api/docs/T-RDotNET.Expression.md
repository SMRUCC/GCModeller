---
title: Expression
---

# Expression
_namespace: [RDotNET](N-RDotNET.html)_

An expression object.



### Methods

#### #ctor
```csharp
RDotNET.Expression.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates an expression object.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|pointer|The pointer to an expression.|


#### Evaluate
```csharp
RDotNET.Expression.Evaluate(RDotNET.REnvironment)
```
Evaluates the expression in the specified environment.

|Parameter Name|Remarks|
|--------------|-------|
|environment|The environment.|

_returns: The evaluation result._

#### TryEvaluate
```csharp
RDotNET.Expression.TryEvaluate(RDotNET.REnvironment,RDotNET.SymbolicExpression@)
```
Evaluates the expression in the specified environment.

|Parameter Name|Remarks|
|--------------|-------|
|environment|The environment.|
|result|The evaluation result, or null if the evaluation failed|

_returns: True if the evaluation succeeded._


