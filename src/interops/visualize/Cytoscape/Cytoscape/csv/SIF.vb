Namespace Tables

    Public Class SIF

        Public Property source As String
        Public Property interaction As String
        Public Property target As String

        Public Overrides Function ToString() As String
            Return $"{source}{vbTab}{interaction}{vbTab}{target}"
        End Function

        Public Shared Function ToText(network As IEnumerable(Of SIF)) As String
            Return network.JoinBy(vbLf)
        End Function
    End Class
End Namespace