Public Module ReportParser

    Public Iterator Function ParseReport(text As String) As IEnumerable(Of GeneReport)
        For Each section As String() In text _
            .LineTokens _
            .Split(
                delimiter:=Function(l) l.FirstOrDefault = ">"c,
                deliPosition:=DelimiterLocation.NextFirst
            )

            Yield ParseSectionData(section)
        Next
    End Function

    Private Function ParseSectionData(lines As String()) As GeneReport

    End Function
End Module
