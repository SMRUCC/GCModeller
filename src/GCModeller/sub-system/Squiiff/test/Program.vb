Imports System

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        Dim ok As Boolean = GradientCheck.Run()
        Environment.ExitCode = If(ok, 0, 1)
    End Sub

End Module
