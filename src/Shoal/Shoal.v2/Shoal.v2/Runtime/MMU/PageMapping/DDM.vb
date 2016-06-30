Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter

Namespace Runtime.MMU.PageMapping

    Public Class Field : Inherits DataSourceModel
        Implements IPageUnit

        Dim BindingField As System.Reflection.FieldInfo

        Sub New(Target As System.Reflection.FieldInfo, Name As String)
            Call MyBase.New(Name)
            BindingField = Target
        End Sub

        Public Overrides Function GetValue() As Object
            Return BindingField.GetValue(Nothing)
        End Function

        Public Overrides Sub SetValue(value As Object)
            Call BindingField.SetValue(Nothing, value)
        End Sub

        Public Overrides ReadOnly Property [TypeOf] As Type
            Get
                Return BindingField.FieldType
            End Get
        End Property
    End Class

    Public Class [Property] : Inherits DataSourceModel
        Implements IPageUnit

        Dim BindingProperty As System.Reflection.PropertyInfo

        Sub New(Target As System.Reflection.PropertyInfo, Name As String)
            Call MyBase.New(Name)
            BindingProperty = Target
        End Sub

        Public Overrides Function GetValue() As Object
            Return BindingProperty.GetValue(Nothing)
        End Function

        Public Overrides Sub SetValue(value As Object)
            Call BindingProperty.SetValue(Nothing, value)
        End Sub

        Public Overrides ReadOnly Property [TypeOf] As Type
            Get
                Return BindingProperty.PropertyType
            End Get
        End Property
    End Class
End Namespace