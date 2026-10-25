Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports SMRUCC.genomics.Analysis.SGrid.GRN
Imports std = System.Math
Imports EffectorRole = SMRUCC.genomics.Analysis.BNLearn.Effector
Imports PriorEdge = SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge
Imports PriorNet = SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork

Namespace IO

    ''' <summary>
    ''' The persistent storage of the fitted S-system gene regulatory network model.
    ''' (拟合得到的 S-system 调控网络模型的压缩包持久化)
    ''' </summary>
    ''' <remarks>
    ''' 压缩包由若干纯文本条目构成，全部 double 数值均使用 <c>G17</c> 格式以不变文化写出，
    ''' 从而保证 <see cref="Load(Stream)"/> 重新加载之后的数值与保存之前逐位相等。
    ''' 
    ''' 条目清单：
    ''' 1. manifest.txt      版本与规模信息
    ''' 2. parameters.csv    每一个基因的速率常数与拟合误差
    ''' 3. matrix_G.csv      合成项的动力学阶矩阵
    ''' 4. matrix_H.csv      降解项的动力学阶矩阵
    ''' 5. series.csv        规整化之后的表达量矩阵（自然对数矩阵在加载时重新计算）
    ''' 6. timepoints.csv    时间轴
    ''' 7. edges.csv         有向调控边（含先验知识的标注）
    ''' 8. options.txt       拟合配置
    ''' 9. prior.csv         本次拟合所使用的先验知识网络
    ''' 10. warnings.txt     拟合过程的告警信息
    ''' 11. report.txt       可读的拟合报告（仅供人工查看，加载时不使用）
    ''' </remarks>
    Public Module ZipPersists

        ''' <summary>
        ''' 压缩包格式的标识
        ''' </summary>
        Public Const FormatTag As String = "sgrid-network-zip"
        ''' <summary>
        ''' 压缩包格式的版本号
        ''' </summary>
        Public Const FormatVersion As Integer = 1

        ''' <summary>
        ''' persists save the fitted model as zip archive file stream
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="file"></param>
        ''' <remarks>调用方负责关闭 <paramref name="file"/>（本函数不会关闭传入的流）</remarks>
        <Extension>
        Public Sub Save(model As SSystemNetwork, file As Stream)
            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If
            If file Is Nothing Then
                Throw New ArgumentNullException(NameOf(file))
            End If
            If Not file.CanWrite Then
                Throw New ArgumentException("The target stream of the model zip archive should be writable!", NameOf(file))
            End If

            Using archive As New ZipArchive(file, ZipArchiveMode.Create, leaveOpen:=True)
                Call writeEntry(archive, "manifest.txt", manifestOf(model))
                Call writeEntry(archive, "parameters.csv", parametersOf(model.Parameters))
                Call writeEntry(archive, "candidates.txt", candidatesOf(model.Parameters))
                Call writeEntry(archive, "matrix_G.csv", matrixOf(model.Parameters.G, model.Parameters.GeneNames))
                Call writeEntry(archive, "matrix_H.csv", matrixOf(model.Parameters.H, model.Parameters.GeneNames))
                Call writeEntry(archive, "series.csv", matrixOf(model.Series.X, model.Series.GeneNames))
                Call writeEntry(archive, "offsets.csv", offsetsOf(model.Series))
                Call writeEntry(archive, "timepoints.csv", timePointsOf(model.Series.TimePoints))
                Call writeEntry(archive, "edges.csv", edgesOf(model.Edges))
                Call writeEntry(archive, "options.txt", optionsOf(model.Options))
                Call writeEntry(archive, "prior.csv", priorOf(model.Prior))
                Call writeEntry(archive, "warnings.txt", String.Join(vbLf, model.Warnings.SafeQuery()))
                Call writeEntry(archive, "report.txt", model.Report())
            End Using
        End Sub

        ''' <summary>
        ''' persists save the fitted model into a zip archive file
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="zipFile"></param>
        <Extension>
        Public Sub Save(model As SSystemNetwork, zipFile As String)
            If String.IsNullOrEmpty(zipFile) Then
                Throw New ArgumentNullException(NameOf(zipFile))
            End If

            Dim dir As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(zipFile))

            If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
                Call Directory.CreateDirectory(dir)
            End If

            Using stream As Stream = System.IO.File.Open(zipFile, FileMode.Create, FileAccess.Write, FileShare.None)
                Call Save(model, stream)
            End Using
        End Sub

        ''' <summary>
        ''' Load SSystemNetwork model from an opened zip archive file stream
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Function Load(file As Stream) As SSystemNetwork
            If file Is Nothing Then
                Throw New ArgumentNullException(NameOf(file))
            End If

            Dim source As Stream = file

            If Not file.CanSeek Then
                ' zip 的读取模式要求底层流可以被定位，故这里先把不可定位的流缓冲到内存之中
                Dim buffer As New MemoryStream()

                Call file.CopyTo(buffer)
                Call buffer.Flush()
                buffer.Position = 0
                source = buffer
            End If

            Using archive As New ZipArchive(source, ZipArchiveMode.Read, leaveOpen:=True)
                Dim manifest As String = readRequired(archive, "manifest.txt")

                Call checkManifest(manifest)

                Dim parameters As SSystemParameters = parseParameters(readRequired(archive, "parameters.csv"))
                Dim names As String() = parameters.GeneNames
                Dim axis As Double() = parseTimePoints(readRequired(archive, "timepoints.csv"))
                Dim series As New ExpressionTimeSeries With {
                    .GeneNames = names,
                    .OriginalNames = parameters.OriginalNames,
                    .TimePoints = axis,
                    .X = parseMatrix(readRequired(archive, "series.csv"), names, axis.Length)
                }

                parameters.G = parseMatrix(readRequired(archive, "matrix_G.csv"), names, names.Length)
                parameters.H = parseMatrix(readRequired(archive, "matrix_H.csv"), names, names.Length)
                parameters.Candidates = parseCandidates(readEntry(archive, "candidates.txt"), names)

                Call parseOffsets(readEntry(archive, "offsets.csv"), series)
                series.LnX = series _
                    .X _
                    .SafeQuery() _
                    .Select(Function(row) row.Select(Function(x) std.Log(std.Max(x, 1.0E-12))).ToArray()) _
                    .ToArray

                Dim prior As PriorNet = Nothing
                Dim priorText As String = readEntry(archive, "prior.csv")

                If Not String.IsNullOrEmpty(priorText) Then
                    prior = parsePrior(priorText)
                End If

                Dim edges As RegulatoryEdge() = parseEdges(readRequired(archive, "edges.csv"))
                Dim warnings As New List(Of String)
                Dim warningsText As String = readEntry(archive, "warnings.txt")

                If Not String.IsNullOrEmpty(warningsText) Then
                    For Each line As String In warningsText.Split({vbLf, vbCrLf}, StringSplitOptions.None).SafeQuery()
                        If Not String.IsNullOrEmpty(line.Trim()) Then
                            Call warnings.Add(line)
                        End If
                    Next
                End If

                Dim model As New SSystemNetwork With {
                    .Series = series,
                    .Parameters = parameters,
                    .Edges = edges,
                    .Options = parseOptions(readEntry(archive, "options.txt")),
                    .Prior = prior,
                    .PriorStatistics = NetworkTopology.PriorMetrics(edges, prior),
                    .Warnings = warnings
                }

                Return model
            End Using
        End Function

        ''' <summary>
        ''' Load SSystemNetwork model from an zip archive file
        ''' </summary>
        ''' <param name="zipFile"></param>
        ''' <returns></returns>
        Public Function Load(zipFile As String) As SSystemNetwork
            If String.IsNullOrEmpty(zipFile) Then
                Throw New ArgumentNullException(NameOf(zipFile))
            End If
            If Not System.IO.File.Exists(zipFile) Then
                Throw New FileNotFoundException($"The network model archive '{zipFile}' does not exist!", zipFile)
            End If

            Using stream As Stream = System.IO.File.Open(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read)
                Return Load(stream)
            End Using
        End Function

        #Region "write"

        Private Function manifestOf(model As SSystemNetwork) As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine($"format={FormatTag}")
            Call sb.AppendLine($"version={FormatVersion}")
            Call sb.AppendLine($"created={DateTime.Now.ToString("s", CultureInfo.InvariantCulture)}")
            Call sb.AppendLine($"genes={model.Parameters.nGenes}")
            Call sb.AppendLine($"time_points={model.Series.nTimePoints}")
            Call sb.AppendLine($"edges={model.Edges.SafeQuery().Count()}")
            Call sb.AppendLine($"prior_edges={model.Prior.EdgeCount()}")
            Call sb.AppendLine($"trajectory_refined={model.Parameters.TrajectoryRefined}")
            Call sb.Append($"mean_slope_rmse={num(model.Parameters.MeanRMSE)}")

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 写出预处理时的逐基因平移常数与归一化信息（外部 context 数据映射到模型空间时必需）
        ''' </summary>
        ''' <param name="series"></param>
        ''' <returns></returns>
        Private Function offsetsOf(series As ExpressionTimeSeries) As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine($"normalize={series.Normalize.ToString()}")
            Call sb.AppendLine($"pseudo={num(series.PseudoCount)}")
            Call sb.AppendLine("gene,offset")

            For i As Integer = 0 To series.nGenes - 1
                Call sb.Append(series.GeneNames(i))
                Call sb.Append(","c)

                If series.Offsets.IsNullOrEmpty OrElse i >= series.Offsets.Length Then
                    Call sb.AppendLine("0")
                Else
                    Call sb.AppendLine(num(series.Offsets(i)))
                End If
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 解析 offsets.csv，返回每一个基因的平移常数；老版本的存档没有该条目，返回全零
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="series"></param>
        Private Sub parseOffsets(text As String, series As ExpressionTimeSeries)
            Dim offsets As Double() = New Double(series.nGenes - 1) {}

            If String.IsNullOrEmpty(text) Then
                series.Offsets = offsets

                Return
            End If

            For Each line As String In textLines(text)
                Dim fields As String() = PriorKnowledge.SplitCsvLine(line)

                If fields.Length < 2 Then
                    Continue For
                End If

                Dim key As String = fields(0).Trim()

                If key = "normalize" Then
                    Dim value As NormalizationMethods

                    If [Enum].TryParse(fields(1), ignoreCase:=True, result:=value) Then
                        series.Normalize = value
                    End If

                    Continue For
                ElseIf key = "pseudo" Then
                    series.PseudoCount = parseDouble(fields, 1)

                    Continue For
                ElseIf key = "gene" Then
                    Continue For
                End If

                Dim index As Integer = System.Array.IndexOf(series.GeneNames, key)

                If index >= 0 Then
                    offsets(index) = parseDouble(fields, 1)
                End If
            Next

            series.Offsets = offsets
        End Sub

        Private Function parametersOf(parameters As SSystemParameters) As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine("gene,original,alpha,beta,slope_sse,slope_rmse,converged,slope_sst")

            For i As Integer = 0 To parameters.nGenes - 1
                Call sb.Append(parameters.GeneNames(i))
                Call sb.Append(","c)
                Call sb.Append(parameters.OriginalNames(i))
                Call sb.Append(","c)
                Call sb.Append(num(parameters.alpha(i)))
                Call sb.Append(","c)
                Call sb.Append(num(parameters.beta(i)))
                Call sb.Append(","c)
                Call sb.Append(num(parameters.SlopeSSE(i)))
                Call sb.Append(","c)
                Call sb.Append(num(parameters.SlopeRMSE(i)))
                Call sb.Append(","c)
                Call sb.Append(parameters.Converged(i).ToString())
                Call sb.Append(","c)
                Call sb.AppendLine(num(If(parameters.SlopeSST.IsNullOrEmpty, 0, parameters.SlopeSST(i))))
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 每一个基因的候选调控因子清单（矩阵行号），为空的时候不写出该条目
        ''' </summary>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Private Function candidatesOf(parameters As SSystemParameters) As String
            If parameters.Candidates.IsNullOrEmpty Then
                Return ""
            End If

            Dim sb As New StringBuilder()

            Call sb.AppendLine("gene,candidates")

            For i As Integer = 0 To parameters.nGenes - 1
                Dim index As Integer() = parameters.Candidates(i)

                Call sb.Append(parameters.GeneNames(i))
                Call sb.Append(","c)
                Call sb.AppendLine(If(index Is Nothing, "", String.Join(";", index.Select(Function(x) x.ToString()))))
            Next

            Return sb.ToString()
        End Function

        Private Function matrixOf(matrix As Double()(), names As String()) As String
            Dim sb As New StringBuilder()

            Call sb.Append("gene")

            For Each name As String In names.SafeQuery()
                Call sb.Append(","c)
                Call sb.Append(name)
            Next

            Call sb.AppendLine()

            For i As Integer = 0 To matrix.Length - 1
                Call sb.Append(names(i))

                For j As Integer = 0 To matrix(i).Length - 1
                    Call sb.Append(","c)
                    Call sb.Append(num(matrix(i)(j)))
                Next

                Call sb.AppendLine()
            Next

            Return sb.ToString()
        End Function

        Private Function timePointsOf(timePoints As Double()) As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine("index,time")

            For i As Integer = 0 To timePoints.Length - 1
                Call sb.Append(i)
                Call sb.Append(","c)
                Call sb.AppendLine(num(timePoints(i)))
            Next

            Return sb.ToString()
        End Function

        Private Function edgesOf(edges As RegulatoryEdge()) As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine("source,target,weight,g,h,interaction,prior_supported,prior_type,prior_confidence")

            For Each edge As RegulatoryEdge In edges.SafeQuery()
                Call sb.Append(edge.source)
                Call sb.Append(","c)
                Call sb.Append(edge.target)
                Call sb.Append(","c)
                Call sb.Append(num(edge.weight))
                Call sb.Append(","c)
                Call sb.Append(num(edge.g))
                Call sb.Append(","c)
                Call sb.Append(num(edge.h))
                Call sb.Append(","c)
                Call sb.Append(edge.interaction)
                Call sb.Append(","c)
                Call sb.Append(edge.PriorSupported.ToString())
                Call sb.Append(","c)
                Call sb.Append(edge.PriorType)
                Call sb.Append(","c)
                Call sb.AppendLine(num(edge.PriorConfidence))
            Next

            Return sb.ToString()
        End Function

        Private Function priorOf(prior As PriorNet) As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine("tf,target,type,confidence")

            If prior Is Nothing Then
                Return sb.ToString()
            End If

            For Each edge As PriorEdge In prior.Edges.SafeQuery()
                Call sb.Append(edge.TF)
                Call sb.Append(","c)
                Call sb.Append(edge.TargetGene)
                Call sb.Append(","c)
                Call sb.Append(edge.RegulationType.ToString())
                Call sb.Append(","c)
                Call sb.AppendLine(num(edge.Confidence))
            Next

            Return sb.ToString()
        End Function

        Private Function optionsOf(options As FittingOptions) As String
            If options Is Nothing Then
                Return ""
            End If

            Dim sb As New StringBuilder()

            Call sb.AppendLine("alpha_min=" & num(options.AlphaBounds.Min))
            Call sb.AppendLine("alpha_max=" & num(options.AlphaBounds.Max))
            Call sb.AppendLine("beta_min=" & num(options.BetaBounds.Min))
            Call sb.AppendLine("beta_max=" & num(options.BetaBounds.Max))
            Call sb.AppendLine("g_min=" & num(options.GBounds.Min))
            Call sb.AppendLine("g_max=" & num(options.GBounds.Max))
            Call sb.AppendLine("h_min=" & num(options.HBounds.Min))
            Call sb.AppendLine("h_max=" & num(options.HBounds.Max))
            Call sb.AppendLine("sparsity=" & num(options.Sparsity))
            Call sb.AppendLine("max_edges_per_gene=" & options.MaxEdgesPerGene)
            Call sb.AppendLine("l1_penalty=" & num(options.L1Penalty))
            Call sb.AppendLine("stability_penalty=" & num(options.StabilityPenalty))
            Call sb.AppendLine("keep_self_loop=" & options.KeepSelfLoop)
            Call sb.AppendLine("max_regulators=" & options.MaxRegulators)
            Call sb.AppendLine("normalize=" & options.Normalize.ToString())
            Call sb.AppendLine("pseudo_count=" & num(options.PseudoCount))
            Call sb.AppendLine("denoise=" & options.Denoise)
            Call sb.AppendLine("de_population=" & options.DEPopulationSize)
            Call sb.AppendLine("de_iterations=" & options.DEMaxIterations)
            Call sb.AppendLine("de_f=" & num(options.DEF))
            Call sb.AppendLine("de_cr=" & num(options.DECR))
            Call sb.AppendLine("de_threshold=" & num(options.DEThreshold))
            Call sb.AppendLine("lbfgs_iterations=" & options.LBFGSIterations)
            Call sb.AppendLine("refine_trajectory=" & options.RefineTrajectory)
            Call sb.AppendLine("refine_max_iterations=" & options.RefineMaxIterations)
            Call sb.AppendLine("seed=" & options.Seed)
            Call sb.AppendLine("parallel=" & options.Parallel)
            Call sb.AppendLine("verbose=" & options.Verbose)
            Call sb.AppendLine("final_time=" & num(options.FinalTime))
            Call sb.AppendLine("precise=" & num(options.Precise))
            Call sb.AppendLine("solver=" & options.Solver.ToString())
            Call sb.AppendLine("steps=" & options.Steps)
            Call sb.AppendLine("use_prior=" & options.UsePrior)
            Call sb.AppendLine("prior_penalty=" & num(options.PriorPenalty))
            Call sb.AppendLine("prior_sign_penalty=" & num(options.PriorSignPenalty))
            Call sb.AppendLine("use_prior_sign=" & options.UsePriorSign)
            Call sb.AppendLine("prior_rank_boost=" & num(options.PriorRankBoost))
            Call sb.Append("prior_candidate_restriction=" & options.PriorCandidateRestriction)

            Return sb.ToString()
        End Function

        Private Sub writeEntry(archive As ZipArchive, name As String, text As String)
            Dim entry As ZipArchiveEntry = archive.CreateEntry(name, CompressionLevel.Optimal)

            Using stream As Stream = entry.Open()
                Using writer As New StreamWriter(stream, New UTF8Encoding(False))
                    Call writer.Write(If(text, ""))
                End Using
            End Using
        End Sub

        #End Region

        #Region "read"

        Private Function readEntry(archive As ZipArchive, name As String) As String
            Dim entry As ZipArchiveEntry = archive.GetEntry(name)

            If entry Is Nothing Then
                Return Nothing
            End If

            Using stream As Stream = entry.Open()
                Using reader As New StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks:=True)
                    Return reader.ReadToEnd()
                End Using
            End Using
        End Function

        Private Function readRequired(archive As ZipArchive, name As String) As String
            Dim text As String = readEntry(archive, name)

            If text Is Nothing Then
                Throw New InvalidDataException($"The network model archive is incomplete: the entry '{name}' is missing!")
            End If

            Return text
        End Function

        Private Sub checkManifest(manifest As String)
            Dim values As Dictionary(Of String, String) = parseKeyValues(manifest)
            Dim format As String = Nothing
            Dim version As String = Nothing

            If Not values.TryGetValue("format", format) Then
                Throw New InvalidDataException("The network model archive does not contain a valid manifest: the 'format' field is missing!")
            End If
            If Not String.Equals(format, FormatTag, StringComparison.OrdinalIgnoreCase) Then
                Throw New InvalidDataException($"Unsupported network model archive format '{format}', expected '{FormatTag}'!")
            End If

            values.TryGetValue("version", version)

            Dim ver As Integer = 0

            If Not Integer.TryParse(version, ver) Then
                Throw New InvalidDataException($"The version number '{version}' of the network model archive is invalid!")
            End If
            If ver > FormatVersion Then
                Throw New InvalidDataException($"The version {ver} of the network model archive is newer than the supported version {FormatVersion}!")
            End If
        End Sub

        Private Function parseParameters(text As String) As SSystemParameters
            Dim lines As String() = textLines(text)
            Dim genes As New List(Of String)
            Dim originals As New List(Of String)
            Dim alpha As New List(Of Double)
            Dim beta As New List(Of Double)
            Dim sse As New List(Of Double)
            Dim rmse As New List(Of Double)
            Dim converged As New List(Of Boolean)
            Dim sst As New List(Of Double)
            Dim header As Boolean = True

            For Each line As String In lines
                If String.IsNullOrWhiteSpace(line) Then
                    Continue For
                End If
                If header Then
                    header = False
                    Continue For
                End If

                Dim fields As String() = PriorKnowledge.SplitCsvLine(line)

                Call genes.Add(fields(0))
                Call originals.Add(If(fields.Length > 1, fields(1), fields(0)))
                Call alpha.Add(parseDouble(fields, 2))
                Call beta.Add(parseDouble(fields, 3))
                Call sse.Add(parseDouble(fields, 4))
                Call rmse.Add(parseDouble(fields, 5))
                Call converged.Add(parseBoolean(fields, 6))
                Call sst.Add(parseDouble(fields, 7))
            Next

            If genes.Count = 0 Then
                Throw New InvalidDataException("The parameters entry of the network model archive is empty!")
            End If

            Return New SSystemParameters With {
                .GeneNames = genes.ToArray,
                .OriginalNames = originals.ToArray,
                .alpha = alpha.ToArray,
                .beta = beta.ToArray,
                .SlopeSSE = sse.ToArray,
                .SlopeRMSE = rmse.ToArray,
                .SlopeSST = sst.ToArray,
                .Converged = converged.ToArray
            }
        End Function

        ''' <summary>
        ''' 解析候选调控因子条目；条目缺失的时候返回空值（表示全部基因都是候选）
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="names"></param>
        ''' <returns></returns>
        Private Function parseCandidates(text As String, names As String()) As Integer()()
            If String.IsNullOrEmpty(text) Then
                Return Nothing
            End If

            Dim result As Integer()() = New Integer(names.Length - 1)() {}
            Dim header As Boolean = True
            Dim row As Integer = 0

            For Each line As String In textLines(text)
                If String.IsNullOrWhiteSpace(line) Then
                    Continue For
                End If
                If header Then
                    header = False
                    Continue For
                End If
                If row >= result.Length Then
                    Exit For
                End If

                Dim fields As String() = PriorKnowledge.SplitCsvLine(line)

                If fields.Length > 1 AndAlso Not String.IsNullOrWhiteSpace(fields(1)) Then
                    Dim index As New List(Of Integer)

                    For Each token As String In fields(1).Split(";"c)
                        Dim value As Integer

                        If Integer.TryParse(token.Trim(), value) AndAlso value >= 0 AndAlso value < names.Length Then
                            Call index.Add(value)
                        End If
                    Next

                    result(row) = index.ToArray
                End If

                row += 1
            Next

            Return If(row = 0, Nothing, result)
        End Function

        ''' <summary>
        ''' 解析矩阵条目：首行为表头，首列为行标签
        ''' </summary>
        ''' <param name="text">条目的文本内容</param>
        ''' <param name="names">行标签（基因编号）</param>
        ''' <param name="columns">每一行的数据列数</param>
        ''' <returns></returns>
        Private Function parseMatrix(text As String, names As String(), columns As Integer) As Double()()
            Dim lines As String() = textLines(text)
            Dim rows As New List(Of Double())
            Dim header As Boolean = True

            For Each line As String In lines
                If String.IsNullOrWhiteSpace(line) Then
                    Continue For
                End If
                If header Then
                    header = False
                    Continue For
                End If

                Dim fields As String() = PriorKnowledge.SplitCsvLine(line)
                Dim values As Double() = New Double(columns - 1) {}

                For i As Integer = 0 To columns - 1
                    values(i) = parseDouble(fields, i + 1)
                Next

                Call rows.Add(values)
            Next

            If rows.Count <> names.Length Then
                Throw New InvalidDataException($"The matrix entry of the network model archive is inconsistent: {rows.Count} row(s) for {names.Length} gene(s)!")
            End If

            Return rows.ToArray()
        End Function

        Private Function parseTimePoints(text As String) As Double()
            Dim lines As String() = textLines(text)
            Dim values As New List(Of Double)
            Dim header As Boolean = True

            For Each line As String In lines
                If String.IsNullOrWhiteSpace(line) Then
                    Continue For
                End If
                If header Then
                    header = False
                    Continue For
                End If

                Dim fields As String() = PriorKnowledge.SplitCsvLine(line)

                Call values.Add(parseDouble(fields, 1))
            Next

            Return values.ToArray()
        End Function

        Private Function parseEdges(text As String) As RegulatoryEdge()
            Dim lines As String() = textLines(text)
            Dim edges As New List(Of RegulatoryEdge)
            Dim header As Boolean = True

            For Each line As String In lines
                If String.IsNullOrWhiteSpace(line) Then
                    Continue For
                End If
                If header Then
                    header = False
                    Continue For
                End If

                Dim fields As String() = PriorKnowledge.SplitCsvLine(line)

                Call edges.Add(New RegulatoryEdge With {
                    .source = fields(0),
                    .target = fields(1),
                    .weight = parseDouble(fields, 2),
                    .g = parseDouble(fields, 3),
                    .h = parseDouble(fields, 4),
                    .PriorSupported = parseBoolean(fields, 6),
                    .PriorType = parseText(fields, 7, "Unknown"),
                    .PriorConfidence = parseDouble(fields, 8)
                })
            Next

            Return edges.ToArray
        End Function

        Private Function parsePrior(text As String) As PriorNet
            Dim lines As String() = textLines(text)
            Dim edges As New List(Of PriorEdge)
            Dim header As Boolean = True

            For Each line As String In lines
                If String.IsNullOrWhiteSpace(line) Then
                    Continue For
                End If
                If header Then
                    header = False
                    Continue For
                End If

                Dim fields As String() = PriorKnowledge.SplitCsvLine(line)

                If fields.Length < 2 Then
                    Continue For
                End If

                Call edges.Add(New PriorEdge With {
                    .TF = fields(0),
                    .TargetGene = fields(1),
                    .RegulationType = parseEffector(parseText(fields, 2, "Unknown")),
                    .Confidence = parseConfidence(fields, 3)
                })
            Next

            If edges.Count = 0 Then
                Return Nothing
            End If

            Return New PriorNet(edges)
        End Function

        ''' <summary>
        ''' 解析调控方向字段，无法识别的时候回落到 <see cref="EffectorRole.Unknown"/>
        ''' </summary>
        ''' <param name="text"></param>
        ''' <returns></returns>
        Private Function parseEffector(text As String) As EffectorRole
            Return PriorEdge.ParseEffector(text)
        End Function

        ''' <summary>
        ''' 解析置信度字段：既接受数值也接受 High/Medium/Low 这样的标签
        ''' </summary>
        ''' <param name="fields"></param>
        ''' <param name="index"></param>
        ''' <returns></returns>
        Private Function parseConfidence(fields As String(), index As Integer) As Double
            Dim text As String = parseText(fields, index, "")
            Dim value As Double

            If String.IsNullOrEmpty(text) Then
                Return 1.0R
            End If
            If Double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
                Return value
            End If

            Dim mapped As Double = PriorEdge.ParseConfidenceEnum(text)

            If mapped <= 0 Then
                Return 0.5R
            Else
                Return mapped
            End If
        End Function

        Private Function parseOptions(text As String) As FittingOptions
            Dim result As New FittingOptions()

            If String.IsNullOrEmpty(text) Then
                Return result
            End If

            Dim values As Dictionary(Of String, String) = parseKeyValues(text)

            result.AlphaBounds = New Microsoft.VisualBasic.ComponentModel.Ranges.Model.DoubleRange(
                valueOf(values, "alpha_min", result.AlphaBounds.Min),
                valueOf(values, "alpha_max", result.AlphaBounds.Max))
            result.BetaBounds = New Microsoft.VisualBasic.ComponentModel.Ranges.Model.DoubleRange(
                valueOf(values, "beta_min", result.BetaBounds.Min),
                valueOf(values, "beta_max", result.BetaBounds.Max))
            result.GBounds = New Microsoft.VisualBasic.ComponentModel.Ranges.Model.DoubleRange(
                valueOf(values, "g_min", result.GBounds.Min),
                valueOf(values, "g_max", result.GBounds.Max))
            result.HBounds = New Microsoft.VisualBasic.ComponentModel.Ranges.Model.DoubleRange(
                valueOf(values, "h_min", result.HBounds.Min),
                valueOf(values, "h_max", result.HBounds.Max))

            result.Sparsity = valueOf(values, "sparsity", result.Sparsity)
            result.MaxEdgesPerGene = CInt(valueOf(values, "max_edges_per_gene", result.MaxEdgesPerGene))
            result.L1Penalty = valueOf(values, "l1_penalty", result.L1Penalty)
            result.StabilityPenalty = valueOf(values, "stability_penalty", result.StabilityPenalty)
            result.KeepSelfLoop = valueOf(values, "keep_self_loop", result.KeepSelfLoop)
            result.MaxRegulators = CInt(valueOf(values, "max_regulators", result.MaxRegulators))
            result.Normalize = parseNormalize(parseText(values, "normalize", result.Normalize.ToString()))
            result.PseudoCount = valueOf(values, "pseudo_count", result.PseudoCount)
            result.Denoise = valueOf(values, "denoise", result.Denoise)
            result.DEPopulationSize = CInt(valueOf(values, "de_population", result.DEPopulationSize))
            result.DEMaxIterations = CInt(valueOf(values, "de_iterations", result.DEMaxIterations))
            result.DEF = valueOf(values, "de_f", result.DEF)
            result.DECR = valueOf(values, "de_cr", result.DECR)
            result.DEThreshold = valueOf(values, "de_threshold", result.DEThreshold)
            result.LBFGSIterations = CInt(valueOf(values, "lbfgs_iterations", result.LBFGSIterations))
            result.RefineTrajectory = valueOf(values, "refine_trajectory", result.RefineTrajectory)
            result.RefineMaxIterations = CInt(valueOf(values, "refine_max_iterations", result.RefineMaxIterations))
            result.Seed = CInt(valueOf(values, "seed", result.Seed))
            result.Parallel = valueOf(values, "parallel", result.Parallel)
            result.Verbose = valueOf(values, "verbose", result.Verbose)
            result.FinalTime = valueOf(values, "final_time", result.FinalTime)
            result.Precise = valueOf(values, "precise", result.Precise)
            result.Solver = parseSolver(parseText(values, "solver", result.Solver.ToString()), result.Solver)
            result.Steps = CInt(valueOf(values, "steps", result.Steps))
            result.UsePrior = valueOf(values, "use_prior", result.UsePrior)
            result.PriorPenalty = valueOf(values, "prior_penalty", result.PriorPenalty)
            result.PriorSignPenalty = valueOf(values, "prior_sign_penalty", result.PriorSignPenalty)
            result.UsePriorSign = valueOf(values, "use_prior_sign", result.UsePriorSign)
            result.PriorRankBoost = valueOf(values, "prior_rank_boost", result.PriorRankBoost)
            result.PriorCandidateRestriction = valueOf(values, "prior_candidate_restriction", result.PriorCandidateRestriction)

            Return result
        End Function

        Private Function parseKeyValues(text As String) As Dictionary(Of String, String)
            Dim values As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

            For Each line As String In textLines(text)
                If String.IsNullOrWhiteSpace(line) Then
                    Continue For
                End If

                Dim index As Integer = line.IndexOf("="c)

                If index <= 0 Then
                    Continue For
                End If

                Dim key As String = line.Substring(0, index).Trim()
                Dim value As String = line.Substring(index + 1).Trim()

                values(key) = value
            Next

            Return values
        End Function

        Private Function textLines(text As String) As String()
            If String.IsNullOrEmpty(text) Then
                Return New String() {}
            Else
                Return text.Split({vbLf, vbCrLf}, StringSplitOptions.None)
            End If
        End Function

        Private Function parseText(values As Dictionary(Of String, String), key As String, fallback As String) As String
            Dim value As String = Nothing

            If values.TryGetValue(key, value) Then
                Return value
            Else
                Return fallback
            End If
        End Function

        Private Function valueOf(values As Dictionary(Of String, String), key As String, fallback As Double) As Double
            Dim text As String = Nothing
            Dim value As Double

            If values.TryGetValue(key, text) AndAlso Double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
                Return value
            Else
                Return fallback
            End If
        End Function

        Private Function valueOf(values As Dictionary(Of String, String), key As String, fallback As Integer) As Integer
            Dim text As String = Nothing
            Dim value As Integer

            If values.TryGetValue(key, text) AndAlso Integer.TryParse(text, value) Then
                Return value
            Else
                Return fallback
            End If
        End Function

        Private Function valueOf(values As Dictionary(Of String, String), key As String, fallback As Boolean) As Boolean
            Dim text As String = Nothing
            Dim value As Boolean

            If Not values.TryGetValue(key, text) Then
                Return fallback
            End If
            If Boolean.TryParse(text, value) Then
                Return value
            End If

            Return String.Equals(text, "1", StringComparison.Ordinal) OrElse String.Equals(text, "yes", StringComparison.OrdinalIgnoreCase)
        End Function

        Private Function parseDouble(fields As String(), index As Integer) As Double
            If fields Is Nothing OrElse index < 0 OrElse index >= fields.Length Then
                Return 0
            End If

            Dim value As Double

            If Double.TryParse(fields(index), NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
                Return value
            Else
                Return 0
            End If
        End Function

        Private Function parseBoolean(fields As String(), index As Integer) As Boolean
            If fields Is Nothing OrElse index < 0 OrElse index >= fields.Length Then
                Return False
            End If

            Dim value As Boolean

            If Boolean.TryParse(fields(index), value) Then
                Return value
            End If

            Return String.Equals(fields(index), "1", StringComparison.Ordinal)
        End Function

        Private Function parseText(fields As String(), index As Integer, fallback As String) As String
            If fields Is Nothing OrElse index < 0 OrElse index >= fields.Length OrElse String.IsNullOrEmpty(fields(index)) Then
                Return fallback
            Else
                Return fields(index)
            End If
        End Function

        Private Function parseNormalize(text As String) As NormalizationMethods
            Dim value As NormalizationMethods

            If [Enum].TryParse(text, ignoreCase:=True, result:=value) Then
                Return value
            Else
                Return NormalizationMethods.Log2
            End If
        End Function

        Private Function parseSolver(text As String, fallback As ODESolverMethod) As ODESolverMethod
            Dim value As ODESolverMethod

            If [Enum].TryParse(text, ignoreCase:=True, result:=value) Then
                Return value
            Else
                Return fallback
            End If
        End Function

        #End Region

        ''' <summary>
        ''' 以 <c>G17</c> 格式输出一个双精度值（往返无损）
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Private Function num(x As Double) As String
            If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then
                Return "0"
            Else
                Return x.ToString("G17", CultureInfo.InvariantCulture)
            End If
        End Function
    End Module
End Namespace
