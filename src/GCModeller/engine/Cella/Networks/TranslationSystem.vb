' ============================================================
' TranslationSystem.vb - 翻译系统
' ============================================================
' 把转录本翻译为蛋白质（酶 / 转运蛋白 / 转录因子）：
'
'     dP_i/dt = k_tl,i · mRNA_i − (k_deg,i + k_dil) · P_i
'
' 每个基因一个方程，速率常数跨越数个数量级 → 刚性系统，用 CVODE 的 BDF
' 求解，并提供解析（对角）雅可比以减少 Newton 迭代次数。
'
' mRNA 在本步内被视为常量（算子分裂：转录调控网络先以离散跳变更新 mRNA，
' 翻译系统再在该常量下积分蛋白质），这是多时间尺度耦合的标准做法。
' ============================================================

Imports Microsoft.VisualBasic.Math.Sundials.CVODE

''' <summary>
''' 将基因表达出来的mRNA翻译为蛋白质，采用 CVODE(BDF) 刚性常微分方程系统来建模
''' </summary>
Public Class TranslationSystem : Inherits OdeSubNetwork

    ReadOnly translationRate As Double()
    ReadOnly decayRate As Double()

    ''' <summary>本步内视为常量的 mRNA 输入</summary>
    Private mrnaInput As Double()

    Public Sub New(cell As VirtualCella, blueprint As CellaBlueprint)
        Call MyBase.New(cell, cell.State.NGene, NameOf(TranslationSystem))

        Dim genes As String() = cell.State.GeneNames

        translationRate = New Double(genes.Length - 1) {}
        decayRate = New Double(genes.Length - 1) {}

        For i As Integer = 0 To genes.Length - 1
            translationRate(i) = blueprint.TranslationRateOf(genes(i))
            decayRate(i) = blueprint.ProteinDegradationOf(genes(i)) + blueprint.DilutionRate
        Next

        mrnaInput = New Double(genes.Length - 1) {}

        Call InitializeFrom(cell.State.Protein)
    End Sub

    Protected Overrides Sub RHS(t As Double, y As NVector, ydot As NVector)
        For i As Integer = 0 To n - 1
            ydot(i) = translationRate(i) * mrnaInput(i) - decayRate(i) * y(i)
        Next
    End Sub

    Protected Overrides Sub Jacobian(t As Double, y As NVector, fy As NVector, J As DenseMatrix)
        For i As Integer = 0 To n - 1
            For j As Integer = 0 To n - 1
                J(i, j) = 0.0
            Next

            J(i, i) = -decayRate(i)
        Next
    End Sub

    Public Overrides Sub Tick(dt As Double)
        Dim state As CellularState = cell.State

        ' 取本步的 mRNA 快照（算子分裂：步内常量）
        Call Array.Copy(state.mRNA, mrnaInput, n)

        ' 蛋白质由本系统独占，求解器内部状态即当前状态；
        ' 只在求解失败时回退到推进前的快照
        Dim rollback As Double() = CurrentState()

        If Not Advance(dt) Then
            Call ResetState(rollback)
            Return
        End If

        Dim result As Double() = CurrentState()

        For i As Integer = 0 To n - 1
            state.Protein(i) = If(result(i) < 0.0, 0.0, result(i))
        Next
    End Sub

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Dim protein As Double() = cell.State.Protein
        Dim sum As Double = 0.0
        Dim max As Double = 0.0

        For i As Integer = 0 To protein.Length - 1
            sum += protein(i)

            If protein(i) > max Then
                max = protein(i)
            End If
        Next

        Dim stats As Dictionary(Of String, Double) = MyBase.GetStats()

        stats("protein_total") = sum
        stats("protein_max") = max

        Return stats
    End Function

End Class
