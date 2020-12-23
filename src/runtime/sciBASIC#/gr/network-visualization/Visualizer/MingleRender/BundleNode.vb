Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.EdgeBundling.Mingle
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports number = System.Double

Public Class BundleNode

    Public Property node As Node
    Public Property expandedEdges As PosItem()()
    Public Property unbundledEdges As Dictionary(Of String, PosItem()())

    Public Function expandEdges() As PosItem()()
        If Not expandedEdges.IsNullOrEmpty Then
            Return expandedEdges
        End If

        Dim ans As PosItem()() = {}
        expandEdgesRichHelper(node, {}, ans)
        expandedEdges = ans
        Return ans
    End Function

    Public Function unbundleEdges(Optional delta As number = 0) As PosItem()()
        Dim expandedEdges = expandEdges(),
            ans = New PosItem(expandedEdges.Length - 1)() {},
            edge As PosItem(), edgeCopy As PosItem()
        Dim normal, x0 As Vector, xk As Vector, xk_x0 As Vector, xi As Vector,
            xi_x0 As Vector, xi_bar As Vector, dot As Double, norm As Double, norm2 As Double,
            c As Double, last As Integer

        If unbundledEdges Is Nothing Then
            unbundledEdges = New Dictionary(Of String, PosItem()())
        End If

        If ((delta = 0 OrElse delta = 1) AndAlso unbundledEdges.ContainsKey(delta)) Then
            Return unbundledEdges(delta.ToString)
        End If

        Dim l = expandedEdges.Length

        For i As Integer = 0 To expandedEdges.Length - 1
            edge = expandedEdges(i)
            last = edge.Length - 1
            edgeCopy = cloneEdge(edge)
            ' edgeCopy = cloneJSON(edge)
            x0 = edge(0).pos
            xk = edge(last).pos
            xk_x0 = xk - x0

            edgeCopy(0).unbundledPos = edgeCopy(0).pos.ToArray
            normal = edgeCopy(1).pos - edgeCopy(0).pos
            normal = New Vector({-normal(1), normal(0)}).Unit
            edgeCopy(0).normal = normal

            edgeCopy(last).unbundledPos = edgeCopy(edge.Length - 1).pos.ToArray
            normal = edgeCopy(last).pos - edgeCopy(last - 1).pos
            normal = New Vector({-normal(1), normal(0)}).Unit
            edgeCopy(last).normal = normal

            For j As Integer = 1 To edge.Length - 2
                xi = edge(j).pos
                xi_x0 = xi - x0
                dot = xi_x0.DotProduct(xk_x0)
                norm = dist(xk, x0)
                norm2 = norm * norm
                c = dot / norm2
                xi_bar = x0 + (c * xk_x0)
                edgeCopy(j).unbundledPos = lerp(xi_bar, xi, delta)
                normal = edgeCopy(j + 1).pos - edgeCopy(j - 1).pos
                normal = New Vector({-normal(1), normal(0)}).Unit
                edgeCopy(j).normal = normal
            Next
            ans(i) = edgeCopy
        Next

        If (delta = 0 OrElse delta = 1) Then
            unbundledEdges(delta.ToString) = ans
        End If

        Return ans
    End Function
End Class
