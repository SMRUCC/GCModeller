Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

''' <summary>
''' helper for create kegg pathway maps enrichment background model
''' </summary>
Public Module KeggPathway

    ''' <summary>
    ''' create a gene background model based on the given of kegg pathway object
    ''' </summary>
    ''' <param name="models"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateModel(models As IEnumerable(Of Pathway)) As Background
        Dim clusters As Cluster() = models _
            .Select(Function(p) p.getGeneCluster) _
            .ToArray
        Dim model As New Background With {
            .build = Now,
            .clusters = clusters,
            .id = "",
            .comments = "",
            .name = "",
            .size = .clusters.BackgroundSize
        }

        Return model
    End Function

    <Extension>
    Private Function getGeneCluster(model As Pathway) As Cluster
        Return New Cluster With {
            .description = model.description,
            .ID = model.EntryId,
            .members = model.GetGeneMembers.ToArray,
            .names = model.name
        }
    End Function

    <Extension>
    Public Iterator Function GetGeneMembers(model As Pathway) As IEnumerable(Of BackgroundGene)
        For Each gene As GeneName In model.genes.SafeQuery
            Yield New BackgroundGene With {
                .accessionID = gene.geneId,
                .name = gene.geneName,
                .[alias] = {gene.geneId, gene.geneName},
                .locus_tag = New NamedValue With {.name = gene.geneId, .text = gene.description},
                .term_id = New String() {gene.KO} _
                    .JoinIterates(gene.EC) _
                    .ToArray
            }
        Next
    End Function
End Module