---
title: DataFrame
---

# DataFrame
_namespace: [RDotNET](N-RDotNET.html)_

A data frame.



### Methods

#### #ctor
```csharp
RDotNET.DataFrame.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The @"T:RDotNET.REngine" handling this instance.|
|coerced|The pointer to a data frame.|


#### GetArrayFast
```csharp
RDotNET.DataFrame.GetArrayFast
```
Gets an array of the columns of this R data frame object

#### GetMetaObject
```csharp
RDotNET.DataFrame.GetMetaObject(System.Linq.Expressions.Expression)
```
returns a new DataFrameDynamicMeta for this DataFrame

|Parameter Name|Remarks|
|--------------|-------|
|parameter|-|


#### GetRow
```csharp
RDotNET.DataFrame.GetRow(System.Int32)
```
Gets the row at the specified index.

|Parameter Name|Remarks|
|--------------|-------|
|rowIndex|The index.|

_returns: The row._

#### GetRow``1
```csharp
RDotNET.DataFrame.GetRow``1(System.Int32)
```
Gets the row at the specified index mapping a specified class.
_returns: The row._

#### GetRows
```csharp
RDotNET.DataFrame.GetRows
```
Enumerates all the rows in the data frame.
_returns: The collection of the rows._

#### GetRows``1
```csharp
RDotNET.DataFrame.GetRows``1
```
Enumerates all the rows in the data frame mapping a specified class.
_returns: The collection of the rows._

#### SetVectorDirect
```csharp
RDotNET.DataFrame.SetVectorDirect(RDotNET.DynamicVector[])
```
Efficient initialisation of R vector values from an array representation in the CLR


### Properties

#### ColumnCount
Gets the number of kinds of data.
#### ColumnNames
Gets the names of columns.
#### DataSize
Gets the data size of each element in this vector, i.e. the offset in memory between elements.
#### Item
Gets or sets the element at the specified names.
#### RowCount
Gets the number of data sets.
#### RowNames
Gets the names of rows.
