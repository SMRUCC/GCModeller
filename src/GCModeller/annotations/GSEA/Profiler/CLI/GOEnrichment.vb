Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection

Partial Module CLI

    ''' <summary>
    ''' 这个与eggHTS工具中的GO富集条形图有一些不一样
    ''' 这个条形图是分开的
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/GO.enrichment.barplot")>
    <Usage("/GO.enrichment.barplot /in <result.csv> [/out <output_directory>]")>
    Public Function GOEnrichmentBarPlot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.go_enrichment.barplots/"

    End Function
End Module
