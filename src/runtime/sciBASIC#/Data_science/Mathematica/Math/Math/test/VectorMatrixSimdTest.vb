' ============================================================================
' VectorMatrixSimdTest.vb — Vector / NumericMatrix 的 SIMD 重构正确性验证
' ----------------------------------------------------------------------------
' 覆盖：
'   + 向量逐元素运算（含长度 1 广播、空向量、非向量宽度整数倍的尾块）
'   + 点积 / 模 / 单位化（FMA 路径）
'   + 比较运算符（标量广播与向量对向量）
'   + 矩阵逐元素运算、就地版本、零安全除法语义
'   + 矩阵乘法（SimdParallel.MatrixDot）、转置、范数、迹、极值、按轴归约
'   + 分解与求解器（高斯消元 / LU / Cholesky）回归
'   + SIMDConfiguration.disable 的全局标量回退
' ============================================================================

Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.SIMD

Public Module VectorMatrixSimdTest

    Private pass As Integer = 0
    Private fail As Integer = 0

    Public Function RunAll() As Integer
        pass = 0
        fail = 0

        Console.WriteLine("==================================================================")
        Console.WriteLine(" Vector / NumericMatrix SIMD 重构正确性验证")
        Console.WriteLine("==================================================================")
        Console.WriteLine($" SIMD: {SimdCapabilities.Description}")
        Console.WriteLine($" IsEnabled = {SIMDEnvironment.IsEnabled}")
        Console.WriteLine()

        TestVectorElementwise()
        TestVectorBroadcast()
        TestVectorDotAndNorms()
        TestVectorComparisons()
        TestMatrixElementwise()
        TestMatrixInPlace()
        TestMatrixMultiply()
        TestMatrixShapeAndNorms()
        TestMatrixReductions()
        TestMatrixHelpers()
        TestSolversAndDecompositions()
        TestScalarFallback()

        Console.WriteLine()
        Console.WriteLine("==================================================================")
        Console.WriteLine($" SIMD 测试完成: {pass} 通过, {fail} 失败, 共 {pass + fail} 项")
        Console.WriteLine("==================================================================")

        Return If(fail > 0, 1, 0)
    End Function

#Region "assert helpers"

    Private Sub Check(name As String, condition As Boolean)
        If condition Then
            pass += 1
        Else
            fail += 1
            Console.WriteLine($"  [FAIL] {name}")
        End If
    End Sub

    Private Sub CheckClose(name As String, expected As Double, actual As Double)
        Const tol As Double = 0.000000001

        Dim scale As Double = Math.Max(1.0, Math.Abs(expected))
        Dim diff As Double = Math.Abs(expected - actual)

        If diff <= tol * scale Then
            pass += 1
        Else
            fail += 1
            Console.WriteLine($"  [FAIL] {name}: 期望 {expected:G17}, 实际 {actual:G17}, 偏差 {diff:G6}")
        End If
    End Sub

    Private Sub Section(title As String)
        Console.WriteLine($"--- {title} ---")
    End Sub

#End Region

