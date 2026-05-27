' ============================================================================
' CorrelationComputation.vb
' OTU表格与代谢物表达矩阵之间的 SparCC / CCLasso / Pearson / Spearman 
' 四种相关性计算的 VB.NET 完整实现
'
' 数据结构约定:
'   matrix: { sample_id: String(), expression: { id:String, expression: Double() }[] }
'
' 算法参考:
'   Pearson   — 经典皮尔逊积矩相关系数
'   Spearman  — 秩相关系数（Pearson on ranks）
'   SparCC    — Friedman & Alm (2012) PLoS Comput Biol, 组成型数据相关性推断
'   CCLasso   — Fang et al. (2015) Bioinformatics, 基于Lasso的组成型数据协方差估计
' ============================================================================

Imports System.Math
Imports Microsoft.VisualBasic.Math.Correlations.Correlations
Imports Microsoft.VisualBasic.Math.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Model.Network.Regulons

' ============================================================================
' 第二部分：数学辅助函数
' ============================================================================

Public Module MathHelpers

    ''' <summary>
    ''' 计算算术均值
    ''' </summary>
    Public Function Mean(data As Double()) As Double
        If data Is Nothing OrElse data.Length = 0 Then Return 0.0
        Dim sum As Double = 0.0
        For i As Integer = 0 To data.Length - 1
            sum += data(i)
        Next
        Return sum / data.Length
    End Function

    ''' <summary>
    ''' 计算无偏样本方差（除以 n-1）
    ''' </summary>
    Public Function Variance(data As Double()) As Double
        If data Is Nothing OrElse data.Length < 2 Then Return 0.0
        Dim m As Double = Mean(data)
        Dim ss As Double = 0.0
        For i As Integer = 0 To data.Length - 1
            ss += (data(i) - m) * (data(i) - m)
        Next
        Return ss / (data.Length - 1)
    End Function

    ''' <summary>
    ''' 计算标准差
    ''' </summary>
    Public Function StdDev(data As Double()) As Double
        Return Sqrt(Variance(data))
    End Function

    ''' <summary>
    ''' 计算无偏样本协方差（除以 n-1）
    ''' </summary>
    Public Function Covariance(x As Double(), y As Double()) As Double
        If x Is Nothing OrElse y Is Nothing Then Return 0.0
        If x.Length <> y.Length OrElse x.Length < 2 Then Return 0.0
        Dim mx As Double = Mean(x)
        Dim my As Double = Mean(y)
        Dim ss As Double = 0.0
        For i As Integer = 0 To x.Length - 1
            ss += (x(i) - mx) * (y(i) - my)
        Next
        Return ss / (x.Length - 1)
    End Function

    ''' <summary>
    ''' 计算秩（Rank），对结值（ties）使用平均秩
    ''' </summary>
    Public Function Rank(data As Double()) As Double()
        If data Is Nothing OrElse data.Length = 0 Then Return New Double() {}
        Dim n As Integer = data.Length
        Dim ranks As Double() = New Double(n - 1) {}
        Dim indices As Integer() = New Integer(n - 1) {}
        For k As Integer = 0 To n - 1
            indices(k) = k
        Next

        ' 按值排序索引
        Array.Sort(indices, Function(a, b) data(a).CompareTo(data(b)))

        ' 分配秩，处理结值
        Dim i As Integer = 0
        While i < n
            Dim j As Integer = i
            While j < n - 1 AndAlso data(indices(j + 1)) = data(indices(j))
                j += 1
            End While
            ' 平均秩（1-based）
            Dim avgRank As Double = (i + j) / 2.0 + 1.0
            For k As Integer = i To j
                ranks(indices(k)) = avgRank
            Next
            i = j + 1
        End While

        Return ranks
    End Function

    ''' <summary>
    ''' 软阈值函数（Soft Thresholding），Lasso 坐标下降的核心算子
    ''' S(x, λ) = sign(x) * max(|x| - λ, 0)
    ''' </summary>
    Public Function SoftThreshold(x As Double, lambda As Double) As Double
        If x > lambda Then Return x - lambda
        If x < -lambda Then Return x + lambda
        Return 0.0
    End Function

    ''' <summary>
    ''' 将值限制在 [minVal, maxVal] 范围内
    ''' </summary>
    Public Function Clamp(value As Double, minVal As Double, maxVal As Double) As Double
        Return Max(minVal, Min(maxVal, value))
    End Function

    ''' <summary>
    ''' Fisher z 变换：将相关系数 r 转换为近似正态分布的 z 值
    ''' z = 0.5 * ln((1+r)/(1-r))
    ''' </summary>
    Public Function FisherZ(r As Double) As Double
        r = Clamp(r, -0.9999999, 0.9999999)
        Return 0.5 * Log((1.0 + r) / (1.0 - r))
    End Function

    ''' <summary>
    ''' Fisher z 逆变换
    ''' </summary>
    Public Function FisherZInv(z As Double) As Double
        Dim e2z As Double = Exp(2.0 * z)
        Return (e2z - 1.0) / (e2z + 1.0)
    End Function

    ''' <summary>
    ''' 标准正态分布累积分布函数（Abramowitz & Stegun 近似）
    ''' </summary>
    Public Function NormalCDF(x As Double) As Double
        Const a1 As Double = 0.254829592
        Const a2 As Double = -0.284496736
        Const a3 As Double = 1.421413741
        Const a4 As Double = -1.453152027
        Const a5 As Double = 1.061405429
        Const p As Double = 0.3275911

        Dim sign As Integer = If(x < 0, -1, 1)
        x = Abs(x) / Sqrt(2.0)

        Dim t As Double = 1.0 / (1.0 + p * x)
        Dim y As Double = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Exp(-x * x)

        Return 0.5 * (1.0 + sign * y)
    End Function

    ''' <summary>
    ''' 基于 Fisher z 变换计算相关系数的近似双侧 p 值
    ''' </summary>
    Public Function CorrelationPValue(r As Double, n As Integer) As Double
        If n < 4 Then Return 1.0
        r = Clamp(r, -0.9999999, 0.9999999)
        ' Fisher z 变换后近似服从 N(0, 1/(n-3))
        Dim z As Double = FisherZ(r) * Sqrt(n - 3.0)
        ' 双侧 p 值
        Return 2.0 * (1.0 - NormalCDF(Abs(z)))
    End Function

    ''' <summary>
    ''' 从 ExpressionMatrix 提取二维数组 [feature, sample]
    ''' </summary>
    Public Function MatrixTo2DArray(matrix As Matrix) As Double(,)
        If matrix Is Nothing OrElse matrix.expression Is Nothing Then Return Nothing
        Dim nF As Integer = matrix.size
        Dim nS As Integer = matrix.sample_count
        Dim result As Double(,) = New Double(nF - 1, nS - 1) {}
        For f As Integer = 0 To nF - 1
            For s As Integer = 0 To nS - 1
                result(f, s) = matrix(f).experiments(s)
            Next
        Next
        Return result
    End Function

    ''' <summary>
    ''' 获取二维数组中某 feature 的所有样本数据
    ''' </summary>
    Public Function GetFeatureColumn(data As Double(,), featureIdx As Integer, nSamples As Integer) As Double()
        Dim col As Double() = New Double(nSamples - 1) {}
        For s As Integer = 0 To nSamples - 1
            col(s) = data(featureIdx, s)
        Next
        Return col
    End Function

    Friend Function CreateCrossCorrelation(otuMatrix As Matrix, metaboliteMatrix As Matrix, cor As Double(,), name As String) As CrossOmicsCorrelation
        Dim nOtu As Integer = otuMatrix.size
        Dim nMet As Integer = metaboliteMatrix.size
        Dim nS As Integer = otuMatrix.sample_count

        Dim OtuIds(nOtu) As String
        Dim MetaboliteIds(nMet) As String

        For i As Integer = 0 To nOtu - 1
            OtuIds(i) = otuMatrix(i).geneID
        Next
        For j As Integer = 0 To nMet - 1
            MetaboliteIds(j) = metaboliteMatrix(j).geneID
        Next

        Dim CorrelationMatrix As New NamedSparseMatrix()
        Dim PValueMatrix As New NamedSparseMatrix()

        For i As Integer = 0 To nOtu - 1
            Dim otu_id As String = otuMatrix(i).geneID

            For j As Integer = 0 To nMet - 1
                Dim metExpr As DataFrameRow = metaboliteMatrix(j)
                Dim r As Double = cor(i, j)

                CorrelationMatrix.SetValue(otu_id, metExpr.geneID, r)
                PValueMatrix.SetValue(otu_id, metExpr.geneID, CorrelationPValue(r, nS))
            Next
        Next

        Return New CrossOmicsCorrelation(CorrelationMatrix, PValueMatrix, OtuIds, MetaboliteIds) With {.methodName = name}
    End Function

    Friend Function ComputeCrossCorrelation(otuMatrix As Matrix, metaboliteMatrix As Matrix, compute As ICorrelation, name As String) As CrossOmicsCorrelation
        Dim nOtu As Integer = otuMatrix.size
        Dim nMet As Integer = metaboliteMatrix.size
        Dim nS As Integer = otuMatrix.sample_count

        Dim OtuIds(nOtu) As String
        Dim MetaboliteIds(nMet) As String

        For i As Integer = 0 To nOtu - 1
            OtuIds(i) = otuMatrix(i).geneID
        Next
        For j As Integer = 0 To nMet - 1
            MetaboliteIds(j) = metaboliteMatrix(j).geneID
        Next

        Dim CorrelationMatrix As New NamedSparseMatrix()
        Dim PValueMatrix As New NamedSparseMatrix()

        For i As Integer = 0 To nOtu - 1
            Dim otuExpr As Double() = otuMatrix(i).experiments
            Dim otu_id As String = otuMatrix(i).geneID

            For j As Integer = 0 To nMet - 1
                Dim metExpr As DataFrameRow = metaboliteMatrix(j)
                Dim r As Double = compute(otuExpr, metExpr.experiments)

                CorrelationMatrix.SetValue(otu_id, metExpr.geneID, r)
                PValueMatrix.SetValue(otu_id, metExpr.geneID, CorrelationPValue(r, nS))
            Next
        Next

        Return New CrossOmicsCorrelation(CorrelationMatrix, PValueMatrix, OtuIds, MetaboliteIds) With {.methodName = name}
    End Function
End Module

' ============================================================================
' 第三部分：Pearson 相关性计算
' ============================================================================

''' <summary>
''' Pearson 皮尔逊积矩相关系数
''' 
''' 公式: r = Σ((xi - x̄)(yi - ȳ)) / √(Σ(xi - x̄)² × Σ(yi - ȳ)²)
''' 
''' 适用场景: 两个连续变量之间的线性相关程度
''' 取值范围: [-1, 1]，绝对值越大表示线性相关性越强
''' </summary>
Public Module PearsonCorrelation

    ''' <summary>
    ''' 计算两个向量之间的 Pearson 相关系数
    ''' </summary>
    Public Function Compute(x As Double(), y As Double()) As Double
        If x Is Nothing OrElse y Is Nothing Then Return 0.0
        If x.Length <> y.Length OrElse x.Length < 2 Then Return 0.0

        Dim n As Integer = x.Length
        Dim mx As Double = MathHelpers.Mean(x)
        Dim my As Double = MathHelpers.Mean(y)

        Dim sumXY As Double = 0.0
        Dim sumXX As Double = 0.0
        Dim sumYY As Double = 0.0

        For i As Integer = 0 To n - 1
            Dim dx As Double = x(i) - mx
            Dim dy As Double = y(i) - my
            sumXY += dx * dy
            sumXX += dx * dx
            sumYY += dy * dy
        Next

        If sumXX <= 0.0 OrElse sumYY <= 0.0 Then Return 0.0
        Return sumXY / Sqrt(sumXX * sumYY)
    End Function

    ''' <summary>
    ''' 计算单个矩阵内所有 feature 间的 Pearson 相关矩阵 [nFeature, nFeature]
    ''' </summary>
    Public Function ComputeInternalMatrix(matrix As Matrix) As Double(,)
        Dim data As Double(,) = MathHelpers.MatrixTo2DArray(matrix)
        Dim nF As Integer = matrix.size
        Dim nS As Integer = matrix.sample_count
        Dim corr As Double(,) = New Double(nF - 1, nF - 1) {}

        For i As Integer = 0 To nF - 1
            corr(i, i) = 1.0
            Dim xi As Double() = MathHelpers.GetFeatureColumn(data, i, nS)
            For j As Integer = i + 1 To nF - 1
                Dim xj As Double() = MathHelpers.GetFeatureColumn(data, j, nS)
                Dim r As Double = Compute(xi, xj)
                corr(i, j) = r
                corr(j, i) = r
            Next
        Next

        Return corr
    End Function

    ''' <summary>
    ''' 计算 OTU 与代谢物之间的 Pearson 交叉相关矩阵
    ''' 结果矩阵维度: [nOtu, nMet]
    ''' </summary>
    Public Function ComputeCrossCorrelation(otuMatrix As Matrix, metaboliteMatrix As Matrix) As CrossOmicsCorrelation
        Return MathHelpers.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix, AddressOf Compute, name:="Pearson")
    End Function

End Module

' ============================================================================
' 第四部分：Spearman 相关性计算
' ============================================================================

''' <summary>
''' Spearman 秩相关系数
''' 
''' 原理: 将原始数据转换为秩（Rank），然后计算秩的 Pearson 相关系数
''' 适用场景: 单调（但不一定线性）相关；对异常值更鲁棒
''' 取值范围: [-1, 1]
''' 
''' 结值处理: 使用平均秩（average rank）方法
''' </summary>
Public Module SpearmanCorrelation

    ''' <summary>
    ''' 计算两个向量之间的 Spearman 秩相关系数
    ''' </summary>
    Public Function Compute(x As Double(), y As Double()) As Double
        If x Is Nothing OrElse y Is Nothing Then Return 0.0
        If x.Length <> y.Length OrElse x.Length < 2 Then Return 0.0

        ' 转换为秩
        Dim rankX As Double() = MathHelpers.Rank(x)
        Dim rankY As Double() = MathHelpers.Rank(y)

        ' 计算秩的 Pearson 相关系数
        Return PearsonCorrelation.Compute(rankX, rankY)
    End Function

    ''' <summary>
    ''' 计算单个矩阵内所有 feature 间的 Spearman 相关矩阵
    ''' </summary>
    Public Function ComputeInternalMatrix(matrix As Matrix) As Double(,)
        Dim data As Double(,) = MathHelpers.MatrixTo2DArray(matrix)
        Dim nF As Integer = matrix.size
        Dim nS As Integer = matrix.sample_count
        Dim corr As Double(,) = New Double(nF - 1, nF - 1) {}

        For i As Integer = 0 To nF - 1
            corr(i, i) = 1.0
            Dim xi As Double() = MathHelpers.GetFeatureColumn(data, i, nS)
            For j As Integer = i + 1 To nF - 1
                Dim xj As Double() = MathHelpers.GetFeatureColumn(data, j, nS)
                Dim r As Double = Compute(xi, xj)
                corr(i, j) = r
                corr(j, i) = r
            Next
        Next

        Return corr
    End Function

    ''' <summary>
    ''' 计算 OTU 与代谢物之间的 Spearman 交叉相关矩阵
    ''' </summary>
    Public Function ComputeCrossCorrelation(otuMatrix As Matrix, metaboliteMatrix As Matrix) As CrossOmicsCorrelation
        Return MathHelpers.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix, AddressOf Compute, name:="Spearman")
    End Function

End Module

' ============================================================================
' 第五部分：SparCC 相关性计算
' ============================================================================

''' <summary>
''' SparCC (Sparse Correlations for Compositional Data)
''' 
''' 参考文献: Friedman, J. & Alm, E.J. (2012). 
'''   Inferring Correlation Networks from Genomic Survey Data. 
'''   PLoS Computational Biology, 8(9), e1002687.
''' 
''' 核心思想:
'''   组成型数据（如 OTU 丰度表）由于总和约束（各样本 feature 之和为常数），
'''   导致 feature 之间存在伪相关。SparCC 通过以下策略消除组成型效应：
'''   1. 利用变异矩阵（variation matrix）T_ij = Var(log(x_i/x_j)) 估计基础方差
'''   2. 通过关系式 ρ_ij = (σ²_i + σ²_j - T_ij) / (2σ_iσ_j) 估计基础相关系数
'''   3. 迭代排除强相关配对后重新估计，以减少伪相关的影响
''' 
''' 假设条件:
'''   - 真实的基础相关性矩阵是稀疏的（大部分 feature 对之间无相关）
'''   - 各 feature 的基础方差为正
''' </summary>
Public Module SparCCComputation

    ''' <summary>
    ''' SparCC 算法参数配置
    ''' </summary>
    Public Class SparCCConfig
        ''' <summary>迭代精炼次数（默认 10）</summary>
        Public Iterations As Integer = 10
        ''' <summary>相关性阈值：迭代中超过此绝对值的配对将被排除（默认 0.1）</summary>
        Public CorrelationThreshold As Double = 0.1
        ''' <summary>零值替换伪计数（默认 1.0，避免 log(0)）</summary>
        Public PseudoCount As Double = 1.0
        ''' <summary>是否启用迭代排除机制（默认 True）</summary>
        Public UseExclusion As Boolean = True
    End Class

    ''' <summary>
    ''' 对组成型数据进行 CLR（Centered Log-Ratio）变换
    ''' CLR_i = log(x_i) - mean(log(x_j))
    ''' 
    ''' CLR 变换是组成型数据分析的标准预处理步骤，它将数据从单纯形空间
    ''' 映射到欧几里得空间，消除总和约束的影响。
    ''' </summary>
    Private Function CLRTransform(
        data As Double(,),
        nFeatures As Integer,
        nSamples As Integer,
        pseudoCount As Double) As Double(,)

        Dim clr As Double(,) = New Double(nFeatures - 1, nSamples - 1) {}

        For s As Integer = 0 To nSamples - 1
            ' 计算该样本所有 feature 的对数均值
            Dim logSum As Double = 0.0
            For f As Integer = 0 To nFeatures - 1
                Dim val As Double = data(f, s) + pseudoCount
                If val > 0 Then
                    logSum += Log(val)
                End If
            Next
            Dim logMean As Double = logSum / nFeatures

            ' CLR 变换: clr_i = log(x_i) - mean(log(x))
            For f As Integer = 0 To nFeatures - 1
                Dim val As Double = data(f, s) + pseudoCount
                clr(f, s) = If(val > 0, Log(val) - logMean, 0.0)
            Next
        Next

        Return clr
    End Function

    ''' <summary>
    ''' 计算变异矩阵（Variation Matrix）
    ''' T_ij = Var(log(x_i / x_j)) = Var(CLR_i - CLR_j)
    ''' 
    ''' 变异矩阵是组成型数据分析的核心工具：
    ''' - 对角线 T_ii = 0（同一 feature 与自身的对数比方差为零）
    ''' - T_ij = σ²_i + σ²_j - 2ρ_ij * σ_i * σ_j
    '''   其中 σ²_i 是基础方差，ρ_ij 是基础相关系数
    ''' </summary>
    Private Function ComputeVariationMatrix(
        data As Double(,),
        nFeatures As Integer,
        nSamples As Integer,
        pseudoCount As Double) As Double(,)

        Dim varMatrix As Double(,) = New Double(nFeatures - 1, nFeatures - 1) {}
        Dim clr As Double(,) = CLRTransform(data, nFeatures, nSamples, pseudoCount)

        For i As Integer = 0 To nFeatures - 1
            varMatrix(i, i) = 0.0
            For j As Integer = i + 1 To nFeatures - 1
                ' T_ij = Var(CLR_i - CLR_j)
                Dim diff As Double() = New Double(nSamples - 1) {}
                For s As Integer = 0 To nSamples - 1
                    diff(s) = clr(i, s) - clr(j, s)
                Next
                Dim v As Double = MathHelpers.Variance(diff)
                varMatrix(i, j) = v
                varMatrix(j, i) = v
            Next
        Next

        Return varMatrix
    End Function

    ''' <summary>
    ''' 计算 CLR 变换后的 Pearson 相关矩阵
    ''' 用于在 SparCC 迭代中寻找"最不相关"的参考 feature
    ''' </summary>
    Private Function ComputeCLRCorrelation(
        data As Double(,),
        nFeatures As Integer,
        nSamples As Integer,
        pseudoCount As Double) As Double(,)

        Dim clr As Double(,) = CLRTransform(data, nFeatures, nSamples, pseudoCount)
        Dim corr As Double(,) = New Double(nFeatures - 1, nFeatures - 1) {}

        For i As Integer = 0 To nFeatures - 1
            corr(i, i) = 1.0
            Dim xi As Double() = New Double(nSamples - 1) {}
            For s As Integer = 0 To nSamples - 1
                xi(s) = clr(i, s)
            Next

            For j As Integer = i + 1 To nFeatures - 1
                Dim xj As Double() = New Double(nSamples - 1) {}
                For s As Integer = 0 To nSamples - 1
                    xj(s) = clr(j, s)
                Next

                Dim r As Double = PearsonCorrelation.Compute(xi, xj)
                r = MathHelpers.Clamp(r, -1.0, 1.0)
                corr(i, j) = r
                corr(j, i) = r
            Next
        Next

        Return corr
    End Function

    ''' <summary>
    ''' SparCC 核心算法：估计组成型数据的基础相关矩阵
    ''' 
    ''' 算法流程:
    '''   1. 计算变异矩阵 T 和 CLR 相关矩阵
    '''   2. 初始基础方差估计：对每个 feature i，选择 CLR 相关性最弱的 feature k
    '''      作为参考，令 σ²_i = T_ik（假设 ρ_ik ≈ 0 时此估计近似无偏）
    '''   3. 初始基础相关估计：ρ_ij = (σ²_i + σ²_j - T_ij) / (2σ_iσ_j)
    '''   4. 迭代精炼：
    '''      a. 排除 |ρ_ij| > threshold 的配对
    '''      b. 在剩余配对中重新选择参考 feature 估计方差
    '''      c. 重新计算相关系数
    '''   5. 将相关系数限制在 [-1, 1] 范围内
    ''' </summary>
    Public Function ComputeSparCCCorrelation(
        data As Double(,),
        nFeatures As Integer,
        nSamples As Integer,
        config As SparCCConfig) As Double(,)

        ' ---- Step 1: 计算变异矩阵 ----
        Dim varMatrix As Double(,) = ComputeVariationMatrix(
            data, nFeatures, nSamples, config.PseudoCount)

        ' ---- Step 2: 计算 CLR 相关矩阵（用于选择参考 feature）----
        Dim clrCorr As Double(,) = ComputeCLRCorrelation(
            data, nFeatures, nSamples, config.PseudoCount)

        ' ---- Step 3: 初始化基础方差估计 ----
        Dim basisVariance As Double() = New Double(nFeatures - 1) {}

        For i As Integer = 0 To nFeatures - 1
            ' 找到与 feature i 的 CLR 相关性绝对值最小的 feature 作为参考
            ' 理由：若 ρ_ik ≈ 0，则 T_ik ≈ σ²_i + σ²_k，以 T_ik 估计 σ²_i 偏差最小
            Dim minAbsCorr As Double = Double.MaxValue
            Dim refIdx As Integer = -1

            For j As Integer = 0 To nFeatures - 1
                If i <> j Then
                    Dim absCorr As Double = Abs(clrCorr(i, j))
                    If absCorr < minAbsCorr Then
                        minAbsCorr = absCorr
                        refIdx = j
                    End If
                End If
            Next

            If refIdx >= 0 Then
                basisVariance(i) = varMatrix(i, refIdx)
            Else
                ' 退化情况：使用自身 CLR 方差
                Dim clr_i As Double() = New Double(nSamples - 1) {}
                Dim clrData As Double(,) = CLRTransform(data, nFeatures, nSamples, config.PseudoCount)
                For s As Integer = 0 To nSamples - 1
                    clr_i(s) = clrData(i, s)
                Next
                basisVariance(i) = MathHelpers.Variance(clr_i)
            End If

            ' 确保方差为正
            If basisVariance(i) <= 0 Then basisVariance(i) = 1e-4
        Next

        ' ---- Step 4: 计算初始基础相关矩阵 ----
        Dim basisCorr As Double(,) = New Double(nFeatures - 1, nFeatures - 1) {}

        For i As Integer = 0 To nFeatures - 1
            basisCorr(i, i) = 1.0
            For j As Integer = i + 1 To nFeatures - 1
                ' ρ_ij = (σ²_i + σ²_j - T_ij) / (2 * σ_i * σ_j)
                Dim numerator As Double = basisVariance(i) + basisVariance(j) - varMatrix(i, j)
                Dim denominator As Double = 2.0 * Sqrt(basisVariance(i) * basisVariance(j))

                If denominator > 0 Then
                    basisCorr(i, j) = MathHelpers.Clamp(numerator / denominator, -1.0, 1.0)
                Else
                    basisCorr(i, j) = 0.0
                End If
                basisCorr(j, i) = basisCorr(i, j)
            Next
        Next

        ' ---- Step 5: 迭代精炼 ----
        If config.UseExclusion Then
            For iter As Integer = 0 To config.Iterations - 1

                ' 5a. 识别强相关配对并标记为排除
                Dim excluded As Boolean(,) = New Boolean(nFeatures - 1, nFeatures - 1) {}
                For i As Integer = 0 To nFeatures - 1
                    For j As Integer = i + 1 To nFeatures - 1
                        If Abs(basisCorr(i, j)) > config.CorrelationThreshold Then
                            excluded(i, j) = True
                            excluded(j, i) = True
                        End If
                    Next
                Next

                ' 5b. 重新估计基础方差（排除强相关配对后选择参考 feature）
                For i As Integer = 0 To nFeatures - 1
                    Dim minAbsCorr As Double = Double.MaxValue
                    Dim refIdx As Integer = -1

                    For j As Integer = 0 To nFeatures - 1
                        If i <> j AndAlso Not excluded(i, j) Then
                            Dim absCorr As Double = Abs(clrCorr(i, j))
                            If absCorr < minAbsCorr Then
                                minAbsCorr = absCorr
                                refIdx = j
                            End If
                        End If
                    Next

                    If refIdx >= 0 Then
                        basisVariance(i) = varMatrix(i, refIdx)
                    Else
                        ' 所有配对都被排除时，使用变异矩阵行均值
                        Dim sumVar As Double = 0.0
                        Dim cnt As Integer = 0
                        For j As Integer = 0 To nFeatures - 1
                            If i <> j Then
                                sumVar += varMatrix(i, j)
                                cnt += 1
                            End If
                        Next
                        basisVariance(i) = If(cnt > 0, sumVar / cnt, 1e-4)
                    End If

                    If basisVariance(i) <= 0 Then basisVariance(i) = 1e-4
                Next

                ' 5c. 重新计算基础相关矩阵
                For i As Integer = 0 To nFeatures - 1
                    For j As Integer = i + 1 To nFeatures - 1
                        Dim numerator As Double = basisVariance(i) + basisVariance(j) - varMatrix(i, j)
                        Dim denominator As Double = 2.0 * Sqrt(basisVariance(i) * basisVariance(j))

                        If denominator > 0 Then
                            basisCorr(i, j) = MathHelpers.Clamp(numerator / denominator, -1.0, 1.0)
                        Else
                            basisCorr(i, j) = 0.0
                        End If
                        basisCorr(j, i) = basisCorr(i, j)
                    Next
                Next

            Next ' 迭代
        End If

        Return basisCorr
    End Function

    ''' <summary>
    ''' 计算 OTU 与代谢物之间的 SparCC 交叉相关矩阵
    ''' 
    ''' 策略说明:
    '''   SparCC 原本设计用于单一组成型矩阵内的相关性估计。对于 OTU-代谢物
    '''   交叉相关性，我们采用"合并矩阵"策略：
    '''   1. 分别对 OTU 和代谢物数据进行比例化（归一化为总和为 1）
    '''   2. 将两个比例化后的矩阵纵向拼接为统一矩阵
    '''   3. 对合并矩阵运行 SparCC 算法
    '''   4. 从完整相关矩阵中提取 OTU × 代谢物 的交叉相关块
    ''' </summary>
    Public Function ComputeCrossCorrelation(
        otuMatrix As Matrix,
        metaboliteMatrix As Matrix,
        Optional config As SparCCConfig = Nothing) As CrossOmicsCorrelation

        If config Is Nothing Then config = New SparCCConfig()

        Dim nOtu As Integer = otuMatrix.size
        Dim nMet As Integer = metaboliteMatrix.size
        Dim nS As Integer = otuMatrix.sample_count
        Dim nTotal As Integer = nOtu + nMet

        ' ---- 合并数据：分别比例化后拼接 ----
        Dim combinedData As Double(,) = New Double(nTotal - 1, nS - 1) {}

        ' OTU 数据比例化（组成型数据归一化）
        For s As Integer = 0 To nS - 1
            Dim otuSum As Double = 0.0
            For f As Integer = 0 To nOtu - 1
                otuSum += otuMatrix(f).experiments(s)
            Next
            If otuSum > 0 Then
                For f As Integer = 0 To nOtu - 1
                    combinedData(f, s) = otuMatrix(f).experiments(s) / otuSum
                Next
            Else
                For f As Integer = 0 To nOtu - 1
                    combinedData(f, s) = 1.0 / nOtu
                Next
            End If
        Next

        ' 代谢物数据比例化（取绝对值后归一化，确保非负）
        For s As Integer = 0 To nS - 1
            Dim metSum As Double = 0.0
            For f As Integer = 0 To nMet - 1
                metSum += Abs(metaboliteMatrix(f).experiments(s))
            Next
            If metSum > 0 Then
                For f As Integer = 0 To nMet - 1
                    combinedData(nOtu + f, s) = Abs(metaboliteMatrix(f).experiments(s)) / metSum
                Next
            Else
                For f As Integer = 0 To nMet - 1
                    combinedData(nOtu + f, s) = 1.0 / nMet
                Next
            End If
        Next

        ' ---- 对合并数据运行 SparCC ----
        Dim fullCorr As Double(,) = ComputeSparCCCorrelation(combinedData, nTotal, nS, config)

        ' ---- 提取交叉相关块 [nOtu, nMet] ----
        Dim CorrelationMatrix = New Double(nOtu - 1, nMet - 1) {}


        For i As Integer = 0 To nOtu - 1
            For j As Integer = 0 To nMet - 1
                CorrelationMatrix(i, j) = fullCorr(i, nOtu + j)
            Next
        Next

        ' SparCC 的 p 值通常通过 bootstrap 重采样计算
        ' 此处使用 Fisher z 近似作为简化方案
        Return CreateCrossCorrelation(otuMatrix, metaboliteMatrix, CorrelationMatrix, "SparCC")
    End Function

    ''' <summary>
    ''' 对单个矩阵计算 SparCC 内部相关矩阵
    ''' </summary>
    Public Function ComputeInternalMatrix(
        matrix As Matrix,
        Optional config As SparCCConfig = Nothing) As Double(,)

        If config Is Nothing Then config = New SparCCConfig()
        Dim data As Double(,) = MathHelpers.MatrixTo2DArray(matrix)

        ' 比例化
        Dim nF As Integer = matrix.size
        Dim nS As Integer = matrix.sample_count
        For s As Integer = 0 To nS - 1
            Dim rowSum As Double = 0.0
            For f As Integer = 0 To nF - 1
                rowSum += data(f, s)
            Next
            If rowSum > 0 Then
                For f As Integer = 0 To nF - 1
                    data(f, s) = data(f, s) / rowSum
                Next
            End If
        Next

        Return ComputeSparCCCorrelation(data, nF, nS, config)
    End Function

