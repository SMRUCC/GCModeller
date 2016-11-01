Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class serialData

    Public Property name As String
    Public Property data As date_count()
    Public Property total As Long

End Class

Public Structure date_count
    Implements sIdEnumerable

    Public Property [date] As String Implements sIdEnumerable.Identifier
    Public Property count As Integer

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure