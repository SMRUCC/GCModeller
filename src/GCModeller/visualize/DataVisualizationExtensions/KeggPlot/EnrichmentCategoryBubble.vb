Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

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

        For Each category As String In enrich.Keys
            For Each term As EnrichmentResult In enrich(category)
                Dim label_size As SizeF = g.MeasureString(term.name, name_label_font)
                Dim label_left = left - label_size.Width
                Dim label_pos As New PointF(label_left, y)

                y += termH
                g.DrawString(term.name, name_label_font, labelColor, label_pos)
            Next
        Next
    End Sub
End Class
