---
title: FileSystem
---

# FileSystem
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.GCModeller.FileSystem](N-SMRUCC.genomics.GCModeller.ModellingEngine.GCModeller.FileSystem.html)_



> 由于可能会修改参数然后在调用的这种情况出现，所以这里的数据可能需要实时更新，所以不再使用属性的简写形式了


### Methods

#### GetLocalBlast
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.GCModeller.FileSystem.FileSystem.GetLocalBlast
```
会自动搜索注册表，配置文件和文件系统之上的目录，实在找不到会返回空字符串并且记录下错误

#### GetMotifLDM
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.GCModeller.FileSystem.FileSystem.GetMotifLDM(System.String)
```
<RegpreciseRoot>/MEME/MAST_LDM/

#### GetPfamDb
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.GCModeller.FileSystem.FileSystem.GetPfamDb(System.String)
```
默认返回NCBI CDD数据库之中的Pfam数据库

|Parameter Name|Remarks|
|--------------|-------|
|name|-|


#### GetRegulations
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.GCModeller.FileSystem.FileSystem.GetRegulations
```
regulations.xml文件在GCModeller数据库之中的位置

#### GetRepositoryRoot
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.GCModeller.FileSystem.FileSystem.GetRepositoryRoot
```
The root directory for stores the GCModeller database such as fasta sequence for annotation.

#### IsRepositoryNullOrEmpty
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.GCModeller.FileSystem.FileSystem.IsRepositoryNullOrEmpty
```
配置文件之中是否包含有GCModeller数据库的位置的路径参数

#### TryGetSource
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.GCModeller.FileSystem.FileSystem.TryGetSource(System.String,System.Func{System.String})
```
这个是为了加载数据而构建的，故而假若数据源不存在的话就会返回备用的

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|alt|-|



### Properties

#### CDD
NCBI CDD数据库的文件夹位置
#### IsNullOrEmpty
配置文件之中是否包含有GCModeller数据库的位置的路径参数
#### KEGGFamilies
Regprecise数据库之中的调控因子蛋白质的摘要Dump信息
#### MotifLDM
<RegpreciseRoot>/MEME/MAST_LDM/
#### RegpreciseRoot
<RepositoryRoot>/Regprecise/
#### Regulations
regulations.xml文件在GCModeller数据库之中的位置
#### RepositoryRoot
The root directory for stores the GCModeller database such as fasta sequence for annotation.
