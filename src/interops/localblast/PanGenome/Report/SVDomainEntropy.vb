Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.DataMining.ComponentModel
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.PanGenome.ReportJSON

''' <summary>
''' 结构变异(SV)的矩阵化与基因家族层面的信息熵分析。
''' </summary>
''' <remarks>
''' 分析思路（参考 sv_entropy.md）：
''' 
''' 1. 把SV结构变异的结果组织成两个矩阵：行是基因家族、列是基因组，
'''    矩阵元素分别是该家族在该基因组之中的 SV CopyNumber 与 SV Median；
''' 2. 对矩阵的每一行计算香农信息熵：CopyNumber 熵衡量该家族的拷贝数
'''    在各个基因组之间的多样性（分布均匀性），Median 熵衡量其结构特征的离散程度；
''' 3. 以这两个信息熵作为坐标绘制散点图，并且使用 KMeans 划分出具有不同演化特征的
'''    基因家族类群（对应"僵化保守型/剂量调谐型/混沌快速进化型/结构微调型"四类象限）。
''' 
''' 只有存在SV事件的基因家族才会进入矩阵（共线性断裂这一类没有家族归属的事件会被排除），
''' 同时按照 sv_entropy.md 的建议，在聚类之前过滤掉出现频率过低（默认低于5%）的家族，
''' 并对两个信息熵做 Z-score 标准化，避免方差大的维度主导聚类结果。
''' </remarks>
Public Module SVDomainEntropy

    ''' <summary>
    ''' 参与聚类分析的最低出现频率：某个家族存在SV事件的基因组比例低于这个值的时候，
    ''' 认为该家族的数据过于稀疏、信息熵容易受到噪声干扰，因此不参与聚类
    ''' （参考 sv_entropy.md 的实施建议）
    ''' </summary>
    Public Const MinPresenceFrequency As Double = 0.05

    ''' <summary>
    ''' KMeans 聚类的默认簇数：对应 sv_entropy.md 之中的四类进化模式象限
    ''' </summary>
    Public Const DefaultClusterCount As Integer = 4

    ''' <summary>散点图X轴标题</summary>
    Const CopyNumberEntropyLabel As String = "CopyNumber 熵 Hcn"
    ''' <summary>散点图Y轴标题</summary>
    Const MedianEntropyLabel As String = "Median 熵 Hmed"

#Region "SV 矩阵"

    ''' <summary>
    ''' SV 结构变异的 CopyNumber / Median 矩阵（行=基因家族，列=基因组）
    ''' </summary>
    Public Class SVMatrixPair

        ''' <summary>
        ''' 基因家族ID（矩阵的行）
        ''' </summary>
        ''' <returns></returns>
        Public Property Families As String()
        ''' <summary>
        ''' 基因组名称（矩阵的列，按名称排序）
        ''' </summary>
        ''' <returns></returns>
        Public Property Genomes As String()
        ''' <summary>
        ''' [家族][基因组] 矩阵元素为该家族在该基因组之中的 SV CopyNumber，没有SV事件的单元格为0
        ''' </summary>
        ''' <returns></returns>
        Public Property CopyNumber As Double()()
        ''' <summary>
        ''' [家族][基因组] 矩阵元素为该家族在该基因组之中的 SV Median，没有SV事件的单元格为0
        ''' </summary>
        ''' <returns></returns>
        Public Property Median As Double()()

    End Class

    ''' <summary>
    ''' 获取SV结构变异的 CopyNumber / Median 矩阵（缓存为空的时候惰性重建）
    ''' </summary>
    ''' <param name="result">泛基因组分析结果</param>
    ''' <remarks>
    ''' 矩阵可以完全由分析结果之中的结构变异事件与基因组列表确定性地重建出来，
    ''' 因此没有写入归档文件；在基因组数量非常多的时候这两个矩阵会占用比较大的内存，
    ''' 所以只在真正需要（生成报告、导出表格）的时候才构建一次并缓存下来。
    ''' </remarks>
    <Extension>
    Public Function GetSVMatrices(result As PanGenomeResult) As SVMatrixPair
        If result Is Nothing Then
            Return Nothing
        End If
        If result.SVMatrices Is Nothing Then
            result.SVMatrices = BuildSVMatrices(result)
        End If

        Return result.SVMatrices
    End Function

    ''' <summary>
    ''' 由结构变异事件构建 SV CopyNumber / Median 矩阵
    ''' </summary>
    ''' <param name="result">泛基因组分析结果</param>
    ''' <remarks>
    ''' 行只包含"至少存在一个SV事件"的基因家族，列是全部基因组。
    ''' 已验证在真实的结构变异事件之中，同一个 家族x基因组 最多只会有一条记录
    ''' （PAV_Absence / PAV_Presence / CNV_Gain|Loss 三类判定互斥），因此矩阵是良定义的。
    ''' </remarks>
    Public Function BuildSVMatrices(result As PanGenomeResult) As SVMatrixPair
        Dim genomes As String() = result.TotalGenesInGenomes.Keys _
            .OrderBy(Function(x) x, StringComparer.Ordinal) _
            .ToArray()
        Dim N As Integer = genomes.Length
        Dim genomeIndex As New Dictionary(Of String, Integer)(N)

        For i As Integer = 0 To N - 1
            genomeIndex(genomes(i)) = i
        Next

        Dim familyIds As New List(Of String)()
        Dim familyIndex As New Dictionary(Of String, Integer)()
        Dim events As New List(Of SVEvent)()

        If result.StructuralVariations IsNot Nothing Then
            For Each sv As StructuralVariation In result.StructuralVariations
                ' 共线性断裂这一类事件没有家族归属(FamilyID为空)，无法进入矩阵
                If sv Is Nothing OrElse String.IsNullOrEmpty(sv.FamilyID) Then
                    Continue For
                End If

                Dim target As Integer = -1

                If Not genomeIndex.TryGetValue(sv.GenomeName, target) Then
                    Continue For
                End If

                Dim row As Integer = -1

                If Not familyIndex.TryGetValue(sv.FamilyID, row) Then
                    row = familyIds.Count
                    Call familyIndex.Add(sv.FamilyID, row)
                    Call familyIds.Add(sv.FamilyID)
                End If

                Call events.Add(New SVEvent(row, target, CDbl(sv.CopyNumber), sv.Median))
            Next
        End If

        ' 矩阵的行按照家族ID排序，保证多次构建出来的矩阵顺序是确定性的
        Dim order As Integer() = familyIds _
            .Select(Function(id, i) (id:=id, index:=i)) _
            .OrderBy(Function(x) x.id, StringComparer.Ordinal) _
            .Select(Function(x) x.index) _
            .ToArray()
        Dim slots(order.Length - 1) As Integer

        For i As Integer = 0 To order.Length - 1
            slots(order(i)) = i
        Next

        Dim families As String() = New String(familyIds.Count - 1) {}
        Dim copyNumber As Double()() = New Double(familyIds.Count - 1)() {}
        Dim median As Double()() = New Double(familyIds.Count - 1)() {}

        For i As Integer = 0 To familyIds.Count - 1
            Dim row As Integer = slots(i)

            families(row) = familyIds(i)
            copyNumber(row) = New Double(N - 1) {}
            median(row) = New Double(N - 1) {}
        Next

        For Each e As SVEvent In events
            Dim row As Integer = slots(e.Family)

            copyNumber(row)(e.Genome) = e.CopyNumber
            median(row)(e.Genome) = e.Median
        Next

        Return New SVMatrixPair With {
            .Families = families,
            .Genomes = genomes,
            .CopyNumber = copyNumber,
            .Median = median
        }
    End Function

    ''' <summary>
    ''' 矩阵之中的一个结构变异事件
    ''' </summary>
    Private Structure SVEvent

        Public ReadOnly Family As Integer
        Public ReadOnly Genome As Integer
        Public ReadOnly CopyNumber As Double
        Public ReadOnly Median As Double

        Sub New(family As Integer, genome As Integer, copyNumber As Double, median As Double)
            Me.Family = family
            Me.Genome = genome
            Me.CopyNumber = copyNumber
            Me.Median = median
        End Sub

    End Structure

#End Region

#Region "信息熵散点图与聚类"

    ''' <summary>
    ''' KMeans聚类的输入实体：以两个标准化之后的香农信息熵作为坐标
    ''' </summary>
    Public Class SVEntropyEntity : Inherits EntityBase(Of Double)

        ''' <summary>
        ''' 基因家族ID
        ''' </summary>
        ''' <returns></returns>
        Public Property familyId As String
        ''' <summary>
        ''' CopyNumber 矩阵行的香农信息熵（原始值）
        ''' </summary>
        ''' <returns></returns>
        Public Property hCopyNumber As Double
        ''' <summary>
        ''' Median 矩阵行的香农信息熵（原始值）
        ''' </summary>
        ''' <returns></returns>
        Public Property hMedian As Double
        ''' <summary>
        ''' 对应的散点图数据点，用于把聚类结果写回到结果对象之中
        ''' </summary>
        ''' <returns></returns>
        Public Property point As SVDomainEntropyPoint

    End Class

    ''' <summary>
    ''' 获取SV信息熵散点图与聚类结果（缓存为空的时候惰性计算）
    ''' </summary>
    ''' <param name="result">泛基因组分析结果</param>
    ''' <param name="k">KMeans 的簇数</param>
    ''' <remarks>
    ''' 如果结果对象之中已经存在缓存，则直接返回缓存（此时<paramref name="k"/>会被忽略），
    ''' 这样子可以保证报告页面与外部脚本拿到的是完全同一份聚类结果。
    ''' </remarks>
    <Extension>
    Public Function GetSVEntropy(result As PanGenomeResult, Optional k As Integer = DefaultClusterCount) As SVDomainEntropyDataset
        If result Is Nothing Then
            Return Nothing
        End If
        If result.SVEntropy Is Nothing Then
            result.SVEntropy = BuildSVEntropy(result, k)
        End If

        Return result.SVEntropy
    End Function

    ''' <summary>
    ''' 计算SV矩阵的信息熵散点图数据并且做KMeans聚类
    ''' </summary>
    ''' <param name="result">泛基因组分析结果</param>
    ''' <param name="k">KMeans 的簇数（默认4）</param>
    ''' <remarks>
    ''' 任何数据不足（没有SV事件、有效家族数量不足）或者聚类失败的情况都会返回一个空数据集，
    ''' 由前端显示"数据不可用"，保证SV熵分析不会导致整个报告生成流程失败。
    ''' </remarks>
    Public Function BuildSVEntropy(result As PanGenomeResult, Optional k As Integer = DefaultClusterCount) As SVDomainEntropyDataset
        If result Is Nothing Then
            Return EmptyDataset(genomeCount:=0)
        End If

        Dim sw As Stopwatch = Stopwatch.StartNew
        Dim empty As SVDomainEntropyDataset = EmptyDataset(genomeCount:=result.TotalGenesInGenomes.Count)

        Dim matrix As SVMatrixPair = result.GetSVMatrices
        Dim N As Integer = matrix.Genomes.Length

        empty.genomeCount = N

        If matrix.Families.Length = 0 OrElse N = 0 Then
            Call "[pan-genome] SV entropy: no structural variation event found, analysis skipped.".debug

            Return empty
        End If

        ' ---------------------------------------------------------------
        ' 1. 逐行计算两个香农信息熵，同时过滤出现频率过低的基因家族
        ' ---------------------------------------------------------------
        Dim minGenomes As Integer = Math.Max(1, CInt(Math.Ceiling(MinPresenceFrequency * N)))
        Dim points As New List(Of SVDomainEntropyPoint)()
        Dim filtered As Integer = 0

        For i As Integer = 0 To matrix.Families.Length - 1
            Dim cnRow As Double() = matrix.CopyNumber(i)
            Dim medRow As Double() = matrix.Median(i)
            Dim present As Integer = 0

            For g As Integer = 0 To N - 1
                If cnRow(g) > 0 OrElse medRow(g) > 0 Then
                    present += 1
                End If
            Next

            If present < minGenomes Then
                filtered += 1
                Continue For
            End If

            Call points.Add(New SVDomainEntropyPoint With {
                .name = matrix.Families(i),
                .hCopyNumber = EntropyOfCounts(cnRow),
                .hMedian = EntropyOfCategories(medRow),
                .presentGenomes = present
            })
        Next

        Call $"[pan-genome] SV entropy: {matrix.Families.Length} families, {points.Count} kept, {filtered} filtered (min present genomes: {minGenomes}/{N}), elapsed {sw.ElapsedMilliseconds} ms".debug

        If points.Count = 0 Then
            empty.filteredCount = filtered
            empty.familyCount = matrix.Families.Length

            Return empty
        End If

        ' ---------------------------------------------------------------
        ' 2. Z-score 标准化（两个熵的量纲可能不同，防止方差大的维度主导聚类）
        ' ---------------------------------------------------------------
        Dim hcn As Double() = points.Select(Function(p) p.hCopyNumber).ToArray
        Dim hmed As Double() = points.Select(Function(p) p.hMedian).ToArray
        Dim zcn As Double() = ZScores(hcn)
        Dim zmed As Double() = ZScores(hmed)

        For i As Integer = 0 To points.Count - 1
            points(i).zCopyNumber = zcn(i)
            points(i).zMedian = zmed(i)
        Next

        ' ---------------------------------------------------------------
        ' 3. KMeans 聚类（按照簇中心在标准化空间之中的位置做确定性重排）
        ' ---------------------------------------------------------------
        sw.Restart()

        Dim clusters As New List(Of SVEntropyCluster)()

        If k >= 2 AndAlso points.Count > k Then
            Try
                Call RunKMeans(points, k, clusters)
            Catch ex As Exception
                Call Debug.WriteLine($"[pangenome] SV entropy kmeans clustering failed: {ex.Message}")

                clusters.Clear()
            End Try
        End If

        If clusters.Count = 0 Then
            ' 点数不足或者聚类失败：退化为不聚类（全部的点都属于同一个簇）
            For Each p As SVDomainEntropyPoint In points
                p.cluster = 0
            Next

            Call clusters.Add(New SVEntropyCluster With {
                .cluster = 0,
                .size = points.Count,
                .meanCopyNumberEntropy = hcn.Average,
                .meanMedianEntropy = hmed.Average
            })
        End If

        ' 按照簇中心相对全局均值的"高/低"位置给出四象限进化模型的解读名称
        Dim meanCN As Double = hcn.Average
        Dim meanMed As Double = hmed.Average

        For Each c As SVEntropyCluster In clusters
            c.label = QuadrantLabel(c.meanCopyNumberEntropy >= meanCN, c.meanMedianEntropy >= meanMed)
        Next

        Call $"[pan-genome] SV entropy kmeans done: k={clusters.Count}, {points.Count} gene families, elapsed {sw.ElapsedMilliseconds} ms".debug

        Return New SVDomainEntropyDataset With {
            .points = points.ToArray,
            .clusters = clusters.ToArray,
            .familyCount = points.Count,
            .filteredCount = filtered,
            .clusterCount = clusters.Count,
            .copyNumberEntropyLabel = CopyNumberEntropyLabel,
            .medianEntropyLabel = MedianEntropyLabel,
            .genomeCount = N
        }
    End Function

    ''' <summary>
    ''' 执行KMeans聚类，并且把簇编号按照簇中心的位置重排为确定性的顺序
    ''' </summary>
    ''' <param name="points">参与聚类的基因家族数据点</param>
    ''' <param name="k">簇数</param>
    ''' <param name="clusters">输出的簇摘要</param>
    ''' <remarks>
    ''' KMeans 的初始聚类中心是随机选取的（<see cref="Microsoft.VisualBasic.Math.RandomExtensions.seeds"/>
    ''' 是按照时间播种的），因此每一次运行得到的簇编号都可能不一样。
    ''' 这里在聚类完成之后按照簇中心在标准化空间之中的 (zCopyNumber, zMedian) 位置
    ''' 做一次确定性的重排，保证同一份数据多次运行得到的簇编号是稳定可复现的，
    ''' 报告页面与导出的CSV表格因此不会在重跑之后发生标签漂移。
    ''' </remarks>
    Private Sub RunKMeans(points As List(Of SVDomainEntropyPoint), k As Integer, clusters As List(Of SVEntropyCluster))
        Dim entities As SVEntropyEntity() = points _
            .Select(Function(p)
                        Return New SVEntropyEntity With {
                            .familyId = p.name,
                            .hCopyNumber = p.hCopyNumber,
                            .hMedian = p.hMedian,
                            .point = p,
                            .entityVector = {p.zCopyNumber, p.zMedian}
                        }
                    End Function) _
            .ToArray()

        Dim kmeans As New KMeansAlgorithm(Of SVEntropyEntity)(n_threads:=Environment.ProcessorCount, auto_parallel:=True)
        Dim raw As ClusterCollection(Of SVEntropyEntity) = kmeans.ClusterDataSet(entities, k)
        Dim singles As New List(Of KMeansCluster(Of SVEntropyEntity))()

        For Each cluster As KMeansCluster(Of SVEntropyEntity) In raw
            ' 在数据高度集中的时候KMeans有可能会产生一些没有任何成员的簇，
            ' 这些空的簇不参与簇编号的排名，否则报告与CSV之中会出现长度为0的簇
            If cluster.NumOfEntity > 0 Then
                Call singles.Add(cluster)
            End If
        Next

        If singles.Count = 0 Then
            Return
        End If

        ' 按照簇中心的坐标排序，得到确定性的簇编号
        Dim order As Integer() = singles _
            .Select(Function(c, i) (index:=i, x:=ClusterMean(c, 0), y:=ClusterMean(c, 1))) _
            .OrderBy(Function(t) t.x) _
            .ThenBy(Function(t) t.y) _
            .Select(Function(t) t.index) _
            .ToArray()
        Dim labels(order.Length - 1) As Integer

        For rank As Integer = 0 To order.Length - 1
            labels(order(rank)) = rank
        Next

        For i As Integer = 0 To singles.Count - 1
            Dim cluster As KMeansCluster(Of SVEntropyEntity) = singles(i)
            Dim summary As New SVEntropyCluster With {
                .cluster = labels(i),
                .size = cluster.NumOfEntity
            }
            Dim sumCN As Double = 0
            Dim sumMed As Double = 0
            Dim n As Integer = 0

            For Each e As SVEntropyEntity In cluster
                If e.point IsNot Nothing Then
                    e.point.cluster = labels(i)
                End If

                sumCN += e.hCopyNumber
                sumMed += e.hMedian
                n += 1
            Next

            summary.meanCopyNumberEntropy = If(n > 0, sumCN / n, 0)
            summary.meanMedianEntropy = If(n > 0, sumMed / n, 0)

            Call clusters.Add(summary)
        Next

        ' 保证输出的簇摘要是按照簇编号的顺序排列的
        Call clusters.Sort(Function(a, b) a.cluster.CompareTo(b.cluster))
    End Sub

    ''' <summary>
    ''' 计算簇在指定维度上面的均值
    ''' </summary>
    Private Function ClusterMean(cluster As KMeansCluster(Of SVEntropyEntity), dimension As Integer) As Double
        Dim sum As Double = 0
        Dim n As Integer = 0

        For Each e As SVEntropyEntity In cluster
            If e.entityVector IsNot Nothing AndAlso e.entityVector.Length > dimension Then
                sum += e.entityVector(dimension)
                n += 1
            End If
        Next

        Return If(n > 0, sum / n, 0)
    End Function

    ''' <summary>
    ''' 根据两个信息熵的"高/低"位置给出四象限进化模型的解读名称（参考 sv_entropy.md）
    ''' </summary>
    ''' <param name="highCopyNumber">该簇的 CopyNumber 熵是否高于整体平均</param>
    ''' <param name="highMedian">该簇的 Median 熵是否高于整体平均</param>
    Private Function QuadrantLabel(highCopyNumber As Boolean, highMedian As Boolean) As String
        If highCopyNumber Then
            If highMedian Then
                Return "混沌/快速进化型"
            Else
                Return "剂量调谐型"
            End If
        Else
            If highMedian Then
                Return "结构微调型"
            Else
                Return "僵化/保守型"
            End If
        End If
    End Function

    Private Function EmptyDataset(genomeCount As Integer) As SVDomainEntropyDataset
        Return New SVDomainEntropyDataset With {
            .points = New SVDomainEntropyPoint() {},
            .clusters = New SVEntropyCluster() {},
            .familyCount = 0,
            .filteredCount = 0,
            .clusterCount = 0,
            .copyNumberEntropyLabel = CopyNumberEntropyLabel,
            .medianEntropyLabel = MedianEntropyLabel,
            .genomeCount = genomeCount
        }
    End Function

#End Region

#Region "香农信息熵"

    ''' <summary>
    ''' 按"各基因组拷贝数占比"的离散概率分布计算香农信息熵（自然对数）
    ''' </summary>
    ''' <param name="row">某一行SV CopyNumber矩阵的数值（列=基因组）</param>
    ''' <remarks>
    ''' p_i = CN_i / ΣCN，H = -Σ p_i * ln(p_i)。
    ''' 
    ''' 熵越高说明该基因家族的拷贝数在各个基因组之间的差异越大（经历了频繁的拷贝数扩张/收缩）；
    ''' 熵越低说明其拷贝数在群体之中非常一致（剂量受到严格的纯化选择）。
    ''' 
    ''' 取值为0的列不会影响结果：它们的 p 都是0，既不参与求和也不会改变其它列的概率。
    ''' </remarks>
    Private Function EntropyOfCounts(row As Double()) As Double
        Dim total As Double = 0

        For Each v As Double In row
            If v > 0 Then
                total += v
            End If
        Next

        ' 全部为0（该家族没有任何SV事件）：信息熵定义为0，避免 Log(0) 产生 NaN
        If total <= 0 Then
            Return 0
        End If

        Dim h As Double = 0

        For Each v As Double In row
            If v > 0 Then
                Dim p As Double = v / total

                h -= p * Math.Log(p)
            End If
        Next

        Return h
    End Function

    ''' <summary>
    ''' 以取值的精确大小作为离散类别统计频率，计算香农信息熵（自然对数）
    ''' </summary>
    ''' <param name="row">某一行SV Median矩阵的数值（列=基因组）</param>
    ''' <remarks>
    ''' 本项目之中的 Median 是"拷贝数中位数"，取值是很小的离散值（0/1/2/3...）而不是bp长度，
    ''' 因此不需要像 sv_entropy.md 之中建议的那样先做分箱，直接使用精确取值作为类别即可。
    ''' 
    ''' 行内为0的单元格表示该基因组没有对应的SV事件，它同样是一种"结构状态"，
    ''' 因此这里会把0也作为一个离散类别参与频率统计。
    ''' 
    ''' 注意：在当前的结构变异数据模型之中，同一个基因家族的所有SV事件共享同一个
    ''' Median 值（Median 是"该家族在有SV事件的基因组之中的拷贝数中位数"，是家族级别的常量），
    ''' 因此这一行的取值实际上只有 {0, median} 两种，Median 熵刻画的是
    ''' "SV 状态在各个基因组之间分布的均衡程度"：
    ''' 
    '''   * 熵越高 -> 该家族的SV事件既不是普遍存在、也不是普遍缺失（高度多态）；
    '''   * 熵越低 -> 该家族的SV状态在所有基因组之中高度一致（几乎全部有、或者几乎没有）。
    ''' 
    ''' 如果后续在SV检出的上游把每个基因组各自的SV尺寸/断裂点记录下来，
    ''' 这个熵就会退化为 sv_entropy.md 之中所描述的"结构特征的离散程度"，无需修改本模块。
    ''' </remarks>
    Private Function EntropyOfCategories(row As Double()) As Double
        If row.Length = 0 Then
            Return 0
        End If

        Dim freq As New Dictionary(Of Double, Integer)()
        Dim n As Integer = row.Length

        For Each v As Double In row
            Dim c As Integer = 0

            freq(v) = If(freq.TryGetValue(v, c), c + 1, 1)
        Next

        Dim h As Double = 0

        For Each c As Integer In freq.Values
            Dim p As Double = c / n

            h -= p * Math.Log(p)
        Next

        Return h
    End Function

    ''' <summary>
    ''' 计算 Z-score 标准化之后的数值序列
    ''' </summary>
    ''' <param name="values">原始数值</param>
    ''' <remarks>
    ''' 当标准差为0（全部取值相同、或者只有一个样本）的时候直接返回全0的序列，避免除零。
    ''' </remarks>
    Private Function ZScores(values As Double()) As Double()
        Dim z As Double() = New Double(values.Length - 1) {}

        If values.Length = 0 Then
            Return z
        End If

        Dim mean As Double = values.Average
        Dim sd As Double = 0

        For Each v As Double In values
            sd += (v - mean) ^ 2
        Next

        sd = Math.Sqrt(sd / values.Length)

        If sd <= 0 Then
            Return z
        End If

        For i As Integer = 0 To values.Length - 1
            z(i) = (values(i) - mean) / sd
        Next

        Return z
    End Function

#End Region

End Module
