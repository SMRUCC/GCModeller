#Region "Microsoft.VisualBasic::8853f9fa41986a52908d1f70fbe921c8, analysis\SequenceToolkit\SequenceLogo\SequenceLogo\Residue.vb"

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

    '     Class Residue
    ' 
    '         Properties: Alphabets, AsChar, Bits, Position
    ' 
    '         Function: CalculatesBits, Hi, ToString
    ' 
    '         Sub: Assign
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.Information

Namespace SequenceLogo

    ''' <summary>
    ''' A drawing site in the sequence logo drawing.(所绘制的序列logo图之中的一个位点)
    ''' </summary>
    Public Class Residue : Implements IAddressOf

        ''' <summary>
        ''' ``ATGC``, 4 characters for ``nt``, and ``aa`` is 20.
        ''' </summary>
        ''' <returns></returns>
        Public Property Alphabets As Alphabet()
        ''' <summary>
        ''' The total height of the letters depicts the information content Of the position, 
        ''' In bits.
        ''' (Bits的值是和比对的序列的数量是有关系的)
        ''' </summary>
        ''' <returns></returns>
        Public Property Bits As Double

        ''' <summary>
        ''' Position value of this residue in the motif sequence.(这个残基的位点编号)
        ''' </summary>
        ''' <returns></returns>
        Public Property Position As Integer Implements IAddressOf.Address

        Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            Me.Position = address
        End Sub

        ''' <summary>
        ''' Display this site as a single alphabet, and this property is used 
        ''' for generates the motif string.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AsChar As Char
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Dim maxInd As Integer = Alphabets.MaxIndex
                Dim c As Char
                Dim p# = Alphabets(maxInd).RelativeFrequency

                If Alphabets.Length = 4 Then
                    c = SequenceModel.NT(maxInd)
                Else
                    c = SequenceModel.AA(maxInd)
                End If

                Return Motif.ResidueSite.ToChar(c, p)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"[{Position}]{AsChar} {NameOf(Bits)}:= {Bits}"
        End Function

        ''' <summary>
        ''' ``Hi`` is the uncertainty (sometimes called the Shannon entropy) of position i
        ''' 
        ''' ``
        ''' Hi = - Σ(f(a,i) x log2(f(a,i))
        ''' ```
        ''' 
        ''' Here, ``f(a,i)`` is the relative frequency of base or amino acid a at position i 
        ''' (in this residue)
        ''' 
        ''' 但是频率是零的时候怎么处理？？？
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Hi() As Double
            Return Alphabets _
                .Select(Function(a) a.RelativeFrequency) _
                .ShannonEntropy()
        End Function
    End Class
End Namespace
