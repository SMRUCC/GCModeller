---
title: NomenclatureDB
---

# NomenclatureDB
_namespace: [SMRUCC.genomics.Assembly.Expasy.Database](N-SMRUCC.genomics.Assembly.Expasy.Database.html)_

ENZYME nomenclature database.(Expasy数据库之中的enzyme.dat注释文件)



### Methods

#### CreateObject
```csharp
SMRUCC.genomics.Assembly.Expasy.Database.NomenclatureDB.CreateObject(System.String)
```
Load the expasy database from the text file.(从文本文件之中加载expasy数据库)

|Parameter Name|Remarks|
|--------------|-------|
|File|File path of the expasy database file.(exapsy数据库文件的文件路径)|


#### GetSwissProtEntries
```csharp
SMRUCC.genomics.Assembly.Expasy.Database.NomenclatureDB.GetSwissProtEntries(System.String,System.Boolean)
```
从Expasy数据库之中导出某一种酶分类编号的所有的Uniprot数据库之中的蛋白质编号

|Parameter Name|Remarks|
|--------------|-------|
|ECNumber|-|
|Strict|是否严格匹配酶分类编号，假若严格匹配的话，则必须要字符串完全相等才会有输出结果，假若不严格匹配，理论上假若所输入的酶分类标号有通配符，即相连着的两个".."符号存在的话，则所有该大分类之下的所有的蛋白酶分子的编号都会被输出|



### Properties

#### DataItem
当目标编号不存在于Expasy数据库之中的时候会返回空值
