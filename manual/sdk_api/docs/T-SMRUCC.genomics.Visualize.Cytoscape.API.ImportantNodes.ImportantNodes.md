---
title: ImportantNodes
---

# ImportantNodes
_namespace: [SMRUCC.genomics.Visualize.Cytoscape.API.ImportantNodes](N-SMRUCC.genomics.Visualize.Cytoscape.API.ImportantNodes.html)_





### Methods

#### EquivalenceClass
```csharp
SMRUCC.genomics.Visualize.Cytoscape.API.ImportantNodes.ImportantNodes.EquivalenceClass(SMRUCC.genomics.Visualize.Cytoscape.Tables.Node[],System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|S|-|
|Fast|
 if fast parameter is set to true, then a parallel edition of the algorithm 
 will implemented for accelerates the network calculation, and this is much 
 helpful for a large scale network.
 |


#### SignificantRegulator
```csharp
SMRUCC.genomics.Visualize.Cytoscape.API.ImportantNodes.ImportantNodes.SignificantRegulator(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{System.Int32,SMRUCC.genomics.Visualize.Cytoscape.Tables.Node[]}},System.Collections.Generic.IEnumerable{SMRUCC.genomics.InteractionModel.Regulon.IRegulatorRegulation},System.Double)
```
这个仅仅是理论上面的计算结果，仅供参考

|Parameter Name|Remarks|
|--------------|-------|
|ImportantNodes|-|
|Regulations|-|
|rankCutoff|-|



