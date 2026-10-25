Imports std = System.Math

Namespace Fusion

    ''' <summary>
    ''' 跨组学一致性奖励项 C^(S)(j)（method.md 步骤4 的核心创新点）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 定义每个样本 j 上各组学方向的符号一致度：
    ''' </para>
    ''' <code>
    ''' a_j = ( 2 / (K(K-1)) ) * Σ_{k &lt; k'} sign(ES_k(j)) * sign(ES_k'(j))  ∈ [-1, 1]
    ''' </code>
    ''' <para>
    ''' 全部组学同向时 a_j = 1；完全相反时 a_j = -1；正负各半时接近 0。
    ''' 融合时采用<b>归一化乘性形式</b>：
    ''' </para>
    ''' <code>
    ''' ES_multi(j) = ES_raw(j) * (1 + λ·a_j) / (1 + λ)
    ''' </code>
    ''' <para>
    ''' 该形式有三个好处：(1) λ = 0 时严格退化为加权平均（Baseline 1）；
    ''' (2) 方向一致的样本不被放大，方向矛盾的样本被压向 0，
    ''' 因此 <c>|ES_multi| ≤ |ES_raw|</c>，不会越出原方法的取值范围；
    ''' (3) 相比加性写法不会因为 λ 而破坏分数边界。
    ''' </para>
    ''' </remarks>
    Public Module ConsistencyReward

        ''' <summary>
        ''' 计算每条向量（组学层）在每个样本上的符号一致度。
        ''' </summary>
        ''' <param name="vectors">按组学层组织的 ES 向量数组</param>
        ''' <returns>长度为样本数的一致度向量，取值 [-1, 1]</returns>
        Public Function SampleAgreement(vectors As Double()()) As Double()
            If vectors Is Nothing OrElse vectors.Length = 0 Then
                Return New Double() {}
            End If

            Dim nSample As Integer = vectors(0).Length
            Dim result As Double() = New Double(nSample - 1) {}

            If vectors.Length = 1 Then
                ' 只有一层时视为完全一致，从而不施加任何惩罚
                For j As Integer = 0 To nSample - 1
                    result(j) = 1.0
                Next

                Return result
            End If

            Dim pairs As Double = vectors.Length * (vectors.Length - 1) / 2.0

            For j As Integer = 0 To nSample - 1
                Dim acc As Double = 0

                For a As Integer = 0 To vectors.Length - 2
                    Dim signA As Double = SignOf(vectors(a)(j))

                    For b As Integer = a + 1 To vectors.Length - 1
                        acc += signA * SignOf(vectors(b)(j))
                    Next
                Next

                result(j) = acc / pairs
            Next

            Return result
        End Function

        ''' <summary>
        ''' 施加一致性奖励，返回融合后的分数向量。
        ''' </summary>
        ''' <param name="raw">加权平均分数 ES_raw</param>
        ''' <param name="agreement">符号一致度 a_j</param>
        ''' <param name="lambda">超参 λ（&lt;= 0 时不施加）</param>
        Public Function Apply(raw As Double(), agreement As Double(), lambda As Double) As Double()
            Dim result As Double() = New Double(raw.Length - 1) {}

            If lambda <= 0 OrElse agreement Is Nothing OrElse agreement.Length <> raw.Length Then
                Call Array.Copy(raw, result, raw.Length)

                Return result
            End If

            For j As Integer = 0 To raw.Length - 1
                result(j) = raw(j) * (1 + lambda * agreement(j)) / (1 + lambda)
            Next

            Return result
        End Function

        Private Function SignOf(x As Double) As Double
            If x > 0 Then
                Return 1.0
            ElseIf x < 0 Then
                Return -1.0
            Else
                Return 0.0
            End If
        End Function

    End Module

End Namespace
