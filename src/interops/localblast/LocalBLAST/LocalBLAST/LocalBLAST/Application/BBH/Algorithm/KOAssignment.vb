Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Distributions

Namespace LocalBLAST.Application.BBH

    ''' <summary>
    ''' 用于存储一个查询基因的KO分配候选
    ''' </summary>
    Public Class KOAssignmentCandidate

        Public Property QueryName As String
        Public Property KO As String
        ''' <summary>
        ''' 最终计算出的KO分配得分 S_KO
        ''' </summary>
        Public Property AssignmentScore As Double
        ''' <summary>
        ''' 最高比特分数 S_h，来自与该KO中所有基因比对的最佳结果
        ''' </summary>
        Public Property Sh As Double
        ''' <summary>
        ''' 满足BHR阈值的基因数
        ''' </summary>
        ''' <returns></returns>
        Public Property N As Integer
        ''' <summary>
        ''' KO中的总基因数
        ''' </summary>
        ''' <returns></returns>
        Public Property X As Integer

    End Class

    ''' <summary>
    ''' KEGG KAAS Assignment Score (S_KO) 计算模块
    ''' </summary>
    Public Module KOAssignment

        ''' <summary>
        ''' 计算KEGG的KO分配得分 S_KO
        ''' </summary>
        ''' <param name="Sh">最高比特分数</param>
        ''' <param name="m">查询基因长度</param>
        ''' <param name="n">参考基因长度</param>
        ''' <param name="x">KO中的总基因数</param>
        ''' <param name="N">满足BHR阈值的基因数</param>
        ''' <param name="p">单个基因满足BHR阈值的经验概率 (p0)</param>
        ''' <returns>Assignment Score (S_KO)</returns>
        ''' <remarks>
        ''' 这个函数直接对应了 KEGG 帮助文档中的公式：
        ''' 
        ''' We define a score for each ortholog group in order to assign the best fitting K numbers to the query gene:
        ''' 
        ''' ```
        ''' S_KO = S_h - log_2(mn) - log_2( sum_{k=N}^{x} C_k * p^k * (1-p)^{x-k} )
        ''' ```
        ''' 
        ''' where Sh is the highest score among all ortholog candidates in the ortholog group, 
        ''' m and n are the sequence lengths of the query and the target of BLAST, respectively, 
        ''' N is the number of organisms in ortholog group, x is the number of organisms in the 
        ''' original ortholog group from which this group is derived, and p is the ratio of the 
        ''' size of the original ortholog group versus the size of the entire GENES database. 
        ''' The second term is for the normalization of the first term by sequence lengths, and 
        ''' the third term is a weighting factor to consider the number of ortholog candidates 
        ''' that are found in the original.
        ''' </remarks>
        Public Function CalculateAssignmentScore(Sh As Double,
                                                 m As Integer,
                                                 ni As Integer,
                                                 x As Integer,
                                                 N As Integer,
                                                 p As Double) As Double

            If Sh <= 0 OrElse
                m <= 0 OrElse
                ni <= 0 OrElse
                x <= 0 OrElse
                p <= 0 OrElse
                p >= 1 Then

                Return Double.NegativeInfinity ' 无效输入
            End If

            ' 第一项: S_h
            Dim term1 As Double = Sh

            ' 第二项: log2(mn)
            Dim term2 As Double = Log2(m * ni)

            ' 第三项: log2( P(k >= N) ) = log2( sum_{k=N}^{x} C_k * p^k * (1-p)^(x-k) )
            ' 为了数值稳定性，我们直接在对数空间计算
            ' 这一项是一个二项分布的右尾概率的对数，计算复杂且容易数值不稳定。
            ' 因此，我们将其委托给一个专门的函数来处理。
            Dim term3 As Double = Log2BinomialRightTailProbability(x, N, p)

            ' S_KO = S_h - log2(mn) - log2(...)
            Return term1 - term2 - term3
        End Function

        ''' <summary>
        ''' 计算二项分布右尾概率的对数 (以2为底)
        ''' 即计算 log2( sum_{k=N}^{x} C(x,k) * p^k * (1-p)^(x-k) )
        ''' </summary>
        ''' <param name="x">总试验次数 (对应KO中的总基因数)</param>
        ''' <param name="N">成功的最小次数 (对应满足BHR阈值的基因数)</param>
        ''' <param name="p">单次试验成功的概率 (对应经验概率 p0)</param>
        ''' <returns>右尾概率的以2为底的对数</returns>
        Private Function Log2BinomialRightTailProbability(x As Integer, N As Integer, p As Double) As Double
            If N > x Then Return Double.NegativeInfinity ' 概率为0
            If N <= 0 Then Return 0 ' 概率为1, log2(1)=0

            Dim logTerms As New List(Of Double)()
            Dim logP As Double = Log2(p)
            Dim Log1MinusP As Double = Log2(1 - p)

            ' 步骤1: 在对数空间中计算求和中的每一项
            For k As Integer = N To x
                ' log2(C_k * p^k * (1-p)^(x-k)) = log2(C_k) + k*log2(p) + (x-k)*log2(1-p)
                Dim logTerm As Double = Log2BinomialCoefficient(x, k) + k * logP + (x - k) * Log1MinusP
                logTerms.Add(logTerm)
            Next

            ' 使用 Log-Sum-Exp 技巧来稳定地计算 log(sum(exp(terms)))
            ' 这是为了避免直接对极小数求和时发生数值下溢。
            ' log2(sum(2^term_i)) = max_term + log2(sum(2^(term_i - max_term)))
            ' 找到最大的 log(term_i)
            Dim maxLogTerm As Double = logTerms.Max()
            ' 通过减去最大值来缩放所有项，使指数部分在0和1之间，防止下溢
            Dim sumOfExps As Double = logTerms.Sum(Function(t) 2 ^ (t - maxLogTerm))
            ' 恢复缩放，得到最终的对数和
            Return maxLogTerm + Log2(sumOfExps)
        End Function

        ''' <summary>
        ''' 计算二项式系数的对数 (以2为底) log2(C(n, k))，
        ''' 使用 LogGamma 函数来避免计算大数阶乘导致的溢出。
        ''' </summary>
        Private Function Log2BinomialCoefficient(n As Integer, k As Integer) As Double
            If k < 0 OrElse k > n Then Return Double.NegativeInfinity
            ' log2(n! / (k! * (n-k)!)) = (ln(n!) - ln(k!) - ln((n-k)!)) / ln(2)
            ' 使用 LogGamma 函数计算 ln(n!), 因为 ln(n!) = LogGamma(n+1)
            Dim logGammaResult As Double = MathGamma.lngamm(n + 1) - MathGamma.lngamm(k + 1) - MathGamma.lngamm(n - k + 1)
            ' 将自然对数转换为以2为底的对数
            Return logGammaResult / Math.Log(2)
        End Function

        ''' <summary>
        ''' 计算以2为底的对数
        ''' </summary>
        Private Function Log2(value As Double) As Double
            Return Math.Log(value, 2)
        End Function

        ''' <summary>
        ''' 为单个查询基因分配最佳KO。它会评估所有可能的KO，并选择得分最高的一个。
        ''' </summary>
        ''' <param name="allBHRHitsForQuery">该查询基因与所有参考基因的BHR计算结果列表</param>
        ''' <param name="koGeneCounts">一个字典，包含每个KO的总基因数</param>
        ''' <param name="bhrThreshold">BHR阈值</param>
        ''' <param name="empiricalProbability">经验概率 p0</param>
        ''' <returns>得分最高的KO分配候选，如果没有则返回Nothing</returns>
        Public Function AssignBestKO(allBHRHitsForQuery As IEnumerable(Of BestHit),
                                     koGeneCounts As Dictionary(Of String, Integer),
                                     bhrThreshold As Double,
                                     empiricalProbability As Double) As KOAssignmentCandidate
            ' 1. 将该查询基因的所有比对结果，按其所属的KO进行分组
            Dim hitsByKO = From hit In allBHRHitsForQuery
                           Where Not String.IsNullOrEmpty(hit.HitName) ' 确保hit有KO信息
                           Group hit By ko = hit.HitName Into Group

            ' 2. 为每个KO组计算其S_KO得分，生成一个候选列表
            Dim candidates As KOAssignmentCandidate() = (
                From hit_group
                In hitsByKO
                Let ko As String = hit_group.ko
                Let groupHits = hit_group.Group.ToArray
                Select groupHits.KOCandidates(ko, koGeneCounts, bhrThreshold, empiricalProbability)
            ).ToArray

            ' 3. 选择得分最高的KO
            If Not candidates.Any() Then
                Return Nothing
            End If

            Return candidates.OrderByDescending(Function(c) c.AssignmentScore).First()
        End Function

        <Extension>
        Private Function KOCandidates(groupHits As BestHit(), ko As String,
                                      koGeneCounts As Dictionary(Of String, Integer),
                                      bhrThreshold As Double,
                                      empiricalProbability As Double) As KOAssignmentCandidate
            ' 获取KO的总基因数 x
            If Not koGeneCounts.ContainsKey(ko) Then Return Nothing
            Dim x As Integer = koGeneCounts(ko)

            ' 找到该KO组中得分最高的比对，用于获取 S_h, m, n
            Dim topHit = groupHits.OrderByDescending(Function(h) h.score).First()
            Dim Sh As Double = topHit.score
            Dim m As Integer = topHit.query_length
            Dim ni As Integer = topHit.hit_length ' n

            ' 计算满足BHR阈值的基因数 N
            Dim N As Integer = Aggregate h As BestHit
                               In groupHits
                               Where h.SBHScore >= bhrThreshold
                               Into Count
            ' 计算S_KO
            Dim score As Double = CalculateAssignmentScore(Sh, m, ni, x, N, empiricalProbability)

            Return New KOAssignmentCandidate With {
                .QueryName = topHit.QueryName,
                .KO = ko,
                .AssignmentScore = score,
                .Sh = Sh,
                .N = N,
                .X = x
            }
        End Function
    End Module
End Namespace