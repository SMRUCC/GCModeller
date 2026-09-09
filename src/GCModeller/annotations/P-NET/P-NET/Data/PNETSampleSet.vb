Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' P-NET 的样本数据集
''' </summary>
''' <remarks>
''' 特征矩阵 <see cref="Features"/> 的形状为 [样本数, 基因数 × 3]，
''' 其中每一个基因按顺序占用 3 列，依次为体细胞突变、拷贝数扩增、拷贝数缺失，
''' 取值均为 0 或者 1 的二值变量（只保留高水平扩增与深缺失的两态编码，
''' 与论文主模型所采用的编码方式一致）。
'''
''' 标签 <see cref="Labels"/> 取 0 表示原发性前列腺癌，取 1 表示转移性 / 去势抵抗性前列腺癌。
''' </remarks>
Public Class PNETSampleSet

    ''' <summary>
    ''' 样本编号数组
    ''' </summary>
    ''' <returns>样本名数组</returns>
    Public Property SampleNames As String()

    ''' <summary>
    ''' 基因名数组，其顺序必须与特征矩阵的列顺序一致
    ''' </summary>
    ''' <returns>基因名数组</returns>
    Public Property GeneNames As String()

    ''' <summary>
    ''' 特征矩阵，形状为 [样本数, 基因数 × 3]
    ''' </summary>
    ''' <returns>特征张量</returns>
    Public Property Features As Tensor

    ''' <summary>
    ''' 标签数组，取值为 0 或者 1
    ''' </summary>
    ''' <returns>标签数组</returns>
    Public Property Labels As Double()

    ''' <summary>
    ''' 样本数量
    ''' </summary>
    ''' <returns>样本数</returns>
    Public ReadOnly Property Count As Integer
        Get
            If Features Is Nothing Then
                Return 0
            End If

            Return Features.Shape(0)
        End Get
    End Property

    ''' <summary>
    ''' 输入特征维度，等于基因数量的 3 倍
    ''' </summary>
    ''' <returns>特征维度</returns>
    Public ReadOnly Property InputSize As Integer
        Get
            If Features Is Nothing Then
                Return 0
            End If

            Return Features.Shape(1)
        End Get
    End Property

    ''' <summary>
    ''' 正样本（转移性 / 耐药）数量
    ''' </summary>
    ''' <returns>正样本数</returns>
    Public ReadOnly Property PositiveCount As Integer
        Get
            Dim n As Integer = 0

            If Labels Is Nothing Then
                Return 0
            End If

            For i As Integer = 0 To Labels.Length - 1
                If Labels(i) > 0.5 Then
                    n += 1
                End If
            Next

            Return n
        End Get
    End Property

    ''' <summary>
    ''' 负样本（原发性）数量
    ''' </summary>
    ''' <returns>负样本数</returns>
    Public ReadOnly Property NegativeCount As Integer
        Get
            Return Count - PositiveCount
        End Get
    End Property

    ''' <summary>
    ''' 正样本在数据集之中的占比
    ''' </summary>
    ''' <returns>正样本比例，位于 [0, 1] 区间内</returns>
    Public ReadOnly Property PositiveRatio As Double
        Get
            If Count = 0 Then
                Return 0.0
            End If

            Return PositiveCount / Count
        End Get
    End Property

    ''' <summary>
    ''' 创建一个空的数据集
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 创建数据集
    ''' </summary>
    ''' <param name="features">特征矩阵，形状为 [样本数, 基因数 × 3]</param>
    ''' <param name="labels">标签数组</param>
    ''' <param name="geneNames">基因名数组</param>
    ''' <param name="sampleNames">样本名数组，取 Nothing 时自动编号</param>
    Public Sub New(features As Tensor, labels As Double(), geneNames As String(),
                   Optional sampleNames As String() = Nothing)

        Me.Features = features
        Me.Labels = labels
        Me.GeneNames = geneNames

        If sampleNames Is Nothing Then
            sampleNames = New String(features.Shape(0) - 1) {}

            For i As Integer = 0 To sampleNames.Length - 1
                sampleNames(i) = $"S{i + 1}"
            Next
        End If

        Me.SampleNames = sampleNames
    End Sub

    ''' <summary>
    ''' 按照给定的下标取出一个子数据集
    ''' </summary>
    ''' <param name="indices">样本下标数组</param>
    ''' <returns>只包含指定样本的新数据集对象</returns>
    ''' <remarks>
    ''' 特征矩阵会被按行复制出来，因此返回的数据集与原始数据集之间不共享内存。
    ''' </remarks>
    Public Function Subset(indices As Integer()) As PNETSampleSet
        Dim rows As Integer = indices.Length
        Dim cols As Integer = InputSize
        Dim subFeatures As New Tensor(rows, cols)
        Dim src As Double() = Features.Data
        Dim dst As Double() = subFeatures.Data
        Dim subLabels As Double() = New Double(rows - 1) {}
        Dim subNames As String() = New String(rows - 1) {}

        For i As Integer = 0 To rows - 1
            Dim srcRow As Integer = indices(i)

            Array.Copy(src, srcRow * cols, dst, i * cols, cols)

            subLabels(i) = Labels(srcRow)
            subNames(i) = SampleNames(srcRow)
        Next

        Return New PNETSampleSet(subFeatures, subLabels, GeneNames, subNames)
    End Function

    ''' <summary>
    ''' 取出指定样本的二值特征行
    ''' </summary>
    ''' <param name="index">样本下标</param>
    ''' <returns>长度为 <see cref="InputSize"/> 的特征向量</returns>
    Public Function GetFeatureVector(index As Integer) As Double()
        Dim cols As Integer = InputSize
        Dim vec As Double() = New Double(cols - 1) {}

        Array.Copy(Features.Data, index * cols, vec, 0, cols)

        Return vec
    End Function

    ''' <summary>
    ''' 把数据集写出到指定目录
    ''' </summary>
    ''' <param name="directory">输出目录，不存在时会被自动创建</param>
    ''' <remarks>
    ''' 会写出两个文件：
    '''
    ''' + <c>features.csv</c>：第一列为样本编号，其余列为 <c>基因名_mut / _amp / _del</c> 三元组；
    ''' + <c>labels.csv</c>：两列，分别为样本编号与标签。
    ''' </remarks>
    Public Sub Save(directory As String)
        If Not System.IO.Directory.Exists(directory) Then
            Call System.IO.Directory.CreateDirectory(directory)
        End If

        Dim featureNames As New List(Of String)()

        For g As Integer = 0 To GeneNames.Length - 1
            For Each suffix As String In PathwayHierarchy.AlterationTypes
                featureNames.Add(GeneNames(g) & suffix)
            Next
        Next

        Dim sb As New StringBuilder()

        Call sb.Append("sample")

        For Each name As String In featureNames
            Call sb.Append(",").Append(name)
        Next

        Call sb.AppendLine()

        Dim data As Double() = Features.Data
        Dim cols As Integer = InputSize

        For i As Integer = 0 To Count - 1
            Call sb.Append(SampleNames(i))

            For j As Integer = 0 To cols - 1
                Call sb.Append(",").Append(data(i * cols + j).ToString("0"))
            Next

            Call sb.AppendLine()
        Next

        Call File.WriteAllText(Path.Combine(directory, "features.csv"), sb.ToString(), Encoding.UTF8)

        sb.Clear()
        Call sb.AppendLine("sample,label")

        For i As Integer = 0 To Count - 1
            Call sb.Append(SampleNames(i)).Append(",").Append(Labels(i).ToString("0")).AppendLine()
        Next

        Call File.WriteAllText(Path.Combine(directory, "labels.csv"), sb.ToString(), Encoding.UTF8)
    End Sub

    ''' <summary>
    ''' 生成数据集的文字描述
    ''' </summary>
    ''' <returns>形如 ``Dataset[600 samples x 144 features, positive=33.00%]`` 的描述文本</returns>
    Public Overrides Function ToString() As String
        Return $"Dataset[{Count} samples x {InputSize} features, positive={(PositiveRatio * 100).ToString("F2")}%]"
    End Function

End Class

''' <summary>
''' 按照训练集 / 验证集 / 测试集划分之后的数据集三元组
''' </summary>
Public Class DataSplit

    ''' <summary>
    ''' 训练集
    ''' </summary>
    ''' <returns>训练数据集</returns>
    Public Property Train As PNETSampleSet

    ''' <summary>
    ''' 验证集
    ''' </summary>
    ''' <returns>验证数据集</returns>
    Public Property Validation As PNETSampleSet

    ''' <summary>
    ''' 测试集
    ''' </summary>
    ''' <returns>测试数据集</returns>
    Public Property Test As PNETSampleSet

    ''' <summary>
    ''' 生成数据划分结果的文字描述
    ''' </summary>
    ''' <returns>多行描述文本</returns>
    Public Overrides Function ToString() As String
        Dim sb As New System.Text.StringBuilder()

        Call sb.AppendLine($"train      : {Train}")
        Call sb.AppendLine($"validation : {Validation}")
        Call sb.AppendLine($"test       : {Test}")

        Return sb.ToString()
    End Function

End Class
