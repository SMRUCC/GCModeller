---
title: heatmap2OUT
---

# heatmap2OUT
_namespace: [RDotNET.Extensions.VisualBasic.gplots](N-RDotNET.Extensions.VisualBasic.gplots.html)_





### Methods

#### __getMaps
```csharp
RDotNET.Extensions.VisualBasic.gplots.heatmap2OUT.__getMaps(System.Int32[],System.String[])
```


|Parameter Name|Remarks|
|--------------|-------|
|inds|索引的下标是从1开始的|
|locus|-|


#### GetColDendrogram
```csharp
RDotNET.Extensions.VisualBasic.gplots.heatmap2OUT.GetColDendrogram(System.Collections.Generic.Dictionary{System.String,System.String})
```
如果字典参数为空，则使用heatmap结果之中的默认字典

|Parameter Name|Remarks|
|--------------|-------|
|names|-|


#### GetRowDendrogram
```csharp
RDotNET.Extensions.VisualBasic.gplots.heatmap2OUT.GetRowDendrogram(System.Collections.Generic.Dictionary{System.String,System.String})
```
如果字典参数为空，则使用heatmap结果之中的默认字典

|Parameter Name|Remarks|
|--------------|-------|
|names|-|


#### RParser
```csharp
RDotNET.Extensions.VisualBasic.gplots.heatmap2OUT.RParser(System.String[],System.String[],System.String[])
```


|Parameter Name|Remarks|
|--------------|-------|
|out|heatmap.2输出结果|


#### TreeBuilder
```csharp
RDotNET.Extensions.VisualBasic.gplots.heatmap2OUT.TreeBuilder(System.String,System.Collections.Generic.Dictionary{System.String,System.String})
```


|Parameter Name|Remarks|
|--------------|-------|
|result|-|
|names|{id, name}|



### Properties

#### breaks
进行@"P:RDotNET.Extensions.VisualBasic.gplots.heatmap2OUT.col"映射的数值等级
#### col
热图里面的颜色代码
#### colInd

#### rowDendrogram
heatmap.2行聚类的结果，(基因)
#### rowInd
基因的排列顺序
