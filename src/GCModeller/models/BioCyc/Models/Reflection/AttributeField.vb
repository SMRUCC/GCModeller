
Imports System.Reflection

<AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
Public Class AttributeField : Inherits Attribute

    Public ReadOnly Property name As String

    Sub New(name As String)
        Me.name = name
    End Sub

    Public Overrides Function ToString() As String
        Return name
    End Function

    Friend Shared Function getMappingKey(p As PropertyInfo) As String
        Dim attrs = p.GetCustomAttributes(Of AttributeField)().ToArray

        If attrs.IsNullOrEmpty Then
            Return Nothing
        Else
            Return attrs(Scan0).name
        End If
    End Function

End Class
