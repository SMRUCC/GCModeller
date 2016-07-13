---
title: FileStream
---

# FileStream
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection](N-SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection.html)_





### Methods

#### CType``1
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.CType``1(SMRUCC.genomics.Assembly.MetaCyc.File.ObjectModel,System.Reflection.PropertyInfo[],System.Reflection.PropertyInfo[],SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection.MetaCycField[])
```
使用反射，将字典之中的数据赋值到相对应的属性之上

|Parameter Name|Remarks|
|--------------|-------|
|om|-|
|ItemProperties|-|
|FieldAttributes|-|
|TSchema|**TObject**的Schema的缓存|


#### Equals``1
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Equals``1(``0,``0,System.Reflection.PropertyInfo[],SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection.MetaCycField[],System.Nullable{System.Boolean})
```
采用类型信息缓存机制的等价判断函数，请保证ItemProperties和FieldAttributes着两个参数的列表的数目的一致性，（注意：在本函数中不允许出现空值）

|Parameter Name|Remarks|
|--------------|-------|
|objA|-|
|objB|-|
|ItemProperties|-|
|FieldAttributes|-|
|OverloadsNullable|无意义的参数，任意值|


#### Generate``1
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Generate``1(``0,System.Reflection.PropertyInfo[],SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection.MetaCycField[])
```
将一个MetaCyc数据库中的对象转换为字符串

|Parameter Name|Remarks|
|--------------|-------|
|e|-|
|props|-|
|Fieldattrs|-|


#### GetMetaCycField``1
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.GetMetaCycField``1(System.Reflection.PropertyInfo[]@,SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection.MetaCycField[]@)
```
获取一个MetaCyc记录类型对象的所有域以及相对应的属性信息

|Parameter Name|Remarks|
|--------------|-------|
|PropertyInfo|-|
|FieldAttributes|-|


#### Read``2
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read``2(System.String,``1@)
```


|Parameter Name|Remarks|
|--------------|-------|
|file|-|
|Stream|
 The stream object for output the read data, it must be construct first before call this method.
 (用于输出所读取的数据的流对象，其在调用本函数之前必须被构造出来)
 |


#### SplitSlotName
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.SplitSlotName(System.String)
```
将字符串按照大写字母进行分割，生成符合MetaCyc字段名称格式的字符串

|Parameter Name|Remarks|
|--------------|-------|
|SlotName|-|

> SlotName:  SLOT-NAME


