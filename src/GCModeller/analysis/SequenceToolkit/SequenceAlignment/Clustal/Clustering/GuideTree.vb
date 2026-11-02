Imports System.Text

Namespace Clustal

    ''' <summary>
    ''' 引导树的一个节点：叶子节点承载序列索引，内部节点承载合并高度(UPGMA 距离)。
    ''' </summary>
    Public Class GuideTreeNode

        ''' <summary>
        ''' 叶子节点对应的序列索引，内部节点为 -1
        ''' </summary>
        Public Property Index As Integer = -1
        ''' <summary>
        ''' 节点名称(叶子为序列名，内部节点为空)
        ''' </summary>
        Public Property Label As String
        Public Property Left As GuideTreeNode
        Public Property Right As GuideTreeNode
        ''' <summary>
        ''' 合并高度/分支长度
        ''' </summary>
        Public Property Height As Double
        ''' <summary>
        ''' 该子树包含的叶子数量
        ''' </summary>
        Public Property LeafCount As Integer = 1

        ''' <summary>
        ''' 是否为叶子节点
        ''' </summary>
        Public ReadOnly Property IsLeaf As Boolean
            Get
                Return Left Is Nothing AndAlso Right Is Nothing
            End Get
        End Property

        Public Overrides Function ToString() As String
            If IsLeaf Then
                Return $"leaf({Index})"
            Else
                Return $"node(h={Math.Round(Height, 4)}, leaves={LeafCount})"
            End If
        End Function
    End Class

    ''' <summary>
    ''' 引导树：由 UPGMA 构造的二叉树，提供后序遍历与 Newick 序列化能力。
    ''' </summary>
    Public Class GuideTree

        ''' <summary>
        ''' 根节点
        ''' </summary>
        Public ReadOnly Property Root As GuideTreeNode
        ''' <summary>
        ''' 叶子总数(= 序列条数)
        ''' </summary>
        Public ReadOnly Property LeafCount As Integer

        Sub New(root As GuideTreeNode, Optional leafCount As Integer = -1)
            Me.Root = root

            If leafCount >= 0 Then
                Me.LeafCount = leafCount
            Else
                Me.LeafCount = If(root Is Nothing, 0, root.LeafCount)
            End If
        End Sub

        ''' <summary>
        ''' 叶子索引(按树的中序顺序)
        ''' </summary>
        Public Function LeafOrder() As Integer()
            Dim order As New List(Of Integer)(LeafCount)

            If Root Is Nothing Then
                Return order.ToArray()
            End If

            Dim stack As New Stack(Of GuideTreeNode)()
            Call stack.Push(Root)

            While stack.Count > 0
                Dim node As GuideTreeNode = stack.Pop()

                If node.IsLeaf Then
                    Call order.Add(node.Index)
                Else
                    ' 先压右再压左，弹出时即为中序
                    If node.Right IsNot Nothing Then
                        Call stack.Push(node.Right)
                    End If
                    If node.Left IsNot Nothing Then
                        Call stack.Push(node.Left)
                    End If
                End If
            End While

            Return order.ToArray()
        End Function

        ''' <summary>
        ''' 后序遍历(先子后父)，用于自底向上的渐进合并
        ''' </summary>
        Public Iterator Function PostOrder() As IEnumerable(Of GuideTreeNode)
            If Root Is Nothing Then
                Return
            End If

            Dim stack As New Stack(Of (node As GuideTreeNode, visited As Boolean))()
            Call stack.Push((Root, False))

            While stack.Count > 0
                Dim item = stack.Pop()

                If item.node.IsLeaf Then
                    Yield item.node
                ElseIf item.visited Then
                    Yield item.node
                Else
                    Call stack.Push((item.node, True))

                    If item.node.Right IsNot Nothing Then
                        Call stack.Push((item.node.Right, False))
                    End If
                    If item.node.Left IsNot Nothing Then
                        Call stack.Push((item.node.Left, False))
                    End If
                End If
            End While
        End Function

        ''' <summary>
        ''' 取某个节点下面的全部叶子索引
        ''' </summary>
        Public Shared Function LeavesOf(node As GuideTreeNode) As Integer()
            Dim result As New List(Of Integer)()
            Dim stack As New Stack(Of GuideTreeNode)()

            If node IsNot Nothing Then
                Call stack.Push(node)
            End If

            While stack.Count > 0
                Dim current As GuideTreeNode = stack.Pop()

                If current.IsLeaf Then
                    Call result.Add(current.Index)
                Else
                    If current.Left IsNot Nothing Then
                        Call stack.Push(current.Left)
                    End If
                    If current.Right IsNot Nothing Then
                        Call stack.Push(current.Right)
                    End If
                End If
            End While

            Return result.ToArray()
        End Function

        ''' <summary>
        ''' 输出 Newick 格式的树文本
        ''' </summary>
        ''' <param name="names">叶子名称，为空时使用索引编号</param>
        Public Function ToNewick(Optional names As String() = Nothing) As String
            Dim sb As New StringBuilder()

            If Root Is Nothing Then
                Return ";"
            End If

            Call WriteNode(sb, Root, names, Root.Height)
            Call sb.Append(";")

            Return sb.ToString()
        End Function

        Private Shared Sub WriteNode(sb As StringBuilder, node As GuideTreeNode, names As String(), parentHeight As Double)
            If node.IsLeaf Then
                Dim label As String = $"seq_{node.Index + 1}"

                If names IsNot Nothing AndAlso node.Index >= 0 AndAlso node.Index < names.Length Then
                    Dim name As String = names(node.Index)

                    If Not String.IsNullOrEmpty(name) Then
                        label = name
                    End If
                End If

                Call sb.Append(Sanitize(label))
                Call sb.Append(":")
                Call sb.Append(FormatLength(parentHeight - node.Height))

                Return
            End If

            Call sb.Append("(")
            Call WriteNode(sb, node.Left, names, node.Height)
            Call sb.Append(",")
            Call WriteNode(sb, node.Right, names, node.Height)
            Call sb.Append("):")
            Call sb.Append(FormatLength(parentHeight - node.Height))
        End Sub

        Private Shared Function FormatLength(value As Double) As String
            If value < 0 Then
                value = 0
            End If

            Return value.ToString("0.00000")
        End Function

        Private Shared Function Sanitize(name As String) As String
            Dim sb As New StringBuilder(name.Length)

            For Each ch As Char In name
                Select Case ch
                    Case " "c, ":"c, "("c, ")"c, ","c, ";"c, "["c, "]"c, "'"c
                        Call sb.Append("_"c)
                    Case Else
                        Call sb.Append(ch)
                End Select
            Next

            Return sb.ToString()
        End Function

        Public Overrides Function ToString() As String
            Return $"GuideTree(leaves={LeafCount})"
        End Function
    End Class
End Namespace
