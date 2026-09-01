Namespace ModularNetwork.WGCNA

    ''' <summary>
    ''' WGCNA gene module color assignment result
    ''' </summary>
    Public Class GeneModuleColor

        Public Property geneID As String
        Public Property moduleColor As String
        ''' <summary>
        ''' membership of current gene to target module color
        ''' </summary>
        ''' <returns></returns>
        Public Property kME As Double

        Public Overrides Function ToString() As String
            Return $"[{moduleColor}] {geneID}:= {kME}"
        End Function

    End Class

End Namespace