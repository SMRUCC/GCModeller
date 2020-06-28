Imports System.Runtime.CompilerServices
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork
Imports VB = Microsoft.VisualBasic.Language.Runtime

Public Module Maps

    <Extension>
    Public Sub WriteMaps(maps As IEnumerable(Of Map), saveRds As String)
        Dim mapList = base.lapply(
            x:=maps,
            FUN:=Function(map)
                     With New VB
                         Return base.list(
                            !id = map.id,
                            !name = map.Name,
                            !blank = map.PathwayImage.TrimNewLine.Replace(" ", ""),
                            !KO = map.GetMembers.Where(Function(id) id.IsPattern("K\d+")).Distinct.ToArray,
                            !reactions = map.GetMembers.Where(Function(id) id.IsPattern("R\d+")).Distinct.ToArray,
                            !compounds = map.GetMembers.Where(Function(id) id.IsPattern("[CGD]\d+")).Distinct.ToArray,
                            !shapes = "&" & base.lapply(
                                x:=map.shapes,
                                key:=Function(a) a.href,
                                FUN:=Function(a)
                                         Return base.list(
                                            !shape = a.shape,
                                            !href = a.href,
                                            !title = a.title,
                                            !type = a.Type.ToLower,
                                            !geo2D = a.coords.Split(","c).Select(AddressOf Val).ToArray,
                                            !objects = a.IDVector
                                         )
                                     End Function
                             )
                         )
                     End With
                 End Function)

        Call base.saveRDS(mapList, file:=saveRds)
    End Sub

    <Extension>
    Public Sub WriteReactions(reactions As IEnumerable(Of ReactionTable), saveRDS As String)
        Dim reactionList = base.lapply(
            x:=reactions,
            FUN:=Function(reaction)
                     With New VB
                         Return base.list(
                            !id = reaction.entry,
                            !name = reaction.name,
                            !formula = reaction.definition,
                            !EC = reaction.EC,
                            !KO = reaction.KO,
                            !geneNames = reaction.geneNames,
                            !substrates = reaction.substrates,
                            !products = reaction.products
                         )
                     End With
                 End Function)

        Call base.saveRDS(reactionList, file:=saveRDS)
    End Sub
End Module
