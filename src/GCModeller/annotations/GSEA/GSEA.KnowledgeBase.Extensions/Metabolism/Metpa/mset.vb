﻿Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace Metabolism.Metpa

    ''' <summary>
    ''' a molecule collection
    ''' </summary>
    Public Class mset

        Public Property metaboliteNames As String()
        Public Property kegg_id As String()

        ''' <summary>
        ''' the pathway id
        ''' </summary>
        ''' <returns></returns>
        Public Property clusterId As String

        Public Function ToClusterModel(id As String) As Cluster
            Dim molecules As BackgroundGene() = New BackgroundGene(kegg_id.Length - 1) {}

            For i As Integer = 0 To molecules.Length - 1
                molecules(i) = New BackgroundGene With {
                    .accessionID = _kegg_id(i),
                    .[alias] = {_kegg_id(i)},
                    .locus_tag = New NamedValue(_kegg_id(i), _metaboliteNames(i)),
                    .name = _metaboliteNames(i),
                    .term_id = {}
                }
            Next

            Return New Cluster With {
                .ID = id,
                .names = clusterId,
                .description = clusterId,
                .members = molecules
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"[{clusterId}] has {kegg_id.Length} compounds. ({metaboliteNames.Take(3).JoinBy("; ")}...)"
        End Function

    End Class

    ''' <summary>
    ''' the molecule collection for each pathway cluster
    ''' </summary>
    ''' <remarks>
    ''' the molecule collection save in vector data model <see cref="mset"/>
    ''' </remarks>
    Public Class msetList

        Public Property list As Dictionary(Of String, mset)

        Public Shared Function CountUnique(Of T As PathwayBrief)(models As T()) As Integer
            Return Aggregate cpd As NamedValue(Of String)
                   In models.Select(Function(a) a.GetCompoundSet).IteratesALL
                   Group By cpd.Name Into Group
                   Into Count
        End Function

        Public Iterator Function GetClusters() As IEnumerable(Of Cluster)
            For Each mapId As String In list.Keys
                Yield list(mapId).ToClusterModel(mapId)
            Next
        End Function

    End Class
End Namespace