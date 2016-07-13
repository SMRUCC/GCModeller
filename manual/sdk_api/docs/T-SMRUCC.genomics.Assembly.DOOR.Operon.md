---
title: Operon
---

# Operon
_namespace: [SMRUCC.genomics.Assembly.DOOR](N-SMRUCC.genomics.Assembly.DOOR.html)_

@"P:SMRUCC.genomics.Assembly.DOOR.Operon.Genes"[操纵子中的基因]在构造函数之中已经进行过按照转录方向排序操作了的



### Methods

#### ConvertToCsvData
```csharp
SMRUCC.genomics.Assembly.DOOR.Operon.ConvertToCsvData
```
将目标Door操纵子对象转换为Csv格式的数据

#### GetLast
```csharp
SMRUCC.genomics.Assembly.DOOR.Operon.GetLast(System.String)
```
依照Operon的方向，获取**GeneId[目标基因对象]**之后的所有基因

|Parameter Name|Remarks|
|--------------|-------|
|GeneId|假若本参数值为操纵子的@"P:SMRUCC.genomics.Assembly.DOOR.Operon.InitialX"[启动子基因]的话，则会返回整个操纵子，
 反之为操纵子之中的最后一个基因的话，则返回最后一个基因，
 若为中间的一个基因的话，则返回该基因以及其后面的所有基因，“后面”是依照@"P:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation.Strand"[转录方向]来判定的|


#### HaveGene
```csharp
SMRUCC.genomics.Assembly.DOOR.Operon.HaveGene(System.String)
```
Has the gene with specific locusId in this operon?

|Parameter Name|Remarks|
|--------------|-------|
|locusId|-|


#### IndexOf
```csharp
SMRUCC.genomics.Assembly.DOOR.Operon.IndexOf(System.String)
```
查看目标基因是否是本操纵子之中的结构基因

|Parameter Name|Remarks|
|--------------|-------|
|GeneId|-|



### Properties

#### Genes
这个属性会返回本Operon里面的一组基因，基因的位置和其在基因组上面的位置相关，并且与链的方向相关
#### InitialX
Get the operon initial structure (promoter) gene base on its transcript direction.
 (根据转录方向来选取目标操纵子的启动子基因，如果转录方向为正向，则取最前面的基因，反之取最后面的基因)
#### Key
OperonId value.(操纵子的Door编号)
#### LastGene
获取本操纵子对象之中的最后一个基因
#### lstLocus
这个Operon里面的所有的结构基因的基因号的集合
#### NumOfGenes
这个操纵子里面的结构基因的数目
#### OperonID
Door数据库之中的操纵子编号
#### SortedIdList
这个列表的@"P:SMRUCC.genomics.Assembly.DOOR.GeneBrief.Synonym"[对象]顺序与@"P:Microsoft.VisualBasic.ComponentModel.Collection.Generic.KeyValuePairObject`2.Value"之中的列表对象的顺序一致
#### TestGuid
在比较差异的时候，Operon编号没有意义，再这里使用操纵子内部的基因的编号的字符串来进行相互比较，基因号进行升序排序
#### Value
Structure genes.
 (这个操纵子对象之中的结构基因，请注意@"P:SMRUCC.genomics.Assembly.DOOR.Operon.Genes"属性和这个属性一样，都是可以返回当前的这个Operon里面的所有的结构基因，
 只不过本属性是没有经过排序的，而@"P:SMRUCC.genomics.Assembly.DOOR.Operon.Genes"属性里面的对象都是按照基因组上下文的顺序进行排序操作了的)
