#Region "Microsoft.VisualBasic::976c780f19c8c6df1fa882de2591260b, WebCloud\SMRUCC.WebCloud.QRCode\BitArrayHelpers.vb"

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

    ' Module BitArrayHelpers
    ' 
    '     Function: (+2 Overloads) ToBitArray, ToBitString, ToByteArray
    ' 
    ' /********************************************************************************/

#End Region

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
