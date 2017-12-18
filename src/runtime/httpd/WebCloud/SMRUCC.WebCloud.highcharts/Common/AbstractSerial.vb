Imports Microsoft.VisualBasic.Serialization.JSON

Public MustInherit Class AbstractSerial(Of T)

    Public Property type As String
    Public Property name As String

    Public Overridable Property data As T()

    Public Overrides Function ToString() As String
        Return $"Dim {name} As {type} = {data.GetJson}"
    End Function
End Class

Public Class GenericDataSerial : Inherits AbstractSerial(Of Double)

End Class