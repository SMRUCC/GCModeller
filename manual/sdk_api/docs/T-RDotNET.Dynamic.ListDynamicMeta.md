---
title: ListDynamicMeta
---

# ListDynamicMeta
_namespace: [RDotNET.Dynamic](N-RDotNET.Dynamic.html)_

Dynamic and binding logic for R lists



### Methods

#### #ctor
```csharp
RDotNET.Dynamic.ListDynamicMeta.#ctor(System.Linq.Expressions.Expression,RDotNET.GenericVector)
```
Creates a new object dealing with the dynamic and binding logic for R lists

|Parameter Name|Remarks|
|--------------|-------|
|parameter|The expression representing this new ListDynamicMeta in the binding process|
|list|The runtime value of the GenericVector, that this new ListDynamicMeta represents|


#### BindGetMember
```csharp
RDotNET.Dynamic.ListDynamicMeta.BindGetMember(System.Dynamic.GetMemberBinder)
```
Performs the binding of the dynamic get member operation.

|Parameter Name|Remarks|
|--------------|-------|
|binder|
 An instance of the System.Dynamic.GetMemberBinder that represents the details of the dynamic operation.
 |

_returns: The new System.Dynamic.DynamicMetaObject representing the result of the binding._

#### GetDynamicMemberNames
```csharp
RDotNET.Dynamic.ListDynamicMeta.GetDynamicMemberNames
```
Returns the enumeration of all dynamic member names.
_returns: The list of dynamic member names_


