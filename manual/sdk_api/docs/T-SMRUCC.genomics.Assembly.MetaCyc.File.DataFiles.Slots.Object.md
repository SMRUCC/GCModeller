---
title: Object
---

# Object
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots](N-SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.html)_

The object type is the base type of the objects definition both in the namespace PGDB.DataFile and PGDB.Schemas



### Methods

#### Exists
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object.Exists(System.String)
```
查询某一个键名是否存在于这个对象之中

|Parameter Name|Remarks|
|--------------|-------|
|KeyName|键名|


#### StringQuery
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object.StringQuery(System.String,System.Boolean)
```
使用关键词查询@"F:SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object._innerHash"字典对象

|Parameter Name|Remarks|
|--------------|-------|
|Key|-|


#### TypeCast``1
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object.TypeCast``1(SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object,``0@)
```
基类至派生类的转换

|Parameter Name|Remarks|
|--------------|-------|
|target|数据源，基类|
|ToType|转换至的目标类型|



### Properties

#### AbbrevName
(Abbrev-Name) This slot stores an abbreviated name for an object. It is used in 
 some displays.
#### Citations
(Citations) This slot lists general citations pertaining to the object containing 
 the slot. Citations may or may not have evidence codes attached to them. Each value 
 of the slot is a string of the form 
 [reference-ID] or 
 [reference-id:evidence-code:timestamp:curator:probability:with]
#### Comment
(Comment) The Comment slot stores a general comment about the object that contains 
 the slot. The comment should always be enclosed in double quotes.
#### CommonName
(Common-Name) This slot defines the primary name by which an object is known 
 to scientists -- a widely used and familiar name (in some cases arbitrary 
 choices must be made). This field can have only one value; that value must 
 be a string.
#### Synonyms
(Synonyms) This field defines one or more secondary names for an object -- names 
 that a scientist might attempt to use to retrieve the object. These names may be 
 out of date or ambiguous, but are used to facilitate retrieval -- the Synonyms 
 should include any name that you might use to try to retrieve an object. In a 
 sense, the name "Synonyms" is misleading because the names listed in this slot may 
 not be exactly synonymous with the preferred name of the object.
#### Table
当前的对象所属的表对象
#### Types
The TYPES enumerate values in each object.
#### UNIQUE_ID_REGX
(解析Unique-Id字段的值所需要的正则表达式)
