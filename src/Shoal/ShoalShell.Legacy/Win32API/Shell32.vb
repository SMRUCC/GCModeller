Imports Microsoft.VisualBasic.CommandLine.Reflection

<[Namespace]("shell32.dll")>
Public Module Shell32

    <Command("ShellExecuteA")>
    Public Declare Function ShellExecute Lib "shell32.dll" Alias "ShellExecuteA" (hwnd As Integer, lpOperation As String, lpFile As String, lpParameters As String, lpDirectory As String, nShowCmd As Integer) As Integer
End Module
