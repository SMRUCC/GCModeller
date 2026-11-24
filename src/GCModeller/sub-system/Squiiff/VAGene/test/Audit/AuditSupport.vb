' ============================================================================
' AuditSupport.vb - VAGene 生产化审计的公共支撑设施
'
' 提供：报告路径约定、审计分组运行器、数值断言辅助。
'
' 设计要点：
'   - 任何未捕获异常都会被折叠成一条审计失败记录，不会中止整个审计流程；
'   - 只使用 DebugAudit 中非 <Conditional("DEBUG")> 的 API（Test/Mark/Reset/
'     Watch/Dump/SaveReport），保证 Release 配置下审计逻辑不会被编译器移除。
' ============================================================================

Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>VAGene 生产化审计的公共支撑设施。</summary>
Public Module AuditSupport

    ''' <summary>审计报告文件名（写入 <see cref="AppContext.BaseDirectory"/>）。</summary>
    Public Const ReportName As String = "vagene-audit-report.json"

    ''' <summary>审计事件明细的纯文本转储文件名。</summary>
    Public Const DumpName As String = "vagene-audit-dump.txt"

    ''' <summary>审计报告的绝对路径。</summary>
    Public Function ReportPath() As String
        Return IO.Path.Combine(AppContext.BaseDirectory, ReportName)
    End Function

    ''' <summary>清空审计状态并写入起始标记。</summary>
    Public Sub Begin()
        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 65536
        Call DebugAudit.Mark("vagene production audit started")
        Console.WriteLine()
        Console.WriteLine(New String("="c, 78))
        Console.WriteLine("  VAGene 虚拟扰动实验审计")
        Console.WriteLine(New String("="c, 78))
    End Sub

    ''' <summary>运行一个审计分组。异常被折叠为失败记录，而不是中断后续分组。</summary>
    Public Sub Group(name As String, action As Action)
        Console.WriteLine()
        Console.WriteLine($"--- [audit] {name} ---")
        Call DebugAudit.Mark($"audit group begin: {name}")

        Using DebugAudit.Watch($"audit:{name}", logOnExit:=True)
            Try
                Call action()
            Catch ex As Exception
                Call DebugAudit.Test(False, $"audit group '{name}' aborted by {ex.GetType().Name}: {ex.Message}")
            End Try
        End Using

        Call DebugAudit.Mark($"audit group end: {name}")
    End Sub

    ''' <summary>运行单个审计用例。异常被折叠为一条失败记录。</summary>
    Public Sub RunCase(name As String, action As Action)
        Try
            Call action()
        Catch ex As Exception
            Call DebugAudit.Test(False,
                                 $"audit case '{name}' aborted by {ex.GetType().Name}: {ex.Message}")
        End Try
    End Sub

    ''' <summary>写出审计报告并打印摘要，返回失败项数量。</summary>
    Public Function Finish() As Integer
        Dim path As String = ReportPath()
        Dim dump As String = DebugAudit.Dump()

        Try
            IO.File.WriteAllText(IO.Path.Combine(AppContext.BaseDirectory, DumpName), dump)
        Catch ex As Exception
            ' 明细转储失败不影响主报告
        End Try

        Try
            Call DebugAudit.SaveReport(path)
        Catch ex As Exception
            Console.WriteLine($"   [audit] 无法写入报告: {ex.Message}")
        End Try

        Console.WriteLine()
        Console.WriteLine(New String("="c, 78))
        Console.WriteLine($" 审计结束: {DebugAudit.FailureCount} 项失败")
        Console.WriteLine($"   报告 -> {path}")
        Console.WriteLine(New String("="c, 78))

        Return DebugAudit.FailureCount
    End Function

#Region "数值断言辅助"

    ''' <summary>判断标量是否为有限值（非 NaN、非 ±Infinity）。</summary>
    Public Function IsFinite(value As Double) As Boolean
        Return Not (Double.IsNaN(value) OrElse Double.IsInfinity(value))
    End Function

    ''' <summary>判断张量的全部元素是否为有限值。</summary>
    Public Function AllFinite(t As Tensor) As Boolean
        If t Is Nothing Then
            Return False
        End If

        Dim data As Double() = t.Data

        For i As Integer = 0 To data.Length - 1
            If Double.IsNaN(data(i)) OrElse Double.IsInfinity(data(i)) Then
                Return False
            End If
        Next

        Return True
    End Function

    ''' <summary>判断一维数组的全部元素是否为有限值。</summary>
    Public Function AllFinite(values As IEnumerable(Of Double)) As Boolean
        If values Is Nothing Then
            Return False
        End If

        For Each value As Double In values
            If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                Return False
            End If
        Next

        Return True
    End Function

    ''' <summary>两个张量之间的最大绝对逐元素误差。</summary>
    Public Function MaxAbsDiff(a As Tensor, b As Tensor) As Double
        Dim n As Integer = std.Min(a.Length, b.Length)
        Dim maxValue As Double = 0

        For i As Integer = 0 To n - 1
            Dim value As Double = std.Abs(a.Data(i) - b.Data(i))

            If value > maxValue Then
                maxValue = value
            End If
        Next

        Return maxValue
    End Function

    ''' <summary>张量整体求和（避免依赖后端 <c>TotalSum</c> 的单精度累积）。</summary>
    Public Function SumOf(t As Tensor) As Double
        Dim data As Double() = t.Data
        Dim acc As Double = 0

        For i As Integer = 0 To data.Length - 1
            acc += data(i)
        Next

        Return acc
    End Function

    ''' <summary>标准正态随机数（Box-Muller）。</summary>
    Public Function Randn(rng As Random) As Double
        Dim u1 As Double = 1.0 - rng.NextDouble()
        Dim u2 As Double = 1.0 - rng.NextDouble()

        Return std.Sqrt(-2.0 * std.Log(u1)) * std.Cos(2.0 * std.PI * u2)
    End Function

#End Region

End Module
