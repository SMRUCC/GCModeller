Namespace KdTree

    Public MustInherit Class KdNodeAccessor(Of T)

        Default Public Property DimensionAccess(x As T, dimName As String) As Double
            Get
                Return getByDimension(x, dimName)
            End Get
            Set(value As Double)
                Call setByDimensin(x, dimName, value)
            End Set
        End Property

        Public MustOverride Function GetDimensions() As String()
        Public MustOverride Function metric(a As T, b As T) As Double
        Public MustOverride Function getByDimension(x As T, dimName As String) As Double
        Public MustOverride Sub setByDimensin(x As T, dimName As String, value As Double)
        Public MustOverride Function nodeIs(a As T, b As T) As Boolean

    End Class
End Namespace