Imports Microsoft.VisualBasic.Text

Namespace LocalBLAST.BLASTOutput.BlastPlus

    Public Module BlastnOutputReader

        Public Iterator Function RunParser(path$, Optional encoding As Encodings = Encodings.UTF8) As IEnumerable(Of Query)
            Dim source As IEnumerable(Of String) = QueryBlockIterates(path, encoding)
            Dim q As Query

            For Each queryText As String In source
                q = Query.BlastnOutputParser(queryText)
                Yield q
            Next
        End Function
    End Module
End Namespace