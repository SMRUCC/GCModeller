' ============================================================================
' PhyloTreeNode.vb - 系统发育树节点
'
' 对应论文模块2：系统发育与祖先状态重建
' 用于表示测序生命树(sequenced Tree of Life, sTOL)的结构
' ============================================================================

Namespace TraitarVB.Models

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
