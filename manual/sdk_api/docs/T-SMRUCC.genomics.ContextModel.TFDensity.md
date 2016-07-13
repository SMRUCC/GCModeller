---
title: TFDensity
---

# TFDensity
_namespace: [SMRUCC.genomics.ContextModel](N-SMRUCC.genomics.ContextModel.html)_

Calculates the relative density of the TF on each gene on the genome.



### Methods

#### __getCisGenes``1
```csharp
SMRUCC.genomics.ContextModel.TFDensity.__getCisGenes``1(``0,``0[],System.Int32)
```
查找当前的基因的上游符合距离范围内的TF目标

|Parameter Name|Remarks|
|--------------|-------|
|g|-|
|TFs|-|
|ranges|-|


#### Density``1
```csharp
SMRUCC.genomics.ContextModel.TFDensity.Density``1(SMRUCC.genomics.ContextModel.IGenomicsContextProvider{``0},System.Collections.Generic.IEnumerable{System.String},System.Int32,System.Boolean)
```
Although this function name means calculate the relative density of the TF on the genome for each gene, 
 but you can also calculate any type of gene its density on the genome.
 (虽然名称是调控因子的密度，但是也可以用作为其他类型的基因的密度的计算，
 这个函数是非顺式的，即只要在ATG前面的范围内或者TGA后面的范围内出现都算存在)

|Parameter Name|Remarks|
|--------------|-------|
|genome|-|
|TF|-|


#### DensityCis``1
```csharp
SMRUCC.genomics.ContextModel.TFDensity.DensityCis``1(SMRUCC.genomics.ContextModel.IGenomicsContextProvider{``0},System.Collections.Generic.IEnumerable{System.String},System.Int32)
```
顺式调控，只有TF出现在上游，并且二者链方向相同才算存在

|Parameter Name|Remarks|
|--------------|-------|
|genome|Bacteria genomic context provider.|
|TF|The TF locus_tag list.|
|ranges|
 This value is set to 20000bp is more perfect works on the bacteria genome, probably...
 |



