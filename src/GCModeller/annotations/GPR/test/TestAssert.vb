Imports System.Text

''' <summary>
''' 单条断言的执行结果
''' </summary>
Public Class AssertionResult

    Public Property CaseName As String
    Public Property Name As String
    Public Property Passed As Boolean
    Public Property Expectation As String
    Public Property Actual As String
    Public Property Message As String

    Public Overrides Function ToString() As String
        Return $"[{If(Passed, "PASS", "FAIL")}] {CaseName} / {Name}"
    End Function

End Class

''' <summary>
''' 轻量断言与用例运行框架。
''' 
''' 遵循本仓库既有约定（WGCNA\test、P-NET\test 都是 Exe 控制台工程），
''' 不引入 xUnit / NUnit 等外部测试框架。
''' </summary>
Public Class TestRunner

    Private ReadOnly items As New List(Of AssertionResult)
    Private currentCase As String = "<no-case>"

    Public ReadOnly Property Assertions As List(Of AssertionResult)
        Get
            Return items
        End Get
    End Property

    Public ReadOnly Property TotalCount As Integer
        Get
            Return items.Count
        End Get
    End Property

    Public ReadOnly Property PassedCount As Integer
        Get
            Return items.Where(Function(a) a.Passed).Count()
        End Get
    End Property

    Public ReadOnly Property FailedCount As Integer
        Get
            Return items.Where(Function(a) Not a.Passed).Count()
        End Get
    End Property

    Public Sub BeginCase(name As String)
        currentCase = name
    End Sub

    Public Sub Section(title As String)
        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.WriteLine($"==== {title} ".PadRight(96, "="c))
        Console.ResetColor()
    End Sub

    Public Sub Ok(name As String, expectation As String, actual As String)
        items.Add(New AssertionResult With {
            .CaseName = currentCase,
            .Name = name,
            .Passed = True,
            .Expectation = expectation,
            .Actual = actual
        })
    End Sub

    Public Sub Fail(name As String, expectation As String, actual As String, Optional message As String = Nothing)
        items.Add(New AssertionResult With {
            .CaseName = currentCase,
            .Name = name,
            .Passed = False,
            .Expectation = expectation,
            .Actual = actual,
            .Message = message
        })
    End Sub

    Public Function AssertTrue(name As String, condition As Boolean, Optional detail As String = Nothing) As Boolean
        If condition Then
            Ok(name, "True", "True")
        Else
            Fail(name, "True", "False", detail)
        End If

        Return condition
    End Function

    Public Function AssertFalse(name As String, condition As Boolean, Optional detail As String = Nothing) As Boolean
        Return AssertTrue(name, Not condition, detail)
    End Function

    Public Function AssertEqual(Of T)(name As String, expected As T, actual As T) As Boolean
        Dim ok As Boolean = EqualityComparer(Of T).Default.Equals(expected, actual)

        If ok Then
            Ok(name, FormatValue(expected), FormatValue(actual))
        Else
            Fail(name, FormatValue(expected), FormatValue(actual))
        End If

        Return ok
    End Function

    ''' <summary>
    ''' 把断言值格式化为可读字符串
    ''' </summary>
    Public Shared Function FormatValue(value As Object) As String
        If value Is Nothing Then Return "<null>"

        Dim array_ As String() = TryCast(value, String())
        If array_ IsNot Nothing Then Return $"[{String.Join(", ", array_)}]"

        Dim objects As Object() = TryCast(value, Object())
        If objects IsNot Nothing Then
            Return $"[{String.Join(", ", objects.Select(Function(o) FormatValue(o)))}]"
        End If

        Return value.ToString()
    End Function

    Public Function AssertNear(name As String, expected As Double, actual As Double, Optional tolerance As Double = 0.000001) As Boolean
        Dim ok As Boolean = Math.Abs(expected - actual) <= tolerance

        If ok Then
            Ok(name, expected.ToString("F6"), actual.ToString("F6"))
        Else
            Fail(name, expected.ToString("F6"), actual.ToString("F6"), $"偏差 {Math.Abs(expected - actual).ToString("F6")} 超过容差 {tolerance}")
        End If

        Return ok
    End Function

    Public Function AssertInRange(name As String, value As Double, low As Double, high As Double) As Boolean
        Dim ok As Boolean = value >= low AndAlso value <= high

        If ok Then
            Ok(name, $"[{low}, {high}]", value.ToString("F6"))
        Else
            Fail(name, $"[{low}, {high}]", value.ToString("F6"))
        End If

        Return ok
    End Function

    ''' <summary>
    ''' 断言某个操作会抛出指定类型的异常。
    ''' 
    ''' 用于验证历史上"能编译、运行时必崩"的缺陷确实已经被修复：真正的通过标准是
    ''' 操作能够正常完成并返回结果，而不是抛出异常。
    ''' </summary>
    Public Function AssertNoThrow(Of TException As Exception)(name As String, action As Action) As Boolean
        Try
            action()
            Ok(name, "不抛出异常", "正常返回")
            Return True
        Catch ex As Exception
            Fail(name, "不抛出异常", ex.GetType().Name, ex.Message)
            Return False
        End Try
    End Function

    Public Function AssertContains(name As String, set As IEnumerable(Of String), item As String) As Boolean
        Dim contains As Boolean = set IsNot Nothing AndAlso set.Contains(item, StringComparer.OrdinalIgnoreCase)

        If contains Then
            Ok(name, $"包含 {item}", "包含")
        Else
            Fail(name, $"包含 {item}", $"实际集合 = [{String.Join(", ", If(set, New String() {}))}]")
        End If

        Return contains
    End Function

    Public Function AssertNotContains(name As String, set As IEnumerable(Of String), item As String) As Boolean
        Dim contains As Boolean = set IsNot Nothing AndAlso set.Contains(item, StringComparer.OrdinalIgnoreCase)

        If contains Then
            Fail(name, $"不包含 {item}", $"实际集合 = [{String.Join(", ", If(set, New String() {}))}]")
        Else
            Ok(name, $"不包含 {item}", "不包含")
        End If

        Return Not contains
    End Function

    ''' <summary>
    ''' 打印用例结果明细，返回失败数量
    ''' </summary>
    Public Function PrintResults() As Integer
        Dim lastCase As String = Nothing

        For Each item As AssertionResult In items
            If Not String.Equals(lastCase, item.CaseName, StringComparison.Ordinal) Then
                lastCase = item.CaseName
                Console.WriteLine()
                Console.ForegroundColor = ConsoleColor.DarkCyan
                Console.WriteLine($"  [{lastCase}]")
                Console.ResetColor()
            End If

            If item.Passed Then
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine($"    PASS  {item.Name}")
            Else
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine($"    FAIL  {item.Name}")
                Console.ResetColor()
                Console.WriteLine($"          期望: {item.Expectation}")
                Console.WriteLine($"          实际: {item.Actual}")
                If Not String.IsNullOrEmpty(item.Message) Then
                    Console.WriteLine($"          说明: {item.Message}")
                End If
                Console.ForegroundColor = ConsoleColor.Red
            End If

            Console.ResetColor()
        Next

        Return FailedCount
    End Function

End Class
