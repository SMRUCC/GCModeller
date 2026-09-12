Namespace Evolution.MaximumLikelihood

    ''' <summary>
    ''' 内置的氨基酸经验替换模型
    ''' </summary>
    Public Enum AminoAcidModel
        ''' <summary>Dayhoff (1978)</summary>
        Dayhoff
        ''' <summary>Jones-Taylor-Thornton (1992)</summary>
        JTT
        ''' <summary>Whelan-Goldman (2001)</summary>
        WAG
        ''' <summary>Le-Gascuel (2008)</summary>
        LG
    End Enum

    ''' <summary>
    ''' 替换模型抽象：描述氨基酸（或其他字符状态）随时间发生替换的概率过程。
    ''' </summary>
    Public MustInherit Class SubstitutionModel

        ''' <summary>
        ''' 平衡频率向量 π
        ''' </summary>
        Public MustOverride ReadOnly Property Pi As Double()

        ''' <summary>
        ''' 状态空间的大小
        ''' </summary>
        Public ReadOnly Property Dimension As Integer
            Get
                Return Pi.Length
            End Get
        End Property

        ''' <summary>
        ''' 计算在时间 <paramref name="t"/> 内从状态 i 变为状态 j 的转移概率矩阵 <c>P(t) = e^{Qt}</c>。
        ''' </summary>
        Public MustOverride Function TransitionProbability(t As Double) As Double()()

        ''' <summary>
        ''' 依据给定的氨基酸替换模型名称加载模型实例。
        ''' </summary>
        Public Shared Function Load(model As AminoAcidModel) As SubstitutionModel
            Return New EmpiricalAminoAcidModel(model)
        End Function
    End Class

    ''' <summary>
    ''' 基于经验交换率矩阵（Dayhoff / JTT / WAG / LG）的氨基酸替换模型。
    ''' </summary>
    ''' <remarks>
    ''' 速率矩阵按 <c>Q_ij = S_ij * π_j</c>（i≠j）、<c>Q_ii = -Σ_{j≠i} Q_ij</c> 构建，
    ''' 并归一化使得每位点的期望替换速率为 1，即以“每个位点一次替换”为时间单位。
    ''' </remarks>
    Public Class EmpiricalAminoAcidModel : Inherits SubstitutionModel

        Private ReadOnly _pi As Double()
        Private ReadOnly _exchangeability As Double()()
        Private ReadOnly _rateMatrix As Double()()
        Private ReadOnly _exponential As ReversibleMatrixExponential

        Public Overrides ReadOnly Property Pi As Double()
            Get
                Return _pi
            End Get
        End Property

        ''' <summary>
        ''' 对称的交换率矩阵 S（已归一化）
        ''' </summary>
        Public ReadOnly Property Exchangeability As Double()()
            Get
                Return _exchangeability
            End Get
        End Property

        ''' <summary>
        ''' 瞬时速率矩阵 Q（行和为 0）
        ''' </summary>
        Public ReadOnly Property RateMatrix As Double()()
            Get
                Return _rateMatrix
            End Get
        End Property

        Public ReadOnly Property Model As AminoAcidModel

        Public Sub New(model As AminoAcidModel)
            Me.Model = model

            Dim raw = AminoAcidModels.LoadRaw(model)
            Dim s As Double()() = raw.Exchangeability

            Me._pi = raw.Pi

            Dim n As Integer = _pi.Length

            ' 归一化：Σ_i π_i * Σ_{j≠i} S_ij π_j = 1
            Dim mu As Double = 0

            For i As Integer = 0 To n - 1
                Dim rowSum As Double = 0

                For j As Integer = 0 To n - 1
                    If i <> j Then
                        rowSum += s(i)(j) * _pi(j)
                    End If
                Next

                mu += _pi(i) * rowSum
            Next

            If mu <= 0 Then
                mu = 1
            End If

            For i As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    s(i)(j) /= mu
                Next
            Next

            Me._exchangeability = s

            ' 构建 Q
            Dim q(n - 1)() As Double

            For i As Integer = 0 To n - 1
                q(i) = New Double(n - 1) {}
            Next

            For i As Integer = 0 To n - 1
                Dim diag As Double = 0

                For j As Integer = 0 To n - 1
                    If i <> j Then
                        q(i)(j) = s(i)(j) * _pi(j)
                        diag += q(i)(j)
                    End If
                Next

                q(i)(i) = -diag
            Next

            Me._rateMatrix = q
            Me._exponential = New ReversibleMatrixExponential(s, _pi)
        End Sub

        Public Overrides Function TransitionProbability(t As Double) As Double()()
            Dim n As Integer = _pi.Length

            If t <= 0 Then
                Dim identity(n - 1)() As Double

                For i As Integer = 0 To n - 1
                    identity(i) = New Double(n - 1) {}
                    identity(i)(i) = 1
                Next

                Return identity
            End If

            Return _exponential.Evaluate(t)
        End Function
    End Class

End Namespace
