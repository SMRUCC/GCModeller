Namespace Core.HttpStream

    Friend Class StreamElement

        Public ContentType As String
        Public Name As String
        Public Filename As String
        Public Start As Long
        Public Length As Long

        Public Overrides Function ToString() As String
            Return "ContentType " & ContentType & ", Name " & Name & ", Filename " & Filename & ", Start " & Start.ToString() & ", Length " & Length.ToString()
        End Function
    End Class
End Namespace