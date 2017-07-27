#Region "Microsoft.VisualBasic::f8bdc437d6031b6a90fb4bf8012595ef, ..\interops\visualize\Cytoscape\Cytoscape.App\NetworkModel\PINetwork.vb"

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

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Model
Imports SMRUCC.genomics.foundation
Imports SMRUCC.genomics.foundation.psidev.XML
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML

Namespace NetworkModel.StringDB

    ''' <summary>
    ''' 构建蛋白质互作网络的绘图模型
    ''' </summary>
    ''' 
    <Package("String-Db.Interactions", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gmail.com")>
    Public Module PINetwork

        <ExportAPI("Build", Info:="Build the protein interaction network cytoscape visualization model file.")>
        Public Function BuildModel(PTT As PTT,
                                   <Parameter("DIR.string-DB")> stringDB As String,
                                   <Parameter("Trim.Confidence")> Optional TrimConfidence As Double = -1,
                                   <Parameter("Trim.Degree")> Optional TrimDegree As Integer = 0) As Graph

            Dim Model As Graph = Graph.CreateObject(
                    Title:=PTT.Title & " - Protein interaction network",
                    Type:="Protein Interaction",
                    Description:=$"Prediction protein interaction network from string-db.org, {PTT.Title}, {PTT.NumOfProducts} proteins.")

            Model.Nodes = (From GeneObject As GeneBrief
                           In PTT.GeneObjects.AsParallel
                           Select New XGMML.Node With {
                               .label = GeneObject.Synonym,
                               .Attributes = __attributes(GeneObject)}).WriteAddress '使用PTT文件首先生成节点

            Dim Network As New List(Of Edge)

            For Each iteraction In stringDB.LoadSourceEntryList({"*.xml"})      ' string-db数据库是用来生成网络之中的边的
                For Each itr As psidev.XML.Entry In iteraction.Value.LoadXml(Of EntrySet).Entries
                    Network += From edge As psidev.XML.Interaction
                               In itr.InteractionList
                               Let edgeModel = __edgeModel(edge, Model, itr)
                               Let conf = Val(edgeModel.Value("ConfidenceList-likelihood")?.Value)
                               Where conf >= TrimConfidence
                               Select edgeModel
                Next
                Call Console.Write(".")
            Next

            Model.Edges = Network.WriteAddress

            Dim nodes = Model.Nodes.ToDictionary(Function(obj) obj.id,
                                                 Function(obj) New Value(Of Integer)(0))
            For Each edge As Edge In Model.Edges
                nodes(edge.source).Value += 1
                nodes(edge.target).Value += 1
            Next

            For Each node In nodes
                Model.GetNode(node.Key).AddAttribute("Degree", node.Value.Value, ATTR_VALUE_TYPE_REAL)
            Next

            If TrimDegree > -1 Then
                Dim LQuery As Integer() =
                    LinqAPI.Exec(Of Integer) <= From x In nodes
                                                Where x.Value.Value >= TrimDegree
                                                Select x.Key
                Model.Nodes =
                    LinqAPI.Exec(Of XGMML.Node) <= From node As XGMML.Node
                                                   In Model.Nodes
                                                   Where Array.IndexOf(LQuery, node.id) > -1
                                                   Select node
                LQuery =
                    LinqAPI.Exec(Of Integer) <= From x In nodes
                                                Where x.Value.Value < TrimDegree
                                                Select x.Key
                Model.Edges =
                    LinqAPI.Exec(Of Edge) <= From edge As Edge
                                             In Model.Edges.AsParallel
                                             Where Not edge.ContainsOneOfNode(LQuery)
                                             Select edge
            End If

            Return Model
        End Function

        Private Function __edgeModel(edge As psidev.XML.Interaction, Model As Graph, itr As Entry) As Edge
            Dim EdgeModel As Edge = New Edge
            Dim source As String = itr.GetInteractor(edge.ParticipantList.First.InteractorRef).Synonym
            Dim target As String = itr.GetInteractor(edge.ParticipantList.Last.InteractorRef).Synonym

            EdgeModel.source = Model.GetNode(source).id
            EdgeModel.target = Model.GetNode(target).id
            EdgeModel.Label = $"{source}::{target}"

            Dim attrs As New List(Of Attribute)
            attrs += New Attribute With {
                .Type = ATTR_VALUE_TYPE_REAL,
                .Name = $"{NameOf(edge.ConfidenceList)}-{edge.ConfidenceList.First.Unit.Names.shortLabel}",
                .Value = edge.ConfidenceList.First.value
            }

            Dim experiment = itr.GetExperiment(edge.ExperimentList.First.value)

            If Not experiment Is Nothing Then
                Dim name As String =
                    If(experiment.Names Is Nothing,
                    experiment.interactionDetectionMethod.Names.shortLabel,
                    experiment.Names.shortLabel)

                attrs += New Attribute With {
                    .Type = ATTR_VALUE_TYPE_STRING,
                    .Name = $"{NameOf(edge.ExperimentList)}-{name}",
                    .Value = experiment.Bibref.Xref.primaryRef.db & ": " & experiment.Bibref.Xref.primaryRef.id
                }
            End If

            EdgeModel.Attributes = attrs.ToArray

            Return EdgeModel
        End Function

        Private Function __attributes(GeneObject As GeneBrief) As Attribute()
            Dim List As New List(Of Attribute)

            List += New Attribute With {
                .Type = ATTR_VALUE_TYPE_STRING,
                .Name = NameOf(GeneBrief.Product),
                .Value = GeneObject.Product
            }
            List += New Attribute With {
                .Type = ATTR_VALUE_TYPE_STRING,
                .Name = NameOf(GeneBrief.PID),
                .Value = GeneObject.PID
            }
            List += New Attribute With {
                .Type = ATTR_VALUE_TYPE_STRING,
                .Name = NameOf(GeneBrief.COG),
                .Value = Regex.Replace(GeneObject.COG, "COG\d+", "", RegexOptions.IgnoreCase)
            }
            List += New Attribute With {
                .Type = ATTR_VALUE_TYPE_STRING,
                .Name = NameOf(NucleotideLocation.Strand),
                .Value = GeneObject.Location.Strand.ToString
            }

            Return List.ToArray
        End Function
    End Module
End Namespace
