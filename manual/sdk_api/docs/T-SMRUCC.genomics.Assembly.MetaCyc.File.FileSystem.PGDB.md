---
title: PGDB
---

# PGDB
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem](N-SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.html)_

The MetaCyc database file reader object.
 (MetaCyc数据库文件的读取对象)

> DataFiles


### Methods

#### Add
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.PGDB.Add(SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.PGDB,SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object)
```
Add the target element object into a specific table in a MetaCyc database object.
 (将目标元素对象添加进入一个MetaCyc数据库中的一张指定的数据表之中)

|Parameter Name|Remarks|
|--------------|-------|
|MetaCyc|-|
|e|-|


#### Load
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.PGDB.Load(System.String)
```
(使用反射机制进行数据库数据的加载操作，根据条件编译来选择为并行加载还是串行加载)

|Parameter Name|Remarks|
|--------------|-------|
|DIR|-|


#### PreLoad
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.PGDB.PreLoad(System.String,System.Boolean)
```
预加载MetaCyc数据库之中的部分数据

|Parameter Name|Remarks|
|--------------|-------|
|DIR|目标MetaCyc数据库的Data文件夹|


#### ReflectionLoadMetaCyc
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.PGDB.ReflectionLoadMetaCyc(System.String)
```
(使用反射机制进行数据库数据的加载操作)

|Parameter Name|Remarks|
|--------------|-------|
|Dir|-|


#### Save
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.PGDB.Save(System.String)
```
保存数据库，假若SavedDir参数不为空的话，则保存至SavedDir所指示的文件夹之内

|Parameter Name|Remarks|
|--------------|-------|
|EXPORT|可选参数，目标MetaCyc数据库所要保存的文件夹位置|



### Properties

#### AddingMethods
Adding methods for general object add to a specific data table in the metacyc database.
#### BindRxns
Binding reactions.(调控因子与DNA分子的结合反应)
#### Compounds
All of the chemical compounds and biomolecule that defines in this table.
 (细胞中的所有的化学物质和生物分子组成)
#### Proteins

#### ProtLigandCplxes
(蛋白质分子和其他的组分结合所形成的蛋白质复合物)
#### Regulations
Regulation rule of the gene expression and enzyme activity.(对基因表达过程的调控以及酶活力的调节)
#### WholeGenome
Get the whole genome sequence data for this species bacteria.
