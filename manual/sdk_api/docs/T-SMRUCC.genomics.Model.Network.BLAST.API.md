---
title: API
---

# API
_namespace: [SMRUCC.genomics.Model.Network.BLAST](N-SMRUCC.genomics.Model.Network.BLAST.html)_

这个多用于宏基因组的研究



### Methods

#### BuildFromBBH
```csharp
SMRUCC.genomics.Model.Network.BLAST.API.BuildFromBBH(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit},System.Collections.Generic.Dictionary{System.String,System.String})
```


|Parameter Name|Remarks|
|--------------|-------|
|hits|-|
|locusDict|现在是通过查找字典的方法来获取基因组的名称，这样子可以避免不必要的麻烦|


#### BuildFromBlastHits
```csharp
SMRUCC.genomics.Model.Network.BLAST.API.BuildFromBlastHits(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit},System.Collections.Generic.Dictionary{System.String,System.String})
```
构建宏基因组网络里面的一个子网络

|Parameter Name|Remarks|
|--------------|-------|
|hits|-|


#### BuildFromBlastOUT
```csharp
SMRUCC.genomics.Model.Network.BLAST.API.BuildFromBlastOUT(System.String,System.Collections.Generic.Dictionary{System.String,System.String})
```


|Parameter Name|Remarks|
|--------------|-------|
|blastout|blast输出文件的路径|



