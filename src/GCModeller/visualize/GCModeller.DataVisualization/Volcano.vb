#Region "Microsoft.VisualBasic::3e0497e50ca875b341cf8f6d7d655b22, ..\GCModeller\visualize\GCModeller.DataVisualization\Volcano.vb"

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
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting

Public Module Volcano

    ReadOnly DEG_diff# = Math.Log(2, 2)
    ReadOnly diffPValue# = -Math.Log10(0.05)

    Const UP$ = "Up"
    Const DOWN$ = "Down"

    <Extension>
    Public Function PlotDEGs(genes As IEnumerable(Of EntityObject),
                             Optional size As Size = Nothing,
                             Optional margin As Size = Nothing,
                             Optional bg$ = "white",
                             Optional logFC$ = "logFC",
                             Optional pvalue$ = "P.value",
                             Optional displayLabel As LabelTypes = LabelTypes.None,
                             Optional labelFontStyle$ = CSSFont.Win10Normal) As Bitmap

        Return genes.PlotDEGs(
            x:=Function(gene) gene(logFC).ParseNumeric,
            y:=Function(gene) gene(pvalue).ParseNumeric,
            label:=Function(gene) gene.ID,
            size:=size,
            margin:=margin,
            bg:=bg,
            displayLabel:=displayLabel,
            labelFontStyle:=labelFontStyle)
    End Function

    <Extension>
    Public Function PlotDEGs(Of T)(genes As IEnumerable(Of T),
                                   x As Func(Of T, Double),
                                   y As Func(Of T, Double),
                                   label As Func(Of T, String),
                                   Optional size As Size = Nothing,
                                   Optional margin As Size = Nothing,
                                   Optional bg$ = "white",
                                   Optional displayLabel As LabelTypes = LabelTypes.None,
                                   Optional labelFontStyle$ = CSSFont.Win10Normal) As Bitmap

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
            {1, Color.Blue},
            {-1, Color.Red},
            {0, Color.Lime}
        }
        Return genes.Select(
            Function(g) New DEGModel With {
                .label = label(g),
                .logFC = x(g),
                .pvalue = y(g)
        }).Plot(factor, colors,
                size, margin, bg,
                ,,,,
                displayLabel, labelFontStyle)
    End Function

    ReadOnly black As Brush = Brushes.Black

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function Plot(genes As IEnumerable(Of DEGModel), factors As Func(Of DEGModel, Integer), colors As Dictionary(Of Integer, Color),
                         Optional size As Size = Nothing,
                         Optional margin As Size = Nothing,
                         Optional bg$ = "white",
                         Optional xlab$ = "log2 Fold Change",
                         Optional ylab$ = "-log10(p.value)",
                         Optional ptSize! = 5,
                         Optional translate As Func(Of Double, Double) = Nothing,
                         Optional displayLabel As LabelTypes = LabelTypes.None,
                         Optional labelFontStyle$ = CSSFont.PlotSubTitle,
                         Optional legendFont$ = CSSFont.Win7LargerBold) As Bitmap

        If translate Is Nothing Then
            translate = Function(pvalue) -Math.Log10(pvalue)
        End If

        Dim DEG_matrix As DEGModel() = genes.ToArray(
            Function(g) New DEGModel With {
                .label = g.label,
                .logFC = g.logFC,
                .pvalue = translate(g.pvalue)
            })
        Dim scaler As New Scaling(DEG_matrix.ToArray(Function(x) (x.logFC, x.pvalue)))
        Dim brushes As Dictionary(Of Integer, Brush) = colors _
            .ToDictionary(Function(k) k.Key,
                          Function(br) DirectCast(New SolidBrush(br.Value), Brush))

        If size.IsEmpty Then
            size = New Size(2000, 1850)
        End If
        If margin.IsEmpty Then
            margin = New Size(120, 120)
        End If

        Return g.Allocate(size, margin, bg) <=
 _
            Sub(ByRef g As Graphics, region As GraphicsRegion)

                Dim scalling = scaler.TupleScaler(region)
                Dim labelFont As Font = CSSFont.TryParse(labelFontStyle)
                Dim lbSize As SizeF
                Dim gdi As Graphics = g
                Dim __drawLabel = Sub(label$, point As PointF)
                                      lbSize = gdi.MeasureString(label, labelFont)
                                      Call gdi.DrawString(label, labelFont, black, New PointF(point.X - lbSize.Width / 2, point.Y + ptSize))
                                  End Sub

                Call Axis.DrawAxis(g, region, scaler, True,, xlab, ylab)

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
                            If Not gene.label.IsBlank Then
                                Call __drawLabel(gene.label, point)
                            End If
                    End Select
                Next

                With region
                    Dim legends = colors.GetLegends(legendFont)
                    Dim lsize As SizeF = legends.MaxLegendSize(g)
                    Dim topleft As New Point(
                        .Size.Width - .Margin.Width - (lsize.Width + 50),
                        .Margin.Height)

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

