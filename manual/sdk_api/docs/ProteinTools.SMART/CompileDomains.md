# CompileDomains
_namespace: [ProteinTools.SMART](./index.md)_





### Methods

#### Compile
```csharp
ProteinTools.SMART.CompileDomains.Compile(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.XmlFile.BlastOutput,System.String,SMRUCC.genomics.SequenceModel.FASTA.FastaFile,SMRUCC.genomics.Assembly.NCBI.CDD.DbFile)
```


|Parameter Name|Remarks|
|--------------|-------|
|BlastLogOutput|-|
|GrepScript|-|
|SubjectDb|Cdd数据库中导出来的序列数据|
|CddDb|与SubjectDb一致的Cdd数据库中的一个子库|


#### Performance
```csharp
ProteinTools.SMART.CompileDomains.Performance(System.String,System.String,System.String,System.String)
```
函数会返回缓存的目标蛋白质序列数据库中的蛋白质对象的结构域列表数据文件

|Parameter Name|Remarks|
|--------------|-------|
|QueryInput|将要进行编译的目标蛋白质数据库|



