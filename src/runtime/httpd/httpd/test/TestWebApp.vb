Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments
Imports SMRUCC.WebCloud.HTTPInternal.Platform

<[Namespace]("test")>
Public Class ClassTestWebApp : Inherits WebApp

    Public Sub New(main As PlatformEngine)
        MyBase.New(main)
    End Sub

    <ExportAPI("/test/get.vbs")>
    <[GET]>
    Public Function testGET(request As HttpRequest, response As HttpResponse) As Boolean
        Dim args = request.URLParameters.ToDictionary

        If args.IsNullOrEmpty Then
            args = New Dictionary(Of String, String) From {
                {"test", "test response data"}
            }
        End If

        Call response.WriteJSON(args, indent:=False)

        Return True
    End Function
End Class
