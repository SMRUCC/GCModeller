Imports Microsoft.VisualBasic.Language

Public Class Project : Inherits ClassObject

    Public Property Summary As Summary
    Public Property Briefs As TableBrief()
    Public Property Ecards As Ecard()

    Public Function Write(EXPORT As String) As Boolean

    End Function

    Public Shared Function Parser(DIR As String) As Project

    End Function
End Class
