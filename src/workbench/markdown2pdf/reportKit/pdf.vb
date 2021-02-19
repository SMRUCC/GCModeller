
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports WkHtmlToPdf
Imports WkHtmlToPdf.Arguments

<Package("pdf", Category:=APICategories.UtilityTools)>
Module pdf

    ''' <summary>
    ''' convert the local html documents to pdf document.
    ''' </summary>
    ''' <param name="files"></param>
    ''' <param name="pdfout"></param>
    <ExportAPI("makePDF")>
    <RApiReturn(GetType(String))>
    Public Function makePDF(files As String(),
                            Optional pdfout As String = "out.pdf",
                            Optional env As Environment = Nothing) As Object

        Dim contentUrls As String() = files _
            .SafeQuery _
            .Select(Function(path) path.GetFullPath) _
            .ToArray

        If contentUrls.IsNullOrEmpty Then
            Return Internal.debug.stop("no pdf content files was found!", env)
        End If

        Dim content As New PdfDocument With {.Url = contentUrls}
        Dim output As New PdfOutput With {.OutputFilePath = pdfout}

        Call pdfout.ParentPath.MkDIR
        Call PdfConvert.ConvertHtmlToPdf(content, output)

        Return Nothing
    End Function
End Module
