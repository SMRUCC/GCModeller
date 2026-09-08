Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' 单层生物学实体定义（通路层 / 基因层）
''' </summary>
''' <remarks>
''' 在 P-NET 中，每一个网络层都对应着一个明确的生物学层级：基因层或者某一抽象级别的通路层。
''' 层内的每一个节点都对应着一个具体的生物学实体（基因或者通路），节点之间的连接关系则由
''' <see cref="Members"/> 所描述的父子包含关系所决定。
''' </remarks>
Public Class HierarchyLevel

    ''' <summary>
    ''' 当前层级的名称，例如 ``L2_fine_pathways``，用于展示与调试
    ''' </summary>
    ''' <returns>层名称字符串</returns>
    Public Property Name As String

    ''' <summary>
    ''' 当前层内的节点（基因或者通路）的名称列表
    ''' </summary>
    ''' <returns>节点名称数组，数组长度即为当前层的节点数</returns>
    Public Property Nodes As String()

    ''' <summary>
    ''' 每一个节点在下一层（更精细的层，或者是基因层）中所包含的子节点的索引
    ''' </summary>
    ''' <returns>
    ''' 一个锯齿数组，``Members(i)`` 为第 ``i`` 个节点所包含的所有子节点在下一层中的索引集合。
    ''' 若下一层为基因层，则子节点索引即基因索引；否则为下一层通路节点的索引。
    ''' </returns>
    Public Property Members As Integer()()

    ''' <summary>
    ''' 当前层的节点数量
    ''' </summary>
    ''' <returns>节点数</returns>
    Public ReadOnly Property Count As Integer
        Get
            If Nodes Is Nothing Then
                Return 0
            End If

            Return Nodes.Length
        End Get
    End Property

    ''' <summary>
    ''' 当前层中所有父子连接边的数量（即掩码矩阵中非零元素的数量）
    ''' </summary>
    ''' <returns>连接边数量</returns>
    Public ReadOnly Property EdgeCount As Integer
        Get
            If Members Is Nothing Then
                Return 0
            End If

            Dim n As Integer = 0

            For i As Integer = 0 To Members.Length - 1
                If Members(i) IsNot Nothing Then
                    n += Members(i).Length
                End If
            Next

            Return n
        End Get
    End Property

    ''' <summary>
    ''' 创建一个空的层级对象
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 使用给定的名称、节点名以及成员关系创建层级对象
    ''' </summary>
    ''' <param name="name">层级名称</param>
    ''' <param name="nodes">节点名称数组</param>
    ''' <param name="members">每一个节点所包含的子节点索引</param>
    Public Sub New(name As String, nodes As String(), members As Integer()())
        Me.Name = name
        Me.Nodes = nodes
        Me.Members = members
    End Sub

    ''' <summary>
    ''' 获取指定节点在下一层中所包含的子节点索引
    ''' </summary>
    ''' <param name="nodeIndex">当前层内的节点索引</param>
    ''' <returns>子节点索引数组，若不存在则返回空数组</returns>
    Public Function GetMembers(nodeIndex As Integer) As Integer()
        If Members Is Nothing OrElse nodeIndex < 0 OrElse nodeIndex >= Members.Length Then
            Return New Integer(-1) {}
        End If
        If Members(nodeIndex) Is Nothing Then
            Return New Integer(-1) {}
        End If

        Return Members(nodeIndex)
    End Function

    ''' <summary>
    ''' 生成当前层级的节点名称的字符串描述
    ''' </summary>
    ''' <returns>形如 ``L2[16 nodes / 64 edges]`` 的描述文本</returns>
    Public Overrides Function ToString() As String
        Return $"{Name}[{Count} nodes / {EdgeCount} edges]"
    End Function

End Class
