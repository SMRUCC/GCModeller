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

Input like:

```
>XAC0334:-186
ATGATCCGAATTCAT

>XAC0333:-19
ATGAATTCGGATCAT

>XCC0315:-19
ATGAATTCGGATCAT

>XCC0316:-105
ATGATCCGAATTCAT

>Smlt2583:-19
ATGAGCAGGATTCAT

>Smlt2584:-71
ATGAATCCTGCTCAT
```

Should output something like:

```
XAC0334:-186    ATG-ATCCGAATTCAT
XAC0333:-19     ATGAATTCGGA-TCAT
XCC0315:-19     ATGAATTCGGA-TCAT
XCC0316:-105    ATGAATTCGGA-TCAT
Smlt2583:-19    ATG-AGCAGGATTCAT
Smlt2584:-71    ATGAATCC-TGCTCAT
                *** *       ****
```