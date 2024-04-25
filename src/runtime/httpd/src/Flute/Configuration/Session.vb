Imports System.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf

Namespace Configurations

    <ClassName("session")>
    Public Class Session

        <Description("the prefix for the user session id.")>
        Public Property session_id_prefix As String = "flute_www_"

        <Description("the directory folder path for save the session data as files.")>
        Public Property session_store As String = "/tmp/flute_sessions/"

        <Description("enable the session?")>
        Public Property session_enable As Boolean = True

    End Class
End Namespace