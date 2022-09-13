#Region "Microsoft.VisualBasic::109c9508d4681ad9b51baed7c9d80ded, GCModeller\core\Bio.Assembly\Assembly\NCBI\Taxonomy\Tree\Ranks.vb"

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

    '   Total Lines: 101
    '    Code Lines: 66
    ' Comment Lines: 19
    '   Blank Lines: 16
    '     File Size: 4.37 KB


    '     Class Ranks
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: getByRank, GetLineage, ToString
    ' 
    '         Sub: addNode
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.NCBI.Taxonomy

    ''' <summary>
    ''' 将物种分类节点按照分类等级进行分组
    ''' </summary>
    Public Class Ranks

        ' taxonomy_name => node
        ReadOnly species As New Dictionary(Of String, List(Of TaxonomyNode))
        ReadOnly genus As New Dictionary(Of String, List(Of TaxonomyNode))
        ReadOnly family As New Dictionary(Of String, List(Of TaxonomyNode))
        ReadOnly order As New Dictionary(Of String, List(Of TaxonomyNode))
        ReadOnly [class] As New Dictionary(Of String, List(Of TaxonomyNode))
        ReadOnly phylum As New Dictionary(Of String, List(Of TaxonomyNode))
        ReadOnly superkingdom As New Dictionary(Of String, List(Of TaxonomyNode))

        ' find lineage
        ReadOnly ncbiTaxonomy As NcbiTaxonomyTree

        ''' <summary>
        ''' 只会将常用的7个分类等级的数据取出
        ''' </summary>
        ''' <param name="tree"></param>
        Sub New(tree As NcbiTaxonomyTree)
            For Each treeNode In tree.Taxonomy
                Dim node As TaxonomyNode = treeNode.Value

                node.taxid = treeNode.Key

                Select Case node.rank
                    Case NcbiTaxonomyTree.class : Call addNode([class], node)
                    Case NcbiTaxonomyTree.family : Call addNode(family, node)
                    Case NcbiTaxonomyTree.genus : Call addNode(genus, node)
                    Case NcbiTaxonomyTree.order : Call addNode(order, node)
                    Case NcbiTaxonomyTree.phylum : Call addNode(phylum, node)
                    Case NcbiTaxonomyTree.species : Call addNode(species, node)
                    Case NcbiTaxonomyTree.superkingdom : Call addNode(superkingdom, node)
                    Case Nothing, "", "no rank"
                        ' do nothing
                    Case Else
                        ' Throw New InvalidConstraintException(node.GetJson)
                        ' do nothing
                End Select
            Next

            Me.ncbiTaxonomy = tree
        End Sub

        Private Shared Sub addNode(pool As Dictionary(Of String, List(Of TaxonomyNode)), node As TaxonomyNode)
            If Not pool.ContainsKey(node.name) Then
                Call pool.Add(node.name, New List(Of TaxonomyNode))
            End If

            Call pool(node.name).Add(node)
        End Sub

        Private Function getByRank(rank As String) As IDictionary(Of String, List(Of TaxonomyNode))
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

        Public Iterator Function GetLineage(taxonomyName$, rank$) As IEnumerable(Of Metagenomics.Taxonomy)
            Dim list As IDictionary(Of String, List(Of TaxonomyNode)) = getByRank(rank)
            Dim nodes As List(Of TaxonomyNode) = list.GetValueOrNull(taxonomyName)
            Dim lineage As TaxonomyNode()
            Dim taxonomy As Metagenomics.Taxonomy

            'With LCase(taxonomyName)
            '    If list.ContainsKey(.ByRef) Then
            '        Return ncbiTaxonomy.GetAscendantsWithRanksAndNames(list(.ByRef).taxid, True)
            '    Else
            '        Return {}
            '    End If
            'End With

            If Not nodes.IsNullOrEmpty Then
                For Each node As TaxonomyNode In nodes
                    lineage = ncbiTaxonomy.GetAscendantsWithRanksAndNames(node.taxid, True)
                    taxonomy = New Metagenomics.Taxonomy(lineage)

                    Yield taxonomy
                Next
            End If
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
