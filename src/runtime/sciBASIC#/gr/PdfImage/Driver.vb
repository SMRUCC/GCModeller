Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.MIME.application.pdf

Module Driver

    Public Function OpenDevice(size As Size) As PdfGraphics
        Dim buffer As New MemoryStream
        Dim stream As New PdfDocument(size.Width, size.Height, 1, buffer)
        Dim info = PdfInfo.CreatePdfInfo(stream)
        Dim localTime = DateTime.Now

        info.Title("Article Example")
        info.Author("Uzi Granot Granotech Limited")
        info.Keywords("PDF, .NET, C#, Library, Document Creator")
        info.Subject("PDF File Writer C# Class Library (Version 1.15.0)")
        info.CreationDate(localTime)
        info.ModDate(localTime)
        info.Creator("PdfFileWriter C# Class Library Version " & PdfDocument.RevisionNumber)
        info.Producer("PdfFileWriter C# Class Library Version " & PdfDocument.RevisionNumber)

        Dim page As New PdfPage(stream)
        Dim g As New PdfGraphics(page)

        Return g
    End Function

End Module
