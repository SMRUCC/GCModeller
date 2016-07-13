---
title: DataFramework
---

# DataFramework
_namespace: [Microsoft.VisualBasic.ComponentModel.DataSourceModel](N-Microsoft.VisualBasic.ComponentModel.DataSourceModel.html)_

在目标对象中必须要具有一个属性有自定义属性@"T:Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps.DataFrameColumnAttribute"



### Methods

#### __toStringInternal
```csharp
Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFramework.__toStringInternal(System.Object)
```
出现错误的时候总是会返回空字符串的

|Parameter Name|Remarks|
|--------------|-------|
|obj|-|


#### CreateObject``1
```csharp
Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFramework.CreateObject``1(System.Collections.Generic.IEnumerable{``0})
```
Convert target data object collection into a datatable for the data source of the @"T:System.Windows.Forms.DataGridView">.
 (将目标对象集合转换为一个数据表对象，用作DataGridView控件的数据源)

|Parameter Name|Remarks|
|--------------|-------|
|source|-|


#### GetValue``1
```csharp
Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFramework.GetValue``1(System.Data.DataTable)
```
Retrive data from a specific datatable object.(从目标数据表中获取数据)

|Parameter Name|Remarks|
|--------------|-------|
|DataTable|-|


#### ValueToString
```csharp
Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFramework.ValueToString(System.Object)
```
Call @"T:System.Object" of the value types

|Parameter Name|Remarks|
|--------------|-------|
|x|Object should be @"T:System.Data.ValueType"|



### Properties

#### Flags
Controls for @"T:Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFramework.PropertyAccessibilityControls" on @"T:System.Reflection.PropertyInfo"
#### PrimitiveFromString
Converts the .NET primitive types from string.(将字符串数据类型转换为其他的数据类型)
#### ToStrings
Object @"T:System.Object" methods.
