---
title: ISequenceModel
---

# ISequenceModel
_namespace: [SMRUCC.genomics.SequenceModel](N-SMRUCC.genomics.SequenceModel.html)_

The biological sequence molecular model.(蛋白质序列，核酸序列都可以使用本对象来表示)



### Methods

#### Get_CompositionVector
```csharp
SMRUCC.genomics.SequenceModel.ISequenceModel.Get_CompositionVector(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel,System.Char[])
```
Get the composition vector for a sequence model using a specific composition description.

|Parameter Name|Remarks|
|--------------|-------|
|SequenceModel|-|
|compositions|This always should be the constant string of @"F:SMRUCC.genomics.SequenceModel.ISequenceModel.AA_CHARS_ALL"[amino acid]
 or @"F:SMRUCC.genomics.SequenceModel.ISequenceModel.NA_CHARS_ALL"[nucleotide].|


#### IsProteinSource
```csharp
SMRUCC.genomics.SequenceModel.ISequenceModel.IsProteinSource(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel)
```
目标序列数据是否为一条蛋白质序列

|Parameter Name|Remarks|
|--------------|-------|
|SequenceData|-|



### Properties

#### AA_CHARS_ALL
Enumerate all of the amino acid.(字符串常量枚举所有的氨基酸分子)
#### IsProtSource
This sequence is a protein type sequence?(判断这条序列是否为蛋白质序列)
#### Length
The @"P:SMRUCC.genomics.SequenceModel.ISequenceModel.SequenceData" string length.
#### NA_CHARS_ALL
Enumerate all of the nucleotides.(字符串常量枚举所有的核苷酸分子类型)
#### SequenceData
Sequence data in a string type.(字符串类型的序列数据)
