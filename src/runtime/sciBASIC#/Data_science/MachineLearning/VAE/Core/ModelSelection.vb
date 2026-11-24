' ============================================================================
' ModelSelection.vb - BIC / AIC 与成分数网格搜索
'
' 生产环境里「K 应该取多少」通常比「参数怎么估」更重要。这里提供：
'   - BIC / AIC 的标准公式（对数似然取求和值，不是均值）
'   - 基于 IModelScore 契约的通用网格搜索
' ============================================================================

Imports std = System.Math

Namespace Core

    ''' <summary>一个候选模型的评分结果。</summary>
    Public Class ModelSelectionResult

        ''' <summary>候选模型的成分数（或其他候选标识）。</summary>
        Public Property NComponents As Integer

        ''' <summary>训练集上的对数似然（求和）。</summary>
        Public Property LogLikelihood As Double

        ''' <summary>自由参数个数。</summary>
        Public Property Parameters As Integer

        ''' <summary>贝叶斯信息准则。</summary>
        Public Property BIC As Double

        ''' <summary>赤池信息准则。</summary>
        Public Property AIC As Double

        Public Overrides Function ToString() As String
            Return $"K={NComponents}, logLik={LogLikelihood:F4}, params={Parameters}, BIC={BIC:F4}, AIC={AIC:F4}"
        End Function

    End Class

    ''' <summary>
    ''' 模型选择：BIC / AIC 计算与候选网格搜索。
    ''' </summary>
    Public Module ModelSelection

        ''' <summary>
        ''' 贝叶斯信息准则：BIC = -2·lnL + p·ln(n)。
        ''' 数值越小越好。
        ''' </summary>
        Public Function Bic(logLikelihood As Double, parameters As Integer, sampleCount As Integer) As Double
            If sampleCount <= 0 Then
                Return Double.PositiveInfinity
            End If

            If Double.IsNegativeInfinity(logLikelihood) Then
                Return Double.PositiveInfinity
            End If

            Return -2.0 * logLikelihood + parameters * std.Log(sampleCount)
        End Function

        ''' <summary>
        ''' 赤池信息准则：AIC = -2·lnL + 2p。数值越小越好。
        ''' </summary>
        Public Function Aic(logLikelihood As Double, parameters As Integer) As Double
            If Double.IsNegativeInfinity(logLikelihood) Then
                Return Double.PositiveInfinity
            End If

            Return -2.0 * logLikelihood + 2.0 * parameters
        End Function

        ''' <summary>
        ''' 在候选成分数上做网格搜索，返回按 BIC 升序排列的评分表。
        ''' </summary>
        ''' <typeparam name="TModel">满足 <see cref="IModelScore"/> 的模型类型</typeparam>
        ''' <param name="candidates">候选成分数列表</param>
        ''' <param name="build">按成分数构造模型（已完成 Fit 前的初始化）</param>
        ''' <param name="fit">执行训练</param>
        ''' <param name="sampleCount">样本数量，用于 BIC 的惩罚项</param>
        Public Function GridSearch(Of TModel As IModelScore)(candidates As Integer(),
                                                             build As Func(Of Integer, TModel),
                                                             fit As Action(Of TModel),
                                                             sampleCount As Integer) As ModelSelectionResult()
            If candidates Is Nothing OrElse candidates.Length = 0 Then
                Return New ModelSelectionResult() {}
            End If

            Dim results As New List(Of ModelSelectionResult)

            For Each k As Integer In candidates
                Dim model As TModel = build(k)

                Call fit(model)

                Dim score As IModelScore = model
                Dim item As New ModelSelectionResult With {
                    .NComponents = k,
                    .LogLikelihood = score.LogLikelihood,
                    .Parameters = score.ParametersCount
                }

                item.BIC = Bic(item.LogLikelihood, item.Parameters, sampleCount)
                item.AIC = Aic(item.LogLikelihood, item.Parameters)

                results.Add(item)
            Next

            Return results.OrderBy(Function(x) x.BIC).ToArray()
        End Function

        ''' <summary>按 BIC 取最优候选；空集合时返回 <c>Nothing</c>。</summary>
        Public Function BestByBic(results As IEnumerable(Of ModelSelectionResult)) As ModelSelectionResult
            If results Is Nothing Then
                Return Nothing
            End If

            Dim best As ModelSelectionResult = Nothing

            For Each item As ModelSelectionResult In results
                If best Is Nothing OrElse item.BIC < best.BIC Then
                    best = item
                End If
            Next

            Return best
        End Function

        ''' <summary>
        ''' 多元高斯混合模型的自由参数个数：
        ''' 权重 (K-1) + 均值 (K·D) + 协方差。
        ''' </summary>
        ''' <param name="nComponents">成分数 K</param>
        ''' <param name="dimensions">数据维度 D</param>
        ''' <param name="covarianceKind">0=full, 1=diagonal, 2=tied, 3=spherical</param>
        Public Function GaussianMixtureParameters(nComponents As Integer, dimensions As Integer,
                                                  covarianceKind As Integer) As Integer
            Dim weights As Integer = nComponents - 1
            Dim means As Integer = nComponents * dimensions

            Select Case covarianceKind
                Case 0
                    Return weights + means + nComponents * dimensions * (dimensions + 1) \ 2
                Case 1
                    Return weights + means + nComponents * dimensions
                Case 2
                    Return weights + means + dimensions * (dimensions + 1) \ 2
                Case Else
                    Return weights + means + nComponents
            End Select
        End Function

    End Module

End Namespace
