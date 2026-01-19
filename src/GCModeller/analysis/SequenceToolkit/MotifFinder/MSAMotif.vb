#Region "Microsoft.VisualBasic::be2e81a9f5686d84b932468680716c5c, analysis\SequenceToolkit\MotifFinder\MSAMotif.vb"

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


' Code Statistics:

'   Total Lines: 19
'    Code Lines: 14 (73.68%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 5 (26.32%)
'     File Size: 500 B


' Class MSAMotif
' 
'     Properties: countMatrix, p, q, rowSum, score
'                 start
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.SequenceAlignment.MSA

''' <summary>
''' 
''' </summary>
''' <remarks>
''' a sub-class of <see cref="MSAOutput"/>.
''' </remarks>
Public Class MSAMotif : Inherits MSAOutput

    Public Property start As ints
    <XmlElement>
    Public Property countMatrix As ints()

    Public Property rowSum As Integer
    Public Property p As Double()
    Public Property q As Double()

    ''' <summary>
    ''' usually be the ACGT
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property alphabets As Char()

    Public ReadOnly Property score As Double()
        Get
            Return SIMD.Divide.f64_op_divide_f64(q, p)
        End Get
    End Property

    Public Iterator Function PWM() As IEnumerable(Of Residue)
        Dim i As i32 = 1

        For Each site As ints In countMatrix
            Dim p As New Dictionary(Of String, Double)
            Dim sum As Integer = site.ints.Sum
            Dim col As Double() = SIMD.Divide.int32_op_divide_int32_scalar(site.ints, sum)

            For j As Integer = 0 To alphabets.Length - 1
                p(alphabets(j).ToString) = col(j)
            Next

            Yield New Residue With {
                .index = ++i,
                .frequency = p
            }
        Next
    End Function

    Public Function CreateMotif() As MotifPWM
        Dim n As Integer = 100  ' normalized as 100 sequence input
        Dim E As Double = Probability.E(nsize:=n)

        Return New MotifPWM With {
            .name = "motif",
            .alphabets = alphabets,
            .note = names.JoinBy(", "),
            .pwm = countMatrix _
                .Select(Function(r, i)
                            Dim sum As Integer = r.ints.Sum
                            Dim col As Double() = SIMD.Divide.int32_op_divide_int32_scalar(r.ints, sum)
                            Dim Hi As Double = Probability.HI(col)
                            Dim bits As Double = Probability.CalculatesBits(Hi, E, NtMol:=alphabets.Length = 4)

                            Return New ResidueSite With {
                                .site = i + 1,
                                .PWM = col,
                                .bits = bits
                            }
                        End Function) _
                .ToArray
        }
    End Function

End Class
