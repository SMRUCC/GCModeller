
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("visualkit.plots")>
Module visualPlot

    <ExportAPI("kegg.category_profiles.plot")>
    Public Function KEGGCategoryProfilePlots(profiles As Dictionary(Of String, Double)) As GraphicsData

    End Function
End Module
