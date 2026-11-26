' ------------------------------------------------------------------------
' ILCudaTensor 融合核的审计编排
'
' 与 ILCuda/test 的 AuditRunner 同构：建立引擎、逐用例断言、
' 汇总误差并把 DebugAudit 的结构化报告写到磁盘。
'
' 注意与 ILCuda 的差异：张量层的 .cu 是通过
' KernelSources.Register(GetType(CudaTensor).Assembly) 注入的，
' 因此这里必须先注册再创建引擎。
' ------------------------------------------------------------------------

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Computing.ILCuda.Runtime
Imports Microsoft.VisualBasic.Computing.ILCuda.Kernels
Imports Microsoft.VisualBasic.Computing.ILCuda.GPUTensor

Namespace Audit

    ''' <summary>审计的运行时上下文与结果汇总</summary>
    Public Module AuditRunner

        ''' <summary>可用的 CUDA 引擎；<c>Nothing</c> 表示本次审计全部走 CPU</summary>
        Public Property Engine As CudaEngine

        ''' <summary>本次审计是否真的跑在 GPU 上</summary>
        Public Property OnGpu As Boolean = False

        ''' <summary>报告输出目录</summary>
        Public Property ReportDir As String = "audit"

        ''' <summary>引擎不可用的原因</summary>
        Public Property EngineNote As String = "CPU only"

        Private m_cases As Integer
        Private m_failed As Integer

        Public ReadOnly Property CaseCount As Integer
            Get
                Return m_cases
            End Get
        End Property

        Public ReadOnly Property FailureCount As Integer
            Get
                Return m_failed
            End Get
        End Property

        ''' <summary>建立审计上下文；任何失败都只记录原因并让后续用例走 CPU。</summary>
        ''' <remarks>
        ''' 参数不能叫 reportDir —— VB 大小写不敏感，它会遮蔽同名的属性 ReportDir，
        ''' 结果变成"参数赋给自己"，--out 永远不生效。
        ''' </remarks>
        Public Sub Setup(cpuOnly As Boolean, deviceOrdinal As Integer, outputDirectory As String)
            Call DebugAudit.Reset()

            m_cases = 0
            m_failed = 0
            OnGpu = False

            If Not String.IsNullOrWhiteSpace(outputDirectory) Then
                ReportDir = outputDirectory
            End If

            If cpuOnly Then
                EngineNote = "命令指定 --cpu-only"

                Return
            End If

            Try
                ' 张量层的 .cu 必须先注册，否则不会进入 NVRTC 编译单元
                Call KernelSources.Register(GetType(CudaTensor).Assembly)

                Dim options As New EngineOptions With {.DeviceOrdinal = deviceOrdinal}

                Engine = CudaEngine.TryCreate(options)

                If Engine Is Nothing Then
                    ' 编译失败时把完整的诊断日志打到控制台，便于定位内核语法问题
                    For Each line As String In options.Diagnostics
                        Call Console.WriteLine($"  [DIAG] {line}")
                    Next

                    ' 把合并后的编译单元落盘，便于用 nvcc 复现编译错误
                    Try
                        Dim combined As String = KernelSources.CombinedSource()

                        Call Directory.CreateDirectory(ReportDir)
                        Call File.WriteAllText(Path.Combine(ReportDir, "combined.cu"), combined)
                        Call Console.WriteLine($"  [DIAG] 合并源码已导出: {Path.Combine(ReportDir, "combined.cu")}")
                    Catch ex As Exception
                        Call Console.WriteLine($"  [DIAG] 导出合并源码失败: {ex.Message}")
                    End Try

                    Dim detail As String = String.Join(" | ", options.Diagnostics)

                    If detail.Length > 1200 Then detail = detail.Substring(detail.Length - 1200)

                    EngineNote = If(String.IsNullOrEmpty(options.ErrorMessage),
                                    "没有可用的 CUDA 引擎",
                                    options.ErrorMessage) & " :: " & detail
                Else
                    OnGpu = True
                    EngineNote = Engine.Image.ToString()
                End If
            Catch ex As Exception
                Engine = Nothing
                EngineNote = $"创建引擎时抛出异常: {ex.Message}"
            End Try
        End Sub

        ''' <summary>记录一个用例的结果（Case 是 VB 保留字，这里用 Record）。</summary>
        Public Sub Record(name As String, ok As Boolean, Optional detail As String = Nothing)
            m_cases += 1

            If Not ok Then m_failed += 1

            Dim message As String = If(String.IsNullOrEmpty(detail), name, $"{name} ({detail})")

            Call DebugAudit.Test(ok, message)
            Call Console.WriteLine($"  [{(If(ok, "PASS", "FAIL"))}] {message}")
        End Sub

        ''' <summary>记录一条说明性标记。</summary>
        Public Sub Note(name As String, Optional value As Object = Nothing)
            Call DebugAudit.Mark(name, value)
            Call Console.WriteLine($"  [INFO] {If(value Is Nothing, name, $"{name} = {value}")}")
        End Sub

        Public Sub Capture(name As String, value As Object)
            Call DebugAudit.Capture(name, value)
        End Sub

        ''' <summary>写出报告并返回失败用例数。</summary>
        Public Function Flush() As Integer
            Dim jsonPath As String = Path.Combine(ReportDir, "ilcudatensor-audit.json")
            Dim textPath As String = Path.Combine(ReportDir, "ilcudatensor-audit.md")

            Call Capture("engine", EngineNote)
            Call Capture("onGpu", OnGpu)
            Call Capture("cases", m_cases)
            Call Capture("failures", m_failed)

            Call DebugAudit.SaveReport(jsonPath)

            Try
                Dim text As String =
                    $"# ILCudaTensor 审计报告{Environment.NewLine}{Environment.NewLine}" &
                    $"- 引擎: {EngineNote}{Environment.NewLine}" &
                    $"- 走 GPU: {OnGpu}{Environment.NewLine}" &
                    $"- 用例数: {m_cases}{Environment.NewLine}" &
                    $"- 失败数: {m_failed}{Environment.NewLine}{Environment.NewLine}" &
                    "```" & Environment.NewLine & DebugAudit.Dump() & Environment.NewLine & "```" & Environment.NewLine

                Call Directory.CreateDirectory(ReportDir)
                Call File.WriteAllText(textPath, text)
            Catch ex As Exception
                Call Console.WriteLine($"  报告写盘失败: {ex.Message}")
            End Try

            Console.WriteLine()
            Console.WriteLine($"  引擎: {EngineNote}")
            Console.WriteLine($"  走 GPU: {OnGpu}")
            Console.WriteLine($"  用例: {m_cases}，失败: {m_failed}")
            Console.WriteLine($"  报告: {jsonPath}")
            Console.WriteLine($"        {textPath}")

            If Engine IsNot Nothing Then
                Try
                    Engine.Dispose()
                Catch
                End Try

                Engine = Nothing
            End If

            Return m_failed
        End Function

        ' ------------------------------------------------------------------
        ' 误差工具
        ' ------------------------------------------------------------------

        Public Function MaxAbsError(actual As Double(), expected As Double()) As Double
            If actual Is Nothing OrElse expected Is Nothing Then Return Double.PositiveInfinity
            If actual.Length <> expected.Length Then Return Double.PositiveInfinity

            Dim worst As Double = 0.0

            For i As Integer = 0 To actual.Length - 1
                If Double.IsNaN(actual(i)) AndAlso Double.IsNaN(expected(i)) Then Continue For

                Dim d As Double = System.Math.Abs(actual(i) - expected(i))

                If d > worst Then worst = d
            Next

            Return worst
        End Function

        Public Function MaxRelError(actual As Double(), expected As Double()) As Double
            If actual Is Nothing OrElse expected Is Nothing Then Return Double.PositiveInfinity
            If actual.Length <> expected.Length Then Return Double.PositiveInfinity

            Dim worst As Double = 0.0

            For i As Integer = 0 To actual.Length - 1
                Dim scale As Double = System.Math.Max(System.Math.Abs(expected(i)), 1.0E-12)
                Dim d As Double = System.Math.Abs(actual(i) - expected(i)) / scale

                If d > worst Then worst = d
            Next

            Return worst
        End Function

        Public Function SameIntegers(actual As Integer(), expected As Integer()) As Boolean
            If actual Is Nothing OrElse expected Is Nothing Then Return False
            If actual.Length <> expected.Length Then Return False

            For i As Integer = 0 To actual.Length - 1
                If actual(i) <> expected(i) Then Return False
            Next

            Return True
        End Function
    End Module
End Namespace
