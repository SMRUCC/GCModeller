#Region "Microsoft.VisualBasic::66e2c3457a2b74a00ccb1a1d1fef044a, R#\cytoscape_toolkit\models.vb"

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


    ' Code Statistics:

    '   Total Lines: 145
    '    Code Lines: 97
    ' Comment Lines: 34
    '   Blank Lines: 14
    '     File Size: 6.08 KB


    ' Module models
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: createGraph, cyjs, GetLayoutedGraph, getNetworks, getSessionInfo
    '               openSessionFile, printSessionInfo, sif
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.Cyjs
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File
Imports SMRUCC.genomics.Visualize.Cytoscape.Session
Imports SMRUCC.genomics.Visualize.Cytoscape.Tables
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports REnv = SMRUCC.Rsharp.Runtime
Imports std = System.Math

''' <summary>
''' api for create network graph model for cytoscape
''' </summary>
<Package("models", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gcmodeller.org")>
Module models

    Sub New()
        REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of virtualColumn())(AddressOf printSessionInfo)
    End Sub

    Private Function printSessionInfo(table As virtualColumn()) As String
        Return table.ToCsvDoc.AsMatrix.Print
    End Function

    ''' <summary>
    ''' create sif network
    ''' </summary>
    ''' <param name="source">a character vector of the source id</param>
    ''' <param name="interaction">a character vector of the iteraction type labels</param>
    ''' <param name="target">a character vector of the target id</param>
    ''' <returns>a simple network graph which consist with a set of the simple iteraction links.</returns>
    <ExportAPI("sif")>
    <RApiReturn(GetType(SIF))>
    Public Function sif(<RRawVectorArgument> source As Object,
                        <RRawVectorArgument> interaction As Object,
                        <RRawVectorArgument> target As Object) As Object

        Dim U As String() = CLRVector.asCharacter(source)
        Dim type As String() = CLRVector.asCharacter(interaction)
        Dim V As String() = CLRVector.asCharacter(target)

        Return Iterator Function() As IEnumerable(Of SIF)
                   Dim n As Integer = std.Max(U.Length, V.Length)
                   Dim u_vec = GetVectorElement.Create(Of String)(U)
                   Dim v_vec = GetVectorElement.Create(Of String)(V)
                   Dim t_vec = GetVectorElement.Create(Of String)(type)

                   For i As Integer = 0 To n - 1
                       Yield New SIF With {
                           .interaction = t_vec(i),
                           .source = u_vec(i),
                           .target = v_vec(i)
                       }
                   Next
               End Function().ToArray
    End Function

    <ExportAPI("cyjs")>
    <RApiReturn(GetType(Cyjs))>
    Public Function cyjs(<RRawVectorArgument> network As Object, Optional env As Environment = Nothing) As Object
        Select Case network.GetType
            Case GetType(SIF()) : Return New Cyjs(DirectCast(network, SIF()))
            Case Else
                Return Internal.debug.stop(Message.InCompatibleType(GetType(SIF()), network.GetType, env), env)
        End Select
    End Function

    ''' <summary>
    ''' convert the cytoscape cyjs/xgmml file to network graph model.
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="propertyNames"></param>
    ''' <returns></returns>
    <ExportAPI("as.graph")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function createGraph(model As Object,
                                <RRawVectorArgument(GetType(String))>
                                Optional propertyNames As Object = "label|class|group.category|group.category.color",
                                Optional env As Environment = Nothing) As Object

        If model Is Nothing Then
            Return Nothing
        ElseIf TypeOf model Is XGMMLgraph Then
            Return DirectCast(model, XGMMLgraph).ToNetworkGraph(DirectCast(propertyNames, String()))
        ElseIf TypeOf model Is Cyjs Then
            Return DirectCast(model, Cyjs).ToNetworkGraph
        Else
            Return Internal.debug.stop(Message.InCompatibleType(GetType(XGMMLgraph), model.GetType, env), env)
        End If
    End Function

    ''' <summary>
    ''' open a new cytoscape session file reader
    ''' </summary>
    ''' <param name="cys"></param>
    ''' <returns></returns>
    <ExportAPI("open.cys")>
    Public Function openSessionFile(cys As String) As CysSessionFile
        Return CysSessionFile.Open(cys)
    End Function

    ''' <summary>
    ''' get session information about current session file
    ''' </summary>
    ''' <param name="cys"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' get session information from <see cref="cyTables"/> xml file
    ''' </remarks>
    <ExportAPI("get.sessionInfo")>
    <RApiReturn(GetType(virtualColumn))>
    Public Function getSessionInfo(cys As CysSessionFile) As Object
        Return cys.GetSessionInfo
    End Function

    ''' <summary>
    ''' list of the network id inside current cytoscape session file
    ''' </summary>
    ''' <param name="cys"></param>
    ''' <returns></returns>
    <ExportAPI("list.networks")>
    Public Function getNetworks(cys As CysSessionFile) As list
        Return New list With {
            .slots = cys _
                .GetNetworks _
                .ToDictionary(Function(a) a.name,
                              Function(a)
                                  Return CObj(a.ToArray)
                              End Function)
        }
    End Function

    <ExportAPI("get.network_graph")>
    Public Function GetLayoutedGraph(cys As CysSessionFile, Optional collection$ = Nothing, Optional name$ = Nothing) As NetworkGraph
        Return cys.GetLayoutedGraph(collection, name)
    End Function
End Module
