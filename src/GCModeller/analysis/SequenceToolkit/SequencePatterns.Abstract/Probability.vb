#Region "Microsoft.VisualBasic::ea93b8d10bf0c55326bdfeeb2b1b91ee, analysis\SequenceToolkit\SequencePatterns.Abstract\Probability.vb"

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

' Class Probability
' 
'     Properties: pvalue, region, score
' 
'     Function: patternString, ToString
'     Structure Residue
' 
'         Properties: frequency, index, isEmpty, topChar
' 
'         Function: Cos, GetEmpty, Max, ToString
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.SequenceModel.Patterns

''' <summary>
''' The PWM model
''' </summary>
Public Class Probability

    Public Property region As Residue()
    Public Property pvalue As Double
    Public Property score As Double

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return patternString() & $" @ {score}, pvalue={pvalue.ToString("G4")}"
    End Function

    Public Function patternString() As String
        Return region _
           .Select(Function(r) r.ToString) _
           .JoinBy("")
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="nsize">
    ''' the count of the fasta sequence.
    ''' </param>
    ''' <returns></returns>
    Public Shared Function E(nsize As Integer) As Double
        Return (1 / Math.Log(2)) * ((4 - 1) / (2 * nsize))
    End Function

    ''' <summary>
    ''' The information content (y-axis) of position i is given by:
    ''' 
    ''' ```
    ''' Ri = log2(4) - (Hi + en)   //nt
    ''' Ri = log2(20) - (Hi + en)  //prot 
    ''' ```
    ''' 
    ''' 4 for DNA/RNA or 20 for protein. Consequently, the maximum sequence conservation 
    ''' per site Is log2 4 = 2 bits for DNA/RNA And log2 20 ≈ 4.32 bits for proteins.
    ''' 
    ''' </summary>
    ''' <param name="En"></param>
    ''' <returns></returns>
    Public Shared Function CalculatesBits(Hi As Double, En As Double, NtMol As Boolean) As Double
        '  Math.Log(n, 2) - (h + en)
        Dim n As Double = If(NtMol, 2, Math.Log(20, newBase:=2))
        Dim bits = n - (Hi + En)

        Return bits
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
    Public Shared Function HI(f As Dictionary(Of Char, Double)) As Double
        ' 零乘以任何数都是得结果零
        Dim h As Double = f.Values.Sum(Function(n) If(n = 0R, 0, n * Math.Log(n, 2)))
        h = 0 - h
        Return h
    End Function

    Public Shared Function HI(f As IPatternSite) As Double
        Dim h As Double = f.EnumerateValues.Sum(Function(n) If(n = 0R, 0, n * Math.Log(n, 2)))
        h = 0 - h
        Return h
    End Function
End Class
