#Region "Microsoft.VisualBasic::099cfa9941a77e2823de342b0c3915c8, core\Bio.Assembly\SequenceModel\Polypeptides\Polypeptides.vb"

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

'   Total Lines: 197
'    Code Lines: 149 (75.63%)
' Comment Lines: 36 (18.27%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 12 (6.09%)
'     File Size: 7.62 KB


'     Module Polypeptide
' 
'         Properties: Abbreviate, MEGASchema, ToChar, ToEnums
' 
'         Function: ConstructVector, GetCompositionVector, GetCount, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq

Namespace SequenceModel.Polypeptides

    ''' <summary>
    ''' Protein polypeptide sequence.
    ''' </summary>
    ''' <remarks>(蛋白质多肽链的一些相关操作)</remarks>
    Public Module Polypeptide

        ''' <summary>
        ''' 将蛋白质序列字符串转换为氨基酸残基的向量
        ''' </summary>
        ''' <param name="prot$"></param>
        ''' <returns></returns>
        Public Iterator Function ConstructVector(prot As String) As IEnumerable(Of AminoAcid)
            For Each ch As Char In prot.ToUpper
                If ToEnums.ContainsKey(ch) Then
                    Yield ToEnums(ch)
                Else
                    Yield AminoAcid.NULL
                End If
            Next
        End Function

        ''' <summary>
        ''' Convert <see cref="AminoAcid"/> enum values to character string
        ''' </summary>
        ''' <param name="aa"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ToString(aa As IEnumerable(Of AminoAcid)) As String
            Return New String(aa.Select(Function(a) ToChar(a)).ToArray)
        End Function

        ''' <summary>
        ''' Generate all polypeptide sequence with given length
        ''' </summary>
        ''' <param name="len"></param>
        ''' <returns></returns>
        Public Iterator Function Generate(len As Integer) As IEnumerable(Of String)
            If len <= 0 Then
                Return
            End If

            Dim peptides As New List(Of List(Of Char))

            ' 初始状态：所有单个氨基酸
            For Each aa As Char In AminoAcidObjUtility.OneLetterFormula.Keys
                Call peptides.Add(New List(Of Char) From {aa})
            Next

            ' 迭代构建更长的肽链
            For currentLength As Integer = 2 To len
                Dim newPeptides As New List(Of List(Of Char))

                ' 为每个现有肽链添加所有可能的氨基酸
                For Each peptide As List(Of Char) In peptides
                    For Each aa As Char In AminoAcidObjUtility.OneLetterFormula.Keys
                        Call newPeptides.Add(New List(Of Char)(peptide.JoinIterates(aa)))
                    Next
                Next

                ' 更新肽链列表为当前长度的组合
                peptides = newPeptides
            Next

            For Each combine As List(Of Char) In peptides
                Yield New String(combine.ToArray)
            Next
        End Function

        ''' <summary>
        ''' 值的氨基酸字符都是大写的
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ToChar As New Dictionary(Of AminoAcid, Char) From {
                                                                                    _
            {AminoAcid.Alanine, "A"c},
            {AminoAcid.Arginine, "R"c},
            {AminoAcid.Asparagine, "N"c},
            {AminoAcid.AsparticAcid, "D"c},
            {AminoAcid.Cysteine, "C"c},
            {AminoAcid.GlutamicAcid, "E"c},
            {AminoAcid.Glutamine, "Q"c},
            {AminoAcid.Glycine, "G"c},
            {AminoAcid.Histidine, "H"c},
            {AminoAcid.Isoleucine, "I"c},
            {AminoAcid.Leucine, "L"c},
            {AminoAcid.Lysine, "K"c},
            {AminoAcid.Methionine, "M"c},
            {AminoAcid.Phenylalanine, "F"c},
            {AminoAcid.Praline, "P"c},
            {AminoAcid.Serine, "S"c},
            {AminoAcid.Threonine, "T"c},
            {AminoAcid.Tryptophane, "W"c},
            {AminoAcid.Tyrosine, "Y"c},
            {AminoAcid.Valine, "V"c}
        }

        ''' <summary>
        ''' 键名字符应该是大写的字母
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ToEnums As New Dictionary(Of Char, AminoAcid) From {
                                                                                     _
             {"A"c, AminoAcid.Alanine},
             {"R"c, AminoAcid.Arginine},
             {"N"c, AminoAcid.Asparagine},
             {"D"c, AminoAcid.AsparticAcid},
             {"C"c, AminoAcid.Cysteine},
             {"E"c, AminoAcid.GlutamicAcid},
             {"Q"c, AminoAcid.Glutamine},
             {"G"c, AminoAcid.Glycine},
             {"H"c, AminoAcid.Histidine},
             {"I"c, AminoAcid.Isoleucine},
             {"L"c, AminoAcid.Leucine},
             {"K"c, AminoAcid.Lysine},
             {"M"c, AminoAcid.Methionine},
             {"F"c, AminoAcid.Phenylalanine},
             {"P"c, AminoAcid.Praline},
             {"S"c, AminoAcid.Serine},
             {"T"c, AminoAcid.Threonine},
             {"W"c, AminoAcid.Tryptophane},
             {"Y"c, AminoAcid.Tyrosine},
             {"V"c, AminoAcid.Valine},
             {"i"c, AminoAcid.InitCode},
             {"*"c, AminoAcid.StopCode}
        }

        ''' <summary>
        ''' Protein sequence display color schema from MEGA software.(MEGA软件的显示蛋白质序列的残基颜色)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MEGASchema As New Dictionary(Of Char, Color) From {
                                                                                    _
            {"A"c, "#ccff00".TranslateColor},
            {"B"c, Color.FromArgb(192, 192, 192)},
            {"D"c, Color.FromArgb(255, 0, 0)},
            {"E"c, Color.FromArgb(255, 0, 0)},
            {"F"c, Color.FromArgb(255, 255, 0)},
            {"G"c, "#ff9900".TranslateColor},
            {"H"c, Color.FromArgb(0, 128, 128)},
            {"I"c, Color.FromArgb(255, 255, 0)},
            {"J"c, Color.FromArgb(192, 192, 192)},
            {"K"c, Color.FromArgb(0, 0, 255)},
            {"L"c, Color.FromArgb(255, 255, 0)},
            {"M"c, Color.FromArgb(255, 255, 0)},
            {"N"c, Color.FromArgb(0, 128, 0)},
            {"O"c, Color.FromArgb(192, 192, 192)},
            {"P"c, Color.FromArgb(0, 0, 255)},
            {"Q"c, Color.FromArgb(0, 128, 0)},
            {"R"c, Color.FromArgb(0, 0, 255)},
            {"S"c, Color.FromArgb(0, 128, 0)},
            {"T"c, "#ff6600".TranslateColor},
            {"V"c, Color.FromArgb(255, 255, 0)},
            {"W"c, Color.FromArgb(0, 128, 0)},
            {"X"c, Color.FromArgb(192, 192, 192)},
            {"Y"c, Color.FromArgb(0, 255, 0)},
            {"Z"c, Color.FromArgb(192, 192, 192)}
        }

        ''' <summary>
        ''' Get the composition vector of this polypeptide sequence.
        ''' </summary>
        ''' <param name="seq">全部必须为大写字母</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCompositionVector(seq As Char()) As Integer()
            Dim B, D, E, F, H, I, J, K, L, M, N, O, P, Q, R, S, V, W, X, Y, Z As Integer

            B = seq.GetCount(aa:="B"c)
            D = seq.GetCount(aa:="D"c)
            E = seq.GetCount(aa:="E"c)
            F = seq.GetCount(aa:="F"c)
            H = seq.GetCount(aa:="H"c)
            I = seq.GetCount(aa:="I"c)
            J = seq.GetCount(aa:="J"c)
            K = seq.GetCount(aa:="K"c)
            L = seq.GetCount(aa:="L"c)
            M = seq.GetCount(aa:="M"c)
            N = seq.GetCount(aa:="N"c)
            O = seq.GetCount(aa:="O"c)
            P = seq.GetCount(aa:="P"c)
            Q = seq.GetCount(aa:="Q"c)
            R = seq.GetCount(aa:="R"c)
            S = seq.GetCount(aa:="S"c)
            V = seq.GetCount(aa:="V"c)
            W = seq.GetCount(aa:="W"c)
            X = seq.GetCount(aa:="X"c)
            Y = seq.GetCount(aa:="Y"c)
            Z = seq.GetCount(aa:="Z"c)

            Return New Integer() {B, D, E, F, H, I, J, K, L, M, N, O, P, Q, R, S, V, W, X, Y, Z}
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function GetCount(sequence As Char(), aa As Char) As Integer
            Return Aggregate ch As Char
                   In sequence
                   Where ch = aa
                   Into Count
        End Function

        ''' <summary>
        ''' 大小写不敏感
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Abbreviate As New Dictionary(Of String, String) From {
                                                                                       _
            {"Ala", "A"}, {"Arg", "R"}, {"Asp", "D"}, {"Asn", "N"},
            {"Cys", "C"},
            {"Gln", "Q"}, {"Glu", "E"}, {"Glt", "E"}, {"Gly", "G"},
            {"His", "H"},
            {"Ile", "I"},
            {"Leu", "L"}, {"Lys", "K"},
            {"Met", "M"},
            {"Phe", "F"}, {"Pro", "P"},
            {"Ser", "S"},
            {"Thr", "T"}, {"Trp", "W"}, {"Tyr", "Y"},
            {"Val", "V"},
            {"Sec", "U"}, {"Pyl", "O"},
                                       _
            {"alaT", "A"}, {"alaX", "A"}, {"alaW", "A"}, {"alaU", "A"}, {"alaV", "A"},
            {"argU", "R"}, {"argQ", "R"}, {"argX", "R"}, {"argV", "R"}, {"argZ", "R"}, {"argY", "R"}, {"argW", "R"},
            {"aspV", "D"}, {"aspT", "D"}, {"aspU", "D"},
            {"asnU", "N"}, {"asnV", "N"}, {"asnT", "N"},
            {"cysT", "C"},
            {"glnX", "Q"}, {"glnV", "Q"}, {"glnW", "Q"}, {"glnU", "Q"}, ' {"glu", "E"},
            {"gltT", "E"}, {"gltW", "E"}, {"gltU", "E"}, {"gltV", "E"},
            {"glyU", "G"}, {"glyV", "G"}, {"glyX", "G"}, {"glyY", "G"}, {"glyT", "G"}, {"glyW", "G"},
            {"hisR", "H"},
            {"ileT", "I"}, {"ileU", "I"}, {"ileV", "I"}, {"ileX", "I"},
            {"leuV", "L"}, {"leuP", "L"}, {"leuQ", "L"}, {"leuW", "L"}, {"leuX", "L"}, {"leuU", "L"}, {"leuZ", "L"}, {"leuT", "L"},
            {"lysV", "K"}, {"lysT", "K"}, {"lysW", "K"},
            {"metY", "M"}, {"metU", "M"}, {"metT", "M"}, {"metZ", "M"}, {"metW", "M"},
            {"pheV", "F"}, {"pheU", "F"},
            {"proK", "P"}, {"proM", "P"}, {"proL", "P"},
            {"serW", "S"}, {"serU", "S"}, {"serT", "S"}, {"serX", "S"}, {"serV", "S"},
            {"thrV", "T"}, {"thrU", "T"}, {"thrW", "T"}, {"thrT", "T"},
            {"trpT", "W"},
            {"tyrV", "Y"}, {"tyrU", "Y"}, {"tyrT", "Y"},
            {"valU", "V"}, {"valX", "V"}, {"valY", "V"}, {"valT", "V"}, {"valV", "V"}, {"valW", "V"},
            {"selC", "U"}, ' {"pyl", "O"},
                           _
            {"ala", "A"}, {"arg", "R"}, {"asp", "D"}, {"asn", "N"},
            {"cys", "C"},
            {"gln", "Q"}, {"glu", "E"}, {"glt", "E"}, {"gly", "G"},
            {"his", "H"},
            {"ile", "I"},
            {"leu", "L"}, {"lys", "K"},
            {"met", "M"},
            {"phe", "F"}, {"pro", "P"},
            {"ser", "S"},
            {"thr", "T"}, {"trp", "W"}, {"tyr", "Y"},
            {"val", "V"},
            {"sec", "U"}, {"pyl", "O"},
                                       _
            {"ALA", "A"}, {"ARG", "R"}, {"ASP", "D"}, {"ASN", "N"},
            {"CYS", "C"},
            {"GLN", "Q"}, {"GLU", "E"}, {"GLT", "E"}, {"GLY", "G"},
            {"HIS", "H"},
            {"ILE", "I"},
            {"LEU", "L"}, {"LYS", "K"},
            {"MET", "M"},
            {"PHE", "F"}, {"PRO", "P"},
            {"SER", "S"},
            {"THR", "T"}, {"TRP", "W"}, {"TYR", "Y"},
            {"VAL", "V"}
        }
    End Module
End Namespace
