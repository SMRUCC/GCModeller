---
title: DOOR
---

# DOOR
_namespace: [SMRUCC.genomics.Assembly.DOOR](N-SMRUCC.genomics.Assembly.DOOR.html)_

DOOR: Database of prOkaryotic OpeRons.
 Door operon prediction data.(Door操纵子预测数据)



### Methods

#### Export
```csharp
SMRUCC.genomics.Assembly.DOOR.DOOR.Export(System.String,System.Boolean)
```
导出为一个Csv格式的文件

|Parameter Name|Remarks|
|--------------|-------|
|SavedPath|-|
|Trim|是否将仅有一个基因的Operon对象进行去除|


#### GetOperon
```csharp
SMRUCC.genomics.Assembly.DOOR.DOOR.GetOperon(System.String)
```
Gets the operon object where the gene is located.

|Parameter Name|Remarks|
|--------------|-------|
|locusId|The gene's locus_tag|


#### IsOprPromoter
```csharp
SMRUCC.genomics.Assembly.DOOR.DOOR.IsOprPromoter(System.String)
```
查看目标基因是否是其所处的操纵子的第一个基因

|Parameter Name|Remarks|
|--------------|-------|
|locus|-|



### Properties

#### Genes
在文件之中，是一个表格的形式来表示整个文件的，这个属性表示文件之中的所有行
#### GetGene
查找不到目标基因对象则会返回空值
