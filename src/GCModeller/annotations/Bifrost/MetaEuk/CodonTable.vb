
' ========================================================================
' MODULE 2: CODON TABLE & SIX-FRAME TRANSLATION
' ========================================================================

Imports System.Text

Public Class CodonTable
    ' Standard genetic code (NCBI translation table 1)
    Private Shared ReadOnly CodonMap As New Dictionary(Of String, Char) From {
        {"TTT", "F"c}, {"TTC", "F"c}, {"TTA", "L"c}, {"TTG", "L"c},
        {"CTT", "L"c}, {"CTC", "L"c}, {"CTA", "L"c}, {"CTG", "L"c},
        {"ATT", "I"c}, {"ATC", "I"c}, {"ATA", "I"c}, {"ATG", "M"c},
        {"GTT", "V"c}, {"GTC", "V"c}, {"GTA", "V"c}, {"GTG", "V"c},
        {"TCT", "S"c}, {"TCC", "S"c}, {"TCA", "S"c}, {"TCG", "S"c},
        {"CCT", "P"c}, {"CCC", "P"c}, {"CCA", "P"c}, {"CCG", "P"c},
        {"ACT", "T"c}, {"ACC", "T"c}, {"ACA", "T"c}, {"ACG", "T"c},
        {"GCT", "A"c}, {"GCC", "A"c}, {"GCA", "A"c}, {"GCG", "A"c},
        {"TAT", "Y"c}, {"TAC", "Y"c}, {"TAA", "*"c}, {"TAG", "*"c},
        {"CAT", "H"c}, {"CAC", "H"c}, {"CAA", "Q"c}, {"CAG", "Q"c},
        {"AAT", "N"c}, {"AAC", "N"c}, {"AAA", "K"c}, {"AAG", "K"c},
        {"GAT", "D"c}, {"GAC", "D"c}, {"GAA", "E"c}, {"GAG", "E"c},
        {"TGT", "C"c}, {"TGC", "C"c}, {"TGA", "*"c}, {"TGG", "W"c},
        {"CGT", "R"c}, {"CGC", "R"c}, {"CGA", "R"c}, {"CGG", "R"c},
        {"AGT", "S"c}, {"AGC", "S"c}, {"AGA", "R"c}, {"AGG", "R"c},
        {"GGT", "G"c}, {"GGC", "G"c}, {"GGA", "G"c}, {"GGG", "G"c}
    }

    ''' <summary>Translate a single codon to amino acid; 'X' for unknown</summary>
    Public Shared Function TranslateCodon(codon As String) As Char
        If codon.Length <> 3 Then Return "X"c
        Dim upper = codon.ToUpper()
        If CodonMap.ContainsKey(upper) Then Return CodonMap(upper)
        ' Handle ambiguous bases: if any N, return X
        Return "X"c
    End Function

    ''' <summary>Translate a DNA sequence in one reading frame</summary>
    Public Shared Function Translate(dna As String, frameOffset As Integer) As String
        Dim sb As New StringBuilder()
        Dim i As Integer = frameOffset
        While i + 2 < dna.Length
            Dim codon = dna.Substring(i, 3)
            Dim aa = TranslateCodon(codon)
            sb.Append(aa)
            i += 3
        End While
        Return sb.ToString()
    End Function

    ''' <summary>Get reverse complement of a DNA sequence</summary>
    Public Shared Function ReverseComplement(dna As String) As String
        Dim complement As New Dictionary(Of Char, Char) From {
            {"A"c, "T"c}, {"T"c, "A"c}, {"G"c, "C"c}, {"C"c, "G"c},
            {"N"c, "N"c}, {"R"c, "Y"c}, {"Y"c, "R"c}, {"M"c, "K"c},
            {"K"c, "M"c}, {"S"c, "S"c}, {"W"c, "W"c}, {"H"c, "D"c},
            {"D"c, "H"c}, {"B"c, "V"c}, {"V"c, "B"c}
        }
        Dim sb As New StringBuilder(dna.Length)
        For i As Integer = dna.Length - 1 To 0 Step -1
            Dim ch = Char.ToUpper(dna(i))
            If complement.ContainsKey(ch) Then
                sb.Append(complement(ch))
            Else
                sb.Append("N"c)
            End If
        Next
        Return sb.ToString()
    End Function

End Class
