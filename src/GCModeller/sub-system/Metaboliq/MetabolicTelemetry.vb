' ============================================================
' MetabolicTelemetry.vb - 积分内核的零依赖计时钩子
' ============================================================
' 为什么需要：`Simulate` 一步就分配十几 GB，但外层的阶段计时只能看到
' "整个 Simulate" 一个桶。要把这十几 GB 定位到「构造输入 / LTC 前向 /
' 通量读出 / τ 读数 / 区间积分」中的某一段，必须在核心里埋更细的钩子。
'
' 约束：
'   * Metaboliq 是底层计算库，不能反向依赖 Cella 的 PhaseProfiler；
'     因此这里只暴露一个委托，由宿主（vCella）注入具体实现；
'   * 未注入时开销仅一次空引用判断；
'   * 钩子内部异常必须吞掉，绝不能因为"测量"影响"计算"。
' ============================================================

''' <summary>积分内核的分段计时钩子（由宿主注入）。</summary>
Public Module MetabolicTelemetry

    ''' <summary>
    ''' 分段计时实现：宿主一般注入 <c>PerformanceMonitor.Measure</c>。
    ''' </summary>
    Public Property Measure As Action(Of String, Action)

    ''' <summary>若已注入则计量执行，否则直接执行。</summary>
    ''' <param name="name">分段名，例如 <c>ltc.forward</c></param>
    ''' <param name="action">被计量的代码</param>
    Public Sub Timed(name As String, action As Action)
        Dim handler As Action(Of String, Action) = Measure

        If handler Is Nothing Then
            Call action()

            Return
        End If

        Try
            Call handler(name, action)
        Catch ex As Exception
            ' 计时失败不能影响计算：退化为直接执行
            Call action()
        End Try
    End Sub

End Module
