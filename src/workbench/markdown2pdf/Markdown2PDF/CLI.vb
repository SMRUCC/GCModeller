Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.MarkDown
Imports WkHtmlToPdf
Imports WkHtmlToPdf.Arguments

Module CLI

    <ExportAPI("/glob")>
    <Usage("/glob /src <dir of *.md> [/pdf <output.pdf>]")>
    Public Function glob(args As CommandLine) As Integer
        Dim src$ = args <= "/src"
        Dim pdf$ = args("/pdf") Or $"{src.TrimDIR}.pdf"
        Dim render As New MarkdownHTML()
        Dim tmp As String = App.GetAppSysTempFile

        Call src.FileCopy(tmp & "/")

        Dim mdlist As String() = tmp _
            .ListFiles("*.md") _
            .OrderBy(Function(path)
                         Return path.BaseName
                     End Function) _
            .ToArray

        For Each file As String In mdlist
            Call file _
                .ReadAllText _
                .DoCall(AddressOf render.Transform) _
                .SaveTo(file.ChangeSuffix("html"))
        Next

        Dim srcDoc As New PdfDocument With {
            .Url = mdlist _
                .Select(Function(file)
                            Return file.ChangeSuffix("html")
                        End Function) _
                .ToArray
        }

        Call PdfConvert.ConvertHtmlToPdf(
            document:=srcDoc,
            output:=New PdfOutput With {
                .OutputFilePath = pdf
            }
        )

        Return 0
    End Function
End Module
