Imports Microsoft.VisualBasic.ApplicationServices.Development.VisualStudio
Imports Microsoft.VisualBasic.Serialization.JSON

Module Module1

    Sub Main()
        Call vlqtest()
    End Sub

    Sub vlqtest()
        Console.WriteLine(base64VLQ.base64VLQ_encode(16))
        Console.WriteLine(base64VLQ.base64VLQ_decode("gB"))
        Console.WriteLine(base64VLQ.getIntegers("AAAAA").ToArray.GetJson)
        Console.WriteLine(base64VLQ.getIntegers("BBBBB").ToArray.GetJson)
        Console.WriteLine(base64VLQ.getIntegers("CCCCC").ToArray.GetJson)

        Console.WriteLine(base64VLQ.getIntegers("AAgBC").ToArray.GetJson)
        Console.WriteLine(base64VLQ.getIntegers("SAAQ").ToArray.GetJson)
        Console.WriteLine(base64VLQ.getIntegers("CAAEA").ToArray.GetJson)

        Pause()
    End Sub
End Module
