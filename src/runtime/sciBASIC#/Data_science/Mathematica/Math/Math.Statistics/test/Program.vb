Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging

''' <summary>
''' 统计组件的验收测试入口：逐个 Audit 顺序执行，汇总失败数，并把
''' <see cref="DebugAudit"/> 的审计记录写成 JSON 报告。
''' </summary>
Module Program

        Friend Const ReportFile As String = "math-statistics-audit-report.json"

        Sub Main(args As String())
            DebugAudit.Reset()
            DebugAudit.MaxEvents = 8192
            DebugAudit.ThrowOnFailure = False

            Dim failures As Integer = 0

            failures += NormalAudit.Run()
            failures += RandomSourceAudit.Run()
            failures += CsrAudit.Run()
            failures += OrderStatisticsAudit.Run()
            failures += SingleCellAudit.Run()
            failures += WilcoxonAudit.Run()
            failures += MetaAnalysisAudit.Run()
            failures += ApportionCopulaAudit.Run()

            Dim report As String = Path.Combine(AppContext.BaseDirectory, ReportFile)

            Call DebugAudit.SaveReport(report)

            Console.WriteLine("=========================================================")
            Console.WriteLine($"failures: {failures}")
            Console.WriteLine($"report  : {report}")
            Console.WriteLine($"audit   : {If(failures = 0, "PASS", "FAIL")}")
            Console.WriteLine("=========================================================")

            Environment.ExitCode = If(failures = 0, 0, 1)
        End Sub

        ''' <summary>
        ''' 断言一个标量落在容差内；返回失败计数（0 或 1）
        ''' </summary>
        ''' <param name="name">断言名称</param>
        ''' <param name="actual">实际值</param>
        ''' <param name="expected">期望值</param>
        ''' <param name="tolerance">绝对容差</param>
        ''' <param name="section">所属测试分区</param>
        ''' <returns></returns>
        Friend Function AssertClose(name As String, actual As Double, expected As Double, tolerance As Double, section As String) As Integer
            Dim difference As Double = System.Math.Abs(actual - expected)
            Dim ok As Boolean = Not Double.IsNaN(difference) AndAlso difference <= tolerance

            Call DebugAudit.Check(ok, $"assertion failed: [{section}] {name}: expected {expected:G17}, actual {actual:G17}, |diff| {difference:G6} > {tolerance:G6}")

            Return If(ok, 0, 1)
        End Function

        ''' <summary>
        ''' 断言一个布尔条件；返回失败计数（0 或 1）
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="condition"></param>
        ''' <param name="detail"></param>
        ''' <returns></returns>
        Friend Function Assert(name As String, condition As Boolean, Optional detail As String = Nothing) As Integer
            Call DebugAudit.Check(condition, $"assertion failed: {name}{If(String.IsNullOrEmpty(detail), "", " -> " & detail)}")

            Return If(condition, 0, 1)
        End Function

End Module
