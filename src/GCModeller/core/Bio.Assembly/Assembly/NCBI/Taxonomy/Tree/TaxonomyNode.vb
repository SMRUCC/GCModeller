#Region "Microsoft.VisualBasic::cccf82a64456efb0b4438dbc7377d154, GCModeller\core\Bio.Assembly\Assembly\NCBI\Taxonomy\Tree\TaxonomyNode.vb"

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

    '   Total Lines: 61
    '    Code Lines: 31
    ' Comment Lines: 22
    '   Blank Lines: 8
    '     File Size: 2.37 KB


    '     Class TaxonomyNode
    ' 
    '         Properties: children, name, parent, rank, taxid
    ' 
    '         Function: RankTable, Taxonomy, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.NCBI.Taxonomy

    ''' <summary>
    ''' The tree node calculation model for <see cref="NcbiTaxonomyTree"/>
    ''' </summary>
    Public Class TaxonomyNode

        Public Property taxid As Integer
        Public Property name As String

        ''' <summary>
        ''' One of the value in array collection <see cref="NcbiTaxonomyTree.stdranks"/>.
        ''' (当前节点的分类等级)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 
        ''' </remarks>
        Public Property rank As String

        ''' <summary>
        ''' 当前的节点的父节点的编号: ``<see cref="taxid"/>``
        ''' </summary>
        ''' <returns></returns>
        Public Property parent As String
        Public Property children As List(Of Integer)

        ''' <summary>
        ''' get size of list <see cref="children"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property nchilds As Integer
            Get
                If children.IsNullOrEmpty Then
                    Return 0
                Else
                    Return children.Count
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' 直接处理<see cref="NcbiTaxonomyTree.GetAscendantsWithRanksAndNames(Integer, Boolean)"/>的输出数据，不需要进行额外的排序操作
        ''' </summary>
        ''' <param name="tree"></param>
        ''' <param name="delimiter"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Taxonomy(tree As TaxonomyNode(), Optional delimiter As String = ",") As String
            Return tree.Reverse.Select(Function(x) x.name).JoinBy(delimiter)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function RankTable(tree As IEnumerable(Of TaxonomyNode)) As Dictionary(Of String, String)
            Return (From x As TaxonomyNode
                    In tree
                    Where Not String.IsNullOrEmpty(x.rank)
                    Select x
                    Group x By x.rank Into Group) _
                         .ToDictionary(Function(x) x.rank,
                                       Function(x)
                                           Return x.Group.First.name
                                       End Function)
        End Function
    End Class
End Namespace
