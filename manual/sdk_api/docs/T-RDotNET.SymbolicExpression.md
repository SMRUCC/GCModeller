---
title: SymbolicExpression
---

# SymbolicExpression
_namespace: [RDotNET](N-RDotNET.html)_

An expression in R environment.



### Methods

#### #ctor
```csharp
RDotNET.SymbolicExpression.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates new instance of SymbolicExpression.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|pointer|The pointer.|


#### Equals
```csharp
RDotNET.SymbolicExpression.Equals(System.Object)
```
Test the equality of this object with another. If this object is also a SymbolicExpression and points to the same R expression, returns true.

|Parameter Name|Remarks|
|--------------|-------|
|obj|Other object to test for equality|

_returns: Returns true if pointing to the same R expression in memory._

#### GetAttribute
```csharp
RDotNET.SymbolicExpression.GetAttribute(System.String)
```
Gets the value of the specified name.

|Parameter Name|Remarks|
|--------------|-------|
|attributeName|The name of attribute.|

_returns: The attribute._

#### GetAttributeNames
```csharp
RDotNET.SymbolicExpression.GetAttributeNames
```
Gets all value names.
_returns: The names of attributes_

#### GetFunction``1
```csharp
RDotNET.SymbolicExpression.GetFunction``1
```
Creates the delegate function for the specified function defined in the DLL.
_returns: The delegate._

#### GetHashCode
```csharp
RDotNET.SymbolicExpression.GetHashCode
```
Returns the hash code for this instance.
_returns: Hash code_

#### GetMetaObject
```csharp
RDotNET.SymbolicExpression.GetMetaObject(System.Linq.Expressions.Expression)
```
returns a new SymbolicExpressionDynamicMeta for this SEXP

|Parameter Name|Remarks|
|--------------|-------|
|parameter|-|


#### op_Dynamic``1
```csharp
RDotNET.SymbolicExpression.op_Dynamic``1(RDotNET.SymbolicExpression,System.String)
```
Experimental

|Parameter Name|Remarks|
|--------------|-------|
|sexp|-|
|name|-|


#### Preserve
```csharp
RDotNET.SymbolicExpression.Preserve
```
Protects the expression from R's garbage collector.

#### ReleaseHandle
```csharp
RDotNET.SymbolicExpression.ReleaseHandle
```
Release the handle on the symbolic expression, i.e. tells R to decrement the reference count to the expression in unmanaged memory

#### SetAttribute
```csharp
RDotNET.SymbolicExpression.SetAttribute(System.String,RDotNET.SymbolicExpression)
```
Sets the new value to the attribute of the specified name.

|Parameter Name|Remarks|
|--------------|-------|
|attributeName|The name of attribute.|
|value|The value|


#### Unpreserve
```csharp
RDotNET.SymbolicExpression.Unpreserve
```
Stops protection.


### Properties

#### Engine
Gets the @"T:RDotNET.REngine" to which this expression belongs.
#### IsInvalid
Is the handle of this SEXP invalid (zero, i.e. null pointer)
#### IsProtected
Gets whether this expression is protected from the garbage collection.
#### lockObject
An object to use to get a lock on if EnableLock is true;
#### Type
Gets the @"T:RDotNET.Internals.SymbolicExpressionType".
