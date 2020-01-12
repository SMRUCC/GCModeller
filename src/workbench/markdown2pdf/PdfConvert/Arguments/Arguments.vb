Imports System.IO
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Arguments

    Public Class PdfOutput

        Public Property OutputFilePath As String
        Public Property OutputStream As Stream
        Public Property OutputCallback As Action(Of PDFContent, Byte())

        Public Overrides Function ToString() As String
            Return OutputFilePath
        End Function

    End Class

    Public Class PdfConvertEnvironment

        Public Property TempFolderPath As String
        Public Property WkHtmlToPdfPath As String
        Public Property Timeout As Integer
        Public Property Debug As Boolean

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace