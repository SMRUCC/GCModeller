Imports System.IO
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports DNMFModel = Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.DNMF
Imports NMFModel = Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.NMF
Imports NumericMatrix = Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.NumericMatrix
Imports std = System.Math

''' <summary>
''' 初始分解所使用的非负矩阵分解算法
''' </summary>
Public Enum DecompositionMethod

    ''' <summary>
    ''' 判别式非负矩阵分解(<see cref="DNMFModel"/>)，标签数组为每一个基因行所属的菌株下标。
    ''' </summary>
    ''' <remarks>
    ''' Fisher 类内/类间散度约束会让同一个菌株的基因在载荷矩阵 <c>W</c> 上聚拢、
    ''' 不同菌株之间分离，因此因子与菌株的对应关系最为稳定，是默认算法。
    ''' </remarks>
    DNMF = 0

    ''' <summary>
    ''' 纯无监督非负矩阵分解(<see cref="NMFModel"/>)。
    ''' </summary>
    NMF = 1
End Enum

''' <summary>
''' 合成菌群转录组矩阵分解流程的参数
''' </summary>
Public Class DecompositionOptions

    ''' <summary>
    ''' 初始分解算法，默认 <see cref="DecompositionMethod.DNMF"/>
    ''' </summary>
    Public Property Method As DecompositionMethod = DecompositionMethod.DNMF

    ''' <summary>
    ''' 初始分解的最大迭代次数
    ''' </summary>
    ''' <remarks>
    ''' <see cref="NMFModel"/> 的 <c>tolerance</c> 是与 “绝对平方误差和” 比较，在原始 hits 量级下
    ''' 几乎必然跑满迭代，因此这里保持一个保守的上限(默认 300)，避免无谓的等待。
    ''' </remarks>
    Public Property MaxIterations As Integer = 300

    ''' <summary>
    ''' 初始分解的收敛阈值
    ''' </summary>
    Public Property Tolerance As Double = 0.00001

    ''' <summary>
    ''' 乘法更新中的非负稳定项
    ''' </summary>
    Public Property Epsilon As Double = 0.000001

    ''' <summary>
    ''' 随机初始化种子，用于让 <see cref="DNMFModel"/> 的结果可复现
    ''' </summary>
    Public Property RandomSeed As Integer? = Nothing

    ''' <summary>
    ''' Fisher 约束的类内散度惩罚权重
    ''' </summary>
    Public Property LambdaWithin As Double = 0.1

    ''' <summary>
    ''' Fisher 约束的类间散度奖励权重
    ''' </summary>
    Public Property LambdaBetween As Double = 0.1

    ''' <summary>
    ''' 结构掩码精化的最大轮数
    ''' </summary>
    Public Property RefineIterations As Integer = 60

    ''' <summary>
    ''' 结构掩码精化的相对变化收敛阈值
    ''' </summary>
    Public Property RefineTolerance As Double = 0.000001

    ''' <summary>
    ''' 是否将同源基因家族的共享信号归并到 “本菌株自己的基因拷贝” 行(默认 True)
    ''' </summary>
    Public Property RedistributeHomologs As Boolean = True

    ''' <summary>
    ''' 是否在单菌株矩阵中补齐 “基因组内存在、但总矩阵中没有测量行” 的零表达基因(默认 False)
    ''' </summary>
    Public Property IncludeSilentGenes As Boolean = False

    ''' <summary>
    ''' 是否按列把各菌株矩阵的总量校正到总矩阵的列总量(默认 False，保留因子模型的原始重构值)
    ''' </summary>
    Public Property RescaleToTotal As Boolean = False

    ''' <summary>
    ''' 基因ID跨基因组重复时的行为：True = 抛出异常，False = 取首个归属并告警
    ''' </summary>
    Public Property Strict As Boolean = True

    ''' <summary>
    ''' 是否显示 <see cref="NMFModel"/>/<see cref="DNMFModel"/> 的迭代进度条
    ''' </summary>
    Public Property Tqdm As Boolean = False

    ''' <summary>
    ''' 结构掩码精化的并行度：0 = 全部逻辑核心，1 = 串行(默认 0)
    ''' </summary>
    Public Property DegreeOfParallelism As Integer = 0
End Class

''' <summary>
''' 一个跨菌株同源的基因家族簇
''' </summary>
Public Class GeneFamily

    ''' <summary>
    ''' 家族ID(并查集的根节点)
    ''' </summary>
    Public Property familyID As String

    ''' <summary>
    ''' 家族内的全部基因ID
    ''' </summary>
    Public Property members As String()

    ''' <summary>
    ''' 按菌株拆分的家族成员：键名为菌株的基因组ID
    ''' </summary>
    Public Property strainMembers As Dictionary(Of String, String())

    ''' <summary>
    ''' 该家族覆盖的菌株数量
    ''' </summary>
    Public ReadOnly Property strainCount As Integer
        Get
            Return If(strainMembers Is Nothing, 0, strainMembers.Count)
        End Get
    End Property

    ''' <summary>
    ''' 是否为跨菌株共享的同源家族(<see cref="strainCount"/> 大于 1)
    ''' </summary>
    Public ReadOnly Property isShared As Boolean
        Get
            Return strainCount > 1
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{familyID}: {strainCount} strain(s), {If(members Is Nothing, 0, members.Length)} gene(s)"
    End Function
End Class

''' <summary>
''' 合成菌群的结构层信息：基因归属、同源家族与标记基因
''' </summary>
Public Class CommunityStructure

    ''' <summary>
    ''' 菌株(基因组)ID 列表，顺序与输入的 genomes 字典保持一致
    ''' </summary>
    Public Property strains As String()

    ''' <summary>
    ''' 规范化之后的 “基因组ID -> 基因ID列表”
    ''' </summary>
    Public Property genomes As Dictionary(Of String, String())

    ''' <summary>
    ''' 基因ID -> 基因组ID 的唯一归属
    ''' </summary>
    Public Property geneStrain As Dictionary(Of String, String)

    ''' <summary>
    ''' 家族ID -> 基因家族(包含全部家族，含单菌株家族)
    ''' </summary>
    Public Property families As Dictionary(Of String, GeneFamily)

    ''' <summary>
    ''' 同源歧义基因 -> 其所属的跨菌株家族
    ''' </summary>
    Public Property homologGenes As Dictionary(Of String, GeneFamily)

    ''' <summary>
    ''' 菌株 -> 标记基因(在其它菌株基因组中不存在同源基因)
    ''' </summary>
    Public Property markers As Dictionary(Of String, String())

    ''' <summary>
    ''' 菌株数量
    ''' </summary>
    Public ReadOnly Property size As Integer
        Get
            Return If(strains Is Nothing, 0, strains.Length)
        End Get
    End Property

    ''' <summary>
    ''' 取出某个基因所属的基因组ID，未知基因返回 Nothing
    ''' </summary>
    Public Function GetStrainOf(geneID As String) As String
        Dim strain As String = Nothing

        If geneStrain IsNot Nothing AndAlso geneStrain.TryGetValue(geneID, strain) Then
            Return strain
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' 判断一个基因是否存在跨菌株的同源歧义
    ''' </summary>
    Public Function IsHomolog(geneID As String) As Boolean
        Return homologGenes IsNot Nothing AndAlso homologGenes.ContainsKey(geneID)
    End Function

    ''' <summary>
    ''' 取出某个基因所属的跨菌株家族，非同源基因返回 Nothing
    ''' </summary>
    Public Function GetFamily(geneID As String) As GeneFamily
        Dim family As GeneFamily = Nothing

        If homologGenes IsNot Nothing AndAlso homologGenes.TryGetValue(geneID, family) Then
            Return family
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' 取出某个家族中属于指定菌株的基因拷贝
    ''' </summary>
    Public Function GetOwnCopies(family As GeneFamily, strainID As String) As String()
        Dim genes As String() = Nothing

        If family IsNot Nothing AndAlso family.strainMembers IsNot Nothing AndAlso
            family.strainMembers.TryGetValue(strainID, genes) Then

            Return genes
        Else
            Return New String() {}
        End If
    End Function

    ''' <summary>
    ''' 取出某个菌株的标记基因
    ''' </summary>
    Public Function GetMarkers(strainID As String) As String()
        Dim genes As String() = Nothing

        If markers IsNot Nothing AndAlso markers.TryGetValue(strainID, genes) Then
            Return genes
        Else
            Return New String() {}
        End If
    End Function

    ''' <summary>
    ''' 取出某个菌株基因组中的全部基因
    ''' </summary>
    Public Function GetGenes(strainID As String) As String()
        Dim genes As String() = Nothing

        If genomes IsNot Nothing AndAlso genomes.TryGetValue(strainID, genes) Then
            Return genes
        Else
            Return New String() {}
        End If
    End Function

    ''' <summary>
    ''' 取出全部跨菌株共享的同源家族
    ''' </summary>
    Public Iterator Function GetSharedFamilies() As IEnumerable(Of GeneFamily)
        If families Is Nothing Then
            Return
        End If

        For Each family As GeneFamily In families.Values
            If family.isShared Then
                Yield family
            End If
        Next
    End Function

    Public Overrides Function ToString() As String
        Return $"{size} strain(s), {If(geneStrain Is Nothing, 0, geneStrain.Count)} gene(s), " &
               $"{If(homologGenes Is Nothing, 0, homologGenes.Count)} homolog gene(s)"
    End Function
