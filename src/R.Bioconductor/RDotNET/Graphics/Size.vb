Imports System

Namespace Graphics
    Public Structure Size
        Implements IEquatable(Of Size)

        Private heightField As Double
        Private widthField As Double

        Public Sub New(ByVal width As Double, ByVal height As Double)
            widthField = width
            heightField = height
        End Sub

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

#Region "IEquatable<Size> Members"

        Public Overloads Function Equals(ByVal other As Size) As Boolean Implements IEquatable(Of Size).Equals
            Return Me = other
        End Function

#End Region

        Public Shared Operator =(ByVal size1 As Size, ByVal size2 As Size) As Boolean
            Return size1.Width = size2.Width AndAlso size1.Height = size2.Height
        End Operator

        Public Shared Operator <>(ByVal size1 As Size, ByVal size2 As Size) As Boolean
            Return Not size1 = size2
        End Operator

        Public Overrides Function GetHashCode() As Integer
            Const Prime = 31
            Return Prime * Width.GetHashCode() + Height.GetHashCode()
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If TypeOf obj Is Size Then
                Dim size = CType(obj, Size)
                Return Me = size
            End If

            Return False
        End Function
    End Structure
End Namespace