End Module

' ============================================================================
' 第六部分：CCLasso 相关性计算
' ============================================================================

''' <summary>
''' CCLasso (Composition Corrected Lasso)
''' 
''' 参考文献: Fang, H., Huang, C., Zhao, H. & Deng, M. (2015).
'''   CCLasso: correlation inference for compositional data through Lasso.
'''   Bioinformatics, 31(19), 3171-3179.
''' 
''' 核心思想:
'''   与 SparCC 类似，CCLasso 也利用变异矩阵 V_ij = Var(log(x_i/x_j)) 与
'''   基础协方差矩阵 Σ 之间的关系：
'''     V_ij = Σ_ii + Σ_jj - 2Σ_ij
'''   但 CCLasso 通过 L1 惩罚（Lasso）回归来估计基础协方差矩阵，
'''   利用稀疏性假设自动将弱相关压缩为零。
''' 
''' 优化目标:
'''   min  Σ_{i&lt;j} (V_ij - Σ_ii - Σ_jj + 2Σ_ij)² + 2λ Σ_{i&lt;j} |Σ_ij|
''' 
''' 求解方法: 坐标下降法（Coordinate Descent）
'''   - 非对角线更新: Σ_ij = SoftThreshold((Σ_ii + Σ_jj - V_ij)/2, λ/2)
'''   - 对角线更新:   Σ_ii = Σ_{j≠i}(V_ij - Σ_jj + 2Σ_ij) / (p-1)
''' </summary>
Public Module CCLassoComputation

    ''' <summary>
    ''' CCLasso 算法参数配置
    ''' </summary>
    Public Class CCLassoConfig
        ''' <summary>L1 惩罚参数 λ（默认 0.05，值越大相关矩阵越稀疏）</summary>
        Public Lambda As Double = 0.05
        ''' <summary>坐标下降最大迭代次数（默认 200）</summary>
        Public MaxIterations As Integer = 200
        ''' <summary>收敛阈值（默认 1e-6）</summary>
        Public Tolerance As Double = 1e-6
        ''' <summary>零值替换伪计数（默认 1.0）</summary>
        Public PseudoCount As Double = 1.0
    End Class

    ''' <summary>
    ''' 计算变异矩阵（与 SparCC 共用同一计算逻辑）
    ''' V_ij = Var(log(x_i / x_j))
    ''' </summary>
    Private Function ComputeVariationMatrix(
        data As Double(,),
        nFeatures As Integer,
        nSamples As Integer,
        pseudoCount As Double) As Double(,)

        Dim varMatrix As Double(,) = New Double(nFeatures - 1, nFeatures - 1) {}

        ' CLR 变换
        Dim clr As Double(,) = New Double(nFeatures - 1, nSamples - 1) {}
        For s As Integer = 0 To nSamples - 1
            Dim logSum As Double = 0.0
            For f As Integer = 0 To nFeatures - 1
                Dim val As Double = data(f, s) + pseudoCount
                logSum += Log(val)
            Next
            Dim logMean As Double = logSum / nFeatures
            For f As Integer = 0 To nFeatures - 1
                Dim val As Double = data(f, s) + pseudoCount
                clr(f, s) = Log(val) - logMean
            Next
        Next

        For i As Integer = 0 To nFeatures - 1
            varMatrix(i, i) = 0.0
            For j As Integer = i + 1 To nFeatures - 1
                Dim diff As Double() = New Double(nSamples - 1) {}
                For s As Integer = 0 To nSamples - 1
                    diff(s) = clr(i, s) - clr(j, s)
                Next
                Dim v As Double = MathHelpers.Variance(diff)
                varMatrix(i, j) = v
                varMatrix(j, i) = v
            Next
        Next

        Return varMatrix
    End Function

    ''' <summary>
    ''' CCLasso 核心算法：通过 L1 惩罚坐标下降估计基础协方差矩阵
    ''' 
    ''' 详细推导:
    '''   目标函数（仅对 i &lt; j 求和，利用对称性）:
    '''     f(Σ) = Σ_{i&lt;j} (V_ij - Σ_ii - Σ_jj + 2Σ_ij)² + 2λ Σ_{i&lt;j} |Σ_ij|
    ''' 
    '''   对非对角线 Σ_ij (i ≠ j) 求偏导并令其为零:
    '''     ∂f/∂Σ_ij = 2(V_ij - Σ_ii - Σ_jj + 2Σ_ij) + 2λ·sign(Σ_ij) = 0
    '''     令 z = (Σ_ii + Σ_jj - V_ij) / 2
    '''     则 Σ_ij = SoftThreshold(z, λ/2)
    ''' 
    '''   对对角线 Σ_ii 求偏导并令其为零:
    '''     ∂f/∂Σ_ii = -2 Σ_{j≠i} (V_ij - Σ_ii - Σ_jj + 2Σ_ij) = 0
    '''     Σ_ii = Σ_{j≠i} (V_ij - Σ_jj + 2Σ_ij) / (p-1)
    ''' </summary>
    Public Function ComputeCCLassoCovariance(
        data As Double(,),
        nFeatures As Integer,
        nSamples As Integer,
        config As CCLassoConfig) As Double(,)

        ' ---- Step 1: 计算变异矩阵 ----
        Dim V As Double(,) = ComputeVariationMatrix(
            data, nFeatures, nSamples, config.PseudoCount)

        ' ---- Step 2: 初始化基础协方差矩阵 ----
        Dim Sigma As Double(,) = New Double(nFeatures - 1, nFeatures - 1) {}

        ' 初始化对角线（基础方差）
        ' 初始估计：σ²_i ≈ mean_j≠i(V_ij) / 2
        ' 理由：若各 feature 不相关，V_ij ≈ σ²_i + σ²_j，取均值后除以 2 近似 σ²_i
        For i As Integer = 0 To nFeatures - 1
            Dim sumV As Double = 0.0
            Dim cnt As Integer = 0
            For j As Integer = 0 To nFeatures - 1
                If i <> j Then
                    sumV += V(i, j)
                    cnt += 1
                End If
            Next
            Sigma(i, i) = If(cnt > 0, sumV / cnt / 2.0, 1.0)
            If Sigma(i, i) <= 0 Then Sigma(i, i) = 1e-4
        Next

        ' 初始化非对角线为零（Lasso 的稀疏初始解）
        For i As Integer = 0 To nFeatures - 1
            For j As Integer = i + 1 To nFeatures - 1
                Sigma(i, j) = 0.0
                Sigma(j, i) = 0.0
            Next
        Next

        ' ---- Step 3: 坐标下降迭代 ----
        Dim lambda As Double = config.Lambda
        Dim converged As Boolean = False
        Dim iteration As Integer = 0

        While Not converged AndAlso iteration < config.MaxIterations
            Dim maxChange As Double = 0.0

            ' 3a. 更新非对角线元素 Σ_ij (i < j)
            '     Σ_ij = SoftThreshold((Σ_ii + Σ_jj - V_ij) / 2, λ/2)
            For i As Integer = 0 To nFeatures - 1
                For j As Integer = i + 1 To nFeatures - 1
                    Dim z As Double = (Sigma(i, i) + Sigma(j, j) - V(i, j)) / 2.0
                    Dim newVal As Double = MathHelpers.SoftThreshold(z, lambda / 2.0)

                    Dim change As Double = Abs(newVal - Sigma(i, j))
                    If change > maxChange Then maxChange = change

                    Sigma(i, j) = newVal
                    Sigma(j, i) = newVal
                Next
            Next

            ' 3b. 更新对角线元素 Σ_ii
            '     Σ_ii = Σ_{j≠i} (V_ij - Σ_jj + 2Σ_ij) / (p-1)
            For i As Integer = 0 To nFeatures - 1
                Dim sumVal As Double = 0.0
                For j As Integer = 0 To nFeatures - 1
                    If i <> j Then
                        sumVal += V(i, j) - Sigma(j, j) + 2.0 * Sigma(i, j)
                    End If
                Next
                Dim newDiag As Double = sumVal / (nFeatures - 1)

                ' 确保方差为正
                If newDiag <= 0 Then newDiag = 1e-4

                Dim change As Double = Abs(newDiag - Sigma(i, i))
                If change > maxChange Then maxChange = change

                Sigma(i, i) = newDiag
            Next

            ' 3c. 检查收敛
            converged = (maxChange < config.Tolerance)
            iteration += 1
        End While

        Return Sigma
    End Function

    ''' <summary>
    ''' 从基础协方差矩阵转换为相关矩阵
    ''' ρ_ij = Σ_ij / √(Σ_ii × Σ_jj)
    ''' </summary>
    Private Function CovarianceToCorrelation(
        cov As Double(,),
        nFeatures As Integer) As Double(,)

        Dim corr As Double(,) = New Double(nFeatures - 1, nFeatures - 1) {}

        For i As Integer = 0 To nFeatures - 1
            corr(i, i) = 1.0
            For j As Integer = i + 1 To nFeatures - 1
                Dim denom As Double = Sqrt(cov(i, i) * cov(j, j))
                If denom > 0 Then
                    corr(i, j) = MathHelpers.Clamp(cov(i, j) / denom, -1.0, 1.0)
                Else
                    corr(i, j) = 0.0
                End If
                corr(j, i) = corr(i, j)
            Next
        Next

        Return corr
    End Function

    ''' <summary>
    ''' 计算 OTU 与代谢物之间的 CCLasso 交叉相关矩阵
    ''' 
    ''' 策略与 SparCC 相同：合并矩阵 → 运行算法 → 提取交叉块
    ''' </summary>
    Public Function ComputeCrossCorrelation(
        otuMatrix As Matrix,
        metaboliteMatrix As Matrix,
        Optional config As CCLassoConfig = Nothing) As CrossOmicsCorrelation

        If config Is Nothing Then config = New CCLassoConfig()

        Dim nOtu As Integer = otuMatrix.size
        Dim nMet As Integer = metaboliteMatrix.size
        Dim nS As Integer = otuMatrix.sample_count
        Dim nTotal As Integer = nOtu + nMet

        ' ---- 合并数据 ----
        Dim combinedData As Double(,) = New Double(nTotal - 1, nS - 1) {}

        ' OTU 比例化
        For s As Integer = 0 To nS - 1
            Dim otuSum As Double = 0.0
            For f As Integer = 0 To nOtu - 1
                otuSum += otuMatrix(f).experiments(s)
            Next
            If otuSum > 0 Then
                For f As Integer = 0 To nOtu - 1
                    combinedData(f, s) = otuMatrix(f).experiments(s) / otuSum
                Next
            Else
                For f As Integer = 0 To nOtu - 1
                    combinedData(f, s) = 1.0 / nOtu
                Next
            End If
        Next

        ' 代谢物比例化
        For s As Integer = 0 To nS - 1
            Dim metSum As Double = 0.0
            For f As Integer = 0 To nMet - 1
                metSum += Abs(metaboliteMatrix(f).experiments(s))
            Next
            If metSum > 0 Then
                For f As Integer = 0 To nMet - 1
                    combinedData(nOtu + f, s) = Abs(metaboliteMatrix(f).experiments(s)) / metSum
                Next
            Else
                For f As Integer = 0 To nMet - 1
                    combinedData(nOtu + f, s) = 1.0 / nMet
                Next
            End If
        Next

        ' ---- 运行 CCLasso ----
        Dim cov As Double(,) = ComputeCCLassoCovariance(combinedData, nTotal, nS, config)
        Dim fullCorr As Double(,) = CovarianceToCorrelation(cov, nTotal)

        ' ---- 提取交叉相关块 ----
        Dim CorrelationMatrix = New Double(nOtu - 1, nMet - 1) {}

        For i As Integer = 0 To nOtu - 1
            For j As Integer = 0 To nMet - 1
                CorrelationMatrix(i, j) = fullCorr(i, nOtu + j)
            Next
        Next

        Return CreateCrossCorrelation(otuMatrix, metaboliteMatrix, CorrelationMatrix, "CCLasso")
    End Function

    ''' <summary>
    ''' 对单个矩阵计算 CCLasso 内部相关矩阵
    ''' </summary>
    Public Function ComputeInternalMatrix(
        matrix As Matrix,
        Optional config As CCLassoConfig = Nothing) As Double(,)

        If config Is Nothing Then config = New CCLassoConfig()
        Dim data As Double(,) = MathHelpers.MatrixTo2DArray(matrix)

        ' 比例化
        Dim nF As Integer = matrix.size
        Dim nS As Integer = matrix.sample_count
        For s As Integer = 0 To nS - 1
            Dim rowSum As Double = 0.0
            For f As Integer = 0 To nF - 1
                rowSum += data(f, s)
            Next
            If rowSum > 0 Then
                For f As Integer = 0 To nF - 1
                    data(f, s) = data(f, s) / rowSum
                Next
            End If
        Next

        Dim cov As Double(,) = ComputeCCLassoCovariance(data, nF, nS, config)
        Return CovarianceToCorrelation(cov, nF)
    End Function

