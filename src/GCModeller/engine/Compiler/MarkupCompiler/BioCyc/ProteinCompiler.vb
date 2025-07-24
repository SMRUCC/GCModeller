Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

Namespace MarkupCompiler.BioCyc

    Public Class ProteinCompiler

        ReadOnly biocyc As Workspace
        ReadOnly peptides As Index(Of String)

        Sub New(biocyc As Workspace)
            Me.biocyc = biocyc
            Me.peptides = biocyc.proteins.features _
                .Select(Function(a) a.uniqueId) _
                .Indexing
        End Sub

        Public Iterator Function CreateProteins() As IEnumerable(Of protein)
            For Each cplx As protligandcplxes In biocyc.protligandcplxes.features
                Yield New protein With {
                    .ligand = cplx.components.Where(Function(id) Not id Like peptides).ToArray,
                    .peptide_chains = cplx.components.Where(Function(id) id Like peptides).ToArray,
                    .name = cplx.commonName,
                    .note = cplx.comment,
                    .protein_id = cplx.uniqueId
                }
            Next
        End Function
    End Class
End Namespace