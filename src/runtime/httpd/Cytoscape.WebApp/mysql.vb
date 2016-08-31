Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.HTTPInternal.AppEngine
Imports SMRUCC.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.HTTPInternal.AppEngine.APIMethods.Arguments
Imports SMRUCC.HTTPInternal.Platform

<[Namespace]("mysql")>
Public Class mysql : Inherits SMRUCC.HTTPInternal.AppEngine.WebApp

    Public Sub New(main As PlatformEngine)
        Call MyBase.New(main)
    End Sub

    <[GET](GetType(String))>
    <ExportAPI("/mysql/network.json")>
    Public Function json(request As HttpRequest, response As HttpResponse) As Boolean

    End Function

    Public Overrides Function Page404() As String
        Return ""
    End Function
End Class
