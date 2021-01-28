#Region "Microsoft.VisualBasic::8b5831c04ee0bed4e959dd21ed15d5c0, LocalBLAST\LocalBLAST\BlastOutput\Common\Score.vb"

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

    '     Class BlastXScore
    ' 
    '         Properties: Frame
    ' 
    '         Function: ParseText, ToString
    ' 
    '     Structure Strand
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Class BlastnScore
    ' 
    '         Properties: Strand
    ' 
    '         Function: ParseBlastn
    ' 
    '     Class Score
    ' 
    '         Properties: Expect, Gaps, Identities, Method, Positives
    '                     RawScore, Score
    ' 
    '         Function: ScoreTable, ToString, (+2 Overloads) TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Serialization.JSON
Imports r = System.Text.RegularExpressions.Regex

Namespace LocalBLAST.BLASTOutput.ComponentModel

    Public Class BlastXScore : Inherits Score

        Public Const REGEX_BLASTX_SCORE As String = " Score = .+?Identities\s+=.+?Frame =\s+[+-]?\d"

        ''' <summary>
        ''' The triple codon offset.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property Frame As Integer

        ' Score = 125 bits (313), Expect = 6e-034, Method: Compositional matrix adjust.
        ' Identities = 90/258 (35%), Positives = 125/258 (48%), Gaps = 14/258 (5%)
        ' Frame = +3

        Public Shared Function ParseText(scoreText$) As BlastXScore
            Dim data As Dictionary(Of String, String) = Nothing
            Dim blastXscore As BlastXScore = TryParse(Of BlastXScore)(scoreText, data)
            blastXscore.Frame = data!Frame
            Return blastXscore
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}{1}  {2}", If(Frame > 0, "+", ""), Frame, MyBase.ToString)
        End Function
    End Class

    Public Structure Strand

        Dim Query$, Subject$

        Sub New(value As String)
            With value.Split("/"c)
                Query = .First
                Subject = .Last
            End With
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Query}/{Subject}"
        End Function
    End Structure

    Public Class BlastnScore : Inherits Score
        Public Property Strand As Strand

        Public Shared Function ParseBlastn(text As String) As BlastnScore
            Dim items = ScoreTable(text)
            Dim score$ = r _
                .Match(items!Score, "\(\d+\)") _
                .Value _
                .GetStackValue("(", ")")

            Return New BlastnScore With {
                .Expect = items!Expect,
                .Gaps = Percentage.TryParse(items!Gaps),
                .Identities = Percentage.TryParse(items!Identities),
                .Score = Val(items!Score),
                .RawScore = Val(score),
                .Strand = New Strand(items!Strand)
            }
        End Function
    End Class

    ''' <summary>
    ''' Query和Subject之间的比对得分
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Score

        <XmlAttribute> Public Property Score As Double
        <XmlAttribute> Public Property RawScore As Double
        ''' <summary>
        ''' E-value 期望值
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Expect As Double
        <XmlAttribute> Public Property Method As String
        ''' <summary>
        ''' 相似度
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property Identities As Percentage
        <XmlElement> Public Property Positives As Percentage
        <XmlElement> Public Property Gaps As Percentage

        Public Const HIT_SCORE As String = "Score\s+=\s+" & _DOUBLE & "\s+bits\s+\(\d+\)"
        Public Const HIT_E_VALUE As String = "Expect = .+?,"
        Public Const HIT_IDENTITIES As String = "Identities\s+=\s+\d+/\d+\s+\(\d+%\)"
        Public Const HIT_POSITIVES As String = "Positives\s+=\s+\d+/\d+\s+\(\d+%\)"
        Public Const HIT_GAPS As String = "Gaps\s+=\s+\d+/\d+\s+\(\d+%\)"

        ''' <summary>
        ''' 一个任意实数
        ''' </summary>
        ''' <remarks></remarks>
        Const _DOUBLE As String = "((-?\d\.\d+e[+-]\d+)|(-?\d+\.\d+)|(-?\d+))"

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function TryParse(text As String) As Score
            Return TryParse(Of Score)(text)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="text"></param>
        ''' <returns></returns>.
        ''' <remarks>
        ''' 2018-10-30 可能有时候会因为原始数据文件分段读取的原因，会出现空缺
        ''' 在这个函数之中会需要进行这些空缺的额外处理来保证程序不会出错
        ''' </remarks>
        Public Shared Function ScoreTable(text As String) As Dictionary(Of String, String)
            Dim lines = r.Replace(text.StringSplit("Length[=]\d+").LastOrDefault(""), "Expect\(\d+\)", "Expect").LineTokens
            Dim items = lines _
                .Select(Function(l) l.Trim.Split(","c)) _
                .IteratesALL _
                .Where(Function(s) Not s.StringEmpty) _
                .Select(Function(s) s.Trim.GetTagValue("=", trim:=True)) _
                .Where(Function(t)
                           Return (Not t.Name Is Nothing) AndAlso (Not Strings.Trim(t.Name).StringEmpty)
                       End Function) _
                .ToDictionary(Function(item)
                                  ' 需要处理可能出现的空白
                                  Return item.Name.Matches("[a-z]+", RegexICSng).JoinBy("")
                              End Function,
                              Function(item) item.Value)

            ' blastn 的结果是没有Method的，method只存在于blastp和blastx之中
            If InStr(lines(Scan0), "Method:") > 0 Then
                Dim method$ = Strings _
                   .Split(lines(Scan0), "Method:") _
                   .Last _
                   .Trim

                items!Method = method
            End If

            Return items
        End Function

        Public Shared Function TryParse(Of T As {New, Score})(text$, Optional ByRef table As Dictionary(Of String, String) = Nothing) As T
            Dim items = TryCatch(Function() ScoreTable(text), text)
            Dim score$ = items!Score

            table = items
            score = r _
                .Match(score, "\(\d+\)") _
                .Value _
                .GetStackValue("(", ")")

            Return New T With {
                .Expect = items!Expect,
                .Method = items.TryGetValue("Method"),
                .Gaps = Percentage.TryParse(items!Gaps),
                .Identities = Percentage.TryParse(items!Identities),
                .Positives = Percentage.TryParse(items!Positives),
                .Score = Val(items!Score),
                .RawScore = Val(score)
            }
        End Function
    End Class
End Namespace
