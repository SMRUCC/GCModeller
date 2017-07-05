Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.ComponentModel.Collection

Public Module PathwayMapNetwork

    Const delimiter$ = "|"

    Public Function BuildModel(br08901$) As NetworkTables
        Dim nodes As New List(Of Node)

        For Each Xml As String In ls - l - r - "*.XML" <= br08901
            Dim pathwayMap As PathwayMap = Xml.LoadXml(Of PathwayMap)

            nodes += New Node(pathwayMap.EntryId) With {
                .Properties = New Dictionary(Of String, String) From {
                    {"KO", pathwayMap.KEGGOrthology _
                        .SafeQuery _
                        .Select(Function(x) x.Key) _
                        .JoinBy(PathwayMapNetwork.delimiter)
                    },
                    {"name", pathwayMap.Name}
                }
            }
        Next

        Dim edges As New List(Of NetworkEdge)

        For Each a As Node In nodes
            Dim KO As Index(Of String) = Strings.Split(a!KO).Indexing

            For Each b As Node In nodes
                Dim kb = Strings.Split(b!KO)
                Dim n = kb.Where(Function(id) KO(id) > -1).AsList

                If Not n = 0 Then
                    edges += New NetworkEdge With {
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

        Return New NetworkTables(edges, nodes)
    End Function
End Module
