Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace PathwayMaps

    ''' <summary>
    ''' 因为同一种代谢物或者代谢反应或者酶分子可能会出现在多个代谢途径之中
    ''' 则这个模块就是通过将目标类型在代谢途径中的归属通过覆盖度进行简单计算
    ''' </summary>
    Public Module MapAssignment

        ''' <summary>
        ''' Biological object map assignment by simple coverage calculation
        ''' </summary>
        ''' <param name="objects">实际的目标集合</param>
        ''' <param name="maps">
        ''' Pathway map作为<paramref name="objects"/> cluster的定义
        ''' </param>
        ''' <param name="coverageCutoff">
        ''' coverage结果值低于这个阈值的代谢通路将不会被抛出，这个参数默认为零，即不进行coverage相关的筛选
        ''' </param>
        ''' <returns></returns>
        Public Iterator Function MapAssignmentByCoverage(objects As IEnumerable(Of String),
                                                         maps As IEnumerable(Of NamedCollection(Of String)),
                                                         Optional includesUnknown As Boolean = False,
                                                         Optional coverageCutoff As Double = 0,
                                                         Optional topMaps As Index(Of String) = Nothing) As IEnumerable(Of NamedCollection(Of String))

            Dim objectPool As Index(Of String) = objects.Distinct.Indexing
            Dim mapList As Dictionary(Of String, String()) =
                maps.ToDictionary(Function(map) map.name,
                                  Function(map)
                                      Return map.ToArray
                                  End Function)

            If topMaps Is Nothing Then
                topMaps = New Index(Of String)
            End If

            Do While mapList.Count > 0
                Dim top As (Map As KeyValuePair(Of String, String()), coverage As Double)

                If topMaps.Count = 0 Then
                    Dim coverages = From map
                                    In mapList
                                    Let coverage As Double = objectPool _
                                        .Intersect(collection:=map.Value) _
                                        .Count
                                    Let numberOfGenes As Integer = map.Value.Length
                                    Select result = (map:=map, coverage:=coverage / (numberOfGenes ^ 2)) ' 尽量规模小的代谢图优先分配，这样子作图的时候更加美观
                                    Order By result.coverage Descending

                    top = coverages.First
                Else
                    Dim orderMap = mapList _
                        .Where(Function(m) topMaps(m.Key) > -1) _
                        .OrderBy(Function(m) topMaps(m.Key)) _
                        .First

                    top = (orderMap, objectPool.Intersect(collection:=orderMap.Value).Count / (orderMap.Value.Length ^ 2))
                    topMaps.Delete(orderMap.Key)

                    Call $"{orderMap.Key}".__INFO_ECHO
                End If

                Dim intersectObjects As String() = objectPool _
                    .Intersect(collection:=top.map.Value) _
                    .ToArray

                ' 最高的coverage已经达到阈值了
                ' 则后面已经不会再出现比较高的coverage结果了
                ' 结束计算循环
                If (top.coverage * top.map.Value.Length) < coverageCutoff Then
                    Exit Do
                End If

                Call mapList.Remove(top.map.Key)
                Call intersectObjects.DoEach(AddressOf objectPool.Delete)

                Yield New NamedCollection(Of String) With {
                    .name = top.map.Key,
                    .value = intersectObjects,
                    .description = top.coverage
                }
            Loop

            If includesUnknown AndAlso objectPool.Count > 0 Then
                ' unknowns
                ' not includes in any given maps data
                Yield New NamedCollection(Of String) With {
                    .name = "unknown",
                    .value = objectPool.Objects
                }
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function CompoundsMapAssignment(maps As IEnumerable(Of Map),
                                               compoundIds As IEnumerable(Of String),
                                               Optional includesUnknown As Boolean = False) As IEnumerable(Of NamedCollection(Of String))
            Return maps _
                .Select(AddressOf BiologicalObjectCluster.CompoundsMap) _
                .DoCall(Function(assign)
                            Return MapAssignmentByCoverage(compoundIds, assign, includesUnknown)
                        End Function)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ReactionsMapAssignment(maps As IEnumerable(Of Map),
                                               reactionIds As IEnumerable(Of String),
                                               Optional includesUnknown As Boolean = False) As IEnumerable(Of NamedCollection(Of String))
            Return maps _
                .Select(AddressOf BiologicalObjectCluster.ReactionMap) _
                .DoCall(Function(assign)
                            Return MapAssignmentByCoverage(reactionIds, assign, includesUnknown)
                        End Function)
        End Function
    End Module
End Namespace