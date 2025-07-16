
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular

Namespace MarkupCompiler.BioCyc

    Public Class GenomeCompiler

        ReadOnly biocyc As Workspace
        ReadOnly geneIndex As Dictionary(Of String, genes)
        ReadOnly proteinIndex As Dictionary(Of String, proteins)

        Sub New(compiler As v2Compiler)
            biocyc = compiler.biocyc
            geneIndex = biocyc.genes.features.ToDictionary(Function(g) g.uniqueId)
            proteinIndex = biocyc.proteins.features.ToDictionary(Function(a) a.uniqueId)
        End Sub

        Public Function CreateReplicon() As replicon
            Return New replicon With {
                .genomeName = biocyc.species.commonName,
                .isPlasmid = False,
                .operons = CreateOperons.ToArray
            }
        End Function

        Private Iterator Function CreateOperons() As IEnumerable(Of TranscriptUnit)
            For Each operon As transunits In biocyc.transunits.features
                Dim genes As gene() = GeneObjects(operon.components).ToArray

                If genes.IsNullOrEmpty Then
                    Continue For
                End If

                Yield New TranscriptUnit With {
                    .id = operon.uniqueId,
                    .name = operon.commonName,
                    .genes = genes,
                    .note = operon.comment
                }
            Next
        End Function

        Private Iterator Function GeneObjects(list As IEnumerable(Of String)) As IEnumerable(Of gene)
            For Each id As String In list
                Dim data As genes = geneIndex.TryGetValue(id)

                If data Is Nothing Then
                    Continue For
                End If

                Dim prot = If(data.product Is Nothing, Nothing, proteinIndex.TryGetValue(data.product))
                Dim rna_type As RNATypes = RNATypes.micsRNA

                If prot IsNot Nothing Then
                    rna_type = RNATypes.mRNA
                End If

                Yield New gene With {
                    .locus_tag = data.uniqueId,
                    .product = data.product,
                    .left = data.left,
                    .right = data.right,
                    .strand = data.direction.ToString,
                    .type = rna_type
                }
            Next
        End Function
    End Class
End Namespace