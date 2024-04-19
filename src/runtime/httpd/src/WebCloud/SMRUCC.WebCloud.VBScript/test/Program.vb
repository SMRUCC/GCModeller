Imports Flute.Template

Module Program

    Sub Main(args As String())
        Console.WriteLine("Hello World!")

        Call variable_reflection_test()
    End Sub

    Const demo_test_template As String = "\GCModeller\src\runtime\httpd\test\template_test\index.vbhtml"

    Sub variable_reflection_test()
        Dim testdata As New Dictionary(Of String, Object) From {
            {"person", New person},
            {"title", "demo html page"},
            {"lang", "zh"}
        }
        Dim html As String = VBHtml.ReadHTML(demo_test_template, testdata)

        Call Console.WriteLine(html)

        testdata!lang = "en"

        Call Console.WriteLine(VBHtml.ReadHTML(demo_test_template, testdata))

        Pause()
    End Sub
End Module

Public Class person

    Public Property name As String = "aaaaaa"
    Public Property age As Integer = 100

End Class
