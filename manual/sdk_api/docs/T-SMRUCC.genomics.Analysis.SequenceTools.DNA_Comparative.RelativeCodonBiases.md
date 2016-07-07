---
title: RelativeCodonBiases
---

# RelativeCodonBiases
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative](N-SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.html)_

Measures of Relative Codon Biases
 
 CAI: codon adaptation index
 
 This specifies a reference set of genes, almost invariably, H, chosen from among “highly expressed genes.”
 
 Defining W(xyz) = f(xyz)/max(xyz[a])*f(xyz,H) as the ratio of the frequency of the codon (xyz) to the 
 maximal codon frequency in H for the same amino acid a ,the CAI of a gene of length L is taken as PI(Wi)^(1/L) (the log average), 
 where i refers to the ith codon of the gene and w is calculated as above. 
 
 H 集合是所选择的高表达量的参考基因



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.RelativeCodonBiases.#ctor(SMRUCC.genomics.SequenceModel.FASTA.FastaToken)
```
ORF的核酸序列之中构建出密码子偏好属性

|Parameter Name|Remarks|
|--------------|-------|
|Sequence|ATG -> TGA这一段之间的ORF的核酸序列|


#### __staticsOfMaxFrequencyCodon
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.RelativeCodonBiases.__staticsOfMaxFrequencyCodon(System.Char)
```
统计某一中氨基酸的编码偏好性

|Parameter Name|Remarks|
|--------------|-------|
|AminoAcid|-|


#### Normalization
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.RelativeCodonBiases.Normalization(Microsoft.VisualBasic.ComponentModel.TripleKeyValuesPair{System.Double})
```
对Profile进行归一化处理

#### W
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.RelativeCodonBiases.W(SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.Codon)
```
计算 W(Codon)
 即计算当前的密码子与编码相同氨基酸的最高频率的密码子的商( 
 Defining W(xyz) = f(xyz)/max(xyz[a])*f(xyz,H) as the ratio of the frequency of the codon (xyz) to the maximal codon frequency in H for the same amino acid a)

|Parameter Name|Remarks|
|--------------|-------|
|Codon|-|



