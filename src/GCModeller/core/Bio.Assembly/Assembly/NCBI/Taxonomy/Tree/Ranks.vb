#Region "Microsoft.VisualBasic::e6c8e08acf2d11775f0c335fff06080b, Bio.Assembly\Assembly\NCBI\Taxonomy\Tree\Ranks.vb"

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

    '     Structure Ranks
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

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

            For Each node In tree.Taxonomy
                Select Case node.Value.rank
                    Case NcbiTaxonomyTree.class
                        [class] += node.Value
                    Case NcbiTaxonomyTree.family
                        family += node.Value
                    Case NcbiTaxonomyTree.genus
                        genus += node.Value
                    Case NcbiTaxonomyTree.order
                        order += node.Value
                    Case NcbiTaxonomyTree.phylum
                        phylum += node.Value
                    Case NcbiTaxonomyTree.species
                        species += node.Value
                    Case NcbiTaxonomyTree.superkingdom
                        superkingdom += node.Value
                    Case Nothing
                    Case Else
                        Throw New InvalidConstraintException(node.Value.GetJson)
                End Select

                node.Value.taxid = node.Key
            Next
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
