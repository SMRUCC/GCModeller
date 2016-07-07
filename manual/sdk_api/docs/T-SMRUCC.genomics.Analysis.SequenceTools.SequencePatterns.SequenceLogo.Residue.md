---
title: Residue
---

# Residue
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo](N-SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo.html)_

A drawing site in the sequence logo drawing.(所绘制的序列logo图之中的一个位点)



### Methods

#### CalculatesBits
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo.Residue.CalculatesBits(SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo.Residue,System.Double,System.Boolean)
```
The information content (y-axis) of position i is given by:
 Ri = log2(4) - (Hi + en) //nt
 Ri = log2(20) - (Hi + en) //prot 
 
 4 for DNA/RNA or 20 for protein. Consequently, the maximum sequence conservation 
 per site Is log2 4 = 2 bits for DNA/RNA And log2 20 ≈ 4.32 bits for proteins.

|Parameter Name|Remarks|
|--------------|-------|
|rsd|-|
|En|-|


#### Hi
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo.Residue.Hi
```
Hi is the uncertainty (sometimes called the Shannon entropy) of position i
 
 Hi = - Σ(f(a,i) x log2(f(a,i))
 
 Here, f(a,i) is the relative frequency of base or amino acid a at position i (in this residue)
 
 但是频率是零的时候怎么处理？？？


### Properties

#### Address
Position value of this residue in the motif sequence.(这个残基的位点编号)
#### Alphabets
ATGC, 4 characters for nt, and aa is 20.
#### AsChar
Display this site as a single alphabet, and this property is used for generates the motif string.
#### Bits
The total height of the letters depicts the information content Of the position, In bits.
 (Bits的值是和比对的序列的数量是有关系的)
