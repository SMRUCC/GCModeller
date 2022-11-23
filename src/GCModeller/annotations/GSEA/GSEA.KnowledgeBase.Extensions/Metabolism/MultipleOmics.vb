Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' helper for create enrichment background model for multiple omics data analysis
''' </summary>
Public Module MultipleOmics

    Public Function CreateOmicsBackground(model As IEnumerable(Of Pathway)) As Background
        Dim clusters As Cluster() = model.Select(Function(m) getCluster(m)).ToArray

        Return New Background With {
            .clusters = clusters,
            .build = Now,
            .comments = "KEGG pathway multiple omics analysis",
            .id = "",
            .name = "",
            .size = .clusters.BackgroundSize
        }
    End Function

    Private Function getCluster(model As Pathway) As Cluster
        Dim molecules As New List(Of BackgroundGene)

        For Each gene As GeneName In model.genes.SafeQuery
            molecules.Add(New BackgroundGene With {
                .accessionID = gene.geneId,
                .name = gene.geneName,
                .[alias] = {gene.geneId, gene.geneName},
                .locus_tag = New NamedValue With {.name = gene.geneId, .text = gene.description},
                .term_id = New String() {gene.KO} _
                    .JoinIterates(gene.EC) _
                    .ToArray
            })
        Next

        For Each compound As NamedValue In model.compound.SafeQuery
            molecules.Add(New BackgroundGene With {
                .accessionID = compound.name,
                .[alias] = {compound.name},
                .term_id = {compound.name},
                .name = compound.text,
                .locus_tag = compound
            })
        Next

        Return New Cluster With {
            .ID = model.EntryId,
            .names = model.name,
            .description = model.description,
            .members = molecules.ToArray
        }
    End Function
End Module