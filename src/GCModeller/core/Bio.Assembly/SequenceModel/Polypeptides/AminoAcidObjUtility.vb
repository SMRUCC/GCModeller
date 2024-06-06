#Region "Microsoft.VisualBasic::2a7e16457295c1560872881b725b0ed6, core\Bio.Assembly\SequenceModel\Polypeptides\AminoAcidObjUtility.vb"

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

    '   Total Lines: 522
    '    Code Lines: 497 (95.21%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 25 (4.79%)
    '     File Size: 11.86 KB


    '     Module AminoAcidObjUtility
    ' 
    '         Function: (+3 Overloads) IsAAEqual
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace SequenceModel.Polypeptides
    Public Module AminoAcidObjUtility


        Public Function IsAAEqual(letter1 As Char, letter2 As Char) As Boolean
            If letter1 = "J"c AndAlso (letter2 = "J"c OrElse letter2 = "L"c OrElse letter2 = "I"c) Then Return True
            If letter2 = "J"c AndAlso (letter1 = "J"c OrElse letter1 = "L"c OrElse letter1 = "I"c) Then Return True
            If letter1 = letter2 Then Return True
            Return False
        End Function

        Public Function IsAAEqual(letter1 As String, letter2 As String) As Boolean
            If Equals(letter1, "J") AndAlso (Equals(letter2, "J") OrElse Equals(letter2, "L") OrElse Equals(letter2, "I")) Then Return True
            If Equals(letter2, "J") AndAlso (Equals(letter1, "J") OrElse Equals(letter1, "L") OrElse Equals(letter1, "I")) Then Return True
            If Equals(letter1, letter2) Then Return True
            Return False
        End Function

        Public Function IsAAEqual(letter1 As Char, letter2 As String) As Boolean
            If letter1 = "J"c AndAlso (Equals(letter2, "J") OrElse Equals(letter2, "L") OrElse Equals(letter2, "I")) Then Return True
            If Equals(letter2, "J") AndAlso (letter1 = "J"c OrElse letter1 = "L"c OrElse letter1 = "I"c) Then Return True
            If Equals(letter1.ToString(), letter2) Then Return True
            Return False
        End Function

        Public AminoAcidLetters As List(Of Char) = New List(Of Char)() From {
            "A"c,
            "R"c,
            "N"c,
            "D"c,
            "C"c,
            "E"c,
            "Q"c,
            "G"c,
            "H"c,
            "I"c,
            "L"c,
            "K"c,
            "M"c,
            "F"c,
            "P"c,
            "S"c,
            "T"c,
            "W"c,
            "Y"c,
            "V"c,
            "U"c,
            "O"c,
            "J"c
        }

        Public OneLetter2Mass As Dictionary(Of String, Double) = New Dictionary(Of String, Double) From {
    {"A", 71.037113805},
    {"R", 156.10111105},
    {"N", 114.04292747},
    {"D", 115.026943065},
    {"C", 103.009184505},
    {"E", 129.042593135},
    {"Q", 128.05857754},
    {"G", 57.021463735},
    {"H", 137.058911875},
    {"I", 113.084064015},
    {"L", 113.084064015},
    {"J", 113.084064015},
    {"K", 128.09496305},
    {"M", 131.040484645},
    {"F", 147.068413945},
    {"P", 97.052763875},
    {"S", 87.032028435},
    {"T", 101.047678505},
    {"W", 186.07931298},
    {"Y", 163.063328575},
    {"V", 99.068413945},
    {"U", 150.953633405},
    {"O", 237.147726925}
}

        Public OneChar2Mass As Dictionary(Of Char, Double) = New Dictionary(Of Char, Double) From {
    {"A"c, 71.037113805},
    {"R"c, 156.10111105},
    {"N"c, 114.04292747},
    {"D"c, 115.026943065},
    {"C"c, 103.009184505},
    {"E"c, 129.042593135},
    {"Q"c, 128.05857754},
    {"G"c, 57.021463735},
    {"H"c, 137.058911875},
    {"I"c, 113.084064015},
    {"L"c, 113.084064015},
    {"J"c, 113.084064015},
    {"K"c, 128.09496305},
    {"M"c, 131.040484645},
    {"F"c, 147.068413945},
    {"P"c, 97.052763875},
    {"S"c, 87.032028435},
    {"T"c, 101.047678505},
    {"W"c, 186.07931298},
    {"Y"c, 163.063328575},
    {"V"c, 99.068413945},
    {"U"c, 150.953633405},
    {"O"c, 237.147726925}
}

        Public OneLetter2ThreeLetter As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
    {"A", "Ala"},
    {"R", "Arg"},
    {"N", "Asn"},
    {"D", "Asp"},
    {"C", "Cys"},
    {"E", "Glu"},
    {"Q", "Gln"},
    {"G", "Gly"},
    {"H", "His"},
    {"I", "Ile"},
    {"L", "Leu"},
    {"J", "Xle"},
    {"K", "Lys"},
    {"M", "Met"},
    {"F", "Phe"},
    {"P", "Pro"},
    {"S", "Ser"},
    {"T", "Thr"},
    {"W", "Trp"},
    {"Y", "Tyr"},
    {"V", "Val"},
    {"O", "Pyl"},
    {"U", "Sec"}
}

        Public OneChar2ThreeLetter As Dictionary(Of Char, String) = New Dictionary(Of Char, String) From {
     {"A"c, "Ala"},
     {"R"c, "Arg"},
     {"N"c, "Asn"},
     {"D"c, "Asp"},
     {"C"c, "Cys"},
     {"E"c, "Glu"},
     {"Q"c, "Gln"},
     {"G"c, "Gly"},
     {"H"c, "His"},
     {"I"c, "Ile"},
     {"L"c, "Leu"},
     {"J"c, "Xle"},
     {"K"c, "Lys"},
     {"M"c, "Met"},
     {"F"c, "Phe"},
     {"P"c, "Pro"},
     {"S"c, "Ser"},
     {"T"c, "Thr"},
     {"W"c, "Trp"},
     {"Y"c, "Tyr"},
     {"V"c, "Val"},
     {"O"c, "Pyl"},
     {"U"c, "Sec"}
 }

        Public ThreeLetter2OneLetter As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
    {"Ala", "A"},
    {"Arg", "R"},
    {"Asn", "N"},
    {"Asp", "D"},
    {"Cys", "C"},
    {"Glu", "E"},
    {"Gln", "Q"},
    {"Gly", "G"},
    {"His", "H"},
    {"Ile", "I"},
    {"Leu", "L"},
    {"Xle", "J"},
    {"Lys", "K"},
    {"Met", "M"},
    {"Phe", "F"},
    {"Pro", "P"},
    {"Ser", "S"},
    {"Thr", "T"},
    {"Trp", "W"},
    {"Tyr", "Y"},
    {"Val", "V"},
    {"Pyl", "O"},
    {"Sec", "U"}
}

        Public ThreeLetter2OneChar As Dictionary(Of String, Char) = New Dictionary(Of String, Char) From {
    {"Ala", "A"c},
    {"Arg", "R"c},
    {"Asn", "N"c},
    {"Asp", "D"c},
    {"Cys", "C"c},
    {"Glu", "E"c},
    {"Gln", "Q"c},
    {"Gly", "G"c},
    {"His", "H"c},
    {"Ile", "I"c},
    {"Leu", "L"c},
    {"Xle", "J"c},
    {"Lys", "K"c},
    {"Met", "M"c},
    {"Phe", "F"c},
    {"Pro", "P"c},
    {"Ser", "S"c},
    {"Thr", "T"c},
    {"Trp", "W"c},
    {"Tyr", "Y"c},
    {"Val", "V"c},
    {"Pyl", "O"c},
    {"Sec", "U"c}
}


        Public OneLetter2FormulaString As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
    {"A", "C3H7O2N"},
    {"R", "C6H14O2N4"},
    {"N", "C4H8O3N2"},
    {"D", "C4H7O4N"},
    {"C", "C3H7O2NS"},
    {"E", "C5H9O4N"},
    {"Q", "C5H10O3N2"},
    {"G", "C2H5O2N"},
    {"H", "C6H9O2N3"},
    {"I", "C6H13O2N"},
    {"L", "C6H13O2N"},
    {"J", "C6H13O2N"},
    {"K", "C6H14O2N2"},
    {"M", "C5H11O2NS"},
    {"F", "C9H11O2N"},
    {"P", "C5H9O2N"},
    {"S", "C3H7O3N"},
    {"T", "C4H9O3N"},
    {"W", "C11H12O2N2"},
    {"Y", "C9H11O3N"},
    {"V", "C5H11O2N"},
    {"O", "C12H21N3O3"},
    {"U", "C3H7NO2Se"}
}

        Public OneChar2FormulaString As Dictionary(Of Char, String) = New Dictionary(Of Char, String) From {
    {"A"c, "C3H7O2N"},
    {"R"c, "C6H14O2N4"},
    {"N"c, "C4H8O3N2"},
    {"D"c, "C4H7O4N"},
    {"C"c, "C3H7O2NS"},
    {"E"c, "C5H9O4N"},
    {"Q"c, "C5H10O3N2"},
    {"G"c, "C2H5O2N"},
    {"H"c, "C6H9O2N3"},
    {"I"c, "C6H13O2N"},
    {"L"c, "C6H13O2N"},
    {"J"c, "C6H13O2N"},
    {"K"c, "C6H14O2N2"},
    {"M"c, "C5H11O2NS"},
    {"F"c, "C9H11O2N"},
    {"P"c, "C5H9O2N"},
    {"S"c, "C3H7O3N"},
    {"T"c, "C4H9O3N"},
    {"W"c, "C11H12O2N2"},
    {"Y"c, "C9H11O3N"},
    {"V"c, "C5H11O2N"},
    {"O"c, "C12H21N3O3"},
    {"U"c, "C3H7NO2Se"}
}


        Public OneLetter2CarbonNuber As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer) From {
    {"A", 3},
    {"R", 6},
    {"N", 4},
    {"D", 4},
    {"C", 3},
    {"E", 5},
    {"Q", 5},
    {"G", 2},
    {"H", 6},
    {"I", 6},
    {"L", 6},
    {"J", 6},
    {"K", 6},
    {"M", 5},
    {"F", 9},
    {"P", 5},
    {"S", 3},
    {"T", 4},
    {"W", 11},
    {"Y", 9},
    {"V", 5},
    {"O", 12},
    {"U", 3}
}

        Public OneChar2CarbonNuber As Dictionary(Of Char, Integer) = New Dictionary(Of Char, Integer) From {
    {"A"c, 3},
    {"R"c, 6},
    {"N"c, 4},
    {"D"c, 4},
    {"C"c, 3},
    {"E"c, 5},
    {"Q"c, 5},
    {"G"c, 2},
    {"H"c, 6},
    {"I"c, 6},
    {"L"c, 6},
    {"J"c, 6},
    {"K"c, 6},
    {"M"c, 5},
    {"F"c, 9},
    {"P"c, 5},
    {"S"c, 3},
    {"T"c, 4},
    {"W"c, 11},
    {"Y"c, 9},
    {"V"c, 5},
    {"O"c, 12},
    {"U"c, 3}
}

        Public OneLetter2NitrogenNuber As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer) From {
    {"A", 1},
    {"R", 4},
    {"N", 2},
    {"D", 1},
    {"C", 1},
    {"E", 1},
    {"Q", 2},
    {"G", 1},
    {"H", 3},
    {"I", 1},
    {"L", 1},
    {"J", 1},
    {"K", 2},
    {"M", 1},
    {"F", 1},
    {"P", 1},
    {"S", 1},
    {"T", 1},
    {"W", 2},
    {"Y", 1},
    {"V", 1},
    {"O", 3},
    {"U", 1}
}

        Public OneChar2NitrogenNuber As Dictionary(Of Char, Integer) = New Dictionary(Of Char, Integer) From {
    {"A"c, 1},
    {"R"c, 4},
    {"N"c, 2},
    {"D"c, 1},
    {"C"c, 1},
    {"E"c, 1},
    {"Q"c, 2},
    {"G"c, 1},
    {"H"c, 3},
    {"I"c, 1},
    {"L"c, 1},
    {"J"c, 1},
    {"K"c, 2},
    {"M"c, 1},
    {"F"c, 1},
    {"P"c, 1},
    {"S"c, 1},
    {"T"c, 1},
    {"W"c, 2},
    {"Y"c, 1},
    {"V"c, 1},
    {"O"c, 3},
    {"U"c, 1}
}

        Public OneLetter2HydrogenNuber As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer) From {
    {"A", 7},
    {"R", 14},
    {"N", 8},
    {"D", 7},
    {"C", 7},
    {"E", 9},
    {"Q", 10},
    {"G", 5},
    {"H", 9},
    {"I", 13},
    {"L", 13},
    {"J", 13},
    {"K", 14},
    {"M", 11},
    {"F", 11},
    {"P", 9},
    {"S", 7},
    {"T", 9},
    {"W", 12},
    {"Y", 11},
    {"V", 11},
    {"O", 21},
    {"U", 7}
}

        Public OneChar2HydrogenNuber As Dictionary(Of Char, Integer) = New Dictionary(Of Char, Integer) From {
    {"A"c, 7},
    {"R"c, 14},
    {"N"c, 8},
    {"D"c, 7},
    {"C"c, 7},
    {"E"c, 9},
    {"Q"c, 10},
    {"G"c, 5},
    {"H"c, 9},
    {"I"c, 13},
    {"L"c, 13},
    {"J"c, 13},
    {"K"c, 14},
    {"M"c, 11},
    {"F"c, 11},
    {"P"c, 9},
    {"S"c, 7},
    {"T"c, 9},
    {"W"c, 12},
    {"Y"c, 11},
    {"V"c, 11},
    {"O"c, 21},
    {"U"c, 7}
}

        Public OneLetter2OxygenNuber As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer) From {
    {"A", 2},
    {"R", 2},
    {"N", 3},
    {"D", 4},
    {"C", 2},
    {"E", 4},
    {"Q", 3},
    {"G", 2},
    {"H", 2},
    {"I", 2},
    {"L", 2},
    {"J", 2},
    {"K", 2},
    {"M", 2},
    {"F", 2},
    {"P", 2},
    {"S", 3},
    {"T", 3},
    {"W", 2},
    {"Y", 3},
    {"V", 2},
    {"O", 3},
    {"U", 2}
}

        Public OneChar2OxygenNuber As Dictionary(Of Char, Integer) = New Dictionary(Of Char, Integer) From {
    {"A"c, 2},
    {"R"c, 2},
    {"N"c, 3},
    {"D"c, 4},
    {"C"c, 2},
    {"E"c, 4},
    {"Q"c, 3},
    {"G"c, 2},
    {"H"c, 2},
    {"I"c, 2},
    {"L"c, 2},
    {"J"c, 2},
    {"K"c, 2},
    {"M"c, 2},
    {"F"c, 2},
    {"P"c, 2},
    {"S"c, 3},
    {"T"c, 3},
    {"W"c, 2},
    {"Y"c, 3},
    {"V"c, 2},
    {"O"c, 3},
    {"U"c, 2}
}

        Public OneLetter2SulfurNuber As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer) From {
    {"A", 0},
    {"R", 0},
    {"N", 0},
    {"D", 0},
    {"C", 1},
    {"E", 0},
    {"Q", 0},
    {"G", 0},
    {"H", 0},
    {"I", 0},
    {"L", 0},
    {"J", 0},
    {"K", 0},
    {"M", 1},
    {"F", 0},
    {"P", 0},
    {"S", 0},
    {"T", 0},
    {"W", 0},
    {"Y", 0},
    {"V", 0},
    {"O", 0},
    {"U", 0}
}

        Public OneChar2SulfurNuber As Dictionary(Of Char, Integer) = New Dictionary(Of Char, Integer) From {
    {"A"c, 0},
    {"R"c, 0},
    {"N"c, 0},
    {"D"c, 0},
    {"C"c, 1},
    {"E"c, 0},
    {"Q"c, 0},
    {"G"c, 0},
    {"H"c, 0},
    {"I"c, 0},
    {"L"c, 0},
    {"J"c, 0},
    {"K"c, 0},
    {"M"c, 1},
    {"F"c, 0},
    {"P"c, 0},
    {"S"c, 0},
    {"T"c, 0},
    {"W"c, 0},
    {"Y"c, 0},
    {"V"c, 0},
    {"O"c, 0},
    {"U"c, 0}
}
    End Module
End Namespace
