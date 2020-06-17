
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.Cytoscape.Automation
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.Cyjs
Imports SMRUCC.genomics.Visualize.Cytoscape.Tables
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("automation")>
Module automation

    Private Function createContainer(version$, port%, host$) As cyREST
        Select Case version.ToLower
            Case "v1" : Return New v1(port, host)
            Case Else
                Return Nothing
        End Select
    End Function

    Private Function getContainer(version$, port%, host$) As cyREST
        Static containers As New Dictionary(Of String, cyREST)

        Dim key$ = $"{host}:{port}/{version}"
        Dim container As cyREST = containers.ComputeIfAbsent(key, lazyValue:=Function() createContainer(version, port, host))

        Return container
    End Function

    <ExportAPI("cache")>
    Public Function cacheFile(file As String) As String
        Return getContainer("v1", 1234, "localhost").addUploadFile(file)
    End Function

    ''' <summary>
    ''' GET list of layout algorithms
    ''' </summary>
    ''' <param name="version$"></param>
    ''' <param name="port%"></param>
    ''' <param name="host$"></param>
    ''' <returns></returns>
    <ExportAPI("layouts")>
    Public Function layouts(Optional version$ = "v1", Optional port% = 1234, Optional host$ = "localhost") As String()
        Dim container As cyREST = automation.getContainer(version, port, host)
        Return container.layouts
    End Function

    <ExportAPI("put_network")>
    Public Function createNetwork(<RRawVectorArgument> network As Object, Optional version$ = "v1", Optional port% = 1234, Optional host$ = "localhost", Optional env As Environment = Nothing)
        Dim container As cyREST = automation.getContainer(version, port, host)
        Dim model As [Variant](Of Cyjs, SIF())

        If network.GetType Is GetType(Cyjs) Then
            model = DirectCast(network, Cyjs)
        ElseIf network.GetType Is GetType(SIF()) Then
            model = DirectCast(network, SIF())
        Else
            Return Internal.debug.stop(Message.InCompatibleType(GetType(Cyjs), network.GetType, env), env)
        End If

        Return container.putNetwork(model)
    End Function
End Module
