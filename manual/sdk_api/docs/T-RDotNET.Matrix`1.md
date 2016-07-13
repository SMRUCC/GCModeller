---
title: Matrix`1
---

# Matrix`1
_namespace: [RDotNET](N-RDotNET.html)_

A matrix base.



### Methods

#### #ctor
```csharp
RDotNET.Matrix`1.#ctor(RDotNET.REngine,System.IntPtr)
```
Creates a new instance for a matrix.

|Parameter Name|Remarks|
|--------------|-------|
|engine|The engine.|
|coerced|The pointer to a matrix.|


#### CopyTo
```csharp
],System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)
```
Copies the elements to the specified array.

|Parameter Name|Remarks|
|--------------|-------|
|destination|The destination array.|
|rowCount__1|The row length to copy.|
|columnCount__2|The column length to copy.|
|sourceRowIndex|The first row index of the matrix.|
|sourceColumnIndex|The first column index of the matrix.|
|destinationRowIndex|The first row index of the destination array.|
|destinationColumnIndex|The first column index of the destination array.|


#### GetArrayFast
```csharp
RDotNET.Matrix`1.GetArrayFast
```
Efficient conversion from R matrix representation to the array equivalent in the CLR
_returns: Array equivalent_

#### GetOffset
```csharp
RDotNET.Matrix`1.GetOffset(System.Int32,System.Int32)
```
Gets the offset for the specified indexes.

|Parameter Name|Remarks|
|--------------|-------|
|rowIndex|The index of row.|
|columnIndex|The index of column.|

_returns: The offset._

#### InitMatrixFastDirect
```csharp
])
```
Initializes this R matrix, using the values in a rectangular array.

|Parameter Name|Remarks|
|--------------|-------|
|matrix|-|


#### ToArray
```csharp
RDotNET.Matrix`1.ToArray
```
Gets a .NET representation as a two dimensional array of an R matrix


### Properties

#### ColumnCount
Gets the column size of elements.
#### ColumnNames
Gets the names of columns.
#### DataPointer
Gets the pointer for the first element.
#### DataSize
Gets the size of an element in byte.
#### Item
Gets or sets the element at the specified names.
#### ItemCount
Gets the total number of items (rows times columns) in this matrix
#### RowCount
Gets the row size of elements.
#### RowNames
Gets the names of rows.
