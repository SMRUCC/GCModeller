Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Visualize.Circos.Configurations

Module Program

    Sub New()
        ' register .net image driver for draw circos in .net via gdi+
        Call ImageDriver.Register()
    End Sub

    Sub Main(args As String())
        Dim root As String = If(args.FirstOrDefault, "Z:\circos-test\")
        ' 第二个参数可以用于选择渲染模式：all(默认) / gdi / circos
        Dim mode As String = If(args.Length > 1, args(1), "all").ToLowerInvariant()

        Call Directory.CreateDirectory(root)

        Dim failures As Integer

        If mode = "all" OrElse mode = "circos" Then
            ' 1. 调用外部的 circos 程序绘图
            Console.WriteLine("==== circos (external program) ====")

            For Each result In DemoGallery.RunAll(root)
                failures += report(result)
            Next
        End If

        If mode = "all" OrElse mode = "gdi" Then
            ' 2. 使用内置的 GDI+ 引擎绘图（不需要外部依赖）
            Console.WriteLine()
            Console.WriteLine("==== GDI+ (built-in engine) ====")

            For Each result In DemoGallery.RunAllGdiPlus(root)
                failures += report(result)
            Next
        End If

        Console.WriteLine()
        Console.WriteLine($"DONE, failure = {failures}")
    End Sub

    Private Function report(result As CircosRenderResult) As Integer
        If result.Success Then
            Console.WriteLine($"  [ OK ] {result.PngPath}")
            Return 0
        Else
            Console.WriteLine($"  [FAIL] {If(result.ConfFile, result.PngPath)}")
            Console.WriteLine(result.Message)
            Return 1
        End If
    End Function
End Module
