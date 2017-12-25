#Region "Microsoft.VisualBasic::90b830dd2d4a281971e265cd4ea4cce7, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Taxonomy\Tree\TaxonomyNode.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Metagenomics.BIOMTaxonomy

Namespace Assembly.NCBI.Taxonomy

    ''' <summary>
    ''' The tree node calculation model for <see cref="NcbiTaxonomyTree"/>
    ''' </summary>
    Public Class TaxonomyNode

        Public Property taxid As Integer
        Public Property name As String
        Public Property rank As String
        ''' <summary>
        ''' 当前的节点的父节点的编号: ``<see cref="taxid"/>``
        ''' </summary>
        ''' <returns></returns>
        Public Property parent As String
        Public Property children As List(Of Integer)

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' 直接处理<see cref="NcbiTaxonomyTree.GetAscendantsWithRanksAndNames(Integer, Boolean)"/>的输出数据，不需要进行额外的排序操作
        ''' </summary>
        ''' <param name="tree"></param>
        ''' <param name="delimiter"></param>
        ''' <returns></returns>
        Public Shared Function Taxonomy(tree As TaxonomyNode(), Optional delimiter As String = ",") As String
            tree = tree.Reverse.ToArray
            Return String.Join(delimiter, tree.Select(Function(x) x.name).ToArray)
        End Function

        Public Shared Function RankTable(tree As IEnumerable(Of TaxonomyNode)) As Dictionary(Of String, String)
            Return (From x As TaxonomyNode
                    In tree
                    Where Not String.IsNullOrEmpty(x.rank)
                    Select x
                    Group x By x.rank Into Group) _
                         .ToDictionary(Function(x) x.rank,
                                       Function(x) x.Group.First.name)
        End Function
    End Class
End Namespace
