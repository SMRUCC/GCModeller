Imports Microsoft.VisualBasic.Imaging.Driver

Module Program

    Sub New()
        Call ImageDriver.Register()
    End Sub

    Sub Main(args As String())
        Console.WriteLine("Hello World!")
    End Sub
End Module
