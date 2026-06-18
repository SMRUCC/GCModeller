Imports Microsoft.VisualBasic.Math.Calculus.Dynamics

''' <summary>
''' 将基因表达出来的mRNA翻译为蛋白质，采用ODEs动力学系统来建模
''' </summary>
Public Class TranslationSystem : Inherits SubNetwork

    ReadOnly core As SolverIterator

    Sub New(cell As VirtualCella)
        Call MyBase.New(cell)
    End Sub

    Public Overrides Sub RunStep()
        Call core.Tick()
    End Sub

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Throw New NotImplementedException()
    End Function
End Class
