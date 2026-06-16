Imports Microsoft.VisualBasic.Math.Calculus.Dynamics

''' <summary>
''' 将基因表达出来的mRNA翻译为蛋白质，采用ODEs动力学系统来建模
''' </summary>
Public Class TranslationSystem : Inherits SubNetwork

    ReadOnly core As SolverIterator

    Public Overrides Sub RunStep(cell As VirtualCella)
        Call core.Tick()
    End Sub
End Class
