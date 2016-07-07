---
title: BBHParser
---

# BBHParser
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH](N-SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.html)_

BBH解析的时候，是不会区分方向的，所以只要保证编号是一致的就会解析出结果，这个不需要担心



### Methods

#### __topBesthit
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BBHParser.__topBesthit(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit[])
```
假若没有最佳比对，则HitName为空值

|Parameter Name|Remarks|
|--------------|-------|
|Query|-|
|SubjectVsQuery|-|


#### BBHTop
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BBHParser.BBHTop(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
```
获取双向的最佳匹配结果.(只取出第一个最好的结果)

|Parameter Name|Remarks|
|--------------|-------|
|QvS|-|
|SvQ|-|


#### get_DiReBh
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BBHParser.get_DiReBh(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
```
获取双向的最佳匹配结果.(只取出第一个最好的结果)

|Parameter Name|Remarks|
|--------------|-------|
|QvS|-|
|SvQ|-|


#### GetBBHTop
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BBHParser.GetBBHTop(SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit[],SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit[],System.Double,System.Double)
```
Only using the first besthit paired result for the orthology data, if the query have no matches then using an empty string for the hit name.
 (只使用第一个做为最佳的双向结果，假若匹配不上，Hitname属性会为空字符串)

|Parameter Name|Remarks|
|--------------|-------|
|qvs|-|
|svq|-|


#### GetDirreBhAll
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BBHParser.GetDirreBhAll(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
```
导出所有的双向最佳比对结果，只要能够在双方的列表之中匹配上，则认为是最佳双向匹配

|Parameter Name|Remarks|
|--------------|-------|
|SvQ|-|
|QvS|-|

> 
>  取出所有符合条件的单向最佳的记录
>  

#### GetDirreBhAll2
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BBHParser.GetDirreBhAll2(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
```
导出所有的双向最佳比对结果

|Parameter Name|Remarks|
|--------------|-------|
|SvQ|-|
|QvS|-|

> 
>  取出所有符合条件的单向最佳的记录
>  

#### TopHit
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BBHParser.TopHit(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit})
```


|Parameter Name|Remarks|
|--------------|-------|
|hits|假设这里面的hits都是通过了cutoff了的数据|



