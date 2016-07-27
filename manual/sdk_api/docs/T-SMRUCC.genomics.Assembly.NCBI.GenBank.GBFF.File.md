---
title: File
---

# File
_namespace: [SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF](N-SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.html)_

NCBI GenBank database file.(NCBI GenBank数据库文件)



### Methods

#### Internal_readBlock
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File.Internal_readBlock(System.String,System.String[]@)
```
快速读取数据库文件中的某一个字段的文本块

|Parameter Name|Remarks|
|--------------|-------|
|keyword|字段名|

_returns: 该字段的内容_

#### Load
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File.Load(System.String)
```
当发生错误的时候，会返回空值

|Parameter Name|Remarks|
|--------------|-------|
|Path|-|


#### LoadDatabase
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File.LoadDatabase(System.String)
```
假若一个gbk文件之中包含有多个记录的话，可以使用这个函数进行数据的加载

|Parameter Name|Remarks|
|--------------|-------|
|filePath|The file path of the genbank database file, this gb file may contains sevral gb sections|


#### op_Implicit
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File.op_Implicit(System.String)~SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File
```
Read a specific GenBank database text file.
 (读取一个特定的GenBank数据库文件)

|Parameter Name|Remarks|
|--------------|-------|
|Path|The target database text file to read.(所要读取的目标数据库文件)|


#### Read
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File.Read(System.String)
```
将一个GBK文件从硬盘文件之中读取出来，当发生错误的时候，会抛出错误

|Parameter Name|Remarks|
|--------------|-------|
|Path|-|



### Properties

#### Accession
LocusID, GI or AccessionID
#### Definition
The definition value for this organism's GenBank data.
#### HasSequenceData
这个Genbank对象是否具有序列数据
#### IsPlasmidSource
这个Genbank对象是否为一个质粒的基因组数据
#### IsWGS
This GenBank data is the WGS(Whole genome shotgun) type data.
#### Locus
The brief entry information of this genbank data.
#### Origin
This GenBank keyword section stores the sequence data for this database.
#### SourceFeature
Gets the original source brief entry information of this genome.(获取这个基因组的摘要信息)
