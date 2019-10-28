Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Partial Module CLI

    ''' <summary>
    ''' 这个与eggHTS工具中的GO富集条形图有一些不一样
    ''' 这个条形图是分开的
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/GO.enrichment.barplot")>
    <Usage("/GO.enrichment.barplot /in <result.csv> [/go <go.obo> /tiff /out <output_directory>]")>
    Public Function GOEnrichmentBarPlot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.go_enrichment.barplots/"
        Dim goDB As String = args("/go") Or (GCModeller.FileSystem.GO & "/go.obo")
        Dim terms = GO_OBO.Open(goDB).ToDictionary
        Dim enrichments As IEnumerable(Of EnrichmentTerm) = [in].LoadCsv(Of EnrichmentTerm)
        Dim saveInTiff As Boolean = args("/tiff")
        Dim outFile$
        Dim tiff As TiffWriter

        For Each plot As NamedValue(Of GraphicsData) In enrichments.NamespaceEnrichmentPlot(terms)
            If TypeOf plot.Value Is ImageData Then
                If saveInTiff Then
                    outFile = $"{out}/{plot.Name}.tiff"
                    tiff = New TiffWriter(DirectCast(plot.Value, ImageData).Image)
                    tiff.MultipageTiffSave(outFile)
                Else
                    outFile = $"{out}/{plot.Name}.png"
                    plot.Value.Save(outFile)
                End If
            Else
                outFile = $"{out}/{plot.Name}.svg"
                plot.Value.Save(outFile)
            End If
        Next

        Return 0
    End Function
End Module
