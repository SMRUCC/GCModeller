# VennDataModel
_namespace: [SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI](./index.md)_

Generates the Venn diagram data model using the bbh orthology method.(模块之中的方法可以应用于使用直系同源来创建文氏图)

> 
>  生成Venn表格所需要的步骤：
>  1. 按照基因组进行导出序列数据
>  2. 两两组合式的双向比对
>  3.
>  


### Methods

#### __parserIndex
```csharp
SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI.VennDataModel.__parserIndex(System.Collections.Generic.Dictionary{System.String,SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit},System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|index|
 进化比较的标尺
 假若为空字符串或者数字0以及first，都表示使用集合之中的第一个元素对象作为标尺
 假若参数值为某一个菌株的名称@``P:SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit.sp``，则会以该菌株的数据作为比对数据
 假若为last，则使用集合之中的最后一个
 对于其他的处于0-集合元素上限的数字，可以认识使用该集合之中的第i-1个元素对象
 还可以选择longest或者shortest参数值来作为最长或者最短的元素作为主标尺
 对于其他的任何无效的字符串，则默认使用第一个
 |


#### DeltaMove
```csharp
SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI.VennDataModel.DeltaMove(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit},System.String,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|mainIndex|
 进化比较的标尺
 假若为空字符串或者数字0以及first，都表示使用集合之中的第一个元素对象作为标尺
 假若参数值为某一个菌株的名称@``P:SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit.sp``，则会以该菌株的数据作为比对数据
 假若为last，则使用集合之中的最后一个
 对于其他的处于0-集合元素上限的数字，可以认识使用该集合之中的第i-1个元素对象
 还可以选择longest或者shortest参数值来作为最长或者最短的元素作为主标尺
 对于其他的任何无效的字符串，则默认使用第一个

 |


#### ExportBidirectionalBesthit
```csharp
SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI.VennDataModel.ExportBidirectionalBesthit(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.AlignEntry},System.String,System.String,System.Boolean)
```
批量导出最佳比对匹配结果

|Parameter Name|Remarks|
|--------------|-------|
|Source|单项最佳的两两比对的结果数据文件夹|
|EXPORT|双向最佳的导出文件夹|
|CDSAll|从GBK文件列表之中所导出来的蛋白质信息的汇总表|


#### NullHash
```csharp
SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI.VennDataModel.NullHash
```
If you don't want the export bbh data contains the protein description information or just don't know how the create the information, using this function to leave it blank.

#### OutputConservedCluster
```csharp
SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI.VennDataModel.OutputConservedCluster(SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.BestHit)
```
计算出可能的保守区域

|Parameter Name|Remarks|
|--------------|-------|
|bh|-|



