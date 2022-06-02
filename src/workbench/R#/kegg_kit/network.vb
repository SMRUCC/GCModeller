Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

<Package("network")>
Module network

    <ExportAPI("fromCompounds")>
    Public Function fromCompoundId(compoundsId As String(), graph As Reaction(),
                                   Optional compounds As CompoundRepository = Nothing,
                                   Optional enzymeBridged As Boolean = True) As NetworkGraph

        ' BuildModel(br08901 As IEnumerable(Of ReactionTable), compounds As IEnumerable(Of NamedValue(Of String)),
        Dim template As ReactionTable() = ReactionTable.Load(graph).ToArray
        Dim cid As NamedValue(Of String)()

        If compounds Is Nothing Then
            cid = compoundsId _
                .Select(Function(c)
                            Return New NamedValue(Of String)(c, c, c)
                        End Function) _
                .ToArray
        Else
            cid = compoundsId _
                .Select(Function(c)
                            Dim model As Compound = compounds.GetByKey(c)
                            Dim name As String = If(model Is Nothing, c, If(model.commonNames.FirstOrDefault, c))

                            Return New NamedValue(Of String)(c, name, c)
                        End Function) _
                .ToArray
        End If

        Return template.BuildModel(
            compounds:=cid,
            extended:=False,
            enzymaticRelated:=False,
            ignoresCommonList:=False,
            enzymeBridged:=enzymeBridged
        )
    End Function

    ''' <summary>
    ''' assign kegg class to the graph nodes
    ''' </summary>
    ''' <param name="g">
    ''' a network graph data model, with nodes label id must 
    ''' be the kegg pathway id and compounds id or kegg KO id
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("assignKeggClass")>
    Public Function assignKeggClass(g As NetworkGraph) As NetworkGraph
        ' first assign the class id to the pathway id node

    End Function
End Module
