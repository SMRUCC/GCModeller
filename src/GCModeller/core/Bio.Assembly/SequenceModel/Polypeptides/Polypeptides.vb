#Region "Microsoft.VisualBasic::099cfa9941a77e2823de342b0c3915c8, Bio.Assembly\SequenceModel\Polypeptides\Polypeptides.vb"

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

Namespace SequenceModel.Polypeptides

    ''' <summary>
    ''' Protein polypeptide sequence.(蛋白质多肽链的一些相关操作)
    ''' </summary>
    ''' <remarks></remarks>
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
            {"B"c, Color.FromArgb(192, 192, 192)},
            {"D"c, Color.FromArgb(255, 0, 0)},
            {"E"c, Color.FromArgb(255, 0, 0)},
            {"F"c, Color.FromArgb(255, 255, 0)},
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
            {"V"c, Color.FromArgb(255, 255, 0)},
            {"W"c, Color.FromArgb(0, 128, 0)},
            {"X"c, Color.FromArgb(192, 192, 192)},
            {"Y"c, Color.FromArgb(0, 255, 0)},
            {"Z"c, Color.FromArgb(192, 192, 192)}
        }

        ''' <summary>
        ''' Get the composition vector of this polypeptide sequence.
        ''' </summary>
        ''' <param name="Seq">全部必须为大写字母</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCompositionVector(Seq As Char()) As Integer()
            Dim B, D, E, F, H, I, J, K, L, M, N, O, P, Q, R, S, V, W, X, Y, Z As Integer

            B = Seq.GetCount(aa:="B"c)
            D = Seq.GetCount(aa:="D"c)
            E = Seq.GetCount(aa:="E"c)
            F = Seq.GetCount(aa:="F"c)
            H = Seq.GetCount(aa:="H"c)
            I = Seq.GetCount(aa:="I"c)
            J = Seq.GetCount(aa:="J"c)
            K = Seq.GetCount(aa:="K"c)
            L = Seq.GetCount(aa:="L"c)
            M = Seq.GetCount(aa:="M"c)
            N = Seq.GetCount(aa:="N"c)
            O = Seq.GetCount(aa:="O"c)
            P = Seq.GetCount(aa:="P"c)
            Q = Seq.GetCount(aa:="Q"c)
            R = Seq.GetCount(aa:="R"c)
            S = Seq.GetCount(aa:="S"c)
            V = Seq.GetCount(aa:="V"c)
            W = Seq.GetCount(aa:="W"c)
            X = Seq.GetCount(aa:="X"c)
            Y = Seq.GetCount(aa:="Y"c)
            Z = Seq.GetCount(aa:="Z"c)

            Return New Integer() {B, D, E, F, H, I, J, K, L, M, N, O, P, Q, R, S, V, W, X, Y, Z}
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function GetCount(sequence As Char(), aa As Char) As Integer
            Dim LQuery = (From ch In sequence Where ch = aa Select 1).Count
            Return LQuery
        End Function

        ''' <summary>
        ''' 大小写不敏感
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Abbreviate As New Dictionary(Of String, String) From {
 _
            {"Ala", "A"}, {"Arg", "R"}, {"Asp", "D"}, {"Asn", "N"},
            {"Cys", "C"},
            {"Gln", "Q"}, {"Glu", "E"}, {"Gly", "G"},
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
            {"ALA", "A"}, {"ARG", "R"}, {"ASP", "D"}, {"ASN", "N"},
            {"CYS", "C"},
            {"GLN", "Q"}, {"GLU", "E"}, {"GLY", "G"},
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
