Imports Microsoft.VisualBasic.Language

Public Module HMMCompares

    ''' <summary>
    ''' 计算HMM模型的相似度
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <param name="cut"></param>
    ''' <returns></returns>
    Public Function Equals(a As HMM, b As HMM, Optional cut As Double = 0.6) As Boolean
        Dim helper As New __compareHelper(cut)
        Dim c As Boolean = helper.Equals(a.COMPO, b.COMPO)
        If Not c Then
            Return False
        End If
        Dim edits As DistResult =
            LevenshteinDistance.ComputeDistance(a.nodes, b.nodes, AddressOf helper.Equals, AddressOf __char)
        If edits Is Nothing Then
            Return False
        End If
        Return edits.MatchSimilarity >= cut
    End Function

    Private Function __char(n As Node) As Char
        Dim i As Integer = n.Match.MaxIndex
        Return CType(i, AAIndex).ToString.First
    End Function

    Const A As Integer = Asc("A"c)

    Private Structure __compareHelper

        Public threshold As Double

        Sub New(cut As Double)
            threshold = cut
        End Sub

        Public Overloads Function Equals(a As Node, b As Node) As Boolean
            Dim i As Boolean = Correlations.GetPearson(a.Insert, b.Insert) >= threshold
            Dim m As Boolean = Correlations.GetPearson(a.Match, b.Match) >= threshold
            Dim s As Boolean = Correlations.GetPearson(a.StateTransitions, b.StateTransitions) >= threshold

            Return i AndAlso m AndAlso s
        End Function
    End Structure
End Module

