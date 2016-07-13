---
title: API
---

# API
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA](N-SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA.html)_





### Methods

#### CallInvoke
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA.API.CallInvoke(System.String,System.String,System.String,System.String,System.String)
```
函数返回的是最终的WGCNA导出到Cytoscape的网络模型文件的文件路径，假若脚本执行失败，则返回空字符串

|Parameter Name|Remarks|
|--------------|-------|
|dataExpr|The text encoding of this document should be ASCII, or the data reading in the R will be failed!(转录组数据的csv文件的位置，请注意！，数据文件都必须是ASCII编码的)|
|GeneIdLabel|使用这个参数来修改Id映射|



