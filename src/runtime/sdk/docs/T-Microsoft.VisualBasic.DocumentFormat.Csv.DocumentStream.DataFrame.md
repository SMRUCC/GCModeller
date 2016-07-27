---
title: DataFrame
---

# DataFrame
_namespace: [Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream](N-Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.html)_

The dynamics data frame object which its first line is not contains the but using for the title property.
 (第一行总是没有的，即本对象类型适用于第一行为列标题行的数据)



### Methods

#### __reviewColumnHeader
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame.__reviewColumnHeader(System.String)
```
这里不能够使用Trim函数，因为Column也可能是故意定义了空格在其实或者结束的位置的，使用Trim函数之后，反而会导致GetOrder函数执行失败。故而在这里只给出警告信息即可

|Parameter Name|Remarks|
|--------------|-------|
|strValue|-|


#### ChangeMapping
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame.ChangeMapping(System.Collections.Generic.Dictionary{System.String,System.String})
```
``Csv.Field -> @"P:System.Reflection.MemberInfo.Name"``

|Parameter Name|Remarks|
|--------------|-------|
|MappingData|{oldFieldName, newFieldName}|


#### Close
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame.Close
```
Closes the @"T:System.Data.IDataReader":@"T:Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame" Object.

#### CopyFrom
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame.CopyFrom(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
```
这个方法会清除当前对象之中的原有数据

|Parameter Name|Remarks|
|--------------|-------|
|source|-|


#### CreateDataSource
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame.CreateDataSource
```
Get the lines data for the convinent data operation.(为了保持一致的顺序，这个函数是非并行化的)

#### GetOrdinal
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame.GetOrdinal(System.String)
```
Function return -1 when column not found.

|Parameter Name|Remarks|
|--------------|-------|
|Column|-|


#### GetOrdinalSchema
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame.GetOrdinalSchema(System.String[])
```
Gets the order list of the specific column list, -1 value will be returned when it is not exists in the table.
 (获取列集合的位置列表，不存在的列则返回-1)

|Parameter Name|Remarks|
|--------------|-------|
|ColumnList|-|

> 由于存在一一对应关系，这里不会再使用并行拓展

#### GetSchemaTable
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame.GetSchemaTable
```
Returns a System.Data.DataTable that describes the column metadata of the System.Data.IDataReader.
_returns: A System.Data.DataTable that describes the column metadata._

#### Load
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame.Load(System.String,System.Text.Encoding,System.Boolean)
```
Try loading a excel csv data file as a dynamics data frame object.(尝试加载一个Csv文件为数据框对象，请注意，第一行必须要为标题行)

|Parameter Name|Remarks|
|--------------|-------|
|path|-|
|encoding|-|


#### Read
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame.Read
```
The data frame object start to reading the data in this table, if the current pointer is reach 
 the top of the lines then this function will returns FALSE to stop the reading loop.

#### Reset
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame.Reset
```
Reset the reading position in the data frame object.


### Properties

#### __columnList
Using the first line of the csv row as the column headers in this csv file.
#### __currentPointer
@"F:Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.DataFrame.__currentLine"在@"F:Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File._innerTable"之中的位置
