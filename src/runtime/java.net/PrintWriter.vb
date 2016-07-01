
Imports Oracle.Java.IO

Public Class PrintWriter

    Dim repFile As File
    Dim Writter As Global.System.IO.StreamWriter

    Public Sub New(repFile As File)
        Me.repFile = repFile
        Writter = New Global.System.IO.StreamWriter(repFile.absolutePath, append:=False)
    End Sub

    Public Overrides Function ToString() As String
        Return repFile.absolutePath
    End Function

    Public Sub println(v As String)
        Call Writter.WriteLine(v)
    End Sub

    Public Sub print(v As String)
        Call Writter.Write(v)
    End Sub

    Public Sub close()
        Call Writter.Flush()
        Call Writter.Close()
    End Sub

    Public Sub println()
        Call Writter.WriteLine()
    End Sub
End Class
