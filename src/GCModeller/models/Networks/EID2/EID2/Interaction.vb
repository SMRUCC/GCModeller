Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
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
    Public Shared Function BuildSharedPathogens(it As IEnumerable(Of Interaction)) As NetworkTables
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
                                      {"pathogens", pathogens.GetJson}
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

        For Each a In carriers.Values
            For Each b In carriers.Values.Where(Function(c) Not c.node Is a.node)

                If (sharedCount = (sharedPathogens = a.pathogens.Intersect(b.pathogens).ToArray).Length) > 0 Then
                    network += New NetworkEdge With {
                        .FromNode = a.node.ID,
                        .ToNode = b.node.ID,
                        .value = sharedCount,
                        .Properties = New Dictionary(Of String, String) From {
                            {"sharedPathogens", sharedPathogens.Value.GetJson}
                        }
                    }
                End If
            Next
        Next

        Return New NetworkTables(nodes, network)
    End Function
End Class