End Module

' ============================================================================
' 第七部分：统一调用接口
' ============================================================================

''' <summary>
''' 交叉相关性统一计算入口
''' 提供对四种相关性方法的统一调用接口
''' </summary>
Public Module CrossCorrelationCalculator

    ''' <summary>
    ''' 相关性计算方法枚举
    ''' </summary>
    Public Enum CorrelationMethod
        Pearson = 0
        Spearman = 1
        SparCC = 2
        CCLasso = 3
    End Enum

    ''' <summary>
    ''' 使用指定方法计算 OTU 与代谢物之间的交叉相关矩阵
    ''' </summary>
    ''' <param name="otuMatrix">OTU 丰度表达矩阵</param>
    ''' <param name="metaboliteMatrix">代谢物表达矩阵</param>
    ''' <param name="method">相关性计算方法</param>
    ''' <returns>交叉相关性计算结果</returns>
    Public Function Compute(
        otuMatrix As Matrix,
        metaboliteMatrix As Matrix,
        method As CorrelationMethod) As CrossOmicsCorrelation

        Select Case method
            Case CorrelationMethod.Pearson : Return PearsonCorrelation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
            Case CorrelationMethod.Spearman : Return SpearmanCorrelation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
            Case CorrelationMethod.SparCC : Return SparCCComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
            Case CorrelationMethod.CCLasso : Return CCLassoComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
            Case Else
                Throw New ArgumentException("不支持的相关性计算方法: " & method.ToString())
        End Select
    End Function

    ''' <summary>
    ''' 使用全部 4 种方法计算 OTU 与代谢物之间的交叉相关矩阵
    ''' </summary>
    ''' <returns>包含 4 个 CorrelationResult 的数组，顺序为 Pearson, Spearman, SparCC, CCLasso</returns>
    Public Function ComputeAll(
        otuMatrix As Matrix,
        metaboliteMatrix As Matrix) As CrossOmicsCorrelation()

        Dim results As CrossOmicsCorrelation() = New CrossOmicsCorrelation(3) {}

        results(0) = PearsonCorrelation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
        results(1) = SpearmanCorrelation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
        results(2) = SparCCComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
        results(3) = CCLassoComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)

        Return results
    End Function

    ''' <summary>
    ''' 使用自定义参数的 SparCC 计算
    ''' </summary>
    Public Function ComputeSparCC(
        otuMatrix As Matrix,
        metaboliteMatrix As Matrix,
        iterations As Integer,
        correlationThreshold As Double,
        pseudoCount As Double) As CrossOmicsCorrelation

        Dim config As New SparCCComputation.SparCCConfig()
        config.Iterations = iterations
        config.CorrelationThreshold = correlationThreshold
        config.PseudoCount = pseudoCount
        config.UseExclusion = True

        Return SparCCComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix, config)
    End Function

    ''' <summary>
    ''' 使用自定义参数的 CCLasso 计算
    ''' </summary>
    Public Function ComputeCCLasso(
        otuMatrix As Matrix,
        metaboliteMatrix As Matrix,
        lambda As Double,
        maxIterations As Integer,
        tolerance As Double,
        pseudoCount As Double) As CrossOmicsCorrelation

        Dim config As New CCLassoComputation.CCLassoConfig()
        config.Lambda = lambda
        config.MaxIterations = maxIterations
        config.Tolerance = tolerance
        config.PseudoCount = pseudoCount

        Return CCLassoComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix, config)
    End Function

End Module
