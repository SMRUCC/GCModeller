Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports std = System.Math

Namespace LinearRegression

    ''' <summary>
    ''' 单个目标基因的线性拟合模型
    ''' 
    ''' ```
    ''' target = intercept + beta_1 * regulator_1 + beta_2 * regulator_2 + ...
    ''' ```
    ''' 
    ''' 其中 <see cref="regulators"/> 与 <see cref="weights"/> 是一一对应的，
    ''' 第 i 个回归系数描述第 i 个调控因子对当前目标基因的表达贡献。
    ''' </summary>
    Public Class GeneRegressionModel

        ''' <summary>
        ''' 目标基因的编号
        ''' </summary>
        ''' <returns></returns>
        Public Property target As String

        ''' <summary>
        ''' 模型的常数项(截距)，即用户在公式之中写出来的 ``const``。
        ''' 
        ''' LASSO 求解器自身给出了标准化还原之后的截距，因此两种方法之下都有常数项。
        ''' </summary>
        ''' <returns></returns>
        Public Property intercept As Double

        ''' <summary>
        ''' 截距项的 t 值
        ''' </summary>
        ''' <returns></returns>
        Public Property interceptT As Double

        ''' <summary>
        ''' 截距项的显著性 p 值
        ''' </summary>
        ''' <returns></returns>
        Public Property interceptPvalue As Double

        ''' <summary>
        ''' 参与拟合的调控因子基因编号，顺序与 <see cref="weights"/> 一致。
        ''' </summary>
        ''' <returns></returns>
        Public Property regulators As String()

        ''' <summary>
        ''' 每一个调控因子的回归系数
        ''' </summary>
        ''' <returns></returns>
        Public Property weights As Double()

        ''' <summary>
        ''' 每一个回归系数对应的 t 值，无法计算的时候为空
        ''' </summary>
        ''' <returns></returns>
        Public Property tvalues As Double()

        ''' <summary>
        ''' 每一个回归系数对应的显著性 p 值，无法计算的时候为空
        ''' </summary>
        ''' <returns></returns>
        Public Property pvalues As Double()

        ''' <summary>
        ''' 参与拟合的样本数量
        ''' </summary>
        ''' <returns></returns>
        Public Property N As Integer

        ''' <summary>
        ''' 残差的自由度，即 ``样本数量 - 自变量个数``
        ''' </summary>
        ''' <returns></returns>
        Public Property df As Integer

        ''' <summary>
        ''' 拟合优度 R^2
        ''' </summary>
        ''' <returns></returns>
        Public Property r2 As Double

        ''' <summary>
        ''' 残差平方和
        ''' </summary>
        ''' <returns></returns>
        Public Property sse As Double

        ''' <summary>
        ''' 总平方和
        ''' </summary>
        ''' <returns></returns>
        Public Property sst As Double

        ''' <summary>
        ''' 当前模型所使用的求解方法
        ''' </summary>
        ''' <returns></returns>
        Public Property method As FittingMethod

        ''' <summary>
        ''' LASSO 模型在正则化路径上面所处的位置对应的 lambda 值，
        ''' 其它求解方法之下这个数值为零。
        ''' </summary>
        ''' <returns></returns>
        Public Property lambda As Double

        ''' <summary>
        ''' 拟合失败的时候的异常信息，求解成功的时候为空字符串
        ''' </summary>
        ''' <returns></returns>
        Public Property errorMessage As String

        ''' <summary>
        ''' 当前模型是否求解成功
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsValid As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return String.IsNullOrEmpty(errorMessage)
            End Get
        End Property

        ''' <summary>
        ''' 模型之中非零或者显著的调控因子的数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property nonZeroWeights As Integer
            Get
                If weights Is Nothing Then
                    Return 0
                End If

                Dim n As Integer = 0

                For i As Integer = 0 To weights.Length - 1
                    If weights(i) <> 0.0R Then
                        n += 1
                    End If
                Next

                Return n
            End Get
        End Property

        ''' <summary>
        ''' 拟合的均方根误差
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property rmse As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return If(N > 0, std.Sqrt(sse / N), 0)
            End Get
        End Property

        ''' <summary>
        ''' 获取指定调控因子对当前目标基因的回归系数，不存在的调控因子返回零。
        ''' </summary>
        ''' <param name="regulator">调控因子基因编号</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetWeight(regulator As String) As Double
            Dim i As Integer = IndexOf(regulator)

            Return If(i < 0, 0.0R, weights(i))
        End Function

        ''' <summary>
        ''' 获取指定调控因子的显著性 p 值，不存在的调控因子返回 1。
        ''' </summary>
        ''' <param name="regulator">调控因子基因编号</param>
        ''' <returns></returns>
        Public Function GetPvalue(regulator As String) As Double
            Dim i As Integer = IndexOf(regulator)

            If i < 0 OrElse pvalues Is Nothing Then
                Return 1
            Else
                Return pvalues(i)
            End If
        End Function

        ''' <summary>
        ''' 当前模型之中所有的非零回归系数，按照绝对值降序排列。
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function NonZeroCoefficients() As IEnumerable(Of NamedValue(Of Double))
            If weights Is Nothing OrElse regulators Is Nothing Then
                Return
            End If

            Dim order As Integer() = Enumerable _
                .Range(0, weights.Length) _
                .Where(Function(i) weights(i) <> 0.0R) _
                .OrderByDescending(Function(i) std.Abs(weights(i))) _
                .ToArray

            For Each i As Integer In order
                Yield New NamedValue(Of Double) With {
                    .Name = regulators(i),
                    .Value = weights(i)
                }
            Next
        End Function

        ''' <summary>
        ''' 查找调控因子在设计矩阵之中的列下标
        ''' </summary>
        ''' <param name="regulator"></param>
        ''' <returns>不存在的时候返回 -1</returns>
        Private Function IndexOf(regulator As String) As Integer
            If regulators Is Nothing OrElse weights Is Nothing Then
                Return -1
            End If

            For i As Integer = 0 To regulators.Length - 1
                If regulators(i) = regulator Then
                    Return i
                End If
            Next

            Return -1
        End Function

        ''' <summary>
        ''' 模型的文本表示，形如 ``target = 0.12 + 1.32*A - 0.87*B``
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim formula As New System.Text.StringBuilder()

            Call formula.Append(target)
            Call formula.Append(" = ")
            Call formula.Append(intercept.ToString("G4"))

            For Each coef As NamedValue(Of Double) In NonZeroCoefficients.Take(8)
                Dim sign As String = If(coef.Value >= 0, " + ", " - ")

                Call formula.Append(sign)
                Call formula.Append(std.Abs(coef.Value).ToString("G4"))
                Call formula.Append("*" & coef.Name)
            Next

            Call formula.Append($"   [R2={r2.ToString("F4")}]")

            Return formula.ToString
        End Function
    End Class
End Namespace
