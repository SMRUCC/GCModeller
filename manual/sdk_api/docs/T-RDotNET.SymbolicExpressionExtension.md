---
title: SymbolicExpressionExtension
---

# SymbolicExpressionExtension
_namespace: [RDotNET](N-RDotNET.html)_

Provides extension methods for @"T:RDotNET.SymbolicExpression".



### Methods

#### AsCharacter
```csharp
RDotNET.SymbolicExpressionExtension.AsCharacter(RDotNET.SymbolicExpression)
```
Converts the specified expression to a CharacterVector.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The LogicalVector. Returns null if the specified expression is not vector._

#### AsCharacterMatrix
```csharp
RDotNET.SymbolicExpressionExtension.AsCharacterMatrix(RDotNET.SymbolicExpression)
```
Converts the specified expression to a CharacterMatrix.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The CharacterMatrix. Returns null if the specified expression is not vector._

#### AsComplex
```csharp
RDotNET.SymbolicExpressionExtension.AsComplex(RDotNET.SymbolicExpression)
```
Converts the specified expression to a ComplexVector.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The LogicalVector. Returns null if the specified expression is not vector._

#### AsComplexMatrix
```csharp
RDotNET.SymbolicExpressionExtension.AsComplexMatrix(RDotNET.SymbolicExpression)
```
Converts the specified expression to a ComplexMatrix.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The ComplexMatrix. Returns null if the specified expression is not vector._

#### AsDataFrame
```csharp
RDotNET.SymbolicExpressionExtension.AsDataFrame(RDotNET.SymbolicExpression)
```
Converts the specified expression to a DataFrame.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The DataFrame. Returns null if the specified expression is not vector._

#### AsEnvironment
```csharp
RDotNET.SymbolicExpressionExtension.AsEnvironment(RDotNET.SymbolicExpression)
```
Gets the expression as an @"T:RDotNET.REnvironment".

|Parameter Name|Remarks|
|--------------|-------|
|expression|The environment.|

_returns: The environment._

#### AsExpression
```csharp
RDotNET.SymbolicExpressionExtension.AsExpression(RDotNET.SymbolicExpression)
```
Gets the expression as an @"T:RDotNET.Expression".

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The expression._

#### AsFactor
```csharp
RDotNET.SymbolicExpressionExtension.AsFactor(RDotNET.SymbolicExpression)
```
Gets the expression as a factor.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The factor._

#### AsFunction
```csharp
RDotNET.SymbolicExpressionExtension.AsFunction(RDotNET.SymbolicExpression)
```
Gets the expression as a function.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The function._

#### AsInteger
```csharp
RDotNET.SymbolicExpressionExtension.AsInteger(RDotNET.SymbolicExpression)
```
Converts the specified expression to an IntegerVector.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The LogicalVector. Returns null if the specified expression is not vector._

#### AsIntegerMatrix
```csharp
RDotNET.SymbolicExpressionExtension.AsIntegerMatrix(RDotNET.SymbolicExpression)
```
Converts the specified expression to an IntegerMatrix.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The IntegerMatrix. Returns null if the specified expression is not vector._

#### AsLanguage
```csharp
RDotNET.SymbolicExpressionExtension.AsLanguage(RDotNET.SymbolicExpression)
```
Gets the expression as a language.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The language._

#### AsList
```csharp
RDotNET.SymbolicExpressionExtension.AsList(RDotNET.SymbolicExpression)
```
Converts the specified expression to a GenericVector.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The GenericVector. Returns null if the specified expression is not vector._

#### AsLogical
```csharp
RDotNET.SymbolicExpressionExtension.AsLogical(RDotNET.SymbolicExpression)
```
Converts the specified expression to a LogicalVector.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The LogicalVector. Returns null if the specified expression is not vector._

#### AsLogicalMatrix
```csharp
RDotNET.SymbolicExpressionExtension.AsLogicalMatrix(RDotNET.SymbolicExpression)
```
Converts the specified expression to a LogicalMatrix.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The LogicalMatrix. Returns null if the specified expression is not vector._

#### AsNumeric
```csharp
RDotNET.SymbolicExpressionExtension.AsNumeric(RDotNET.SymbolicExpression)
```
Converts the specified expression to a NumericVector.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The LogicalVector. Returns null if the specified expression is not vector._

