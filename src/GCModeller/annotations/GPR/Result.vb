Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.MetabolicModel

Public Class ScoredReaction : Implements INamedValue
    Public Property Id As String Implements INamedValue.Key
    Public Property Score As Double
End Class

Public Class GeneAssociation : Implements INamedValue

    Public Property GeneId As String Implements INamedValue.Key
    Public Property Reactions As Dictionary(Of String, ScoredReaction)

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