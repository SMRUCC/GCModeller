
Namespace Graphics

    Public Structure Size
        Implements IEquatable(Of Size)
        Private m_height As Double
        Private m_width As Double

        Public Sub New(width As Double, height As Double)
            Me.m_width = width
            Me.m_height = height
        End Sub

        Public Property Width() As Double
            Get
                Return Me.m_width
            End Get
            Set(value As Double)
                Me.m_width = Value
            End Set
        End Property

        Public Property Height() As Double
            Get
                Return Me.m_height
            End Get
            Set(value As Double)
                Me.m_height = Value
            End Set
        End Property

#Region "IEquatable<Size> Members"

        Public Overloads Function Equals(other As Size) As Boolean Implements IEquatable(Of RDotNet.Graphics.Size).Equals
            Return (Me = other)
        End Function

#End Region

        Public Shared Operator =(size1 As Size, size2 As Size) As Boolean
            Return size1.Width = size2.Width AndAlso size1.Height = size2.Height
        End Operator

        Public Shared Operator <>(size1 As Size, size2 As Size) As Boolean
            Return Not (size1 = size2)
        End Operator

        Public Overrides Function GetHashCode() As Integer
            Const Prime As Integer = 31
            Return Prime * Width.GetHashCode() + Height.GetHashCode()
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is Size Then
                Dim size = CType(obj, Size)
                Return (Me = size)
            End If
            Return False
        End Function
    End Structure
End Namespace