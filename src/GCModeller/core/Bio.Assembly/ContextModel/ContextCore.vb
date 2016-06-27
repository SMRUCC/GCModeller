Imports LANS.SystemsBiology.ComponentModel
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Language

Namespace ContextModel

    ''' <summary>
    ''' The working core of the genomics context provider.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Structure RelationDelegate(Of T As IGeneBrief)

        Dim dataSource As OrderSelector(Of IntTag(Of T))
        Dim loci As NucleotideLocation

        ''' <summary>
        ''' 为了提高上下文的搜索效率，只在附近的位置搜索
        ''' 对于正向，是从小到大排序的
        ''' </summary>
        ''' <param name="relType"></param>
        ''' <param name="dist"></param>
        ''' <returns></returns>
        Public Function GetRelation(relType As SegmentRelationships, dist As Integer) As T()
            Dim loci As NucleotideLocation = Me.loci
            Dim result As IEnumerable(Of IntTag(Of T)) = Nothing

            If Not dataSource.Desc Then ' 目标序列是升序排序的
                Dim n As Integer = loci.Right + dist
                result = dataSource.SelectUntilGreaterThan(n)
            Else ' 目标序列是降序排序的
                Dim n As Integer = loci.Left - dist
                result = dataSource.SelectUntilLessThan(n)
            End If

            Dim genes As T() =
                LinqAPI.Exec(Of T) <= From gene As T
                                      In result.Select(Function(x) x.x)
                                      Let Relation As SegmentRelationships =
                                          GetLociRelations(gene, loci)
                                      Where Relation = relType
                                      Select gene
            Return genes
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="relType"></param>
        ''' <param name="ATGdist"></param>
        ''' <returns></returns>
        Public Function GetATGRelation(relType As SegmentRelationships, ATGdist As Integer) As T()
            Dim genes As T() = GetRelation(relType, ATGdist)
            Dim loci As NucleotideLocation = Me.loci
            Dim LQuery As T() =
                LinqAPI.Exec(Of T) <= From gene As T
                                      In genes
                                      Where Math.Abs(GetATGDistance(loci, gene)) <= ATGdist
                                      Select gene '获取ATG距离小于阈值的所有基因
            Return LQuery
        End Function
    End Structure
End Namespace
