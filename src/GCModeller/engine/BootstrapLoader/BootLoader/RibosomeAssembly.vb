Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Namespace ModelLoader

    Public Class RibosomeAssembly : Inherits ProteinComplexGenerator

        ReadOnly rRNA_genes As Dictionary(Of String, List(Of String))

        Public Sub New(loader As Loader, rRNA_genes As Dictionary(Of String, List(Of String)))
            MyBase.New(loader)
            Me.rRNA_genes = rRNA_genes
        End Sub

        Protected Overrides Iterator Function GetComplexSet() As IEnumerable(Of Model.Cellular.Molecule.Protein)
            Dim cellular_id As String = cell.CellularEnvironmentName
            Dim totalProteinCount As Integer = cell.Genotype.ProteinMatrix.Length
            Dim L As New StatusMapFactor("L1-L36 Ribosomal Proteins", MassTable _
                .GetRole(MassRoles.protein) _
                .Where(Function(c) c.cellular_compartment = cellular_id) _
                .Keys, cellular_id, MassTable) With {.coefficient = 1 / totalProteinCount}
            Dim S As New StatusMapFactor("S1-S21 Ribosomal Proteins", MassTable _
                .GetRole(MassRoles.protein) _
                .Where(Function(c) c.cellular_compartment = cellular_id) _
                .Keys, cellular_id, MassTable) With {.coefficient = 1 / totalProteinCount}

            Call MassTable.AddOrUpdate(L, L.ID, cellular_id)
            Call MassTable.AddOrUpdate(S, S.ID, cellular_id)
            Call MassTable.addNew("50sRibosomal", MassRoles.protein, cellular_id)
            Call MassTable.addNew("30sRibosomal", MassRoles.protein, cellular_id)

            ' 5s + 23s + 34 * L = 50s
            ' 16s + 21 * S = 30s
            ' 30s + mRNA + 50s + GTP = 70s_mRNA + GDP + Pi
            ' 70s_mRNA + N * charged-aa-tRNA = 70s_mRNA + polypeptide + N * aa-tRNA + N * Pi
            ' 70s_mRNA = 30s + mRNA + 50s + Pi

            ' gene entity mapping to rRNA terms
            For Each type As KeyValuePair(Of String, List(Of String)) In rRNA_genes
                Dim rRNA_key As String = $"{type.Key}_rRNA"
                Dim rRNA As New StatusMapFactor(
                    id:=rRNA_key,
                    mass:=(From gene_id As String
                           In type.Value
                           Let mid As String = $"{gene_id}@{cellular_id}"
                           Select mid),
                    compart_id:=cellular_id,
                    env:=MassTable)

                ' add rRNA term mapping
                Call MassTable.AddOrUpdate(rRNA, rRNA.ID, cellular_id)
            Next

            ' 5s + 23s + 34 * L = 50s
            Yield New Model.Cellular.Molecule.Protein() With {
                .ProteinID = $"50sRibosomal@{cellular_id}",
                .compounds = {$"5s_rRNA@{cellular_id}", $"23s_rRNA@{cellular_id}"},
                .polypeptides = {$"L1-L36 Ribosomal Proteins@{cellular_id}"}
            }

            ' 16s + 21 * S = 30s
            Yield New Model.Cellular.Molecule.Protein() With {
                .ProteinID = $"30sRibosomal@{cellular_id}",
                .compounds = {$"16s_rRNA@{cellular_id}"},
                .polypeptides = {$"S1-S21 Ribosomal Proteins@{cellular_id}"}
            }
        End Function

        Protected Overrides Function Title(complex As Model.Cellular.Molecule.Protein, cellular_id As String) As String
            Return $"Ribosome assembly of {complex.ProteinID} in cell {cellular_id}"
        End Function
    End Class
End Namespace