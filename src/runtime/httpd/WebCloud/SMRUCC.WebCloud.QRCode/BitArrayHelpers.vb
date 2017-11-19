Imports System.Runtime.CompilerServices
Imports System.Text

''' <summary>
''' Helpers functions for BitArrays
''' </summary>
Public Module BitArrayHelpers

    <Extension>
    Public Function ToBitString(bits As BitArray) As String
        Dim sb As New StringBuilder()

        For i As Integer = 0 To bits.Length - 1
            sb.Append(If(bits(i), "1", "0"))
            If i Mod 8 = 7 Then
                sb.Append(" ")
            End If
        Next

        Return sb.ToString()
    End Function

    <Extension>
    Public Function ToByteArray(bits As BitArray) As Byte()
        Dim bytes As Byte() = New Byte((bits.Length - 1) \ 8) {}
        For b As Integer = 0 To bits.Length - 1
            If bits(b) Then
                bytes(b \ 8) = bytes(b \ 8) Or CByte(&H80 >> (b Mod 8))
            End If
        Next
        Return bytes
    End Function

    <Extension>
    Public Function ToBitArray(bytes As Byte()) As BitArray
        Dim b = New BitArray(8 * bytes.Length, False)

        For i As Integer = 0 To b.Length - 1
            If (bytes(i \ 8) And (&H80 >> (i Mod 8))) <> 0 Then
                b(i) = True
            End If
        Next

        Return b
    End Function

    <Extension>
    Public Function ToBitArray(x As Integer, bits As Integer) As BitArray
        Dim b = New BitArray(bits, False)
        For i As Integer = 0 To bits - 1
            If (x And ((1 << (bits - 1)) >> i)) <> 0 Then
                b(i) = True
            End If
        Next
        Return b
    End Function
End Module