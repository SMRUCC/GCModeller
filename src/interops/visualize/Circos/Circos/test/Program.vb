Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.Imaging.Driver

Module Program

    Sub New()
        ' register .net image driver for draw circos in .net via gdi+
        Call ImageDriver.Register()
    End Sub

    Sub Main(args As String())
        Dim root As String = If(args.FirstOrDefault, "Z:\circos-test\")

        Call Directory.CreateDirectory(root)

        Dim failures As Integer

        For Each result In DemoGallery.RunAll(root)
            If result.Success Then
                Console.WriteLine($"  [ OK ] {result.PngPath}")
            Else
                failures += 1
                Console.WriteLine($"  [FAIL] {result.ConfFile}")
                Console.WriteLine(result.Message)
            End If
        Next

        Console.WriteLine()
        Console.WriteLine($"DONE, failure = {failures}")
    End Sub
End Module