End Class

''' <summary>
''' 因子与菌株的指派结果
''' </summary>
Friend Class FactorAssignment

    ''' <summary>
    ''' 与 <see cref="CommunityStructure.strains"/> 等长的因子下标数组
    ''' </summary>
    Public Property components As Integer()

    ''' <summary>
    ''' 菌株 -> (因子 -> 打分)
    ''' </summary>
    Public Property scores As Dictionary(Of String, Dictionary(Of String, Double))

    ''' <summary>
    ''' 菌株 -> 参与指派计算的基因数量
    ''' </summary>
    Public Property markerCount As Dictionary(Of String, Integer)

    ''' <summary>
    ''' 菌株 -> 参与指派计算的基因列表(优先标记基因)
    ''' </summary>
    Public Property markerGenes As Dictionary(Of String, String())
End Class

''' <summary>
''' 结构掩码精化的结果
''' </summary>
Friend Class Refinement

    ''' <summary>
    ''' 每个基因(按“已归属菌株的基因”行下标)的载荷
    ''' </summary>
    Public Property weights As Double()

    ''' <summary>
    ''' 每个菌株的活性谱(菌株下标 -> 样本向量)
    ''' </summary>
    Public Property activity As Double()()

    ''' <summary>
    ''' 实际迭代的轮数
    ''' </summary>
    Public Property iterations As Integer

    ''' <summary>
    ''' 是否因相对变化小于阈值而提前收敛
    ''' </summary>
    Public Property converged As Boolean
End Class

''' <summary>
''' 单个菌株的重构结果
''' </summary>
Friend Class StrainReconstruction

    Public Property strainID As String
    Public Property genes As String()
    Public Property values As Double()()

    ''' <summary>
    ''' 因子模型在该菌株基因行上的相对重构误差
    ''' </summary>
    Public Property residual As Double
End Class

