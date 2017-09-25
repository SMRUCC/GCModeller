#Region "Microsoft.VisualBasic::5df2d2a3beca1cd18e9491ecc69e88e0, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Motif\PWM.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Patterns

Namespace Motif

    Public Class MotifPWM : Inherits BaseClass
        Implements IIterator(Of ResidueSite)

        Public Property PWM As ResidueSite()
        Public Property Alphabets As Char()

        Public Iterator Function GetEnumerator() As IEnumerator(Of ResidueSite) Implements IIterator(Of ResidueSite).GetEnumerator
            For Each x As ResidueSite In PWM
                Yield x
            Next
        End Function

        Public Iterator Function IGetEnumerator() As IEnumerator Implements IIterator(Of ResidueSite).IGetEnumerator
            Yield GetEnumerator()
        End Function

        Public Shared Function NT_PWM(sites As IEnumerable(Of ResidueSite)) As MotifPWM
            Return New MotifPWM With {
                .Alphabets = ColorSchema.NT.ToArray,
                .PWM = sites.ToArray
            }
        End Function

        Public Shared Function AA_PWM(sites As IEnumerable(Of ResidueSite)) As MotifPWM
            Return New MotifPWM With {
                .Alphabets = ColorSchema.AA,
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
            Dim n As Integer = fa.NumberOfFasta
            Dim base As Integer = If(fa.First.IsProtSource, 20, 4)
            Dim E As Double = (1 / Math.Log(2)) * ((base - 1) / (2 * n))
            Dim H As Double() = f.Residues.ToArray(Function(x) x.Alphabets.__hi)
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
                alphabets = LinqAPI.Exec(Of Double) <= From c As Char In ColorSchema.AA Select f(c)
            End If

            Return New ResidueSite With {
                .Bits = R,
                .PWM = alphabets,
                .Site = i
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="f"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' If n equals ZERO, then log2(0) is NaN, n * Math.Log(n, 2) could not be measure,
        ''' due to the reason of ZERO multiple any number is ZERO, so that if n is ZERO, 
        ''' then set n * Math.Log(n, 2) its value to Zero directly.
        ''' </remarks>
        <Extension>
        Private Function __hi(f As Dictionary(Of Char, Double)) As Double
            Dim h As Double = f.Values.Sum(Function(n) If(n = 0R, 0, n * Math.Log(n, 2))) ' 零乘以任何数都是得结果零
            h = 0 - h
            Return h
        End Function
    End Module
End Namespace
