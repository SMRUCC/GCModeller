#Region "Microsoft.VisualBasic::e461f3b74cc1f45ddf3a1bafca022285, R#\cytoscape_toolkit\models.vb"

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
Imports REnv = SMRUCC.Rsharp.Runtime
Imports stdNum = System.Math

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

    <ExportAPI("sif")>
    Public Function sif(<RRawVectorArgument> source As Object, <RRawVectorArgument> interaction As Object, <RRawVectorArgument> target As Object) As SIF()
        Dim U As String() = REnv.asVector(Of String)(source)
        Dim type As String() = REnv.asVector(Of String)(interaction)
        Dim V As String() = REnv.asVector(Of String)(target)

        Return Iterator Function() As IEnumerable(Of SIF)
                   Dim n As Integer = stdNum.Max(U.Length, V.Length)

                   For i As Integer = 0 To n - 1
                       Yield New SIF With {
                           .interaction = type.ElementAtOrDefault(i),
                           .source = U.ElementAtOrDefault(i),
                           .target = V.ElementAtOrDefault(i)
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

    <ExportAPI("get.sessionInfo")>
    Public Function getSessionInfo(cys As CysSessionFile) As virtualColumn()
        Return cys.GetSessionInfo
    End Function

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
