Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.GraphTheory.KdTree
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Imaging.LayoutModel
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Class DrawKDTree : Inherits Plot

    ReadOnly tree As KdTree(Of Point2D)
    ReadOnly query As NamedValue(Of PointF)()
    ReadOnly k As Integer

    Public Sub New(tree As KdTree(Of Point2D), query As NamedValue(Of PointF)(), k As Integer, theme As Theme)
        MyBase.New(theme)

        Me.tree = tree
        Me.query = query
        Me.k = k
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim allPoints As Point2D() = tree.GetPoints.ToArray
        Dim xTicks As Double() = allPoints.Select(Function(p) p.X).CreateAxisTicks
        Dim yTicks As Double() = allPoints.Select(Function(p) p.Y).CreateAxisTicks
        Dim rect As Rectangle = canvas.PlotRegion
        Dim xscale = d3js.scale.linear.domain(xTicks).range(New Double() {rect.Left, rect.Right})
        Dim yscale = d3js.scale.linear.domain(yTicks).range(New Double() {rect.Top, rect.Bottom})
        Dim scaler As New DataScaler() With {
            .AxisTicks = (xTicks.AsVector, yTicks.AsVector),
            .region = rect,
            .X = xscale,
            .Y = yscale
        }

        Call render(g, scaler, root:=tree.rootNode)

        If Not query.IsNullOrEmpty Then
            For Each q As NamedValue(Of PointF) In query
                Dim pos As PointF = scaler.Translate(q.Value.X, q.Value.Y)
                Dim color As Pen = New Pen(q.Description.TranslateColor, 2)

                Call g.DrawCircle(pos, theme.pointSize, color, fill:=True)

                For Each knn In tree.nearest(New Point2D(q.Value), k)
                    pos = knn.node.data
                    pos = scaler.Translate(pos.X, pos.Y)

                    Call g.DrawCircle(pos, theme.pointSize, color, fill:=False)
                Next
            Next
        End If
    End Sub

    Private Sub render(g As IGraphics, scaler As DataScaler, root As KdTreeNode(Of Point2D))
        Dim pos As PointF, pos2 As PointF

        pos = root.data.PointF
        pos = scaler.Translate(pos.X, pos.Y)

        Call g.DrawCircle(pos, theme.pointSize, Pens.LightGray, fill:=True)

        If Not root.left Is Nothing Then
            pos2 = root.left.data.PointF
            pos2 = scaler.Translate(pos2.X, pos2.Y)

            Call Console.Write("->(")
            Call g.DrawLine(Pens.Black, pos, pos2)
            Call render(g, scaler, root.left)
            Call Console.Write(")")
        End If
        If Not root.right Is Nothing Then
            pos2 = root.right.data.PointF
            pos2 = scaler.Translate(pos2.X, pos2.Y)

            Call Console.Write("<-(")
            Call g.DrawLine(Pens.Black, pos, pos2)
            Call render(g, scaler, root.right)
            Call Console.Write(")")
        End If
    End Sub

    Public Overloads Shared Function Plot(tree As KdTree(Of Point2D),
                                          Optional query As NamedValue(Of PointF)() = Nothing,
                                          Optional k As Integer = 13,
                                          Optional size As String = "3600,2700",
                                          Optional padding As String = g.DefaultPadding,
                                          Optional bg$ = "white",
                                          Optional pointSize As Integer = 5) As GraphicsData

        Dim theme As New Theme With {
            .padding = padding,
            .background = bg,
            .pointSize = pointSize
        }

        Return New DrawKDTree(tree, query, k, theme).Plot(size)
    End Function

End Class
