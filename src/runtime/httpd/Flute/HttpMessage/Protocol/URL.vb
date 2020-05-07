Namespace Core.Message

    Public Class URL

        ''' <summary>
        ''' the url query parameters
        ''' </summary>
        ''' <returns></returns>
        Public Property query As Dictionary(Of String, String)
        Public Property path As String
        Public Property hostName As String
        Public Property port As Integer
        Public Property protocol As String
        ''' <summary>
        ''' #....
        ''' </summary>
        ''' <returns></returns>
        Public Property hashcode As String

        Sub New(url As String)

        End Sub
    End Class
End Namespace