Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module PathwayMap

    ''' <summary>
    ''' 将KEGG的参考代谢途径的注释信息和代谢反应的连接信息保存至rda数据集之中
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <param name="connection"></param>
    ''' <param name="rda$"></param>
    ''' <returns></returns>
    Public Function Save(maps As IEnumerable(Of Map), connection As IEnumerable(Of Reaction), rda$) As Boolean
        Dim pathwayList$ = "pathway.list"
        Dim pathwayAssign$ = "pathway.assigns"

        SyncLock R
            With R

                Dim assign As New Dictionary(Of String, List(Of String))

                .call = $"{pathwayList}   <- list();"
                .call = $"{pathwayAssign} <- list();"

                For Each pathway As Map In maps
                    Dim map As String = App.NextTempName
                    Dim groups = pathway _
                        .Areas _
                        .GroupBy(Function(x) x.Type) _
                        .ToDictionary(Function(g) g.Key,
                                      Function(list)
                                          Return list.ToArray
                                      End Function)

                    Dim compounds$, genes$
                    Dim compoundList$()
                    Dim geneList$()

                    With groups
                        compoundList = !Compound _
                            .Select(Function(a) a.IDVector) _
                            .IteratesALL _
                            .ToArray
                        geneList = !Gene _
                            .Select(Function(a) a.IDVector) _
                            .IteratesALL _
                            .ToArray

                        genes = base.c(geneList, stringVector:=True)
                        compounds = base.c(compoundList, stringVector:=True)
                    End With

                    .call = $"{map}           <- list();"
                    .call = $"{map}$compounds <- {compounds};"
                    .call = $"{map}$genes     <- {genes};"
                    .call = $"{map}$ID        <- {Rstring(pathway.ID)};"
                    .call = $"{map}$Name      <- {Rstring(pathway.Name)};"
                    .call = $"{pathwayList}[[""{pathway.ID}""]] <- {map};"

                    For Each ID As String In compounds.AsList + genes
                        If Not assign.ContainsKey(ID) Then
                            Call assign.Add(ID, New List(Of String))
                        End If

                        Call assign(ID).Add(pathway.ID)
                    Next
                Next

                For Each entity In assign
                    .call = $"{pathwayAssign}[[{Rstring(entity.Key)}]] <- {base.c(entity.Value, stringVector:=True)};"
                Next
            End With
        End SyncLock

        Call base.save({pathwayList, pathwayAssign}, file:=rda)

        Return True
    End Function
End Module
