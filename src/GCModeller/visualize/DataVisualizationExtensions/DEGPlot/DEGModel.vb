Public Structure DEGModel
    Implements IDeg

    Public Property label$ Implements IDeg.label
    Public Property logFC# Implements IDeg.log2FC
    Public Property pvalue# Implements IDeg.pvalue

    Public Overrides Function ToString() As String
        Return $"[{label}] log2FC={logFC}, pvalue={pvalue}"
    End Function
End Structure