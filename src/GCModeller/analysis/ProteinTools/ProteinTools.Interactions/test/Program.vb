Imports Microsoft.VisualBasic.ApplicationServices.Debugging

Namespace test

    ''' <summary>
    ''' 数值对照测试的控制台入口。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 本工程不依赖任何测试框架，而是直接用
    ''' <see cref="Microsoft.VisualBasic.ApplicationServices.Debugging.DebugAudit"/>
    ''' 记录断言、计时与变量快照，最后输出一份 JSON 测试报告。
    ''' 这样可以保证在没有任何外部依赖的前提下，测试结果仍然是可归档、可比较的。
    ''' </para>
    ''' <para>
    ''' 全部对照基准都是<b>解析解、手算值或者自洽的暴力枚举</b>：
    ''' 解析解用于验证 Gamma 函数与矩阵行列式；暴力枚举用于验证变量消除、团树、
    ''' 最大生成树以及 Gibbs 采样。这样即使本机没有任何 Python / R 环境，
    ''' 也能完成严格的数值校验。
    ''' </para>
    ''' </remarks>
    Module Program

        Sub Main(args As String())
            Call DebugAudit.Reset()

            Console.OutputEncoding = System.Text.Encoding.UTF8

            Call Console.WriteLine("GCModeller :: ProteinTools.Interactions :: numerical conformance tests")
            Call Console.WriteLine($"started at {Date.Now:yyyy-MM-dd HH:mm:ss}")

            Call Support.RunGroup("numeric", AddressOf Tests.Numeric.Run)
            Call Support.RunGroup("dirichlet-multinomial", AddressOf Tests.Dirichlet.Run)
            Call Support.RunGroup("dependency tree & matrix-tree", AddressOf Tests.Tree.Run)
            Call Support.RunGroup("belief network inference", AddressOf Tests.BeliefNetwork.Run)
            Call Support.RunGroup("parameter / structure learning", AddressOf Tests.Learning.Run)
            Call Support.RunGroup("joint alignment & counting", AddressOf Tests.Alignment.Run)
            Call Support.RunGroup("ppi model & scoring", AddressOf Tests.Ppi.Run)
            Call Support.RunGroup("gibbs sampler", AddressOf Tests.Gibbs.Run)
            Call Support.RunGroup("evaluation metrics", AddressOf Tests.Evaluation.Run)

            Call Console.WriteLine()
            Call Console.WriteLine(DebugAudit.Dump())

            Dim report As String = IO.Path.Combine(AppContext.BaseDirectory, "test-report.json")
            Call DebugAudit.SaveReport(report)

            Call Console.WriteLine()
            Call Console.WriteLine($"{Support.Assertions} assertion(s), {DebugAudit.FailureCount} failure(s)")
            Call Console.WriteLine($"report saved to {report}")

            Environment.ExitCode = If(DebugAudit.FailureCount > 0, 1, 0)
        End Sub
    End Module
End Namespace
