#Region "Microsoft.VisualBasic::b8e86cd11ba520f8f1d51d556cff50eb, Bio.Assembly\Assembly\NCBI\Taxonomy\Tree\Ranks.vb"

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

    ''' <summary>
    ''' 将物种分类节点按照分类等级进行分组
    ''' </summary>
    Public Class Ranks

        ' taxonomy_name => node
        ReadOnly species As New Dictionary(Of String, TaxonomyNode)
        ReadOnly genus As New Dictionary(Of String, TaxonomyNode)
        ReadOnly family As New Dictionary(Of String, TaxonomyNode)
        ReadOnly order As New Dictionary(Of String, TaxonomyNode)
        ReadOnly [class] As New Dictionary(Of String, TaxonomyNode)
        ReadOnly phylum As New Dictionary(Of String, TaxonomyNode)
        ReadOnly superkingdom As New Dictionary(Of String, TaxonomyNode)

        ' find lineage
        ReadOnly ncbiTaxonomy As NcbiTaxonomyTree

        Sub New(tree As NcbiTaxonomyTree)
            For Each treeNode In tree.Taxonomy
                Dim node As TaxonomyNode = treeNode.Value

                node.taxid = treeNode.Key

                Select Case node.rank
                    Case NcbiTaxonomyTree.class : [class].Add(LCase(node.name), node)
                    Case NcbiTaxonomyTree.family : family.Add(LCase(node.name), node)
                    Case NcbiTaxonomyTree.genus : genus.Add(LCase(node.name), node)
                    Case NcbiTaxonomyTree.order : order.Add(LCase(node.name), node)
                    Case NcbiTaxonomyTree.phylum : phylum.Add(LCase(node.name), node)
                    Case NcbiTaxonomyTree.species : species.Add(LCase(node.name), node)
                    Case NcbiTaxonomyTree.superkingdom : superkingdom.Add(LCase(node.name), node)
                    Case Nothing
                        ' do nothing
                    Case Else
                        Throw New InvalidConstraintException(node.GetJson)
                End Select
            Next

            Me.ncbiTaxonomy = tree
        End Sub

        Private Function getByRank(rank As String) As Dictionary(Of String, TaxonomyNode)
            Select Case LCase(rank)
                Case NcbiTaxonomyTree.class : Return [class]
                Case NcbiTaxonomyTree.family : Return family
                Case NcbiTaxonomyTree.genus : Return genus
                Case NcbiTaxonomyTree.order : Return order
                Case NcbiTaxonomyTree.phylum : Return phylum
                Case NcbiTaxonomyTree.species : Return species
                Case NcbiTaxonomyTree.superkingdom : Return superkingdom
                Case Else
                    Throw New NotImplementedException(rank)
            End Select
        End Function

        Public Function GetLineage(taxonomyName$, rank$) As TaxonomyNode()
            Dim list As Dictionary(Of String, TaxonomyNode) = getByRank(rank)

            With LCase(taxonomyName)
                If list.ContainsKey(.ByRef) Then
                    Return ncbiTaxonomy.GetAscendantsWithRanksAndNames(list(.ByRef).taxid, True)
                Else
                    Return {}
                End If
            End With
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
