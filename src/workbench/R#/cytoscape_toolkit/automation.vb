
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.Cytoscape.Automation

<Package("automation")>
Module automation

    ReadOnly containers As New Dictionary(Of String, cyREST)

    Private Function createContainer(version$, port%, host$) As cyREST
        Select Case version.ToLower
            Case "v1" : Return New v1(port, host)
            Case Else
                Return Nothing
        End Select
    End Function

    <ExportAPI("layouts")>
    Public Function layouts(Optional version$ = "v1", Optional port% = 1234, Optional host$ = "localhost") As String()
        Dim key As String = $"{host}:{port}/{version}"
        Dim container As cyREST = containers.ComputeIfAbsent(key, lazyValue:=Function() createContainer(version, port, host))

        Return container.layouts
    End Function
End Module
