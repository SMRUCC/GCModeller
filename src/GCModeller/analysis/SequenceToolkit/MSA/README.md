# Multiple-sequence-alignment

This program calculates the multiple sequence alignment of ``k > 1`` DNA sequences.

The program use the ``Matrix.txt`` file for the substitution matrix. The matrix can be changed, and it used with default values as: 

+ 0-Match
+ 1-Missmatch
+ 2-Indel

Algorithm used for this purpose is Center Star Algotrithm

## Usage

```vbnet
Dim matrix = New ScoreMatrix("./Matrix.txt").matrix 
Dim MSA = FastaFile _
	.LoadNucleotideData("Xanthomonadales_MetR___Xanthomonadales.fasta") _
	.MultipleAlignment(matrix)

Call Console.WriteLine(MSA)
```