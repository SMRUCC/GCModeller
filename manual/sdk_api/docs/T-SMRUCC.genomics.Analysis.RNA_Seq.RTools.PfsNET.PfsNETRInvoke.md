---
title: PfsNETRInvoke
---

# PfsNETRInvoke
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET](N-SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.html)_

PFSNet computes signifiance of subnetworks generated through a process that selects genes in a pathway based on fuzzy scoring and a majority voting procedure.
 (一个程序仅一个实例，是否是因为将Module修改为Class的原因所以导致了在64位服务器上面的Java初始化失败？？？？)



### Methods

#### Evaluate
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.PfsNETRInvoke.Evaluate(System.String,System.String,System.String,System.String,System.String,System.String,System.String)
```
R脚本版本的PfsNET计算引擎

|Parameter Name|Remarks|
|--------------|-------|
|file1|-|
|file2|-|
|file3|-|
|b|-|
|t1|-|
|t2|-|
|n|-|


#### InitializeSession
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.PfsNETRInvoke.InitializeSession(System.String,System.String)
```
假若**java_class**参数为空，则初始化为非rJava的调用版本

|Parameter Name|Remarks|
|--------------|-------|
|java_class|-|
|R_HOME|-|



