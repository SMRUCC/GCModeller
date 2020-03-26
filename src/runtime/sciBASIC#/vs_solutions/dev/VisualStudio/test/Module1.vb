Imports Microsoft.VisualBasic.ApplicationServices.Development.VisualStudio

Module Module1

    Sub Main()
        Call vlqtest()
    End Sub

    Sub vlqtest()
        Console.WriteLine(base64VLQ.base64VLQ_encode(16))
        Console.WriteLine(base64VLQ.base64VLQ_decode("gB"))

        Pause()
    End Sub
End Module
