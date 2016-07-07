---
title: BLASTPlus
---

# BLASTPlus
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs](N-SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.html)_

请注意：当目标FASTA序列文件的文件路径中的空格字符过多的时候，BLAST+程序组将不能够很好的工作，故而当程序出错的时候，请检查文件路径是否存在此问题



### Methods

#### Blastp
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.BLASTPlus.Blastp(System.String,System.String,System.String,System.String)
```
这个函数只是生成了一个blastp的操作任务，但是并没有被启动

|Parameter Name|Remarks|
|--------------|-------|
|Input|-|
|TargetDb|-|
|Output|-|
|e|-|


#### GetHelp
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.BLASTPlus.GetHelp
```
USAGE and DESCRIPTION

#### GetManual
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.BLASTPlus.GetManual
```
Get USAGE, DESCRIPTION and ARGUMENTS


### Properties

#### MolTypeNucleotide
The nucleotide molecular type value for the makeblastdb operation.(用于makeblastdb操作所需要的核酸链分子的命令行参数值)
#### MolTypeProtein
The protein molecular type value for the makeblastdb operation.(用于makeblastdb操作所需要的蛋白质分子的命令行参数值)
#### Version
Gets the blast program group version.(获取blast程序组的版本信息)
