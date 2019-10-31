#Region "Microsoft.VisualBasic::0a78b43a760487236df5b222f7294371, RDotNET.Extensions.GCModeller\KEGG\PathwayMap.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module PathwayMap
    ' 
    '     Function: buildAssign, SaveRda
    ' 
    '     Sub: append
    ' 
    ' /********************************************************************************/

#End Region

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
    <Extension> Public Function SaveRda(maps As IEnumerable(Of Map), drugs As Dictionary(Of String, String()), rda$) As Boolean
        Dim assignGenes As New Dictionary(Of String, List(Of String))
        Dim assignCompounds As New Dictionary(Of String, List(Of String))
        Dim assignReactions As New Dictionary(Of String, List(Of String))

        ' Connect to R server, and gets the memory pointer for read/write
        SyncLock R
            With R

                .call = $"{pathwayList}   <- list();"
                .call = $"{pathwayAssign} <- list();"

                For Each pathway As Map In maps
                    Dim map As String = App.NextTempName
                    Dim groups = pathway _
                        .shapes _
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

                        Dim replaceList As New List(Of String)

                        ' 可能会有药物的编号，将他们转换为Compound编号
                        For Each cpdID As String In compoundList
                            If drugs.ContainsKey(cpdID) Then
                                With drugs(cpdID)
                                    If Not .IsNullOrEmpty Then
                                        replaceList += .ByRef
                                    End If
                                End With
                            Else
                                replaceList += cpdID
                            End If
                        Next

                        compoundList = replaceList
                        reactions = base.c(reactionList, stringVector:=True)
                        genes = base.c(geneList, stringVector:=True)
                        compounds = base.c(compoundList, stringVector:=True)
                    End With

                    ' Create dictionary table in R language, and then set the result value for sevral keys.
                    .call = $"{map}           <- list();"
                    .call = $"{map}$compounds <- {compounds};"
                    .call = $"{map}$genes     <- {genes};"
                    .call = $"{map}$reactions <- {reactions};"
                    .call = $"{map}$ID        <- {Rstring(pathway.ID)};"
                    .call = $"{map}$Name      <- {Rstring(pathway.Name)};"

                    ' add the map dictionary table into a larger dictionary.
                    .call = $"{pathwayList}[[{Rstring(pathway.ID)}]] <- {map};"

                    Call assignCompounds.append(compoundList, pathway.ID)
                    Call assignGenes.append(geneList, pathway.ID)
                    Call assignReactions.append(reactionList, pathway.ID)
                Next

                ' At last create pathway assign tuple.
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
