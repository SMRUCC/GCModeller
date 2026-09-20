Imports System

Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8
        Environment.ExitCode = If(GradientCheck.Run(), 0, 1)
    End Sub

End Module
