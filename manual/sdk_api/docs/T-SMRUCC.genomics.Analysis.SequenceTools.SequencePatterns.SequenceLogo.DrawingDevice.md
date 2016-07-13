---
title: DrawingDevice
---

# DrawingDevice
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo](N-SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo.html)_

In bioinformatics, a sequence logo is a graphical representation of the sequence conservation 
 of nucleotides (in a strand Of DNA/RNA) Or amino acids (In protein sequences).
 A sequence logo Is created from a collection of aligned sequences And depicts the consensus 
 sequence And diversity of the sequences. Sequence logos are frequently used to depict sequence 
 characteristics such as protein-binding sites in DNA Or functional units in proteins.



### Methods

#### DrawFrequency
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo.DrawingDevice.DrawFrequency(SMRUCC.genomics.SequenceModel.FASTA.FastaFile,System.String,SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.MotifPWM@)
```
Drawing the sequence logo just simply modelling this motif site from the clustal multiple sequence alignment.
 (绘制各个残基的出现频率)

|Parameter Name|Remarks|
|--------------|-------|
|Fasta|The alignment export data from the clustal software.|
|title|The sequence logo display title.|


#### E
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo.DrawingDevice.E(System.Int32,System.Int32)
```
The approximation for the small-sample correction, en, Is given by
 en = 1/ln2 x (s-1)/2n

|Parameter Name|Remarks|
|--------------|-------|
|s|s Is 4 For nucleotides, 20 For amino acids|
|n|n Is the number Of sequences In the alignment|


#### InvokeDrawing
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo.DrawingDevice.InvokeDrawing(SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo.DrawingModel,System.Boolean,System.Int32,System.Boolean)
```
Drawing the sequence logo for the sequence motif model.(绘制SequenceLogo图)

|Parameter Name|Remarks|
|--------------|-------|
|model|The model can be achieve from clustal alignment or meme software.|
|frequencyOrder|Reorder the alphabets in each residue site in the order of frequency values. default is yes!|
|reverse|Reverse the residue sequence order in the drawing model?|



### Properties

#### WordSize
The width of the character in the sequence logo.(字符的宽度)
