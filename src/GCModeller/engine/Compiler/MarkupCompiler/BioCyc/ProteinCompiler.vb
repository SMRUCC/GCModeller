#Region "Microsoft.VisualBasic::48183f97baaa8d70c0d349fd95ab326f, engine\Compiler\MarkupCompiler\BioCyc\ProteinCompiler.vb"

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

    '   Total Lines: 94
    '    Code Lines: 79 (84.04%)
    ' Comment Lines: 2 (2.13%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 13 (13.83%)
    '     File Size: 3.86 KB


    '     Class ProteinCompiler
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateProteins, proteinFeatures
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

        Private Iterator Function proteinFeatures() As IEnumerable(Of String)
            For Each gene As genes In biocyc.genes.features
                For Each gene_id As String In gene.product.SafeQuery
                    Yield gene_id
                Next
            Next

            For Each prot As proteins In biocyc.proteins.features
                Yield prot.uniqueId
            Next

            For Each cplx As protligandcplxes In biocyc.protligandcplxes.features
                Yield cplx.uniqueId
            Next
        End Function

        Public Iterator Function CreateProteins() As IEnumerable(Of protein)
            Dim pid As New Index(Of String)
            Dim translates As Index(Of String) = proteinFeatures.Distinct.Indexing

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