''' <summary>
''' 分解流程的完整结果
''' </summary>
Public Class CommunityDecomposition

    ''' <summary>
    ''' 实际采用的分解路径：<c>DNMF</c>、<c>NMF</c> 或者降级路径 <c>split</c>
    ''' </summary>
    Public Property method As String

    ''' <summary>
    ''' 主要输出：菌株基因组ID -> 该菌株自己的基因表达矩阵
    ''' </summary>
    Public Property strains As Dictionary(Of String, Matrix)

    ''' <summary>
    ''' 菌株 -> 因子下标
    ''' </summary>
    Public Property assign As Dictionary(Of String, Integer)

    ''' <summary>
    ''' 菌株 -> 参与指派计算的基因数量(标记基因数量)
    ''' </summary>
    Public Property markers As Dictionary(Of String, Integer)

    ''' <summary>
    ''' 菌株 -> 参与指派计算的基因列表(优先使用无同源的标记基因)
    ''' </summary>
    Public Property markerGenes As Dictionary(Of String, String())

    ''' <summary>
    ''' 菌株 -> (因子 -> 打分)，可用于人工核对指派是否合理
    ''' </summary>
    Public Property markerScore As Dictionary(Of String, Dictionary(Of String, Double))

    ''' <summary>
    ''' 菌株活性谱矩阵：行为菌株、列为样本
    ''' </summary>
    Public Property abundance As Matrix

    ''' <summary>
    ''' 菌株 -> 因子模型的相对重构误差
    ''' </summary>
    Public Property strainResidual As Dictionary(Of String, Double)

    ''' <summary>
    ''' 总矩阵之中没有任何基因组归属的基因
    ''' </summary>
    Public Property unassignedGenes As String()

    ''' <summary>
    ''' 初始分解的目标函数值(平方误差和)
    ''' </summary>
    Public Property cost As Double

    ''' <summary>
    ''' 初始分解每一轮的目标函数值
    ''' </summary>
    Public Property errors As Double()

    ''' <summary>
    ''' 输出矩阵的样本标签
    ''' </summary>
    Public Property sampleID As String()

    ''' <summary>
    ''' 菌株数量
    ''' </summary>
    Public ReadOnly Property strainCount As Integer
        Get
            Return If(strains Is Nothing, 0, strains.Count)
        End Get
    End Property

    ''' <summary>
    ''' 把每个菌株的表达矩阵导出为 <c>outDir/&lt;基因组ID&gt;.csv</c>
    ''' </summary>
    ''' <param name="outDir">输出文件夹，不存在会自动创建</param>
    ''' <returns>成功写出的文件路径列表</returns>
    Public Function SaveAll(outDir As String) As String()
        If String.IsNullOrEmpty(outDir) Then
            Throw New ArgumentException("the output directory can not be nothing or empty!", NameOf(outDir))
        End If

        Dim files As New List(Of String)

        Call Directory.CreateDirectory(outDir)

        If strains IsNot Nothing Then
            For Each strain As KeyValuePair(Of String, Matrix) In strains
                Dim file As String = Path.Combine(outDir, $"{strain.Key}.csv")

                Call strain.Value.SaveMatrix(file)
                Call files.Add(file)
            Next
        End If

        Return files.ToArray
    End Function

    Public Overrides Function ToString() As String
        Return $"[{method}] {strainCount} strain matrix, {If(sampleID Is Nothing, 0, sampleID.Length)} sample(s)"
    End Function
End Class

''' <summary>
''' 合成菌群(synthetic community)转录组总表达矩阵的分解流程。
''' </summary>
''' <remarks>
''' <para>
''' 输入是一张以 “多株菌基因组全部基因的并集” 为行、测序样本为列的总 hits 表达矩阵。
''' 每一行基因只属于其中某一个菌株的基因组，但由于跨菌株的同源基因(直系同源/旁系同源)
''' 在并集定量时会发生 reads 交叉分配，因此单行的观测值实际上是多个菌株同源基因信号的叠加。
''' </para>
''' <para>
''' 本流程的分解由三段构成：
''' </para>
''' <list type="number">
''' <item>
''' <b>结构层</b>：由 “基因组ID -> 基因ID列表” 建立 “基因 -> 菌株” 的唯一归属；由
''' <see cref="UnionFind.GetClusters"/> 的基因家族簇识别跨菌株同源基因，从而区分
''' “无同源的标记基因(marker gene)” 与 “同源歧义基因”。
''' </item>
''' <item>
''' <b>分解层</b>：以 <c>k = 菌株数</c> 调用 <see cref="DNMFModel"/> (默认，标签为每行基因所属的菌株)
''' 或 <see cref="NMFModel"/> 得到 “基因载荷 W” 与 “菌株活性谱 H”；再以标记基因载荷均值完成
''' “因子 -> 菌株” 的最优一对一指派。
''' </item>
''' <item>
''' <b>掩码精化层</b>：加入 “一个基因只允许挂载到自身菌株因子” 的结构掩码先验，
''' 在指派固定的前提下做闭式交替非负最小二乘
''' (<c>w_g = (A_g . h_s) / (h_s . h_s)</c>、<c>h_s = sum(w_g * A_g) / sum(w_g^2)</c>)，
''' 使分解结果可辨识、可重复，且每个菌株的重构矩阵不会携带其它菌株的信号。
''' 其中菌株活性谱 <c>h_s</c> 只用该菌株的标记基因估计，避免同源基因行把其它菌株的信号
''' 通过活性谱扩散到整株基因；而 <c>w_g</c> 仍然对该菌株的全部基因行做投影。
''' </item>
''' <item>
''' <b>同源归并层</b>：同一个基因家族内的成员行是 “各菌株信号的非负线性组合”，
''' 因为活性谱之间不正交，这里用坐标下降做联合非负最小二乘
''' (<c>min ||y_g - sum_s w_g^s h_s||^2, w &gt;= 0</c>) 求出归属，
''' 再把 <c>sum_{g in C} w_g^s h_s</c> 汇总回本菌株自己的拷贝行。
''' 由于 <c>sum_s sum_{g in C} w_g^s h_s = sum_{g in C} y_g</c>，家族层面的总质量严格守恒，
''' 既不丢失信号也不会跨菌株重复计数。
''' </item>
''' </list>
''' </remarks>
Public Class Decomposition

    ''' <summary>
    ''' 从总表达矩阵中分解出每一个菌株各自的基因表达矩阵
    ''' </summary>
    ''' <param name="total">以 “多菌株基因并集” 为行、样本为列的总 hits 表达矩阵</param>
    ''' <param name="genomes">菌株基因组ID -> 该基因组中的全部基因ID</param>
    ''' <param name="families">基因并集与跨基因组同源关系(<see cref="UnionFind"/>)</param>
    ''' <param name="opt">分解参数</param>
    ''' <returns>包含每个菌株矩阵与诊断信息的结果对象</returns>
    Public Shared Function Decompose(total As Matrix,
                                     genomes As Dictionary(Of String, String()),
                                     families As UnionFind,
                                     Optional opt As DecompositionOptions = Nothing) As CommunityDecomposition

        If opt Is Nothing Then
            opt = New DecompositionOptions
        End If
        If total Is Nothing Then
            Throw New ArgumentNullException(NameOf(total), "the total expression matrix can not be nothing!")
        End If
        If total.expression Is Nothing OrElse total.expression.Length = 0 Then
            Throw New ArgumentException("the total expression matrix has no gene feature row!", NameOf(total))
        End If
        If total.sample_count = 0 Then
            Throw New ArgumentException("the total expression matrix has no sample column!", NameOf(total))
        End If

        Dim n As Integer = total.sample_count
        Dim rownames As String() = total.rownames
        Dim rowIndex As Dictionary(Of String, Integer) = IndexOfGenes(rownames)

        ' 结构层
        Dim community As CommunityStructure = BuildCommunityStructure(genomes, families, opt.Strict)

        Call $"community annotation: {community}".info
        Call $"factorize the total matrix({rowIndex.Count} genes x {n} samples) via {opt.Method}".info

        ' 非负性校验 + 稠密拷贝
        Dim A As NumericMatrix = PackMatrix(total, opt)

        ' 行 -> 菌株归属
        Dim strainIndex As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

        For i As Integer = 0 To community.strains.Length - 1
            strainIndex(community.strains(i)) = i
        Next

        Dim assignedRows As New List(Of Integer)
        Dim assignedGenes As New List(Of String)
        Dim labels As New List(Of Integer)
        Dim unassigned As New List(Of String)

        For i As Integer = 0 To rownames.Length - 1
            Dim strain As String = community.GetStrainOf(rownames(i))
            Dim si As Integer

            If strain Is Nothing Then
                Call unassigned.Add(rownames(i))
            ElseIf strainIndex.TryGetValue(strain, si) Then
                Call assignedRows.Add(i)
                Call assignedGenes.Add(rownames(i))
                Call labels.Add(si)
            Else
                Call unassigned.Add(rownames(i))
            End If
        Next

        If unassigned.Count > 0 Then
            Call $"{unassigned.Count} gene feature(s) in the total matrix have no genome assignment and will be ignored: {unassigned.Take(10).JoinBy(", ")}".warning
        End If

        Dim strains As String() = community.strains
        Dim k As Integer = strains.Length

        Dim result As New CommunityDecomposition With {
            .strains = New Dictionary(Of String, Matrix)(StringComparer.Ordinal),
            .assign = New Dictionary(Of String, Integer)(StringComparer.Ordinal),
            .markers = New Dictionary(Of String, Integer)(StringComparer.Ordinal),
            .markerGenes = New Dictionary(Of String, String())(StringComparer.Ordinal),
            .markerScore = New Dictionary(Of String, Dictionary(Of String, Double))(StringComparer.Ordinal),
            .strainResidual = New Dictionary(Of String, Double)(StringComparer.Ordinal),
            .unassignedGenes = unassigned.ToArray,
            .sampleID = total.sampleID.ToArray
        }

        ' 样本数/行数不足以支撑 k 个因子时，降级为 “按基因集切分 + 家族均分”
        If k < 2 Then
            Return SplitFallback(total, community, result, "only one strain in the community, nothing to decompose")
        End If
        If assignedRows.Count < k Then
            Return SplitFallback(total, community, result, $"not enough gene feature rows({assignedRows.Count}) for {k} strain factors")
        End If
        If n < k Then
            Return SplitFallback(total, community, result, $"not enough sample columns({n}) for {k} strain factors")
        End If

        ' 只对已归属菌株的行做分解
        Dim A2 As NumericMatrix = SubMatrix(A, assignedRows)
        Dim geneRow As New Dictionary(Of String, Integer)(assignedGenes.Count, StringComparer.Ordinal)

        For i As Integer = 0 To assignedGenes.Count - 1
            geneRow(assignedGenes(i)) = i
        Next

        ' 分解层：DNMF / NMF
        Dim labelsArray As Integer() = labels.ToArray
        Dim W As NumericMatrix = Nothing
        Dim H As NumericMatrix = Nothing
        Dim cost As Double = 0
        Dim errors As Double() = New Double() {}
        Dim method As String = "NMF"

        If opt.Method = DecompositionMethod.DNMF AndAlso DistinctLabels(labelsArray) >= 2 Then
            Try
                Dim model As DNMFModel = DNMFModel.Factorisation(
                    A:=A2,
                    labels:=labelsArray,
                    k:=k,
                    max_iterations:=std.Max(2, opt.MaxIterations),
                    tolerance:=opt.Tolerance,
                    epsilon:=opt.Epsilon,
                    lambdaWithin:=opt.LambdaWithin,
                    lambdaBetween:=opt.LambdaBetween,
                    randomSeed:=opt.RandomSeed,
                    tqdm:=opt.Tqdm)

                W = model.W
                H = model.H
                cost = model.cost
                errors = model.errors
                method = "DNMF"
            Catch ex As Exception
                Call $"DNMF factorization failed({ex.Message}), fallback to NMF!".warning
            End Try
        End If

        If W Is Nothing Then
            Try
                Dim model As NMFModel = NMFModel.Factorisation(
                    A:=A2,
                    k:=k,
                    max_iterations:=std.Max(2, opt.MaxIterations),
                    tolerance:=opt.Tolerance,
                    epsilon:=opt.Epsilon,
                    tqdm:=opt.Tqdm)

                W = model.W
                H = model.H
                cost = model.cost
                errors = model.errors
                method = "NMF"
            Catch ex As Exception
                Return SplitFallback(total, community, result, $"the NMF factorization failed: {ex.Message}")
            End Try
        End If

        result.method = method
        result.cost = cost
        result.errors = errors

        ' 因子 -> 菌株 的一对一指派
        Dim assignment As FactorAssignment = AssignFactors(W, community, geneRow, strains)
        Dim comp As Integer() = assignment.components

        For i As Integer = 0 To strains.Length - 1
            result.assign(strains(i)) = comp(i)
            result.markers(strains(i)) = assignment.markerCount(strains(i))
            result.markerGenes(strains(i)) = assignment.markerGenes(strains(i))
            result.markerScore(strains(i)) = assignment.scores(strains(i))
        Next

        Call $"factor assignment: {strains.Select(Function(s, i) $"{s} -> factor[{comp(i) + 1}]").JoinBy("; ")}".info

        ' 结构掩码精化
        Dim rowOfStrain As Integer()() = RowsOfStrains(community, strains, geneRow, markersOnly:=False)
        Dim markerRows As Integer()() = RowsOfStrains(community, strains, geneRow, markersOnly:=True)
        Dim refine As Refinement = RefineMasked(A2, rowOfStrain, markerRows, comp, W, H, opt)

        Call $"strain activity is estimated from marker genes: {strains.Select(Function(s, i) $"{s}={markerRows(i).Length}").JoinBy(", ")}".info

        Call $"masked refinement: {refine.iterations} iteration(s), converged = {refine.converged}".info

        ' 重构层
        Dim reconstructions As StrainReconstruction() = Reconstruct(A2, community, strains, rowOfStrain, geneRow, refine, opt)
        Dim scaled As Integer = 0

        If opt.RescaleToTotal Then
            scaled = RescaleToTotal(reconstructions, refine, total)
        End If

        ' 输出层
        For Each reconstruction As StrainReconstruction In reconstructions
            Dim rows As DataFrameRow() = New DataFrameRow(reconstruction.genes.Length - 1) {}

            For i As Integer = 0 To rows.Length - 1
                rows(i) = New DataFrameRow With {
                    .geneID = reconstruction.genes(i),
                    .experiments = reconstruction.values(i)
                }
            Next

            result.strains(reconstruction.strainID) = New Matrix With {
                .sampleID = total.sampleID.ToArray,
                .expression = rows,
                .tag = $"strain[{reconstruction.strainID}]|{total.tag}"
            }
            result.strainResidual(reconstruction.strainID) = reconstruction.residual
        Next

        result.abundance = CreateAbundanceMatrix(strains, refine.activity, total)

        Dim message As String = $"decomposition done({If(scaled > 0, $"rescaled {scaled} sample(s) to the total matrix", "no rescale")}): " &
                                $"{strains.Select(Function(s) $"{s}={result.strains(s).size}").JoinBy(", ")}"

        Call message.info

        Return result
    End Function

    ''' <summary>
    ''' 从总表达矩阵中分解出每一个菌株各自的基因表达矩阵，直接返回 <see cref="Matrix"/> 对象集合
    ''' </summary>
    ''' <param name="total">以 “多菌株基因并集” 为行、样本为列的总 hits 表达矩阵</param>
    ''' <param name="genomes">菌株基因组ID -> 该基因组中的全部基因ID</param>
    ''' <param name="families">基因并集与跨基因组同源关系(<see cref="UnionFind"/>)</param>
    ''' <param name="opt">分解参数</param>
    ''' <returns>菌株基因组ID -> 该菌株自己的基因表达矩阵</returns>
    Public Shared Function DecomposeMatrices(total As Matrix,
                                             genomes As Dictionary(Of String, String()),
                                             families As UnionFind,
                                             Optional opt As DecompositionOptions = Nothing) As Dictionary(Of String, Matrix)
        Return Decompose(total, genomes, families, opt).strains
    End Function

    ''' <summary>
    ''' 把分解结果之中的每个菌株矩阵导出为 CSV 文件
    ''' </summary>
    ''' <param name="result">分解结果</param>
    ''' <param name="outDir">输出文件夹</param>
    ''' <returns>成功写出的文件路径列表</returns>
    Public Shared Function SaveAll(result As CommunityDecomposition, outDir As String) As String()
        If result Is Nothing Then
            Throw New ArgumentNullException(NameOf(result))
        End If

        Return result.SaveAll(outDir)
    End Function

    ''' <summary>
    ''' 构建结构层：基因归属、基因家族与标记基因
    ''' </summary>
    ''' <param name="genomes">菌株基因组ID -> 该基因组中的全部基因ID</param>
    ''' <param name="families">基因并集与同源关系，可以为空(此时所有基因都视为无同源的标记基因)</param>
    ''' <param name="strict">基因ID跨基因组重复时是否抛出异常</param>
    Public Shared Function BuildCommunityStructure(genomes As Dictionary(Of String, String()),
                                                   families As UnionFind,
                                                   Optional strict As Boolean = True) As CommunityStructure

        If genomes Is Nothing OrElse genomes.Count = 0 Then
            Throw New ArgumentException("no strain genome is provided for the community!", NameOf(genomes))
        End If

        Dim strains As String() = genomes.Keys.ToArray
        Dim geneStrain As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Dim normalized As New Dictionary(Of String, String())(StringComparer.Ordinal)
        Dim conflicts As New List(Of String)
        Dim conflictKeys As New HashSet(Of String)(StringComparer.Ordinal)

        For Each strain As String In strains
            Dim genes As String() = genomes(strain)

            If genes Is Nothing OrElse genes.Length = 0 Then
                Call $"strain '{strain}' has no gene feature!".warning
                normalized(strain) = New String() {}
                Continue For
            End If

            Dim list As New List(Of String)(genes.Length)
            Dim seen As New HashSet(Of String)(StringComparer.Ordinal)

            For Each gene As String In genes
                Dim id As String = If(gene, "").Trim

                If id = "" OrElse Not seen.Add(id) Then
                    Continue For
                End If

                Dim owner As String = Nothing

                If geneStrain.TryGetValue(id, owner) Then
                    ' 同一个基因组内的重复基因ID直接忽略，跨基因组重复属于输入冲突
                    If Not String.Equals(owner, strain, StringComparison.Ordinal) AndAlso conflictKeys.Add(id) Then
                        Call conflicts.Add($"{id}({owner} <-> {strain})")
                    End If
                Else
                    geneStrain(id) = strain
                End If

                Call list.Add(id)
            Next

            normalized(strain) = list.ToArray
        Next

        If conflicts.Count > 0 Then
            If strict Then
                Throw New ArgumentException(
                    $"gene id must be unique across the community genomes, but {conflicts.Count} conflict(s) were found: " &
                    $"{conflicts.Take(10).JoinBy(", ")}")
            Else
                Call $"{conflicts.Count} gene id conflict(s) were found, keep the first owner: {conflicts.Take(10).JoinBy(", ")}".warning
            End If
        End If

        Dim familyIndex As New Dictionary(Of String, GeneFamily)(StringComparer.Ordinal)
        Dim homologGenes As New Dictionary(Of String, GeneFamily)(StringComparer.Ordinal)

        If families IsNot Nothing Then
            Dim clusters As Dictionary(Of String, List(Of String)) = families.GetClusters()

            For Each cluster As KeyValuePair(Of String, List(Of String)) In clusters
                Dim strainMembers As New Dictionary(Of String, List(Of String))(StringComparer.Ordinal)
                Dim members As New List(Of String)

                For Each gene As String In cluster.Value
                    Dim strain As String = Nothing

                    If gene Is Nothing OrElse Not geneStrain.TryGetValue(gene, strain) Then
                        Continue For
                    End If

                    Dim bucket As List(Of String) = Nothing

                    If Not strainMembers.TryGetValue(strain, bucket) Then
                        bucket = New List(Of String)
                        strainMembers(strain) = bucket
                    End If

                    Call bucket.Add(gene)
                    Call members.Add(gene)
                Next

                If members.Count = 0 Then
                    Continue For
                End If

                Dim family As New GeneFamily With {
                    .familyID = cluster.Key,
                    .members = members.ToArray,
                    .strainMembers = strainMembers.ToDictionary(
                        Function(a) a.Key,
                        Function(a) a.Value.ToArray)
                }

                familyIndex(cluster.Key) = family

                If family.isShared Then
                    For Each gene As String In family.members
                        homologGenes(gene) = family
                    Next
                End If
            Next
        End If

        Dim markers As New Dictionary(Of String, String())(StringComparer.Ordinal)

        For Each strain As String In strains
            Dim genes As String() = normalized(strain)
            Dim list As New List(Of String)(genes.Length)

            For Each gene As String In genes
                If Not homologGenes.ContainsKey(gene) Then
                    Call list.Add(gene)
                End If
            Next

            markers(strain) = list.ToArray

            If list.Count = 0 Then
                Call $"strain '{strain}' has no strain-specific marker gene, all of its genes are homologous to other strains!".warning
            End If
        Next

        Return New CommunityStructure With {
            .strains = strains,
            .genomes = normalized,
            .geneStrain = geneStrain,
            .families = familyIndex,
            .homologGenes = homologGenes,
            .markers = markers
        }
    End Function

    ''' <summary>
    ''' 建立基因ID -> 行下标的索引，出现重复行时抛出异常
    ''' </summary>
    Private Shared Function IndexOfGenes(rownames As String()) As Dictionary(Of String, Integer)
        Dim index As New Dictionary(Of String, Integer)(rownames.Length, StringComparer.Ordinal)

        For i As Integer = 0 To rownames.Length - 1
            Dim id As String = rownames(i)

            If index.ContainsKey(id) Then
                Throw New ArgumentException($"the total matrix contains duplicated gene feature '{id}', please merge or aggregate the rows before decomposition!")
            End If

            index(id) = i
        Next

        Return index
    End Function

    ''' <summary>
    ''' 校验非负/有限性并拷贝为 <see cref="NumericMatrix"/>
    ''' </summary>
    Private Shared Function PackMatrix(total As Matrix, opt As DecompositionOptions) As NumericMatrix
        Dim rows As DataFrameRow() = total.expression
        Dim n As Integer = total.sample_count
        Dim samples As String() = total.sampleID
        Dim data As Double()() = New Double(rows.Length - 1)() {}

        For i As Integer = 0 To rows.Length - 1
            Dim src As Double() = rows(i).experiments

            If src Is Nothing OrElse src.Length <> n Then
                Throw New InvalidProgramException($"the gene feature '{rows(i).geneID}' has {If(src Is Nothing, 0, src.Length)} sample value(s), but {n} is required!")
            End If

            Dim dst As Double() = New Double(n - 1) {}

            For j As Integer = 0 To n - 1
                Dim v As Double = src(j)

                If Not Double.IsFinite(v) Then
                    Throw New ArgumentException($"the expression value of gene '{rows(i).geneID}' at sample '{samples(j)}' is not a finite number(NaN/Infinity)!")
                End If

                If v < 0 Then
                    Throw New ArgumentException(
                        $"the expression value of gene '{rows(i).geneID}' at sample '{samples(j)}' is negative({v}), " &
                        "the non-negative matrix factorization requires a non-negative matrix; " &
                        "you can make the matrix non-negative via Matrix.Exp/MinMaxNorm or by shifting the baseline first.")
                End If

                dst(j) = v
            Next

            data(i) = dst
        Next

        Return New NumericMatrix(data)
    End Function

    ''' <summary>
    ''' 按行号抽取子矩阵
    ''' </summary>
    Private Shared Function SubMatrix(A As NumericMatrix, rows As List(Of Integer)) As NumericMatrix
        Dim n As Integer = A.ColumnDimension
        Dim data As Double()() = New Double(rows.Count - 1)() {}

        For i As Integer = 0 To rows.Count - 1
            Dim r As Integer = rows(i)
            Dim dst As Double() = New Double(n - 1) {}

            For j As Integer = 0 To n - 1
                dst(j) = A(r, j)
            Next

            data(i) = dst
        Next

        Return New NumericMatrix(data)
    End Function

    Private Shared Function DistinctLabels(labels As Integer()) As Integer
        Dim distinct As New HashSet(Of Integer)

        For Each label As Integer In labels
            Call distinct.Add(label)
        Next

        Return distinct.Count
    End Function

    ''' <summary>
    ''' 以标记基因在载荷矩阵 <c>W</c> 上的均值完成 “因子 -> 菌株” 的最优一对一指派
    ''' </summary>
    Private Shared Function AssignFactors(W As NumericMatrix,
                                          community As CommunityStructure,
                                          geneRow As Dictionary(Of String, Integer),
                                          strains As String()) As FactorAssignment

        Dim k As Integer = W.ColumnDimension
        Dim score(strains.Length - 1)() As Double
        Dim scores As New Dictionary(Of String, Dictionary(Of String, Double))(StringComparer.Ordinal)
        Dim markerCount As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
        Dim markerGenes As New Dictionary(Of String, String())(StringComparer.Ordinal)

        For si As Integer = 0 To strains.Length - 1
            Dim strain As String = strains(si)
            Dim markers As String() = community.GetMarkers(strain)
            Dim used As New List(Of String)(markers.Length)

            For Each gene As String In markers
                If geneRow.ContainsKey(gene) Then
                    Call used.Add(gene)
                End If
            Next

            If used.Count = 0 Then
                ' 该菌株没有可用的标记基因(基因组之间同源性过高)，退化为使用其全部基因
                For Each gene As String In community.GetGenes(strain)
                    If geneRow.ContainsKey(gene) Then
                        Call used.Add(gene)
                    End If
                Next

                Call $"strain '{strain}' has no measured marker gene, factor assignment falls back to all of its genes({used.Count})".warning
            End If

            Dim row As Double() = New Double(k - 1) {}

            For c As Integer = 0 To k - 1
                Dim sum As Double = 0

                For Each gene As String In used
                    sum += W(geneRow(gene), c)
                Next

                row(c) = If(used.Count > 0, sum / used.Count, 0)
            Next

            score(si) = row
            markerCount(strain) = used.Count
            markerGenes(strain) = used.ToArray

            Dim table As New Dictionary(Of String, Double)(StringComparer.Ordinal)

            For c As Integer = 0 To k - 1
                table($"factor{c + 1}") = row(c)
            Next

            scores(strain) = table
        Next

        Dim comp As Integer()

        If k <= 8 Then
            comp = BestPermutation(score, k)
        Else
            comp = GreedyAssignment(score, k)
        End If

        Return New FactorAssignment With {
            .components = comp,
            .scores = scores,
            .markerCount = markerCount,
            .markerGenes = markerGenes
        }
    End Function

    ''' <summary>
    ''' 枚举全排列求最大总得分的指派(<c>k &lt;= 8</c> 时使用，结果确定且与顺序无关)
    ''' </summary>
    Private Shared Function BestPermutation(score As Double()(), k As Integer) As Integer()
        Dim current As Integer() = New Integer(k - 1) {}
        Dim counters As Integer() = New Integer(k - 1) {}

        For i As Integer = 0 To k - 1
            current(i) = i
        Next

        Dim best As Integer() = CType(current.Clone(), Integer())
        Dim bestScore As Double = PermutationScore(score, current)
        Dim i2 As Integer = 0

        While i2 < k
            If counters(i2) < i2 Then
                Dim tmp As Integer

                If i2 Mod 2 = 0 Then
                    tmp = current(0)
                    current(0) = current(i2)
                    current(i2) = tmp
                Else
                    tmp = current(counters(i2))
                    current(counters(i2)) = current(i2)
                    current(i2) = tmp
                End If

                Dim s As Double = PermutationScore(score, current)

                If s > bestScore Then
                    bestScore = s
                    best = CType(current.Clone(), Integer())
                End If

                counters(i2) += 1
                i2 = 0
            Else
                counters(i2) = 0
                i2 += 1
            End If
        End While

        Return best
    End Function

    Private Shared Function PermutationScore(score As Double()(), perm As Integer()) As Double
        Dim sum As Double = 0

        For i As Integer = 0 To perm.Length - 1
            sum += score(i)(perm(i))
        Next

        Return sum
    End Function

    ''' <summary>
    ''' 贪心指派(因子数大于 8 时使用)
    ''' </summary>
    Private Shared Function GreedyAssignment(score As Double()(), k As Integer) As Integer()
        Dim pairs As New List(Of (s As Integer, c As Integer, v As Double))

        For si As Integer = 0 To score.Length - 1
            For c As Integer = 0 To k - 1
                Call pairs.Add((si, c, score(si)(c)))
            Next
        Next

        Call pairs.Sort(Function(a, b) b.v.CompareTo(a.v))

        Dim comp As Integer() = New Integer(k - 1) {}
        Dim usedStrain As New HashSet(Of Integer)
        Dim usedFactor As New HashSet(Of Integer)

        For Each pair In pairs
            If usedStrain.Contains(pair.s) OrElse usedFactor.Contains(pair.c) Then
                Continue For
            End If

            comp(pair.s) = pair.c
            Call usedStrain.Add(pair.s)
            Call usedFactor.Add(pair.c)

            If usedStrain.Count = k Then
                Exit For
            End If
        Next

        Return comp
    End Function

    ''' <summary>
    ''' 计算每个菌株在 “已归属菌株的基因行” 之中的行下标
    ''' </summary>
    ''' <param name="community">结构层信息</param>
    ''' <param name="strains">菌株ID列表</param>
    ''' <param name="geneRow">基因ID -> 分解矩阵行下标</param>
    ''' <param name="markersOnly">
    ''' True = 只返回该菌株的标记基因(无跨菌株同源)行，用于估计菌株活性谱；
    ''' False = 返回该菌株的全部基因行
    ''' </param>
    Private Shared Function RowsOfStrains(community As CommunityStructure,
                                          strains As String(),
                                          geneRow As Dictionary(Of String, Integer),
                                          markersOnly As Boolean) As Integer()()

        Dim rows(strains.Length - 1)() As Integer

        For si As Integer = 0 To strains.Length - 1
            Dim list As New List(Of Integer)
            Dim genes As String() = If(markersOnly,
                                       community.GetMarkers(strains(si)),
                                       community.GetGenes(strains(si)))

            For Each gene As String In genes
                Dim r As Integer

                If geneRow.TryGetValue(gene, r) Then
                    Call list.Add(r)
                End If
            Next

            rows(si) = list.ToArray
        Next

        Return rows
    End Function

    ''' <summary>
    ''' 结构掩码交替非负精化
    ''' </summary>
    ''' <remarks>
    ''' 固定 “因子 -> 菌株” 的指派之后，模型退化为
    ''' <c>A[g] ≈ w_g * h_{s(g)}</c>：每个基因行只是其所属菌株活性谱的一个非负倍数。
    ''' 于是 <c>w_g</c> 与 <c>h_s</c> 都存在闭式最小二乘解，交替更新即可单调下降且天然非负；
    ''' 不同菌株只写自己的行槽位与自己的活性谱，因此可以安全地按菌株并行。
    ''' 
    ''' 其中 <c>h_s</c> 只用该菌株的标记基因(<paramref name="markerRows"/>)来估计：
    ''' 同源基因行携带了其它菌株的信号，若参与活性谱更新会把偏差扩散到整株基因；
    ''' 而 <c>w_g</c> 仍然对该菌株的全部基因行做投影，保证每条基因都有重构值。
    ''' 当某菌株没有可用的标记基因时自动退化为使用其全部基因行。
    ''' </remarks>
    Private Shared Function RefineMasked(A2 As NumericMatrix,
                                         rowOfStrain As Integer()(),
                                         markerRows As Integer()(),
                                         comp As Integer(),
                                         W As NumericMatrix,
                                         H As NumericMatrix,
                                         opt As DecompositionOptions) As Refinement

        Dim m As Integer = A2.RowDimension
        Dim n As Integer = A2.ColumnDimension
        Dim eps As Double = If(opt.Epsilon > 0, opt.Epsilon, 0.000001)
        Dim weights As Double() = New Double(m - 1) {}
        Dim activity As Double()() = New Double(comp.Length - 1)() {}

        For si As Integer = 0 To comp.Length - 1
            Dim rows As Integer() = rowOfStrain(si)
            Dim profile As Double() = New Double(n - 1) {}
            Dim usable As Boolean = False

            For j As Integer = 0 To n - 1
                Dim value As Double = H(comp(si), j)

                If value > eps Then
                    usable = True
                End If
                If value < eps Then
                    value = eps
                End If

                profile(j) = value
            Next

            ' 初始分解给出的活性谱退化(全为 0)时，用该菌株基因行的均值作为起点，避免迭代被 eps 卡死
            If Not usable AndAlso rows.Length > 0 Then
                For j As Integer = 0 To n - 1
                    profile(j) = 0
                Next

                For Each r As Integer In rows
                    For j As Integer = 0 To n - 1
                        profile(j) += A2(r, j)
                    Next
                Next

                For j As Integer = 0 To n - 1
                    profile(j) = profile(j) / rows.Length + eps
                Next
            End If

            activity(si) = profile

            For Each r As Integer In rows
                Dim value As Double = W(r, comp(si))

                If value < eps Then
                    value = eps
                End If

                weights(r) = value
            Next
        Next

        Dim maxIterations As Integer = std.Max(0, opt.RefineIterations)
        Dim tolerance As Double = If(opt.RefineTolerance > 0, opt.RefineTolerance, 0.000001)
        Dim changes As Double() = New Double(comp.Length - 1) {}
        Dim iterations As Integer = 0
        Dim converged As Boolean = False

        Dim body As Action(Of Integer) =
            Sub(si As Integer)
                Dim rows As Integer() = rowOfStrain(si)
                Dim profile As Double() = activity(si)

                ' 菌株活性谱优先由无跨菌株同源的标记基因估计：
                ' 同源基因行携带了其它菌株的信号，如果参与活性谱的估计会把偏差传递给整株基因。
                Dim profileRows As Integer() = If(markerRows(si).Length > 0, markerRows(si), rows)

                If rows.Length = 0 OrElse profileRows.Length = 0 Then
                    changes(si) = 0
                    Return
                End If

                Dim denominator As Double = 0

                For j As Integer = 0 To n - 1
                    denominator += profile(j) * profile(j)
                Next

                If denominator <= eps Then
                    changes(si) = 0
                    Return
                End If

                Dim wChange As Double = 0
                Dim wTotal As Double = 0

                For Each r As Integer In rows
                    Dim dot As Double = 0

                    For j As Integer = 0 To n - 1
                        dot += A2(r, j) * profile(j)
                    Next

                    Dim weight As Double = dot / denominator

                    If weight < eps Then
                        weight = eps
                    End If

                    wChange += std.Abs(weight - weights(r))
                    wTotal += std.Abs(weight)
                    weights(r) = weight
                Next

                Dim numerator As Double() = New Double(n - 1) {}
                Dim wSquare As Double = 0

                For Each r As Integer In profileRows
                    Dim weight As Double = weights(r)

                    wSquare += weight * weight

                    For j As Integer = 0 To n - 1
                        numerator(j) += weight * A2(r, j)
                    Next
                Next

                If wSquare <= eps Then
                    changes(si) = 0
                    Return
                End If

                Dim hChange As Double = 0
                Dim hTotal As Double = 0

                For j As Integer = 0 To n - 1
                    Dim value As Double = numerator(j) / wSquare

                    If value < eps Then
                        value = eps
                    End If

                    hChange += std.Abs(value - profile(j))
                    hTotal += std.Abs(value)
                    profile(j) = value
                Next

                changes(si) = (wChange + hChange) / (wTotal + hTotal + eps)
            End Sub

        Dim parallel As Boolean = comp.Length > 1 AndAlso
                                  opt.DegreeOfParallelism <> 1 AndAlso
                                  CLng(m) * n >= 200000L

        For iterations = 1 To maxIterations
            If parallel Then
                Dim options As New System.Threading.Tasks.ParallelOptions()

                ' Parallel 必须全限定：Microsoft.VisualBasic.Parallel 会遮蔽同名类型
                options.MaxDegreeOfParallelism = If(opt.DegreeOfParallelism > 0, opt.DegreeOfParallelism, -1)
                Call System.Threading.Tasks.Parallel.For(0, comp.Length, options, body)
            Else
                For si As Integer = 0 To comp.Length - 1
                    Call body(si)
                Next
            End If

            Dim epsilon As Double = 0

            For si As Integer = 0 To changes.Length - 1
                epsilon = std.Max(epsilon, changes(si))
            Next

            If epsilon <= tolerance Then
                converged = True
                Exit For
            End If
        Next

        Return New Refinement With {
            .weights = weights,
            .activity = activity,
            .iterations = std.Min(iterations, maxIterations),
            .converged = converged
        }
    End Function

    ''' <summary>
    ''' 按菌株重构表达矩阵，并把同源家族的共享信号归并到本菌株的基因拷贝行
    ''' </summary>
    Private Shared Function Reconstruct(A2 As NumericMatrix,
                                        community As CommunityStructure,
                                        strains As String(),
                                        rowOfStrain As Integer()(),
                                        geneRow As Dictionary(Of String, Integer),
                                        refine As Refinement,
                                        opt As DecompositionOptions) As StrainReconstruction()

        Dim n As Integer = A2.ColumnDimension
        Dim eps As Double = If(opt.Epsilon > 0, opt.Epsilon, 0.000001)
        Dim result As StrainReconstruction() = New StrainReconstruction(strains.Length - 1) {}
        Dim homologShares As Dictionary(Of String, Dictionary(Of String, Double())) = Nothing

        If opt.RedistributeHomologs Then
            homologShares = BuildHomologShares(A2, community, strains, geneRow, refine)
        End If

        For si As Integer = 0 To strains.Length - 1
            Dim strain As String = strains(si)
            Dim h As Double() = refine.activity(si)
            Dim genes As New List(Of String)
            Dim values As New List(Of Double())
            Dim outIndex As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For Each gene As String In community.GetGenes(strain)
                Dim r As Integer

                If geneRow.TryGetValue(gene, r) Then
                    Dim v As Double() = New Double(n - 1) {}
                    Dim w As Double = refine.weights(r)

                    For j As Integer = 0 To n - 1
                        v(j) = w * h(j)
                    Next

                    outIndex(gene) = genes.Count
                    Call genes.Add(gene)
                    Call values.Add(v)
                ElseIf opt.IncludeSilentGenes Then
                    outIndex(gene) = genes.Count
                    Call genes.Add(gene)
                    Call values.Add(New Double(n - 1) {})
                End If
            Next

            Dim ownShares As Dictionary(Of String, Double()) = Nothing

            If homologShares IsNot Nothing AndAlso homologShares.TryGetValue(strain, ownShares) AndAlso ownShares IsNot Nothing Then
                For Each share As KeyValuePair(Of String, Double()) In ownShares
                    Dim idx As Integer

                    If outIndex.TryGetValue(share.Key, idx) Then
                        values(idx) = share.Value
                    End If
                Next
            End If

            ' 因子模型在该菌株基因行上的相对重构误差
            Dim numeratorError As Double = 0
            Dim denominatorError As Double = 0

            For Each r As Integer In rowOfStrain(si)
                Dim w As Double = refine.weights(r)

                For j As Integer = 0 To n - 1
                    Dim d As Double = A2(r, j) - w * h(j)

                    numeratorError += d * d
                    denominatorError += A2(r, j) * A2(r, j)
                Next
            Next

            result(si) = New StrainReconstruction With {
                .strainID = strain,
                .genes = genes.ToArray,
                .values = values.ToArray,
                .residual = std.Sqrt(numeratorError / std.Max(denominatorError, eps))
            }
        Next

        Return result
    End Function

    ''' <summary>
    ''' 同源家族信号归并：把家族内每一个成员行对本菌株的贡献汇总后写回本菌株自己的基因拷贝行
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 对家族 <c>C</c> 中的任一行 <c>g</c>，其观测值都可以写成 “各菌株活性谱的非负线性组合”
    ''' <c>y_g ≈ sum_s w_g^s * h_s</c>，其中 <c>w^s</c> 由非负最小二乘(坐标下降)联合求解。
    ''' 之所以要联合求解而不是像无同源的标记基因那样做单谱投影，是因为活性谱之间并不正交，
    ''' 逐谱投影会把同一份信号重复计入多个菌株。
    ''' </para>
    ''' <para>
    ''' 菌株 <c>s</c> 在该家族上的真实表达由家族内所有成员行共同承载(reads 交叉分配)，
    ''' 因此把 <c>sum_{g in C} w_g^s * h_s</c> 汇总后写回 <c>s</c> 自己的拷贝行；
    ''' 由于 <c>sum_s sum_{g in C} w_g^s h_s = sum_{g in C} y_g</c>，
    ''' 家族层面的总质量严格守恒，既不丢信号也不跨菌株重复计数。
    ''' </para>
    ''' </remarks>
    Private Shared Function BuildHomologShares(A2 As NumericMatrix,
                                               community As CommunityStructure,
                                               strains As String(),
                                               geneRow As Dictionary(Of String, Integer),
                                               refine As Refinement) As Dictionary(Of String, Dictionary(Of String, Double()))

        Dim n As Integer = A2.ColumnDimension
        Dim eps As Double = 0.000001
        Dim strainIndex As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
        Dim shares As New Dictionary(Of String, Dictionary(Of String, Double()))(StringComparer.Ordinal)

        For i As Integer = 0 To strains.Length - 1
            strainIndex(strains(i)) = i
        Next

        For Each family As GeneFamily In community.GetSharedFamilies()
            Dim candidates As New List(Of Integer)

            For Each strain As String In family.strainMembers.Keys
                Dim si As Integer

                If strainIndex.TryGetValue(strain, si) Then
                    Call candidates.Add(si)
                End If
            Next

            If candidates.Count < 2 Then
                Continue For
            End If

            Dim acc As Double()() = New Double(candidates.Count - 1)() {}

            For i As Integer = 0 To candidates.Count - 1
                acc(i) = New Double(n - 1) {}
            Next

            Dim any As Boolean = False

            For Each gene As String In family.members
                Dim r As Integer

                If Not geneRow.TryGetValue(gene, r) Then
                    Continue For
                End If

                Dim y As Double() = New Double(n - 1) {}

                For j As Integer = 0 To n - 1
                    y(j) = A2(r, j)
                Next

                Dim attribution As Double() = FitAttribution(y, refine.activity, candidates.ToArray)

                For i As Integer = 0 To candidates.Count - 1
                    If attribution(i) <= 0 Then
                        Continue For
                    End If

                    Dim h As Double() = refine.activity(candidates(i))

                    For j As Integer = 0 To n - 1
                        acc(i)(j) += attribution(i) * h(j)
                    Next

                    any = True
                Next
            Next

            If Not any Then
                Continue For
            End If

            For i As Integer = 0 To candidates.Count - 1
                Dim strain As String = strains(candidates(i))
                Dim own As String() = community.GetOwnCopies(family, strain)
                Dim total As Double = 0

                For j As Integer = 0 To n - 1
                    total += acc(i)(j)
                Next

                If own.Length = 0 OrElse total <= eps Then
                    Continue For
                End If

                Dim share As Double() = New Double(n - 1) {}

                For j As Integer = 0 To n - 1
                    share(j) = acc(i)(j) / own.Length
                Next

                Dim table As Dictionary(Of String, Double()) = Nothing

                If Not shares.TryGetValue(strain, table) Then
                    table = New Dictionary(Of String, Double())(StringComparer.Ordinal)
                    shares(strain) = table
                End If

                For Each gene As String In own
                    table(gene) = CType(share.Clone(), Double())
                Next
            Next
        Next

        Return shares
    End Function

    ''' <summary>
    ''' 用坐标下降求解非负最小二乘 <c>min ||y - sum_c w_c * h_c||^2, w_c &gt;= 0</c>
    ''' </summary>
    ''' <param name="y">一行(一个基因)的观测表达向量</param>
    ''' <param name="profiles">菌株活性谱(按菌株下标索引)</param>
    ''' <param name="candidates">参与拟合的菌株下标(只有该家族的成员菌株才参与)</param>
    ''' <returns>与 <paramref name="candidates"/> 等长的非负权重</returns>
    ''' <remarks>
    ''' 坐标下降对于这个凸问题可以收敛到全局最优，每个坐标的更新都是闭式解
    ''' <c>w_c = (r . h_c) / (h_c . h_c) + w_c</c>(<c>r</c> 为当前残差)，
    ''' 并且天然保持非负。活性谱之间不正交，因此不能退化为逐谱的单变量投影。
    ''' </remarks>
    Private Shared Function FitAttribution(y As Double(), profiles As Double()(), candidates As Integer()) As Double()
        Dim k As Integer = candidates.Length
        Dim weights As Double() = New Double(k - 1) {}
        Dim residual As Double() = CType(y.Clone(), Double())
        Dim scale As Double = 0

        For j As Integer = 0 To y.Length - 1
            scale = std.Max(scale, std.Abs(y(j)))
        Next

        If scale <= 0 Then
            Return weights
        End If

        Dim tolerance As Double = scale * 0.0000000001

        For sweep As Integer = 1 To 100
            Dim delta As Double = 0

            For c As Integer = 0 To k - 1
                Dim h As Double() = profiles(candidates(c))
                Dim denominator As Double = 0
                Dim numerator As Double = 0

                For j As Integer = 0 To h.Length - 1
                    denominator += h(j) * h(j)
                    numerator += residual(j) * h(j)
                Next

                If denominator <= 0 Then
                    Continue For
                End If

                Dim value As Double = numerator / denominator + weights(c)

                If value < 0 Then
                    value = 0
                End If

                Dim diff As Double = value - weights(c)

                If diff <> 0 Then
                    For j As Integer = 0 To h.Length - 1
                        residual(j) -= diff * h(j)
                    Next

                    weights(c) = value
                    delta = std.Max(delta, std.Abs(diff))
                End If
            Next

            If delta <= tolerance Then
                Exit For
            End If
        Next

        Return weights
    End Function

    ''' <summary>
    ''' 按列把重构总量校正到总矩阵的列总量，返回被校正的样本数量
    ''' </summary>
    Private Shared Function RescaleToTotal(reconstructions As StrainReconstruction(),
                                           refine As Refinement,
                                           total As Matrix) As Integer

        Dim n As Integer = total.sample_count
        Dim observed As Double() = New Double(n - 1) {}

        For Each gene As DataFrameRow In total.expression
            For j As Integer = 0 To n - 1
                observed(j) += gene.experiments(j)
            Next
        Next

        Dim computed As Double() = New Double(n - 1) {}

        For Each reconstruction As StrainReconstruction In reconstructions
            For Each v As Double() In reconstruction.values
                For j As Integer = 0 To n - 1
                    computed(j) += v(j)
                Next
            Next
        Next

        Dim scale As Double() = New Double(n - 1) {}
        Dim scaled As Integer = 0

        For j As Integer = 0 To n - 1
            If computed(j) > 0.000000001 Then
                scale(j) = observed(j) / computed(j)
            Else
                scale(j) = 1
            End If

            If std.Abs(scale(j) - 1) > 0.000000001 Then
                scaled += 1
            End If
        Next

        If scaled = 0 Then
            Return 0
        End If

        For Each reconstruction As StrainReconstruction In reconstructions
            For Each v As Double() In reconstruction.values
                For j As Integer = 0 To n - 1
                    v(j) *= scale(j)
                Next
            Next
        Next

        For si As Integer = 0 To refine.activity.Length - 1
            Dim h As Double() = refine.activity(si)

            For j As Integer = 0 To n - 1
                h(j) *= scale(j)
            Next
        Next

        Return scaled
    End Function

    Private Shared Function CreateAbundanceMatrix(strains As String(),
                                                  activity As Double()(),
                                                  total As Matrix) As Matrix
        Dim rows As DataFrameRow() = New DataFrameRow(strains.Length - 1) {}

        For i As Integer = 0 To strains.Length - 1
            rows(i) = New DataFrameRow With {
                .geneID = strains(i),
                .experiments = CType(activity(i).Clone(), Double())
            }
        Next

        Return New Matrix With {
            .sampleID = total.sampleID.ToArray,
            .expression = rows,
            .tag = $"abundance({total.tag})"
        }
    End Function

    ''' <summary>
    ''' 降级路径：不做分解，直接按基因集切分总矩阵的行，并把同源家族的信号在菌株之间均分
    ''' </summary>
    ''' <remarks>
    ''' 只在样本数或基因行数不足以支撑 <c>k = 菌株数</c> 的非负分解，或者分解算法抛出异常时使用。
    ''' 结果会保留 <see cref="CommunityDecomposition.method"/> = <c>split</c> 以及告警信息，
    ''' 不会静默地伪装成正常分解。
    ''' </remarks>
    Private Shared Function SplitFallback(total As Matrix,
                                          community As CommunityStructure,
                                          result As CommunityDecomposition,
                                          reason As String) As CommunityDecomposition

        Call $"decomposition is degraded to the row splitting mode: {reason}".warning

        Dim n As Integer = total.sample_count
        Dim strains As String() = community.strains
        Dim values As New Dictionary(Of String, Dictionary(Of String, Double()))(StringComparer.Ordinal)

        For Each strain As String In strains
            Dim table As New Dictionary(Of String, Double())(StringComparer.Ordinal)

            For Each gene As String In community.GetGenes(strain)
                Dim row As DataFrameRow = total(gene)

                If row IsNot Nothing Then
                    table(gene) = row.experiments.ToArray
                End If
            Next

            values(strain) = table
        Next

        ' 同源家族：家族总量在本家族覆盖的菌株之间均分(家族质量守恒)
        For Each family As GeneFamily In community.GetSharedFamilies()
            Dim acc As Double() = New Double(n - 1) {}
            Dim any As Boolean = False

            For Each gene As String In family.members
                Dim row As DataFrameRow = total(gene)

                If row IsNot Nothing Then
                    For j As Integer = 0 To n - 1
                        acc(j) += row.experiments(j)
                    Next

                    any = True
                End If
            Next

            If Not any Then
                Continue For
            End If

            Dim strainCount As Integer = family.strainCount

            For Each strain As String In family.strainMembers.Keys
                Dim own As String() = community.GetOwnCopies(family, strain)
                Dim table As Dictionary(Of String, Double()) = Nothing

                If own.Length = 0 OrElse Not values.TryGetValue(strain, table) Then
                    Continue For
                End If

                Dim share As Double() = New Double(n - 1) {}

                For j As Integer = 0 To n - 1
                    share(j) = acc(j) / strainCount / own.Length
                Next

                For Each gene As String In own
                    table(gene) = CType(share.Clone(), Double())
                Next
            Next
        Next

        Dim abundanceRows As DataFrameRow() = New DataFrameRow(strains.Length - 1) {}

        For si As Integer = 0 To strains.Length - 1
            Dim strain As String = strains(si)
            Dim table As Dictionary(Of String, Double()) = values(strain)
            Dim genes As String() = table.Keys.ToArray
            Dim rows As DataFrameRow() = New DataFrameRow(genes.Length - 1) {}
            Dim sum As Double() = New Double(n - 1) {}

            For i As Integer = 0 To genes.Length - 1
                Dim v As Double() = table(genes(i))

                rows(i) = New DataFrameRow With {
                    .geneID = genes(i),
                    .experiments = v
                }

                For j As Integer = 0 To n - 1
                    sum(j) += v(j)
                Next
            Next

            result.strains(strain) = New Matrix With {
                .sampleID = total.sampleID.ToArray,
                .expression = rows,
                .tag = $"split[{strain}]|{total.tag}"
            }
            result.strainResidual(strain) = Double.NaN

            abundanceRows(si) = New DataFrameRow With {
                .geneID = strain,
                .experiments = sum
            }
        Next

        result.method = "split"
        result.abundance = New Matrix With {
            .sampleID = total.sampleID.ToArray,
            .expression = abundanceRows,
            .tag = $"abundance({total.tag})"
        }

        Return result
    End Function
End Class
