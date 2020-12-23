Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.EdgeBundling.Mingle
Imports number = System.Double

Module RenderHelpers

    Private Function createPosItem(node As Node, pos As Double(), Index As Integer, total As Double) As PosItem
        Return New PosItem With {
     .node = node,
     .pos = pos,
      .normal = Nothing
    }
    End Function

    ''' <summary>
    ''' Extend generic Graph class with bundle methods And rendering options
    ''' </summary>
    ''' <param name="node"></param>
    Friend Sub expandEdgesHelper(node As Node, Array As List(Of number()), collect As List(Of number()()))
        Dim coords = DirectCast(node.data, MingleNodeData).coords
        Dim ps As Node()

        If Array.IsNullOrEmpty Then
            Array.Add({(coords(0) + coords(2)) / 2,
        (coords(1) + coords(3)) / 2})
        End If

        Array.unshift({coords(0), coords(1)})
        Array.Add({coords(2), coords(3)})
        ps = DirectCast(node.data, MingleNodeData).parents
        If Not ps.IsNullOrEmpty Then
            For i As Integer = 0 To ps.Length - 1
                expandEdgesHelper(ps(i), Array.ToList, collect)
            Next
        Else
            collect.Add(Array.ToArray)
        End If
    End Sub

    ''' <summary>
    ''' Extend generic Graph class with bundle methods And rendering options
    ''' </summary>
    ''' <param name="node"></param>
    ''' <param name="Array"></param>
    ''' <param name="collect"></param>
    Friend Sub expandEdgesRichHelper(node As Node, Array As PosItem(), collect As PosItem()())
        Dim coords = DirectCast(node.data, MingleNodeData).coords
        Dim l As Integer, p As Double()
        Dim ps As Node() = DirectCast(node.data, MingleNodeData).parents
        Dim a As List(Of PosItem)
        Dim posItem As PosItem

        If Not ps.IsNullOrEmpty Then
            l = ps.Length

            For i As Integer = 0 To ps.Length - 1
                a = Array.ToList
                If a.Count <> 0 Then
                    p = {(coords(0) + coords(2)) / 2, (coords(1) + coords(3)) / 2}
                    posItem = createPosItem(node, p, i, l)
                    a.Add(posItem)
                End If

                posItem = createPosItem(node, {coords(0), coords(1)}, i, l)
                a.unshift(posItem)
                posItem = createPosItem(node, {coords(2), coords(3)}, i, l)
                a.Add(posItem)

                expandEdgesRichHelper(ps(i), a.ToArray, collect)
            Next
        Else
            a = Array.ToList
            If a.Count <> 0 Then
                p = {(coords(0) + coords(2)) / 2, (coords(1) + coords(3)) / 2}
                posItem = createPosItem(node, p, 0, 1)
                a.Add(posItem)
            End If

            posItem = createPosItem(node, {coords(0), coords(1)}, 0, 1)
            a.unshift(posItem)
            posItem = createPosItem(node, {coords(2), coords(3)}, 0, 1)
            a.Add(posItem)

            collect.Add(a.ToArray)
        End If
    End Sub

    <Extension>
    Private Sub unshift(Of T)(list As List(Of T), x As T)
        Call list.Insert(Scan0, x)
    End Sub
End Module
