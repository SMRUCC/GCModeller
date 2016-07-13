---
title: DataFrameRow
---

# DataFrameRow
_namespace: [RDotNET](N-RDotNET.html)_

A data frame row.



### Methods

#### #ctor
```csharp
RDotNET.DataFrameRow.#ctor(RDotNET.DataFrame,System.Int32)
```
Creates a new object representing a data frame row

|Parameter Name|Remarks|
|--------------|-------|
|frame|R Data frame|
|rowIndex|zero-based row index|


#### GetDynamicMemberNames
```csharp
RDotNET.DataFrameRow.GetDynamicMemberNames
```
Gets the column names of the data frame.

#### GetInnerValue
```csharp
RDotNET.DataFrameRow.GetInnerValue(System.Int32)
```
Gets the inner representation of the value; an integer if the column is a factor

|Parameter Name|Remarks|
|--------------|-------|
|index|-|


#### SetInnerValue
```csharp
RDotNET.DataFrameRow.SetInnerValue(System.Int32,System.Object)
```
Sets the inner representation of the value; an integer if the column is a factor

|Parameter Name|Remarks|
|--------------|-------|
|index|-|
|value|-|


#### TryGetMember
```csharp
RDotNET.DataFrameRow.TryGetMember(System.Dynamic.GetMemberBinder,System.Object@)
```
Try to get a member to a specified value

|Parameter Name|Remarks|
|--------------|-------|
|binder|Dynamic get member operation at the call site; Binder whose name should be one of the data frame column name|
|result|The value of the member|

_returns: false if setting failed_

#### TrySetMember
```csharp
RDotNET.DataFrameRow.TrySetMember(System.Dynamic.SetMemberBinder,System.Object)
```
Try to set a member to a specified value

|Parameter Name|Remarks|
|--------------|-------|
|binder|Dynamic set member operation at the call site; Binder whose name should be one of the data frame column name|
|value|The value to set|

_returns: false if setting failed_


### Properties

#### DataFrame
Gets the data frame containing this row.
#### Item
Gets and sets the value at the specified column.
#### RowIndex
Gets the index of this row.
