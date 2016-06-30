Imports Microsoft.VisualBasic.CommandLine.Reflection

<[Namespace]("test1")>
Public Class Test1

    <Command("msgbox")>
    Public Shared Function _MsgBox(str As String) As String
        MsgBox(str)
        Return str
    End Function

    <Command("array_def")>
    Public Shared Function createarray(str As String) As Char()
        Return str.ToArray
    End Function
End Class
