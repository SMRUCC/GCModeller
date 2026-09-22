Imports System.IO
Imports System.Text
Imports SMRUCC.genomics.GCModeller.CompilerServices.GPRLink

''' <summary>
''' GPR 关联算法的 Demo 测试入口。
''' 
''' 运行流程：
''' 
''' 1. 打印算法与参数说明；
''' 2. 依次运行全部测试用例（合成数据 + 内置断言）；
''' 3. 打印通过 / 失败明细；
''' 4. 输出"基因 - 代谢反应 - 打分"结果表、基因级汇总与分数分布；
''' 5. 把完整明细表导出为 CSV。
''' 
''' 进程退出码：0 表示全部断言通过，1 表示存在失败，2 表示运行时崩溃。
''' </summary>
Module Program

    Private Const CsvRelativePath As String = "debug\gene_reaction_score.csv"

    Sub Main(args As String())
        Try
            Console.OutputEncoding = Encoding.UTF8
        Catch
            ' 某些终端不支持修改输出编码，忽略即可
        End Try

        ' 默认关闭 sciBASIC# 运行库的调试输出，保持 Demo 输出整洁；
        ' 需要查看算法内部的阶段日志时传入 --verbose。
        If args Is Nothing OrElse Not args.Any(Function(a) String.Equals(a, "--verbose", StringComparison.OrdinalIgnoreCase)) Then
            Microsoft.VisualBasic.VBDebugger.Mute = True
        End If

        RunDemo()
    End Sub

    Private Sub RunDemo()
        PrintBanner()

        Dim runner As New TestRunner
        Dim demo As (Associator As MetabolicAssociator, Data As SyntheticCase)

        Try
            demo = GPRTestCases.RunAll(runner)
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine()
            Console.WriteLine("!! Demo 运行过程中发生未捕获的异常，这本身就是算法存在缺陷的信号：")
            Console.WriteLine($"   {ex.GetType().Name}: {ex.Message}")
            Console.WriteLine(ex.StackTrace)
            Console.ResetColor()

            Environment.ExitCode = 2
            Return
        End Try

        runner.Section("测试用例执行结果")

        Dim failed As Integer = runner.PrintResults()

        PrintReport(demo)

        runner.Section("汇总")
        PrintSummary(runner, failed)

        Environment.ExitCode = If(failed = 0, 0, 1)
    End Sub

    Private Sub PrintBanner()
        Dim opt As New GPRParameters

        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine("================================================================================")
        Console.WriteLine(" GCModeller :: 基因 - 代谢反应关联算法 (GPR Link) Demo 测试")
        Console.WriteLine("================================================================================")
        Console.ResetColor()
        Console.WriteLine()
        Console.WriteLine("被测模块 : annotations\GPR\GPR.vbproj")
        Console.WriteLine("测试数据 : 完全内置的合成基因组与参考通路（离线、可复现、自带正确答案）")
        Console.WriteLine()
        Console.WriteLine("打分模型 : 证据可枚举 -> 同类归并 -> 跨类 noisy-OR 聚合 -> 置信度过滤")
        Console.WriteLine("            score = 1 - PI_kind (1 - weight_kind * raw_kind)")
        Console.WriteLine()
        Console.WriteLine("算法参数 :")
        Console.WriteLine($"    DirectMatchScore                = {opt.DirectMatchScore}")
        Console.WriteLine($"    BaseContextScore (窗口上下文)      = {opt.BaseContextScore}")
        Console.WriteLine($"    SameOperonBonus (操纵子奖励)       = {opt.SameOperonBonus}")
        Console.WriteLine($"    BaseComplexScore (酶复合体)        = {opt.BaseComplexScore}")
        Console.WriteLine($"    BaseCoexpressionScore (共表达)     = {opt.BaseCoexpressionScore}")
        Console.WriteLine($"    BaseSyntenyScore (保守共线性)       = {opt.BaseSyntenyScore}")
        Console.WriteLine($"    PathwayCompletenessWeight         = {opt.PathwayCompletenessWeight}")
        Console.WriteLine($"    ReactionContinuityWeight          = {opt.ReactionContinuityWeight}")
        Console.WriteLine($"    CorroborationGain (同类旁证增益)    = {opt.CorroborationGain}")
        Console.WriteLine($"    ConfidenceThreshold (输出阈值)     = {opt.ConfidenceThreshold}")
        Console.WriteLine($"    MaxPhysicalDistance               = {opt.MaxPhysicalDistance}")
        Console.WriteLine($"    MaxWindowSpan                     = {opt.MaxWindowSpan}")
    End Sub

    Private Sub PrintReport(demo As (Associator As MetabolicAssociator, Data As SyntheticCase))
        If demo.Associator Is Nothing Then Return

        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine("================================================================================")
        Console.WriteLine(" 综合演示：基因 - 代谢反应关联结果")
        Console.WriteLine("================================================================================")
        Console.ResetColor()
        Console.WriteLine()
        Console.WriteLine($"合成数据   : {demo.Data.Description}")
        Console.WriteLine($"基因组     : {demo.Data.Genes.Length} 个基因")
        Console.WriteLine($"参考网络   : {demo.Data.Pathways.Length} 条通路 / {demo.Data.ReactionIds().Length} 个反应")
        Console.WriteLine($"潜在操纵子 : {demo.Associator.Operons.Count} 个")
        Console.WriteLine($"潜在复合体 : {demo.Associator.Complexes.Count} 个")

        Dim report As New GeneReactionReport
        Call report.Load(demo.Associator, "gene-reaction association", demo.Data.Description)

        Call report.PrintDetail(maxRows:=80)
        Call report.PrintGeneSummary(demo.Associator)
        Call report.PrintScoreDistribution()

        Dim csvPath As String = Path.Combine(AppContext.BaseDirectory, CsvRelativePath)

        Try
            Call report.ExportCsv(csvPath)
            Console.WriteLine()
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine($"关联结果已导出为 CSV: {csvPath}")
            Console.ResetColor()
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine()
            Console.WriteLine($"CSV 导出失败: {ex.Message}")
            Console.ResetColor()
        End Try
    End Sub

    Private Sub PrintSummary(runner As TestRunner, failed As Integer)
        Console.WriteLine()
        Console.WriteLine($"断言总数 : {runner.TotalCount}")
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine($"通过     : {runner.PassedCount}")
        Console.ResetColor()

        If failed = 0 Then
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("失败     : 0")
            Console.WriteLine()
            Console.WriteLine("全部断言通过。")
        Else
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine($"失败     : {failed}")
            Console.WriteLine()
            Console.WriteLine("存在未通过的断言，请检查上方 FAIL 明细。")
        End If

        Console.ResetColor()
        Console.WriteLine()
    End Sub

End Module
