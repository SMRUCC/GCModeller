﻿#Region "Microsoft.VisualBasic::bb84ad1fc3f5ca9a52fd196c5fd8e9a1, visualize\Cytoscape\CLI_tool\CLI\Cli.vb"

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

    ' Module CLI
    ' 
    '     Function: cyjsAsGraph, DrawingInvoke, getNetworkParser, getStyleParser
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.ManView
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Visualize.Cytoscape
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML
Imports SMRUCC.genomics.Visualize.Cytoscape.Visualization

<Package("Cytoscape.CLI",
                  Category:=APICategories.CLI_MAN,
                  Description:="Cytoscape model generator and visualization tools utils for GCModeller",
                  Publisher:="xie.guigang@gcmodeller.org",
                  Url:="https://github.com/xieguigang/GCModeller.Cytoscape")>
<GroupingDefine(CLIGrouping.KEGGPhenotype, Description:=CLIGrouping.KEGGPhenotypeDescription)>
<ExceptionHelp(Documentation:="http://docs.gcmodeller.org", Debugging:="https://github.com/SMRUCC/GCModeller/wiki", EMailLink:="xie.guigang@gcmodeller.org")>
<CLI> Module CLI

    <ExportAPI("-Draw",
               Usage:="-draw /network <net_file> /parser <xgmml/cyjs> [-size <width,height> -out <out_image> /style <style_file> /style_parser <vizmap/json>]",
               Info:="Drawing a network image visualization based on the generate network layout from the officials cytoscape software.")>
    Public Function DrawingInvoke(argvs As CommandLine.CommandLine) As Integer
        Dim Size As Size = argvs.GetObject(Of Size)("-size", AddressOf getSize)
        Dim Output As String = argvs("-out")
        Dim Style As Visualization.VizMap =
            argvs.GetObject(Of VizMap)("/style", argvs.GetObject(Of Func(Of String, VizMap))("/style_parser", AddressOf getStyleParser))

        Dim NetworkGraph = argvs.GetObject(Of Graph)("/network", argvs.GetObject(Of Func(Of String, Graph))("/parser", AddressOf getNetworkParser))
        Dim res As Image = Nothing

        If Style Is Nothing Then
            Call $"{NameOf(Style)} data is nothing, irnored of the drawing styles...".__DEBUG_ECHO
            res = GraphDrawing.InvokeDrawing(NetworkGraph, Size)
        Else

        End If

        If String.IsNullOrEmpty(Output) Then
            Output = argvs("/network") & ".png"
        End If

        Call res.Save(Output, Imaging.ImageFormat.Png)

        Return 0
    End Function

    Private Function getNetworkParser(name As String) As Func(Of String, Graph)
        If String.Equals(name, "xgmml", StringComparison.OrdinalIgnoreCase) Then
            Return Function(path As String) Graph.Load(path)
        ElseIf String.Equals(name, "cyjs", StringComparison.OrdinalIgnoreCase) Then
            Return AddressOf CLI.cyjsAsGraph
        Else
            Call $"Network file parser ""{name}"" was not recognized!".__DEBUG_ECHO
            Return Nothing
        End If
    End Function

    Private Function cyjsAsGraph(cyjs As String) As Graph
        Dim jsonText As String = IO.File.ReadAllText(cyjs)
        Dim json As Cyjs.Cyjs = (jsonText).LoadJSON(Of Cyjs.Cyjs)
        Return json.ToGraphModel
    End Function

    Private Function getStyleParser(name As String) As Func(Of String, VizMap)
        If String.Equals(name, "vizmap", StringComparison.OrdinalIgnoreCase) Then
            Return Function(path As String) path.LoadXml(Of Visualization.VizMap)(throwEx:=False)
        ElseIf String.Equals(name, "json", StringComparison.OrdinalIgnoreCase) Then
        Else
            Call $"Network style file parser ""{name}"" was not recognized!".__DEBUG_ECHO
            Return Nothing
        End If

        Throw New NotImplementedException
    End Function

End Module
