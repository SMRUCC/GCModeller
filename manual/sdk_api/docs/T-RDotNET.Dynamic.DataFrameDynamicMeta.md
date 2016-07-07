---
title: DataFrameDynamicMeta
---

# DataFrameDynamicMeta
_namespace: [RDotNET.Dynamic](N-RDotNET.Dynamic.html)_

Dynamic and binding logic for R data frames



### Methods

#### #ctor
```csharp
RDotNET.Dynamic.DataFrameDynamicMeta.#ctor(System.Linq.Expressions.Expression,RDotNET.DataFrame)
```
Creates a new object dealing with the dynamic and binding logic for R data frames

|Parameter Name|Remarks|
|--------------|-------|
|parameter|The expression representing this new DataFrameDynamicMeta in the binding process|
|frame|The runtime value of the DataFrame, that this new DataFrameDynamicMeta represents|


#### BindGetMember
```csharp
RDotNET.Dynamic.DataFrameDynamicMeta.BindGetMember(System.Dynamic.GetMemberBinder)
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
RDotNET.Dynamic.DataFrameDynamicMeta.GetDynamicMemberNames
```
Returns the enumeration of all dynamic member names.
_returns: The list of dynamic member names_


