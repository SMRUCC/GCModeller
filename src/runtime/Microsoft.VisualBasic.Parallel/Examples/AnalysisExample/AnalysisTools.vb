Imports System.IO

Public Module API

    Public Function LongTest1(file As Stream, a2 As String) As String()
        Dim buf As Byte() = file.CopyStream
        Dim txt As String = System.Text.Encoding.Default.GetString(buf)
        Dim lines As String() = txt.lTokens
        For Each x In lines
            Call Console.WriteLine(x)
        Next

        Call MsgBox(a2, MsgBoxStyle.Critical)

        Return lines
    End Function
End Module