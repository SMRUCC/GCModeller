Namespace Graphics
    Public Structure Point : Implements IEquatable(Of Point)

        Public Sub New(x As Double, y As Double)
            _X = x
            _Y = y
        End Sub

        Public Property X() As Double
        Public Property Y() As Double

#Region "IEquatable<Point> Members"

        Public Overloads Function Equals(other As Point) As Boolean Implements IEquatable(Of Point).Equals
            Return (Me = other)
        End Function

#End Region

        Public Shared Operator =(p1 As Point, p2 As Point) As Boolean
            Return p1.X = p2.X AndAlso p1.Y = p2.Y
        End Operator

        Public Shared Operator <>(p1 As Point, p2 As Point) As Boolean
            Return Not (p1 = p2)
        End Operator

        Public Overrides Function GetHashCode() As Integer
            Const Prime As Integer = 31
            Return Prime * X.GetHashCode() + Y.GetHashCode()
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is Point Then
                Dim point = CType(obj, Point)
                Return (Me = point)
            End If
            Return False
        End Function
    End Structure
End Namespace