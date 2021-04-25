﻿#Region "Microsoft.VisualBasic::9ae2f10099e1521e30cdf73b50c764d4, Microsoft.VisualBasic.Core\src\Extensions\Math\Math.vb"

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

    '     Module VBMath
    ' 
    '         Function: Covariance, CumSum, Distance, (+6 Overloads) EuclideanDistance, Factorial
    '                   FactorialSequence, FormatNumeric, Hypot, IsPowerOf2, (+2 Overloads) Log2
    '                   LogN, Max, Permut, PoissonPDF, Pow2
    '                   (+3 Overloads) ProductALL, (+3 Overloads) RangesAt, RMS, RMSE, RSD
    '                   (+4 Overloads) SD, (+2 Overloads) seq, (+5 Overloads) Sum, WeighedAverage
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports stdNum = System.Math

Namespace Math

    ''' <summary>
    ''' Provides constants and static methods for trigonometric, logarithmic, and other
    ''' common mathematical functions.To browse the .NET Framework source code for this
    ''' type, see the Reference Source.
    ''' </summary>
    <Package("VBMath", Publisher:="xie.guigang@gmail.com")>
    Public Module VBMath

        Public Function Permut(k As Integer, n As Integer) As Long
            Dim nfactors As Integer() = (n - 1).SeqIterator(offset:=1).ToArray
            Dim nkfactors As Integer() = (n - k - 1).SeqIterator(offset:=1).ToArray

            ' removes the same factor element
            Dim nf As Integer()
            Dim nk As Integer()

            With nfactors.Indexing
                nk = nkfactors.Where(Function(x) .IndexOf(x:=x) = -1).ToArray
            End With
            With nkfactors.Indexing
                nf = nfactors.Where(Function(x) .IndexOf(x:=x) = -1).ToArray
            End With

            Dim i As Long = 1

            For Each x In nf
                i = i * x
            Next

            Dim j As Long = 1

            For Each x In nk
                j = j * x
            Next

            Return i / j
        End Function

        ''' <summary>
        ''' ``Math.Log(x, newBase:=2)``
        ''' </summary>
        ''' <param name="x#"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function Log2(x#) As Double
            Return stdNum.Log(x, newBase:=2)
        End Function

        <Extension>
        Public Iterator Function CumSum(vector As IEnumerable(Of Double)) As IEnumerable(Of Double)
            Dim sum#

            For Each x As Double In vector
                sum += x
                Yield sum
            Next
        End Function

        ''' <summary>
        ''' 阶乘
        ''' </summary>
        ''' <param name="a"></param>
        ''' <returns></returns>
        Public Function Factorial(a As Integer) As Double
            If a <= 1 Then
                Return 1
            Else
                Dim n As Double = a

                For i As Integer = a - 1 To 1 Step -1
                    n *= i
                Next

                Return n
            End If
        End Function

        Public Iterator Function FactorialSequence(a As Integer) As IEnumerable(Of Integer)
            If a <= 1 Then
                Yield 1
            Else
                For i As Integer = a To 1 Step -1
                    Yield i
                Next
            End If
        End Function

        ''' <summary>
        ''' Returns the covariance of two data vectors. </summary>
        ''' <param name="a">	double[] of data </param>
        ''' <param name="b">	double[] of data
        ''' @return	the covariance of a and b, cov(a,b) </param>
        Public Function Covariance(a As Double(), b As Double()) As Double
            If a.Length <> b.Length Then
                Throw New ArgumentException("Cannot take covariance of different dimension vectors.")
            End If

            Dim divisor As Double = a.Length - 1
            Dim sum As Double = 0
            Dim aMean As Double = a.Average
            Dim bMean As Double = b.Average

            For i As Integer = 0 To a.Length - 1
                sum += (a(i) - aMean) * (b(i) - bMean)
            Next

            Return sum / divisor
        End Function

        ''' <summary>
        ''' 请注意,<paramref name="data"/>的元素数量必须要和<paramref name="weights"/>的长度相等
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="weights">这个数组里面的值的和必须要等于1</param>
        ''' <returns></returns>
        <Extension>
        Public Function WeighedAverage(data As IEnumerable(Of Double), ParamArray weights As Double()) As Double
            Dim avg#

            For Each x As SeqValue(Of Double) In data.SeqIterator
                avg += (x.value * weights(x))
            Next

            Return avg
        End Function

        ''' <summary>
        ''' [Sequence Generation] Generate regular sequences. seq is a standard generic with a default method.
        ''' </summary>
        ''' <param name="From">
        ''' the starting and (maximal) end values of the sequence. Of length 1 unless just from is supplied as an unnamed argument.
        ''' </param>
        ''' <param name="To">
        ''' the starting and (maximal) end values of the sequence. Of length 1 unless just from is supplied as an unnamed argument.
        ''' </param>
        ''' <param name="By">number: increment of the sequence</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Public Iterator Function seq([from] As Value(Of Double), [to] As Double, Optional by As Double = 0.1) As IEnumerable(Of Double)
            Yield from

            Do While (from = from.Value + by) <= [to]
                Yield from
            Loop
        End Function

        <Extension>
        Public Iterator Function seq(range As DoubleRange, Optional steps# = 0.1) As IEnumerable(Of Double)
            For Each x# In seq(range.Min, range.Max, steps)
                Yield x#
            Next
        End Function

        ''' <summary>
        ''' 以 N 为底的对数 ``LogN(X) = Log(X) / Log(N)`` 
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="N"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LogN(x As Double, N As Double) As Double
            Return stdNum.Log(x) / stdNum.Log(N)
        End Function

        ''' <summary>
        ''' return the maximum of a, b and c </summary>
        ''' <param name="a"> </param>
        ''' <param name="b"> </param>
        ''' <param name="c">
        ''' @return </param>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Max(a As Integer, b As Integer, c As Integer) As Integer
            Return stdNum.Max(a, stdNum.Max(b, c))
        End Function

        ''' <summary>
        '''  sqrt(a^2 + b^2) without under/overflow.
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>

        Public Function Hypot(a As Double, b As Double) As Double
            Dim r As Double

            If stdNum.Abs(a) > stdNum.Abs(b) Then
                r = b / a
                r = stdNum.Abs(a) * stdNum.Sqrt(1 + r * r)
            ElseIf b <> 0 Then
                r = a / b
                r = stdNum.Abs(b) * stdNum.Sqrt(1 + r * r)
            Else
                r = 0.0
            End If

            Return r
        End Function

        ''' <summary>
        ''' Calculates power of 2.
        ''' </summary>
        ''' 
        ''' <param name="power">Power to raise in.</param>
        ''' 
        ''' <returns>Returns specified power of 2 in the case if power is in the range of
        ''' [0, 30]. Otherwise returns 0.</returns>
        ''' 
        Public Function Pow2(power As Integer) As Integer
            Return If(((power >= 0) AndAlso (power <= 30)), (1 << power), 0)
        End Function

        ''' <summary>
        ''' Get base of binary logarithm.
        ''' </summary>
        ''' 
        ''' <param name="x">Source integer number.</param>
        ''' 
        ''' <returns>Power of the number (base of binary logarithm).</returns>
        ''' 
        Public Function Log2(x As Integer) As Integer
            If x <= 65536 Then
                If x <= 256 Then
                    If x <= 16 Then
                        If x <= 4 Then
                            If x <= 2 Then
                                If x <= 1 Then
                                    Return 0
                                End If
                                Return 1
                            End If
                            Return 2
                        End If
                        If x <= 8 Then
                            Return 3
                        End If
                        Return 4
                    End If
                    If x <= 64 Then
                        If x <= 32 Then
                            Return 5
                        End If
                        Return 6
                    End If
                    If x <= 128 Then
                        Return 7
                    End If
                    Return 8
                End If
                If x <= 4096 Then
                    If x <= 1024 Then
                        If x <= 512 Then
                            Return 9
                        End If
                        Return 10
                    End If
                    If x <= 2048 Then
                        Return 11
                    End If
                    Return 12
                End If
                If x <= 16384 Then
                    If x <= 8192 Then
                        Return 13
                    End If
                    Return 14
                End If
                If x <= 32768 Then
                    Return 15
                End If
                Return 16
            End If

            If x <= 16777216 Then
                If x <= 1048576 Then
                    If x <= 262144 Then
                        If x <= 131072 Then
                            Return 17
                        End If
                        Return 18
                    End If
                    If x <= 524288 Then
                        Return 19
                    End If
                    Return 20
                End If
                If x <= 4194304 Then
                    If x <= 2097152 Then
                        Return 21
                    End If
                    Return 22
                End If
                If x <= 8388608 Then
                    Return 23
                End If
                Return 24
            End If
            If x <= 268435456 Then
                If x <= 67108864 Then
                    If x <= 33554432 Then
                        Return 25
                    End If
                    Return 26
                End If
                If x <= 134217728 Then
                    Return 27
                End If
                Return 28
            End If
            If x <= 1073741824 Then
                If x <= 536870912 Then
                    Return 29
                End If
                Return 30
            End If
            Return 31
        End Function

        ''' <summary>
        ''' Checks if the specified integer is power of 2.
        ''' </summary>
        ''' 
        ''' <param name="x">Integer number to check.</param>
        ''' 
        ''' <returns>Returns <b>true</b> if the specified number is power of 2.
        ''' Otherwise returns <b>false</b>.</returns>
        ''' 
        <Extension>
        Public Function IsPowerOf2(x As Integer) As Boolean
            Return If((x > 0), ((x And (x - 1)) = 0), False)
        End Function

        ''' <summary>
        ''' Logical true values are regarded as one, false values as zero. For historical reasons, NULL is accepted and treated as if it were integer(0).
        ''' </summary>
        ''' <param name="bc"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Sum")>
        <Extension> Public Function Sum(bc As IEnumerable(Of Boolean)) As Double
            If bc Is Nothing Then
                Return 0
            Else
                Return bc _
                    .Select(Function(b) If(True = b, 1.0R, 0R)) _
                    .Sum
            End If
        End Function

#If NET_48 = 1 Or netcore5 = 1 Then

#Region "Sum all tuple members"

        ''' <summary>
        ''' Sum all tuple members
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Sum(t As ValueTuple(Of Double, Double)) As Double
            Return t.Item1 + t.Item2
        End Function

        ''' <summary>
        ''' Sum all tuple members
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Sum(t As ValueTuple(Of Double, Double, Double)) As Double
            Return t.Item1 + t.Item2 + t.Item3
        End Function

        ''' <summary>
        ''' Sum all tuple members
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Sum(t As ValueTuple(Of Double, Double, Double, Double)) As Double
            Return t.Item1 + t.Item2 + t.Item3 + t.Item4
        End Function

        ''' <summary>
        ''' Sum all tuple members
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Sum(t As ValueTuple(Of Double, Double, Double, Double, Double)) As Double
            Return t.Item1 + t.Item2 + t.Item3 + t.Item4 + t.Item5
        End Function
#End Region

#End If

        ''' <summary>
        ''' 计算出所有的数的乘积
        ''' </summary>
        ''' <param name="[in]"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ProductALL([in] As IEnumerable(Of Double)) As Double
            Dim product# = 1

            ' 因为会存在 0 * Inf = NaN
            ' 所以在下面做了一下if判断来避免出现这种情况的NaN值

            For Each x As Double In [in]
                ' 0乘上任何数应该都是零来的
                If x = 0R Then
                    Return 0
                Else
                    product *= x
                End If
            Next

            Return product
        End Function

        ''' <summary>
        ''' 计算出所有的数的乘积
        ''' </summary>
        ''' <param name="[in]"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ProductALL([in] As IEnumerable(Of Integer)) As Double
            Return [in].Select(Function(x) CDbl(x)).ProductALL
        End Function

        ''' <summary>
        ''' 计算出所有的数的乘积
        ''' </summary>
        ''' <param name="[in]"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ProductALL([in] As IEnumerable(Of Long)) As Double
            Return [in].Select(Function(x) CDbl(x)).ProductALL
        End Function

        ''' <summary>
        ''' ## Standard Deviation
        ''' 
        ''' In statistics, the standard deviation (SD, also represented by the Greek letter sigma σ or the Latin letter s) 
        ''' is a measure that is used to quantify the amount of variation or dispersion of a set of data values. A low 
        ''' standard deviation indicates that the data points tend to be close to the mean (also called the expected value) 
        ''' of the set, while a high standard deviation indicates that the data points are spread out over a wider range of 
        ''' values.
        ''' 
        ''' > https://en.wikipedia.org/wiki/Standard_deviation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Public Function SD(values As IEnumerable(Of Double), Optional isSample As Boolean = False) As Double
            Dim data#() = values.ToArray
            Dim avg# = data.Average
            Dim sumValue# = Aggregate n As Double
                            In data
                            Into Sum((n - avg) ^ 2)

            If isSample Then
                Return stdNum.Sqrt(sumValue / (data.Length - 1))
            Else
                Return stdNum.Sqrt(sumValue / data.Length)
            End If
        End Function

        ''' <summary>
        ''' Standard Deviation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension> Public Function SD(values As IEnumerable(Of Integer)) As Double
            Return values.Select(Function(x) CDbl(x)).SD
        End Function

        ''' <summary>
        ''' Standard Deviation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension> Public Function SD(values As IEnumerable(Of Long)) As Double
            Return values.Select(Function(x) CDbl(x)).SD
        End Function

        ''' <summary>
        ''' Standard Deviation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension> Public Function SD(values As IEnumerable(Of Single)) As Double
            Return values.Select(Function(x) CDbl(x)).SD
        End Function

        ''' <summary>
        ''' 多位坐标的欧几里得距离，与坐标点0进行比较
        ''' </summary>
        ''' <param name="vector"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function EuclideanDistance(vector As IEnumerable(Of Double)) As Double
            ' 由于是和令进行比较，减零仍然为原来的数，所以这里直接使用n^2了
            Return stdNum.Sqrt((From n In vector Select n ^ 2).Sum)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function EuclideanDistance(Vector As IEnumerable(Of Integer)) As Double
            Return stdNum.Sqrt((From n In Vector Select n ^ 2).Sum)
        End Function

        <Extension>
        Public Function EuclideanDistance(a As IEnumerable(Of Integer), b As IEnumerable(Of Integer)) As Double
            If a.Count <> b.Count Then
                Return -1
            Else
                Return stdNum.Sqrt((From i As Integer In a.Sequence Select (a(i) - b(i)) ^ 2).Sum)
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function EuclideanDistance(a As IEnumerable(Of Double), b As IEnumerable(Of Double)) As Double
            Return EuclideanDistance(a.ToArray, b.ToArray)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="a">Point A</param>
        ''' <param name="b">Point B</param>
        ''' <returns></returns>
        <Extension>
        Public Function EuclideanDistance(a As Byte(), b As Byte()) As Double
            If a.Length <> b.Length Then
                Return -1.0R
            Else
                Return stdNum.Sqrt((From i As Integer In a.Sequence Select (CInt(a(i)) - CInt(b(i))) ^ 2).Sum)
            End If
        End Function

        ''' <summary>
        ''' 计算两个向量之间的欧氏距离，请注意，这两个向量的长度必须要相等
        ''' </summary>
        ''' <param name="a">Point A</param>
        ''' <param name="b">Point B</param>
        ''' <returns></returns>
        <Extension>
        Public Function EuclideanDistance(a As Double(), b As Double()) As Double
            If a.Length <> b.Length Then
                Return -1.0R
            Else
                Return stdNum.Sqrt((From i As Integer In a.Sequence Select (a(i) - b(i)) ^ 2).Sum)
            End If
        End Function

#If NET_48 = 1 Or netcore5 = 1 Then

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Distance(pt As (X#, Y#), x#, y#) As Double
            Return {pt.X, pt.Y}.EuclideanDistance({x, y})
        End Function

#End If

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <ExportAPI("RangesAt")>
        <Extension> Public Function RangesAt(n As Double, LowerBound As Double, UpBound As Double) As Boolean
            Return n <= UpBound AndAlso n > LowerBound
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <ExportAPI("RangesAt")>
        <Extension> Public Function RangesAt(n As Integer, LowerBound As Double, UpBound As Double) As Boolean
            Return n <= UpBound AndAlso n > LowerBound
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <ExportAPI("RangesAt")>
        <Extension> Public Function RangesAt(n As Long, LowerBound As Double, UpBound As Double) As Boolean
            Return n <= UpBound AndAlso n > LowerBound
        End Function

        ''' <summary>
        ''' Root mean square.(均方根)
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <ExportAPI("RMS")>
        <Extension>
        Public Function RMS(data As IEnumerable(Of Double)) As Double
            With (From n In data Select n ^ 2).ToArray
                Return stdNum.Sqrt(.Sum / .Length)
            End With
        End Function

        Public Function RMSE(a#(), b#()) As Double
            Dim sum#
            Dim n% = a.Length

            For i As Integer = 0 To n - 1
                sum += (a(i) - b(i)) ^ 2
            Next

            Return stdNum.Sqrt(sum)
        End Function

        ''' <summary>
        ''' ``相对标准偏差（RSD）= 标准偏差（SD）/ 计算结果的算术平均值（X）* 100%``
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' RSD is also an alias of ``CV%``
        ''' </remarks>
        <Extension>
        Public Function RSD(data As IEnumerable(Of Double)) As Double
            Dim vec As Double() = data.ToArray
            Dim sd As Double = vec.SD

            If sd = 0.0 Then
                Return 0
            Else
                Return sd / vec.Average
            End If
        End Function

        ''' <summary>
        ''' Returns the PDF value at x for the specified Poisson distribution.
        ''' </summary>
        ''' 
        <ExportAPI("Poisson.PDF")>
        Public Function PoissonPDF(x As Integer, lambda As Double) As Double
            Dim result As Double = stdNum.Exp(-lambda)
            Dim k As Integer = x

            While k >= 1
                result *= lambda / k
                k -= 1
            End While

            Return result
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function FormatNumeric(v As IEnumerable(Of Double), Optional digitals% = 2) As String()
            Return v.Select(Function(x) x.ToString("F" & digitals)).ToArray
        End Function
    End Module
End Namespace
