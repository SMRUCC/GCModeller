Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments
Imports SMRUCC.WebCloud.HTTPInternal.Platform

<[Namespace]("mysql")>
Public Class mysql : Inherits SMRUCC.WebCloud.HTTPInternal.AppEngine.WebApp

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
