Imports System
Imports SMRUCC.genomics.Data

Module Program
    Sub Main(args As String())
        download_regprecise()
        Console.WriteLine("Hello World!")
    End Sub

    Sub download_regprecise()
        Dim target = "F:\ecoli\regprecise"

        WebServiceUtils.Proxy = Nothing ' "http://127.0.0.1:10809"

        Call Regprecise.WebAPI.Download(target)
    End Sub
End Module
