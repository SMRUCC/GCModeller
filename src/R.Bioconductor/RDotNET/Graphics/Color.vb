#Region "Microsoft.VisualBasic::d140315ef71b9d5910be3ecd2ce9d0b7, RDotNET\Graphics\Color.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Structure Color
    ' 
    '         Properties: Alpha, Blue, Green, IsTransparent, Opaque
    '                     Red
    ' 
    '         Function: (+2 Overloads) Equals, FromArgb, FromRgb, FromUInt32, GetHashCode
    '         Operators: <>, =
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices

Namespace Graphics

    ''' <summary>
    ''' 32-bit color of ABGR model.
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)>
    Public Structure Color : Implements IEquatable(Of Color)

        ''' <summary>
        ''' Gets and sets the alpha value.
        ''' </summary>
        Public Property Alpha() As Byte

        ''' <summary>
        ''' Gets the opaque value.
        ''' </summary>
        Public ReadOnly Property Opaque() As Byte
            Get
                Return CByte(Byte.MaxValue - _Alpha)
            End Get
        End Property

        ''' <summary>
        ''' Gets and sets the red value.
        ''' </summary>
        Public Property Red() As Byte

        ''' <summary>
        ''' Gets and sets the green value.
        ''' </summary>
        Public Property Green() As Byte

        ''' <summary>
        ''' Gets and sets the blue value.
        ''' </summary>
        Public Property Blue() As Byte

        ''' <summary>
        ''' Gets whether the point is transparent.
        ''' </summary>
        Public ReadOnly Property IsTransparent() As Boolean
            Get
                Return _Alpha = 0
            End Get
        End Property

        ''' <summary>
        ''' Gets a color from 32-bit value.
        ''' </summary>
        ''' <param name="rgba">UInt32.</param>
        ''' <returns>The color.</returns>
        Public Shared Function FromUInt32(rgba As UInteger) As Color
            Dim color = New Color()
            color.alpha = CByte((rgba And &HFF000000UI) >> 24)
            color.blue = CByte((rgba And &HFF0000UI) >> 16)
            color.green = CByte((rgba And &HFF00UI) >> 8)
            color.red = CByte(rgba And &HFFUI)
            Return color
        End Function

        ''' <summary>
        ''' Gets a color from bytes.
        ''' </summary>
        ''' <param name="red">Red.</param>
        ''' <param name="green">Green.</param>
        ''' <param name="blue">Blue.</param>
        ''' <returns>The color.</returns>
        Public Shared Function FromRgb(red As Byte, green As Byte, blue As Byte) As Color
            Dim color = New Color()
            color.alpha = Byte.MaxValue
            color.blue = blue
            color.green = green
            color.red = red
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
        Public Shared Function FromArgb(alpha As Byte, red As Byte, green As Byte, blue As Byte) As Color
            Dim color = New Color()
            color.alpha = alpha
            color.blue = blue
            color.green = green
            color.red = red
            Return color
        End Function

        Public Shared Operator =(c1 As Color, c2 As Color) As Boolean
            Return c1.Alpha = c2.Alpha AndAlso c1.Blue = c2.Blue AndAlso c1.Green = c2.Green AndAlso c1.Red = c2.Red
        End Operator

        Public Shared Operator <>(c1 As Color, c2 As Color) As Boolean
            Return Not (c1 = c2)
        End Operator

        Public Overrides Function GetHashCode() As Integer
            Return (Alpha << 24) Or (Blue << 16) Or (Green << 8) Or Red
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is Color Then
                Dim color = CType(obj, Color)
                Return (Me = color)
            End If
            Return False
        End Function

        Public Overloads Function Equals(other As Color) As Boolean Implements IEquatable(Of Color).Equals
            Return (Me = other)
        End Function
    End Structure
End Namespace
