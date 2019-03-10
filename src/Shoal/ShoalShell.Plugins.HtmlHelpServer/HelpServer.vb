Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports ShoalShell.Plugins.HtmlHelpServer.HttpInternal
Imports System.Threading
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData

<[PackageNamespace]("Wiki.Http_Server",
                    Description:="Running the shoal shell local HTTP server for served the help information about the shoal shell system.",
                    Category:=APICategories.SoftwareTools,
                    Publisher:="xie.guigang@gcmodeller.org",
                    Url:="http://gcmodeller.org")>
Public Module HelpServer

    Dim _serverEngine As ShoalShell.Plugins.HtmlHelpServer.NotificationIcon

    <ExportAPI("_start()")>
    Public Function StartServer() As Integer
        Call (Sub() NotificationIcon.Main(Nothing)).BeginInvoke(Nothing, Nothing)
        Return 0
    End Function

    Friend Function __runServer(Optional LocalPort As Integer = 8080) As Thread
        Dim HttpServer As HttpServer = New HttpInternal.HttpFileSystem(LocalPort, App.HOME & "/html", True)
        Dim svrThread = RunTask(AddressOf HttpServer.Run)
        Call Thread.Sleep(1000)
        Call Process.Start("http://127.0.0.1:8080")
        Return svrThread
    End Function

    <ExportAPI("Doc.Build")>
    Public Sub BuildDoc(Optional library As String = "./html/")
        Call Index.GenerateDocument(library)
    End Sub
End Module
