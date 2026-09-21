' ============================================================
' MetabolicTrainingSet.vb - 代谢网络训练数据
' ============================================================
' Metaboliq 的 PINN 风格训练器需要的时序数据。全部在归一化空间
' （log1p + z-score）中给出，时间网格必须严格单调递增。
' ============================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Public Class MetabolicTrainingSet

    ''' <summary>时间网格，严格单调递增</summary>
    Public Property Times As Double()

    ''' <summary>观测到的内部代谢物浓度 [T × m]</summary>
    Public Property Observed As Tensor

    ''' <summary>各反应的酶水平 [T × r]，取值 [0,1]</summary>
    Public Property Enzymes As Tensor

    ''' <summary>边界（胞外）代谢物浓度 [T × nB]</summary>
    Public Property Boundary As Tensor

    ''' <summary>观测到的反应通量（可选，用于 λ_flux 监督项）[T × r]</summary>
    Public Property Flux As Tensor

    ''' <summary>训练配置；为 Nothing 时用默认配置</summary>
    Public Property Config As SMRUCC.genomics.Analysis.Metaboliq.MetabolicTrainerConfig

    Public Function Validate() As Boolean
        If Times.IsNullOrEmpty Then
            Throw New InvalidOperationException("训练数据缺少时间网格")
        End If
        If Observed Is Nothing OrElse Enzymes Is Nothing OrElse Boundary Is Nothing Then
            Throw New InvalidOperationException("训练数据缺少观测 / 酶 / 边界矩阵")
        End If

        For i As Integer = 1 To Times.Length - 1
            If Times(i) <= Times(i - 1) Then
                Throw New InvalidOperationException("时间网格必须严格单调递增")
            End If
        Next

        If Observed.Shape(0) <> Times.Length Then
            Throw New InvalidOperationException($"观测矩阵行数 {Observed.Shape(0)} 与时间步数 {Times.Length} 不一致")
        End If

        Return True
    End Function

End Class
