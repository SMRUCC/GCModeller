
Namespace Graphics

    Public Structure Point
        Implements IEquatable(Of Point)
        Private m_x As Double
        Private m_y As Double

        Public Sub New(x As Double, y As Double)
            Me.m_x = x
            Me.m_y = y
        End Sub

        Public Property X() As Double
            Get
                Return Me.m_x
            End Get
            Set(value As Double)
                Me.m_x = Value
            End Set
        End Property

        Public Property Y() As Double
            Get
                Return Me.m_y
            End Get
            Set(value As Double)
                Me.m_y = Value
            End Set
        End Property

#Region "IEquatable<Point> Members"

        Public Overloads Function Equals(other As Point) As Boolean Implements IEquatable(Of RDotNet.Graphics.Point).Equals
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