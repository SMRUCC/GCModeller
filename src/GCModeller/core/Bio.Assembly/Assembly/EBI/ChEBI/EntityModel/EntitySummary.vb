Namespace Assembly.EBI.ChEBI

    ''' <summary>
    ''' 物质注释信息摘要表格
    ''' </summary>
    Public Class EntitySummary

        Public Property chebiId As String
        Public Property chebiAsciiName As String
        Public Property smiles As String
        Public Property charge As Integer
        Public Property mass As Double
        Public Property formulae As String
        Public Property secondaryChebiIds As String()
        Public Property kegg_Ids As String()
        Public Property wikipedia As String
        Public Property inchi As String
        Public Property inchiKey As String
        Public Property cas As String()
        Public Property hmdb As String
        Public Property biosamples As String()

    End Class
End Namespace