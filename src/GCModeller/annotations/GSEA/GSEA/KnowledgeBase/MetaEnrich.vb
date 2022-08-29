Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Module MetaEnrich

    Public Delegate Function GraphQuery(geneId As String) As IEnumerable(Of NamedCollection(Of String))

    ''' <summary>
    ''' create a new background model based on the enrichment analysis result
    ''' </summary>
    ''' <param name="background"></param>
    ''' <param name="query"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CastBackground(background As IEnumerable(Of EnrichmentResult), query As GraphQuery) As Background
        Return New Background With {
            .build = Now,
            .clusters = background _
                .Select(Function(a) a.CastCluster(query)) _
                .ToArray
        }
    End Function

    <Extension>
    Private Function CastCluster(background As EnrichmentResult, query As GraphQuery) As Cluster
        Return New Cluster With {
            .ID = background.term,
            .names = background.name,
            .description = background.description,
            .members = background.geneIDs _
                .Select(AddressOf query.Invoke) _
                .IteratesALL _
                .Select(Function(a) a.value) _
                .IteratesALL _
                .Distinct _
                .Select(Function(id)
                            Return New BackgroundGene With {
                                .accessionID = id,
                                .[alias] = {id},
                                .locus_tag = New NamedValue With {
                                    .name = id,
                                    .text = id
                                },
                                .name = id,
                                .term_id = {id}
                            }
                        End Function) _
                .ToArray
        }
    End Function

End Module
