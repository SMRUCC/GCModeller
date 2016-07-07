---
title: DomnArch
---

# DomnArch
_namespace: [SMRUCC.genomics.Assembly.NCBI.CDD.Blastp](N-SMRUCC.genomics.Assembly.NCBI.CDD.Blastp.html)_

The blast+ program alignment output log file analysis module.
 (blast+程序的序列比对日志输出文件的分析模块)



### Methods

#### Load
```csharp
SMRUCC.genomics.Assembly.NCBI.CDD.Blastp.DomnArch.Load(System.String)
```
Restore a domain architecture model from a xml file.
 (从一个XML文件之中读取一个蛋白质结构域模型)

|Parameter Name|Remarks|
|--------------|-------|
|XmlFile|-|


#### op_Implicit
```csharp
SMRUCC.genomics.Assembly.NCBI.CDD.Blastp.DomnArch.op_Implicit(System.String)~SMRUCC.genomics.Assembly.NCBI.CDD.Blastp.DomnArch
```
Read the out put log file from a specific file.
 (从日个指定的文件之中读取输出日志，进而进行下一步的分析)

|Parameter Name|Remarks|
|--------------|-------|
|File|The path of the log file.(目标日志文件的文件路径)|


#### Parse
```csharp
SMRUCC.genomics.Assembly.NCBI.CDD.Blastp.DomnArch.Parse(System.String)
```
Parsing a blastp output log file and get the protein domains.
 (通过分析一个BLASTP分析得到的报告文件而获取一个蛋白质的结构数据)

|Parameter Name|Remarks|
|--------------|-------|
|LogFile|The path of the target log file.(目标日志文件的文件路径)|


#### Save
```csharp
SMRUCC.genomics.Assembly.NCBI.CDD.Blastp.DomnArch.Save(System.String)
```
Save the domain architecture model into a xml file.
 (将本蛋白质的结构域模型保存为一个XML文件)

|Parameter Name|Remarks|
|--------------|-------|
|XmlFile|-|



