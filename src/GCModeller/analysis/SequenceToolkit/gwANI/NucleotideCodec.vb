#Region "Microsoft.VisualBasic::3c5d7e9f1a2b4c6d8e0f2a4b6c8d0e2f, analysis\SequenceToolkit\gwANI\NucleotideCodec.vb"

' Author:
'
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
'
' Copyright (c) 2018 GPL3 Licensed
'
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

Option Strict On

Imports System.Runtime.CompilerServices


    ''' <summary>
    ''' The character to byte nucleotide codec of the gwANI matrix.
    ''' </summary>
    ''' <remarks>
    ''' The encoding keeps the semantics of the original panito tool bit by bit:
    ''' the unknown bases(<c>N</c>, <c>n</c>, <c>-</c>, <c>?</c>) collapse into the single
    ''' sentinel value <see cref="Unknown"/> so that a vectorized comparison can test
    ''' them with one <c>CompareEqual</c>, while every other character keeps its upper
    ''' case ASCII code so that any two distinct symbols still mismatch each other.
    ''' </remarks>
    Public NotInheritable Class NucleotideCodec

        ''' <summary>
        ''' The unknown base sentinel. It never collides with an upper case ASCII letter,
        ''' therefore "either side is unknown" reduces to "either side equals this value".
        ''' </summary>
        Public Const Unknown As Byte = 255

        ''' <summary>
        ''' The SIMD vector width in bytes(AVX2). Every encoded row is padded up to a
        ''' multiple of this value so that the vectorized kernel never reads out of
        ''' bounds and never needs a scalar tail loop for the aligned part.
        ''' </summary>
        Public Const VectorWidth As Integer = 32

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Encodes a single nucleotide character.
        ''' </summary>
        ''' <param name="symbol"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function EncodeBase(symbol As Char) As Byte
            If IsUnknown(symbol) <> 0 Then
                Return Unknown
            End If

            Dim code As Integer = AscW(Char.ToUpper(symbol))

            ' non ASCII symbols are treated as unknown, they never appear inside
            ' a nucleotide alignment but the encoding has to stay total.
            If code > 254 Then
                Return Unknown
            Else
                Return CByte(code)
            End If
        End Function

        ''' <summary>
        ''' Rounds an alignment length up to the next multiple of <see cref="VectorWidth"/>.
        ''' </summary>
        ''' <param name="alignmentLength"></param>
        ''' <returns></returns>
        Public Shared Function PaddedLength(alignmentLength As Integer) As Integer
            If alignmentLength <= 0 Then
                Return 0
            Else
                ' the Visual Basic integer division operator has a lower precedence
                ' than the multiplication, so the rounding has to be parenthesized
                Return ((alignmentLength + VectorWidth - 1) \ VectorWidth) * VectorWidth
            End If
        End Function

        ''' <summary>
        ''' Encodes a sequence string into a padded row that is filled with the
        ''' <see cref="Unknown"/> sentinel beyond the end of the sequence.
        ''' </summary>
        ''' <param name="sequence"></param>
        ''' <returns></returns>
        Public Shared Function EncodeRow(sequence As String) As Byte()
            Dim length As Integer = If(sequence Is Nothing, 0, sequence.Length)
            Dim padded As Integer = PaddedLength(length)
            Dim row As Byte() = New Byte(padded - 1) {}

            If length > 0 Then
                For i As Integer = 0 To length - 1
                    row(i) = EncodeBase(sequence(i))
                Next
            End If

            For i As Integer = length To padded - 1
                row(i) = Unknown
            Next

            Return row
        End Function

    End Class


#End Region
