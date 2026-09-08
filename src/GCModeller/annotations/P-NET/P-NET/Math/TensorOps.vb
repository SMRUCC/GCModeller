Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' P-NET 所需要的基础数值算子
''' </summary>
''' <remarks>
''' 这里的所有热路径算子都直接操作 <see cref="Tensor.Data"/> 所暴露的底层 <see cref="Double"/> 数组，
''' 目的是规避 <see cref="Tensor"/> 默认索引器与 <c>Tensor.MatMul</c> 带来的额外开销
''' （<c>Tensor.MatMul</c> 为 O(n^3) 的三重循环且逐元素走属性索引器）。
'''
''' 张量本身仍然统一使用 <see cref="Tensor"/> 类型，本模块只是为其补充了若干运算工具。
''' </remarks>
Public Module TensorOps

    ''' <summary>
    ''' 数值计算中用于防止除零与对数溢出的极小正数
    ''' </summary>
    ''' <returns>1e-12</returns>
    Public Const Epsilon As Double = 0.000000000001

    ''' <summary>
    ''' 生成服从正态分布的张量（Double 精度）
    ''' </summary>
    ''' <param name="shape">张量形状</param>
    ''' <param name="mean">均值</param>
    ''' <param name="stdDev">标准差</param>
    ''' <param name="seed">随机数种子，给出之后结果可复现</param>
    ''' <returns>使用 Box-Muller 变换生成的正态随机张量</returns>
    ''' <remarks>
    ''' <see cref="Tensor.RandomNormal"/> 内部使用 <c>CSng</c> 把结果截断为单精度，
    ''' 这里直接写 <see cref="Double"/> 数组，可以获得完整的双精度初始化值。
    ''' </remarks>
    Public Function RandomNormal(shape As Integer(), Optional mean As Double = 0.0,
                                Optional stdDev As Double = 1.0,
                                Optional seed As Integer? = Nothing) As Tensor
        Dim random As Random = If(seed.HasValue, New Random(seed.Value), New Random())
        Dim tensor As New Tensor(shape)
        Dim data As Double() = tensor.Data

        For i As Integer = 0 To data.Length - 1
            Dim u1 As Double = 1.0 - random.NextDouble()
            Dim u2 As Double = 1.0 - random.NextDouble()
            Dim gauss As Double = std.Sqrt(-2.0 * std.Log(u1)) * std.Sin(2.0 * std.PI * u2)

            data(i) = mean + stdDev * gauss
        Next

        Return tensor
    End Function

    ''' <summary>
    ''' Xavier 初始化，适用于 sigmoid / tanh 这类对称有界激活函数
    ''' </summary>
    ''' <param name="fanIn">输入维度</param>
    ''' <param name="fanOut">输出维度</param>
    ''' <param name="seed">随机数种子</param>
    ''' <returns>形状为 [fanIn, fanOut] 的权重张量</returns>
    Public Function XavierInit(fanIn As Integer, fanOut As Integer, Optional seed As Integer? = Nothing) As Tensor
        Dim stdDev As Double = std.Sqrt(2.0 / (fanIn + fanOut))

        Return RandomNormal(New Integer() {fanIn, fanOut}, 0.0, stdDev, seed)
    End Function

    ''' <summary>
    ''' 数值稳定的 sigmoid 函数
    ''' </summary>
    ''' <param name="x">输入值</param>
    ''' <returns>位于 (0, 1) 区间内的输出值</returns>
    Public Function Sigmoid(x As Double) As Double
        If x >= 0 Then
            Dim e As Double = std.Exp(-x)
            Return 1.0 / (1.0 + e)
        Else
            Dim e As Double = std.Exp(x)
            Return e / (1.0 + e)
        End If
    End Function

    ''' <summary>
    ''' 双曲正切函数，对应论文中隐藏层所使用的 <c>f(x) = (e^2x - 1) / (e^2x + 1)</c>
    ''' </summary>
    ''' <param name="x">输入值</param>
    ''' <returns>位于 (-1, 1) 区间内的输出值</returns>
    Public Function Tanh(x As Double) As Double
        Return std.Tanh(x)
    End Function

    ''' <summary>
    ''' 对矩阵逐元素施加 tanh 激活函数
    ''' </summary>
    ''' <param name="z">线性变换结果，形状为 [N, D]</param>
    ''' <returns>形状相同的激活值矩阵</returns>
    Public Function TanhActivate(z As Tensor) As Tensor
        Dim a As New Tensor(z.Shape)
        Dim src As Double() = z.Data
        Dim dst As Double() = a.Data

        For i As Integer = 0 To src.Length - 1
            dst(i) = std.Tanh(src(i))
        Next

        Return a
    End Function

    ''' <summary>
    ''' tanh 激活函数关于前一层激活值的导数，由激活值直接计算得到 <c>1 - a^2</c>
    ''' </summary>
    ''' <param name="a">tanh 的输出值</param>
    ''' <returns>导数值</returns>
    Public Function TanhDerivative(a As Double) As Double
        Return 1.0 - a * a
    End Function

    ''' <summary>
    ''' 把偏置向量按列加到矩阵的每一行上
    ''' </summary>
    ''' <param name="m">形状为 [N, D] 的矩阵</param>
    ''' <param name="bias">形状为 [1, D] 的偏置行向量</param>
    ''' <remarks>
    ''' 这里不能使用 <see cref="Tensor"/> 的 <c>+</c> 运算符做广播：
    ''' 该运算符在遇到 <c>[1, n] + [m, 1]</c> 时会触发"广播加法"特例产生外扩矩阵，
    ''' 因此偏置相加必须显式写循环完成。
    ''' </remarks>
    Public Sub AddBiasInPlace(m As Tensor, bias As Tensor)
        Dim data As Double() = m.Data
        Dim b As Double() = bias.Data
        Dim cols As Integer = m.Shape(1)
        Dim rows As Integer = m.Shape(0)

        For i As Integer = 0 To rows - 1
            Dim offset As Integer = i * cols

            For j As Integer = 0 To cols - 1
                data(offset + j) += b(j)
            Next
        Next
    End Sub

    ''' <summary>
    ''' 用偏置向量填充矩阵（先把矩阵按行填为偏置的复制，再加上原有内容）
    ''' </summary>
    ''' <param name="output">输出矩阵，形状为 [N, D]</param>
    ''' <param name="bias">形状为 [1, D] 的偏置行向量</param>
    Public Sub FillBias(output As Tensor, bias As Tensor)
        Dim data As Double() = output.Data
        Dim b As Double() = bias.Data
        Dim cols As Integer = output.Shape(1)
        Dim rows As Integer = output.Shape(0)

        For i As Integer = 0 To rows - 1
            Dim offset As Integer = i * cols

            For j As Integer = 0 To cols - 1
                data(offset + j) = b(j)
            Next
        Next
    End Sub

    ''' <summary>
    ''' 计算矩阵沿行方向（即按列）的累加和
    ''' </summary>
    ''' <param name="m">形状为 [N, D] 的矩阵</param>
    ''' <returns>长度为 D 的列和数组</returns>
    Public Function ColumnSum(m As Tensor) As Double()
        Dim data As Double() = m.Data
        Dim cols As Integer = m.Shape(1)
        Dim rows As Integer = m.Shape(0)
        Dim sums As Double() = New Double(cols - 1) {}

        For i As Integer = 0 To rows - 1
            Dim offset As Integer = i * cols

            For j As Integer = 0 To cols - 1
                sums(j) += data(offset + j)
            Next
        Next

        Return sums
    End Function

    ''' <summary>
    ''' 把矩阵沿行方向（即按列）累加到目标数组之上
    ''' </summary>
    ''' <param name="m">形状为 [N, D] 的矩阵</param>
    ''' <param name="target">长度为 D 的累加目标数组，计算会被原地累加进去</param>
    Public Sub AccumulateColumnSum(m As Tensor, target As Double())
        Dim data As Double() = m.Data
        Dim cols As Integer = m.Shape(1)
        Dim rows As Integer = m.Shape(0)

        For i As Integer = 0 To rows - 1
            Dim offset As Integer = i * cols

            For j As Integer = 0 To cols - 1
                target(j) += data(offset + j)
            Next
        Next
    End Sub

    ''' <summary>
    ''' 计算带类别权重的二元交叉熵损失
    ''' </summary>
    ''' <param name="p">预测概率，取值范围为 (0, 1)</param>
    ''' <param name="y">真实标签，取值为 0 或者 1</param>
    ''' <param name="positiveWeight">正样本的损失权重</param>
    ''' <param name="negativeWeight">负样本的损失权重</param>
    ''' <returns>单个样本的加权二元交叉熵损失值</returns>
    ''' <remarks>
    ''' 论文的数据集存在类别不平衡（333 例 CRPC / 转移性 对比 680 例原发性），
    ''' 因此按照训练集中的类别比例对损失加权，以抑制模型向多数类偏移。
    ''' </remarks>
    Public Function WeightedBinaryCrossEntropy(p As Double, y As Double,
                                              Optional positiveWeight As Double = 1.0,
                                              Optional negativeWeight As Double = 1.0) As Double
        Dim pc As Double = ClipProbability(p)
        Dim w As Double = If(y > 0.5, positiveWeight, negativeWeight)
        Dim loss As Double = -(y * std.Log(pc) + (1.0 - y) * std.Log(1.0 - pc))

        Return w * loss
    End Function

    ''' <summary>
    ''' 加权二元交叉熵损失关于预测概率的导数
    ''' </summary>
    ''' <param name="p">预测概率</param>
    ''' <param name="y">真实标签</param>
    ''' <param name="positiveWeight">正样本的损失权重</param>
    ''' <param name="negativeWeight">负样本的损失权重</param>
    ''' <returns>损失对预测概率的梯度</returns>
    Public Function WeightedBCEGradient(p As Double, y As Double,
                                        Optional positiveWeight As Double = 1.0,
                                        Optional negativeWeight As Double = 1.0) As Double
        Dim pc As Double = ClipProbability(p)
        Dim w As Double = If(y > 0.5, positiveWeight, negativeWeight)

        Return w * (pc - y) / (pc * (1.0 - pc))
    End Function

    ''' <summary>
    ''' 把概率值裁剪到 [Epsilon, 1 - Epsilon] 区间内，防止对数运算溢出
    ''' </summary>
    ''' <param name="p">原始概率值</param>
    ''' <returns>裁剪之后的概率值</returns>
    Public Function ClipProbability(p As Double) As Double
        If p < Epsilon Then
            Return Epsilon
        End If
        If p > 1.0 - Epsilon Then
            Return 1.0 - Epsilon
        End If

        Return p
    End Function

    ''' <summary>
    ''' 计算数组的均值
    ''' </summary>
    ''' <param name="x">输入数组</param>
    ''' <returns>算术平均值</returns>
    Public Function Mean(x As Double()) As Double
        If x Is Nothing OrElse x.Length = 0 Then
            Return 0.0
        End If

        Dim sum As Double = 0.0

        For i As Integer = 0 To x.Length - 1
            sum += x(i)
        Next

        Return sum / x.Length
    End Function

    ''' <summary>
    ''' 计算数组的标准差
    ''' </summary>
    ''' <param name="x">输入数组</param>
    ''' <returns>总体标准差</returns>
    Public Function StdDev(x As Double()) As Double
        If x Is Nothing OrElse x.Length < 2 Then
            Return 0.0
        End If

        Dim mu As Double = Mean(x)
        Dim sum As Double = 0.0

        For i As Integer = 0 To x.Length - 1
            Dim d As Double = x(i) - mu
            sum += d * d
        Next

        Return std.Sqrt(sum / x.Length)
    End Function

    ''' <summary>
    ''' 返回按值降序排列的下标序列
    ''' </summary>
    ''' <param name="x">输入数组</param>
    ''' <returns>下标数组，``result(0)`` 对应于原数组中最大值的下标</returns>
    Public Function ArgsortDescending(x As Double()) As Integer()
        Dim idx As Integer() = New Integer(x.Length - 1) {}

        For i As Integer = 0 To idx.Length - 1
            idx(i) = i
        Next

        Array.Sort(idx, Function(a As Integer, b As Integer) x(b).CompareTo(x(a)))

        Return idx
    End Function

End Module