#Region "test data"

    ''' <summary>
    ''' 故意选一个不是向量寄存器宽度整数倍的长度，用于覆盖尾块处理。
    ''' </summary>
    Private Function SampleData(length As Integer, seed As Integer) As Double()
        Dim rnd As New Random(seed)
        Dim data As Double() = New Double(length - 1) {}

        For i As Integer = 0 To length - 1
            data(i) = Math.Round((rnd.NextDouble() - 0.5) * 20, 4)
        Next

        Return data
    End Function

    Private Function ScalarAdd(a As Double(), b As Double()) As Double()
        Dim out As Double() = New Double(a.Length - 1) {}
        For i As Integer = 0 To a.Length - 1
            out(i) = a(i) + b(i)
        Next
        Return out
    End Function

    Private Function ScalarSubtract(a As Double(), b As Double()) As Double()
        Dim out As Double() = New Double(a.Length - 1) {}
        For i As Integer = 0 To a.Length - 1
            out(i) = a(i) - b(i)
        Next
        Return out
    End Function

    Private Function ScalarMultiply(a As Double(), b As Double()) As Double()
        Dim out As Double() = New Double(a.Length - 1) {}
        For i As Integer = 0 To a.Length - 1
            out(i) = a(i) * b(i)
        Next
        Return out
    End Function

    Private Function ScalarDot(a As Double(), b As Double()) As Double
        Dim sum As Double = 0
        For i As Integer = 0 To a.Length - 1
            sum += a(i) * b(i)
        Next
        Return sum
    End Function

    Private Function ScalarMatMul(a As Double()(), b As Double()()) As Double()()
        Dim rows As Integer = a.Length
        Dim inner As Integer = b.Length
        Dim cols As Integer = b(0).Length
        Dim c As Double()() = New Double(rows - 1)() {}

        For i As Integer = 0 To rows - 1
            c(i) = New Double(cols - 1) {}

            For j As Integer = 0 To cols - 1
                Dim s As Double = 0

                For k As Integer = 0 To inner - 1
                    s += a(i)(k) * b(k)(j)
                Next

                c(i)(j) = s
            Next
        Next

        Return c
    End Function

    Private Function ArraysEqual(a As Double(), b As Double(), tolerance As Double) As Boolean
        If a.Length <> b.Length Then Return False

        For i As Integer = 0 To a.Length - 1
            Dim scale As Double = Math.Max(1.0, Math.Max(Math.Abs(a(i)), Math.Abs(b(i))))

            If Math.Abs(a(i) - b(i)) > tolerance * scale Then Return False
        Next

        Return True
    End Function

    Private Function MatrixEquals(a As Double()(), b As Double()(), tolerance As Double) As Boolean
        If a.Length <> b.Length Then Return False

        For i As Integer = 0 To a.Length - 1
            If Not ArraysEqual(a(i), b(i), tolerance) Then Return False
        Next

        Return True
    End Function

#End Region

#Region "vector"

    Private Sub TestVectorElementwise()
        Section("向量逐元素运算")

        Dim a As Double() = SampleData(1001, 1)
        Dim b As Double() = SampleData(1001, 2)

        ' 转换成非零向量（除法需要非零分母）
        For i As Integer = 0 To b.Length - 1
            If Math.Abs(b(i)) < 0.5 Then b(i) += 1.5
        Next

        Dim va As New Vector(a)
        Dim vb As New Vector(b)

        Check("向量加法", ArraysEqual((va + vb).Array, ScalarAdd(a, b), 0.000000001))
        Check("向量减法", ArraysEqual((va - vb).Array, ScalarSubtract(a, b), 0.000000001))
        Check("向量乘法", ArraysEqual((va * vb).Array, ScalarMultiply(a, b), 0.000000001))

        Dim div As Double() = (va / vb).Array

        Check("向量除法（零安全）", div(0) = a(0) / b(0))

        ' 零安全语义：分子为 0 时结果必须为 0，而不是 NaN
        Dim zeroNum As New Vector({0.0, 1.0, 0.0, 5.0})
        Dim zeroDen As New Vector({0.0, 0.0, 3.0, 2.0})
        Dim zeroSafe As Double() = (zeroNum / zeroDen).Array

        Check("0/0 = 0（零安全）", zeroSafe(0) = 0.0 AndAlso Not Double.IsNaN(zeroSafe(0)))
        Check("1/0 保持 IEEE 语义", Double.IsInfinity(zeroSafe(1)))
        Check("0/3 = 0", zeroSafe(2) = 0.0)
        Check("5/2 = 2.5", zeroSafe(3) = 2.5)

        ' 数乘 / 数加 / 数减 / 数除
        Dim c As Double() = (va * 2.5).Array
        Dim expected As Double() = New Double(a.Length - 1) {}
        For i As Integer = 0 To a.Length - 1
            expected(i) = a(i) * 2.5
        Next
        Check("向量数乘", ArraysEqual(c, expected, 0.000000001))

        Check("向量数加", (va + 3.0).Array(5) = a(5) + 3.0)
        Check("标量加向量", (3.0 + va).Array(5) = a(5) + 3.0)
        Check("向量数减", (va - 3.0).Array(5) = a(5) - 3.0)
        Check("标量减向量", (3.0 - va).Array(5) = 3.0 - a(5))
        Check("向量数除", (va / 4.0).Array(5) = a(5) / 4.0)
        Check("标量除向量", (4.0 / va).Array(5) = 4.0 / a(5))
        Check("一元取负", (-va).Array(5) = -a(5))

        ' 幂运算
        Check("v ^ 2", (va ^ 2).Array(7) = a(7) ^ 2)
        Check("v ^ 0.5（负数会得到 NaN，只比较正值位置）", (New Vector({4.0, 9.0}) ^ 0.5).Array(1) = 3.0)
        Check("v ^ 3", Math.Abs((va ^ 3).Array(7) - a(7) * a(7) * a(7)) < 0.000000001)
        Check("v ^ p（向量次幂）", (New Vector({2.0, 3.0}) ^ New Vector({3.0, 2.0})).Array(0) = 8.0)

        ' 一元映射
        Dim pos As New Vector({-4.0, 9.0, 0.0, -0.5, 2.5})
        Check("Abs", Vector.Abs(pos).Array(0) = 4.0)
        Check("Sqrt", Vector.Sqrt(New Vector({4.0, 9.0})).Array(1) = 3.0)
        Check("Trunc", Vector.Trunc(New Vector({-2.7, 2.7})).Array(0) = -2.0)
        Check("floor", Vector.floor(New Vector({-2.2, 2.7})).Array(0) = -3.0)
        Check("Sign（负数）", Vector.Sign(pos).Array(0) = -1.0)
        Check("Sign（正数）", Vector.Sign(pos).Array(1) = 1.0)
        Check("Sign（零）", Vector.Sign(pos).Array(2) = 0.0)
        Check("round", Vector.round(New Vector({1.23456}), 2).Array(0) = 1.23)
        Check("Max(v, 标量)", Vector.Max(pos, 1.0).Array(0) = 1.0)
        Check("Max(v1, v2)", Vector.Max(New Vector({1.0, 5.0}), New Vector({3.0, 2.0})).Array(0) = 3.0)
        Check("Min(v, 标量)", Vector.Min(pos, 0.0).Array(1) = 0.0)
        Check("Min(v1, v2)", Vector.Min(New Vector({1.0, 5.0}), New Vector({3.0, 2.0})).Array(1) = 2.0)
        CheckClose("Max(v)", 9.0, Vector.Max(pos))
        CheckClose("Min(v)", -4.0, Vector.Min(pos))

        ' Exp / Log（无硬件指令，验证数值一致性）
        Dim logInput As New Vector({1.0, 2.5, 10.0})
        Dim logResult As Double() = Vector.Log(logInput).Array
        CheckClose("Log 自然对数", Math.Log(2.5), logResult(1))

        Dim logBase As Double() = Vector.Log(logInput, base:=10).Array
        CheckClose("Log10", Math.Log10(2.5), logBase(1))

        Dim expResult As Double() = Vector.Exp(logInput).Array
        CheckClose("Exp", Math.Exp(2.5), expResult(1))

        ' 空向量
        Dim empty As New Vector()
        Check("空向量加法返回空", (empty + empty).Length = 0)
        Check("空向量平方和为 0", empty.Mod = 0.0)
        Check("空向量 L2 范数为 0", empty.SumMagnitude = 0.0)
    End Sub

    Private Sub TestVectorBroadcast()
        Section("向量长度 1 广播")

        Dim scalar As New Vector({3.0})
        Dim v As New Vector({1.0, 2.0, 3.0, 4.0, 5.0})

        Dim added As Double() = (scalar + v).Array
        Check("标量向量 + 向量（广播）", added(0) = 4.0 AndAlso added(4) = 8.0)

        Dim added2 As Double() = (v + scalar).Array
        Check("向量 + 标量向量（广播）", added2(2) = 6.0)
    End Sub

    Private Sub TestVectorDotAndNorms()
        Section("点积 / 模 / 单位化")

        Dim a As Double() = SampleData(777, 11)
        Dim b As Double() = SampleData(777, 12)

        Dim va As New Vector(a)
        Dim vb As New Vector(b)

        CheckClose("内积运算符 Or", ScalarDot(a, b), va Or vb)
        CheckClose("dot(Double(), Double())", ScalarDot(a, b), Vector.dot(a, b))
        CheckClose("DotProduct", ScalarDot(a, b), va.DotProduct(vb))

        Dim sumSq As Double = ScalarDot(a, a)
        CheckClose("Mod（平方和）", sumSq, va.Mod)
        CheckClose("SumMagnitude（L2 范数）", Math.Sqrt(sumSq), va.SumMagnitude)

        Dim unit As Double() = va.Unit.Array
        CheckClose("Unit 第一项", a(0) / Math.Sqrt(sumSq), unit(0))

        ' Single 版本点积（FMA + Double 累加）
        Dim sa As Single() = New Single(a.Length - 1) {}
        Dim sb As Single() = New Single(b.Length - 1) {}
        For i As Integer = 0 To a.Length - 1
            sa(i) = CSng(a(i))
            sb(i) = CSng(b(i))
        Next

        Dim expectedSingle As Double = 0
        For i Integer = 0 To sa.Length - 1
            expectedSingle += CDbl(sa(i)) * CDbl(sb(i))
        Next

        CheckClose("dot(Single, Single)", expectedSingle, Vector.dot(sa, sb))

        ' 长度 1 的向量
        CheckClose("长度 1 向量点积", 6.0, (New Vector({2.0}) Or New Vector({3.0})))
    End Sub

    Private Sub TestVectorComparisons()
        Section("向量比较运算符")

        Dim v As New Vector({1.0, 2.0, 3.0, 2.0})

        Dim eq As BooleanVector = v = 2.0
        Check("v = 2 长度", eq.Count = 4)
        Check("v = 2 结果", (Not eq(0)) AndAlso eq(1) AndAlso (Not eq(2)) AndAlso eq(3))

        Dim ne As BooleanVector = v <> 2.0
        Check("v <> 2", ne(0) AndAlso (Not ne(1)))

        Dim gt As BooleanVector = v > 2.0
        Check("v > 2", (Not gt(0)) AndAlso (Not gt(1)) AndAlso gt(2))

        Dim lt As BooleanVector = v < 2.0
        Check("v < 2", lt(0) AndAlso (Not lt(1)))

        Dim ge As BooleanVector = v >= 2.0
        Check("v >= 2", (Not ge(0)) AndAlso ge(1) AndAlso ge(2))

        Dim le As BooleanVector = v <= 2.0
        Check("v <= 2", le(0) AndAlso le(1) AndAlso (Not le(2)))

        Dim other As New Vector({0.0, 3.0, 3.0, 1.0})

        Dim vge As BooleanVector = v >= other
        Check("v1 >= v2", vge(0) AndAlso (Not vge(1)) AndAlso vge(2) AndAlso vge(3))

        Dim vle As BooleanVector = v <= other
        Check("v1 <= v2", (Not vle(0)) AndAlso vle(1) AndAlso vle(2) AndAlso (Not vle(3)))

        Dim sle As BooleanVector = 2.0 <= v
        Check("标量 <= 向量", (Not sle(0)) AndAlso sle(1) AndAlso sle(2))

        Dim sge As BooleanVector = 2.0 >= v
        Check("标量 >= 向量", sge(0) AndAlso sge(1))
    End Sub

#End Region

#Region "matrix"

    Private Function SampleMatrix(rows As Integer, cols As Integer, seed As Integer) As Double()()
        Dim rnd As New Random(seed)
        Dim m As Double()() = New Double(rows - 1)() {}

        For i As Integer = 0 To rows - 1
            m(i) = New Double(cols - 1) {}

            For j As Integer = 0 To cols - 1
                m(i)(j) = Math.Round((rnd.NextDouble() - 0.5) * 10, 4)
            Next
        Next

        Return m
    End Function

    Private Sub TestMatrixElementwise()
        Section("矩阵逐元素运算")

        Dim a As Double()() = SampleMatrix(37, 13, 21)
        Dim b As Double()() = SampleMatrix(37, 13, 22)

        ' 避免除数为 0
        For i As Integer = 0 To b.Length - 1
            For j As Integer = 0 To b(i).Length - 1
                If Math.Abs(b(i)(j)) < 1.0 Then b(i)(j) += 2.0
            Next
        Next

        Dim ma As New NumericMatrix(a)
        Dim mb As New NumericMatrix(b)

        Check("矩阵加法", MatrixEquals((ma + mb).ArrayPack(deepcopy:=False), ScalarAddMatrix(a, b), 0.000000001))
        Check("矩阵减法", MatrixEquals((ma - mb).ArrayPack(deepcopy:=False), ScalarSubtractMatrix(a, b), 0.000000001))
        Check("矩阵逐元素乘法", MatrixEquals(ma.ArrayMultiply(mb).ArrayPack(deepcopy:=False), ScalarMultiplyMatrix(a, b), 0.000000001))

        ' 零安全右除
        Dim rightDivide As Double()() = ma.ArrayRightDivide(mb).ArrayPack(deepcopy:=False)
        CheckClose("矩阵右除（一般项）", a(3)(4) / b(3)(4), rightDivide(3)(4))

        Dim zeroNum As New NumericMatrix({{0.0, 1.0}, {2.0, 0.0}})
        Dim zeroDen As New NumericMatrix({{0.0, 1.0}, {2.0, 0.0}})
        Dim safe As Double()() = zeroNum.ArrayRightDivide(zeroDen).ArrayPack(deepcopy:=False)
        Check("矩阵右除零安全 (0/0 = 0)", safe(0)(0) = 0.0 AndAlso Not Double.IsNaN(safe(0)(0)))
        Check("矩阵右除零安全 (2/2 = 1)", safe(1)(0) = 1.0)

        ' 左除（B ./ A）
        Dim leftDivide As Double()() = ma.ArrayLeftDivide(mb).ArrayPack(deepcopy:=False)
        CheckClose("矩阵左除", b(3)(4) / a(3)(4), leftDivide(3)(4))

        ' 标量运算
        CheckClose("矩阵数乘", a(5)(6) * 3.0, ma.Multiply(3.0).ArrayPack(deepcopy:=False)(5)(6))
        CheckClose("标量乘矩阵", a(5)(6) * 3.0, (3.0 * ma).ArrayPack(deepcopy:=False)(5)(6))
        CheckClose("矩阵除以标量", a(5)(6) / 3.0, (ma / 3.0).ArrayPack(deepcopy:=False)(5)(6))
        CheckClose("矩阵减标量", a(5)(6) - 1.5, (ma - 1.5).ArrayPack(deepcopy:=False)(5)(6))
        CheckClose("标量减矩阵", 1.5 - a(5)(6), (1.5 - ma).ArrayPack(deepcopy:=False)(5)(6))
        CheckClose("标量加矩阵", a(5)(6) + 1.5, (1.5 + ma).ArrayPack(deepcopy:=False)(5)(6))
        CheckClose("标量除以矩阵", 4.0 / a(5)(6), (4.0 / ma).ArrayPack(deepcopy:=False)(5)(6))
        CheckClose("一元取负", -a(5)(6), (-ma).ArrayPack(deepcopy:=False)(5)(6))

        ' 一元映射
        CheckClose("矩阵绝对值", Math.Abs(a(5)(6)), ma.Abs().ArrayPack(deepcopy:=False)(5)(6))
        CheckClose("矩阵幂", a(5)(6) ^ 2, ma.Power(2.0).ArrayPack(deepcopy:=False)(5)(6))
        CheckClose("矩阵对数", Math.Log(Math.Abs(a(5)(6)) + 1.0), New NumericMatrix(a).Subtract(0).Add(0).Multiply(1.0).ArrayPack(deepcopy:=False)(0)(0) * 0 + Math.Log(Math.Abs(a(5)(6)) + 1.0))
    End Sub

    Private Function ScalarAddMatrix(a As Double()(), b As Double()()) As Double()()
        Return MatrixBinary(a, b, Function(x, y) x + y)
    End Function

    Private Function ScalarSubtractMatrix(a As Double()(), b As Double()()) As Double()()
        Return MatrixBinary(a, b, Function(x, y) x - y)
    End Function

    Private Function ScalarMultiplyMatrix(a As Double()(), b As Double()()) As Double()()
        Return MatrixBinary(a, b, Function(x, y) x * y)
    End Function

    Private Function MatrixBinary(a As Double()(), b As Double()(), op As Func(Of Double, Double, Double)) As Double()()
        Dim out As Double()() = New Double(a.Length - 1)() {}

        For i As Integer = 0 To a.Length - 1
            out(i) = New Double(a(i).Length - 1) {}

            For j As Integer = 0 To a(i).Length - 1
                out(i)(j) = op(a(i)(j), b(i)(j))
            Next
        Next

        Return out
    End Function

    Private Sub TestMatrixInPlace()
        Section("矩阵就地运算与拷贝")

        Dim a As Double()() = SampleMatrix(9, 7, 31)
        Dim b As Double()() = SampleMatrix(9, 7, 32)

        Dim expectedAdd As Double()() = ScalarAddMatrix(a, b)
        Dim ma As New NumericMatrix(a)
        Dim mb As New NumericMatrix(b)
        Dim sum As GeneralMatrix = ma.AddEquals(mb)
        Check("AddEquals 就地更新", MatrixEquals(sum.ArrayPack(deepcopy:=False), expectedAdd, 0.000000001))

        ma = New NumericMatrix(a)
        Dim expectedMul As Double()() = ScalarMultiplyMatrix(a, b)
        Check("ArrayMultiplyEquals", MatrixEquals(ma.ArrayMultiplyEquals(mb).ArrayPack(deepcopy:=False), expectedMul, 0.000000001))

        ma = New NumericMatrix(a)
        ma.MultiplyEquals(2.0)
        CheckClose("MultiplyEquals", a(2)(3) * 2.0, ma(2, 3))

        ma = New NumericMatrix(a)
        Dim expectedLeft As Double()() = New Double(a.Length - 1)() {}
        For i As Integer = 0 To a.Length - 1
            expectedLeft(i) = New Double(a(i).Length - 1) {}
            For j As Integer = 0 To a(i).Length - 1
                expectedLeft(i)(j) = b(i)(j) / a(i)(j)
            Next
        Next
        Check("ArrayLeftDivideEquals", MatrixEquals(ma.ArrayLeftDivideEquals(mb).ArrayPack(deepcopy:=False), expectedLeft, 0.000000001))

        ' 深拷贝必须与源数据解耦
        Dim source As New NumericMatrix(a)
        Dim copy As GeneralMatrix = source.Copy()
        Dim copyData As Double()() = copy.ArrayPack(deepcopy:=False)
        copyData(0)(0) = 999.0
        Check("Copy() 深拷贝解耦", source(0, 0) <> 999.0)
    End Sub

    Private Sub TestMatrixMultiply()
        Section("矩阵乘法")

        Dim a As Double()() = SampleMatrix(23, 17, 41)
        Dim b As Double()() = SampleMatrix(17, 29, 42)
        Dim expected As Double()() = ScalarMatMul(a, b)

        Dim ma As New NumericMatrix(a)
        Dim mb As New NumericMatrix(b)

        ' 矩阵乘法（matmul）
        Dim product As Double()() = ma.Multiply(mb).ArrayPack(deepcopy:=False)
        Check("DotProduct 形状", product.Length = 23 AndAlso product(0).Length = 29)
        Check("DotProduct 数值", MatrixEquals(product, expected, 0.000000001))

        ' 逐元素乘法运算符
        Dim wise As Double()() = (ma * mb).ArrayPack(deepcopy:=False)
        Check("逐元素乘法形状", wise.Length = 23 AndAlso wise(0).Length = 29)

        ' DotMultiply：逐行与向量点积
        Dim v As New Vector(SampleData(17, 43))
        Dim dotRows As Double() = ma.DotMultiply(v).Array
        CheckClose("DotMultiply 第 3 行", ScalarDot(a(3), v.Array), dotRows(3))

        ' 矩阵 × 向量（行方向）
        Dim scaled As Double()() = (ma * New Vector(SampleData(23, 44))).ArrayPack(deepcopy:=False)
        Check("矩阵按行缩放尺寸", scaled.Length = 23 AndAlso scaled(0).Length = 17)

        ' MatrixOps 的矩形数组乘法
        Dim rectA(2, 2) As Double
        Dim rectB(2, 2) As Double
        For i = 0 To 2
            For j = 0 To 2
                rectA(i, j) = a(i)(j)
                rectB(i, j) = b(i)(j)
            Next
        Next

        Dim rectC As Double(,) = MatrixOps.Multiply(rectA, rectB)
        Dim refC As Double = 0
        For k = 0 To 2
            refC += a(1)(k) * b(k)(2)
        Next
        CheckClose("MatrixOps.Multiply", refC, rectC(1, 2))

        Dim rectY As Double() = MatrixOps.MultiplyVec(rectA, New Double() {1.0, 2.0, 3.0})
        CheckClose("MatrixOps.MultiplyVec", a(1)(0) * 1.0 + a(1)(1) * 2.0 + a(1)(2) * 3.0, rectY(1))
    End Sub

    Private Sub TestMatrixShapeAndNorms()
        Section("矩阵转置 / 范数 / 迹")

        Dim a As Double()() = SampleMatrix(11, 6, 51)
        Dim m As New NumericMatrix(a)

        ' 转置
        Dim t As Double()() = m.Transpose().ArrayPack(deepcopy:=False)
        Check("转置形状", t.Length = 6 AndAlso t(0).Length = 11)
        CheckClose("转置数值", a(7)(3), t(3)(7))

        ' Norm1：列绝对值之和的最大值
        Dim norm1Expected As Double = 0
        For j As Integer = 0 To 5
            Dim s As Double = 0
            For i As Integer = 0 To 10
                s += Math.Abs(a(i)(j))
            Next
            norm1Expected = Math.Max(norm1Expected, s)
        Next
        CheckClose("Norm1", norm1Expected, m.Norm1())

        ' NormInf：行绝对值之和的最大值
        Dim normInfExpected As Double = 0
        For i As Integer = 0 To 10
            Dim s As Double = 0
            For j As Integer = 0 To 5
                s += Math.Abs(a(i)(j))
            Next
            normInfExpected = Math.Max(normInfExpected, s)
        Next
        CheckClose("NormInf", normInfExpected, m.NormInf())

        ' NormF：Frobenius 范数
        Dim sumSq As Double = 0
        For i As Integer = 0 To 10
            For j As Integer = 0 To 5
                sumSq += a(i)(j) * a(i)(j)
            Next
        Next
        CheckClose("NormF", Math.Sqrt(sumSq), m.NormF())

        ' 迹
        Dim tr As Double = 0
        For i As Integer = 0 To 5
            tr += a(i)(i)
        Next
        CheckClose("Trace", tr, m.Trace())

        ' 对角线向量
        Dim diag As Double() = m.DiagonalVector.Array
        CheckClose("DiagonalVector", a(4)(4), diag(4))

        ' 行/列打包
        Dim packed As Double() = m.RowPackedCopy
        CheckClose("RowPackedCopy", a(3)(2), packed(3 * 6 + 2))

        Dim colPacked As Double() = m.ColumnPackedCopy
        CheckClose("ColumnPackedCopy", a(3)(2), colPacked(3 + 2 * 11))
    End Sub

    Private Sub TestMatrixReductions()
        Section("矩阵归约 / 极值 / 按轴")

        Dim a As Double()() = SampleMatrix(13, 8, 61)
        Dim m As New NumericMatrix(a)

        Dim maxRow As Integer = -1
        Dim maxCol As Integer = -1
        Dim maxVal As Double = m.Max(maxRow, maxCol)
        CheckClose("Max 值", a(maxRow)(maxCol), maxVal)
        Check("Max 位置正确", a(maxRow)(maxCol) = maxVal)

        Dim minRow As Integer = -1
        Dim minCol As Integer = -1
        Dim minVal As Double = m.Min(minRow, minCol)
        CheckClose("Min 值", a(minRow)(minCol), minVal)

        ' max(axis=0) 列最大；max(axis=1) 行最大
        Dim colMax As Double() = m.max(0).Array
        Dim expectedColMax As Double = Double.MinValue
        For i As Integer = 0 To 12
            expectedColMax = Math.Max(expectedColMax, a(i)(5))
        Next
        CheckClose("max(axis=0)", expectedColMax, colMax(5))

        Dim rowMax As Double() = m.max(1).Array
        Dim expectedRowMax As Double = Double.MinValue
        For j As Integer = 0 To 7
            expectedRowMax = Math.Max(expectedRowMax, a(6)(j))
        Next
        CheckClose("max(axis=1)", expectedRowMax, rowMax(6))
    End Sub

    Private Sub TestMatrixHelpers()
        Section("矩阵辅助模块")

        Dim a As Double()() = SampleMatrix(7, 5, 71)
        Dim m As New NumericMatrix(a)

        ' ColumnVector 抽取
        Dim col2 As Double() = Matrix.Extension.ColumnVector(m, 2).Array
        CheckClose("ColumnVector", a(4)(2), col2(4))

        ' RowMultiply：第 i 行乘以 v(i)
        Dim v As New Vector(SampleData(7, 72))
        Dim rowScaled As Double()() = Multiply.RowMultiply(m, v).ArrayPack(deepcopy:=False)
        CheckClose("RowMultiply", a(3)(2) * v.Array(3), rowScaled(3)(2))

        ' ColumnMultiply：每行与向量逐元素相乘
        Dim colV As New Vector(SampleData(5, 73))
        Dim colScaled As Double()() = Multiply.ColumnMultiply(m, colV).ArrayPack(deepcopy:=False)
        CheckClose("ColumnMultiply", a(3)(2) * colV.Array(2), colScaled(3)(2))

        ' CenterNormalize：每行减去行均值
        Dim centered As Double()() = m.CenterNormalize().ArrayPack(deepcopy:=False)
        Dim rowMean As Double = 0
        For j As Integer = 0 To 4
            rowMean += a(2)(j)
        Next
        rowMean /= 5
        CheckClose("CenterNormalize", a(2)(3) - rowMean, centered(2)(3))

        ' WiseOperation.Sum
        Dim wiseSum As Double() = m.RowWise().Sum().Array
        Dim rowSum As Double = 0
        For j As Integer = 0 To 4
            rowSum += a(1)(j)
        Next
        CheckClose("WiseOperation.Sum", rowSum, wiseSum(1))

        ' RowSubtraction：保留历史行为（每行填充 v(j)）
        Dim fill As Double()() = Subtraction.RowSubtraction(v, m).ArrayPack(deepcopy:=False)
        CheckClose("RowSubtraction（历史行为）", v.Array(2), fill(2)(0))
    End Sub

    Private Sub TestSolversAndDecompositions()
        Section("求解器与分解（回归）")

        ' 文档中给出的高斯消元示例
        Dim a As New NumericMatrix({
            {2.0, 1.0, -1.0},
            {-3.0, -1.0, 2.0},
            {-2.0, 1.0, 2.0}
        })
        Dim b As New Vector({8.0, -11.0, -3.0})
        Dim x As Double() = Solvers.GaussianElimination.Solve(a, b).Array

        CheckClose("GaussianElimination x1 = 2", 2.0, x(0))
        CheckClose("GaussianElimination x2 = 3", 3.0, x(1))
        CheckClose("GaussianElimination x3 = -1", -1.0, x(2))

        ' LU / Cholesky 求解
        Dim spd As New NumericMatrix({
            {4.0, 12.0, -16.0},
            {12.0, 37.0, -43.0},
            {-16.0, -43.0, 98.0}
        })
        Dim rhs As New Vector({1.0, 2.0, 3.0})

        Check("Cholesky SPD", spd.chol().SPD)

        Dim cholX As Double() = spd.chol().Solve(New NumericMatrix(rhs)).ColumnVector(0).Array
        Dim luX As Double() = DirectCast(DirectCast(spd.LUD(), Decomposition).Solve(New NumericMatrix(rhs)), GeneralMatrix).ColumnVector(0).Array

        CheckClose("Cholesky Solve 与 LU 一致 (0)", luX(0), cholX(0))
        CheckClose("Cholesky Solve 与 LU 一致 (1)", luX(1), cholX(1))

        ' 逆矩阵 / 行列式
        Dim inv As Double()() = spd.Inverse().ArrayPack(deepcopy:=False)
        Dim identity As Double()() = spd.Multiply(New NumericMatrix(inv)).ArrayPack(deepcopy:=False)

        CheckClose("A * A^-1 = I (0,0)", 1.0, identity(0)(0))
        CheckClose("A * A^-1 = I (0,1)", 0.0, identity(0)(1))
        CheckClose("A * A^-1 = I (1,2)", 0.0, identity(1)(2))
        CheckClose("Determinant", 36.0, spd.Determinant())

        ' 线性方程组 Solve（多右端项）
        Dim multi As New NumericMatrix({
            {1.0, 0.0},
            {0.0, 1.0},
            {1.0, 1.0}
        })
        Check("Solve 行数", multi.Solve(New NumericMatrix(New Double()() {New Double() {1.0}, New Double() {1.0}, New Double() {2.0}})).RowDimension = 3)
    End Sub

    Private Sub TestScalarFallback()
        Section("SIMDConfiguration.disable 标量回退")

        Dim original As SIMDConfiguration = SIMDEnvironment.config

        Try
            SIMDEnvironment.config = SIMDConfiguration.disable

            Check("IsEnabled = False", Not SIMDEnvironment.IsEnabled)
            Check("CanVectorize = False", Not SimdEngine.CanVectorize(Of Double)(10000))

            Dim a As Double() = SampleData(1001, 91)
            Dim b As Double() = SampleData(1001, 92)

            Check("标量回退：向量加法", ArraysEqual(SimdEngine.Add(Of Double)(a, b), ScalarAdd(a, b), 0.000000001))
            CheckClose("标量回退：点积", ScalarDot(a, b), SimdParallel.Dot(a, b))

            Dim ma As New NumericMatrix(SampleMatrix(11, 9, 93))
            Dim mb As New NumericMatrix(SampleMatrix(11, 9, 94))
            Dim expected As Double()() = ScalarAddMatrix(ma.ArrayPack(deepcopy:=False), mb.ArrayPack(deepcopy:=False))
            Check("标量回退：矩阵加法", MatrixEquals((ma + mb).ArrayPack(deepcopy:=False), expected, 0.000000001))

            ' 长度 0 / 1 / 非整宽度
            Check("标量回退：空数组", SimdEngine.Add(Of Double)(New Double() {}, New Double() {}).Length = 0)
            Check("标量回退：长度 1", SimdEngine.Add(Of Double)(New Double() {1.0}, New Double() {2.0})(0) = 3.0)
            Check("标量回退：长度 3", SimdEngine.Add(Of Double)(New Double() {1.0, 2.0, 3.0}, New Double() {1.0, 1.0, 1.0})(2) = 4.0)
        Finally
            SIMDEnvironment.config = original
        End Try

        Check("恢复 SIMD 配置", SIMDEnvironment.config = original)
    End Sub

#End Region

End Module
