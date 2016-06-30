Imports System.Text

Public Module EMailMsg

    Public Function GetMessage(Title As String, Message As String, User As String, Link As String, LinkTitle As String) As String
        Dim html As New StringBuilder(My.Resources.readmail)

        Call html.Replace("{Title}", Title)
        Call html.Replace("{UserName}", User)
        Call html.Replace("{Message}", Message)
        Call html.Replace("{LinkTitle}", LinkTitle)
        Call html.Replace("{Link}", Link)

        Return html.ToString
    End Function
End Module
