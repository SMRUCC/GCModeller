
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

<Package("visualkit.plots")>
Module visualPlot

    <ExportAPI("kegg.category_profiles.plot")>
    Public Function KEGGCategoryProfilePlots(profiles As Dictionary(Of String, Double)) As GraphicsData
        Dim brite As Dictionary(Of String, Pathway) = Pathway.LoadDictionary
    End Function
End Module
