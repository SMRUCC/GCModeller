Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' Cargoes are found in/on Carriers. Cargoes are often pathogens and carriers are often hosts
''' </summary>
Public Class Interaction

    Public Property Cargo As String
    Public Property Carrier As String

    ''' <summary>
    ''' 返回来的网络对象为绘制``Database of host-pathogen and related species interactions, and their global distribution`` Figure3的网络数据模型
    ''' </summary>
    ''' <param name="it"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Shared pathogens between vertebrate species in Data Citation 1. Each node presents a vertebrate
    ''' species. The size of the node Is in proportion to the number of unique pathogen species found to interact with
    ''' it. Edges between two nodes indicate they both share at least one possible pathogen species. The weight
    ''' (thickness) of the edges Is in proportion to the number of possible pathogen species shared between the two
    ''' nodes. The location of each particular node corresponds to the size of all nodes in the graph And the weight of
    ''' the edges linking this particular node With other nodes.
    ''' </remarks>
    Public Shared Function BuildSharedPathogens(it As IEnumerable(Of Interaction), Optional cut% = 0) As NetworkTables
        ' 查看一种宿主会有多少种会感染的病原菌
        Dim carriers = it _
            .GroupBy(Function(i) i.Carrier) _
            .ToDictionary(Function(c) c.Key,
                          Function(g)
                              Dim pathogens$() = g _
                                  .Select(Function(c) c.Cargo) _
                                  .Distinct _
                                  .ToArray
                              Dim node As New Node With {
                                  .ID = g.Key,
                                  .NodeType = NameOf(Carrier),
                                  .Properties = New Dictionary(Of String, String) From {
                                      {"pathogens", pathogens.GetJson},
                                      {"n", pathogens.Length}
                                  }
                              }

                              Return (pathogens:=pathogens.Indexing, node:=node)
                          End Function)
        Dim network As New List(Of NetworkEdge)
        Dim nodes = carriers _
            .Values _
            .Select(Function(c) c.node) _
            .ToArray
        Dim sharedCount As int = 0
        Dim sharedPathogens As New Value(Of String())
        Dim color$
        Dim colors$() = Designer _
            .GetColors("scibasic.category31()", nodes.Length) _
            .HTMLColors _
            .ToArray

        For i As Integer = 0 To colors.Length - 1
            nodes(i)!color = colors(i)
        Next

        For Each a In carriers.Values
            For Each b In carriers.Values.Where(Function(c) Not c.node Is a.node)

                If (sharedCount = (sharedPathogens = a.pathogens.Intersect(b.pathogens).ToArray).Length) > cut Then
                    If Val(a.node!n) > Val(b.node!n) Then
                        color = a.node!color
                    Else
                        color = b.node!color
                    End If

                    network += New NetworkEdge With {
                        .FromNode = a.node.ID,
                        .ToNode = b.node.ID,
                        .value = sharedCount,
                        .Properties = New Dictionary(Of String, String) From {
                            {"sharedPathogens", sharedPathogens.Value.GetJson},
                            {"color", color}
                        }
                    }
                End If
            Next
        Next

        Return New NetworkTables(nodes, network)
    End Function

    Public Shared Function Load(path$, Optional cargo$ = "", Optional carrier$ = "") As Interaction()
        Dim maps As New Dictionary(Of String, String)

        If Not cargo.StringEmpty Then
            maps(cargo) = NameOf(Interaction.Cargo)
        End If
        If Not carrier.StringEmpty Then
            maps(carrier) = NameOf(Interaction.Carrier)
        End If

        Return path.LoadCsv(Of Interaction)(maps:=maps)
    End Function
End Class