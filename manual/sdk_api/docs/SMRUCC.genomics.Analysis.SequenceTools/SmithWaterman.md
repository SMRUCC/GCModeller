# SmithWaterman
_namespace: [SMRUCC.genomics.Analysis.SequenceTools](./index.md)_

Smith-Waterman local alignment algorithm.

 Design Note: this class implements AminoAcids interface: a simple fix customized to amino acids, since that is all we deal with in this class
 Supporting both DNA and Aminoacids, will require a more general design.



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SmithWaterman.#ctor(System.String,System.String,SMRUCC.genomics.Analysis.SequenceTools.Blosum)
```


|Parameter Name|Remarks|
|--------------|-------|
|query|-|
|subject|-|
|blosum|
 If the matrix parameter is null, then the default build in blosum62 matrix will be used.
 |


#### Align
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SmithWaterman.Align(SMRUCC.genomics.SequenceModel.FASTA.FastaToken,SMRUCC.genomics.SequenceModel.FASTA.FastaToken,SMRUCC.genomics.Analysis.SequenceTools.Blosum)
```
Default using ``Blosum62`` matrix.

|Parameter Name|Remarks|
|--------------|-------|
|query|-|
|subject|-|
|blosum|-|


#### GetOutput
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SmithWaterman.GetOutput(System.Double,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|cutoff|0%-100%|



