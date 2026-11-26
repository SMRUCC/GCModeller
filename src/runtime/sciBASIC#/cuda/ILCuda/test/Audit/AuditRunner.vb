' ------------------------------------------------------------------------
' ILCuda 审计编排
'
' 统一负责：建立引擎（失败时说明原因并退回 CPU）、逐用例断言、
' 汇总误差统计、把 DebugAudit 的结构化报告写到磁盘。
'
' 每个用例都是"待测路径（GPU 可用则走 GPU，否则走 CPU 回退）
' vs 独立参考实现"的对比，因此在无显卡的环境里依然能验证 CPU 参考路径，
' 有显卡时同一套用例直接校验内核。
' ------------------------------------------------------------------------

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Computing.ILCuda.Runtime

Namespace Audit

    ''' <summary>审计的运行时上下文与结果汇总</summary>
    Public Module AuditRunner

        ''' <summary>可用的 CUDA 引擎；<c>Nothing</c> 表示本次审计全部走 CPU</summary>
        Public Property Engine As CudaEngine

        ''' <summary>本次审计是否真的跑在 GPU 上</summary>
        Public Property OnGpu As Boolean = False

        ''' <summary>报告输出目录</summary>
        Public Property ReportDir As String = "audit"

        ''' <summary>引擎不可用（或强制 CPU）的原因，写进报告便于排查</summary>
        Public Property EngineNote As String = "CPU only"

        Private m_cases As Integer
        Private m_failed As Integer

        ''' <summary>已执行的用例数</summary>
        Public ReadOnly Property CaseCount As Integer
            Get
                Return m_cases
            End Get
        End Property

        ''' <summary>失败的用例数</summary>
        Public ReadOnly Property FailureCount As Integer
            Get
                Return m_failed
            End Get
        End Property

        ''' <summary>
        ''' 建立审计上下文。任何一步失败都不抛异常，只记录下来并让后续用例走 CPU。
        ''' </summary>
        Public Sub Setup(cpuOnly As Boolean,
                         deviceOrdinal As Integer,
                         nvrtcPath As String,
                         imagePath As String,
                         forceImage As Boolean,
                         outputDirectory As String)

            Call DebugAudit.Reset()

            m_cases = 0
            m_failed = 0
            OnGpu = False

            ' 参数不能叫 reportDir —— VB 大小写不敏感，会遮蔽同名属性 ReportDir
            If Not String.IsNullOrWhiteSpace(outputDirectory) Then
                ReportDir = outputDirectory
            End If

            If cpuOnly Then
                EngineNote = "命令指定 --cpu-only"

                Return
            End If

            Dim options As New EngineOptions With {
                .DeviceOrdinal = deviceOrdinal,
                .NvrtcPath = nvrtcPath,
                .ImagePath = imagePath,
                .ForceImage = forceImage
            }

            Try
                Engine = CudaEngine.TryCreate(options)
            Catch ex As Exception
                Engine = Nothing
                EngineNote = $"创建引擎时抛出异常: {ex.Message}"
            End Try

            If Engine Is Nothing Then
                EngineNote = If(String.IsNullOrEmpty(options.ErrorMessage),
                                "没有可用的 CUDA 引擎",
                                options.ErrorMessage)

                Return
            End If

            OnGpu = True
            EngineNote = Engine.Image.ToString()
        End Sub

        ''' <summary>记录一个用例的结果（Case 是 VB 保留字，这里用 Record）。</summary>
        Public Sub Record(name As String, ok As Boolean, Optional detail As String = Nothing)
            m_cases += 1

            If Not ok Then m_failed += 1

            Dim message As String = If(String.IsNullOrEmpty(detail), name, $"{name} ({detail})")

            Call DebugAudit.Test(ok, message)
            Call Console.WriteLine($"  [{(If(ok, "PASS", "FAIL"))}] {message}")
        End Sub

        ''' <summary>记录一条说明性标记（不参与通过/失败统计）。</summary>
        Public Sub Note(name As String, Optional value As Object = Nothing)
            Call DebugAudit.Mark(name, value)
            Call Console.WriteLine($"  [INFO] {If(value Is Nothing, name, $"{name} = {value}")}")
        End Sub

        ''' <summary>把捕获的指标登记为快照（随报告一起导出）。</summary>
        Public Sub Capture(name As String, value As Object)
            Call DebugAudit.Capture(name, value)
        End Sub

        ''' <summary>
        ''' 写出报告（JSON + 文本）并打印汇总，返回失败用例数。
        ''' </summary>
        Public Function Flush() As Integer
            Dim jsonPath As String = Path.Combine(ReportDir, "ilcuda-audit.json")
            Dim textPath As String = Path.Combine(ReportDir, "ilcuda-audit.md")

            Call Capture("engine", EngineNote)
            Call Capture("onGpu", OnGpu)
            Call Capture("cases", m_cases)
            Call Capture("failures", m_failed)

            Call DebugAudit.SaveReport(jsonPath)

            Try
                Dim text As String =
                    $"# ILCuda 审计报告{Environment.NewLine}{Environment.NewLine}" &
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

        ''' <summary>逐元素的绝对误差最大值；长度不一致时返回 <see cref="Double.PositiveInfinity"/>。</summary>
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

        ''' <summary>逐元素的绝对误差最大值（单精度）。</summary>
        Public Function MaxAbsError(actual As Single(), expected As Single()) As Double
            If actual Is Nothing OrElse expected Is Nothing Then Return Double.PositiveInfinity
            If actual.Length <> expected.Length Then Return Double.PositiveInfinity

            Dim worst As Double = 0.0

            For i As Integer = 0 To actual.Length - 1
                If Single.IsNaN(actual(i)) AndAlso Single.IsNaN(expected(i)) Then Continue For

                Dim d As Double = System.Math.Abs(CDbl(actual(i)) - CDbl(expected(i)))

                If d > worst Then worst = d
            Next

            Return worst
        End Function

        ''' <summary>逐元素的相对误差最大值（以参考值的绝对值为分母，含 1e-12 的底）。</summary>
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

        ''' <summary>整数数组是否逐元素相等。</summary>
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
