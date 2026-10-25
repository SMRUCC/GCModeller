Imports System.Globalization
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics.Data
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.SGrid.GRN
Imports SMRUCC.genomics.Analysis.SGrid.Perturbation
Imports SMRUCC.genomics.Analysis.SSystem.Runner
Imports SSCompiler = SMRUCC.genomics.Analysis.SSystem.Script.ScriptCompiler
Imports SSEquation = SMRUCC.genomics.Analysis.SSystem.Script.SEquation
Imports SSExperiment = SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.Experiment
Imports SSModel = SMRUCC.genomics.Analysis.SSystem.Script.Model

''' <summary>
''' 基因表达调控网络拟合与虚拟扰动实验的命令行实现
''' </summary>
''' <remarks>
''' 这些函数由 <see cref="CLI">CLI</see> 模块之中的 ExportAPI 方法转发调用，
''' 因为底层的 RunCLI 只会反射扫描单个类型之上的 ExportAPI 成员。
''' </remarks>
Public Module GRN

    ''' <summary>
    ''' 从基因表达矩阵之中拟合 S-system 基因调控网络
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    Public Function Fit(args As CommandLine) As Integer
        Dim input As String = optStr(args, "-i", "")

        If String.IsNullOrEmpty(input) OrElse Not File.Exists(input) Then
            Call Console.Error.WriteLine($"ERROR: the input expression matrix file '{input}' was not found!")
            Return 1
        End If

        Dim outDir As String = optStr(args, "-o", "")

        If String.IsNullOrEmpty(outDir) Then
            outDir = Path.Combine(Path.GetDirectoryName(input), Path.GetFileNameWithoutExtension(input) & ".grn")
        End If

        Dim options As New FittingOptions With {
            .Normalize = parseNormalize(optStr(args, "/normalize", "log2")),
            .PseudoCount = optVal(args, "/pseudo", 1.0R),
            .Sparsity = optVal(args, "/sparsity", 0.2R),
            .MaxEdgesPerGene = optInt(args, "/max-edges", 3),
            .MaxRegulators = optInt(args, "/max-regulators", 0),
            .KeepSelfLoop = Not optBool(args, "/no-self-loop", False),
            .L1Penalty = optVal(args, "/l1", 0),
            .AlphaBounds = optRange(args, "/alpha", New DoubleRange(0, 10)),
            .BetaBounds = optRange(args, "/beta", New DoubleRange(0, 10)),
            .GBounds = optRange(args, "/g", New DoubleRange(-3, 3)),
            .HBounds = optRange(args, "/h", New DoubleRange(-3, 3)),
            .DEPopulationSize = optInt(args, "/de-pop", 60),
            .DEMaxIterations = optInt(args, "/de-iter", 3000),
            .DEF = optVal(args, "/de-f", 0.8R),
            .DECR = optVal(args, "/de-cr", 0.7R),
            .LBFGSIterations = optInt(args, "/lb", 500),
            .RefineTrajectory = optBool(args, "/refine", False),
            .RefineMaxIterations = optInt(args, "/refine-iter", 60),
            .Denoise = Not optBool(args, "/no-denoise", False),
            .Seed = optInt(args, "/seed", 0),
            .Parallel = Not optBool(args, "/no-parallel", False),
            .Verbose = Not optBool(args, "/silent", False),
            .FinalTime = optVal(args, "/time", 0)
        }

        Dim times As Double() = optList(args, "/time-points")

        If Not times.IsNullOrEmpty Then
            options.TimePoints = times
        End If

        Call Console.WriteLine($"[INFO] loading the expression matrix: {input}")

        Dim matrix As Matrix = Matrix.LoadData(input)

        Call Console.WriteLine($"[INFO] {matrix.size} gene(s) x {matrix.sample_count} sample(s)")

        Dim network As SSystemNetwork = SSystemNetwork.Fit(matrix, options)

        Call Console.WriteLine()
        Call Console.WriteLine(network.Report())

        Dim ok As Boolean = network.SaveAll(outDir)

        Call Console.WriteLine()
        Call Console.WriteLine($"The fitted model and all of the output files were saved into the directory: {outDir}")

        Return If(ok, 0, 1)
    End Function

    ''' <summary>
    ''' 在一个已经拟合/编译好的 S-system 模型之上做虚拟扰动实验
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    Public Function Perturb(args As CommandLine) As Integer
        Dim input As String = optStr(args, "-i", "")

        If String.IsNullOrEmpty(input) OrElse Not File.Exists(input) Then
            Call Console.Error.WriteLine($"ERROR: the input model file '{input}' was not found!")
            Return 1
        End If

        Dim outDir As String = optStr(args, "-o", "")
        Dim model As SSModel

        Try
            model = loadModel(input)
        Catch ex As Exception
            Call Console.Error.WriteLine($"ERROR: unable to load the S-system model '{input}': {ex.Message}")
            Return 1
        End Try

        Dim genes As String() = optStr(args, "/genes", "") _
            .Split(","c) _
            .Select(Function(s) s.Trim) _
            .Where(Function(s) Not String.IsNullOrEmpty(s)) _
            .ToArray

        If genes.Length = 0 Then
            genes = model.Vars _
                .SafeQuery() _
                .Select(Function(v) v.Id) _
                .ToArray
        End If

        Dim designs As PerturbationDesign() = PerturbationDesign.Batch(
            genes:=genes,
            type:=parsePerturbation(optStr(args, "/type", "knockdown")),
            magnitude:=optVal(args, "/magnitude", 0.8R),
            start:=optVal(args, "/start", 0),
            kicks:=optInt(args, "/kicks", 1))

        Dim finalTime As Double = optVal(args, "/time", 0)
        Dim runOptions As New RunOptions With {
            .Input = input,
            .FinalTime = If(finalTime > 0, finalTime, model.FinalTime),
            .UseODEs = True,
            .Precise = optVal(args, "/precise", 0.1R),
            .Steps = optInt(args, "/steps", 0),
            .SolverName = optStr(args, "/solver", ""),
            .NonNegative = True
        }

        Call runOptions.Validate()

        ' 计算与 IO 分离：轨迹文件通过 onSimulated 回调交给 SaveTrajectory 写出
        Dim onSimulated As Action(Of PerturbationDesign, ODEsOut) = Nothing

        If Not String.IsNullOrEmpty(outDir) Then
            onSimulated = Sub(design, output)
                              Call VirtualScreening.SaveTrajectory(
                                  output,
                                  Path.Combine(outDir, VirtualScreening.TrajectoryFileOf(design)))
                          End Sub
        End If

        Dim result As ScreeningResult = VirtualScreening.RunModel(
            model:=model,
            designs:=designs,
            finalTime:=finalTime,
            runOptions:=runOptions,
            onSimulated:=onSimulated)

        Call Console.WriteLine(result.Summary())

        If Not String.IsNullOrEmpty(outDir) Then
            Call VirtualScreening.SaveScreening(result, outDir)
            Call Console.WriteLine($"The trajectory files of the perturbation screening were saved into: {outDir}")
        End If

        Return 0
    End Function

    ''' <summary>
    ''' 查看一个 S-system 模型的摘要信息
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    Public Function Info(args As CommandLine) As Integer
        Dim input As String = optStr(args, "-i", "")

        If String.IsNullOrEmpty(input) OrElse Not File.Exists(input) Then
            Call Console.Error.WriteLine($"ERROR: the input model file '{input}' was not found!")
            Return 1
        End If

        Dim model As SSModel = loadModel(input)

        Call Console.WriteLine($"title     : {model.Title}")
        Call Console.WriteLine($"final time: {model.FinalTime}")
        Call Console.WriteLine($"variables : {model.Vars.SafeQuery().Count()}")
        Call Console.WriteLine($"equations : {model.sEquations.SafeQuery().Count()}")
        Call Console.WriteLine()

        For Each eq As SSEquation In model.sEquations.SafeQuery
            Call Console.WriteLine($"  d({eq.x})/dt = {eq.Expression}")
        Next

        If Not model.Experiments.IsNullOrEmpty Then
            Call Console.WriteLine()
            Call Console.WriteLine("perturbation experiments:")

            For Each exp As SSExperiment In model.Experiments
                Call Console.WriteLine($"  {exp.Id}: {exp.DisturbType.ToString}, start={exp.Start}, interval={exp.Interval}, kicks={exp.Kicks}, value={exp.Value}")
            Next
        End If

        Return 0
    End Function

    Private Function loadModel(path As String) As SSModel
        Dim ext As String = System.IO.Path.GetExtension(path).ToLower

        If ext = ".xml" Then
            Return SSModel.Load(path)
        Else
            Return SSCompiler.Compile(path)
        End If
    End Function

    Private Function parseNormalize(value As String) As SMRUCC.genomics.Analysis.SGrid.GRN.NormalizationMethods
        Select Case value.ToLower
            Case "none"
                Return NormalizationMethods.None
            Case "zscore", "rowzscore"
                Return NormalizationMethods.RowZScore
            Case "quantile"
                Return NormalizationMethods.Quantile
            Case Else
                Return NormalizationMethods.Log2
        End Select
    End Function

    Private Function parsePerturbation(value As String) As SMRUCC.genomics.Analysis.SGrid.Perturbation.PerturbationTypes
        Select Case value.ToLower.Replace("-", "")
            Case "knockout", "ko"
                Return PerturbationTypes.KnockOut
            Case "overexpress", "oe"
                Return PerturbationTypes.OverExpress
            Case "custom"
                Return PerturbationTypes.Custom
            Case Else
                Return PerturbationTypes.KnockDown
        End Select
    End Function

    ''' <summary>
    ''' 读取一个可选的命令行开关值，缺失的时候返回默认值
    ''' </summary>
    Private Function optStr(args As CommandLine, name As String, Optional def As String = "") As String
        Dim value As String = args.GetValue(name, def)

        If value Is Nothing Then
            Return def
        Else
            Return value.Trim
        End If
    End Function

    Private Function optVal(args As CommandLine, name As String, Optional def As Double = 0) As Double
        Dim value As String = optStr(args, name, "")

        If String.IsNullOrEmpty(value) Then
            Return def
        End If

        Dim x As Double

        If Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, x) Then
            Return x
        Else
            Return def
        End If
    End Function

    Private Function optInt(args As CommandLine, name As String, Optional def As Integer = 0) As Integer
        Return CInt(optVal(args, name, def))
    End Function

    Private Function optBool(args As CommandLine, name As String, Optional def As Boolean = False) As Boolean
        Dim value As String = optStr(args, name, If(def, "T", "F"))

        Select Case value.ToLower
            Case "t", "true", "1", "yes", "y"
                Return True
            Case "f", "false", "0", "no", "n", ""
                Return False
            Case Else
                Return def
        End Select
    End Function

    ''' <summary>
    ''' 读取形如 ``0,10`` 的区间参数
    ''' </summary>
    Private Function optRange(args As CommandLine, name As String, def As DoubleRange) As DoubleRange
        Dim value As String = optStr(args, name, "")
        Dim parts As String() = value.Split(","c)

        If parts.Length <> 2 Then
            Return def
        End If

        Dim lo As Double
        Dim hi As Double

        If Double.TryParse(parts(0).Trim, NumberStyles.Float, CultureInfo.InvariantCulture, lo) AndAlso
            Double.TryParse(parts(1).Trim, NumberStyles.Float, CultureInfo.InvariantCulture, hi) Then

            If hi > lo Then
                Return New DoubleRange(lo, hi)
            End If
        End If

        Return def
    End Function

    ''' <summary>
    ''' 读取形如 ``0,0.5,1,1.5`` 的数值列表
    ''' </summary>
    Private Function optList(args As CommandLine, name As String) As Double()
        Dim value As String = optStr(args, name, "")

        If String.IsNullOrEmpty(value) Then
            Return Nothing
        End If

        Dim parts As String() = value.Split(","c)
        Dim out As New System.Collections.Generic.List(Of Double)

        For Each part As String In parts
            Dim x As Double

            If Double.TryParse(part.Trim, NumberStyles.Float, CultureInfo.InvariantCulture, x) Then
                Call out.Add(x)
            Else
                Return Nothing
            End If
        Next

        Return out.ToArray
    End Function
End Module
