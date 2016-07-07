---
title: StorageProvider
---

# StorageProvider
_namespace: [Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.ComponentModels](N-Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.ComponentModels.html)_

数据读写对象的基本类型



### Methods

#### ToString
```csharp
Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.ComponentModels.StorageProvider.ToString(System.Object)
```
By using this function that save the property value as a cell value string

|Parameter Name|Remarks|
|--------------|-------|
|obj|-|



### Properties

#### BindProperty
The bind property in the reflected class object.(在反射的类型定义之中所绑定的属性入口定义)
#### CanReadDataFromObject
从目标类型对象之中可以读取这个属性的值将数据写入到文件之中
#### CanWriteDataToObject
可以在读取Csv文件之中的数据之后将数据写入到这个属性之中从而将数据加载进入内存之中
#### Name
假若目标属性之中没有提供名称的话，则会使用属性名称来代替
#### Ordinal
这个属性值在Csv文件的第几列？
