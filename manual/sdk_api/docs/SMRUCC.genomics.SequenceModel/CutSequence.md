# CutSequence
_namespace: [SMRUCC.genomics.SequenceModel](./index.md)_

Cut sequence for DNA/protein



### Methods

#### CutSequenceCircular
```csharp
SMRUCC.genomics.SequenceModel.CutSequence.CutSequenceCircular(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel,SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation,SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation)
```
**`site`** at the end of nt sequence join with **`join`** location to consist a completed gene.

|Parameter Name|Remarks|
|--------------|-------|
|seq|-|
|site|环状的分子只能够是DNA分子，所以这里是核酸序列的位置|

> Not sure, probably success.

#### CutSequenceLinear
```csharp
SMRUCC.genomics.SequenceModel.CutSequence.CutSequenceLinear(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel,SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation)
```


|Parameter Name|Remarks|
|--------------|-------|
|seq|-|
|loci|-|

> Tested by XC_1184/XC_0012, no problem.


