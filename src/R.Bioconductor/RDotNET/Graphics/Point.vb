Imports System

Namespace Graphics
    Public Structure Point
        Implements IEquatable(Of Point)

        Private xField As Double
        Private yField As Double

        Public Sub New(ByVal x As Double, ByVal y As Double)
            xField = x
            yField = y
        End Sub

        Public Property X As Double
            Get
                Return xField
            End Get
            Set(ByVal value As Double)
                xField = value
            End Set
        End Property

        Public Property Y As Double
            Get
                Return yField
            End Get
            Set(ByVal value As Double)
                yField = value
            End Set
        End Property

#Region "IEquatable<Point> Members"

        Public Overloads Function Equals(ByVal other As Point) As Boolean Implements IEquatable(Of Point).Equals
            Return Me = other
        End Function

#End Region

        Public Shared Operator =(ByVal p1 As Point, ByVal p2 As Point) As Boolean
            Return p1.X = p2.X AndAlso p1.Y = p2.Y
        End Operator

        Public Shared Operator <>(ByVal p1 As Point, ByVal p2 As Point) As Boolean
            Return Not p1 = p2
        End Operator

        Public Overrides Function GetHashCode() As Integer
            Const Prime = 31
            Return Prime * X.GetHashCode() + Y.GetHashCode()
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If TypeOf obj Is Point Then
                Dim point = CType(obj, Point)
                Return Me = point
            End If

            Return False
        End Function
    End Structure
End Namespace
