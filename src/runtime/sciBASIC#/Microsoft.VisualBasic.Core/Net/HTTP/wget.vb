Imports System.Net

Namespace Net.Http

    ''' <summary>
    ''' 命令行下的下载程序组件
    ''' </summary>
    Public Class wget

        Dim WithEvents task As wgetTask
        Dim cursorTop%

        Sub New(url$, save$)
            task = New wgetTask(url, save)
            cursorTop = Console.CursorTop
        End Sub

        Public Sub Run()
            Call task.StartTask()
        End Sub

        Private Sub DownloadProcess(wget As wgetTask, percentage As Double) Handles task.DownloadProcess
            Console.CursorTop = cursorTop
            Console.CursorLeft = 1
            Console.Write(New String(" "c, Console.BufferWidth))
            Console.CursorLeft = 1
            Console.WriteLine(wget.ToString)
        End Sub

        Private Sub ReportRequest(req As WebRequest) Handles task.ReportRequest
            Dim domain As New DomainName(task.url)

            Call Console.WriteLine($"--{Now.ToString}--  {task.url}")
            Call Console.WriteLine($"     => '{task.saveFile.FileName}'")
            Call Console.WriteLine($"Resolving {domain} ({domain})... {req.RequestUri.Host}")
            Call Console.WriteLine($"==> SIZE {task.saveFile.FileName} ... {req.ContentLength}")
            Call Console.WriteLine($"==> CONTENT-TYPE ... {req.ContentType}")
            Call Console.WriteLine($"Length: {req.ContentLength} ()")
            Call Console.WriteLine()

            cursorTop = Console.CursorTop
        End Sub
    End Class
End Namespace