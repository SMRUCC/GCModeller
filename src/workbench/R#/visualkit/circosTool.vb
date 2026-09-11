
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights

<Package("circos")>
Module circosTool

    <ExportAPI("IdentityColors")>
    Public Function IdentityColors([default] As String) As IdentityColors
        Return New IdentityLevels([default])
    End Function

    <ExportAPI("IdentityColors")>
    Public Function IdentityColors(min#, max#, Optional depth% = 10, Optional default$ = "Brown", Optional mapName$ = "Jet") As IdentityColors
        Return New IdentityGradients(min, max, depth, [default], mapName)
    End Function
End Module
