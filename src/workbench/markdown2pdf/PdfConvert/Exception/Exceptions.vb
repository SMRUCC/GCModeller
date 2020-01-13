Public Class PdfConvertException
    Inherits Exception

    Public Sub New(msg As String)
        MyBase.New(msg)
    End Sub
End Class

Public Class PdfConvertTimeoutException
    Inherits PdfConvertException

    Const msg$ = "HTML to PDF conversion process has not finished in the given period."

    Public Sub New()
        Call MyBase.New(msg)
    End Sub
End Class