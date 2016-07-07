---
title: InteropService
---

# InteropService
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService](N-SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService.html)_

InteropService to the local blast program.(对本地BLAST程序的中间服务)



### Methods

#### Blastn
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService.InteropService.Blastn(System.String,System.String,System.String,System.String)
```
Generate the command line arguments of the program blastn.(生成blastn程序的命令行参数)

|Parameter Name|Remarks|
|--------------|-------|
|Input|The target sequence FASTA file.(包含有目标待比对序列的FASTA文件)|
|TargetDb|The selected database that to aligned.(将要进行比对的目标FASTA数据库的文件名)|
|Output|结果文件|
|e|The E-value|


#### Blastp
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService.InteropService.Blastp(System.String,System.String,System.String,System.String)
```
Generate the command line arguments of the program blastp.(生成blastp程序的命令行参数)

|Parameter Name|Remarks|
|--------------|-------|
|InputQuery|The target sequence FASTA file.(包含有目标待比对序列的FASTA文件)|
|TargetSubjectDb|The selected database that to aligned.(将要进行比对的目标数据库)|
|Output|结果文件|
|e|The E-value|


#### FormatDb
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService.InteropService.FormatDb(System.String,System.String)
```
Format theta target fasta sequence database for the blast search.

|Parameter Name|Remarks|
|--------------|-------|
|Db|The name of the target formated db.(目标格式化数据库的名称)|
|dbType|Database type for the target sequence database(目标序列数据库的分子类型)|


#### GetLastLogFile
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService.InteropService.GetLastLogFile
```
Get the blast program output result file from the last BLAST operation. Filepath: @"P:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService.InteropService.LastBLASTOutputFilePath".
 (获取上一个BLAST操作所输出的结果日志文件)

#### TryInvoke
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService.InteropService.TryInvoke(System.String,System.String,System.String,System.String,System.String)
```
Try invoke the blast program based on its program name as the executable file name.

|Parameter Name|Remarks|
|--------------|-------|
|Program|The program file name|
|Query|-|
|Subject|-|
|Evalue|-|
|Output|-|



### Properties

#### LastBLASTOutputFilePath
Get the last blast operation output log file path.(获取上一次BLAST操作的输出文件的文件名)
#### MolTypeNucleotide
Gets the command line parameter of the nucleotide sequence molecular type.(获取核酸序列类型的分子序列的命令行参数的值)
#### MolTypeProtein
Gets the command line parameter of the protein sequence molecular type.(获取蛋白质分子序列的命令行参数的值)
