
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots

<Package("circos.document", Category:=APICategories.UtilityTools)>
Module Document

    ''' <summary>
    ''' Creats a new <see cref="Circos"/> plots configuration document.
    ''' </summary>
    <ExportAPI("circos")>
    Public Function CreateDataModel() As Circos
        Return Circos.CreateObject
    End Function

    ''' <summary>
    ''' Add track plot data model into circos doc object.
    ''' </summary>
    ''' <param name="circos"></param>
    ''' <param name="track"></param>
    ''' <returns></returns>
    <ExportAPI("add.track")>
    Public Function AddTrack(circos As Circos, track As ITrackPlot) As Circos
        circos.AddTrack(track)
        Return circos
    End Function

End Module
