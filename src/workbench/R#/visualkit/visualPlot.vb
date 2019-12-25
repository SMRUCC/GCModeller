
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Visualize.CatalogProfiling

<Package("visualkit.plots")>
Module visualPlot

    <ExportAPI("kegg.category_profiles.plot")>
    Public Function KEGGCategoryProfilePlots(profiles As Dictionary(Of String, Double),
                                             Optional title$ = "KEGG Orthology Profiling",
                                             Optional axisTitle$ = "Number Of Proteins",
                                             Optional size$ = "2300,2000",
                                             Optional tick# = -1,
                                             Optional colors$ = "#E41A1C,#377EB8,#4DAF4A,#984EA3,#FF7F00,#CECE00",
                                             Optional displays% = 10) As GraphicsData

        Dim profile As Dictionary(Of String, NamedValue(Of Double)()) = profiles _
            .KEGGCategoryProfiles _
            .ToDictionary(Function(p) p.Key,
                          Function(group)
                              Return group.Value _
                                 .OrderByDescending(Function(t) t.Value) _
                                 .Take(displays) _
                                 .ToArray
                          End Function)

        Return profile.ProfilesPlot(title,
            size:=size,
            tick:=tick,
            axisTitle:=axisTitle,
            labelRightAlignment:=False,
            valueFormat:="F0",
            colorSchema:=colors
        )
    End Function


End Module
