Imports System
Imports System.Runtime.InteropServices

Namespace Graphics
    ''' <summary>
    ''' 32-bit color of ABGR model.
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)>
    Public Structure Color
        Implements IEquatable(Of Color)

        Private redField As Byte
        Private greenField As Byte
        Private blueField As Byte
        Private alphaField As Byte

        ''' <summary>
        ''' Gets and sets the alpha value.
        ''' </summary>
        Public Property Alpha As Byte
            Get
                Return alphaField
            End Get
            Set(ByVal value As Byte)
                alphaField = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the opaque value.
        ''' </summary>
        Public ReadOnly Property Opaque As Byte
            Get
                Return Byte.MaxValue - alphaField
            End Get
        End Property

        ''' <summary>
        ''' Gets and sets the red value.
        ''' </summary>
        Public Property Red As Byte
            Get
                Return redField
            End Get
            Set(ByVal value As Byte)
                redField = value
            End Set
        End Property

        ''' <summary>
        ''' Gets and sets the green value.
        ''' </summary>
        Public Property Green As Byte
            Get
                Return greenField
            End Get
            Set(ByVal value As Byte)
                greenField = value
            End Set
        End Property

        ''' <summary>
        ''' Gets and sets the blue value.
        ''' </summary>
        Public Property Blue As Byte
            Get
                Return blueField
            End Get
            Set(ByVal value As Byte)
                blueField = value
            End Set
        End Property

        ''' <summary>
        ''' Gets whether the point is transparent.
        ''' </summary>
        Public ReadOnly Property IsTransparent As Boolean
            Get
                Return alphaField = 0
            End Get
        End Property

        ''' <summary>
        ''' Gets a color from 32-bit value.
        ''' </summary>
        ''' <param name="rgba">UInt32.</param>
        ''' <returns>The color.</returns>
        Public Shared Function FromUInt32(ByVal rgba As UInteger) As Color
            Dim color = New Color()
            color.alphaField = CByte((rgba And &HFF000000UI) >> 24)
            color.blueField = CByte((rgba And &HFF0000UI) >> 16)
            color.greenField = CByte((rgba And &HFF00UI) >> 8)
            color.redField = CByte(rgba And &HFFUI)
            Return color
        End Function

        ''' <summary>
        ''' Gets a color from bytes.
        ''' </summary>
        ''' <param name="red">Red.</param>
        ''' <param name="green">Green.</param>
        ''' <param name="blue">Blue.</param>
        ''' <returns>The color.</returns>
        Public Shared Function FromRgb(ByVal red As Byte, ByVal green As Byte, ByVal blue As Byte) As Color
            Dim color = New Color()
            color.alphaField = Byte.MaxValue
            color.blueField = blue
            color.greenField = green
            color.redField = red
            Return color
        End Function

        ''' <summary>
        ''' Gets a color from bytes.
        ''' </summary>
        ''' <param name="alpha">Alpha.</param>
        ''' <param name="red">Red.</param>
        ''' <param name="green">Green.</param>
        ''' <param name="blue">Blue.</param>
        ''' <returns>The color.</returns>
        Public Shared Function FromArgb(ByVal alpha As Byte, ByVal red As Byte, ByVal green As Byte, ByVal blue As Byte) As Color
            Dim color = New Color()
            color.alphaField = alpha
            color.blueField = blue
            color.greenField = green
            color.redField = red
            Return color
        End Function

        Public Shared Operator =(ByVal c1 As Color, ByVal c2 As Color) As Boolean
            Return c1.Alpha = c2.Alpha AndAlso c1.Blue = c2.Blue AndAlso c1.Green = c2.Green AndAlso c1.Red = c2.Red
        End Operator

        Public Shared Operator <>(ByVal c1 As Color, ByVal c2 As Color) As Boolean
            Return Not c1 = c2
        End Operator

        Public Overrides Function GetHashCode() As Integer
            Return Alpha << 24 Or Blue << 16 Or Green << 8 Or Red
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If TypeOf obj Is Color Then
                Dim color = CType(obj, Color)
                Return Me = color
            End If

            Return False
        End Function

        Public Overloads Function Equals(ByVal other As Color) As Boolean Implements IEquatable(Of Color).Equals
            Return Me = other
        End Function
    End Structure
End Namespace
