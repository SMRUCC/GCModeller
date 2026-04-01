Namespace MetabolicThermodynamics

    ''' <summary>
    ''' 计算结果封装
    ''' </summary>
    Public Class ThermoBoundResult
        ''' <summary>反应标准吉布斯自由能变 ΔG'0 (kJ/mol)</summary>
        Public Property DeltaG0 As Double

        ''' <summary>实际吉布斯自由能变 ΔG' (kJ/mol)</summary>
        Public Property DeltaGActual As Double

        ''' <summary>判定出的反应方向性描述</summary>
        Public Property Direction As String

        ''' <summary>计算得出的 FBA 下界</summary>
        Public Property LowerBound As Double

        ''' <summary>计算得出的 FBA 上界</summary>
        Public Property UpperBound As Double
    End Class
End Namespace