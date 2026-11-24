''' <summary>
''' 一次定点迭代求解的结果
''' </summary>
Public Class SolveResult

    ''' <summary>
    ''' 稳态表达矩阵：<c>steadyState(gene)(cell)</c>
    ''' </summary>
    Public Property SteadyState As Double()() = New Double()() {}

    ''' <summary>实际执行的迭代轮数</summary>
    Public Property Iterations As Integer

    ''' <summary>是否所有细胞都达到了收敛阈值</summary>
    Public Property Converged As Boolean

    ''' <summary>达到收敛阈值的细胞数量</summary>
    Public Property ConvergedCells As Integer

    ''' <summary>最后一轮的最大相对变化量</summary>
    Public Property MaxChange As Double

    ''' <summary>每一轮的最大相对变化量（用于诊断震荡）</summary>
    Public Property History As Double() = New Double() {}

    ''' <summary>参与迭代的细胞（初始状态）数量</summary>
    Public Property PopulationSize As Integer

    ''' <summary>
    ''' 是否把“所有细胞完全一致”的初始状态折叠成了单细胞轨迹
    ''' </summary>
    ''' <remarks>
    ''' 全基因组上下文会把种群的所有细胞设置成同一个状态，此时 1000 个细胞的迭代是完全重复的计算；
    ''' 折叠之后单次扰动的耗时可以从分钟级降到秒级，而均值/差异统计不受影响。
    ''' </remarks>
    Public Property Collapsed As Boolean

    ''' <summary>
    ''' 是否超过了软上限（<c>GeneTreeConfig.MaxIterations</c>）之后继续迭代
    ''' </summary>
    ''' <remarks>
    ''' 超过软上限说明软上限不足以让网络收敛，求解器会在硬上限
    ''' （<c>GeneTreeConfig.EffectiveMaxIterationsHardCap</c>）之内继续迭代。
    ''' </remarks>
    Public Property ExceededSoftLimit As Boolean

    ''' <summary>基因数量</summary>
    Public ReadOnly Property GeneCount As Integer
        Get
            Return SteadyState.Length
        End Get
    End Property

    Public Property log As String

    ''' <summary>
    ''' 稳态下每个基因在细胞种群上的均值
    ''' </summary>
    ''' <returns></returns>
    Public Function GetMeans() As Double()
        Dim result As Double() = New Double(SteadyState.Length - 1) {}

        For g As Integer = 0 To SteadyState.Length - 1
            Dim values As Double() = SteadyState(g)

            If values IsNot Nothing AndAlso values.Length > 0 Then
                Dim sum As Double = 0.0R

                For c As Integer = 0 To values.Length - 1
                    sum += values(c)
                Next

                result(g) = sum / values.Length
            End If
        Next

        Return result
    End Function

    ''' <summary>
    ''' 稳态下每个基因在细胞种群上的标准差（样本标准差）
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSDs() As Double()
        Dim means As Double() = GetMeans()
        Dim result As Double() = New Double(SteadyState.Length - 1) {}

        For g As Integer = 0 To SteadyState.Length - 1
            Dim values As Double() = SteadyState(g)

            If values IsNot Nothing AndAlso values.Length > 1 Then
                Dim sse As Double = 0.0R

                For c As Integer = 0 To values.Length - 1
                    sse += (values(c) - means(g)) ^ 2
                Next

                result(g) = Math.Sqrt(sse / (values.Length - 1))
            End If
        Next

        Return result
    End Function

    Public Overrides Function ToString() As String
        Return $"{GeneCount} genes x {PopulationSize} cells, {Iterations} iterations, " &
               $"converged={Converged} ({ConvergedCells}/{PopulationSize} cells), " &
               $"collapsed={Collapsed}, exceeded_soft_limit={ExceededSoftLimit}, " &
               $"max change={MaxChange.ToString("G4")}"
    End Function
End Class
