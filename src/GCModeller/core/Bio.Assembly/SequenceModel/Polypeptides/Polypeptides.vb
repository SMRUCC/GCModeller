#Region "Microsoft.VisualBasic::f4edd97db54d2ec0710509a1d9c7efbf, ..\core\Bio.Assembly\SequenceModel\Polypeptides\Polypeptides.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid

Namespace SequenceModel.Polypeptides

    ''' <summary>
    ''' Protein polypeptide sequence.(蛋白质多肽链的一些相关操作)
    ''' </summary>
    ''' <remarks></remarks>
    Public Module Polypeptides

        ''' <summary>
        ''' Enumerates all of the 20 amino acids.(枚举20种常见的氨基酸)
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum AminoAcid As Integer

            ''' <summary>
            ''' 用于表示空载的tRNA
            ''' </summary>
            ''' <remarks></remarks>
            NULL = -1

            ''' <summary>
            ''' Ala(A) 丙氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Alanine = 0

            ''' <summary>
            ''' Arg(R) 精氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Arginine

            ''' <summary>
            ''' Asn(N) 天冬酰胺
            ''' </summary>
            ''' <remarks></remarks>
            Asparagine

            ''' <summary>
            ''' Asp(D) 天冬氨酸
            ''' </summary>
            ''' <remarks></remarks>
            AsparticAcid

            ''' <summary>
            ''' Cys(C) 半胱氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Cysteine

            ''' <summary>
            ''' Gln(Q) 谷氨酰胺
            ''' </summary>
            ''' <remarks></remarks>
            Glutamine

            ''' <summary>
            ''' Glu(E) 谷氨酸
            ''' </summary>
            ''' <remarks></remarks>
            GlutamicAcid

            ''' <summary>
            ''' Gly(G) 甘氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Glycine

            ''' <summary>
            ''' His(H) 组氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Histidine

            ''' <summary>
            ''' Ile(I) 异亮氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Isoleucine

            ''' <summary>
            ''' Leu(L) 亮氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Leucine

            ''' <summary>
            ''' Lys(K) 赖氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Lysine

            ''' <summary>
            ''' Met(M) 甲硫氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Methionine

            ''' <summary>
            ''' Ser(S) 丝氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Serine

            ''' <summary>
            ''' Phe(F) 苯丙氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Phenylalanine

            ''' <summary>
            ''' Pro(P) 脯氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Praline

            ''' <summary>
            ''' Thr(T) 苏氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Threonine

            ''' <summary>
            ''' TrP(W) 色氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Tryptophane

            ''' <summary>
            ''' Tyr(Y) 酪氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Tyrosine

            ''' <summary>
            ''' Val(V) 结氨酸
            ''' </summary>
            ''' <remarks></remarks>
            Valine

            ''' <summary>
            ''' 终止密码子
            ''' </summary>
            ''' <remarks></remarks>
            StopCode = 100
            ''' <summary>
            ''' 起始密码子
            ''' </summary>
            InitCode = 200
        End Enum

        Public Function ConstructVector(SequenceData As String) As Polypeptides.AminoAcid()
            Dim LQuery As AminoAcid() = (From ch As Char
                                         In SequenceData.ToUpper
                                         Let AA As AminoAcid = If(ToEnums.ContainsKey(ch), ToEnums(ch), AminoAcid.NULL)
                                         Select AA).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 值的氨基酸字符都是大写的
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ToChar As Dictionary(Of AminoAcid, Char) =
            New Dictionary(Of AminoAcid, Char) From {
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
        Public ReadOnly Property ToEnums As Dictionary(Of Char, AminoAcid) =
            New Dictionary(Of Char, AminoAcid) From {
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
        Public ReadOnly Property MEGASchema As Dictionary(Of Char, Color) =
            New Dictionary(Of Char, Color) From {
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

        <Extension>
        Private Function GetCount(sequence As Char(), aa As Char) As Integer
            Dim LQuery = (From ch In sequence Where ch = aa Select 1).Count
            Return LQuery
        End Function

        Public ReadOnly Property Abbreviate As Dictionary(Of String, String) =
            New Dictionary(Of String, String) From {
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
