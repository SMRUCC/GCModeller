Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.ConvexHull
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
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
                        Dim uniprotID$()

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
                                uniprotID = .accessions
                            End With
                        Else
                            KO = {}
                            pathways = {}
                            uniprotID = {}
                        End If

                        Dim data As New Dictionary(Of String, String)

                        data!KO = KO.JoinBy("|")
                        data!uniprotID = uniprotID.JoinBy("|")

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
    ''' <param name="model">是以STRING的蛋白编号为标识符的，所以在这里还需要使用uniprot的数据进行转换</param>
    ''' <param name="DEGs">uniprot蛋白编号</param>
    ''' <param name="colors"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function RenderDEGsColor(model As NetGraph,
                                    DEGs As (up As String(), down As String()),
                                    colors As (up$, down$),
                                    Optional nonDEPcolor$ = "gray") As NetGraph

        For Each node As NetNode In model.Nodes
            Dim uniprot = node!uniprotID

            If Not uniprot.StringEmpty Then
                Dim notHit As Boolean = False

                For Each id In uniprot.Split("|"c)
                    notHit = False

                    If DEGs.up.IndexOf(id) > -1 Then
                        node!color = colors.up
                    ElseIf DEGs.down.IndexOf(id) > -1 Then
                        node!color = colors.down
                    Else
                        ' not hit
                        notHit = True
                    End If

                    If Not notHit Then
                        Exit For
                    End If
                Next

                If notHit Then
                    node!color = nonDEPcolor
                End If
            Else
                node!color = nonDEPcolor
            End If
        Next

        Return model
    End Function

    ''' <summary>
    ''' 这个函数需要编写一个网络布局生成函数的参数配置文件
    ''' </summary>
    ''' <param name="model"></param>
    ''' <returns></returns>
    <Extension>
    Public Function VisualizeKEGG(model As NetGraph, Optional colorSchema$ = "Set1:c10") As Image
        Dim graph = model.CreateGraph(nodeColor:=Function(n) (n!color).GetBrush)
        Dim parameters As ForceDirectedArgs = Layouts.Parameters.Load

        ' 生成layout信息               
        Call graph.doRandomLayout
        Call graph.doForceLayout(showProgress:=True, parameters:=parameters)

        Dim nodeGroups = model.Nodes _
            .Select(Function(n)
                        Return Strings _
                            .Split(n.NodeType, delimiter) _
                            .Select(Function(path) (path, n))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(x) x.Item1) _
            .ToDictionary(Function(g) g.Key,
                          Function(nodes)
                              Return nodes.Select(Function(x) x.Item2).ToArray
                          End Function)
        Dim nodePoints As Dictionary(Of Graph.Node, Point) = Nothing
        Dim colors As Color() = GDIColors.ChartColors

        Call $"{colors.Length} colors --> {nodeGroups.Count} KEGG pathways".__DEBUG_ECHO

        Using g As Graphics2D = graph _
            .DrawImage(canvasSize:="5000,4500", scale:=3, nodePoints:=nodePoints) _
            .AsGDIImage _
            .CreateCanvas2D(directAccess:=True)

            For Each pathway In nodeGroups.SeqIterator
                Dim nodes = (+pathway).Value
                Dim name$ = (+pathway).Key
                Dim polygon As Point() = nodePoints.Selects(nodes)

                polygon = ConvexHull.GrahamScan(polygon)

                With colors(pathway)
                    Dim pen As New Pen(.ref, 10)
                    Dim fill As New SolidBrush(Color.FromArgb(40, .ref))

                    Call g.DrawPolygon(pen, polygon)
                    Call g.FillPolygon(fill, polygon)
                End With
            Next

            Return g.ImageResource
        End Using
    End Function
End Module
