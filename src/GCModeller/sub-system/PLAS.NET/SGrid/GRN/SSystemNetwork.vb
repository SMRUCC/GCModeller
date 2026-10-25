Imports System.Globalization
Imports System.Text
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.SSystem.Runner
Imports PriorNet = SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork
Imports SSExperiment = SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.Experiment
Imports SSModel = SMRUCC.genomics.Analysis.SSystem.Script.Model
Imports SSVar = SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.var

Namespace GRN

    ''' <summary>
    ''' The fitted S-system gene regulatory network model.
    ''' (拟合得到的基因表达调控网络模型)
    ''' </summary>
    ''' <remarks>
    ''' 本对象是 SGrid 的门面类型：它同时持有
    ''' 1. 预处理之后的时序数据 <see cref="Series"/>
    ''' 2. 动力学参数 <see cref="Parameters"/>
    ''' 3. 稀疏化之后的调控拓扑 <see cref="Edges"/>
    ''' 并且可以生成可以直接被 PLAS 运行的 S-system 模型对象。
    ''' </remarks>
    Public Class SSystemNetwork

        ''' <summary>
        ''' 预处理之后的基因表达时序数据
        ''' </summary>
        ''' <returns></returns>
        Public Property Series As ExpressionTimeSeries
        ''' <summary>
        ''' 拟合得到的动力学参数
        ''' </summary>
        ''' <returns></returns>
        Public Property Parameters As SSystemParameters
        ''' <summary>
        ''' 稀疏化之后的有向调控边集合
        ''' </summary>
        ''' <returns></returns>
        Public Property Edges As RegulatoryEdge()
        ''' <summary>
        ''' 本次拟合所使用的配置
        ''' </summary>
        ''' <returns></returns>
        Public Property Options As FittingOptions
        ''' <summary>
        ''' 本次拟合所使用的先验知识网络（基因符号已经映射到规整化之后的编号）
        ''' </summary>
        ''' <returns></returns>
        Public Property Prior As PriorNet
        ''' <summary>
        ''' 先验知识与拟合拓扑之间的对照指标；没有启用先验的时候为空
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorStatistics As PriorMetrics
        ''' <summary>
        ''' 拟合过程的诊断信息
        ''' </summary>
        ''' <returns></returns>
        Public Property Warnings As New List(Of String)

        ''' <summary>
        ''' 从基因表达矩阵之中反演出基因表达调控网络的S-system模型
        ''' </summary>
        ''' <param name="matrix">行是基因、列是时间点的表达矩阵</param>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Public Shared Function Fit(matrix As Matrix, Optional options As FittingOptions = Nothing) As SSystemNetwork
            If matrix Is Nothing Then
                Throw New ArgumentNullException(NameOf(matrix))
            End If
            If options Is Nothing Then
                options = New FittingOptions()
            End If

            Call options.Validate()

            Dim series As ExpressionTimeSeries = ExpressionTimeSeries.FromMatrix(matrix, options)
            Dim fitOptions As FittingOptions = options.Clone()
            Dim prior As PriorNet = Nothing

            If options.UsePrior AndAlso Not options.Prior Is Nothing Then
                prior = options.Prior.Remap(geneNameMap(series))
                fitOptions.Prior = prior

                If options.Verbose Then
                    Call Console.WriteLine($"[INFO] prior knowledge: {options.Prior.EdgeCount()} edge(s) loaded, {prior.EdgeCount()} edge(s) matched to the expression matrix.")
                End If

                If prior.EdgeCount() = 0 Then
                    Call series.Warnings.Add("None of the prior knowledge edges was matched to the expression matrix, the prior constraint is disabled.")
                    fitOptions.Prior = Nothing
                    prior = Nothing
                End If
            End If

            If options.Verbose Then
                Call Console.WriteLine($"[INFO] {series.ToString}")
                Call Console.WriteLine($"[INFO] normalization={options.Normalize.ToString}, log-pseudo={options.PseudoCount}")

                For Each warn As String In series.Warnings
                    Call Console.WriteLine($"[WARN] {warn}")
                Next
            End If

            Dim slopes As Double()() = SlopeEstimator.Estimate(series, options.Denoise)
            Dim fitter As New DecoupledFitter(series, slopes, fitOptions)
            Dim parameters As SSystemParameters = fitter.Fit()
            Dim edges As RegulatoryEdge() = NetworkTopology.Sparsify(parameters, fitOptions, prior)

            If options.Verbose Then
                Call Console.WriteLine($"[INFO] {edges.Length} regulatory edge(s) was recovered by the sparsity threshold {options.Sparsity}.")
            End If

            If options.RefineTrajectory Then
                If options.Verbose Then
                    Call Console.WriteLine("[INFO] phase 2: refining the parameters by the trajectory fitting...")
                End If

                parameters = TrajectoryRefiner.Refine(series, parameters, fitOptions)
                parameters.TrajectoryRefined = True
                edges = NetworkTopology.Sparsify(parameters, fitOptions, prior)
            End If

            Return New SSystemNetwork With {
                .Series = series,
                .Parameters = parameters,
                .Edges = edges,
                .Options = fitOptions,
                .Prior = prior,
                .PriorStatistics = NetworkTopology.PriorMetrics(edges, prior),
                .Warnings = series.Warnings
            }
        End Function

        ''' <summary>
        ''' 构造「表达矩阵原始基因符号 → 规整化基因编号」的映射表，
        ''' 用于把先验知识网络映射到拟合过程所使用的编号空间之上
        ''' </summary>
        ''' <param name="series"></param>
        ''' <returns></returns>
        Private Shared Function geneNameMap(series As ExpressionTimeSeries) As System.Collections.Generic.IDictionary(Of String, String)
            Dim map As New System.Collections.Generic.Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

            For i As Integer = 0 To series.nGenes - 1
                Dim original As String = series.OriginalNames(i)
                Dim sanitized As String = series.GeneNames(i)

                If String.IsNullOrEmpty(original) Then
                    Continue For
                End If
                If Not map.ContainsKey(original) Then
                    Call map.Add(original, sanitized)
                End If
                ' 同时接受规整化之后的编号作为键，方便调用方直接使用规整化编号构建先验
                If Not map.ContainsKey(sanitized) Then
                    Call map.Add(sanitized, sanitized)
                End If
            Next

            Return map
        End Function

        ''' <summary>
        ''' 生成一个可以直接被 PLAS 运行的 S-system 模型对象
        ''' </summary>
        ''' <param name="experiments">挂载到模型之上的扰动实验</param>
        ''' <param name="finalTime">仿真的终止时间，非正数表示使用时间轴的终点</param>
        ''' <returns></returns>
        Public Function CreateModel(Optional experiments As SSExperiment() = Nothing,
                                    Optional finalTime As Double = 0) As SSModel

            Dim t As Double = If(finalTime > 0, finalTime, Series.TimeEnd)
            Dim equations As New System.Collections.Generic.List(Of SMRUCC.genomics.Analysis.SSystem.Script.SEquation)
            Dim vars As New System.Collections.Generic.List(Of SSVar)

            For i As Integer = 0 To Parameters.nGenes - 1
                Call equations.Add(New SMRUCC.genomics.Analysis.SSystem.Script.SEquation(
                    Parameters.GeneNames(i),
                    BuildExpression(Parameters, i)))

                Call vars.Add(New SSVar With {
                    .Id = Parameters.GeneNames(i),
                    .title = Parameters.OriginalNames(i),
                    .Value = Series.X(i)(0)
                })
            Next

            Dim model As New SSModel With {
                .Title = "S-system GRN model",
                .Comment = NetworkTopology.StabilityReport(Parameters),
                .FinalTime = t,
                .sEquations = equations.ToArray,
                .Vars = vars.ToArray,
                .Experiments = experiments
            }

            Return model
        End Function

        ''' <summary>
        ''' 创建用于本模型的仿真求解配置
        ''' </summary>
        ''' <param name="finalTime"></param>
        ''' <returns></returns>
        Public Function CreateRunOptions(Optional finalTime As Double = 0) As RunOptions
            Dim t As Double = If(finalTime > 0, finalTime, If(Options.FinalTime > 0, Options.FinalTime, Series.TimeEnd))

            Return Options.CreateRunOptions(t)
        End Function

        ''' <summary>
        ''' 生成目标基因的 S-system 幂律方程表达式
        ''' </summary>
        ''' <param name="parameters"></param>
        ''' <param name="i">靶基因在矩阵之中的行号</param>
        ''' <returns></returns>
        Public Shared Function BuildExpression(parameters As SSystemParameters, i As Integer) As String
            Dim sb As New StringBuilder()

            If parameters.alpha(i) > 0 Then
                Call sb.Append(FormatNumber(parameters.alpha(i)))

                For j As Integer = 0 To parameters.nGenes - 1
                    If parameters.G(i)(j) <> 0 Then
                        Call sb.Append("*")
                        Call sb.Append(parameters.GeneNames(j))
                        Call sb.Append("^")
                        Call sb.Append(FormatNumber(parameters.G(i)(j)))
                    End If
                Next
            End If

            If parameters.beta(i) > 0 Then
                If sb.Length > 0 Then
                    Call sb.Append("-")
                End If

                Call sb.Append(FormatNumber(parameters.beta(i)))

                For j As Integer = 0 To parameters.nGenes - 1
                    If parameters.H(i)(j) <> 0 Then
                        Call sb.Append("*")
                        Call sb.Append(parameters.GeneNames(j))
                        Call sb.Append("^")
                        Call sb.Append(FormatNumber(parameters.H(i)(j)))
                    End If
                Next
            End If

            If sb.Length = 0 Then
                Call sb.Append("0")
            End If

            Return sb.ToString
        End Function

        ''' <summary>
        ''' 把数值格式化为数学表达式引擎可以解析的文本形式
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Shared Function FormatNumber(x As Double) As String
            If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then
                Return "0"
            End If

            Dim s As String = x.ToString("0.#########", CultureInfo.InvariantCulture)

            If String.IsNullOrEmpty(s) Then
                Return "0"
            End If

            Return s
        End Function

        ''' <summary>
        ''' 生成拟合报告
        ''' </summary>
        ''' <returns></returns>
        Public Function Report() As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine($"S-system GRN fitting report")
            Call sb.AppendLine($"time series     : {Series.ToString}")
            Call sb.AppendLine($"normalization   : {Options.Normalize.ToString} (pseudo={Options.PseudoCount})")
            Call sb.AppendLine($"sparsity        : {Options.Sparsity}")
            Call sb.AppendLine($"max edges/gene  : {Options.MaxEdgesPerGene}")
            Call sb.AppendLine($"mean slope RMSE : {Parameters.MeanRMSE.ToString("G6")}")
            Call sb.AppendLine($"regulatory edges: {Edges.Length}")
            Call sb.AppendLine($"trajectory refit: {Parameters.TrajectoryRefined}")
            Call sb.AppendLine(NetworkTopology.StabilityReport(Parameters))

            If Not Prior Is Nothing Then
                Call sb.AppendLine()
                Call sb.AppendLine($"prior knowledge : {Prior.EdgeCount()} edge(s) mapped, {Prior.TypeStatistic()}")

                If Not PriorStatistics Is Nothing Then
                    Call sb.Append(PriorStatistics.Summary())
                End If
            End If

            If Not Warnings.IsNullOrEmpty Then
                Call sb.AppendLine()
                Call sb.AppendLine("warnings:")

                For Each warn As String In Warnings
                    Call sb.AppendLine($"  {warn}")
                Next
            End If

            Call sb.AppendLine()
            Call sb.Append(Parameters.Summary())

            Return sb.ToString
        End Function

        ''' <summary>
        ''' 把拟合结果的全部产物保存到指定的文件夹
        ''' </summary>
        ''' <param name="dir"></param>
        ''' <returns></returns>
        Public Function SaveAll(dir As String) As Boolean
            Return IO.NetworkWriter.SaveAll(Me, dir)
        End Function

        ''' <summary>
        ''' 把整个模型对象持久化保存为一个压缩包文件
        ''' </summary>
        ''' <param name="zipFile"></param>
        ''' <returns></returns>
        Public Function SaveZip(zipFile As String) As Boolean
            Call IO.ZipPersists.Save(Me, zipFile)

            Return True
        End Function

        ''' <summary>
        ''' 从压缩包文件之中重新加载模型对象
        ''' </summary>
        ''' <param name="zipFile"></param>
        ''' <returns></returns>
        Public Shared Function LoadZip(zipFile As String) As SSystemNetwork
            Return IO.ZipPersists.Load(zipFile)
        End Function

        Public Overrides Function ToString() As String
            Return $"S-system GRN: {Parameters.nGenes} genes, {Edges.Length} edges, mean RMSE={Parameters.MeanRMSE.ToString("G4")}"
        End Function
    End Class
End Namespace
