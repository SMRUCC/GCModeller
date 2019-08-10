Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

''' <summary>
''' Module for create GSEA background model from bbh annotation result.
''' </summary>
Public Module BBHLibrary

    ''' <summary>
    ''' Create GSEA background model from bbh annotation result.
    ''' </summary>
    ''' <param name="annotations"></param>
    ''' <param name="backgroundSize">
    ''' The total number of genes in background genome. 
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateBackground(annotations As IEnumerable(Of BiDirectionalBesthit), define As GetClusterTerms,
                                     Optional backgroundSize% = -1,
                                     Optional outputAll As Boolean = True,
                                     Optional genomeName$ = "Unknown") As Background

        ' [clusterName => members]
        Dim clusters As New Dictionary(Of String, List(Of BackgroundGene))
        Dim clusterNotes As New Dictionary(Of String, NamedValue(Of String))
        Dim counts%

        For Each gene As BiDirectionalBesthit In annotations
            Dim clusterList = define(gene.QueryName)

            counts += 1

            For Each cluster As NamedValue(Of String) In clusterList
                If Not clusters.ContainsKey(cluster.Name) Then
                    clusters(cluster.Name) = New List(Of BackgroundGene)
                    clusterNotes(cluster.Name) = cluster
                End If

                clusters(cluster.Name) += New BackgroundGene With {
                    .accessionID = gene.QueryName,
                    .[alias] = {gene.HitName},
                    .locus_tag = New NamedValue With {
                        .name = gene.QueryName
                    },
                    .term_id = {gene.HitName},
                    .name = gene.Description
                }
            Next
        Next

        Return New Background With {
            .build = Now,
            .clusters = clusters _
                .Where(Function(c)
                           If outputAll Then
                               Return True
                           Else
                               Return c.Value > 0
                           End If
                       End Function) _
                .Select(Function(c)
                            Return c.Value.CreateCluster(c.Key, clusterNotes(c.Key))
                        End Function) _
                .ToArray,
            .comments = "GSEA background model build from BBH annotation result by GCModeller",
            .name = genomeName,
            .size = backgroundSize Or counts.When(backgroundSize <= 0)
        }
    End Function
End Module
