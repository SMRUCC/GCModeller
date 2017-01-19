Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Data.STRING.StringDB.Tsv

Public Module Extensions

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="IDlist"><see cref="NamedValue(Of String).Name"/>为STRING之中的蛋白质的编号</param>
    ''' <param name="actions$"><see cref="LinkAction"/></param>
    ''' <returns></returns>
    <Extension>
    Public Function MatchNetwork(IDlist As IEnumerable(Of NamedValue(Of String)), actions$) As Network
        Dim idData As Dictionary(Of String, String) = IDlist _
            .ToDictionary(Function(x) x.Name,
                          Function(x) x.Value)
        Dim edges As New List(Of NetworkEdge)
        Dim nodes As New Dictionary(Of Node)
        Dim testAdd As Action(Of String) =
            Sub(id$)
                If Not nodes.ContainsKey(id) Then
                    nodes += New Node With {
                        .ID = id,
                        .NodeType = "protein",
                        .Properties = New Dictionary(Of String, String) From {
                            {"geneID", idData(.ID)}
                        }
                    }
                End If
            End Sub

        For Each link As LinkAction In LinkAction.LoadText(actions)
            If Not idData.ContainsKey(link.item_id_a) OrElse
                Not idData.ContainsKey(link.item_id_b) Then
                ' DO NOTHING
            Else

                Call testAdd(link.item_id_a)
                Call testAdd(link.item_id_b)

            End If
        Next

        Return New Network With {
            .Edges = edges,
            .Nodes = nodes.Values.ToArray
        }
    End Function
End Module
