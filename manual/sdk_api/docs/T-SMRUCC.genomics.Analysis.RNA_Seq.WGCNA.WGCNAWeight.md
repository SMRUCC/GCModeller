---
title: WGCNAWeight
---

# WGCNAWeight
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.WGCNA](N-SMRUCC.genomics.Analysis.RNA_Seq.WGCNA.html)_

包含有结果数据的加载模块以及脚本的执行调用模块



### Methods

#### Filtering
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.WGCNA.WGCNAWeight.Filtering(System.String[])
```
将目标对象相关的WGCNA weight值过滤出来，作为计算数据，以减少计算开销

|Parameter Name|Remarks|
|--------------|-------|
|IdList|-|


#### Find
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.WGCNA.WGCNAWeight.Find(System.String,System.String,System.Boolean)
```
找不到会返回空值

|Parameter Name|Remarks|
|--------------|-------|
|Id1|-|
|Id2|-|
|Parallel|可选参数，这个是为了控制并行计算的颗粒粒度而设置的参数，假若CPU利用率较低的话，可以尝试关闭本参数以增加颗粒粒度|



