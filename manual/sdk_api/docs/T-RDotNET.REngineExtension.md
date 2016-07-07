---
title: REngineExtension
---

# REngineExtension
_namespace: [RDotNET](N-RDotNET.html)_

Provides extension methods for @"T:RDotNET.REngine".



### Methods

#### CreateCharacter
```csharp
RDotNET.REngineExtension.CreateCharacter(RDotNET.REngine,System.String)
```
Create a vector with a single value

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|value|The value|

_returns: The new vector._

#### CreateCharacterMatrix
```csharp
])
```
Creates a new CharacterMatrix with the specified values.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|matrix|The values.|

_returns: The new matrix._

#### CreateCharacterVector
```csharp
RDotNET.REngineExtension.CreateCharacterVector(RDotNET.REngine,System.Collections.Generic.IEnumerable{System.String})
```
Creates a new CharacterVector with the specified values.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|vector|The values.|

_returns: The new vector._

#### CreateComplex
```csharp
RDotNET.REngineExtension.CreateComplex(RDotNET.REngine,System.Numerics.Complex)
```
Create a vector with a single value

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|value|The value|

_returns: The new vector._

#### CreateComplexMatrix
```csharp
])
```
Creates a new ComplexMatrix with the specified values.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|matrix|The values.|

_returns: The new matrix._

#### CreateComplexVector
```csharp
RDotNET.REngineExtension.CreateComplexVector(RDotNET.REngine,System.Collections.Generic.IEnumerable{System.Numerics.Complex})
```
Creates a new ComplexVector with the specified values.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|vector|The values.|

_returns: The new vector._

#### CreateDataFrame
```csharp
RDotNET.REngineExtension.CreateDataFrame(RDotNET.REngine,System.Collections.IEnumerable[],System.String[],System.String[],System.Boolean,System.Boolean,System.Boolean)
```
Create an R data frame from managed arrays and objects.

|Parameter Name|Remarks|
|--------------|-------|
|engine|R engine|
|columns|The columns with the values for the data frame. These must be array of supported types (double, string, bool, integer, byte)|
|columnNames|Column names. default: null.|
|rowNames|Row names. Default null.|
|checkRows|Check rows. See data.frame R documentation|
|checkNames|See data.frame R documentation|
|stringsAsFactors|Should columns of strings be considered as factors (categories). See data.frame R documentation|


#### CreateEnvironment
```csharp
RDotNET.REngineExtension.CreateEnvironment(RDotNET.REngine,RDotNET.REnvironment)
```
Creates a new environment.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|parent|The parent environment.|

_returns: The newly created environment._

#### CreateInteger
```csharp
RDotNET.REngineExtension.CreateInteger(RDotNET.REngine,System.Int32)
```
Create an integer vector with a single value

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|value|The value|

_returns: The new vector._

#### CreateIntegerMatrix
```csharp
])
```
Creates a new IntegerMatrix with the specified values.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|matrix|The values.|

_returns: The new matrix._

#### CreateIntegerVector
```csharp
RDotNET.REngineExtension.CreateIntegerVector(RDotNET.REngine,System.Collections.Generic.IEnumerable{System.Int32})
```
Creates a new IntegerVector with the specified values.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|vector|The values.|

_returns: The new vector._

#### CreateIsolatedEnvironment
```csharp
RDotNET.REngineExtension.CreateIsolatedEnvironment(RDotNET.REngine)
```
Creates a new isolated environment.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|

_returns: The newly created isolated environment._

#### CreateLogical
```csharp
RDotNET.REngineExtension.CreateLogical(RDotNET.REngine,System.Boolean)
```
Create a vector with a single value

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|value|The value|

_returns: The new vector._

#### CreateLogicalMatrix
```csharp
])
```
Creates a new LogicalMatrix with the specified values.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|matrix|The values.|

_returns: The new matrix._

#### CreateLogicalVector
```csharp
RDotNET.REngineExtension.CreateLogicalVector(RDotNET.REngine,System.Collections.Generic.IEnumerable{System.Boolean})
```
Creates a new LogicalVector with the specified values.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|vector|The values.|

_returns: The new vector._

#### CreateNumeric
```csharp
RDotNET.REngineExtension.CreateNumeric(RDotNET.REngine,System.Double)
```
Create a vector with a single value

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|value|The value|

_returns: The new vector._

#### CreateNumericMatrix
```csharp
])
```
Creates a new NumericMatrix with the specified values.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|matrix|The values.|

_returns: The new matrix._

#### CreateNumericVector
```csharp
RDotNET.REngineExtension.CreateNumericVector(RDotNET.REngine,System.Collections.Generic.IEnumerable{System.Double})
```
Creates a new NumericVector with the specified values.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|vector|The values.|

_returns: The new vector._

#### CreateRaw
```csharp
RDotNET.REngineExtension.CreateRaw(RDotNET.REngine,System.Byte)
```
Create a vector with a single value

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|value|The value|

_returns: The new vector._

#### CreateRawMatrix
```csharp
])
```
Creates a new RawMatrix with the specified values.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|matrix|The values.|

_returns: The new matrix._

#### CreateRawVector
```csharp
RDotNET.REngineExtension.CreateRawVector(RDotNET.REngine,System.Collections.Generic.IEnumerable{System.Byte})
```
Creates a new RawVector with the specified values.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|vector|The values.|

_returns: The new vector._


