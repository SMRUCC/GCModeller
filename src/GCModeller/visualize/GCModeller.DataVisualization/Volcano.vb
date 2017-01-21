Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.csv.DocumentStream
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
                         Optional labelFontStyle$ = CSSFont.Win10Normal) As Bitmap

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
                                      Call gdi.DrawString(label, labelFont, Drawing.Brushes.Black, New PointF(point.X - lbSize.Width / 2, point.Y + ptSize))
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
            End Sub
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
