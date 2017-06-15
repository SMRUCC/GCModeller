Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.STRING
Imports NetGraph = Microsoft.VisualBasic.Data.visualize.Network.FileStream.Network
Imports NetNode = Microsoft.VisualBasic.Data.visualize.Network.FileStream.Node

''' <summary>
''' 功能富集网络
''' </summary>
Public Module FunctionalEnrichmentPlot

    ''' <summary>
    ''' Using string-db ID as the uniprot data index key
    ''' </summary>
    ''' <param name="uniprot"></param>
    ''' <returns></returns>
    <Extension> Public Function StringUniprot(uniprot As UniprotXML) As Dictionary(Of String, entry)
        Return uniprot _
            .entries _
            .Where(Function(protein) protein.Xrefs.ContainsKey(InteractExports.STRING)) _
            .Select(Function(protein) protein.Xrefs(InteractExports.STRING) _
            .Select(Function(id) (id, protein))) _
            .IteratesALL _
            .GroupBy(Function(id) id.Item1.id) _
            .ToDictionary(Function(x) x.Key,
                          Function(proteins) proteins.First.Item2)
    End Function

    Const delimiter$ = " === "

    <Extension>
    Public Function BuildModel(interacts As IEnumerable(Of InteractExports), uniprot As Dictionary(Of String, entry)) As NetGraph
        Dim KOCatagory = PathwayMapping.DefaultKOTable
        Dim nodes = interacts _
            .NodesID _
            .Select(Function(stringID$)
                        Dim pathways$()
                        Dim KO$()

                        If uniprot.ContainsKey(stringID) Then
                            With uniprot(stringID)
                                KO = .Xrefs.TryGetValue("KO") _
                                     .SafeQuery _
                                     .Select(Function(x) x.id) _
                                     .ToArray
                                pathways = KO _
                                    .Where(Function(ID) KOCatagory.ContainsKey(ID)) _
                                    .Select(Function(ID) KOCatagory(ID).Parent.Description) _
                                    .Distinct _
                                    .ToArray
                            End With
                        Else
                            KO = {}
                            pathways = {}
                        End If

                        Dim data As New Dictionary(Of String, String)

                        data!KO = KO.JoinBy("|")

                        Return New NetNode(stringID) With {
                            .NodeType = pathways.JoinBy(FunctionalEnrichmentPlot.delimiter),
                            .Properties = data
                        }
                    End Function) _
            .ToDictionary
        Dim links = interacts _
            .Select(Function(l)
                        Dim a = nodes(l.node1_external_id)
                        Dim b = nodes(l.node2_external_id)
                        Dim pa = Strings.Split(a.NodeType, FunctionalEnrichmentPlot.delimiter)
                        Dim pb = Strings.Split(b.NodeType, FunctionalEnrichmentPlot.delimiter)
                        Dim type$

                        If pa.Where(Function(pathway) pb.IndexOf(pathway) > -1).Count > 0 Then
                            type = "pathway_internal"
                        ElseIf pa.IsNullOrEmpty AndAlso pb.IsNullOrEmpty Then
                            type = "Unknown"
                        Else
                            type = "pathway_outbounds"
                        End If

                        Return New NetworkEdge With {
                            .FromNode = l.node1_external_id,
                            .ToNode = l.node2_external_id,
                            .Interaction = type,
                            .value = l.combined_score
                        }
                    End Function).ToArray

        Return New NetGraph(links, nodes.Values)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="DEGs">uniprot蛋白编号</param>
    ''' <param name="colors"></param>
    ''' <param name="uniprotXML">
    ''' 因为<paramref name="model"/>是以STRING的蛋白编号为标识符的，所以在这里还需要使用uniprot的数据进行转换
    ''' </param>
    ''' <returns></returns>
    Public Function RenderDEGsColor(model As NetGraph,
                                    DEGs As (up As String(), down As String()),
                                    colors As (up$, down$),
                                    uniprotXML As UniprotXML) As NetGraph

    End Function

    <Extension>
    Public Function VisualizeKEGG(model As NetGraph, up$()) As Image

    End Function
End Module
