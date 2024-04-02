Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports std = System.Math

Public Class EnrichmentCategoryBubble : Inherits Plot

    ReadOnly enrich As Dictionary(Of String, EnrichmentResult())

    Public Sub New(enrich As IEnumerable(Of EnrichmentResult), theme As Theme)
        Call MyBase.New(theme)

        Dim kegg = htext.br08901.GetEntryDictionary

        Me.enrich = enrich _
            .GroupBy(Function(ti)
                         Dim key As String = ti.term.Match("\d+")

                         If kegg.ContainsKey(key) Then
                             Return kegg(key).class
                         Else
                             Return "NO_CLASS"
                         End If
                     End Function) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.ToArray
                          End Function)
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim plotH As Single = canvas.PlotRegion.Height
        Dim termH As Single = plotH / (Aggregate ci In enrich Into Sum(ci.Value.Length))
        Dim max_string = enrich.Values.IteratesALL.Select(Function(ti) ti.name).MaxLengthString
        Dim name_label_font As Font = CSSFont.TryParse(theme.axisLabelCSS).GDIObject(g.Dpi)
        Dim left = canvas.PlotRegion.Left + g.MeasureString(max_string, name_label_font).Width
        Dim labelColor As Brush = theme.tagColor.GetBrush
        Dim y As Single = canvas.PlotRegion.Top
        Dim dashline As Pen = Stroke.TryParse(theme.gridStrokeY)
        Dim plotW As Single = canvas.PlotRegion.Width - g.MeasureString(max_string, name_label_font).Width
        Dim x_scaler As DoubleRange = -enrich.Values.IteratesALL.Select(Function(ti) ti.pvalue).AsVector.Log10
        Dim radius_scaler As DoubleRange = enrich.Values.IteratesALL.Select(Function(ti) Val(ti.enriched)).AsVector
        Dim color_scaler As DoubleRange = enrich.Values.IteratesALL.Select(Function(ti) ti.FDR).AsVector
        Dim radius As New DoubleRange(0.1 * termH, termH)
        Dim colors As Brush() = Designer.GetBrushes(theme.colorSet)
        Dim colorOffset As New DoubleRange(0, colors.Length - 1)
        Dim x = d3js.scale.linear().range(values:={left, left + plotW}).domain(x_scaler)

        For Each category As String In enrich.Keys
            For Each term As EnrichmentResult In enrich(category)
                Dim label_size As SizeF = g.MeasureString(term.name, name_label_font)
                Dim label_left = left - label_size.Width
                Dim label_pos As New PointF(label_left, y)
                Dim r As Single = radius_scaler.ScaleMapping(Val(term.enriched), radius)
                Dim c As Brush = colors(CInt(color_scaler.ScaleMapping(term.FDR, colorOffset)))
                Dim xi As Single = -std.Log10(term.pvalue)

                xi = x(xi)
                y += termH
                g.DrawString(term.name, name_label_font, labelColor, label_pos)
                g.DrawLine(dashline, New PointF(left, label_pos.Y), New PointF(left + plotW, label_pos.Y))
                g.DrawCircle(New PointF(xi, label_pos.Y), r, c)
            Next

            y += termH / 2
        Next
    End Sub
End Class