#### AsNumericMatrix
```csharp
RDotNET.SymbolicExpressionExtension.AsNumericMatrix(RDotNET.SymbolicExpression)
```
Converts the specified expression to a NumericMatrix.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The NumericMatrix. Returns null if the specified expression is not vector._

#### AsRaw
```csharp
RDotNET.SymbolicExpressionExtension.AsRaw(RDotNET.SymbolicExpression)
```
Converts the specified expression to a RawVector.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The LogicalVector. Returns null if the specified expression is not vector._

#### AsRawMatrix
```csharp
RDotNET.SymbolicExpressionExtension.AsRawMatrix(RDotNET.SymbolicExpression)
```
Converts the specified expression to a RawMatrix.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The RawMatrix. Returns null if the specified expression is not vector._

#### AsS4
```csharp
RDotNET.SymbolicExpressionExtension.AsS4(RDotNET.SymbolicExpression)
```
Coerce the specified expression to an S4 object.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The DataFrame. Returns null if the specified expression is not vector._

#### AsSymbol
```csharp
RDotNET.SymbolicExpressionExtension.AsSymbol(RDotNET.SymbolicExpression)
```
Gets the expression as a symbol.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The symbol._

#### AsVector
```csharp
RDotNET.SymbolicExpressionExtension.AsVector(RDotNET.SymbolicExpression)
```
Converts the specified expression to a DynamicVector.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: The DynamicVector. Returns null if the specified expression is not vector._

#### IsDataFrame
```csharp
RDotNET.SymbolicExpressionExtension.IsDataFrame(RDotNET.SymbolicExpression)
```
Gets whether the specified expression is data frame.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: True if the specified expression is data frame._

#### IsEnvironment
```csharp
RDotNET.SymbolicExpressionExtension.IsEnvironment(RDotNET.SymbolicExpression)
```
Specifies the expression is an @"T:RDotNET.REnvironment" object or not.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: True if it is an environment._

#### IsExpression
```csharp
RDotNET.SymbolicExpressionExtension.IsExpression(RDotNET.SymbolicExpression)
```
Specifies the expression is an @"T:RDotNET.Expression" object or not.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: True if it is an expression._

#### IsFactor
```csharp
RDotNET.SymbolicExpressionExtension.IsFactor(RDotNET.SymbolicExpression)
```
Gets whether the specified expression is factor.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: True if the specified expression is factor._

#### IsFunction
```csharp
RDotNET.SymbolicExpressionExtension.IsFunction(RDotNET.SymbolicExpression)
```
Specifies the expression is a function object or not.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: True if it is a function._

#### IsLanguage
```csharp
RDotNET.SymbolicExpressionExtension.IsLanguage(RDotNET.SymbolicExpression)
```
Specifies the expression is a language object or not.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: True if it is a language._

#### IsList
```csharp
RDotNET.SymbolicExpressionExtension.IsList(RDotNET.SymbolicExpression)
```
Gets whether the specified expression is list.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: True if the specified expression is list._

#### IsMatrix
```csharp
RDotNET.SymbolicExpressionExtension.IsMatrix(RDotNET.SymbolicExpression)
```
Gets whether the specified expression is matrix.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: True if the specified expression is matrix._

#### IsS4
```csharp
RDotNET.SymbolicExpressionExtension.IsS4(RDotNET.SymbolicExpression)
```
Gets whether the specified expression is an S4 object.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: True if the specified expression is an S4 object._

#### IsSymbol
```csharp
RDotNET.SymbolicExpressionExtension.IsSymbol(RDotNET.SymbolicExpression)
```
Specifies the expression is a symbol object or not.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: True if it is a symbol._

#### IsVector
```csharp
RDotNET.SymbolicExpressionExtension.IsVector(RDotNET.SymbolicExpression)
```
Gets whether the specified expression is vector.

|Parameter Name|Remarks|
|--------------|-------|
|expression|The expression.|

_returns: True if the specified expression is vector._


### Properties

#### engine
A cache of the REngine - WARNING this assumes there can be only one per process, initialized once only.
