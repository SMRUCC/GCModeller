' ============================================================================
' AuditRunner.vb - 生产化审计的统一入口
'
' 用法:
'   dotnet run -- --audit
'
' 运行结束后在 bin 目录生成 vae-audit-report.json 与 vae-audit-dump.txt。
' 进程退出码为失败项数量（0 表示全部通过），便于 CI 直接判读。
' ============================================================================

''' <summary>
''' VAE 生成模型生产化审计的统一入口。
''' </summary>
Public Module AuditRunner

    ''' <summary>
    ''' 运行全部审计分组，返回失败项数量。
    ''' </summary>
    Public Function Run() As Integer
        Call AuditSupport.Begin()

        ' 公共基础设施自检
        Call AuditSupport.Group("numerics", AddressOf NumericsAudit.Run)

        ' 各模型的生产级验收
        Call AuditSupport.Group("gmm", AddressOf GmmAudit.Run)
        Call AuditSupport.Group("gmvae", AddressOf GmvaeAudit.Run)
        Call AuditSupport.Group("graphvae", AddressOf GraphVaeAudit.Run)
        Call AuditSupport.Group("cells", AddressOf CellsAudit.Run)

        ' 已确认缺陷的回归用例（重构前后对比的基准）
        Call AuditSupport.Group("regression", AddressOf RegressionAudit.Run)

        Return AuditSupport.Finish()
    End Function

End Module
