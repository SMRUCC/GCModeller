Imports Flute.Http.Core.Message
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Class SessionManager : Inherits ServerComponent

    Public ReadOnly Property Id As String
    Public ReadOnly Property SetCookie As Boolean = False

    Public Const CookieName As String = "flute_session"

    Sub New(cookies As Cookies, settings As Configuration)
        Call MyBase.New(settings)

        If cookies.CheckCookie(CookieName) Then
            Id = cookies.GetCookie(CookieName).FirstOrDefault
        End If

        If Id.StringEmpty Then
            Id = settings.session_id_prefix & "_" & (Now.ToString & randf.NextDouble).MD5.Substring(8, 8)
            SetCookie = True
        End If
    End Sub

    Public Function GetSession(name As String) As Object

    End Function

    Public Sub SaveSession(name As String, value As String)

    End Sub

    Public Sub SaveSession(name As String, value As String())

    End Sub

End Class
