#Region "Microsoft.VisualBasic::54e925c4b472b2902ec5db3c53b225d9, ..\localblast\LocalBLAST\LocalBLAST\BlastOutput\Common\Score.vb"

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

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization.JSON
Imports r = System.Text.RegularExpressions.Regex

Namespace LocalBLAST.BLASTOutput.ComponentModel

    Public Class BlastXScore : Inherits Score

        Public Const REGEX_BLASTX_SCORE As String = " Score = .+?Identities\s+=.+?Frame =\s+[+-]?\d"

        <XmlAttribute>
        Public Property Frame As Integer

        ' Score = 125 bits (313), Expect = 6e-034, Method: Compositional matrix adjust.
        ' Identities = 90/258 (35%), Positives = 125/258 (48%), Gaps = 14/258 (5%)
        ' Frame = +3

        Public Shared Function ParseText(score$) As BlastXScore
            Dim lines = score.lTokens
            Dim items = lines _
                .Select(Function(l) l.Split(","c)) _
                .IteratesALL _
                .Select(Function(s) s.GetTagValue("=", trim:=True)) _
                .ToDictionary _
                .FlatTable
            Dim method$ = Strings _
                .Split(lines(Scan0), "Method:") _
                .Last _
                .Trim

            score = items!Score
            score = r _
                .Match(score, "\(\d+\)") _
                .Value _
                .GetStackValue("(", ")")

            Return New BlastXScore With {
                .Expect = items!Expect,
                .Method = method,
                .Frame = items!Frame,
                .Gaps = Percentage.TryParse(items!Gaps),
                .Identities = Percentage.TryParse(items!Identities),
                .Positives = Percentage.TryParse(items!Positives),
                .Score = Val(items!Score),
                .RawScore = Val(score)
            }
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}{1}  {2}", If(Frame > 0, "+", ""), Frame, MyBase.ToString)
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

        Public Shared Function TryParse(text As String) As Score
            Return TryParse(Of Score)(text)
        End Function

        Public Shared Function TryParse(Of T As Score)(text As String) As T
            Dim score As T = Activator.CreateInstance(Of T)()
            score.Expect = Val(Regex.Replace(text.Match(HIT_E_VALUE), "Expect\s+=\s+", "").Trim)
            score.Identities = Percentage.TryParse(Mid(text.Match(HIT_IDENTITIES), 14))
            score.Positives = Percentage.TryParse(Mid(text.Match(HIT_POSITIVES), 13))
            score.Gaps = Percentage.TryParse(Mid(text.Match(HIT_GAPS), 8))

            text = Regex.Match(text, "Score\s=.+?bits\s+\(\d+\)").Value

            Dim Scores As String() = Regex.Matches(text, _DOUBLE).ToArray

            If Scores.Length >= 2 Then
                score.Score = Scores(0).RegexParseDouble
                score.RawScore = Scores(1).RegexParseDouble
                score.Method = Mid(Regex.Match(text, "Method: .+?$", RegexOptions.Multiline).Value, 9)

                If Not String.IsNullOrEmpty(score.Method) Then
                    score.Method = Mid(score.Method, 1, Len(score.Method) - 2)
                End If
            Else

                Dim ex As New Exception("[DEBUG] Exception Snaps:" & vbCrLf & vbCrLf & text)

                Call App.LogException(ex)
                Call ex.PrintException
            End If

            Return score
        End Function
    End Class
End Namespace
