---
title: Function
---

# Function
_namespace: [RDotNET](N-RDotNET.html)_

A function is one of closure, built-in function, or special function.



### Methods

#### #ctor
```csharp
RDotNET.Function.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a function object.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|pointer|The pointer.|


#### Invoke
```csharp
RDotNET.Function.Invoke(System.Collections.Generic.IDictionary{System.String,RDotNET.SymbolicExpression})
```
Executes the function. Match the function arguments by name.

|Parameter Name|Remarks|
|--------------|-------|
|args|The arguments, indexed by argument name|

_returns: The result of the function evaluation_

#### InvokeNamed
```csharp
RDotNET.Function.InvokeNamed(System.Tuple{System.String,RDotNET.SymbolicExpression}[])
```
Executes the function. Match the function arguments by name.

|Parameter Name|Remarks|
|--------------|-------|
|args|one or more tuples, conceptually a pairlist of arguments. The argument names must be unique|

_returns: The result of the function evaluation_

#### InvokeOrderedArguments
```csharp
RDotNET.Function.InvokeOrderedArguments(RDotNET.SymbolicExpression[])
```
Invoke this function with unnamed arguments.

|Parameter Name|Remarks|
|--------------|-------|
|args|The arguments passed to function call.|

_returns: The result of the function evaluation._

#### InvokeStrArgs
```csharp
RDotNET.Function.InvokeStrArgs(System.String[])
```
A convenience method to executes the function. Match the function arguments by order, after evaluating each to an R expression.

|Parameter Name|Remarks|
|--------------|-------|
|args|string representation of the arguments; each is evaluated to symbolic expression before being passed as argument to this object (i.e. this Function)|

_returns: The result of the function evaluation_

#### InvokeViaPairlist
```csharp
RDotNET.Function.InvokeViaPairlist(System.String[],RDotNET.SymbolicExpression[])
```
Executes the function. Match the function arguments by name.

|Parameter Name|Remarks|
|--------------|-------|
|argNames|The names of the arguments. These can be empty strings for unnamed function arguments|
|args|The arguments passed to the function|



