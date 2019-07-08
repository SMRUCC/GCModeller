﻿#Region "Microsoft.VisualBasic::afbe7a5576ae77967dee4d2f2ee66435, Data_science\Mathematica\Math\Math\Algebra\Vector\Class\Vector.vb"

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

    '     Class Vector
    ' 
    '         Properties: [Mod], Data, Inf, IsNumeric, NAN
    '                     Range, SumMagnitude, Unit, Zero
    ' 
    '         Constructor: (+8 Overloads) Sub New
    '         Function: Abs, CumSum, DotProduct, Ones, Order
    '                   Product, rand, ScaleToRange, slice, SumMagnitudes
    '                   (+2 Overloads) ToString
    '         Operators: (+4 Overloads) -, (+5 Overloads) *, (+3 Overloads) /, (+2 Overloads) ^, (+4 Overloads) +
    '                    <, (+3 Overloads) <=, (+2 Overloads) <>, (+2 Overloads) =, >
    '                    (+3 Overloads) >=, (+2 Overloads) Or, (+2 Overloads) Xor
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.Vectorization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SyntaxAPI.Vectors
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports sys = System.Math
Imports numpy = Microsoft.VisualBasic.Language.Python

Namespace LinearAlgebra

    ''' <summary>
    ''' Vector was inherits from type <see cref="List(Of Double)"/>
    ''' </summary>
    Public Class Vector : Inherits GenericVector(Of Double)
        Implements IVector

#Region "Properties"

#Region "Default values"
        ''' <summary>
        ''' <see cref="System.Double.NaN"/>
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property NAN As Vector
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New Vector({Double.NaN})
            End Get
        End Property

        ''' <summary>
        ''' <see cref="System.Double.PositiveInfinity"/>
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property Inf As Vector
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New Vector({Double.PositiveInfinity})
            End Get
        End Property

        ''' <summary>
        ''' Only one number in the vector and its value is ZERO
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property Zero(Optional [Dim] As Integer = 1) As Vector
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New Vector(Linq.Extensions.Repeats(0R, [Dim]))
            End Get
        End Property
#End Region

        ''' <summary>
        ''' <see cref="[Dim]"/>为1?即当前的向量对象是否是只包含有一个数字？
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsNumeric As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return [Dim] = 1
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="rangeExpression"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' syntax helper
        ''' </remarks>
        Default Public Overloads Property Item(rangeExpression As String) As Vector
            Get
                Return MyBase.Item(rangeExpression).AsVector
            End Get
            Set(value As Vector)
                MyBase.Item(rangeExpression) = value.AsList
            End Set
        End Property

        ''' <summary>
        ''' ``norm2()``
        ''' 
        ''' 向量模的平方，``||x||``是向量``x=(x1，x2，…，xp)``的欧几里得范数
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property [Mod] As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return (Me ^ 2).Sum
            End Get
        End Property

        ''' <summary>
        ''' ``norm()``
        ''' 
        ''' http://math.stackexchange.com/questions/440320/what-is-magnitude-of-sum-of-two-vector
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SumMagnitude As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return sys.Sqrt(Me.Mod)
            End Get
        End Property

        Public ReadOnly Property Unit As Vector
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Me / SumMagnitude
            End Get
        End Property

        ''' <summary>
        ''' ``[min, max]``
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Range As DoubleRange
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New DoubleRange(Me)
            End Get
        End Property

        Private ReadOnly Property Data As Double() Implements IVector.Data
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return buffer
            End Get
        End Property
