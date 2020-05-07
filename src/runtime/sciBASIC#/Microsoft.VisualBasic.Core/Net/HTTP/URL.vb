Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Net.Http

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
            With url.GetTagValue("?")

            End With
        End Sub

        Private Sub New()
        End Sub

        Public Shared Function BuildUrl(url As String, query As IEnumerable(Of NamedValue(Of String))) As URL

        End Function
    End Class
End Namespace