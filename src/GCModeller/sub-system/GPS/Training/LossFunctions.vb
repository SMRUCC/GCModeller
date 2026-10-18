Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace Training

    ''' <summary>
    ''' 训练损失与评估指标（method.md §5.1）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>autofocus 损失</b>：<c>L = mean(|y − ŷ|^(2+γ))</c>。误差指数从 2 提升到 2+γ，
    ''' 使差异表达基因（大误差）被指数级放大权重，避免被大量"不变化的基因"主导。
    ''' </para>
    ''' <para>
    ''' <b>方向感知损失</b>：原文定义为 <c>[sign(y−y_prev) − sign(ŷ−y_prev)]²</c>，
    ''' 它对 <c>ŷ</c> 几乎处处导数为 0，无法直接用于梯度下降。
    ''' 训练时改用光滑替代 <c>L = mean(1 − tanh(β·Δŷ·Δy))</c>
    ''' （方向一致时趋近 0、反向时趋近 2），
    ''' 而 <see cref="DirectionAccuracy"/> 保留原文的精确判据用于评估。
    ''' </para>
    ''' <para>
    ''' 全部损失在双精度下计算：其指数放大会放大舍入误差，属于 MustDouble 语义。
    ''' </para>
    ''' </remarks>
    Public Module LossFunctions

        ''' <summary>
        ''' autofocus 损失：<c>mean(|e|^(2+γ))</c>。
        ''' </summary>
        ''' <param name="prediction">预测 <c>[N, 1]</c></param>
        ''' <param name="target">真值 <c>[N, 1]</c></param>
        ''' <param name="gamma">指数增量（0 退化为 MSE）</param>
        ''' <returns>平均损失</returns>
        Public Function Autofocus(prediction As Tensor, target As Tensor, gamma As Double) As Double
            Dim p As Double() = prediction.Data
            Dim t As Double() = target.Data
            Dim exponent As Double = 2.0 + gamma
            Dim sum As Double = 0.0

            For i As Integer = 0 To p.Length - 1
                sum += std.Pow(std.Abs(p(i) - t(i)), exponent)
            Next

            Return sum / p.Length
        End Function

        ''' <summary>
        ''' autofocus 损失的梯度：<c>(2+γ)·sign(e)·|e|^(1+γ) / N</c>。
        ''' </summary>
        ''' <param name="prediction">预测 <c>[N, 1]</c></param>
        ''' <param name="target">真值 <c>[N, 1]</c></param>
        ''' <param name="gamma">指数增量</param>
        ''' <returns>对预测的梯度 <c>[N, 1]</c></returns>
        Public Function AutofocusGradient(prediction As Tensor, target As Tensor, gamma As Double) As Tensor
            Dim result As Tensor = New Tensor(prediction.Shape)
            Dim p As Double() = prediction.Data
            Dim t As Double() = target.Data
            Dim g As Double() = result.Data
            Dim exponent As Double = 1.0 + gamma
            Dim scale As Double = (2.0 + gamma) / p.Length

            For i As Integer = 0 To p.Length - 1
                Dim e As Double = p(i) - t(i)
                Dim sign As Double = If(e < 0.0, -1.0, If(e > 0.0, 1.0, 0.0))

                g(i) = scale * sign * std.Pow(std.Abs(e), exponent)
            Next

            Call result.MarkHostModified()

            Return result
        End Function

        ''' <summary>
        ''' 掩码 autofocus 损失：只在 <paramref name="mask"/> 为 True 的基因上求平均。
        ''' </summary>
        ''' <param name="prediction">预测 <c>[N, 1]</c></param>
        ''' <param name="target">真值 <c>[N, 1]</c></param>
        ''' <param name="mask">长度为 N 的基因掩码</param>
        ''' <param name="gamma">指数增量</param>
        ''' <returns>掩码上的平均损失；掩码为空时返回 0</returns>
        Public Function MaskedAutofocus(prediction As Tensor, target As Tensor,
                                        mask As Boolean(), gamma As Double) As Double
            Dim p As Double() = prediction.Data
            Dim t As Double() = target.Data
            Dim exponent As Double = 2.0 + gamma
            Dim sum As Double = 0.0
            Dim count As Integer = 0

            For i As Integer = 0 To p.Length - 1
                If mask IsNot Nothing AndAlso i < mask.Length AndAlso Not mask(i) Then Continue For

                sum += std.Pow(std.Abs(p(i) - t(i)), exponent)
                count += 1
            Next

            Return If(count > 0, sum / count, 0.0)
        End Function

        ''' <summary>
        ''' 掩码 autofocus 损失的梯度（未掩码位置为 0）。
        ''' </summary>
        ''' <param name="prediction">预测 <c>[N, 1]</c></param>
        ''' <param name="target">真值 <c>[N, 1]</c></param>
        ''' <param name="mask">长度为 N 的基因掩码</param>
        ''' <param name="gamma">指数增量</param>
        ''' <returns>对预测的梯度 <c>[N, 1]</c></returns>
        Public Function MaskedAutofocusGradient(prediction As Tensor, target As Tensor,
                                                mask As Boolean(), gamma As Double) As Tensor
            Dim result As Tensor = New Tensor(prediction.Shape)
            Dim p As Double() = prediction.Data
            Dim t As Double() = target.Data
            Dim g As Double() = result.Data
            Dim exponent As Double = 1.0 + gamma
            Dim count As Integer = 0

            For i As Integer = 0 To p.Length - 1
                If mask IsNot Nothing AndAlso i < mask.Length AndAlso Not mask(i) Then Continue For

                count += 1
            Next

            If count = 0 Then
                Call result.MarkHostModified()
                Return result
            End If

            Dim scale As Double = (2.0 + gamma) / count

            For i As Integer = 0 To p.Length - 1
                If mask IsNot Nothing AndAlso i < mask.Length AndAlso Not mask(i) Then Continue For

                Dim e As Double = p(i) - t(i)
                Dim sign As Double = If(e < 0.0, -1.0, If(e > 0.0, 1.0, 0.0))

                g(i) = scale * sign * std.Pow(std.Abs(e), exponent)
            Next

            Call result.MarkHostModified()

            Return result
        End Function

        ''' <summary>
        ''' 光滑方向一致性损失：<c>mean(1 − tanh(β·Δŷ·Δy))</c>。
        ''' </summary>
        ''' <param name="prediction">预测 <c>[N, 1]</c></param>
        ''' <param name="target">真值 <c>[N, 1]</c></param>
        ''' <param name="previous">窗口最后一个时刻 <c>[N]</c></param>
        ''' <param name="beta">温度参数，越大越接近 sign</param>
        ''' <returns>平均损失</returns>
        Public Function SoftDirection(prediction As Tensor, target As Tensor,
                                      previous As Double(), beta As Double) As Double
            Dim p As Double() = prediction.Data
            Dim t As Double() = target.Data
            Dim sum As Double = 0.0

            For i As Integer = 0 To p.Length - 1
                Dim dy As Double = t(i) - previous(i)
                Dim dPrediction As Double = p(i) - previous(i)

                sum += 1.0 - std.Tanh(beta * dPrediction * dy)
            Next

            Return sum / p.Length
        End Function

        ''' <summary>
        ''' 光滑方向一致性损失的梯度。
        ''' </summary>
        ''' <param name="prediction">预测 <c>[N, 1]</c></param>
        ''' <param name="target">真值 <c>[N, 1]</c></param>
        ''' <param name="previous">窗口最后一个时刻 <c>[N]</c></param>
        ''' <param name="beta">温度参数</param>
        ''' <returns>对预测的梯度 <c>[N, 1]</c></returns>
        Public Function SoftDirectionGradient(prediction As Tensor, target As Tensor,
                                              previous As Double(), beta As Double) As Tensor
            Dim result As Tensor = New Tensor(prediction.Shape)
            Dim p As Double() = prediction.Data
            Dim t As Double() = target.Data
            Dim g As Double() = result.Data
            Dim scale As Double = -beta / p.Length

            For i As Integer = 0 To p.Length - 1
                Dim dy As Double = t(i) - previous(i)
                Dim dPrediction As Double = p(i) - previous(i)
                Dim th As Double = std.Tanh(beta * dPrediction * dy)

                g(i) = scale * dy * (1.0 - th * th)
            Next

            Call result.MarkHostModified()

            Return result
        End Function

        ''' <summary>
        ''' 方向准确率（原文的精确定义，仅用于评估）：
        ''' <c>mean(sign(ŷ − y_prev) == sign(y − y_prev))</c>。
        ''' </summary>
        ''' <param name="prediction">预测 <c>[N, 1]</c></param>
        ''' <param name="target">真值 <c>[N, 1]</c></param>
        ''' <param name="previous">窗口最后一个时刻 <c>[N]</c></param>
        ''' <returns>一致比例，0~1</returns>
        Public Function DirectionAccuracy(prediction As Tensor, target As Tensor, previous As Double()) As Double
            Dim p As Double() = prediction.Data
            Dim t As Double() = target.Data
            Dim agree As Integer = 0

            For i As Integer = 0 To p.Length - 1
                If SignOf(p(i) - previous(i)) = SignOf(t(i) - previous(i)) Then
                    agree += 1
                End If
            Next

            Return agree / p.Length
        End Function

        ''' <summary>
        ''' 高斯负对数似然（可选的贝叶斯不确定性损失）。
        ''' </summary>
        ''' <param name="prediction">预测均值 <c>[N, 1]</c></param>
        ''' <param name="sigma">预测标准差 <c>[N, 1]</c>（softplus 输出，恒正）</param>
        ''' <param name="target">真值 <c>[N, 1]</c></param>
        ''' <param name="gamma">autofocus 指数增量</param>
        ''' <returns>平均负对数似然</returns>
        Public Function GaussianNll(prediction As Tensor, sigma As Tensor, target As Tensor, gamma As Double) As Double
            Dim p As Double() = prediction.Data
            Dim s As Double() = sigma.Data
            Dim t As Double() = target.Data
            Dim exponent As Double = 2.0 + gamma
            Dim sum As Double = 0.0

            For i As Integer = 0 To p.Length - 1
                Dim variance As Double = std.Max(s(i) * s(i), 1.0E-08)

                sum += std.Pow(std.Abs(p(i) - t(i)), exponent) / variance + std.Log(variance)
            Next

            Return sum / p.Length
        End Function

        ''' <summary>
        ''' 高斯负对数似然的梯度（分别对均值与标准差）。
        ''' </summary>
        ''' <param name="prediction">预测均值 <c>[N, 1]</c></param>
        ''' <param name="sigma">预测标准差 <c>[N, 1]</c></param>
        ''' <param name="target">真值 <c>[N, 1]</c></param>
        ''' <param name="gamma">指数增量</param>
        ''' <param name="dSigma">输出：对标准差的梯度 <c>[N, 1]</c></param>
        ''' <returns>对均值的梯度 <c>[N, 1]</c></returns>
        Public Function GaussianNllGradients(prediction As Tensor, sigma As Tensor, target As Tensor,
                                              gamma As Double, ByRef dSigma As Tensor) As Tensor
            Dim dPrediction As Tensor = New Tensor(prediction.Shape)
            dSigma = New Tensor(sigma.Shape)

            Dim p As Double() = prediction.Data
            Dim s As Double() = sigma.Data
            Dim t As Double() = target.Data
            Dim gp As Double() = dPrediction.Data
            Dim gs As Double() = dSigma.Data
            Dim exponent As Double = 2.0 + gamma
            Dim scale As Double = 1.0 / p.Length

            For i As Integer = 0 To p.Length - 1
                Dim e As Double = p(i) - t(i)
                Dim variance As Double = std.Max(s(i) * s(i), 1.0E-08)
                Dim absE As Double = std.Abs(e)
                Dim sign As Double = If(e < 0.0, -1.0, If(e > 0.0, 1.0, 0.0))
                Dim powered As Double = std.Pow(absE, exponent)

                ' d/dμ [ |e|^(2+γ)/σ² ] = (2+γ)·sign(e)·|e|^(1+γ)/σ²
                gp(i) = scale * (2.0 + gamma) * sign * std.Pow(absE, 1.0 + gamma) / variance

                ' d/dσ [ |e|^(2+γ)/σ² + 2·ln σ ] = −2·|e|^(2+γ)/σ³ + 2/σ
                gs(i) = scale * (-2.0 * powered / (variance * s(i)) + 2.0 / s(i))
            Next

            Call dPrediction.MarkHostModified()
            Call dSigma.MarkHostModified()

            Return dPrediction
        End Function

        ''' <summary>符号函数（0 视为正向）。</summary>
        ''' <param name="value">输入值</param>
        ''' <returns>-1、0 或 1</returns>
        Private Function SignOf(value As Double) As Integer
            If value > 0.0 Then Return 1
            If value < 0.0 Then Return -1

            Return 0
        End Function

    End Module

End Namespace
