#Region "Microsoft.VisualBasic::c7eddf9a1b6392abb79794b0f599b757, ..\GCModeller\shoalAPI\Cytoscape.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView.XGMML
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView

<[PackageNamespace]("Cytoscape", Publisher:="amethyst.asuka@gcmodeller.org")>
Module Cytoscape

    <IO_DeviceHandle(GetType(Graph))>
    <ExportAPI("Write.XML.Cytoscape")>
    Public Function WriteCytoscapeXML(Model As Graph, <Parameter("Path.SaveTo", "The network model will be save to this file path.")> SaveTo As String) As Boolean
        Return Model.Save(SaveTo)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="VirtualFootprints"></param>
    ''' <param name="PfsNET">不设置被参数的时候默认导出整个网络</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <InputDeviceHandle("Cytoscape")>
    <ExportAPI("Virtual_Footprints.Export.Cytoscape")>
    Public Function ExportVirtualFootprint(VirtualFootprints As IEnumerable(Of LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint),
                                           <Parameter("Apply.Filter.PfsNET", "If the parameter is nothing in this function, then the whole network data will be export.")>
                                           Optional PfsNET As LANS.SystemsBiology.AnalysisTools.CellularNetwork.PFSNet.DataStructure.PFSNetResultOut = Nothing) As Graph

        'Dim CytoscapeGraph As LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.DocumentFormat.CytoscapeGraphView.Graph =
        '    LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.DocumentFormat.CytoscapeGraphView.Graph.CreateObject("Virtual footprints cytoscape network.", "Virtual-Footprint-TF-Regulations")
        'Dim NodesID As String() = (From Regulation As LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint
        '                           In VirtualFootprints
        '                           Let ORFID As String() = {Regulation.ORF}
        '                           Let InternalIDMatrix As String()() = {ORFID, Regulation.StructGenes, Regulation.Regulators}
        '                           Select InternalIDMatrix.MatrixToList).ToArray.MatrixToList.Distinct.ToArray
        'CytoscapeGraph.Nodes = (From ID As String
        '                        In NodesID.AsParallel
        '                        Select New LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.DocumentFormat.CytoscapeGraphView.DocumentElements.Node With {.LabelTitle = ID}).ToArray.AddHandle
        'CytoscapeGraph.Edges = (From TFRegulation As LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint
        '                        In VirtualFootprints.AsParallel
        '                        Select BuildEdges(TFRegulation, Graph:=CytoscapeGraph)).ToArray.MatrixToVector.AddHandle
        'If Not PfsNET Is Nothing Then
        '    Dim Nodes As String() = (From p In {PfsNET.Phenotype1, PfsNET.Phenotype2}.MatrixToList Select (From Node In p.Nodes Select Node.Name).ToArray).ToArray.MatrixToList.Distinct.ToArray
        '    CytoscapeGraph.Nodes = (From Node In CytoscapeGraph.Nodes Where Array.IndexOf(Nodes, Node.LabelTitle) > -1 Select Node).ToArray
        '    CytoscapeGraph.Edges = (From Edge In CytoscapeGraph.Edges Where CytoscapeGraph.ExistEdge(Edge) Select Edge).ToArray
        'End If

        'For Each ID As String In (From Edge In VirtualFootprints.AsParallel Select Edge.Regulators).ToArray.MatrixToList.Distinct.ToArray
        '    Dim TFNode = CytoscapeGraph.GetNode(ID)
        '    If TFNode Is Nothing Then
        '        Continue For
        '    End If
        '    Call TFNode.SetAttribute("Gene_Type", "TF")
        'Next

        'Return CytoscapeGraph
    End Function

    <InputDeviceHandle("PfsNET")>
    Public Function LoadPfsNETXml(Path As String) As LANS.SystemsBiology.AnalysisTools.CellularNetwork.PFSNet.DataStructure.PFSNetResultOut
        Return Path.LoadXml(Of LANS.SystemsBiology.AnalysisTools.CellularNetwork.PFSNet.DataStructure.PFSNetResultOut)()
    End Function

    ''' <summary>
    ''' 主要将结构基因以及预测的调控因子进行组合展开
    ''' </summary>
    ''' <param name="TFRegulation"></param>
    ''' <param name="Graph"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function BuildEdges(TFRegulation As LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint,
                                Graph As Graph) As XGMML.Edge()
        'Dim LQuery = (From GeneID As String
        '              In (New String()() {TFRegulation.StructGenes, New String() {TFRegulation.ORF}}).MatrixToList
        '              Select (From RegulatorID As String
        '                      In TFRegulation.Regulators
        '                      Select New LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.DocumentFormat.CytoscapeGraphView.DocumentElements.Edge With {
        '                          .Label = String.Format("{0} -> {1}", RegulatorID, GeneID),
        '                          .source = Graph.GetNode(RegulatorID).IDPointer,
        '                          .target = Graph.GetNode(GeneID).IDPointer}).ToArray).ToArray.MatrixToList.ToArray
        'Return LQuery
    End Function
End Module
