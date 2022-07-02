Public Module NetworkByteOrderBitConvertor

    Dim f64Bytes As Func(Of Double, Byte())
    Dim f64 As Func(Of Byte(), Integer, Double)

    Dim f32Bytes As Func(Of Single, Byte())
    Dim f32 As Func(Of Byte(), Integer, Single)

    Dim i64Bytes As Func(Of Long, Byte())
    Dim i64 As Func(Of Byte(), Integer, Long)

    Dim i32Bytes As Func(Of Integer, Byte())
    Dim i32 As Func(Of Byte(), Integer, Integer)

    Sub New()
        f64 = getf64()
        f64Bytes = f64Bits()
    End Sub

    Private Function getf64() As Func(Of Byte(), Integer, Double)
        If BitConverter.IsLittleEndian Then
            Return Function(bytes, offset)
                       Call Array.Reverse(bytes)
                       Return BitConverter.ToDouble(bytes, offset)
                   End Function
        Else
            Return AddressOf BitConverter.ToDouble
        End If
    End Function

    Private Function f64Bits() As Func(Of Double, Byte())
        If BitConverter.IsLittleEndian Then
            Return Function(d)
                       Dim chunk As Byte() = BitConverter.GetBytes(d)
                       Call Array.Reverse(chunk)
                       Return chunk
                   End Function
        Else
            Return AddressOf BitConverter.GetBytes
        End If
    End Function

    ''' <summary>
    ''' Returns the specified double-precision floating-point value as an array of bytes.
    ''' </summary>
    ''' <param name="value">The number to convert.</param>
    ''' <returns>An array of bytes with length 8.</returns>
    Public Function GetBytes(value As Double) As Byte()
        Return f64Bytes(value)
    End Function

    ''' <summary>
    ''' Returns a double-precision floating point number converted from eight bytes at
    ''' a specified position in a byte array.
    ''' </summary>
    ''' <param name="value">An array of bytes that includes the eight bytes to convert.</param>
    ''' <param name="startIndex">The starting position within value.</param>
    ''' <returns>A double-precision floating point number formed by eight bytes beginning at startIndex.</returns>
    Public Function ToDouble(value() As Byte, startIndex As Integer) As Double
        Return f64(value, startIndex)
    End Function

End Module
