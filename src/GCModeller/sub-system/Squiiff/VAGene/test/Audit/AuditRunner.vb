' ============================================================================
' AuditRunner.vb - VAGene 审计的统一入口
'
' 用法:
'   dotnet run -- --audit
' ============================================================================

''' <summary>VAGene 虚拟扰动实验审计的统一入口。</summary>
Public Module AuditRunner

    ''' <summary>运行全部审计分组，返回失败项数量。</summary>
    Public Function Run() As Integer
        Call AuditSupport.Begin()

        Call AuditSupport.Group("dataset", AddressOf VAGeneAudit.DatasetMustNormalizeAndSlice)
        Call AuditSupport.Group("likelihood", AddressOf VAGeneAudit.LikelihoodMustMatchClosedForm)
        Call AuditSupport.Group("model", AddressOf VAGeneAudit.ModelMustFitAndExposeExpressionApi)
        Call AuditSupport.Group("counterfactual", AddressOf VAGeneAudit.FacadeMustPredictCounterfactual)
        Call AuditSupport.Group("context", AddressOf VAGeneAudit.ContextMustRealignAndClear)
        Call AuditSupport.Group("registry", AddressOf VAGeneAudit.ErrorsMustBeExplicit)
        Call AuditSupport.Group("persistence", AddressOf VAGeneAudit.StorageMustRoundTrip)

        Return AuditSupport.Finish()
    End Function

End Module
