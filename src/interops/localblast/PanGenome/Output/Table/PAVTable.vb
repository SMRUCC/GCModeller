Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.ComponentModel.Annotation

Public Class PAVTable : Implements IDynamicMeta(Of Integer), IOrthologyCluster, INamedValue

    Public Property FamilyID As String Implements IOrthologyCluster.FamilyID, INamedValue.Key
    Public Property PAV As Dictionary(Of String, Integer) Implements IDynamicMeta(Of Integer).Properties
    Public Property ClusterGenes As String() Implements IOrthologyCluster.GeneCluster
    Public Property Category As GeneCategoryType
    Public Property Dispensable As Boolean
    Public Property SingleCopyOrtholog As Boolean

    Default Public Property GenomeData(name As String) As Integer
        Get
            If PAV Is Nothing OrElse Not PAV.ContainsKey(name) Then
                Return 0
            Else
                Return PAV(name)
            End If
        End Get
        Set(value As Integer)
            If PAV Is Nothing Then
                PAV = New Dictionary(Of String, Integer)
            End If

            PAV(name) = value
        End Set
    End Property

    Public ReadOnly Property Size As Integer
        Get
            Return ClusterGenes.TryCount
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return FamilyID
    End Function

End Class
