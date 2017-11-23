Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports IDMap = System.Collections.Generic.KeyValuePair(Of String, Microsoft.VisualBasic.Language.List(Of String))

Public Module PathwayMap

    Const pathwayAssign$ = "pathway.assigns"
    Const pathwayList$ = "pathway.list"

    ''' <summary>
    ''' 将KEGG的参考代谢途径的注释信息和代谢反应的连接信息保存至``*.rda``数据集之中
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <param name="rda$"></param>
    ''' <returns></returns>
    <Extension> Public Function SaveRda(maps As IEnumerable(Of Map), rda$) As Boolean
        Dim assignGenes As New Dictionary(Of String, List(Of String))
        Dim assignCompounds As New Dictionary(Of String, List(Of String))
        Dim assignReactions As New Dictionary(Of String, List(Of String))

        ' Connect to R server
        SyncLock R
            With R

                .call = $"{pathwayList}   <- list();"
                .call = $"{pathwayAssign} <- list();"

                For Each pathway As Map In maps
                    Dim map As String = App.NextTempName
                    Dim groups = pathway _
                        .Areas _
                        .GroupBy(Function(x)
                                     Try
                                         Return x.Type
                                     Catch ex As Exception
                                         Return "Compound"
                                     End Try
                                 End Function) _
                        .ToDictionary(Function(g) g.Key,
                                      Function(list)
                                          Return list.ToArray
                                      End Function)

                    Dim compounds$, genes$, reactions$
                    Dim compoundList$()
                    Dim geneList$()
                    Dim reactionList$()

                    With groups

                        If .ContainsKey("Compound") Then
                            compoundList = !Compound _
                                .Select(Function(a) a.IDVector) _
                                .IteratesALL _
                                .ToArray
                        Else
                            compoundList = {}
                        End If

                        If .ContainsKey("Gene") Then
                            geneList = !Gene _
                                .Select(Function(a) a.IDVector) _
                                .IteratesALL _
                                .ToArray
                        Else
                            geneList = {}
                        End If

                        If .ContainsKey("Reaction") Then
                            reactionList = !Reaction _
                               .Select(Function(a) a.IDVector) _
                               .IteratesALL _
                               .ToArray
                        Else
                            reactionList = {}
                        End If

                        reactions = base.c(reactionList, stringVector:=True)
                        genes = base.c(geneList, stringVector:=True)
                        compounds = base.c(compoundList, stringVector:=True)
                    End With

                    .call = $"{map}           <- list();"
                    .call = $"{map}$compounds <- {compounds};"
                    .call = $"{map}$genes     <- {genes};"
                    .call = $"{map}$reactions <- {reactions};"
                    .call = $"{map}$ID        <- {Rstring(pathway.ID)};"
                    .call = $"{map}$Name      <- {Rstring(pathway.Name)};"

                    .call = $"{pathwayList}[[{Rstring(pathway.ID)}]] <- {map};"

                    Call assignCompounds.append(compoundList, pathway.ID)
                    Call assignGenes.append(geneList, pathway.ID)
                    Call assignReactions.append(reactionList, pathway.ID)
                Next

                .call = $"{pathwayAssign} <- list(
                    Compound = {assignCompounds.buildAssign("Compound")},
                    Gene     = {assignGenes.buildAssign("Gene")},
                    Reaction = {assignReactions.buildAssign("Reaction")}
                );"
            End With
        End SyncLock

        Call base.save({pathwayList, pathwayAssign}, file:=rda)

        Return True
    End Function

    <Extension>
    Private Function buildAssign(assign As Dictionary(Of String, List(Of String)), slot$) As String
        Dim mapID$, members$

        SyncLock R
            With R

                .call = $"{slot} <- list();"

                For Each entity As IDMap In assign
                    With entity
                        mapID = Rstring(.Key)
                        members = base.c(.Value, stringVector:=True)
                    End With

                    .call = $"{slot}[[{mapID}]] <- {members};"
                Next
            End With
        End SyncLock

        Return slot
    End Function

    <Extension>
    Private Sub append(assign As Dictionary(Of String, List(Of String)), list$(), pathwayID$)
        For Each ID As String In list
            If Not assign.ContainsKey(ID) Then
                Call assign.Add(ID, New List(Of String))
            End If

            Call assign(ID).Add(pathwayID)
        Next
    End Sub
End Module
