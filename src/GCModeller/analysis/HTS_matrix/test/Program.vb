Imports System

Module Program

    ''' <summary>
    ''' 超大型矩阵流式 IO 的回归测试入口
    ''' </summary>
    ''' <remarks>
    ''' 退出码 = 失败的断言数量，便于 CI 判断结果；测试报告写在 <c>report/hts_matrix_io.json</c>。
    ''' </remarks>
    Sub Main(args As String())
        ' 1. 二进制/CSV 流式 IO 的回归测试
        Dim failures As Integer = MatrixIoTest.Run()

        ' 2. 与 GNU R (limma/edgeR/DESeq2) 的数学对拍
        failures += RCrossCheckTest.Run()

        ' 3. 输出审计报告（文本摘要 + report/hts_matrix_io.json）
        Call MatrixIoTest.Report()

        ' 4. 合成菌群表达矩阵分解（report/community-decomposition-report.json）
        failures += DecompositionTest.Run()

        Environment.ExitCode = failures
    End Sub

End Module
