' ============================================================================
' TestAssert.vb — 自检断言原语（零第三方依赖，仅 .NET BCL）
' ----------------------------------------------------------------------------
' 提供 Check / CheckEqual / CheckNear / CheckThrows / CheckNoThrow / Guard，
' 统一收集失败项并打印「期望 / 实际 / 容差」，供 SelfTest 编排器汇总退出码。
'
' Guard(name, body) 用于包裹整组用例：单组崩溃不会中断整轮自检，
' 这样即使被测实现抛出异常（例如修复前 InitFromSeed 的 IndexOutOfRange），
' 仍然能拿到完整的「红灯清单」而不是一处崩溃掩盖其余全部用例。
' ============================================================================

Option Strict On

Namespace EmMotif

    Public Module TestAssert

        Private ReadOnly _failures As New List(Of String)()
        Private _checks As Integer = 0

        Public Sub Reset()
            _failures.Clear()
            _checks = 0
        End Sub

        Public ReadOnly Property CheckCount As Integer
            Get
                Return _checks
            End Get
        End Property

        Public ReadOnly Property FailureCount As Integer
            Get
                Return _failures.Count
            End Get
        End Property

        Public ReadOnly Property FailureList As IReadOnlyList(Of String)
            Get
                Return _failures
            End Get
        End Property

        Public Sub Section(title As String)
            Console.WriteLine()
            Console.WriteLine($"-- {title} --")
        End Sub

        Public Sub Note(text As String)
            Console.WriteLine($"         {text}")
        End Sub

        Private Sub Pass(name As String)
            _checks += 1
            Console.WriteLine($"  [PASS] {name}")
        End Sub

        Private Sub Fail(name As String, detail As String)
            _checks += 1
            Dim line = $"{name} — {detail}"
            _failures.Add(line)
            Console.WriteLine($"  [FAIL] {line}")
        End Sub

        ''' <summary>布尔断言</summary>
        Public Sub Check(cond As Boolean, name As String)
            If cond Then
                Pass(name)
            Else
                Fail(name, "期望 True，实际 False")
            End If
        End Sub

        ''' <summary>相等断言（泛型，走 EqualityComparer 默认比较器）</summary>
        Public Sub CheckEqual(Of T)(actual As T, expected As T, name As String)
            If EqualityComparer(Of T).Default.Equals(actual, expected) Then
                Pass(name)
            Else
                Fail(name, $"期望 <{expected}>，实际 <{actual}>")
            End If
        End Sub

        ''' <summary>浮点近似断言（NaN / Infinity 一律判失败，避免「假通过」）</summary>
        Public Sub CheckNear(actual As Double, expected As Double, tol As Double, name As String)
            If Double.IsNaN(actual) OrElse Double.IsNaN(expected) Then
                Fail(name, $"出现 NaN（实际 {actual}，期望 {expected}）")
            ElseIf Double.IsInfinity(actual) OrElse Double.IsInfinity(expected) Then
                Fail(name, $"出现 Infinity（实际 {actual}，期望 {expected}）")
            ElseIf Math.Abs(actual - expected) <= tol Then
                Pass(name)
            Else
                Fail(name, $"期望 {expected:G12}，实际 {actual:G12}，|Δ|={Math.Abs(actual - expected):G4} > 容差 {tol:G4}")
            End If
        End Sub

        ''' <summary>序列（数组）逐元素近似断言，长度不符直接失败</summary>
        Public Sub CheckNearAll(actual As Double(), expected As Double(), tol As Double, name As String)
            If actual Is Nothing OrElse expected Is Nothing Then
                Fail(name, "数组为 Nothing")
                Return
            End If
            If actual.Length <> expected.Length Then
                Fail(name, $"长度不符：实际 {actual.Length}，期望 {expected.Length}")
                Return
            End If
            Dim worst As Double = 0
            Dim worstAt As Integer = -1
            For i = 0 To actual.Length - 1
                Dim d = Math.Abs(actual(i) - expected(i))
                If Double.IsNaN(d) Then
                    Fail(name, $"第 {i} 个元素为 NaN")
                    Return
                End If
                If d > worst Then
                    worst = d
                    worstAt = i
                End If
            Next
            If worst <= tol Then
                Pass(name)
            Else
                Fail(name, $"最大偏差 {worst:G4} 出现在下标 {worstAt}（实际 {actual(worstAt):G12}，期望 {expected(worstAt):G12}），容差 {tol:G4}")
            End If
        End Sub

        ''' <summary>按被测实现的 clamp 语义夹逼数值，供期望值计算使用</summary>
        Public Function ClampLike(v As Double, lo As Double, hi As Double) As Double
            If v < lo Then Return lo
            If v > hi Then Return hi
            Return v
        End Function

        ''' <summary>断言 action 不抛异常；抛出则计入失败并打印异常类型与消息</summary>
        Public Sub CheckNoThrow(action As Action, name As String)
            Try
                action()
                Pass(name)
            Catch ex As Exception
                Fail(name, $"不应抛出异常，实际抛出 {ex.GetType().Name}: {ex.Message}")
            End Try
        End Sub

        ''' <summary>断言 action 抛出指定类型（或其派生类型）的异常</summary>
        Public Sub CheckThrows(Of TEx As Exception)(action As Action, name As String)
            Dim expectedType As Type = GetType(TEx)
            Try
                action()
                Fail(name, $"期望抛出 {expectedType.Name}，但未抛出异常")
            Catch ex As Exception
                If expectedType.IsInstanceOfType(ex) Then
                    Pass(name)
                Else
                    Fail(name, $"期望抛出 {expectedType.Name}，实际抛出 {ex.GetType().Name}: {ex.Message}")
                End If
            End Try
        End Sub

        ''' <summary>
        ''' 包裹一组用例：组内的未捕获异常记为一条失败，不中断整轮自检。
        ''' 这是「红灯优先」流程的基础——被测实现崩溃时仍能看到全部失败点。
        ''' </summary>
        Public Sub Guard(name As String, body As Action)
            Try
                body()
            Catch ex As Exception
                Fail(name, $"用例组异常终止：{ex.GetType().Name}: {ex.Message}")
            End Try
        End Sub

    End Module

End Namespace
