<AttributeUsage(AttributeTargets.Field, AllowMultiple:=False, Inherited:=False)>
Public Class Content : Inherits Attribute

    Public ReadOnly Property Content As String

    Sub New(content As String)
        Me.Content = content
    End Sub

    Public Overrides Function ToString() As String
        Return Content
    End Function
End Class
