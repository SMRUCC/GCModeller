#Region "Microsoft.VisualBasic::3637d7ef0ed084d124cb2482375580bf, visualize\Cytoscape\CLI_tool\CLI\SimpleRegulations.vb"

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
    '     Function: __getNodes, SimpleRegulation, TFNet
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Model.Network.Regulons
Imports SMRUCC.genomics.Visualize.Cytoscape

Partial Module CLI

    <ExportAPI("--graph.regulates", Usage:="--graph.regulates /footprint <footprints.csv> [/trim]")>
    <Group(CLIGrouping.RegulationNetwork)>
    Public Function SimpleRegulation(args As CommandLine) As Integer
        Dim input As String = args("/footprint")
        Dim footprints = input.LoadCsv(Of PredictedRegulationFootprint)
        Dim Trim As Boolean = args.GetBoolean("/trim")

        If Trim Then
            footprints = (From x As PredictedRegulationFootprint
                          In footprints.AsParallel
                          Where Not String.IsNullOrEmpty(x.Regulator)
                          Select x).AsList
        End If

        Dim cytoscape = CytoscapeGraphView.Serialization.Export(__getNodes(footprints), footprints.ToArray)
        Dim path As String = input.TrimSuffix & ".Cytoscape.Xml"
        Return cytoscape.Save(path)
    End Function

    Private Function __getNodes(footprints As List(Of PredictedRegulationFootprint)) As FileStream.Node()
        Dim ORF = footprints.Select(Function(x) x.ORF).Distinct.AsList
        Dim TFs As List(Of String) =
            LinqAPI.MakeList(Of String) <= From x As PredictedRegulationFootprint
                                           In footprints
                                           Where Not String.IsNullOrEmpty(x.Regulator)
                                           Select x.Regulator
                                           Distinct
        Dim Hybrids As String() = (New [Set](TFs) And New [Set](ORF)).ToArray(Of String)
        For Each sId As String In Hybrids
            Call ORF.Remove(sId)
            Call TFs.Remove(sId)
        Next

        Dim Nodes = ORF.Select(Function(sId) New FileStream.Node With {.ID = sId, .NodeType = "ORF"}).AsList
        Nodes += TFs.Select(Function(sId) New FileStream.Node With {.ID = sId, .NodeType = "Regulator"})
        Nodes += Hybrids.Select(Function(sId) New FileStream.Node With {.ID = sId, .NodeType = "ORF+TF"})
        Return Nodes
    End Function

    ''' <summary>
    ''' 调控因子之间的调控网络
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/NetModel.TF_regulates",
               Info:="Builds the regulation network between the TF.",
               Usage:="/NetModel.TF_regulates /in <footprints.csv> [/out <outDIR> /cut 0.45]")>
    <Group(CLIGrouping.RegulationNetwork)>
    Public Function TFNet(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim footprints = inFile.LoadCsv(Of PredictedRegulationFootprint)
        Dim cut As Double = args.GetValue("/cut", 0.45)
        Dim net As FileStream.NetworkTables = footprints.BuildNetwork(cut)
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & $".regulates-TF_NET,cut={cut}/")
        Return net.Save(out, Encoding.ASCII).CLICode
    End Function
End Module
