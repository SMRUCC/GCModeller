#Region "Microsoft.VisualBasic::92ca7b1f98e6666c6fa5ce0b1d4aabf6, GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\Translation\Codon.vb"

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

    '   Total Lines: 134
    '    Code Lines: 73
    ' Comment Lines: 44
    '   Blank Lines: 17
    '     File Size: 4.42 KB


    '     Class Codon
    ' 
    '         Properties: CodonValue, IsInitCodon, IsStopCodon, TranslHashCode, X
    '                     Y, Z
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: CreateHashTable, Equals, GetHashCode, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Conversion

Namespace SequenceModel.NucleotideModels.Translation

    ''' <summary>
    ''' 密码子对象
    ''' </summary>
    Public Class Codon

        ''' <summary>
        ''' 密码子中的第一个碱基
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property X As DNA
        ''' <summary>
        ''' 密码子中的第二个碱基
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Y As DNA
        ''' <summary>
        ''' 密码子中的第三个碱基
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Z As DNA

        ''' <summary>
        ''' 第一个碱基*1000+第二个碱基*100+第三个碱基
        ''' </summary>
        ''' <returns>哈希值</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TranslHashCode As Integer
            Get
                Return GetHashCode(X, Y, Z)
            End Get
        End Property

        ''' <summary>
        ''' 返回三联体密码子的核酸片段，以三联体密码子字符串的形式显示当前的这个密码子内的内容
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CodonValue As String
            Get
                Return NucleicAcid.ToString({X, Y, Z})
            End Get
        End Property

        ''' <summary>
        ''' 是否为终止密码子
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsStopCodon As Boolean
        Public ReadOnly Property IsInitCodon As Boolean

        ''' <summary>
        ''' 非翻译用途的
        ''' </summary>
        Sub New()
        End Sub

        ''' <summary>
        ''' 翻译用途的
        ''' </summary>
        ''' <param name="tokens"></param>
        Friend Sub New(tokens As String())
            If tokens.Length = 4 Then
                IsInitCodon = True
            End If

            If String.Equals(tokens(1), "*") Then
                IsStopCodon = True
            End If

            Dim codon As DNA() = tokens(Scan0) _
                .Select(Function(ntch) NucleotideConvert(ntch)) _
                .ToArray

            X = codon(Scan0)
            Y = codon(1)
            Z = codon(2)
        End Sub

        ''' <param name="X">密码子中的第一个碱基</param>
        ''' <param name="Y">密码子中的第二个碱基</param>
        ''' <param name="Z">密码子中的第三个碱基</param>
        Public Overloads Shared Function GetHashCode(X As DNA, Y As DNA, Z As DNA) As Integer
            Return X * 1000 + Y * 100 + Z * 10000
        End Function

        Public Overrides Function ToString() As String
            Return X.Description & Y.Description & Z.Description
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If obj.GetType Is GetType(Codon) Then
                With DirectCast(obj, Codon)
                    Return .X = X AndAlso .Y = Y AndAlso .Z = Z
                End With
            Else
                Return False
            End If
        End Function

        Public Overloads Shared Narrowing Operator CType(obj As Codon) As Integer
            Return obj.TranslHashCode
        End Operator

        ''' <summary>
        ''' 生成非翻译用途的密码子列表
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function CreateHashTable() As Codon()
            Dim NNCols As DNA() = {DNA.dAMP, DNA.dCMP, DNA.dGMP, DNA.dTMP}
            Dim combos = CreateCombos(NNCols, NNCols)
            Dim tripleCombos = CreateCombos(combos, NNCols)
            Dim codens() = LinqAPI.Exec(Of Codon) _
 _
                () <= From coden As (a As (DNA, DNA), b As DNA)
                      In tripleCombos
                      Select New Codon With {
                          .X = coden.Item1.Item1,
                          .Y = coden.Item1.Item2,
                          .Z = coden.Item2
                      }

            Return codens
        End Function
    End Class
End Namespace
