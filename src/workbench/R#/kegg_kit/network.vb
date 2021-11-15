Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

<Package("network")>
Module network

    <ExportAPI("fromCompounds")>
    Public Function fromCompoundId(compoundsId As String(), graph As Reaction(), Optional compounds As Compound() = Nothing) As NetworkGraph
        ' BuildModel(br08901 As IEnumerable(Of ReactionTable), compounds As IEnumerable(Of NamedValue(Of String)),
        Dim template As ReactionTable() = ReactionTable.Load(graph)
        Dim cid As NamedValue(Of String)()

        If compounds.IsNullOrEmpty Then
            cid = compoundsId _
                .Select(Function(c)
                            Return New NamedValue(Of String)(c, c, c)
                        End Function) _
                .ToArray
        Else
            With compounds _
                .GroupBy(Function(c) c.entry) _
                .ToDictionary(Function(c) c.Key,
                              Function(list)
                                  Return list.First
                              End Function)

                cid = compoundsId _
                    .Select(Function(c)
                                Dim model As Compound = .TryGetValue(c)
                                Dim name As String = If(model Is Nothing, c, If(model.commonNames.FirstOrDefault, c))

                                Return New NamedValue(Of String)(c, name, c)
                            End Function) _
                    .ToArray
            End With
        End If

        Return template.BuildModel(
            compounds:=cid,
            extended:=False,
            enzymaticRelated:=False,
            ignoresCommonList:=False,
            enzymeBridged:=True
        )
    End Function
End Module
