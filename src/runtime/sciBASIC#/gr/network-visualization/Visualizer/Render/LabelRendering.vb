﻿Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.EdgeBundling
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.d3js.Layout
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.ConcaveHull
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Text
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports stdNum = System.Math

''' <summary>
''' 使用退火算法计算出节点标签文本的位置
''' </summary>
''' 
Friend Class LabelRendering

    ReadOnly labelColorAsNodeColor As Boolean,
        iteration As Integer,
        showLabelerProgress As Boolean,
        defaultLabelColorValue As String,
        labelTextStrokeCSS As String,
        getLabelColor As Func(Of Node, Color)

    Sub New(labelColorAsNodeColor As Boolean,
        iteration As Integer,
        showLabelerProgress As Boolean,
        defaultLabelColorValue As String,
        labelTextStrokeCSS As String,
        getLabelColor As Func(Of Node, Color))

        Me.labelColorAsNodeColor = labelColorAsNodeColor
        Me.iteration = iteration
        Me.showLabelerProgress = showLabelerProgress
        Me.defaultLabelColorValue = defaultLabelColorValue
        Me.labelTextStrokeCSS = labelTextStrokeCSS
        Me.getLabelColor = getLabelColor
    End Sub

    Public Sub renderLabels(g As IGraphics, labelList As IEnumerable(Of LayoutLabel))
        Dim labels As New List(Of LayoutLabel)(labelList)
        Dim defaultLabelColor As New SolidBrush(defaultLabelColorValue.TranslateColor)
        Dim labelTextStroke As Pen = Stroke.TryParse(labelTextStrokeCSS)

        ' 小于等于零的时候表示不进行布局计算
        If iteration > 0 Then
            Call $"Do node label layouts, iteration={iteration}".__INFO_ECHO
            Call d3js _
                .labeler(maxMove:=1, maxAngle:=1, w_len:=1, w_inter:=2, w_lab2:=10, w_lab_anc:=10, w_orient:=2) _
                .Anchors(labels.Select(Function(x) x.anchor)) _
                .Labels(labels.Select(Function(x) x.label)) _
                .Size(g.Size) _
                .Start(nsweeps:=iteration, showProgress:=showLabelerProgress)
        End If

        For Each label As LayoutLabel In labels.Where(Function(a) Not a.color Is Nothing)
            Call renderLabel(label, g, defaultLabelColor, labelTextStroke)
        Next
    End Sub

    Private Sub renderLabel(label As LayoutLabel, g As IGraphics, defaultLabelColor As SolidBrush, labelTextStroke As Pen)
        Dim br As Brush
        Dim rect As Rectangle
        Dim lx, ly As Single
        Dim color As Color
        Dim frameSize = g.Size

        With label
            If Not labelColorAsNodeColor Then
                color = getLabelColor(label.node)

                If color.IsEmpty Then
                    br = defaultLabelColor
                Else
                    br = New SolidBrush(color)
                End If
            Else
                br = .color
                br = New SolidBrush(DirectCast(br, SolidBrush).Color.Darken(0.005))
            End If

            lx = .label.X
            ly = .label.Y

            If iteration > 0 Then
                If label.offsetDistance >= stdNum.Max(g.Size.Width, g.Size.Height) * 0.01 Then
                    Call g.DrawLine(New Pen(Brushes.Gray, 3) With {.DashStyle = DashStyle.Dot}, label.anchor, label.GetTextAnchor)
                End If
            End If

            With g.MeasureString(.label.text, .style)
                If lx < 0 Then
                    lx = 1
                ElseIf lx + .Width > frameSize.Width Then
                    lx -= (lx + .Width - frameSize.Width) + 5
                End If

                If ly < 0 Then
                    ly = 1
                ElseIf ly + .Height > frameSize.Height Then
                    ly -= (ly + .Height - frameSize.Height) + 5
                End If

                rect = New Rectangle(lx, ly, .Width, .Height)
            End With

            Dim path As GraphicsPath = Imaging.GetStringPath(
                .label.text,
                g.DpiX,
                rect.OffSet2D(.style.Size / 5, 0).ToFloat,
                .style,
                StringFormat.GenericTypographic
            )

            If Not labelTextStroke Is Nothing Then
                ' 绘制轮廓（描边）
                Call g.DrawString(.label.text, .style, br, lx, ly)
                Call g.DrawPath(labelTextStroke, path)
            Else
                Call WordWrap.DrawTextCentraAlign(g, .label, New PointF(lx, ly), br, .style)
            End If
        End With
    End Sub
End Class