Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.DataVisualization.Network
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.InteractionModel.Network.Regulons
Imports Microsoft.VisualBasic.Language

Partial Module CLI

    <ExportAPI("--graph.regulates", Usage:="--graph.regulates /footprint <footprints.csv> [/trim]")>
    Public Function SimpleRegulation(args As CommandLine.CommandLine) As Integer
        Dim input As String = args("/footprint")
        Dim footprints = input.LoadCsv(Of PredictedRegulationFootprint)
        Dim Trim As Boolean = args.GetBoolean("/trim")

        If Trim Then
            footprints = (From x As PredictedRegulationFootprint
                          In footprints.AsParallel
                          Where Not String.IsNullOrEmpty(x.Regulator)
                          Select x).ToList
        End If

        Dim cytoscape = CytoscapeGraphView.Serialization.Export(__getNodes(footprints), footprints.ToArray)
        Dim path As String = input.TrimFileExt & ".Cytoscape.Xml"
        Return cytoscape.Save(path)
    End Function

    Private Function __getNodes(footprints As List(Of PredictedRegulationFootprint)) As FileStream.Node()
        Dim ORF = footprints.ToArray(Function(x) x.ORF).Distinct.ToList
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

        Dim Nodes = ORF.ToArray(Function(sId) New FileStream.Node With {.Identifier = sId, .NodeType = "ORF"}).ToList
        Nodes += TFs.ToArray(Function(sId) New FileStream.Node With {.Identifier = sId, .NodeType = "Regulator"})
        Nodes += Hybrids.ToArray(Function(sId) New FileStream.Node With {.Identifier = sId, .NodeType = "ORF+TF"})
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
    Public Function TFNet(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim footprints = inFile.LoadCsv(Of PredictedRegulationFootprint)
        Dim cut As Double = args.GetValue("/cut", 0.45)
        Dim net As FileStream.Network = footprints.BuildNetwork(cut)
        Dim out As String = args.GetValue("/out", inFile.TrimFileExt & $".regulates-TF_NET,cut={cut}/")
        Return net.Save(out, Encodings.ASCII).CLICode
    End Function
End Module