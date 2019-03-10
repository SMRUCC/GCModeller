# NucleicAcidStaticsProperty
_namespace: [SMRUCC.genomics.SequenceModel.NucleotideModels](./index.md)_

NucleicAcid sequence property calculator



### Methods

#### __contentCommon
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty.__contentCommon(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel,System.Int32,System.Int32,System.Boolean,System.Char[])
```


|Parameter Name|Remarks|
|--------------|-------|
|SequenceModel|-|
|SlideWindowSize|-|
|Steps|-|
|Circular|-|
|base|必须是大写的字符|


#### GC_Content
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty.GC_Content(System.Collections.Generic.IEnumerable{SMRUCC.genomics.SequenceModel.NucleotideModels.DNA})
```
Calculate the GC content of the target sequence data.

|Parameter Name|Remarks|
|--------------|-------|
|seq|-|


#### GCContent
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty.GCContent(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel,System.Int32,System.Int32,System.Boolean)
```
Calculate the GC content of the target sequence data.

|Parameter Name|Remarks|
|--------------|-------|
|SequenceModel|-|
|SlideWindowSize|-|
|Steps|-|
|Circular|-|


#### GCData
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty.GCData(System.Collections.Generic.IEnumerable{SMRUCC.genomics.SequenceModel.FASTA.FastaToken},System.Int32,System.Int32,SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty.NtProperty)
```
批量计算出GCSkew或者GC%

|Parameter Name|Remarks|
|--------------|-------|
|nts|-|
|winSize|-|
|steps|-|
|method|-|


#### GCSkew
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty.GCSkew(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel,System.Int32,System.Int32,System.Boolean)
```
Calculation the GC skew of a specific nucleotide acid sequence.
 (对核酸链分子计算GC偏移量，请注意，当某一个滑窗区段内的GC是相等的话，则会出现正无穷)

|Parameter Name|Remarks|
|--------------|-------|
|SequenceModel|Target sequence object should be a nucleotide acid sequence.(目标对象必须为核酸链分子)|
|isCircular|-|


_returns: 返回的矩阵是每一个核苷酸碱基上面的GC偏移量_

#### GetCompositionVector
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty.GetCompositionVector(System.Char[])
```
A, T, G, C

|Parameter Name|Remarks|
|--------------|-------|
|Sequence|-|


_returns: A, T, G, C_

#### Tm
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty.Tm(System.String)
```
The melting temperature of P1 is Tm(P1), which is a reference temperature for a primer to perform annealing and known as the Wallace formula


