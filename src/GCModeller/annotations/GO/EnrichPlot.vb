Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Public Module EnrichPlot

    <Extension>
    Public Function BubblePlot(data As IEnumerable(Of EnrichmentTerm),
                               GO_terms As Dictionary(Of Term),
                               Optional size$ = "1600,1200",
                               Optional padding$ = g.DefaultPadding,
                               Optional bg$ = "white",
                               Optional unenrichColor$ = "gray",
                               Optional enrichColorSchema$ = "Paired:c8") As Image

        Dim enrichResult = data.EnrichResult(GO_terms)
        Dim colors As Color() = Designer.GetColors(enrichColorSchema)
        Dim unenrich As Color = unenrichColor.TranslateColor

        Return g.GraphicsPlots(
            size.SizeParser, padding,
            bg,
            Sub(ByRef g, region)
                Call g.__plotInternal(region, enrichResult, unenrich, colors)
            End Sub)
    End Function

    <Extension>
    Public Function EnrichResult(data As IEnumerable(Of EnrichmentTerm), GO_terms As Dictionary(Of Term)) As Dictionary(Of String, EnrichmentTerm())
        Dim result As New Dictionary(Of String, List(Of EnrichmentTerm))

        For Each term As EnrichmentTerm In data
            Dim goTerm As Term = GO_terms(term.ID)

            If Not result.ContainsKey(goTerm.namespace) Then
                Call result.Add(goTerm.namespace, New List(Of EnrichmentTerm))
            End If

            Call result(goTerm.namespace).Add(term)
        Next

        Dim out = result.ToDictionary(Function(k) k.Key, Function(v) v.Value.ToArray)
        Return out
    End Function

    <Extension>
    Private Sub __plotInternal(g As Graphics,
                               region As GraphicsRegion,
                               result As Dictionary(Of String, EnrichmentTerm()),
                               unenrich As Color,
                               enrichColors As Color())
        Dim serials As SerialData() = result _
            .SeqIterator _
            .Select(Function(cat) (+cat).Value.__createModel(ns:=(+cat).Key, color:=enrichColors(cat))) _
            .ToArray
        Dim plot As Bitmap = Bubble.Plot(
            serials,
            size:=New Size(region.Size.Width * 0.85, region.Size.Height),
            legend:=False)

        Call g.DrawImageUnscaled(plot, New Point)

    End Sub

    <Extension>
    Private Function __createModel(catalog As EnrichmentTerm(), ns$, color As Color) As SerialData
        Return New SerialData With {
            .color = color,
            .title = ns,
            .pts = catalog.Select(
                Function(gene) New PointData With {
                    .value = gene.number,
                    .pt = New PointF(x:=gene.number / gene.Backgrounds, y:=gene.P)
                }).ToArray
        }
    End Function
End Module
