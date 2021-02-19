
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.MarkDown
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports WkHtmlToPdf
Imports WkHtmlToPdf.Arguments

<Package("pdf", Category:=APICategories.UtilityTools)>
Module pdf

    <Extension>
    Private Iterator Function GetContentHtml(files As IEnumerable(Of String)) As IEnumerable(Of String)
        Dim render As New MarkdownHTML
        Dim dir As String = App.CurrentDirectory

        For Each file As String In files.SafeQuery
            If file.ExtensionSuffix("html") Then
                ' Yield RelativePath(dir, file.GetFullPath)
                Yield file.GetFullPath
            Else
                Dim htmlfile As String = file.GetFullPath.ChangeSuffix("html")
                Dim html As String = file _
                    .ReadAllText _
                    .DoCall(AddressOf render.Transform)

                Call html.SaveTo(htmlfile)

                ' Yield RelativePath(dir, htmlfile)
                Yield htmlfile
            End If
        Next
    End Function

    ''' <summary>
    ''' convert the local html documents to pdf document.
    ''' </summary>
    ''' <param name="files">
    ''' markdown files or html files
    ''' </param>
    ''' <param name="pdfout"></param>
    <ExportAPI("makePDF")>
    <RApiReturn(GetType(String))>
    Public Function makePDF(files As String(),
                            Optional pdfout As String = "out.pdf",
                            Optional env As Environment = Nothing) As Object

        Dim contentUrls As String() = files.GetContentHtml.ToArray

        If contentUrls.IsNullOrEmpty Then
            Return Internal.debug.stop("no pdf content files was found!", env)
        End If

        Dim content As New PdfDocument With {.Url = contentUrls}
        Dim output As New PdfOutput With {.OutputFilePath = pdfout}
        Dim wkhtmltopdf As New PdfConvertEnvironment With {
            .TempFolderPath = App.GetAppSysTempFile("__pdf", App.PID.ToHexString, "wkhtmltopdf"),
            .Debug = env.globalEnvironment.Rscript.debug,
            .Timeout = 60000,
            .WkHtmlToPdfPath = env.globalEnvironment.options.getOption("wkhtmltopdf")
        }

        If wkhtmltopdf.WkHtmlToPdfPath.StringEmpty Then
            Return Internal.debug.stop("please config wkhtmltopdf program at first!", env)
        ElseIf Not wkhtmltopdf.WkHtmlToPdfPath.FileExists Then
            Return Internal.debug.stop($"wkhtmltopdf program is not exists at the given location: '{wkhtmltopdf.WkHtmlToPdfPath}'...", env)
        End If

        If wkhtmltopdf.Debug Then
            Call Console.WriteLine("wkhtmltopdf config:")
            Call Console.WriteLine(wkhtmltopdf.GetJson)
        End If

        If Not content.CheckContentSource Then
            Return Internal.debug.stop("part of the content is missing... break pdf conversion progress...", env)
        End If

        Call pdfout.ParentPath.MkDIR
        Call PdfConvert.ConvertHtmlToPdf(content, output, environment:=wkhtmltopdf)

        Return Nothing
    End Function
End Module
