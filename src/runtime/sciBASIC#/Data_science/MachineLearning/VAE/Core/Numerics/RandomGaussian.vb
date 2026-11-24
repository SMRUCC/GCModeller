' ============================================================================
' RandomGaussian.vb - 可复现的随机数服务
'
' 修正模型层内部散落 `New Random()` 导致结果不可复现的问题
' （GMVAE.vb L644、GMM.vb L664、GMM.vb L202）。所有采样必须经由本服务。
' ============================================================================

Namespace Core.Numerics

    ''' <summary>
    ''' 可复现的随机数服务：种子隔离派生、正态 / 均匀 / 类别采样。
    ''' </summary>
    Public Class RandomGaussian

        Private ReadOnly _rng As Random
        Private ReadOnly _seed As Integer

        ''' <summary>使用时间相关的种子创建服务。</summary>
        Public Sub New()
            _seed = Environment.TickCount
            _rng = New Random(_seed)
        End Sub

        ''' <summary>使用指定的种子创建服务。</summary>
        Public Sub New(seed As Integer)
            _seed = seed
            _rng = New Random(seed)
        End Sub

        ''' <summary>
        ''' 包装一个既有的 <see cref="Random"/> 实例（用于向后兼容那些
        ''' 直接接收 <see cref="Random"/> 参数的公开 API）。
        ''' </summary>
        Public Sub New(source As Random)
            If source Is Nothing Then
                Throw New ArgumentNullException(NameOf(source))
            End If

            _seed = 0
            _rng = source
        End Sub

        ''' <summary>创建服务时使用的种子。</summary>
        Public ReadOnly Property Seed As Integer
            Get
                Return _seed
            End Get
        End Property

        ''' <summary>[0, 1) 之间的均匀分布样本。</summary>
        Public Function NextDouble() As Double
            Return _rng.NextDouble()
        End Function

        ''' <summary>[0, upperBound) 之间的均匀整数。</summary>
        Public Function NextIndex(upperBound As Integer) As Integer
            Return _rng.Next(upperBound)
        End Function

        ''' <summary>标准正态样本。</summary>
        Public Function NextNormal() As Double
            Return BoxMuller.Sample(_rng)
        End Function

        ''' <summary>指定均值与标准差的正态样本。</summary>
        Public Function NextNormal(mean As Double, stdDev As Double) As Double
            Return mean + stdDev * BoxMuller.Sample(_rng)
        End Function

        ''' <summary>成对填充标准正态样本。</summary>
        Public Sub NextNormals(buffer As Double(), start As Integer, count As Integer)
            Call BoxMuller.Fill(_rng, buffer, start, count)
        End Sub

        ''' <summary>生成指定长度的标准正态样本数组。</summary>
        Public Function NextNormals(count As Integer) As Double()
            Return BoxMuller.Fill(_rng, count)
        End Function

        ''' <summary>
        ''' 按概率分布采样一个类别下标。概率之和不必严格为 1，内部会按累计和判定。
        ''' </summary>
        Public Function NextCategorical(probability As Double()) As Integer
            If probability Is Nothing OrElse probability.Length = 0 Then
                Return -1
            End If

            Dim r As Double = _rng.NextDouble()
            Dim acc As Double = 0

            For i As Integer = 0 To probability.Length - 1
                acc += probability(i)

                If r <= acc Then
                    Return i
                End If
            Next

            Return probability.Length - 1
        End Function

        ''' <summary>
        ''' 从同一个种子派生出可复现的独立子流。用于让不同组件（初始化 / 采样 /
        ''' 打乱）互不干扰，同时保持整体可复现。
        ''' </summary>
        Public Function Derive(offset As Integer) As RandomGaussian
            Return New RandomGaussian(_seed + offset)
        End Function

        Public Overrides Function ToString() As String
            Return $"RandomGaussian(seed={_seed})"
        End Function

    End Class

End Namespace
