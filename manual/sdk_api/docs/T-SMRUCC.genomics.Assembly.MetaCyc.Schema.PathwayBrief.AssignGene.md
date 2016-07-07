---
title: AssignGene
---

# AssignGene
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.Schema.PathwayBrief](N-SMRUCC.genomics.Assembly.MetaCyc.Schema.PathwayBrief.html)_

将基因与相应的反应过程映射起来



### Methods

#### AssignGenes
```csharp
SMRUCC.genomics.Assembly.MetaCyc.Schema.PathwayBrief.AssignGene.AssignGenes(SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Enzrxn)
```
获取某一个酶促反应中所涉及到的所有基因

|Parameter Name|Remarks|
|--------------|-------|
|Enzrxn|-|


#### ConvertId
```csharp
SMRUCC.genomics.Assembly.MetaCyc.Schema.PathwayBrief.AssignGene.ConvertId(System.Collections.Generic.Dictionary{System.String,System.String[]})
```


|Parameter Name|Remarks|
|--------------|-------|
|MetaCycData|从@"M:SMRUCC.genomics.Assembly.MetaCyc.Schema.PathwayBrief.AssignGene.Performance"函数所得到的结果参数列表|


#### GetGenes
```csharp
SMRUCC.genomics.Assembly.MetaCyc.Schema.PathwayBrief.AssignGene.GetGenes(SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Protein,SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Proteins)
```
递归的查找某一个蛋白质复合物的基因

|Parameter Name|Remarks|
|--------------|-------|
|Protein|-|
|ProteinList|-|


#### Performance
```csharp
SMRUCC.genomics.Assembly.MetaCyc.Schema.PathwayBrief.AssignGene.Performance
```
String() => {Reaction, Associated-Genes}
_returns: {Reaction, Associated-Genes}_


