Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace ReactionNetwork

    Public Module Styles

        <Extension>
        Public Sub AssignNodeClassFromPathwayMaps(net As NetworkGraph, maps As Map(), Optional delimiter$ = FunctionalNetwork.Delimiter)
            ' 生成了 compound => maps 的包含关系
            Dim compoundIndex As Dictionary(Of String, String()) = maps _
                .Select(Function(pathway)
                            Return pathway.shapes _
                                .Select(Function(a) a.IDVector) _
                                .IteratesALL _
                                .Where(Function(id) id.IsPattern("C\d+")) _
                                .Select(Function(id) (id, pathway))
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(link) link.Item1) _
                .ToDictionary(Function(compound) compound.Key,
                              Function(mapList)
                                  Return mapList _
                                      .Select(Function(l) l.Item2) _
                                      .Select(Function(map) $"[{map.id}] {map.Name}") _
                                      .ToArray
                              End Function)

            For Each node As Node In net.vertex
                If compoundIndex.ContainsKey(node.ID) Then
                    node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = compoundIndex(node.ID).JoinBy(delimiter)
                Else
                    node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "KEGG Compound"
                End If
            Next
        End Sub

        ''' <summary>
        ''' 将代谢物网络之中的reaction编号转换为pathway的名称
        ''' </summary>
        ''' <param name="net"></param>
        ''' <param name="ko0001"></param>
        <Extension>
        Public Sub AssignNodeClassFromReactionLinks(net As NetworkGraph, ko0001 As KOLinks(), Optional delimiter$ = FunctionalNetwork.Delimiter)
            ' 生成了reaction => pathway的对应关系
            Dim index As Dictionary(Of String, KOLinks()) = ko0001 _
                .Where(Function(ko) Not ko.reactions.IsNullOrEmpty) _
                .Select(Function(ko) ko.reactions.Select(Function(rn) (rn, ko))) _
                .IteratesALL _
                .GroupBy(Function(id) id.Item1) _
                .ToDictionary(Function(id) id.Key,
                              Function(rn)
                                  Return rn.Select(Function(x) x.Item2).ToArray
                              End Function)

            For Each node As Node In net.vertex
                Dim [class] As New List(Of String)
                Dim rn$() = Strings.Split(node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE), delimiter)

                For Each id In rn
                    If index.ContainsKey(id) Then
                        [class] += index(id) _
                        .Select(Function(ko) ko.pathways.Select(Function(x) x.text)) _
                        .IteratesALL _
                        .Distinct
                    End If
                Next

                [class] = [class].Distinct.AsList

                If [class].IsNullOrEmpty Then
                    node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "KEGG Compound"
                Else
                    node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = [class].JoinBy(delimiter)
                End If
            Next
        End Sub
    End Module
End Namespace