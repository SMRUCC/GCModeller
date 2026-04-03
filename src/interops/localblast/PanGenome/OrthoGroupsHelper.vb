Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Public Module OrthoGroupsHelper

    ''' <summary>
    ''' 将基因组vs参考库的BHR直系同源结果放入到基因簇构建模块之中
    ''' </summary>
    ''' <param name="uf"></param>
    ''' <param name="bhr">query为待分析的基因组内的基因ID，hit为参考库中的基因ID</param>
    ''' <returns></returns>
    Public Function SetReferenceBHR(uf As UnionFind, bhr As BiDirectionalBesthit()) As UnionFind
        For Each hit As BiDirectionalBesthit In bhr
            Call uf.AddElement(hit.QueryName)
            Call uf.AddElement(hit.HitName)
            Call uf.Union(hit.HitName, hit.QueryName)
        Next

        Return uf
    End Function

    ''' <summary>
    ''' 处理基因组比对数据，生成两两同源关系
    ''' </summary>
    Public Iterator Function BuildHomologyRelations(Of T As IBlastHit)(genomeData As Dictionary(Of String, T())) As IEnumerable(Of HomologyPair)
        ' 第一步：构建倒排索引
        ' Key: 参考序列ID (Hit)
        ' Value: 比对到该参考序列的基因组及其基因列表
        Dim refIndex As New Dictionary(Of String, List(Of (String, String)))()

        For Each genomeKvp In genomeData
            Dim genomeName As String = genomeKvp.Key
            Dim alignments As T() = genomeKvp.Value

            For Each aln In alignments
                If Not refIndex.ContainsKey(aln.HitName) Then
                    refIndex(aln.HitName) = New List(Of (String, String))()
                End If
                ' 存储 (基因组名, 基因ID)
                refIndex(aln.HitName).Add((genomeName, aln.QueryName))
            Next
        Next

        ' 第二步：遍历索引，生成两两关系
        For Each refKvp In refIndex
            Dim refId As String = refKvp.Key
            Dim hitsList As List(Of (String, String)) = refKvp.Value

            ' 只有当有两个以上的基因比对到同一个Ref时，才存在同源关系
            If hitsList.Count > 1 Then
                ' 双重循环生成两两组合
                For i As Integer = 0 To hitsList.Count - 1
                    For j As Integer = i + 1 To hitsList.Count - 1
                        Dim item1 = hitsList(i)
                        Dim item2 = hitsList(j)

                        ' 关键点：判断是否为“两两基因组”之间的关系
                        ' 如果你只想要不同基因组间的关系，加上这个判断；
                        ' 如果你想包含同一基因组内的旁系同源，则去掉这个判断。
                        If item1.Item1 <> item2.Item1 Then
                            Dim newPair As New HomologyPair With {
                                .GenomeA = item1.Item1,
                                .GeneA = item1.Item2,
                                .GenomeB = item2.Item1,
                                .GeneB = item2.Item2,
                                .BridgeRef = refId
                            }

                            Yield newPair
                        End If
                    Next
                Next
            End If
        Next
    End Function

End Module

''' <summary>
''' 定义最终输出的两两同源关系结构
''' </summary>
Public Class HomologyPair

    Public Property GenomeA As String
    Public Property GeneA As String
    Public Property GenomeB As String
    Public Property GeneB As String
    Public Property BridgeRef As String ' 作为桥梁的参考序列ID

    Public Overrides Function ToString() As String
        Return $"[{GenomeA}]{GeneA} <--> [{GenomeB}]{GeneB} (via {BridgeRef})"
    End Function
End Class