#Region "Microsoft.VisualBasic::5b973b1dd517509711ace0310b9fa14d, ..\core\Bio.Assembly\Assembly\NCBI\Taxonomy\Tree\Ranks.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language

Namespace Assembly.NCBI.Taxonomy

    Public Structure Ranks

        Public species As TaxonomyNode(),
            genus As TaxonomyNode(),
            family As TaxonomyNode(),
            order As TaxonomyNode(),
            [class] As TaxonomyNode(),
            phylum As TaxonomyNode(),
            superkingdom As TaxonomyNode()

        Sub New(tree As NcbiTaxonomyTree)
            Dim species As New List(Of TaxonomyNode),
                genus As New List(Of TaxonomyNode),
                family As New List(Of TaxonomyNode),
                order As New List(Of TaxonomyNode),
                [class] As New List(Of TaxonomyNode),
                phylum As New List(Of TaxonomyNode),
                superkingdom As New List(Of TaxonomyNode)

            For Each x In tree.Taxonomy
                Select Case x.Value.rank
                    Case NcbiTaxonomyTree.class
                        [class] += x.Value
                    Case NcbiTaxonomyTree.family
                        family += x.Value
                    Case NcbiTaxonomyTree.genus
                        genus += x.Value
                    Case NcbiTaxonomyTree.order
                        order += x.Value
                    Case NcbiTaxonomyTree.phylum
                        phylum += x.Value
                    Case NcbiTaxonomyTree.species
                        species += x.Value
                    Case NcbiTaxonomyTree.superkingdom
                        superkingdom += x.Value
                    Case Nothing
                    Case Else
                        Throw New InvalidConstraintException(x.Value.GetJson)
                End Select

                x.Value.taxid = x.Key
            Next
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
