
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' BASys Annotation Table for 1329830 
''' </summary>
Public Class TableBrief

    Public Property Start As Integer
    Public Property [End] As Integer
    Public Property Strand As Char
    Public Property Accession As String
    Public Property Gene As String
    Public Property COG As String
    ''' <summary>
    ''' Protein Function
    ''' </summary>
    ''' <returns></returns>
    Public Property [Function] As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function TableParser(path As String) As TableBrief()
        Dim html As String = path.GET
    End Function
End Class
