#Region "Microsoft.VisualBasic::e79e519baa6d36f3861390d4a954e875, analysis\Metagenome\MetaFunction\metaTraits\Traitar\Models\PhyloTreeNode.vb"

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

    '   Total Lines: 121
    '    Code Lines: 61 (50.41%)
    ' Comment Lines: 39 (32.23%)
    '    - Xml Docs: 84.62%
    ' 
    '   Blank Lines: 21 (17.36%)
    '     File Size: 4.43 KB


    '     Class PhyloTreeNode
    ' 
    '         Properties: BranchLength, Children, IsLeaf, Name, Parent
    '                     PfamGainProb, PfamLossProb, PfamPresenceProb, PhenotypeGainProb, PhenotypeLossProb
    '                     PhenotypePresenceProb
    ' 
    '         Function: Clone, GetAllLeaves, GetAllNodes, ToString
    ' 
    '         Sub: CollectLeaves, CollectNodes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' PhyloTreeNode.vb - 系统发育树节点
'
' 对应论文模块2：系统发育与祖先状态重建
' 用于表示测序生命树(sequenced Tree of Life, sTOL)的结构
' ============================================================================

Namespace metaTraits.Traitar.Models

    ''' <summary>
    ''' 系统发育树节点
    ''' </summary>
    Public Class PhyloTreeNode

        ''' <summary>节点名称（叶节点为物种名，内部节点为祖先名）</summary>
        Public Property Name As String

        ''' <summary>分支长度（到父节点的距离）</summary>
        Public Property BranchLength As Double

        ''' <summary>父节点</summary>
        Public Property Parent As PhyloTreeNode

        ''' <summary>子节点列表</summary>
        Public Property Children As New List(Of PhyloTreeNode)()

        ''' <summary>是否为叶节点</summary>
        Public ReadOnly Property IsLeaf As Boolean
            Get
                Return Children.Count = 0
            End Get
        End Property

        ''' <summary>
        ''' 祖先状态：蛋白质家族在该节点的存在概率
        ''' 由GLOOME最大似然法推断
        ''' </summary>
        Public Property PfamPresenceProb As New Dictionary(Of String, Double)()

        ''' <summary>
        ''' 祖先状态：表型在该节点的存在概率
        ''' </summary>
        Public Property PhenotypePresenceProb As New Dictionary(Of String, Double)()

        ''' <summary>
        ''' 该分支上蛋白质家族的获得概率
        ''' </summary>
        Public Property PfamGainProb As New Dictionary(Of String, Double)()

        ''' <summary>
        ''' 该分支上蛋白质家族的丢失概率
        ''' </summary>
        Public Property PfamLossProb As New Dictionary(Of String, Double)()

        ''' <summary>
        ''' 该分支上表型的获得概率
        ''' </summary>
        Public Property PhenotypeGainProb As New Dictionary(Of String, Double)()

        ''' <summary>
        ''' 该分支上表型的丢失概率
        ''' </summary>
        Public Property PhenotypeLossProb As New Dictionary(Of String, Double)()

        Public Function Clone() As PhyloTreeNode
            Return New PhyloTreeNode With {
                .Name = Name,
                .BranchLength = BranchLength,
                .Parent = Parent,
                .Children = New List(Of PhyloTreeNode)(Children),
                .PfamGainProb = New Dictionary(Of String, Double)(PfamGainProb),
                .PfamLossProb = New Dictionary(Of String, Double)(PfamLossProb),
                .PfamPresenceProb = New Dictionary(Of String, Double)(PfamPresenceProb),
                .PhenotypeGainProb = New Dictionary(Of String, Double)(PhenotypeGainProb),
                .PhenotypeLossProb = New Dictionary(Of String, Double)(PhenotypeLossProb),
                .PhenotypePresenceProb = New Dictionary(Of String, Double)(PhenotypePresenceProb)
            }
        End Function

        ''' <summary>
        ''' 获取所有叶节点
        ''' </summary>
        Public Function GetAllLeaves() As List(Of PhyloTreeNode)
            Dim leaves As New List(Of PhyloTreeNode)()
            CollectLeaves(leaves)
            Return leaves
        End Function

        Private Sub CollectLeaves(leaves As List(Of PhyloTreeNode))
            If IsLeaf Then
                leaves.Add(Me)
            Else
                For Each child As PhyloTreeNode In Children
                    child.CollectLeaves(leaves)
                Next
            End If
        End Sub

        ''' <summary>
        ''' 获取所有节点（包括内部节点）
        ''' </summary>
        Public Function GetAllNodes() As List(Of PhyloTreeNode)
            Dim nodes As New List(Of PhyloTreeNode)()
            CollectNodes(nodes)
            Return nodes
        End Function

        Private Sub CollectNodes(nodes As List(Of PhyloTreeNode))
            nodes.Add(Me)
            For Each child As PhyloTreeNode In Children
                child.CollectNodes(nodes)
            Next
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} (bl={1:F4}, children={2})", Name, BranchLength, Children.Count)
        End Function

    End Class

End Namespace