#End Region

        ''' <summary>
        ''' Creates vector with <paramref name="m"/> element and init value set to zero
        ''' It creates a double-precision vector of the specified length with each element equal to 0.
        ''' </summary>
        ''' <param name="m"></param>
        ''' <remarks>
        ''' equality to R expression
        ''' 
        ''' ```R
        ''' numeric(<paramref name="m"/>)
        ''' ```
        ''' </remarks>
        Public Sub New(m As Integer)
            Call Me.New(0R, m)
        End Sub

        ''' <summary>
        ''' 创建一个空的向量，包含有零个元素
        ''' </summary>
        Public Sub New()
            Call MyBase.New()
        End Sub

        ''' <summary>
        ''' Creates vector with a specific value sequence.
        ''' </summary>
        ''' <param name="data"></param>
        Sub New(data As IEnumerable(Of Double))
            Call MyBase.New(data)
        End Sub

        Sub New(shorts As IEnumerable(Of Single))
            Call Me.New(shorts.Select(Function(x) CDbl(x)))
        End Sub

        ''' <summary>
        ''' Init with transform
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="apply"></param>
        Sub New(x As IEnumerable(Of Double), apply As Func(Of Double, Double))
            Me.New(x.Select(apply))
        End Sub

        Sub New(from#, to#, Optional by# = 0.01)
            Me.New(VBMath.seq(from, [to], by))
        End Sub

        Sub New(integers As IEnumerable(Of Integer))
            Me.New(integers.Select(Function(n) CDbl(n)))
        End Sub

        ''' <summary>
        ''' Creates vector with m element and init value specific by init parameter.
        ''' </summary>
        ''' <param name="init"></param>
        ''' <param name="m"></param>
        Sub New(init#, m%)
            Call MyBase.New(capacity:=m)

            For i As Integer = 0 To m - 1
                buffer(i) = init
            Next
        End Sub

#Region "Operators"
        ''' <summary>
        ''' 两个向量加法算符重载，分量分别相加
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Operator +(v1 As Vector, v2 As Vector) As Vector
            Dim N0 As Integer = v1.[Dim] ' 获取变量维数
            Dim v3 As New Vector(N0)

            For j As Integer = 0 To N0 - 1
                v3(j) = v1(j) + v2(j)
            Next

            Return v3
        End Operator

        ''' <summary>
        ''' 向量减法算符重载，分量分别相减
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Operator -(v1 As Vector, v2 As Vector) As Vector
            Dim N0 As Integer = v1.[Dim]    ' 获取变量维数
            Dim v3 As New Vector(N0)

            For j As Integer = 0 To N0 - 1
                v3(j) = v1(j) - v2(j)
            Next
            Return v3
        End Operator

        ''' <summary>
        ''' 向量乘法算符重载，分量分别相乘，相当于MATLAB中的``.*``算符
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Operator *(v1 As Vector, v2#()) As Vector
            Dim N0 As Integer = v1.[Dim]        '获取变量维数
            Dim v3 As New Vector(N0)

            For j As Integer = 0 To N0 - 1
                v3(j) = v1(j) * v2(j)
            Next

            Return v3
        End Operator

        ''' <summary>
        ''' 向量乘法算符重载，分量分别相乘，相当于MATLAB中的``.*``算符
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator *(v1 As Vector, v2 As Vector) As Vector
            Return v1 * v2.buffer
        End Operator

        ''' <summary>
        ''' 向量除法算符重载，分量分别相除，相当于MATLAB中的   ./算符
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator /(v1 As Vector, v2 As Vector) As Vector
            Dim N0 As Integer = v1.[Dim]         ' 获取变量维数
            Dim v3 As New Vector(N0)

            For j = 0 To N0 - 1
                v3(j) = v1(j) / v2(j)
            Next
            Return v3
        End Operator

        ''' <summary>
        ''' 向量减加实数，各分量分别加实数
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="a"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Operator +(v1 As Vector, a#) As Vector
            '向量数加算符重载
            Dim N0 As Integer = v1.[Dim]         ' 获取变量维数
            Dim v2 As New Vector(N0)

            For j = 0 To N0 - 1
                v2(j) = v1(j) + a
            Next
            Return v2
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator +(v1 As Vector, a As Int16) As Vector
            Return v1 + CDbl(a)
        End Operator

        ''' <summary>
        ''' 向量减实数，各分量分别减实数
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="a"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Operator -(v1 As Vector, a#) As Vector
            '向量数加算符重载
            Dim N0 As Integer = v1.[Dim] ' 获取变量维数
            Dim v2 As New Vector(N0)

            For j = 0 To N0 - 1
                v2(j) = v1(j) - a
            Next

            Return v2
        End Operator

        ''' <summary>
        ''' 向量 数乘，各分量分别乘以实数
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="a"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Operator *(v1 As Vector, a#) As Vector
            Dim N0 As Integer = v1.[Dim] ' 获取变量维数
            Dim v2 As New Vector(N0)

            For j As Integer = 0 To N0 - 1
                v2(j) = v1(j) * a
            Next

            Return v2
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator *(v1 As Vector, a!) As Vector
            Return v1 * CDbl(a)
        End Operator

        ''' <summary>
        ''' 向量 数除，各分量分别除以实数
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="a"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator /(v1 As Vector, a#) As Vector
            Dim N0 As Integer = v1.[Dim]         '获取变量维数
            Dim v2 As New Vector(N0)

            For j = 0 To N0 - 1
                v2(j) = v1(j) / a
            Next

            Return v2
        End Operator

        Public Shared Operator /(x As Double, v As Vector) As Vector
            Dim N0 As Integer = v.[Dim]         '获取变量维数
            Dim v2 As New Vector(N0)

            For j = 0 To N0 - 1
                v2(j) = x / v(j)
            Next

            Return v2
        End Operator

        ''' <summary>
        ''' 实数加向量
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="v1"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Operator +(a As Double, v1 As Vector) As Vector
            '向量数加算符重载
            Dim N0 As Integer = v1.[Dim]        '获取变量维数
            Dim v2 As New Vector(N0)

            For j = 0 To N0 - 1
                v2(j) = v1(j) + a
            Next

            Return v2
        End Operator

        ''' <summary>
        ''' 实数减向量
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="v1"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Operator -(a As Double, v1 As Vector) As Vector
            '向量数加算符重载
            Dim N0 As Integer = v1.[Dim]         '获取变量维数
            Dim v2 As New Vector(N0)

            For j = 0 To N0 - 1
                v2(j) = a - v1(j)
            Next
            Return v2
        End Operator

        ''' <summary>
        ''' 向量 数乘
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="v1"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Operator *(a As Double, v1 As Vector) As Vector
            Dim N0 As Integer = v1.[Dim]
            Dim v2 As New Vector(N0)

            For j = 0 To N0 - 1
                v2(j) = v1(j) * a
            Next

            Return v2
        End Operator

        ''' <summary>
        ''' 向量内积
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator Or(v1 As Vector, v2 As Vector) As Double
            '获取变量维数
            Dim N0 = v1.[Dim]
            Dim M0 = v2.[Dim]

            If N0 <> M0 Then
                ' 如果向量维数不匹配，给出告警信息
                Throw New ArgumentException("Inner vector dimensions must agree！")
            End If

            Dim sum As Double

            For j = 0 To N0 - 1
                sum = sum + v1(j) * v2(j)
            Next
            Return sum
        End Operator

        ''' <summary>
        ''' 向量外积（相当于列向量，乘以横向量）
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator Xor(v1 As Vector, v2 As Vector) As Matrix
            '获取变量维数
            Dim N0 = v1.[Dim]
            Dim M0 = v2.[Dim]

            If N0 <> M0 Then
                ' 如果向量维数不匹配，给出告警信息
                Throw New ArgumentException("Inner vector dimensions must agree！")
            End If

            Dim vvmat As New Matrix(N0, N0)

            For i As Integer = 0 To N0 - 1
                For j As Integer = 0 To N0 - 1
                    vvmat(i, j) = v1(i) * v2(j)
                Next
            Next

            '返回外积矩阵
            Return vvmat
        End Operator

        ''' <summary>
        ''' 负向量 
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator -(v1 As Vector) As Vector
            Dim N0 As Integer = v1.[Dim]
            Dim v2 As New Vector(N0)

            For i As Integer = 0 To N0 - 1
                v2(i) = -v1(i)
            Next

            Return v2
        End Operator

        ''' <summary>
        ''' <paramref name="x"/>向量之中的每一个元素是否都等于<paramref name="n"/>?
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator =(x As Vector, n As Integer) As BooleanVector
            Return x = CDbl(n)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator <>(x As Vector, n As Integer) As BooleanVector
            Return Not x = CDbl(n)
        End Operator

        ''' <summary>
        ''' <paramref name="x"/>向量之中的每一个元素是否都等于<paramref name="n"/>?
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator =(x As Vector, n As Double) As BooleanVector
            ' 不可以缺少这一对括号，否则会被当作为属性d，而非值比较
            Dim asserts = From d As Double In x Select (d = n)
            Return New BooleanVector(asserts)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator <>(x As Vector, n As Double) As BooleanVector
            Return Not x = n
        End Operator

        ''' <summary>
        ''' Power: <see cref="Math.Pow(Double, Double)"/>
        ''' </summary>
        ''' <param name="v"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator ^(v As Vector, n As Integer) As Vector
            Return New Vector(From d As Double In v Select d ^ n)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator ^(n As Double, v As Vector) As Vector
            Return New Vector(From p As Double In v Select n ^ p)
        End Operator

        Public Overloads Shared Operator ^(x As Vector, p As Vector) As Vector
            Dim N0 = x.[Dim]
            Dim M0 = p.[Dim]

            If N0 <> M0 Then
                ' 如果向量维数不匹配，给出告警信息
                Throw New ArgumentException("Inner vector dimensions must agree！")
            End If

            Dim v2 As New Vector(N0)

            For j As Integer = 0 To N0 - 1
                v2(j) = x(j) * p(j)
            Next

            Return v2
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator >(x As Vector, n As Double) As BooleanVector
            Return New BooleanVector(From d As Double In x Select d > n)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Operator <(x As Vector, n As Double) As BooleanVector
            Return New BooleanVector(From d As Double In x Select d < n)
        End Operator

        ''' <summary>
        ''' 返回一个逻辑向量，用来指示向量对象的每一个分量与目标比较的逻辑值结果
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator >=(x As Vector, n As Double) As BooleanVector
            Return New BooleanVector(From d As Double In x Select d >= n)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator <=(x As Vector, n As Double) As BooleanVector
            Return New BooleanVector(From d As Double In x Select d <= n)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator >=(x As Vector, y As Vector) As BooleanVector
            Return New BooleanVector(From a In x.SeqIterator Select a.value >= y.buffer(a))
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator <=(x As Vector, y As Vector) As BooleanVector
            Return New BooleanVector(From a In x.SeqIterator Select a.value <= y.buffer(a))
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator <=(x#, y As Vector) As BooleanVector
            Return New BooleanVector(From a In y Select x <= a)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator >=(x#, y As Vector) As BooleanVector
            Return Not x <= y
        End Operator

#Region "CType"

#Region "Syntax support for: Dim v As Vector = {1, 2, 3, 4, 5, 6}"
        Public Shared Widening Operator CType(v As Double()) As Vector
            Return New Vector(v)
        End Operator

        Public Shared Widening Operator CType(v%()) As Vector
            Return New Vector(v.Select(Function(x) CDbl(x)))
        End Operator

        Public Shared Widening Operator CType(v&()) As Vector
            Return New Vector(v.Select(Function(x) CDbl(x)))
        End Operator

        Public Shared Widening Operator CType(v!()) As Vector
            Return New Vector(v.Select(Function(x) CDbl(x)))
        End Operator
#End Region

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Narrowing Operator CType(v As Vector) As Double()
            Return v.ToArray
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Narrowing Operator CType(v As Vector) As DoubleRange
            Return New DoubleRange(v)
        End Operator

        ''' <summary>
        ''' [1,2,3,4,5,6,...]
        ''' </summary>
        ''' <param name="vector$"></param>
        ''' <returns></returns>
        Public Shared Widening Operator CType(vector As String) As Vector
            Dim exp As String = Trim(vector)

            If exp.StringEmpty Then
                Return New Vector

            ElseIf exp.First = "["c AndAlso exp.Last = "]"c Then

                Dim array#() = exp _
                    .GetStackValue("[", "]") _
                    .Split(","c) _
                    .Select(AddressOf Trim) _
                    .Select(AddressOf ParseNumeric) _
                    .ToArray

                Return New Vector(array)

            Else

                Dim array#() = Expressions _
                    .TranslateIndex(exp) _
                    .Select(Function(int) CDbl(int)) _
                    .ToArray
                Return New Vector(array)

            End If
        End Operator

        Public Shared Widening Operator CType(x As Double) As Vector
            Return New Vector() From {x}
        End Operator

        Public Shared Widening Operator CType(x As Integer) As Vector
            Return New Vector() From {CDbl(x)}
        End Operator

        Public Shared Widening Operator CType(x As Long) As Vector
            Return New Vector() From {CDbl(x)}
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Widening Operator CType(args As DefaultString) As Vector
            Return CType(args.DefaultValue, Vector)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Widening Operator CType(vector As VectorShadows(Of Double)) As Vector
            Return vector.As(Of Double).AsVector
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Widening Operator CType(list As List(Of Double)) As Vector
            Return New Vector(list)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Narrowing Operator CType(v As Vector) As List(Of Double)
            Return v.buffer.AsList
        End Operator
#End Region
#End Region

        ''' <summary>
        ''' 进行序列切片操作
        ''' </summary>
        ''' <param name="start%"></param>
        ''' <param name="stop%"></param>
        ''' <param name="steps%"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function slice(Optional start% = 0, Optional stop% = -1, Optional steps% = 1) As Vector
            Return numpy.slice(buffer, start, [stop], steps).AsVector
        End Function

        ''' <summary>
        ''' + http://mathworld.wolfram.com/DotProduct.html
        ''' + http://www.mathsisfun.com/algebra/vectors-dot-product.html
        ''' </summary>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        Public Function DotProduct(v2 As Vector) As Double
            Dim sum#

            For i As Integer = 0 To Me.Count - 1
                sum += Me(i) * v2(i)
            Next

            Return sum
        End Function

        ''' <summary>
        ''' 返回这个向量之中的所有的元素的乘积
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Product() As Double
            Return Me.ProductALL
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CumSum() As Vector
            Return Math.CumSum(Me)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Abs() As Vector
            Return Abs(Me)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ScaleToRange(range As DoubleRange) As Vector
            Return Me.RangeTransform(range)
        End Function

        ''' <summary>
        ''' Display member's data as json array
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return "[" & Me.ToString("G4").JoinBy(", ") & "]"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Function ToString(format$) As String()
            Return Me _
                .Select(Function(x) x.ToString(format)) _
                .ToArray
        End Function

        ''' <summary>
        ''' ``order()``的返回值是对应``排名``的元素所在向量中的位置。
        ''' 
        ''' ```R
        ''' x &lt;- c(97, 93, 85, 74, 32, 100, 99, 67);
        ''' 
        ''' sort(x)
        ''' # [1] 32  67  74  85  93  97  99 100
        ''' order(x)
        ''' # [1] 5 8 4 3 2 1 7 6
        ''' rank(x)
        ''' # [1] 6 5 4 3 1 8 7 2
        ''' ```
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Order() As Vector
            Return Vector.Order(Me)
        End Function

        ''' <summary>
        ''' http://math.stackexchange.com/questions/440320/what-is-magnitude-of-sum-of-two-vector
        ''' </summary>
        ''' <param name="x1"></param>
        ''' <param name="x2"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function SumMagnitudes(x1 As Vector, x2 As Vector) As Double
            Return (x1 + x2).Mod
        End Function

        ''' <summary>
        ''' Returns a numeric vector with all elements is value ``1``
        ''' </summary>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Public Shared Function Ones(n As Integer) As Vector
            Dim result As New Vector(n)

            For i As Integer = 0 To result.Count - 1
                result(i) = 1.0
            Next

            Return result
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function rand(size%) As Vector
            Return Extensions.rand(size)
        End Function
    End Class
End Namespace
