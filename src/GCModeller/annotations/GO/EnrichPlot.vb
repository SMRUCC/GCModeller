Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
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
    Public Function EnrichResult(data As IEnumerable(Of EnrichmentTerm), GO_terms As Dictionary(Of Term)) As Dictionary(Of String, NamedValue(Of (P#, richFactor#))())
        Dim result As New Dictionary(Of String, List(Of NamedValue(Of (P#, richFactor#))))

        For Each term As EnrichmentTerm In data
            Dim goTerm As Term = GO_terms(term.ID)

            If Not result.ContainsKey(goTerm.namespace) Then
                Call result.Add(
                    goTerm.namespace,
                    New List(Of NamedValue(Of (P As Double, richFactor As Double))))
            End If

            Call result(goTerm.namespace).Add(
                New NamedValue(Of (P As Double, richFactor As Double)) With {
                    .Name = term.Term,
                    .Value = (term.P, richFactor:=term.number / term.Backgrounds)
                })
        Next

        Dim out = result.ToDictionary(Function(k) k.Key, Function(v) v.Value.ToArray)
        Return out
    End Function

    <Extension>
    Private Sub __plotInternal(g As Graphics,
                               region As GraphicsRegion,
                               result As Dictionary(Of String, NamedValue(Of (P#, richFactor#))()),
                               unenrich As Color,
                               enrichColors As Color())

    End Sub
End Module
