Imports System

Namespace Graphics
    Public Structure Rectangle
        Implements IEquatable(Of Rectangle)

        Private heightField As Double
        Private widthField As Double
        Private xField As Double
        Private yField As Double

        Public Sub New(ByVal x As Double, ByVal y As Double, ByVal width As Double, ByVal height As Double)
            xField = x
            yField = y
            widthField = width
            heightField = height
        End Sub

        Public Sub New(ByVal location As Point, ByVal size As Size)
            xField = location.X
            yField = location.Y
            widthField = size.Width
            heightField = size.Height
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

        Public Property Width As Double
            Get
                Return widthField
            End Get
            Set(ByVal value As Double)
                widthField = value
            End Set
        End Property

        Public Property Height As Double
            Get
                Return heightField
            End Get
            Set(ByVal value As Double)
                heightField = value
            End Set
        End Property

        Public ReadOnly Property Left As Double
            Get
                Return X
            End Get
        End Property

        Public ReadOnly Property Right As Double
            Get
                Return X + Width
            End Get
        End Property

        Public ReadOnly Property Bottom As Double
            Get
                Return Y
            End Get
        End Property

        Public ReadOnly Property Top As Double
            Get
                Return Y + Height
            End Get
        End Property

        Public Property Location As Point
            Get
                Return New Point(X, Y)
            End Get
            Set(ByVal value As Point)
                X = value.X
                Y = value.Y
            End Set
        End Property

        Public Property Size As Size
            Get
                Return New Size(Width, Height)
            End Get
            Set(ByVal value As Size)
                Width = value.Width
                Height = value.Height
            End Set
        End Property

#Region "IEquatable<Rectangle> Members"

        Public Overloads Function Equals(ByVal other As Rectangle) As Boolean Implements IEquatable(Of Rectangle).Equals
            Return Me = other
        End Function

#End Region

        Public Shared Operator =(ByVal r1 As Rectangle, ByVal r2 As Rectangle) As Boolean
            Return r1.Location = r2.Location AndAlso r1.Size = r2.Size
        End Operator

        Public Shared Operator <>(ByVal r1 As Rectangle, ByVal r2 As Rectangle) As Boolean
            Return Not r1 = r2
        End Operator

        Public Overrides Function GetHashCode() As Integer
            Return Location.GetHashCode() Xor Size.GetHashCode()
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If TypeOf obj Is Rectangle Then
                Dim rectangle = CType(obj, Rectangle)
                Return Me = rectangle
            End If

            Return False
        End Function
    End Structure
End Namespace
