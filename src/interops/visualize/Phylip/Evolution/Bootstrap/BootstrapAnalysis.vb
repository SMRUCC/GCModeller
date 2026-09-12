Imports System.Threading.Tasks
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Models
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.TreeIO
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview

Namespace Evolution.Bootstrap

    ''' <summary>
    ''' Bootstrap 分析结果。
    ''' </summary>
    Public Class BootstrapResult
        ''' <summary>
        ''' 参考树；内部节点的 <see cref="PhyloNode.BootStrap"/> 为 bootstrap 支持度（百分比）
        ''' </summary>
        Public Property Tree As PhyloNode
        ''' <summary>
        ''' 各 split 的支持度（0~1）
        ''' </summary>
        Public Property SplitSupport As Dictionary(Of String, Double)
        ''' <summary>
        ''' 重采样次数
        ''' </summary>
        Public Property Replicates As Integer
        Public Property Algorithm As EvolutionAlgorithm
    End Class

    ''' <summary>
    ''' Bootstrap 支持度评估：从原始比对中有放回地随机抽取位点（保留总位点数），
    ''' 用同一算法重建树，重复成百上千次；每个内部分支在重抽样树中出现的频率即其支持度
    ''' （通常 ≥95% 视为高度可信）。
    ''' </summary>
    Public Module BootstrapAnalysis

        ''' <summary>
        ''' 执行 bootstrap 支持度评估。
        ''' </summary>
        ''' <param name="matrix">原始比对位点矩阵</param>
        ''' <param name="algorithm">重建树所使用的算法</param>
        ''' <param name="replicates">重采样次数（默认 100）</param>
        ''' <param name="options">算法参数</param>
        ''' <param name="referenceTree">
        ''' 参考树；为空时使用原始矩阵按同一算法构建的树
        ''' </param>
        ''' <param name="parallel">是否并行执行重采样</param>
        Public Function Run(matrix As CharacterMatrix,
                            algorithm As EvolutionAlgorithm,
                            Optional replicates As Integer = 100,
                            Optional options As EvolutionOptions = Nothing,
                            Optional referenceTree As PhyloNode = Nothing,
                            Optional parallel As Boolean = True) As BootstrapResult

            If replicates < 1 Then
                replicates = 1
            End If
            If options Is Nothing Then
                options = New EvolutionOptions
            End If

            Console.WriteLine($"[Bootstrap] algorithm={algorithm}, replicates={replicates}, sites={matrix.SiteCount}")

            ' 参考树
            If referenceTree Is Nothing Then
                referenceTree = TreeBuilder.Build(matrix, algorithm, options)
            End If

            Dim siteCount As Integer = matrix.SiteCount
            Dim replicateSplits(replicates - 1) As Dictionary(Of String, Integer)
            Dim replicateSuccess(replicates - 1) As Boolean

            Dim runReplicate As Action(Of Integer) =
                Sub(index As Integer)
                    Try
                        Dim rand As New Random(options.Seed + index * 7919 + 13)
                        Dim indices(siteCount - 1) As Integer

                        For k As Integer = 0 To siteCount - 1
                            indices(k) = rand.Next(siteCount)
                        Next

                        Dim resampled As CharacterMatrix = matrix.SubColumns(indices)
                        Dim tree As PhyloNode = TreeBuilder.Build(resampled, algorithm, options)
                        Dim splits As New Dictionary(Of String, Integer)

                        For Each key As String In PhyloTreeFactory.AllSplits(tree).Keys
                            Dim count As Integer = 0
                            splits.TryGetValue(key, count)
                            splits(key) = count + 1
                        Next

                        replicateSplits(index) = splits
                        replicateSuccess(index) = True
                    Catch ex As Exception
                        ' 某些重采样数据可能不包含足够的信息位点（例如最大简约法），此时跳过该次重采样
                        Console.WriteLine($"[Bootstrap] replicate {index} skipped: {ex.Message} @ {ex.StackTrace}")
                        replicateSplits(index) = New Dictionary(Of String, Integer)
                    End Try
                End Sub

            If parallel AndAlso replicates > 1 Then
                Call System.Threading.Tasks.Parallel.For(0, replicates, runReplicate)
            Else
                For index As Integer = 0 To replicates - 1
                    Call runReplicate(index)
                Next
            End If

            ' 汇总各 split 的支持度
            Dim counts As New Dictionary(Of String, Integer)

            For Each splits As Dictionary(Of String, Integer) In replicateSplits
                If splits Is Nothing Then
                    Continue For
                End If

                For Each kv As KeyValuePair(Of String, Integer) In splits
                    Dim current As Integer = 0
                    counts.TryGetValue(kv.Key, current)
                    counts(kv.Key) = current + kv.Value
                Next
            Next

            Dim support As New Dictionary(Of String, Double)
            Dim successful As Integer = replicateSuccess.Count(Function(ok) ok)

            If successful < 1 Then
                successful = 1
            End If

            For Each kv As KeyValuePair(Of String, Integer) In counts
                support(kv.Key) = kv.Value / successful
            Next

            ' 将支持度写回参考树的内部节点
            Call PhyloTreeFactory.TransferSupport(referenceTree, counts, successful)

            Console.WriteLine($"[Bootstrap] done, {counts.Count} splits supported ({successful}/{replicates} replicates succeeded)")

            Return New BootstrapResult With {
                .Tree = referenceTree,
                .SplitSupport = support,
                .Replicates = replicates,
                .Algorithm = algorithm
            }
        End Function
    End Module

End Namespace
