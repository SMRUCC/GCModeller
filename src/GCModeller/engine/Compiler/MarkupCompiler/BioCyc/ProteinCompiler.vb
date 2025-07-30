Imports System.Runtime.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
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
            Dim pid As New Index(Of String)
            Dim translates As Index(Of String) = biocyc.genes.features _
                .Select(Function(gene) gene.product) _
                .IteratesALL _
                .Indexing

            For Each cplx As protligandcplxes In biocyc.protligandcplxes.features
                Yield New protein With {
                    .ligand = cplx.components.Where(Function(id) Not id Like peptides).ToArray,
                    .peptide_chains = cplx.components.Where(Function(id) id Like peptides).ToArray,
                    .name = cplx.commonName,
                    .note = cplx.comment,
                    .protein_id = cplx.uniqueId
                }

                pid += cplx.uniqueId
            Next

            For Each prot As proteins In biocyc.proteins.features
                If prot.uniqueId Like pid Then
                    Continue For
                End If

                If Not prot.components.IsNullOrEmpty Then
                    Yield New protein With {
                        .name = prot.commonName,
                        .note = prot.comment,
                        .protein_id = prot.uniqueId,
                        .ligand = prot.components.Where(Function(c) Not c Like translates).ToArray,
                        .peptide_chains = prot.components.Where(Function(c) c Like translates).ToArray
                    }
                ElseIf Not prot.unmodified_form.StringEmpty(, True) Then
                    Yield New protein With {
                        .name = prot.commonName,
                        .note = prot.comment,
                        .protein_id = prot.uniqueId,
                        .peptide_chains = {prot.unmodified_form}
                    }
                ElseIf Not prot.gene.StringEmpty(, True) Then
                    Yield New protein With {
                        .protein_id = prot.uniqueId,
                        .name = prot.commonName,
                        .note = prot.comment,
                        .peptide_chains = {prot.uniqueId}
                    }
                Else
                    ' this protein object is broken
                    ' unsure its source 
                    Call VBDebugger.Warning($"Unsure how to processing the protein data: {prot.ToString}")

                    Yield New protein With {
                        .protein_id = prot.uniqueId,
                        .name = prot.commonName,
                        .note = prot.comment,
                        .peptide_chains = {prot.uniqueId}
                    }
                End If
            Next
        End Function
    End Class
End Namespace