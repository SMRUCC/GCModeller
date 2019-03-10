# GCWindows
_namespace: [SMRUCC.genomics.Analysis.bacgFingerprints](./index.md)_

Build the sampling windows by using GC% or GC skew.



### Methods

#### GetWindows
```csharp
SMRUCC.genomics.Analysis.bacgFingerprints.GCWindows.GetWindows(SMRUCC.genomics.SequenceModel.FASTA.FastaToken,System.Int32,System.Int32,SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty.NtProperty)
```


|Parameter Name|Remarks|
|--------------|-------|
|nt|-|
|slideWin|-|
|steps|-|
|[property]|@``M:SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty.GCSkew(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel,System.Int32,System.Int32,System.Boolean)`` or @``M:SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty.GCContent(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel)`` or your custom engine.|



