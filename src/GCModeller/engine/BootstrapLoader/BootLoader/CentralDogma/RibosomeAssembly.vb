#Region "Microsoft.VisualBasic::7b88797e299cda0587ca637e4ec13a5c, engine\BootstrapLoader\BootLoader\CentralDogma\RibosomeAssembly.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 81
    '    Code Lines: 58 (71.60%)
    ' Comment Lines: 9 (11.11%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 14 (17.28%)
    '     File Size: 3.90 KB


    '     Class RibosomeAssembly
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetComplexSet, GetMappedComplexID, Title
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Namespace ModelLoader

    Public Class RibosomeAssembly : Inherits ProteinComplexGenerator

        ReadOnly rRNA_genes As Dictionary(Of String, List(Of String))

        Public Const Ribosomal50s As String = "50sRibosomal"
        Public Const Ribosomal30s As String = "30sRibosomal"

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
            Call MassTable.addNew(Ribosomal50s, MassRoles.protein, cellular_id)
            Call MassTable.addNew(Ribosomal30s, MassRoles.protein, cellular_id)

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
                .ProteinID = $"{Ribosomal50s}@{cellular_id}",
                .compounds = {$"5s_rRNA@{cellular_id}", $"23s_rRNA@{cellular_id}"},
                .polypeptides = {$"L1-L36 Ribosomal Proteins@{cellular_id}"}
            }

            ' 16s + 21 * S = 30s
            Yield New Model.Cellular.Molecule.Protein() With {
                .ProteinID = $"{Ribosomal30s}@{cellular_id}",
                .compounds = {$"16s_rRNA@{cellular_id}"},
                .polypeptides = {$"S1-S21 Ribosomal Proteins@{cellular_id}"}
            }
        End Function

        Protected Overrides Function Title(complex As Model.Cellular.Molecule.Protein, cellular_id As String) As String
            Return $"Ribosome assembly of {complex.ProteinID} in cell {cellular_id}"
        End Function

        Protected Overrides Function GetMappedComplexID(protein As Model.Cellular.Molecule.Protein) As String
            Return protein.ProteinID
        End Function
    End Class
End Namespace
