' ============================================================================
' VAGene 测试入口
'
' 用法：
'   dotnet run -- --audit
'
' 运行结束后在 bin 目录生成 vagene-audit-report.json 与 vagene-audit-dump.txt，
' 进程退出码为失败项数量（0 表示全部通过），便于 CI 直接判读。
' ============================================================================

''' <summary>VAGene 审计入口。</summary>
Public Module Program

    Public Sub Main(args As String())
        If args IsNot Nothing AndAlso Array.IndexOf(args, "--audit") >= 0 Then
            Environment.ExitCode = AuditRunner.Run()
            Return
        End If

        Console.WriteLine("VAGene: 条件变分自编码器虚拟扰动实验")
        Console.WriteLine("使用 --audit 运行 DebugAudit 审计。")
        Environment.ExitCode = AuditRunner.Run()
    End Sub

End Module
