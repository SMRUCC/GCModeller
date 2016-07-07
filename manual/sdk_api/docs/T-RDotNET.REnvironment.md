---
title: REnvironment
---

# REnvironment
_namespace: [RDotNET](N-RDotNET.html)_

An environment object.



### Methods

#### #ctor
```csharp
RDotNET.REnvironment.#ctor(RDotNET.REngine,RDotNET.REnvironment)
```
Creates a new environment object.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|parent|The parent environment.|


#### GetSymbol
```csharp
RDotNET.REnvironment.GetSymbol(System.String)
```
Gets a symbol defined in this environment.

|Parameter Name|Remarks|
|--------------|-------|
|name|The name.|

_returns: The symbol._

#### GetSymbolNames
```csharp
RDotNET.REnvironment.GetSymbolNames(System.Boolean)
```
Gets the symbol names defined in this environment.

|Parameter Name|Remarks|
|--------------|-------|
|all|Including special functions or not.|

_returns: Symbol names._

#### SetSymbol
```csharp
RDotNET.REnvironment.SetSymbol(System.String,RDotNET.SymbolicExpression)
```
Defines a symbol in this environment.

|Parameter Name|Remarks|
|--------------|-------|
|name|The name.|
|expression|The symbol.|



### Properties

#### Parent
Gets the parental environment.
