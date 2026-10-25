Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq

Namespace GRNDemo

    ''' <summary>
    ''' The command line options of the SGrid GRN demo.
    ''' (SGrid 基因调控网络演示程序的命令行参数)
    ''' </summary>
    Public Class DemoOptions

        ''' <summary>
        ''' pseudotime 排序之后的基因表达矩阵（行是基因、列是细胞）
        ''' </summary>
        ''' <returns></returns>
        Public Property MatrixFile As String = "K:\hsa_grn\20261025\outputs\pseudotime_ordered_expression_matrix.csv"
        ''' <summary>
        ''' 细胞到 pseudotime 的映射表
        ''' </summary>
        ''' <returns></returns>
        Public Property MappingFile As String = "K:\hsa_grn\20261025\outputs\cell_pseudotime_mapping.csv"
        ''' <summary>
        ''' 先验调控网络表
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorFile As String = "G:\vcc2026\data\TF_target_prior_curated.csv"
        ''' <summary>
        ''' 全部产物的输出文件夹
        ''' </summary>
        ''' <returns></returns>
        Public Property OutDir As String = "K:\hsa_grn\20261025\outputs\sgrid_demo"

        ''' <summary>
        ''' 子集矩阵之中需要提取的基因数量
        ''' </summary>
        ''' <returns></returns>
        Public Property ExtractGenes As Integer = 200
        ''' <summary>
        ''' 最终参与拟合的基因数量
        ''' </summary>
        ''' <returns></returns>
        Public Property FitGenes As Integer = 30
        ''' <summary>
        ''' pseudotime 的分箱数量
        ''' </summary>
        ''' <returns></returns>
        Public Property Bins As Integer = 60

        ''' <summary>
        ''' 表达矩阵的归一化方法：none / log2 / zscore / quantile
        ''' </summary>
        ''' <returns></returns>
        Public Property Normalize As String = "log2"
        ''' <summary>
        ''' 稀疏化阈值
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 真实的单细胞 pseudotime 数据经过分箱与对数变换之后动态范围被压缩，
        ''' 净调控弹性的量级通常在 1E-2 附近（而非合成的金标准数据上的 0.5），
        ''' 故这里的缺省阈值取得比库的缺省值（0.2）小很多。
        ''' </remarks>
        Public Property Sparsity As Double = 0.01R
        ''' <summary>
        ''' 每一个靶基因最多保留的调控边数量
        ''' </summary>
        ''' <returns></returns>
        Public Property MaxEdgesPerGene As Integer = 4
        ''' <summary>
        ''' 数值微分之前的滑动平均窗口（奇数）
        ''' </summary>
        ''' <returns></returns>
        Public Property Smooth As Integer = 9
        ''' <summary>
        ''' 动力学阶的 L1 稀疏惩罚权重
        ''' </summary>
        ''' <returns></returns>
        Public Property L1Penalty As Double = 0
        ''' <summary>
        ''' 稳定性的自环惩罚权重
        ''' </summary>
        ''' <returns></returns>
        Public Property StabilityPenalty As Double = 0.05R
        ''' <summary>
        ''' 每一个靶基因预筛选的候选调控因子数量（按相关性挑选）；零表示使用全部基因
        ''' </summary>
        ''' <returns></returns>
        Public Property MaxRegulators As Integer = 0
        ''' <summary>
        ''' 差分进化的种群大小
        ''' </summary>
        ''' <returns></returns>
        Public Property DEPopulation As Integer = 40
        ''' <summary>
        ''' 差分进化的最大迭代次数
        ''' </summary>
        ''' <returns></returns>
        Public Property DEIterations As Integer = 1200
        ''' <summary>
        ''' 先验知识对非先验交互的软惩罚强度
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 在这批数据之上取 1.0 时先验支持率与方向一致率都比较理想；
        ''' 该系数越大则网络越贴近先验，但斜率残差也会随之上升
        ''' （实测 1.5 时先验支持率不再提升，而 mean slope RMSE 由 0.10 升到 0.105）
        ''' </remarks>
        Public Property PriorPenalty As Double = 0.2R
        ''' <summary>
        ''' 先验知识对调控方向冲突的惩罚强度
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorSignPenalty As Double = 0.2R
        ''' <summary>
        ''' 稀疏化阶段对先验支持边的排序加权系数
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorRankBoost As Double = 1.0R
        ''' <summary>
        ''' 随机数种子
        ''' </summary>
        ''' <returns></returns>
        Public Property Seed As Integer = 1234567
        ''' <summary>
        ''' 仿真积分的步长
        ''' </summary>
        ''' <returns></returns>
        Public Property Precise As Double = 0.1R
        ''' <summary>
        ''' 虚拟扰动实验之中需要检验的枢纽基因数量
        ''' </summary>
        ''' <returns></returns>
        Public Property Hubs As Integer = 5
        ''' <summary>
        ''' 是否是并行执行拟合
        ''' </summary>
        ''' <returns></returns>
        Public Property Parallel As Boolean = True
        ''' <summary>
        ''' 是否静默执行
        ''' </summary>
        ''' <returns></returns>
        Public Property Quiet As Boolean = False
        ''' <summary>
        ''' 是否忽略子集矩阵与基因清单的缓存，强制重新扫描表达矩阵
        ''' </summary>
        ''' <returns></returns>
        Public Property Force As Boolean = False
        ''' <summary>
        ''' 需要执行的子命令
        ''' </summary>
        ''' <returns></returns>
        Public Property Command As String = "all"

        ''' <summary>
        ''' 子集矩阵文件的路径
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SubsetFile As String
            Get
                Return Path.Combine(OutDir, "subset_expression_matrix.csv")
            End Get
        End Property

        ''' <summary>
        ''' 子集基因清单文件的路径
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SubsetGeneFile As String
            Get
                Return Path.Combine(OutDir, "subset_genes.txt")
            End Get
        End Property

        ''' <summary>
        ''' 表达矩阵基因清单的缓存文件路径
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MatrixGeneFile As String
            Get
                Return Path.Combine(OutDir, "matrix_genes.txt")
            End Get
        End Property

        ''' <summary>
        ''' 分箱之后的表达矩阵文件路径
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property BinnedFile As String
            Get
                Return Path.Combine(OutDir, "binned_expression_matrix.csv")
            End Get
        End Property

        ''' <summary>
        ''' 拟合模型压缩包的路径
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ModelZipFile As String
            Get
                Return Path.Combine(OutDir, "sgrid_network.zip")
            End Get
        End Property

        ''' <summary>
        ''' 虚拟扰动实验的产物文件夹
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property PerturbDir As String
            Get
                Return Path.Combine(OutDir, "perturb")
            End Get
        End Property

        ''' <summary>
        ''' 拟合产物（表格与模型脚本）的文件夹
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FitDir As String
            Get
                Return Path.Combine(OutDir, "fit")
            End Get
        End Property

        ''' <summary>
        ''' 解析命令行参数
        ''' </summary>
        ''' <param name="args"></param>
        ''' <returns></returns>
        Public Shared Function Parse(args As String()) As DemoOptions
            Dim options As New DemoOptions()
            Dim values As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            Dim flags As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Dim i As Integer = 0

            If args.IsNullOrEmpty Then
                Return options
            End If

            While i < args.Length
                Dim arg As String = If(args(i), "").Trim()

                If String.IsNullOrEmpty(arg) Then
                    i += 1
                    Continue While
                End If

                If arg.StartsWith("--", StringComparison.Ordinal) Then
                    Dim key As String = arg.Substring(2)

                    If i + 1 < args.Length AndAlso Not If(args(i + 1), "").StartsWith("--", StringComparison.Ordinal) Then
                        values(key) = args(i + 1)
                        i += 2
                    Else
                        Call flags.Add(key)
                        i += 1
                    End If
                ElseIf arg.StartsWith("-", StringComparison.Ordinal) AndAlso arg.Length = 2 Then
                    Call flags.Add(arg.Substring(1))
                    i += 1
                Else
                    options.Command = arg.ToLowerInvariant()
                    i += 1
                End If
            End While

            options.MatrixFile = textOf(values, "matrix", options.MatrixFile)
            options.MappingFile = textOf(values, "mapping", options.MappingFile)
            options.PriorFile = textOf(values, "prior", options.PriorFile)
            options.OutDir = textOf(values, "out", options.OutDir)
            options.ExtractGenes = integerOf(values, "extract-genes", options.ExtractGenes)
            options.FitGenes = integerOf(values, "fit-genes", options.FitGenes)
            options.Bins = integerOf(values, "bins", options.Bins)
            options.Normalize = textOf(values, "normalize", options.Normalize)
            options.Sparsity = doubleOf(values, "sparsity", options.Sparsity)
            options.MaxEdgesPerGene = integerOf(values, "max-edges", options.MaxEdgesPerGene)
            options.MaxRegulators = integerOf(values, "max-regulators", options.MaxRegulators)
            options.Smooth = integerOf(values, "smooth", options.Smooth)
            options.L1Penalty = doubleOf(values, "l1", options.L1Penalty)
            options.StabilityPenalty = doubleOf(values, "stability", options.StabilityPenalty)
            options.DEPopulation = integerOf(values, "de-pop", options.DEPopulation)
            options.DEIterations = integerOf(values, "de-iters", options.DEIterations)
            options.PriorPenalty = doubleOf(values, "prior-penalty", options.PriorPenalty)
            options.PriorSignPenalty = doubleOf(values, "prior-sign-penalty", options.PriorSignPenalty)
            options.PriorRankBoost = doubleOf(values, "prior-rank-boost", options.PriorRankBoost)
            options.Seed = integerOf(values, "seed", options.Seed)
            options.Precise = doubleOf(values, "precise", options.Precise)
            options.Hubs = integerOf(values, "hubs", options.Hubs)
            options.Parallel = Not flags.Contains("serial")
            options.Quiet = flags.Contains("quiet") OrElse flags.Contains("q")
            options.Force = flags.Contains("force")

            Return options
        End Function

        ''' <summary>
        ''' 命令行的用法说明
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property Usage As String
            Get
                Dim sb As New StringBuilder()

                Call sb.AppendLine("SGrid gene regulatory network demo")
                Call sb.AppendLine()
                Call sb.AppendLine("usage: test [command] [options]")
                Call sb.AppendLine()
                Call sb.AppendLine("commands:")
                Call sb.AppendLine("  all       extract -> fit -> persist -> perturb (default)")
                Call sb.AppendLine("  extract   only export the gene subset matrix from the huge expression matrix")
                Call sb.AppendLine("  fit       extract (if needed), fit the network and persist the model zip archive")
                Call sb.AppendLine("  persist   reload the model zip archive and verify it round trip")
                Call sb.AppendLine("  perturb   run the virtual perturbation screening on the saved model")
                Call sb.AppendLine("  selftest  run the built in 3 genes golden standard self test")
                Call sb.AppendLine("  help      show this message")
                Call sb.AppendLine()
                Call sb.AppendLine("options:")
                Call sb.AppendLine("  --matrix <file>          pseudotime ordered expression matrix")
                Call sb.AppendLine("  --mapping <file>         cell id to pseudotime mapping table")
                Call sb.AppendLine("  --prior <file>           curated TF to target prior knowledge table")
                Call sb.AppendLine("  --out <dir>              output directory")
                Call sb.AppendLine("  --extract-genes <n>      number of genes exported into the subset matrix (default 200)")
                Call sb.AppendLine("  --fit-genes <n>          number of genes used by the fitting (default 30)")
                Call sb.AppendLine("  --bins <n>               number of pseudotime bins (default 60)")
                Call sb.AppendLine("  --normalize <method>     none | log2 | zscore | quantile (default log2)")
                Call sb.AppendLine("  --sparsity <x>           sparsity threshold of the network topology (default 0.01)")
                Call sb.AppendLine("  --max-edges <n>          max edges per target gene (default 4)")
                Call sb.AppendLine("  --max-regulators <n>     candidate regulators per target, by correlation (0 = all genes)")
                Call sb.AppendLine("  --smooth <n>             moving average window before differentiation (default 9)")
                Call sb.AppendLine("  --l1 <x>                 L1 penalty weight of the kinetic orders (default 0)")
                Call sb.AppendLine("  --stability <x>          penalty weight of a positive self loop (default 0.05)")
                Call sb.AppendLine("  --de-pop <n>             differential evolution population size (default 40)")
                Call sb.AppendLine("  --de-iters <n>           differential evolution iterations (default 1200)")
                Call sb.AppendLine("  --prior-penalty <x>      soft penalty for the interactions which are not supported by the prior (default 0.2)")
                Call sb.AppendLine("  --prior-sign-penalty <x> penalty for the direction conflict against the prior (default 0.2)")
                Call sb.AppendLine("  --prior-rank-boost <x>   ranking boost of the prior supported edges (default 1.0)")
                Call sb.AppendLine("  --seed <n>               random seed (default 1234567)")
                Call sb.AppendLine("  --precise <x>            integration step of the simulation (default 0.1)")
                Call sb.AppendLine("  --hubs <n>               number of hub genes used by the perturbation (default 5)")
                Call sb.AppendLine("  --serial                 disable the parallel fitting")
                Call sb.AppendLine("  --force                  ignore the cached subset matrix and gene list")
                Call sb.AppendLine("  --quiet                  only print the stage summaries")

                Return sb.ToString()
            End Get
        End Property

        Private Shared Function textOf(values As Dictionary(Of String, String), key As String, fallback As String) As String
            Dim value As String = Nothing

            If values.TryGetValue(key, value) AndAlso Not String.IsNullOrEmpty(value) Then
                Return value
            Else
                Return fallback
            End If
        End Function

        Private Shared Function integerOf(values As Dictionary(Of String, String), key As String, fallback As Integer) As Integer
            Dim value As String = Nothing
            Dim parsed As Integer

            If values.TryGetValue(key, value) AndAlso Integer.TryParse(value, parsed) Then
                Return parsed
            Else
                Return fallback
            End If
        End Function

        Private Shared Function doubleOf(values As Dictionary(Of String, String), key As String, fallback As Double) As Double
            Dim value As String = Nothing
            Dim parsed As Double

            If values.TryGetValue(key, value) AndAlso Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, parsed) Then
                Return parsed
            Else
                Return fallback
            End If
        End Function
    End Class
End Namespace
