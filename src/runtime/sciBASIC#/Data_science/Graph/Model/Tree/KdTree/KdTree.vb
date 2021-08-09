#Region "Microsoft.VisualBasic::84bf24556f098eb861c49d7edc214b61, Data_science\Graph\Model\Tree\KdTree\KdTree.vb"

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

    '     Class KdTree
    ' 
    '         Properties: balanceFactor
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: buildTree, count, findMax, findMin, height
    '                   innerSearch, insert, nearest, nodeSearch, remove
    ' 
    '         Sub: nearestSearch, removeNode, saveNode
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.Python
Imports stdNum = System.Math

Namespace KdTree

    ''' <summary>
    ''' KDTree is a class supporting KD-tree insertion, deletion, equality search,
    ''' range search, and nearest neighbor(s) using double-precision floating-point
    ''' keys. Splitting dimension is chosen naively, by depth modulo K. Semantics are
    ''' as follows:
    ''' 
    ''' + Two different keys containing identical numbers should retrieve the same
    ''' value from a given KD-tree. Therefore keys are cloned when a node is
    ''' inserted. 
    ''' + As with Hashtables, values inserted into a KD-tree are <I>not</I> cloned.
    ''' Modifying a value between insertion and retrieval will therefore modify the
    ''' value stored in the tree.
    ''' 
    ''' @author Simon Levy, Bjoern Heckel
    ''' @version %I%, %G%
    ''' @since JDK1.2
    ''' </summary>
    Public Class KdTree(Of T As New)

        Dim dimensions As String()
        Dim access As KdNodeAccessor(Of T)
        Dim root As KdTreeNode(Of T)

        Public ReadOnly Property balanceFactor() As Double
            Get
                Return height(root) / (stdNum.Log(count(root)) / stdNum.Log(2))
            End Get
        End Property

        Public ReadOnly Property dimSize As Integer
            Get
                Return dimensions.Length
            End Get
        End Property

        Sub New(points As T(), metric As KdNodeAccessor(Of T))
            Me.access = metric
            Me.dimensions = metric.GetDimensions
            Me.root = buildTree(points, Scan0, Nothing)
        End Sub

        Private Function buildTree(points As T(), depth As Integer, parent As KdTreeNode(Of T)) As KdTreeNode(Of T)
            Dim [dim] = depth Mod dimensions.Length
            Dim median As Integer
            Dim node As KdTreeNode(Of T)

            If points.Length = 0 Then
                Return Nothing
            ElseIf points.Length = 1 Then
                Return New KdTreeNode(Of T)(points(Scan0), [dim], parent)
            Else
                Call points.Sort(Function(a, b) access(a, dimensions([dim])) - access(b, dimensions([dim])))
            End If

            median = stdNum.Floor(points.Length / 2)
            node = New KdTreeNode(Of T)(points(median), [dim], parent)
            node.left = buildTree(points.slice(0, median).ToArray, depth + 1, node)
            node.right = buildTree(points.slice(median + 1).ToArray, depth + 1, node)

            Return node
        End Function

        Public Function insert(point As T) As KdTreeNode(Of T)
            Dim insertPosition = innerSearch(point, root, Nothing),
                newNode As KdTreeNode(Of T),
                dimension As String

            If insertPosition Is Nothing Then
                root = New KdTreeNode(Of T)(point, 0, Nothing)
                Return root
            Else
                newNode = New KdTreeNode(Of T)(point, (insertPosition.dimension + 1) Mod dimensions.Length, insertPosition)
                dimension = dimensions(insertPosition.dimension)
            End If

            If access.getByDimension(point, dimension) < access.getByDimension(insertPosition.obj, dimension) Then
                insertPosition.left = newNode
            Else
                insertPosition.right = newNode
            End If

            Return newNode
        End Function

        Private Function nodeSearch(point As T, node As KdTreeNode(Of T)) As KdTreeNode(Of T)
            If node Is Nothing Then
                Return Nothing
            ElseIf access.nodeIs(node.obj, point) Then
                Return node
            End If

            Dim dimension = dimensions(node.dimension)

            If access.getByDimension(point, dimension) < access.getByDimension(node.obj, dimension) Then
                Return nodeSearch(node.left, node)
            Else
                Return nodeSearch(node.right, node)
            End If
        End Function

        Private Function innerSearch(point As T, node As KdTreeNode(Of T), parent As KdTreeNode(Of T)) As KdTreeNode(Of T)
            If node Is Nothing Then
                Return parent
            End If

            Dim dimension = dimensions(node.dimension)

            If access.getByDimension(point, dimension) < access.getByDimension(node.obj, dimension) Then
                Return innerSearch(point, node.left, node)
            Else
                Return innerSearch(point, node.right, node)
            End If
        End Function

        Public Function remove(point As T) As KdTreeNode(Of T)
            Dim node = nodeSearch(point, root)

            If Not node Is Nothing Then
                Call removeNode(node)
            End If

            Return node
        End Function

        Private Sub removeNode(node As KdTreeNode(Of T))
            Dim nextNode As KdTreeNode(Of T)
            Dim nextObj As T
            Dim pDimension As String

            If node.left Is Nothing AndAlso node.right Is Nothing Then
                If node.parent Is Nothing Then
                    root = Nothing
                    Return
                End If

                pDimension = dimensions(node.parent.dimension)

                If access.getByDimension(node, pDimension) < access.getByDimension(node.parent.obj, pDimension) Then
                    node.parent.left = Nothing
                Else
                    node.parent.right = Nothing
                End If

                Return
            End If

            If Not node.left Is Nothing Then
                nextNode = findMax(node.left, node.dimension)
            Else
                nextNode = findMin(node.right, node.dimension)
            End If

            nextObj = nextNode.obj
            Call removeNode(nextNode)
            node.obj = nextObj
        End Sub

        Private Function findMax(node As KdTreeNode(Of T), [dim] As Integer) As KdTreeNode(Of T)
            Dim dimension As Integer
            Dim own As Double
            Dim Left, Right, max As KdTreeNode(Of T)

            If node Is Nothing Then
                Return Nothing
            Else
                dimension = dimensions([dim])
            End If

            If node.dimension = [dim] Then
                If Not node.right Is Nothing Then
                    Return findMax(node.right, [dim])
                Else
                    Return node
                End If
            End If

            own = access.getByDimension(node.obj, dimension)
            Left = findMax(node.left, [dim])
            Right = findMax(node.right, [dim])
            max = node

            If Not Left Is Nothing AndAlso access.getByDimension(Left.obj, dimension) > own Then
                max = Left
            End If
            If Not Right Is Nothing AndAlso access.getByDimension(Right.obj, dimension) > access.getByDimension(max.obj, dimension) Then
                max = Right
            End If

            Return max
        End Function

        Private Function findMin(node As KdTreeNode(Of T), [dim] As Integer) As KdTreeNode(Of T)
            Dim dimension As Integer
            Dim own As Double
            Dim left, Right, min As KdTreeNode(Of T)

            If node Is Nothing Then
                Return Nothing
            End If

            dimension = dimensions([dim])

            If node.dimension = [dim] Then
                If Not node.left Is Nothing Then
                    Return findMin(node.left, [dim])
                Else
                    Return node
                End If
            End If

            own = access.getByDimension(node.obj, dimension)
            left = findMin(node.left, [dim])
            Right = findMin(node.right, [dim])
            min = node

            If Not left Is Nothing AndAlso access.getByDimension(left.obj, dimension) < own Then
                min = left
            End If

            If Not Right Is Nothing AndAlso access.getByDimension(Right.obj, dimension) < access.getByDimension(min.obj, dimension) Then
                min = Right
            End If

            Return min
        End Function

        ''' <summary>
        ''' Find KD-tree node whose key is identical to key. Uses algorithm
        ''' translated from 352.srch.c of Gonnet &amp; Baeza-Yates.
        ''' </summary>
        ''' <param name="point">
        ''' key for KD-tree node
        ''' </param>
        ''' <param name="maxDistance"></param>
        ''' <param name="maxNodes">
        ''' k
        ''' </param>
        ''' <returns></returns>
        Public Iterator Function nearest(point As T, maxNodes As Integer, Optional maxDistance As Double? = Nothing) As IEnumerable(Of KdNodeHeapItem(Of T))
            Dim bestNodes As New BinaryHeap(Of KdNodeHeapItem(Of T))(Function(e) -e.distance)

            If Not maxDistance Is Nothing Then
                For i As Integer = 0 To maxNodes - 1
                    bestNodes.push(New KdNodeHeapItem(Of T)(Nothing, maxDistance))
                Next
            End If

            Call nearestSearch(point, root, bestNodes, maxNodes)

            For i As Integer = 0 To maxNodes - 1
                If Not bestNodes(i) Is Nothing Then
                    Yield New KdNodeHeapItem(Of T)(bestNodes(i).node, bestNodes(i).distance)
                End If
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="point">
        ''' the user query target point
        ''' </param>
        ''' <param name="parentNode">
        ''' the parent node for search
        ''' </param>
        ''' <param name="bestNodes"></param>
        ''' <param name="maxNodes"></param>
        Private Sub nearestSearch(point As T, parentNode As KdTreeNode(Of T), bestNodes As BinaryHeap(Of KdNodeHeapItem(Of T)), maxNodes%)
            Dim bestChild As KdTreeNode(Of T)
            Dim dimension = dimensions(parentNode.dimension),
               ownDistance = access.metric(point, parentNode.obj),
               linearPoint = access.activate,
               linearDistance As Double,
               otherChild As KdTreeNode(Of T)

            For i As Integer = 0 To dimensions.Length - 1
                If i <> parentNode.dimension Then
                    access(linearPoint, dimensions(i)) = access.getByDimension(point, dimensions(i))
                Else
                    access(linearPoint, dimensions(i)) = access.getByDimension(parentNode.obj, dimensions(i))
                End If
            Next

            linearDistance = access.metric(linearPoint, parentNode.obj)

            If parentNode.right Is Nothing AndAlso parentNode.left Is Nothing Then
                If bestNodes.size < maxNodes OrElse ownDistance < bestNodes.peek().distance Then
                    saveNode(bestNodes, parentNode, ownDistance, maxNodes)
                End If

                Return
            End If

            If parentNode.right Is Nothing Then
                bestChild = parentNode.left
            ElseIf parentNode.left Is Nothing Then
                bestChild = parentNode.right
            Else
                If access.getByDimension(point, dimension) < access.getByDimension(parentNode.obj, dimension) Then
                    bestChild = parentNode.left
                Else
                    bestChild = parentNode.right
                End If
            End If

            Call nearestSearch(point, bestChild, bestNodes, maxNodes)

            If bestNodes.size() < maxNodes OrElse ownDistance < bestNodes.peek.distance Then
                saveNode(bestNodes, parentNode, ownDistance, maxNodes)
            End If

            If bestNodes.size < maxNodes OrElse stdNum.Abs(linearDistance) < bestNodes.peek.distance Then
                If bestChild Is parentNode.left Then
                    otherChild = parentNode.right
                Else
                    otherChild = parentNode.left
                End If

                If Not otherChild Is Nothing Then
                    nearestSearch(point, otherChild, bestNodes, maxNodes)
                End If
            End If
        End Sub

        Private Sub saveNode(bestNodes As BinaryHeap(Of KdNodeHeapItem(Of T)), node As KdTreeNode(Of T), distance#, maxNodes%)
            Call bestNodes.push(New KdNodeHeapItem(Of T)(node, distance))

            If (bestNodes.size > maxNodes) Then
                bestNodes.pop()
            End If
        End Sub

        Private Function height(node As KdTreeNode(Of T)) As Integer
            If node Is Nothing Then
                Return 0
            Else
                Return stdNum.Max(height(node.left), height(node.right)) + 1
            End If
        End Function

        Private Function count(node As KdTreeNode(Of T)) As Integer
            If node Is Nothing Then
                Return 0
            Else
                Return count(node.left) + count(node.right) + 1
            End If
        End Function
    End Class
End Namespace
