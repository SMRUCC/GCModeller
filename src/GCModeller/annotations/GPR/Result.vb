Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.MetabolicModel

Public Class ScoredReaction : Implements INamedValue

    Public Property Id As String Implements INamedValue.Key
    Public Property Score As Double
    Public Property Unmapped As Boolean = False

    Public Overrides Function ToString() As String
        Return $"{Id}: {Score}"
    End Function
End Class

Public Class GeneAssociation : Implements INamedValue

    Public Property GeneId As String Implements INamedValue.Key
    Public Property Reactions As Dictionary(Of String, ScoredReaction)

    Public ReadOnly Property GPRLinks As Integer
        Get
            Return Reactions.Count
        End Get
    End Property

    Public ReadOnly Property MeanScore As Double
        Get
            If Reactions.Count = 0 Then
                Return 0
            Else
                Return Reactions.Values.Average(Function(a) a.Score)
            End If
        End Get
    End Property

    Public ReadOnly Property MedianScore As Double
        Get
            If Reactions.Count = 0 Then
                Return 0
            Else
                Return Reactions.Values.Select(Function(a) a.Score).Median
            End If
        End Get
    End Property

    Public ReadOnly Property TopGPRLinks As String()
        Get
            Dim cutoff As Double = MeanScore
            Dim mapped As IEnumerable(Of ScoredReaction) = From r As ScoredReaction
                                                           In Reactions.Values
                                                           Where Not r.Unmapped
            Return mapped _
                .Where(Function(r) r.Score >= cutoff) _
                .Select(Function(r) r.Id) _
                .ToArray
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{GeneId} - [{Reactions.Count}]{Reactions.Keys.GetJson}"
    End Function

End Class

Public Class Genome : Inherits GenomeContext(Of GeneTable)

    Public Property MetabolicNetwork As New Dictionary(Of String, GeneAssociation)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="name"></param>
    ''' <remarks>
    ''' impute <paramref name="genome"/> context data has been sorted by left in asc order
    ''' </remarks>
    Public Sub New(genome As IEnumerable(Of GeneTable), Optional name As String = "unnamed")
        MyBase.New(genome, name)
    End Sub

    Public Function GetGeneReactions(coGene As String) As IEnumerable(Of MetabolicReaction)
        If MetabolicNetwork.ContainsKey(coGene) Then
            Return MetabolicNetwork(coGene)
        Else
            Return {}
        End If
    End Function

    Public Iterator Function GetGenesForReaction(id As String) As IEnumerable(Of GeneTable)
        For Each gene As GeneAssociation In MetabolicNetwork.Values
            If gene.Reactions.ContainsKey(id) Then
                Yield Me(gene.GeneId)
            End If
        Next
    End Function
End Class