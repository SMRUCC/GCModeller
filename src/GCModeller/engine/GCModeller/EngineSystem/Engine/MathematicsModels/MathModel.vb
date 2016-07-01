Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects

Namespace EngineSystem.MathematicsModels

    Public MustInherit Class MathematicsModel : Inherits RuntimeObject

        ''' <summary>
        ''' 正态分布函数，采用默认参数则为标准正太分布
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="u"></param>
        ''' <param name="sigma"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function NormalDistribution(x As Double, Optional u As Double = 0, Optional sigma As Double = 1) As Double
            Dim c As Double = (System.Math.Sqrt(2 * System.Math.PI) * sigma)
            Dim p As Double = ((x - u) / sigma) ^ 2 * -0.5

            Dim value As Double = System.Math.E ^ p / c
            Return value
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="vMax">最大反应速率</param>
        ''' <param name="S">底物的浓度</param>
        ''' <param name="Km">米氏常数</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function MichaelisMenten(vMax As Double, S As Double, Km As Double) As Double
            Return (vMax * [S]) / (Km + [S])
        End Function

        ''' <summary>
        ''' 希尔函数
        ''' </summary>
        ''' <param name="k1n">k1-</param>
        ''' <param name="k1p">k1+</param>
        ''' <param name="k2">k2小于k3，正调控</param>
        ''' <param name="k3"></param>
        ''' <param name="X">调控因子的浓度</param>
        ''' <param name="n">需要多少个调控因子参加调控反应</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Hill(k1n As Double, k1p As Double, k2 As Double, k3 As Double, X As Double, n As Double) As Double
            Dim Kn = k1n / k1p, p As Double
            Dim Xn As Double = X ^ n

            If k2 < k3 Then
                p = k2 / k3
                Return k3 * (p + (1 - p) * (Xn / (Kn + Xn)))
            Else
                p = k3 / k2
                Return k2 * (p + (1 - p) * (Kn / (Kn + Xn)))
            End If
        End Function

        ''' <summary>
        ''' 摩尔气体常数
        ''' </summary>
        ''' <remarks></remarks>
        Const R As Double = 8.314

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="A"></param>
        ''' <param name="Ea">右端分子质量与左端分子质量之差</param>
        ''' <param name="T">当前温度</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Arrhenius(A As Double, Ea As Double, T As Double) As Double
            Dim p = Ea / (R * T) * -1
            Dim value = A * System.Math.E ^ p
            Return value
        End Function

        ''' <summary>
        ''' 在初始化带些方程的时候计算A系数所需要的
        ''' </summary>
        ''' <param name="K"></param>
        ''' <param name="T"></param>
        ''' <param name="Ea">右端分子质量与左端分子质量之差</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function A(K As Double, T As Double, Ea As Double) As Double
            Dim p = Ea / (R * T) * -1
            Dim value = K / (System.Math.E ^ p)
            Return value
        End Function

        Protected Friend Sub New()
        End Sub
    End Class
End Namespace