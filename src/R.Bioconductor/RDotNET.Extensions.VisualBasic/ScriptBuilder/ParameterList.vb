Namespace SymbolBuilder

    ''' <summary>
    ''' <see cref="ValueTypes.List"/>
    ''' </summary>
    Public Class ParameterList

        Public Property parameters As String()

        Sub New(list$())
            parameters = list
        End Sub

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return parameters.JoinBy(", ")
        End Function
    End Class
End Namespace