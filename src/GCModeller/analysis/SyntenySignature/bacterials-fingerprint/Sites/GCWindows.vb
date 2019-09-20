#Region "Microsoft.VisualBasic::4d11d4c73218e9275733beb4ab919798, analysis\SyntenySignature\bacterials-fingerprint\Sites\GCWindows.vb"

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

    ' Module GCWindows
    ' 
    '     Function: GetWindows
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

''' <summary>
''' Build the sampling windows by using GC% or GC skew.
''' </summary>
Public Module GCWindows

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="nt"></param>
    ''' <param name="slideWin"></param>
    ''' <param name="steps"></param>
    ''' <param name="[property]"><see cref="GCSkew"/> or <see cref="GCContent"/> or your custom engine.</param>
    ''' <returns></returns>
    Public Iterator Function GetWindows(nt As FastaSeq, slideWin As Integer, steps As Integer, Optional [property] As NtProperty = Nothing) As IEnumerable(Of NucleotideLocation)
        Dim gc As Double() = If([property] Is Nothing, New NtProperty(AddressOf GCSkew), [property])(nt, slideWin, steps, True)
        Dim avg As Double = gc.Average
        Dim pdelta As Double = (gc.Max - gc.Min) / 10
        Dim range As New DoubleRange(avg - pdelta, avg + pdelta)
        Dim pre As Double = -1
        Dim left As Integer = 0
        Dim right As Integer

        For Each win In gc.SlideWindows(slideWin / 2, steps / 2)
            Dim d As Double = win.Average

            If Not range.IsInside(d) Then
                If Math.Sign(d - avg) = Math.Sign(pre - avg) Then  ' 任然是在一起的，因为方向相同并且大于阈值
                    right += (steps / 2) * steps
                Else
                    ' 方向发生了变换
                    Yield New NucleotideLocation(left, right)
                    left = right
                End If

                pre = d
            Else
                right += (steps / 2) * steps
                Yield New NucleotideLocation(left, right)
                left = right
            End If
        Next
    End Function
End Module
