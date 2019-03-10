Imports ShoalShell.Plugins.HtmlHelpServer.HttpInternal
Imports System.IO
Imports System.Net.Sockets

Public Class ShoalHttpServer : Inherits HttpServer

    Dim _InternalShoalWikiServer As Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.ScriptEngine =
        New Scripting.ShoalShell.Runtime.ScriptEngine

    Public Sub New(port As Integer)
        Call MyBase.New(port)
    End Sub

    Public Overrides Sub handleGETRequest(p As HttpProcessor)

        If String.Equals(p.http_url, "/") Then
            '  Call p.writeSuccess()
            Call p.outputStream.WriteLine(FileIO.FileSystem.ReadAllText(Scripting.ShoalShell.HTML.RequestHtml("index")))
            Return

        Else


        End If

        'If p.http_url.Equals("/Test.png") Then
        '    Dim fs As Stream = File.Open("../../Test.png", FileMode.Open)

        '    p.writeSuccess("image/png")
        '    fs.CopyTo(p.outputStream.BaseStream)
        '    p.outputStream.BaseStream.Flush()
        'End If

        'Console.WriteLine("request: {0}", p.http_url)
        'p.writeSuccess()
        'p.outputStream.WriteLine("<html><body><h1>Shoal SystemsBiology Shell Language</h1>")
        'p.outputStream.WriteLine("Current Time: " & DateTime.Now.ToString())
        'p.outputStream.WriteLine("url : {0}", p.http_url)

        'p.outputStream.WriteLine("<form method=post action=/local_wiki>")
        'p.outputStream.WriteLine("<input type=text name=SearchValue value=Keyword>")
        'p.outputStream.WriteLine("<input type=submit name=Invoker value=""Search"">")
        'p.outputStream.WriteLine("</form>")
    End Sub

    Public Overrides Sub handlePOSTRequest(p As HttpProcessor, inputData As StreamReader)
        Console.WriteLine("POST request: {0}", p.http_url)
        Dim data As String = inputData.ReadToEnd()

        data = System.Text.RegularExpressions.Regex.Match(data, "SearchValue=.+?&Invoker=", Text.RegularExpressions.RegexOptions.IgnoreCase).Value
        data = Mid(data, 13)
        data = Mid(data, 1, Len(data) - 9)

        p.writeSuccess()
        p.outputStream.WriteLine("<html><body><h1>Shoal SystemsBiology Shell Language</h1>")
        p.outputStream.WriteLine("<a href=/>>>> HOME</a><p>")
        p.outputStream.WriteLine("=====================================================================================<pre>Search results for ""{0}""</pre>", data)

        Dim ChunkBuffer = _InternalShoalWikiServer.GetHelpInfo(data, False)
        p.outputStream.WriteLine("<pre>{0}</pre>", ChunkBuffer)
    End Sub

    Protected Overrides Function __httpProcessor(client As TcpClient) As HttpProcessor
        Return New HttpProcessor(client, Me) With {._404Page = My.Resources._404}
    End Function
End Class