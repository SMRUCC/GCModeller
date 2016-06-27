Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace LocalBLAST.BLASTOutput.ComponentModel

    Public Class BlastXScore : Inherits Score

        Public Const REGEX_BLASTX_SCORE As String = " Score = .+?Identities\s+=.+?Frame =\s+(\+|-)?\d"

        <XmlAttribute>
        Public Property Frame As Integer

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

        Public Shared Function TryParse(Of T As Score)(text As String) As T
            Dim score As T = Activator.CreateInstance(Of T)()
            score.Expect = Val(Regex.Replace(text.Match(HIT_E_VALUE), "Expect\s+=\s+", "").Trim)
            score.Identities = Percentage.TryParse(Mid(text.Match(HIT_IDENTITIES), 14))
            score.Positives = Percentage.TryParse(Mid(text.Match(HIT_POSITIVES), 13))
            score.Gaps = Percentage.TryParse(Mid(text.Match(HIT_GAPS), 8))

            text = Regex.Match(text, "Score\s=.+?bits\s+\(\d+\)").Value

            Dim Scores As String() = (From m As Match In Regex.Matches(text, _DOUBLE) Select m.Value).ToArray

            If Scores.Count >= 2 Then
                score.Score = Scores(0).RegexParseDouble
                score.RawScore = Scores(1).RegexParseDouble
                score.Method = Mid(Regex.Match(text, "Method: .+?$", RegexOptions.Multiline).Value, 9)
                If Not String.IsNullOrEmpty(score.Method) Then score.Method = Mid(score.Method, 1, Len(score.Method) - 2)
            Else

                Call Console.WriteLine("[DEBUG] Exception Snaps:" & vbCrLf & vbCrLf & text)
                Call FileIO.FileSystem.WriteAllText(RandomDouble() & "_blast_out_parse_error.log", text, append:=False)

            End If

            Return score
        End Function
    End Class
End Namespace