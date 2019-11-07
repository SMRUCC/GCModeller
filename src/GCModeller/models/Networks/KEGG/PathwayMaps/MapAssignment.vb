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
        ''' <returns></returns>
        Public Iterator Function MapAssignmentByCoverage(objects As IEnumerable(Of String),
                                                         maps As IEnumerable(Of NamedCollection(Of String)),
                                                         Optional includesUnknown As Boolean = False) As IEnumerable(Of NamedCollection(Of String))

            Dim objectPool As Index(Of String) = objects.Distinct.Indexing
            Dim mapList As Dictionary(Of String, String()) =
                maps.ToDictionary(Function(map) map.name,
                                  Function(map)
                                      Return map.ToArray
                                  End Function)

            Do While mapList.Count > 0
                Dim coverages = From map
                                In mapList
                                Let coverage As Double = objectPool _
                                    .Intersect(collection:=map.Value) _
                                    .Count
                                Select map, coverage = coverage / (maps.Count ^ 2) ' 尽量规模小的代谢图优先分配，这样子作图的时候更加美观
                                Order By coverage Descending

                Dim top = coverages.First
                Dim intersectObjects As String() = objectPool _
                    .Intersect(collection:=top.map.Value) _
                    .ToArray

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