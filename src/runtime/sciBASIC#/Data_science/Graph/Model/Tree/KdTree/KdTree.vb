#Region "Microsoft.VisualBasic::ad89000b9f382eec97cff3a64a305bc5, Data_science\Graph\Model\Tree\KdTree\KdTree.vb"

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
'         Constructor: (+1 Overloads) Sub New
'         Function: buildTree
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language.Python
Imports Microsoft.VisualBasic.Language

Namespace KdTree

    Public Class KdTree

        Dim dimensions As Integer()
        Dim points As Object()
        Dim metric As Object

        Dim root As Node

        Sub New(points As Object(), metric As Object, dimensions As Integer())
            Me.points = points
            Me.metric = metric
            Me.dimensions = dimensions
            Me.root = buildTree(points, Scan0, Nothing)
        End Sub

        Private Function buildTree(points As Object(), depth As Integer, parent As Node) As Node
            Dim [dim] = depth Mod dimensions.Length
            Dim median As Integer
            Dim node As Node

            If points.Length = 0 Then
                Return Nothing
            ElseIf points.Length = 1 Then
                Return New Node(points(Scan0), [dim], parent)
            End If

            points.Sort(Function(a, b) As Integer
                            Return a(dimensions([dim])) - b(dimensions([dim]))
                        End Function)

            median = Math.Floor(points.Length / 2)
            node = New Node(points(median), [dim], parent)
            node.left = buildTree(points.slice(0, median), depth + 1, node)
            node.right = buildTree(points.slice(median + 1), depth + 1, node)

            Return node
        End Function

        Public Function insert(point) As Node
            Dim insertPosition = innerSearch(point, root, Nothing),
                newNode As Node,
                dimension As Object

            If insertPosition Is Nothing Then
                root = New Node(point, 0, Nothing)
                Return root
            Else
                newNode = New Node(point, (insertPosition.dimension + 1) Mod dimensions.Length, insertPosition)
                dimension = dimensions(insertPosition.dimension)
            End If

            If point(dimension) < insertPosition.obj(dimension) Then
                insertPosition.left = newNode
            Else
                insertPosition.right = newNode
            End If

            Return newNode
        End Function

        Private Function nodeSearch(point As Object, node As Node) As Node
            If node Is Nothing Then
                Return Nothing
            ElseIf node.obj Is point Then
                Return node
            End If

            Dim dimension = dimensions(node.dimension)

            If point(dimension) < node.obj(dimension) Then
                Return nodeSearch(node.left, node)
            Else
                Return nodeSearch(node.right, node)
            End If
        End Function

        Private Function innerSearch(point As Object, node As Node, parent As Node)
            If node Is Nothing Then
                Return parent
            End If

            Dim dimension = dimensions(node.dimension)

            If point(dimension) < node.obj(dimension) Then
                Return innerSearch(point, node.left, node)
            Else
                Return innerSearch(point, node.right, node)
            End If
        End Function

        Public Function remove(point) As Node
            Dim node = nodeSearch(point, root)

            If Not node Is Nothing Then
                Call removeNode(node)
            End If

            Return node
        End Function

        Private Sub removeNode(node As Node)
            Dim nextNode As Node
            Dim nextObj
            Dim pDimension

            If node.left Is Nothing AndAlso node.right Is Nothing Then
                If node.parent Is Nothing Then
                    root = Nothing
                    Return
                End If

                pDimension = dimensions(node.parent.dimension)

                If node.obj(pDimension) < node.parent.obj(pDimension) Then
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
            removeNode(nextNode)
            node.obj = nextObj
        End Sub

        Private Function findMax(node As Node, [dim] As Integer)
            Dim dimension As Integer
            Dim own
            Dim Left, Right, max As Node

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

            own = node.obj(dimension)
            Left = findMax(node.left, [dim])
            Right = findMax(node.right, [dim])
            max = node

            If Not Left Is Nothing AndAlso Left.obj(dimension) > own Then
                max = Left
            End If
            If Not Right Is Nothing AndAlso Right.obj(dimension) > max.obj(dimension) Then
                max = Right
            End If

            Return max
        End Function

        Private Function findMin(node As Node, [dim] As Integer)
            Dim dimension As Integer
            Dim own
            Dim Left,
            Right,
            min As Node

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

            own = node.obj(dimension)
            Left = findMin(node.left, [dim])
            Right = findMin(node.right, [dim])
            min = node

            If Not Left Is Nothing AndAlso Left.obj(dimension) < own Then
                min = Left
            End If

            If Not Right Is Nothing AndAlso Right.obj(dimension) < min.obj(dimension) Then
                min = Right
            End If

            Return min
        End Function

        Public Function nearest(point As Object, maxNodes As Integer, maxDistance As Double)
            Dim i, result, bestNodes

        End Function
    End Class

    Public Class BinaryHeap

        Public content As New List(Of Object)
        Public scoreFunction As Func(Of Object, Double)

        Public ReadOnly Property size As Integer
            Get
                Return content.Count
            End Get
        End Property

        Sub New(scoreFunction As Func(Of Object, Double))
            Me.scoreFunction = scoreFunction
        End Sub

        Public Sub push(element)
            content.Add(element)
            bubbleUp(content.Count - 1)
        End Sub

        Public Function pop()
            Dim result = content(Scan0)
            Dim [end] = content.Pop

            If content > 0 Then
                content(Scan0) = [end]
                sinkdown(Scan0)
            End If

            Return result
        End Function

        Public Function peek() As Object
            Return content(Scan0)
        End Function

        Public Sub remove(node)
            Dim len = content.Count

            For i As Integer = 0 To len - 1
                If content(i) Is node Then
                    Dim [end] = content.Pop

                    If i <> len - 1 Then
                        content(i) = [end]

                        If scoreFunction([end]) < scoreFunction(node) Then
                            bubbleUp(i)
                        Else
                            sinkDown(i)
                        End If
                    End If

                    Return
                End If
            Next

            Throw New Exception("Node not found.")
        End Sub

        Private Sub bubbleUp(n As Integer)
            Dim element = content(n)

            Do While n > 0
                Dim parentN = Math.Floor((n + 1) / 2) - 1,
            parent = content(parentN)

                If scoreFunction(element) < scoreFunction(parent) Then
                    content(parentN) = element
                    content(n) = parent
                    n = parentN
                Else
                    Exit Do
                End If
            Loop
        End Sub

        Private Sub sinkDown(n As Integer)
            Dim length = content.Count,
           element = content(n),
           elemScore = scoreFunction(element)
            Dim child1Score As Double

            Do While True
                Dim child2N = (n + 1) * 2, child1N = child2N - 1
                Dim swap = Nothing

                If (child1N < length) Then
                    ' Look it up And compute its score.
                    Dim child1 = content(child1N)
                    child1Score = scoreFunction(child1)
                    ' If the score Is less than our element's, we need to swap.
                    If child1Score < elemScore Then
                        swap = child1N
                    End If
                End If

                If child2N < length Then
                    Dim child2 = content(child2N),
              child2Score = scoreFunction(child2)
                    If (child2Score < If(swap Is Nothing, elemScore, child1Score)) Then
                        swap = child2N
                    End If
                End If

                If Not swap Is Nothing Then
                    content(n) = content(swap)
                    content(swap) = element
                    n = swap
                Else
                    Exit Do
                End If
            Loop
        End Sub
    End Class
End Namespace
