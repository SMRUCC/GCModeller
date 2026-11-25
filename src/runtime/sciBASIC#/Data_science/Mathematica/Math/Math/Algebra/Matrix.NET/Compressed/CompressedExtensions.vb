Imports System.Runtime.CompilerServices
Imports std = System.Math

Namespace LinearAlgebra.Matrix.Compressed

    ''' <summary>
    ''' 压缩稀疏矩阵的通用扩展方法：形状检查、非负整数判定、数值裁剪与整数计数视图。
    ''' </summary>
    ''' <remarks>
    ''' 这里只放"与统计语义无关"的结构/数值工具；秩和检验、分位数、元分析等
    ''' 统计算子位于 <c>Microsoft.VisualBasic.Math.Statistics</c> 工程。
    ''' </remarks>
    <HideModuleName>
    Public Module CompressedExtensions

        ''' <summary>
        ''' 计数矩阵允许的最大整数值上限（float32 能够精确表示的整数上界）
        ''' </summary>
        Public Const ExactIntegerLimit As Double = 16777216.0 ' 2^24

        ''' <summary>
        ''' 检查两个压缩矩阵是否同形
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SameShape(a As CsrMatrix, b As CsrMatrix) As Boolean
            If a Is Nothing OrElse b Is Nothing Then Return False
            Return a.Rows = b.Rows AndAlso a.Columns = b.Columns
        End Function

        ''' <summary>
        ''' 非零元的最大绝对值（空矩阵返回 0）
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <returns></returns>
        <Extension>
        Public Function MaxAbs(matrix As CsrMatrix) As Double
            If matrix Is Nothing Then Return 0.0

            Dim max As Double = 0.0
            Dim values As Double() = matrix.Values

            For k As Integer = 0 To values.Length - 1
                Dim v As Double = std.Abs(values(k))

                If v > max Then max = v
            Next

            Return max
        End Function

        ''' <summary>
        ''' 非零元的最小值（空矩阵返回 0）
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <returns></returns>
        <Extension>
        Public Function MinValue(matrix As CsrMatrix) As Double
            If matrix Is Nothing OrElse matrix.NonZeros = 0 Then Return 0.0

            Dim min As Double = Double.MaxValue
            Dim values As Double() = matrix.Values

            For k As Integer = 0 To values.Length - 1
                If values(k) < min Then min = values(k)
            Next

            Return min
        End Function

        ''' <summary>
        ''' 判定所有非零元是否都是"非负整数"（允许 <paramref name="tolerance"/> 的相对误差，
        ''' 用于吸收浮点运算引入的 <c>1.0000000001</c> 这类噪声）
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <param name="tolerance">绝对容差，默认 0</param>
        ''' <returns></returns>
        <Extension>
        Public Function IsNonNegativeIntegers(matrix As CsrMatrix, Optional tolerance As Double = 0.0) As Boolean
            If matrix Is Nothing Then Return True

            Dim values As Double() = matrix.Values

            For k As Integer = 0 To values.Length - 1
                Dim v As Double = values(k)

                If v < -tolerance Then Return False
                If std.Abs(v - std.Round(v)) > tolerance Then Return False
            Next

            Return True
        End Function

        ''' <summary>
        ''' 校验计数矩阵：全部非负整数且小于 <c>2^24</c>（float32 精确表示上界）
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <param name="matrixName">异常信息中使用的矩阵名称</param>
        ''' <returns>原矩阵（便于链式调用）</returns>
        ''' <exception cref="ArgumentException">存在负值、非整数或超过 2^24 的计数</exception>
        <Extension>
        Public Function AssertIntegerCounts(matrix As CsrMatrix, Optional matrixName As String = "matrix") As CsrMatrix
            If matrix Is Nothing Then Throw New ArgumentNullException(matrixName)

            Dim values As Double() = matrix.Values
            Dim indices As Integer() = matrix.Indices

            For k As Integer = 0 To values.Length - 1
                Dim v As Double = values(k)

                If v < 0.0 Then
                    Throw New ArgumentException($"{matrixName}: negative count {v} at non-zero entry #{k}", matrixName)
                End If
                If v <> std.Round(v) Then
                    Throw New ArgumentException($"{matrixName}: non-integer count {v} at non-zero entry #{k}", matrixName)
                End If
                If v >= ExactIntegerLimit Then
                    Throw New ArgumentException(
                        $"{matrixName}: count {v} at non-zero entry #{k} (column {indices(k)}) exceeds the 2^24 exact integer limit",
                        matrixName)
                End If
            Next

            Return matrix
        End Function

        ''' <summary>
        ''' 把负的零碎值裁剪为 0（浮点误差清理），返回新实例
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <param name="tolerance">绝对值不超过该阈值的负值被清零，默认 0</param>
        ''' <returns></returns>
        <Extension>
        Public Function ClipNegativeToZero(matrix As CsrMatrix, Optional tolerance As Double = 0.0) As CsrMatrix
            If matrix Is Nothing Then Throw New ArgumentNullException(NameOf(matrix))

            Dim values As Double() = matrix.Values
            Dim needsFix As Boolean = False

            For k As Integer = 0 To values.Length - 1
                If values(k) < -tolerance Then
                    Throw New ArgumentException(
                        $"negative value {values(k)} at non-zero entry #{k} exceeds the clipping tolerance {tolerance}")
                End If
                If values(k) < 0.0 Then
                    needsFix = True
                    Exit For
                End If
            Next

            If Not needsFix Then Return matrix

            Dim out As Double() = DirectCast(values.Clone(), Double())

            For k As Integer = 0 To out.Length - 1
                If out(k) < 0.0 Then out(k) = 0.0
            Next

            Return New CsrMatrix(
                matrix.Rows,
                matrix.Columns,
                DirectCast(matrix.Indptr.Clone(), Integer()),
                matrix.Indices,
                out)
        End Function

        ''' <summary>
        ''' 转换为整数计数视图（<see cref="Long"/> 锯齿数组），同时校验非负整数与 <c>2^24</c> 上界
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <param name="matrixName"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ToIntegerCounts(matrix As CsrMatrix, Optional matrixName As String = "matrix") As Long()()
            If matrix Is Nothing Then Throw New ArgumentNullException(matrixName)

            Call matrix.AssertIntegerCounts(matrixName)

            Dim rows As Integer = matrix.Rows
            Dim columns As Integer = matrix.Columns
            Dim indptr As Integer() = matrix.Indptr
            Dim values As Double() = matrix.Values
            Dim indices As Integer() = matrix.Indices
            Dim dense As Long()() = New Long(rows - 1)() {}

            For i As Integer = 0 To rows - 1
                Dim row As Long() = New Long(columns - 1) {}
                Dim n As Integer = indptr(i + 1) - indptr(i)

                If n > 0 Then
                    For k As Integer = indptr(i) To indptr(i + 1) - 1
                        row(indices(k)) = CLng(values(k))
                    Next
                End If

                dense(i) = row
            Next

            Return dense
        End Function

    End Module
End Namespace
