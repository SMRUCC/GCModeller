Imports System
Imports Microsoft.VisualBasic.Imaging.Driver

Module Program

    Sub New()
        ' register .net image driver for draw circos in .net via gdi+
        Call ImageDriver.Register()
    End Sub

    Sub Main(args As String())
        Console.WriteLine("Hello World!")
    End Sub
End Module
