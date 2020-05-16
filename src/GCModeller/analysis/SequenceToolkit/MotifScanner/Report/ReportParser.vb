Public Module ReportParser

    Public Iterator Function ParseReport(text As String) As IEnumerable(Of GeneReport)
        For Each section As String In text.StringSplit("^[>]", , RegexICMul)
            Yield ParseSectionData(section)
        Next
    End Function

    Private Function ParseSectionData(text As String) As GeneReport

    End Function
End Module
