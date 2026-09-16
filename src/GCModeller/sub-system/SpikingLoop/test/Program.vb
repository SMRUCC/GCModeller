' ============================================================================
' Program.vb — SpikingLoop 演示入口
'
' 完整流程见 SpikingLoopDemo.Run()。
' 可选命令行参数：第 1 个参数为 demo 数据目录（内含 gene_expression_matrix.csv
' 与 regulatory_network_prior.csv）；省略时自动向上回溯查找 sub-system/demo/TestData1。
' ============================================================================

Module Program

    Sub Main()
        Environment.ExitCode = SpikingLoopDemo.Run()
    End Sub

End Module
