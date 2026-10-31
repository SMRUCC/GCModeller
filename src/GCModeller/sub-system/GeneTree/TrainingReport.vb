Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 逐基因拟合的训练报告：既可以在控制台打印摘要，也可以写成两张可读的表格文件
''' （<c>report.tsv</c> 与 <c>importance.tsv</c>），用于人工核查调控网络是否合理。
''' </summary>
Public Class TrainingReport

    ''' <summary>本次训练使用的配置</summary>
    Public Property Config As GeneTreeConfig

    ''' <summary>参与训练的细胞数量</summary>
    Public Property Cells As Integer

    ''' <summary>每个基因的模型（下标与表达矩阵的行一致，可能包含 <c>Nothing</c>）</summary>
    Public Property Models As GeneModel() = New GeneModel() {}

    ''' <summary>成功训练出梯度提升树的基因数量</summary>
    Public ReadOnly Property BoostedModels As Integer
        Get
            Return Models.SafeQuery.Count(Function(m) m IsNot Nothing AndAlso m.Kind = "xgboost")
        End Get
    End Property

    ''' <summary>退化为常数模型的基因数量</summary>
    Public ReadOnly Property ConstantModels As Integer
        Get
            Return Models.SafeQuery.Count(Function(m) m IsNot Nothing AndAlso m.Kind = "constant")
        End Get
    End Property

    ''' <summary>没有任何调控者的源节点数量</summary>
    Public ReadOnly Property SourceNodes As Integer
        Get
            Return Models.SafeQuery.Count(Function(m) m Is Nothing)
        End Get
    End Property

    ''' <summary>所有梯度提升树模型在验证集上的 R² 中位数</summary>
    Public ReadOnly Property MedianR2 As Double
        Get
            Return median(Function(m) m.Performance.r2)
        End Get
    End Property

    ''' <summary>所有梯度提升树模型在验证集上的 R² 均值</summary>
    Public ReadOnly Property MeanR2 As Double
        Get
            Dim values As Double() = r2Values()

            If values.Length = 0 Then
                Return 0.0R
            Else
                Return values.Average()
            End If
        End Get
    End Property

    ''' <summary>
    ''' R² 大于 0.05（说明该靶基因确实受到其调控者的非线性影响）的模型数量
    ''' </summary>
    Public ReadOnly Property InformativeModels As Integer
        Get
            Return r2Values().Count(Function(x) x > 0.05)
        End Get
    End Property

    ''' <summary>
    ''' 构造训练报告
    ''' </summary>
    ''' <param name="models">模型集合</param>
    ''' <param name="config">训练配置</param>
    ''' <param name="cells">参与训练的细胞数量</param>
    ''' <returns></returns>
    Public Shared Function FromModels(models As GeneModel(),
                                      config As GeneTreeConfig,
                                      cells As Integer) As TrainingReport

        Return New TrainingReport With {
            .Models = If(models, New GeneModel() {}),
            .Config = config,
            .Cells = cells
        }
    End Function

    Private Function r2Values() As Double()
        Return Models _
            .SafeQuery _
            .Where(Function(m) m IsNot Nothing AndAlso m.Kind = "xgboost") _
            .Select(Function(m) m.Performance.r2) _
            .ToArray
    End Function

    Private Function median(selector As Func(Of GeneModel, Double)) As Double
        Dim values As Double() = Models _
            .SafeQuery _
            .Where(Function(m) m IsNot Nothing AndAlso m.Kind = "xgboost") _
            .Select(selector) _
            .OrderBy(Function(x) x) _
            .ToArray

        If values.Length = 0 Then
            Return 0.0R
        Else
            Return values(values.Length \ 2)
        End If
    End Function

    ''' <summary>
    ''' 把报告写盘：<c>{folder}/report.tsv</c> 与 <c>{folder}/importance.tsv</c>
    ''' </summary>
    ''' <param name="folder">目标文件夹（不存在时自动创建）</param>
    Public Sub Write(folder As String)
        If String.IsNullOrEmpty(folder) Then
            Return
        End If
        If Not Directory.Exists(folder) Then
            Call Directory.CreateDirectory(folder)
        End If

        Call WriteTable(Path.Combine(folder, "report.tsv"))
        Call WriteImportance(Path.Combine(folder, "importance.tsv"))
    End Sub

    ''' <summary>
    ''' 写出逐基因的拟合优度表
    ''' </summary>
    ''' <param name="file">目标文件路径</param>
    Public Sub WriteTable(file As String)
        Using writer As New StreamWriter(file, False, Encoding.UTF8)
            Call writer.WriteLine("gene,kind,nRegulators,nTrain,nValidate,r2,rmse,baselineRmse,improvement,trees,minValue,maxValue")

            For Each model As GeneModel In Models.SafeQuery.Where(Function(m) m IsNot Nothing)
                Dim p As ModelPerformance = model.Performance

                Call writer.WriteLine(String.Join(",",
                    model.Gene,
                    p.kind,
                    p.nRegulators.ToString,
                    p.nTrain.ToString,
                    p.nValidate.ToString,
                    number(p.r2),
                    number(p.rmse),
                    number(p.baselineRmse),
                    number(p.improvement),
                    p.trees.ToString,
                    number(p.minValue),
                    number(p.maxValue)))
            Next
        End Using
    End Sub

    ''' <summary>
    ''' 写出逐调控因子的重要性表
    ''' </summary>
    ''' <param name="file">目标文件路径</param>
    Public Sub WriteImportance(file As String)
        Using writer As New StreamWriter(file, False, Encoding.UTF8)
            Call writer.WriteLine("gene,regulator,rank,mi,correlation,splits,sensitivity")

            For Each model As GeneModel In Models.SafeQuery.Where(Function(m) m IsNot Nothing)
                For Each item As RegulatorImportance In model.Performance.importance.SafeQuery
                    Call writer.WriteLine(String.Join(",",
                        model.Gene,
                        item.regulator,
                        item.rank.ToString,
                        number(item.mi),
                        number(item.correlation),
                        item.splits.ToString,
                        number(item.sensitivity)))
                Next
            Next
        End Using
    End Sub

    Private Shared Function number(x As Double) As String
        Return x.ToString("G6", Globalization.CultureInfo.InvariantCulture)
    End Function

    ''' <summary>
    ''' 多行摘要文本
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Dim sb As New StringBuilder()

        Call sb.AppendLine($"训练完成: {BoostedModels} 个 xgboost 模型、{ConstantModels} 个常数模型、" &
                           $"{SourceNodes} 个无调控者的源节点，细胞数 {Cells}")
        Call sb.AppendLine($"拟合优度: 中位 R² = {MedianR2.ToString("F4")}、平均 R² = {MeanR2.ToString("F4")}、" &
                           $"{InformativeModels} 个模型的 R² > 0.05")
        Call sb.Append($"配置: {If(Config Is Nothing, "N/A", Config.ToString)}")

        Return sb.ToString()
    End Function

End Class

