Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView.XGMML
Imports LANS.SystemsBiology.DatabaseServices.StringDB
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView
Imports Microsoft.VisualBasic.Language

Namespace NetworkModel.StringDB

    ''' <summary>
    ''' 构建蛋白质互作网络的绘图模型
    ''' </summary>
    ''' 
    <[PackageNamespace]("String-Db.Interactions", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gmail.com")>
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
                               .Attributes = __attributes(GeneObject)}).AddHandle '使用PTT文件首先生成节点

            Dim Network As New List(Of Edge)

            For Each iteraction In stringDB.LoadSourceEntryList({"*.xml"})      ' string-db数据库是用来生成网络之中的边的
                For Each itr As MIF25.Nodes.Entry In iteraction.Value.LoadXml(Of MIF25.EntrySet).Entries
                    Network += From edge As MIF25.Nodes.Interaction
                               In itr.InteractionList
                               Let edgeModel = __edgeModel(edge, Model, itr)
                               Let conf = Val(edgeModel.Value("ConfidenceList-likelihood")?.Value)
                               Where conf >= TrimConfidence
                               Select edgeModel
                Next
                Call Console.Write(".")
            Next

            Model.Edges = Network.AddHandle

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

        Private Function __edgeModel(edge As MIF25.Nodes.Interaction, Model As Graph, itr As MIF25.Nodes.Entry) As Edge
            Dim EdgeModel As Edge = New Edge
            Dim source As String = itr.GetInteractor(edge.ParticipantList.First.InteractorRef).Synonym
            Dim target As String = itr.GetInteractor(edge.ParticipantList.Last.InteractorRef).Synonym

            EdgeModel.source = Model.GetNode(source).id
            EdgeModel.target = Model.GetNode(target).id
            EdgeModel.Label = $"{source}::{target}"

            Dim attrs As New List(Of Attribute)
            attrs += New Attribute With {
                .Type = ATTR_VALUE_TYPE_REAL,
                .Name = $"{NameOf(edge.ConfidenceList)}-{edge.ConfidenceList.First.Unit.Names.ShortLabel}",
                .Value = edge.ConfidenceList.First.value
            }

            Dim experiment = itr.GetExperiment(edge.ExperimentList.First.value)

            If Not experiment Is Nothing Then
                Dim name As String =
                    If(experiment.Names Is Nothing,
                    experiment.interactionDetectionMethod.Names.ShortLabel,
                    experiment.Names.ShortLabel)

                attrs += New Attribute With {
                    .Type = ATTR_VALUE_TYPE_STRING,
                    .Name = $"{NameOf(edge.ExperimentList)}-{name}",
                    .Value = experiment.Bibref.Xref.PrimaryReference.Db & ": " & experiment.Bibref.Xref.PrimaryReference.Id
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