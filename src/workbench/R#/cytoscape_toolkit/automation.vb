#Region "Microsoft.VisualBasic::dba205ba2975d275dc1e85a8a183f5ab, cytoscape_toolkit\automation.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module automation
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: applyLayout, createContainer, createNetwork, getContainer, getCurrentViewReference
    '               layouts, networkView, printNetworkReference, saveSession
    ' 
    '     Sub: destroySession
    ' 
    ' /********************************************************************************/

#End Region


Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Visualize.Cytoscape.Automation
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.Cyjs
Imports SMRUCC.genomics.Visualize.Cytoscape.Tables
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("automation")>
Module automation

    Sub New()
        Call Internal.ConsolePrinter.AttachConsoleFormatter(Of NetworkReference())(AddressOf printNetworkReference)
    End Sub

    Private Function printNetworkReference(ref As NetworkReference()) As String
        Return ref.ToDictionary(Function(a) a.source, Function(a) a.networkSUID(Scan0)).GetJson
    End Function

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
    <RApiReturn(GetType(NetworkReference))>
    Public Function createNetwork(<RRawVectorArgument>
                                  network As Object,
                                  Optional collection$ = Nothing,
                                  Optional title$ = Nothing,
                                  Optional version$ = "v1",
                                  Optional port% = 1234,
                                  Optional host$ = "localhost",
                                  Optional env As Environment = Nothing) As Object

        Dim container As cyREST = automation.getContainer(version, port, host)
        Dim model As [Variant](Of Cyjs, SIF())

        If network.GetType Is GetType(Cyjs) Then
            model = DirectCast(network, Cyjs)
        ElseIf network.GetType Is GetType(SIF()) Then
            model = DirectCast(network, SIF())
        Else
            Return Internal.debug.stop(Message.InCompatibleType(GetType(Cyjs), network.GetType, env), env)
        End If

        Return container.putNetwork(model, collection, title)
    End Function

    <ExportAPI("layout")>
    Public Function applyLayout(networkId As Object,
                                Optional algorithmName As String = "force-directed",
                                Optional version$ = "v1",
                                Optional port% = 1234,
                                Optional host$ = "localhost",
                                Optional env As Environment = Nothing) As Object

        Dim container As cyREST = automation.getContainer(version, port, host)

        If networkId Is Nothing Then
            Return Internal.debug.stop("no network specified!", env)
        ElseIf TypeOf networkId Is Integer OrElse TypeOf networkId Is Long Then
            Return container.applyLayout(networkId, algorithmName)
        ElseIf TypeOf networkId Is NetworkReference Then
            Return container.applyLayout(DirectCast(networkId, NetworkReference).networkSUID, algorithmName)
        Else
            Return Internal.debug.stop(Message.InCompatibleType(GetType(Integer), networkId.GetType, env), env)
        End If
    End Function

    <ExportAPI("session.save")>
    Public Function saveSession(file As String, Optional version$ = "v1", Optional port% = 1234, Optional host$ = "localhost") As Object
        Dim container As cyREST = automation.getContainer(version, port, host)
        Return container.saveSession(file)
    End Function

    <ExportAPI("view")>
    Public Function getCurrentViewReference(Optional version$ = "v1", Optional port% = 1234, Optional host$ = "localhost") As Integer
        Return automation.getContainer(version, port, host).getViewReference
    End Function

    <ExportAPI("networkView")>
    <RApiReturn(GetType(Cyjs))>
    Public Function networkView(networkId As Object, viewId As Object, Optional version$ = "v1", Optional port% = 1234, Optional host$ = "localhost", Optional env As Environment = Nothing) As Object
        If networkId Is Nothing Then
            Return Internal.debug.stop("the network reference id can not be nothing!", env)
        ElseIf TypeOf networkId Is Long Then
            networkId = CType(networkId, Integer)
        ElseIf TypeOf networkId Is NetworkReference Then
            networkId = DirectCast(networkId, NetworkReference).networkSUID.DoCall(AddressOf Integer.Parse)
        ElseIf Not TypeOf networkId Is Integer Then
            Return Internal.debug.stop(Message.InCompatibleType(GetType(Integer), networkId.GetType, env), env)
        End If

        If viewId Is Nothing Then
            Return Internal.debug.stop("the network viewer reference id can not be nothing!", env)
        ElseIf TypeOf viewId Is Long Then
            viewId = CType(viewId, Integer)
        ElseIf Not TypeOf viewId Is Integer Then
            Return Internal.debug.stop(Message.InCompatibleType(GetType(Integer), viewId.GetType, env), env)
        End If

        Return automation.getContainer(version, port, host).getView(networkId, viewId)
    End Function

    <ExportAPI("finalize")>
    Public Sub destroySession(Optional version$ = "v1", Optional port% = 1234, Optional host$ = "localhost")
        Call automation.getContainer(version, port, host).destroySession()
    End Sub
End Module

