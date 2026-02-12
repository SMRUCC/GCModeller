#Region "Microsoft.VisualBasic::5019dcba748908a216296427d1ff95e5, analysis\SequenceToolkit\SequencePatterns.Abstract\Probability.vb"

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

'   Total Lines: 82
'    Code Lines: 36 (43.90%)
' Comment Lines: 36 (43.90%)
'    - Xml Docs: 80.56%
' 
'   Blank Lines: 10 (12.20%)
'     File Size: 2.74 KB


' Class Probability
' 
'     Properties: pvalue, region, score
' 
'     Function: CalculatesBits, E, (+2 Overloads) HI, patternString, ToString
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.SequenceModel.Patterns
Imports std = System.Math

''' <summary>
''' The PWM model
''' </summary>
Public Class Probability : Implements INamedValue, IReadOnlyId

    ''' <summary>
    ''' the PWM matrix data
    ''' </summary>
    ''' <returns></returns>
    Public Property region As Residue()

    <XmlAttribute> Public Property pvalue As Double
    <XmlAttribute> Public Property score As Double

    ''' <summary>
    ''' the unique id/name reference of the motif PWM model
    ''' </summary>
    ''' <returns></returns>
    Public Property name As String Implements INamedValue.Key, IReadOnlyId.Identity

    ''' <summary>
    ''' motif residue number
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property width As Integer
        Get
            Return region.TryCount
        End Get
    End Property

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
    ''' 计算小样本校正项 e_n = 1 / (2 * ln(2) * n)，这个公式是用于对 Shannon 熵
    ''' 进行小样本校正（small-sample correction）的经典项，常用于 motif 分析中
    ''' 计算信息含量（Information Content）时，以减少因有限序列数量带来的偏差。
    ''' </summary>
    ''' <param name="nsize">
    ''' the count of the input fasta sequence.
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
    ''' <param name="En">e_n = \frac{1}{2 \ln(2) \cdot n}</param>
    ''' <param name="NtMol">
    ''' calculate for the nucleotide sequence model?
    ''' </param>
    ''' <returns></returns>
    Public Shared Function CalculatesBits(Hi As Double, En As Double, NtMol As Boolean) As Double
        ' Math.Log(n, 2) - (h + en)
        ' log2(4)=2, log2(20)≈4.32
        Dim maxInfo As Double = If(NtMol, 2, Math.Log(20, newBase:=2))
        Dim bits = maxInfo - (Hi + En)

        ' 信息含量不能为负（理论上最小为0）
        Return std.Max(0, bits)
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
        Dim h As Double = Aggregate n As Double
                          In f.Values
                          Into Sum(If(n = 0R, 0, n * Math.Log(n, 2)))
        h = 0 - h
        Return h
    End Function

    Public Shared Function HI(f As IPatternSite) As Double
        Dim h As Double = Aggregate n As Double
                          In f.EnumerateValues
                          Into Sum(If(n = 0R, 0, n * Math.Log(n, 2)))
        h = 0 - h
        Return h
    End Function

    Public Shared Function HI(col As IEnumerable(Of Double)) As Double
        Dim h As Double = Aggregate n As Double
                          In col
                          Into Sum(If(n = 0R, 0, n * Math.Log(n, 2)))
        h = 0 - h
        Return h
    End Function
End Class
