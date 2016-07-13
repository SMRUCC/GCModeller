---
title: AssignGene
---

# AssignGene
_namespace: [SMRUCC.genomics.Visualize.Cytoscape](N-SMRUCC.genomics.Visualize.Cytoscape.html)_

将基因与相应的反应过程映射起来



### Methods

#### AssignGenes
```csharp
SMRUCC.genomics.Visualize.Cytoscape.AssignGene.AssignGenes(SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Enzrxn)
```
获取某一个酶促反应中所涉及到的所有基因

|Parameter Name|Remarks|
|--------------|-------|
|Enzrxn|-|


#### GetGenes
```csharp
SMRUCC.genomics.Visualize.Cytoscape.AssignGene.GetGenes(SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Protein,SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Proteins)
```
递归的查找某一个蛋白质复合物的基因

|Parameter Name|Remarks|
|--------------|-------|
|Protein|-|
|ProteinList|-|


#### Performance
```csharp
SMRUCC.genomics.Visualize.Cytoscape.AssignGene.Performance
```
String() => {Reaction, Associated-Genes}
_returns: {Reaction, Associated-Genes}_


