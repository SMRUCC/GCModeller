Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Module PathwayMapNetwork

    Const delimiter$ = "|"

    ''' <summary>
    ''' 这个函数所产生的模型是以代谢途径为主体对象的
    ''' </summary>
    ''' <param name="br08901$"></param>
    ''' <returns></returns>
    Public Function BuildModel(br08901$) As NetworkTables
        Dim nodes As New List(Of Node)

        For Each Xml As String In ls - l - r - "*.XML" <= br08901
            Dim pathwayMap As PathwayMap = Xml.LoadXml(Of PathwayMap)

            nodes += New Node(pathwayMap.EntryId) With {
                .NodeType = pathwayMap.Brite?.Class,
                .Properties = New Dictionary(Of String, String) From {
                    {"KO", pathwayMap.KEGGOrthology _
                        .SafeQuery _
                        .Select(Function(x) x.Key) _
                        .JoinBy(PathwayMapNetwork.delimiter)
                    },
                    {"KO.counts", pathwayMap.KEGGOrthology?.Length},
                    {"pathway.name", pathwayMap.Name} ' 直接使用name作为键名会和cytoscape网络模型之中的name产生冲突
                }
            }
        Next

        Dim edges As New List(Of NetworkEdge)

        For Each a As Node In nodes
            Dim KO As Index(Of String) = Strings.Split(a!KO, delimiter).Indexing

            For Each b As Node In nodes.Where(Function(node) node.ID <> a.ID)
                Dim kb = Strings.Split(b!KO, delimiter)
                Dim n = kb.Where(Function(id) KO(id) > -1).AsList
                Dim type$

                If a.NodeType = b.NodeType Then
                    type = "module internal"
                Else
                    type = "module outbounds"
                End If

                If Not n = 0 Then
                    edges += New NetworkEdge With {
                        .Interaction = type,
                        .FromNode = a.ID,
                        .ToNode = b.ID,
                        .value = n.Count,
                        .Properties = New Dictionary(Of String, String) From {
                            {"intersets", n.JoinBy(delimiter)}
                        }
                    }
                End If
            Next
        Next

        Dim ranks As Vector = edges _
            .Select(Function(x) x.value) _
            .RangeTransform("0,100") _
            .AsVector

        edges = edges(Which.IsTrue(ranks >= 3))

        Return New NetworkTables(edges, nodes)
    End Function
End Module
