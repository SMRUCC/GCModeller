' ============================================================================
' MathUtils.vb - 基础数学工具函数
'
' 论文中涉及的核心数学方法：
'   1. 线性代数运算（向量点积、矩阵乘法）
'   2. 概率论运算（联合概率、条件概率）
'   3. 统计学运算（皮尔逊相关系数、均值、方差）
'   4. 信息论运算（对数似然）
'   5. 特殊函数（Gamma函数、Logistic函数）
'
' 全部基于VB.NET基础数学函数实现，不依赖外部数学库
' ============================================================================

Imports System.Runtime.InteropServices

Namespace metaTraits.Traitar.Utils

    ''' <summary>
    ''' 数学工具类
    ''' 实现论文中涉及的所有基础数学运算
    ''' </summary>
    Public Module MathUtils

        ' ================================================================
        ' 1. 线性代数运算
        ' ================================================================

        ''' <summary>
        ''' 向量点积：a · b = Σ(a_i × b_i)
        ''' 用于SVM预测：score = w · x + b
        ''' </summary>
        Public Function DotProduct(a As Double(), b As Double()) As Double
            If a Is Nothing OrElse b Is Nothing Then Return 0.0
            Dim n As Integer = Math.Min(a.Length, b.Length)
            Dim sum As Double = 0.0
            For i As Integer = 0 To n - 1
                sum += a(i) * b(i)
            Next
            Return sum
        End Function

        ''' <summary>
        ''' 向量L1范数：||v||_1 = Σ|v_i|
        ''' 用于L1正则化项的计算
        ''' </summary>
        Public Function L1Norm(v As Double()) As Double
            If v Is Nothing Then Return 0.0
            Dim sum As Double = 0.0
            For i As Integer = 0 To v.Length - 1
                sum += Math.Abs(v(i))
            Next
            Return sum
        End Function

        ''' <summary>
        ''' 向量L2范数：||v||_2 = √(Σ v_i²)
        ''' </summary>
        Public Function L2Norm(v As Double()) As Double
            If v Is Nothing Then Return 0.0
            Dim sumSq As Double = 0.0
            For i As Integer = 0 To v.Length - 1
                sumSq += v(i) * v(i)
            Next
            Return Math.Sqrt(sumSq)
        End Function

        ''' <summary>
        ''' 向量L2范数的平方：||v||_2² = Σ v_i²
        ''' 用于L2损失函数
        ''' </summary>
        Public Function L2NormSquared(v As Double()) As Double
            If v Is Nothing Then Return 0.0
            Dim sumSq As Double = 0.0
            For i As Integer = 0 To v.Length - 1
                sumSq += v(i) * v(i)
            Next
            Return sumSq
        End Function

        ' ================================================================
        ' 2. 概率论运算（模块3：数据融合）
        ' ================================================================

        ''' <summary>
        ''' 计算分支上发生Gain或Loss的联合概率
        ''' 论文公式：x = g + l - g × l
        ''' 其中 g = Gain概率，l = Loss概率
        ''' 这是两个独立事件至少发生一个的概率（并集概率）
        ''' </summary>
        Public Function GainOrLossProbability(gainProb As Double,
                                              lossProb As Double) As Double
            ' P(A∪B) = P(A) + P(B) - P(A)×P(B)
            Return gainProb + lossProb - gainProb * lossProb
        End Function

        ''' <summary>
        ''' 计算两个事件同时发生的联合概率（假设独立）
        ''' P(A∩B) = P(A) × P(B)
        ''' 用于phypat+PGL中蛋白质家族事件与表型事件的联合概率
        ''' </summary>
        Public Function JointProbability(probA As Double, probB As Double) As Double
            Return probA * probB
        End Function

        ''' <summary>
        ''' 阈值过滤：将概率转为离散标签
        ''' 论文：设定阈值t=0.5，只保留高置信度的进化事件
        ''' 低于该概率的不确定样本会被丢弃
        ''' </summary>
        Public Function ThresholdFilter(probability As Double,
                                        threshold As Double,
                                        <Out()> ByRef isConfident As Boolean) As Integer
            If probability >= threshold Then
                isConfident = True
                Return 1
            ElseIf probability <= 1.0 - threshold Then
                isConfident = True
                Return 0
            Else
                ' 不确定样本，丢弃
                isConfident = False
                Return -1
            End If
        End Function

        ' ================================================================
        ' 3. 统计学运算（模块7：特征选择与关联解释）
        ' ================================================================

        ''' <summary>
        ''' 计算均值
        ''' </summary>
        Public Function Mean(values As Double()) As Double
            If values Is Nothing OrElse values.Length = 0 Then Return 0.0
            Dim sum As Double = 0.0
            For Each v As Double In values
                sum += v
            Next
            Return sum / values.Length
        End Function

        ''' <summary>
        ''' 计算方差（总体方差）
        ''' </summary>
        Public Function Variance(values As Double()) As Double
            If values Is Nothing OrElse values.Length = 0 Then Return 0.0
            Dim m As Double = Mean(values)
            Dim sumSq As Double = 0.0
            For Each v As Double In values
                sumSq += (v - m) * (v - m)
            Next
            Return sumSq / values.Length
        End Function

        ''' <summary>
        ''' 计算标准差
        ''' </summary>
        Public Function StandardDeviation(values As Double()) As Double
            Return Math.Sqrt(Variance(values))
        End Function

        ''' <summary>
        ''' 计算皮尔逊相关系数（Pearson Correlation Coefficient, PCC）
        ''' 论文模块7：利用皮尔逊相关系数，对选出的蛋白质家族
        ''' 与表型的相关性进行排序
        '''
        ''' 公式：r = Σ((x_i - x̄)(y_i - ȳ)) / √(Σ(x_i - x̄)² × Σ(y_i - ȳ)²)
        ''' </summary>
        Public Function PearsonCorrelation(x As Double(), y As Double()) As Double
            If x Is Nothing OrElse y Is Nothing Then Return 0.0
            If x.Length <> y.Length OrElse x.Length = 0 Then Return 0.0

            Dim n As Integer = x.Length
            Dim meanX As Double = Mean(x)
            Dim meanY As Double = Mean(y)

            Dim sumXY As Double = 0.0
            Dim sumXX As Double = 0.0
            Dim sumYY As Double = 0.0

            For i As Integer = 0 To n - 1
                Dim dx As Double = x(i) - meanX
                Dim dy As Double = y(i) - meanY
                sumXY += dx * dy
                sumXX += dx * dx
                sumYY += dy * dy
            Next

            Dim denom As Double = Math.Sqrt(sumXX * sumYY)
            If denom = 0.0 Then Return 0.0
            Return sumXY / denom
        End Function

        ''' <summary>
        ''' 计算二值变量的皮尔逊相关系数（简化版）
        ''' x和y均为0/1二值向量
        ''' </summary>
        Public Function BinaryPearsonCorrelation(x As Integer(), y As Integer()) As Double
            If x Is Nothing OrElse y Is Nothing Then Return 0.0
            If x.Length <> y.Length OrElse x.Length = 0 Then Return 0.0

            Dim n As Integer = x.Length
            Dim xD As Double() = New Double(n - 1) {}
            Dim yD As Double() = New Double(n - 1) {}
            For i As Integer = 0 To n - 1
                xD(i) = CDbl(x(i))
                yD(i) = CDbl(y(i))
            Next
            Return PearsonCorrelation(xD, yD)
        End Function

        ' ================================================================
        ' 4. 信息论与对数运算（模块2：祖先状态重建）
        ' ================================================================

        ''' <summary>
        ''' 自然对数（安全版本，避免log(0)）
        ''' </summary>
        Public Function SafeLog(x As Double) As Double
            If x <= 0.0 Then Return -700.0  ' 接近负无穷
            Return Math.Log(x)
        End Function

        ''' <summary>
        ''' 以2为底的对数
        ''' </summary>
        Public Function Log2(x As Double) As Double
            If x <= 0.0 Then Return -700.0
            Return Math.Log(x) / Math.Log(2.0)
        End Function

        ''' <summary>
        ''' 计算对数似然
        ''' 用于GLOOME最大似然法推断祖先状态
        ''' LL = Σ(y_i × log(p_i) + (1-y_i) × log(1-p_i))
        ''' </summary>
        Public Function LogLikelihood(observed As Integer(),
                                      predictedProb As Double()) As Double
            If observed Is Nothing OrElse predictedProb Is Nothing Then Return 0.0
            If observed.Length <> predictedProb.Length Then Return 0.0

            Dim ll As Double = 0.0
            For i As Integer = 0 To observed.Length - 1
                Dim p As Double = Math.Max(0.0000000001, Math.Min(1.0 - 0.0000000001, predictedProb(i)))
                If observed(i) = 1 Then
                    ll += SafeLog(p)
                Else
                    ll += SafeLog(1.0 - p)
                End If
            Next
            Return ll
        End Function

        ' ================================================================
        ' 5. 特殊函数
        ' ================================================================

        ''' <summary>
        ''' Logistic/Sigmoid函数：σ(x) = 1 / (1 + e^(-x))
        ''' 用于将得分转为概率
        ''' </summary>
        Public Function Sigmoid(x As Double) As Double
            If x < -700 Then Return 0.0
            If x > 700 Then Return 1.0
            Return 1.0 / (1.0 + Math.Exp(-x))
        End Function

        ''' <summary>
        ''' Gamma函数（Lanczos近似）
        ''' 用于GLOOME中Gamma分布采样，模拟获得/丢失速率的方差
        ''' </summary>
        Public Function Gamma(x As Double) As Double
            ' Lanczos近似
            Dim g As Double = 7.0
            Dim p As Double() = {
                0.99999999999980993,
                676.5203681218851,
                -1259.1392167224028,
                771.32342877765313,
                -176.61502916214059,
                12.507343278686905,
                -0.13857109526572012,
                9.9843695780195716E-6,
                1.5056327351493116E-7
            }

            If x < 0.5 Then
                ' 反射公式：Γ(x)Γ(1-x) = π/sin(πx)
                Return Math.PI / (Math.Sin(Math.PI * x) * Gamma(1.0 - x))
            End If

            x -= 1.0
            Dim a As Double = p(0)
            Dim t As Double = x + g + 0.5
            For i As Integer = 1 To p.Length - 1
                a += p(i) / (x + i)
            Next

            Return Math.Sqrt(2.0 * Math.PI) * Math.Pow(t, x + 0.5) * Math.Exp(-t) * a
        End Function

        ''' <summary>
        ''' Gamma函数的对数（避免数值溢出）
        ''' </summary>
        Public Function LogGamma(x As Double) As Double
            Return SafeLog(Gamma(x))
        End Function

        ''' <summary>
        ''' Logistic函数的反函数（Logit）
        ''' </summary>
        Public Function Logit(p As Double) As Double
            If p <= 0.0 Then Return -700.0
            If p >= 1.0 Then Return 700.0
            Return Math.Log(p / (1.0 - p))
        End Function

        ' ================================================================
        ' 6. 随机数生成（用于交叉验证划分）
        ' ================================================================

        Private _rng As New Random(42)  ' 固定种子保证可复现

        ''' <summary>
        ''' 设置随机种子
        ''' </summary>
        Public Sub SetSeed(seed As Integer)
            _rng = New Random(seed)
        End Sub

        ''' <summary>
        ''' 生成[0,1)之间的随机数
        ''' </summary>
        Public Function RandDouble() As Double
            Return _rng.NextDouble()
        End Function

        ''' <summary>
        ''' 生成[min,max]之间的随机整数
        ''' </summary>
        Public Function RandInt(min As Integer, max As Integer) As Integer
            Return _rng.Next(min, max + 1)
        End Function

        ''' <summary>
        ''' Fisher-Yates洗牌算法
        ''' 用于交叉验证中随机打乱样本顺序
        ''' </summary>
        Public Sub Shuffle(Of T)(arr As T())
            If arr Is Nothing Then Return
            Dim n As Integer = arr.Length
            For i As Integer = n - 1 To 1 Step -1
                Dim j As Integer = _rng.Next(i + 1)
                Dim temp As T = arr(i)
                arr(i) = arr(j)
                arr(j) = temp
            Next
        End Sub

        ' ================================================================
        ' 7. 矩阵运算辅助
        ' ================================================================

        ''' <summary>
        ''' 矩阵转置
        ''' </summary>
        Public Function Transpose(m As Double(,)) As Double(,)
            If m Is Nothing Then Return Nothing
            Dim rows As Integer = m.GetLength(0)
            Dim cols As Integer = m.GetLength(1)
            Dim result As Double(,) = New Double(cols - 1, rows - 1) {}
            For i As Integer = 0 To rows - 1
                For j As Integer = 0 To cols - 1
                    result(j, i) = m(i, j)
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' Hinge Loss（L2损失版本）
        ''' 论文：L2损失函数
        ''' L(y, f(x)) = max(0, 1 - y·f(x))²
        ''' </summary>
        Public Function L2HingeLoss(y As Integer, score As Double) As Double
            Dim margin As Double = y * score
            Dim loss As Double = 1.0 - margin
            If loss < 0 Then loss = 0
            Return loss * loss
        End Function

        ''' <summary>
        ''' Hinge Loss（L1损失版本，用于对比）
        ''' L(y, f(x)) = max(0, 1 - y·f(x))
        ''' </summary>
        Public Function L1HingeLoss(y As Integer, score As Double) As Double
            Dim loss As Double = 1.0 - y * score
            If loss < 0 Then loss = 0
            Return loss
        End Function

    End Module

End Namespace
