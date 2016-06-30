
<[Namespace]("kernel32.dll")>
Public Module Kernel32

    ''' <summary>
    ''' Sleep pauses program execution for a certain amount of time. This is more accurate than using a do-nothing loop, waiting for a certain amount of time to pass. The function does not return a value. 
    ''' </summary>
    ''' <param name="dwMilliseconds">The number of milliseconds to halt program execution for. </param>
    ''' <remarks></remarks>
    ''' 
    <Command("Sleep")>
    Public Declare Sub Sleep Lib "kernel32.dll" (ByVal dwMilliseconds As Long)
    <Command("GetWindowsDirectoryA")>
    Public Declare Function GetWindowsDirectory Lib "kernel32" Alias "GetWindowsDirectoryA" (lpBuffer As String, nSize As Integer) As Integer
    <Command("GetSystemDirectoryA")>
    Public Declare Function GetSystemDirectory Lib "kernel32" Alias "GetSystemDirectoryA" (lpBuffer As String, nSize As Integer) As Integer
    <Command("lstrlenA")>
    Public Declare Function lstrlen Lib "kernel32" Alias "lstrlenA" (lpString As String) As Integer
End Module
