#Region "Microsoft.VisualBasic::ab7fe717e41fa5a5c2d7b39965df098d, analysis\SequenceToolkit\SequencePatterns\Motif\PWM.vb"

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

    '     Class MotifPWM
    ' 
    '         Properties: Alphabets, PWM
    ' 
    '         Function: AA_PWM, NT_PWM
    ' 
    '     Module PWM
    ' 
    '         Function: __hi, __residue, FromMla
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Patterns

Namespace Motif

    Public Class MotifPWM

        Public Property PWM As ResidueSite()
        Public Property Alphabets As Char()

        Public Shared Function NT_PWM(sites As IEnumerable(Of ResidueSite)) As MotifPWM
            Return New MotifPWM With {
                .Alphabets = SequenceModel.NT.ToArray,
                .PWM = sites.ToArray
            }
        End Function

        Public Shared Function AA_PWM(sites As IEnumerable(Of ResidueSite)) As MotifPWM
            Return New MotifPWM With {
                .Alphabets = SequenceModel.AA,
                .PWM = sites.ToArray
            }
        End Function
    End Class

    ''' <summary>
    ''' Build probability matrix from clustal multiple sequence alignment.
    ''' </summary>
    Public Module PWM

        ''' <summary>
        ''' Build probability matrix from clustal multiple sequence alignment, this matrix model can be 
        ''' used for the downstream sequence logo drawing visualization.
        ''' (从Clustal比对结果之中生成PWM用于SequenceLogo的绘制)
        ''' </summary>
        ''' <param name="fa">A fasta sequence file from the clustal multiple sequence alignment.</param>
        ''' <returns></returns>
        Public Function FromMla(fa As FastaFile) As MotifPWM
            Dim f As PatternModel = PatternsAPI.Frequency(fa)
            Dim n As Integer = fa.Count
            Dim base As Integer = If(fa.First.IsProtSource, 20, 4)
            Dim E As Double = (1 / Math.Log(2)) * ((base - 1) / (2 * n))
            Dim H As Double() = f.Residues.Select(Function(x) Probability.HI(x.Alphabets)).ToArray
            Dim PWM As ResidueSite() =
                LinqAPI.Exec(Of SimpleSite, ResidueSite) _
               (f.Residues) <= Function(x, i) __residue(x.Alphabets, H(i), E, base, i)

            If base = 20 Then
                Return MotifPWM.AA_PWM(PWM)
            Else
                Return MotifPWM.NT_PWM(PWM)
            End If
        End Function

        ''' <summary>
        ''' Construct of the residue model in the PWM
        ''' </summary>
        ''' <param name="f">ATGC</param>
        ''' <param name="h"></param>
        ''' <param name="en"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Private Function __residue(f As Dictionary(Of Char, Double), h As Double, en As Double, n As Integer, i As Integer) As ResidueSite
            Dim R As Double = Math.Log(n, 2) - (h + en)
            Dim alphabets As Double()

            If n = 4 Then
                alphabets = {f("A"c), f("T"c), f("G"c), f("C"c)}
            Else
                alphabets = LinqAPI.Exec(Of Double) <= From c As Char In SequenceModel.AA Select f(c)
            End If

            Return New ResidueSite With {
                .Bits = R,
                .PWM = alphabets,
                .Site = i
            }
        End Function
    End Module
End Namespace
