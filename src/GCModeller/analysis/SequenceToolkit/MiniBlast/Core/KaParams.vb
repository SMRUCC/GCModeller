Namespace Core

    ''' <summary>Karlin-Altschul 参数集</summary>
    Public Class KaParams

        Public Property Lambda As Double
        Public Property K As Double
        Public Property H As Double

        ''' <summary>[式5-1] E = K·m·n·e^(-λS)</summary>
        Public Function EValue(rawScore As Double, searchSpace As Double) As Double
            Return K * searchSpace * Math.Exp(-Lambda * rawScore)
        End Function

        ''' <summary>[式5-1] m、n 分列的便捷重载（searchSpace = m·n）</summary>
        Public Function EValue(m As Double, n As Double, rawScore As Double) As Double
            Return K * m * n * Math.Exp(-Lambda * rawScore)
        End Function

        ''' <summary>[式5-2] Bit Score</summary>
        Public Function BitScore(rawScore As Double) As Double
            Return (Lambda * rawScore - Math.Log(K)) / Math.Log(2.0)
        End Function

    End Class

End Namespace