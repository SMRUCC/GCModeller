#Region "Microsoft.VisualBasic::e724834206f0cd5fa6958a418e76e075, ..\visualize\GCModeller.DataVisualization\Volcano.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting

''' <summary>
''' 用来可视化差异表达基因
''' </summary>
Public Module Volcano

    ReadOnly DEG_diff# = Math.Log(2, 2)
    ReadOnly diffPValue# = -Math.Log10(0.05)

    Const UP$ = "Up"
    Const DOWN$ = "Down"

    <Extension>
    Public Function PlotDEGs(genes As IEnumerable(Of EntityObject),
                             Optional size As Size = Nothing,
                             Optional padding$ = g.DefaultPadding,
                             Optional bg$ = "white",
                             Optional logFC$ = "logFC",
                             Optional pvalue$ = "P.value",
                             Optional displayLabel As LabelTypes = LabelTypes.None,
                             Optional labelFontStyle$ = CSSFont.PlotTitle,
                             Optional ylayout As YAxisLayoutStyles = YAxisLayoutStyles.ZERO) As GraphicsData

        Return genes.PlotDEGs(
            x:=Function(gene) gene(logFC).ParseNumeric,
            y:=Function(gene) gene(pvalue).ParseNumeric,
            label:=Function(gene) gene.ID,
            size:=size,
            padding:=padding,
            bg:=bg,
            displayLabel:=displayLabel,
            labelFontStyle:=labelFontStyle,
            ylayout:=ylayout)
    End Function

    <Extension>
    Public Function PlotDEGs(Of T)(genes As IEnumerable(Of T),
                                   x As Func(Of T, Double),
                                   y As Func(Of T, Double),
                                   label As Func(Of T, String),
                                   Optional size As Size = Nothing,
                                   Optional padding$ = g.DefaultPadding,
                                   Optional bg$ = "white",
                                   Optional displayLabel As LabelTypes = LabelTypes.None,
                                   Optional labelFontStyle$ = CSSFont.Win10Normal,
                                   Optional ylayout As YAxisLayoutStyles = YAxisLayoutStyles.ZERO) As GraphicsData

        Dim factor As Func(Of DEGModel, Integer) =
            Function(DEG)
                If DEG.pvalue < diffPValue Then
                    Return 0
                End If

                If DEG.logFC >= DEG_diff Then
                    Return 1
                Else
                    Return -1
                End If
            End Function
        Dim colors As New Dictionary(Of Integer, Color) From {
            {1, Color.Red},    ' 上调
            {-1, Color.Lime}, ' 下调
            {0, Color.Gray}    ' 没有变化
        }
        Return genes.Select(
            Function(g) New DEGModel With {
                .label = label(g),
                .logFC = x(g),
                .pvalue = y(g)
        }).Plot(factor, colors,
                size, padding, bg,
                displayLabel:=displayLabel,
                labelFontStyle:=labelFontStyle,
                axisLayout:=ylayout)
    End Function

    ReadOnly black As Brush = Brushes.Black

    ''' <summary>
    ''' 绘制差异表达基因的火山图
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function Plot(genes As IEnumerable(Of DEGModel), factors As Func(Of DEGModel, Integer), colors As Dictionary(Of Integer, Color),
                         Optional size As Size = Nothing,
                         Optional padding$ = g.DefaultPadding,
                         Optional bg$ = "white",
                         Optional xlab$ = "log<sub>2</sub>(Fold Change)",
                         Optional ylab$ = "-log<sub>10</sub>(p-value)",
                         Optional ptSize! = 5,
                         Optional translate As Func(Of Double, Double) = Nothing,
                         Optional displayLabel As LabelTypes = LabelTypes.None,
                         Optional labelFontStyle$ = CSSFont.PlotTitle,
                         Optional legendFont$ = CSSFont.UbuntuNormal,
                         Optional axisLayout As YAxisLayoutStyles = YAxisLayoutStyles.ZERO) As GraphicsData

        If translate Is Nothing Then
            translate = Function(pvalue) -Math.Log10(pvalue)
        End If

        Dim DEG_matrix As DEGModel() = genes.ToArray(
            Function(g) New DEGModel With {
                .label = g.label,
                .logFC = g.logFC,
                .pvalue = translate(g.pvalue)
            })
        Dim scaler As New Mapper(New Scaling(DEG_matrix.ToArray(Function(x) (x.logFC, x.pvalue))))
        Dim brushes As Dictionary(Of Integer, Brush) = colors _
            .ToDictionary(Function(k) k.Key,
                          Function(br) DirectCast(New SolidBrush(br.Value), Brush))

        If size.IsEmpty Then
            size = New Size(2000, 1850)
        End If

        Return g.Allocate(size, padding, bg) <=
 _
            Sub(ByRef g As IGraphics, region As GraphicsRegion)

                Dim scalling = scaler.TupleScaler(region)
                Dim labelFont As Font = CSSFont.TryParse(labelFontStyle)
                Dim lbSize As SizeF
                Dim gdi As IGraphics = g
                Dim __drawLabel = Sub(label$, point As PointF)
                                      lbSize = gdi.MeasureString(label, labelFont)
                                      Call gdi.DrawString(label, labelFont, black, New PointF(point.X - lbSize.Width / 2, point.Y + ptSize))
                                  End Sub

                Call Axis.DrawAxis(g, region, scaler, True, xlabel:=xlab, ylabel:=ylab, ylayout:=axisLayout)

                For Each gene As DEGModel In DEG_matrix
                    Dim factor As Integer = factors(gene)
                    Dim color As Brush = brushes(factor)
                    Dim point As PointF = scalling((gene.logFC, gene.pvalue))

                    Call g.DrawCircle(point, ptSize, color)

                    Select Case displayLabel
                        Case LabelTypes.None' 不进行任何操作
                        Case LabelTypes.DEG
                            If factor <> 0 Then
                                Call __drawLabel(gene.label, point)
                            End If
                        Case LabelTypes.ALL
                            Call __drawLabel(gene.label, point)
                        Case Else  ' 自定义
                            If Not gene.label.StringEmpty Then
                                Call __drawLabel(gene.label, point)
                            End If
                    End Select
                Next

                With region
                    Dim legends = colors.GetLegends(legendFont)
                    Dim lsize As SizeF = legends.MaxLegendSize(g)
                    Dim topleft As New Point(
                        .Size.Width - .Padding.Left - (lsize.Width + 50),
                        .Padding.Top)

                    Call g.DrawLegends(topleft, legends)
                End With
            End Sub
    End Function

    <Extension>
    Private Function GetLegends(colors As Dictionary(Of Integer, Color), font$) As Legend()
        Dim up As New Legend With {
            .color = colors(1).RGBExpression,
            .fontstyle = font,
            .style = LegendStyles.Circle,
            .title = "log2FC > UP"
        }
        Dim down As New Legend With {
            .color = colors(-1).RGBExpression,
            .fontstyle = font,
            .style = LegendStyles.Circle,
            .title = "log2FC < DOWN"
        }
        Dim normal As New Legend With {
            .color = colors(0).RGBExpression,
            .fontstyle = font,
            .style = LegendStyles.Circle,
            .title = "Normal"
        }

        Return {normal, up, down}
    End Function

    Public Structure DEGModel
        Dim label$
        Dim logFC#
        Dim pvalue#
    End Structure

    Public Enum LabelTypes
        None
        ''' <summary>
        ''' <see cref="DEGModel.label"/>不为空字符串的时候就会被显示出来
        ''' </summary>
        Custom
        ALL
        DEG
    End Enum
End Module
