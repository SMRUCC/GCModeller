
Namespace RBase.Vectors

    Public Class BooleanVector : Inherits GenericVector(Of Boolean)

        Public Shared ReadOnly Property [True] As BooleanVector
            Get
                Return New BooleanVector({True})
            End Get
        End Property

        Sub New(Elements As Generic.IEnumerable(Of Boolean))
            Me.Elements = Elements.ToArray
        End Sub

        Public Shared Operator &(x As Boolean, y As BooleanVector) As BooleanVector
            Return New BooleanVector((From b As Boolean In y Select b AndAlso x).ToArray)
        End Operator

        Public Shared Operator &(x As BooleanVector, y As BooleanVector) As BooleanVector
            Return New BooleanVector((From i As Integer In x.Sequence Select x.Elements(i) AndAlso y.Elements(i)).ToArray)
        End Operator

        Public Shared Operator Not(x As BooleanVector) As BooleanVector
            Return New BooleanVector((From b As Boolean In x Select Not b).ToArray)
        End Operator

        Public Shared Narrowing Operator CType(x As BooleanVector) As Boolean
            Return x.Elements(0)
        End Operator

        Public Shared Widening Operator CType(b As Boolean()) As BooleanVector
            Return New BooleanVector(b)
        End Operator

        Public Shared Operator Or(x As BooleanVector, y As Boolean()) As BooleanVector
            Return New BooleanVector((From i As Integer In x.Elements.Sequence Select x.Elements(i) Or y(i)).ToArray)
        End Operator

        Public Shared Operator Or(x As BooleanVector, y As BooleanVector) As BooleanVector
            Return x Or y.Elements
        End Operator

        Public Shared Narrowing Operator CType(x As BooleanVector) As Boolean()
            Return x.Elements
        End Operator
    End Class
End Namespace