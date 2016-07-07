---
title: Protein
---

# Protein
_namespace: [SMRUCC.genomics.ProteinModel](N-SMRUCC.genomics.ProteinModel.html)_

A type of data structure for descript the protein domain architecture distribution.(一个用于描述蛋白质结构域分布的数据结构)



### Methods

#### ContainsAnyDomain
```csharp
SMRUCC.genomics.ProteinModel.Protein.ContainsAnyDomain(System.Collections.Generic.IEnumerable{System.String})
```
本蛋白质之中是否包含有目标结构域编号列表中的任何结构域信息，返回所包含的编号列表

|Parameter Name|Remarks|
|--------------|-------|
|DomainAccessions|-|


#### InteractionWith
```csharp
SMRUCC.genomics.ProteinModel.Protein.InteractionWith(SMRUCC.genomics.Assembly.DOMINE.Database)
```
获取与本蛋白质相互作用的结构域列表

|Parameter Name|Remarks|
|--------------|-------|
|DOMINE|-|


#### SimilarTo
```csharp
SMRUCC.genomics.ProteinModel.Protein.SimilarTo(SMRUCC.genomics.ProteinModel.Protein,SMRUCC.genomics.ProteinModel.Protein,System.Double)
```
简单的根据结构域分布来判断两个蛋白质是否相似，两个蛋白质的结构域分布必须以相似的方式进行排布

|Parameter Name|Remarks|
|--------------|-------|
|Protein1|-|
|Protein2|-|



### Properties

#### Domains
结构域分布
#### Identifier
该目标蛋白质的唯一标识符
