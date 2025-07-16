
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

Namespace MarkupCompiler.BioCyc

    Public Class GenomeCompiler

        ReadOnly biocyc As Workspace
        ReadOnly geneIndex As Dictionary(Of String, genes)

        Sub New(compiler As v2Compiler)
            biocyc = compiler.biocyc
            geneIndex = biocyc.genes.features.ToDictionary(Function(g) g.uniqueId)
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
                Yield New TranscriptUnit With {
                    .id = operon.uniqueId,
                    .name = operon.commonName,
                    .genes = GeneObjects(operon.components).ToArray,
                    .note = operon.comment
                }
            Next
        End Function

        Private Iterator Function GeneObjects(list As IEnumerable(Of String)) As IEnumerable(Of gene)
            For Each id As String In list
                Dim data As genes = geneIndex(id)

                Yield New gene With {
                    .locus_tag = data.uniqueId,
                    .product = data.product,
                    .left = data.left,
                    .right = data.right,
                    .strand = data.direction.ToString
                }
            Next
        End Function
    End Class
End Namespace