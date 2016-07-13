---
title: SymbolicExpressionDynamicMeta
---

# SymbolicExpressionDynamicMeta
_namespace: [RDotNET.Dynamic](N-RDotNET.Dynamic.html)_

Dynamic and binding logic for S expressions



### Methods

#### #ctor
```csharp
RDotNET.Dynamic.SymbolicExpressionDynamicMeta.#ctor(System.Linq.Expressions.Expression,RDotNET.SymbolicExpression)
```
Dynamic and binding logic for S expressions

|Parameter Name|Remarks|
|--------------|-------|
|parameter|The expression representing this new SymbolicExpressionDynamicMeta in the binding process|
|expression|The runtime value of this SymbolicExpression represented by this new SymbolicExpressionDynamicMeta|


#### BindGetMember
```csharp
RDotNET.Dynamic.SymbolicExpressionDynamicMeta.BindGetMember(System.Dynamic.GetMemberBinder)
```
Performs the binding of the dynamic get member operation.

|Parameter Name|Remarks|
|--------------|-------|
|binder|
 An instance of the System.Dynamic.GetMemberBinder that represents the details of the dynamic operation.
 |

_returns: The new System.Dynamic.DynamicMetaObject representing the result of the binding._

#### BindGetMember``2
```csharp
RDotNET.Dynamic.SymbolicExpressionDynamicMeta.BindGetMember``2(System.Dynamic.GetMemberBinder,System.Type[])
```
Creates the binding of the dynamic get member operation.

|Parameter Name|Remarks|
|--------------|-------|
|binder|The binder; its name must be one of the names of the R object represented by this meta object|
|indexerNameType|-|


#### GetDynamicMemberNames
```csharp
RDotNET.Dynamic.SymbolicExpressionDynamicMeta.GetDynamicMemberNames
```
Returns the enumeration of all dynamic member names.
_returns: The list of dynamic member names_


### Properties

#### Empty
A string array of length zero
