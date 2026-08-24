#Region "Microsoft.VisualBasic::23527632d9a602b3f105a887588c0db2, annotations\Bifrost\MetaEuk\CodonTable.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 71
    '    Code Lines: 56 (78.87%)
    ' Comment Lines: 8 (11.27%)
    '    - Xml Docs: 37.50%
    ' 
    '   Blank Lines: 7 (9.86%)
    '     File Size: 3.24 KB


    ' Class CodonTable
    ' 
    '     Function: ReverseComplement, Translate, TranslateCodon
    ' 
    ' /********************************************************************************/

#End Region


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

