Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.FuzzyGene.Data
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Namespace IO

    ''' <summary>
    ''' 一条轨迹在导出时的角色与样本元数据约定。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 同一次虚拟扰动会导出两条轨迹（突变型与野生型），它们共用同一套
    ''' 基因行顺序，只有样本前缀、分组名与扰动信息不同，
    ''' 因此这些差异统一由本选项对象描述。
    ''' </para>
    ''' <para>
    ''' <see cref="Dt"/> 把「迭代帧号」换算成 <c>metadata("time")</c> 的数值：
    ''' 稳态迭代的帧号本身不是时间，默认 <c>1.0</c> 表示「一帧算一个单位」。
    ''' 只有滞后（时序）路线且阻尼系数为 1 时，<see cref="Dt"/> 才应该
    ''' 填成训练数据的采样间隔。
    ''' </para>
    ''' </remarks>
    Public Class TraceExportOptions

        ''' <summary>轨迹角色：<c>mutant</c> 或 <c>wildtype</c>。</summary>
        Public Property Role As String = "mutant"

        ''' <summary>样本 ID 前缀，列名形如 <c>{prefix}000</c>。</summary>
        Public Property SamplePrefix As String = "mut_t"

        ''' <summary>样本分组名（<see cref="SampleGroup.sample_info"/>）。</summary>
        Public Property GroupName As String = "mutant"

        ''' <summary>被扰动的靶基因；野生型轨迹为 <c>Nothing</c>。</summary>
        Public Property TargetGene As String

        ''' <summary>干预模式名（<c>Knockout</c> / <c>Knockdown</c> / <c>Overexpression</c> / <c>Custom</c>）。</summary>
        Public Property Mode As String

        ''' <summary>施加的钳制值（原始表达量）；野生型轨迹为 <c>NaN</c>。</summary>
        Public Property ClampValue As Double = Double.NaN

        ''' <summary>一帧对应的时间单位数（仅用于 <c>metadata("time")</c>）。</summary>
        Public Property Dt As Double = 1.0

        ''' <summary>绘图颜色。</summary>
        Public Property Color As String = "#D62728"

        ''' <summary>图例形状。</summary>
        Public Property Shape As String = "21"

        ''' <summary>
        ''' 输出的矩阵文件名；<c>Nothing</c> 时按角色生成 <c>trace.{Role}.csv</c>。
        ''' </summary>
        Public Property FileName As String

        ''' <summary>实际使用的矩阵文件名。</summary>
        Friend ReadOnly Property ResolvedFileName As String
            Get
                Return If(FileName.StringEmpty(True), $"trace.{Role}.csv", FileName)
            End Get
        End Property

        ''' <summary>与矩阵同名的样本元数据文件名。</summary>
        Friend ReadOnly Property SampleInfoFileName As String
            Get
                Return System.IO.Path.GetFileNameWithoutExtension(ResolvedFileName) & ".sampleinfo.json"
            End Get
        End Property

    End Class

    ''' <summary>
    ''' 稳态迭代轨迹 → 表达矩阵（CSV） + 样本元数据（<c>SampleInfo</c> sidecar）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 矩阵的列是**迭代帧**而不是生物学重复，所以 sidecar 里的每个 <see cref="SampleInfo"/>
    ''' 描述的是「一帧」；<c>metadata("time")</c> 按 <see cref="TraceExportOptions.Dt"/>
    ''' 由帧号换算，<c>metadata("iteration")</c> 保留原始的帧号。
    ''' </para>
    ''' <para>
    ''' 落盘格式与 <c>vCella</c> 的矩阵导出保持一致：矩阵走
    ''' <see cref="Document.SaveMatrix"/>（UTF-8 无 BOM），sidecar 先尝试真实
    ''' <see cref="SampleInfo"/> 序列化，再用 <see cref="System.Text.Json"/> 校验
    ''' （数组、数量、ID 非空、<c>time</c> 可解析），不通过时回退到字段镜像，
    ''' 保证下游拿到的 schema 稳定。
    ''' </para>
    ''' </remarks>
    Public Module TraceExporter

        ''' <summary>扰动摘要清单的文件名。</summary>
        Public Const ManifestFileName As String = "trace.perturbation.json"

        ''' <summary>
        ''' 把一条轨迹导出到目录：矩阵 CSV + 同名 <c>*.sampleinfo.json</c>。
        ''' </summary>
        ''' <param name="trace">轨迹</param>
        ''' <param name="dir">输出目录（不存在时自动创建）</param>
        ''' <param name="options">角色与样本元数据约定</param>
        ''' <returns>实际写出的文件路径（矩阵在前，元数据在后）</returns>
        Public Function Export(trace As SteadyStateTrace,
                               dir As String,
                               options As TraceExportOptions) As String()

            If trace Is Nothing Then
                Throw New ArgumentNullException(NameOf(trace))
            End If
            If String.IsNullOrWhiteSpace(dir) Then
                Throw New ArgumentException("the output directory is empty", NameOf(dir))
            End If
            If options Is Nothing Then
                Throw New ArgumentNullException(NameOf(options))
            End If

            Call CheckShape(trace)

            Call dir.MakeDir

            Dim matrixFile As String = System.IO.Path.Combine(dir, options.ResolvedFileName)
            Dim infoFile As String = System.IO.Path.Combine(dir, options.SampleInfoFileName)

            Call trace.ToMatrix(options.SamplePrefix, MatrixTag(trace, options)).SaveMatrix(matrixFile)
            Call WriteSidecar(trace, options, infoFile)

            Return New String() {matrixFile, infoFile}
        End Function

        ''' <summary>
        ''' 写出扰动摘要清单（一次扰动一条记录，汇总两条轨迹）。
        ''' </summary>
        ''' <param name="dir">输出目录</param>
        ''' <param name="options">突变型的导出选项（靶基因、模式、钳制值从这里取）</param>
        ''' <param name="mutant">突变型轨迹</param>
        ''' <param name="wildtype">野生型（对照）轨迹；可为 <c>Nothing</c></param>
        ''' <param name="files">已经写出的文件路径</param>
        ''' <returns>清单文件路径</returns>
        Public Function WriteManifest(dir As String,
                                      options As TraceExportOptions,
                                      mutant As SteadyStateTrace,
                                      wildtype As SteadyStateTrace,
                                      files As String()) As String

            Call dir.MakeDir

            Dim manifest As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase) From {
                {"format", "fuzzygene steady-state trace"},
                {"created_at", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                {"target_gene", If(options.TargetGene, "")},
                {"perturbation_mode", If(options.Mode, "")},
                {"clamp_value", Num(options.ClampValue)},
                {"dt", Num(options.Dt)},
                {"n_genes", If(mutant Is Nothing, 0, mutant.NGene)},
                {"mutant_iterations", If(mutant Is Nothing, 0, mutant.Iterations)},
                {"mutant_converged", mutant IsNot Nothing AndAlso mutant.Converged},
                {"mutant_exit_reason", If(mutant Is Nothing, "", mutant.ExitReason.ToString())},
                {"mutant_free_residual", If(mutant Is Nothing, Double.NaN, mutant.FreeResidual)},
                {"mutant_clamp_residual", If(mutant Is Nothing, Double.NaN, mutant.ClampResidual)},
                {"mutant_attractor_period", If(mutant Is Nothing, 0, mutant.AttractorPeriod)},
                {"mutant_stabilized_by", If(mutant Is Nothing, "", mutant.StabilizedBy)},
                {"mutant_max_deltas", If(mutant Is Nothing, New Double() {}, mutant.MaxDeltas.ToArray())},
                {"wildtype_iterations", If(wildtype Is Nothing, 0, wildtype.Iterations)},
                {"wildtype_converged", wildtype IsNot Nothing AndAlso wildtype.Converged},
                {"wildtype_exit_reason", If(wildtype Is Nothing, "", wildtype.ExitReason.ToString())},
                {"wildtype_free_residual", If(wildtype Is Nothing, Double.NaN, wildtype.FreeResidual)},
                {"wildtype_stabilized_by", If(wildtype Is Nothing, "", wildtype.StabilizedBy)},
                {"wildtype_max_deltas", If(wildtype Is Nothing, New Double() {}, wildtype.MaxDeltas.ToArray())},
                {"omega", If(mutant Is Nothing, Double.NaN, mutant.Relaxation)},
                {"tolerance", If(mutant Is Nothing, Double.NaN, mutant.Tolerance)},
                {"lagged", mutant IsNot Nothing AndAlso mutant.Lagged},
                {"note", "frame index is a solver iteration, not biological time; read exit_reason / stabilized_by before using the end state"},
                {"files", If(files, New String() {})}
            }

            Dim path As String = System.IO.Path.Combine(dir, ManifestFileName)

            Call File.WriteAllText(path, manifest.GetJson(indent:=True), Utf8NoBom)

            Return path
        End Function

        ''' <summary>
        ''' 按轨迹构建样本元数据：一帧一个 <see cref="SampleInfo"/>，顺序与矩阵列严格一致。
        ''' </summary>
        ''' <param name="trace">轨迹</param>
        ''' <param name="options">角色与元数据约定</param>
        ''' <returns>与矩阵列一一对应的样本元数据</returns>
        Public Function BuildSampleInfo(trace As SteadyStateTrace,
                                        options As TraceExportOptions) As SampleInfo()

            Dim items(trace.NFrames - 1) As SampleInfo

            For frame As Integer = 0 To trace.NFrames - 1
                Dim id As String = options.SamplePrefix & frame.ToString("000")
                Dim delta As Double = trace.DeltaAt(frame)

                Dim metadata As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
                    {"time", Num(frame * options.Dt)},
                    {"iteration", frame.ToString(Globalization.CultureInfo.InvariantCulture)},
                    {"role", If(options.Role, "")},
                    {"group", If(options.GroupName, "")},
                    {"omega", Num(trace.Relaxation)},
                    {"tolerance", Num(trace.Tolerance)},
                    {"converged", If(trace.Converged, "1", "0")},
                    {"lagged", If(trace.Lagged, "1", "0")},
                    {"n_genes", trace.NGene.ToString(Globalization.CultureInfo.InvariantCulture)},
                    {"target_gene", If(options.TargetGene, "")},
                    {"perturbation_mode", If(options.Mode, "")},
                    {"clamped", If(String.IsNullOrEmpty(options.TargetGene), "0", "1")}
                }

                ' 第 0 帧是起点，没有「产生它的那一步」；用空串而不是 0 表示「无值」
                metadata("max_delta") = If(frame = 0 OrElse Double.IsNaN(delta), "", Num(delta))

                ' 与 GEARS 的 Perturb-seq 约定对齐：这两组键名是它的读取入口
                ' （SMRUCC.genomics.Analysis.GEARS.GEARS.metadata_perturbed_genes 等），
                ' 值分别是 JSON 字符串数组与 InterventionMode 枚举名。
                '
                ' 注意落盘后值内部的引号会显示为 \u0022（["\u0022TPPP3\u0022"]）：
                ' 那是 GetJson 外层的 HTML-safe 编码器对字符串内容做的转义，JSON 语义不变，
                ' 下游 JsonSerializer 反序列化后仍然得到 ["TPPP3"]。
                If Not String.IsNullOrEmpty(options.TargetGene) Then
                    metadata("perturbed_genes") =
                        "[" & String.Join(",", options.TargetGene.Split("+"c).Select(Function(g) $"""{g}""")) & "]"
                    metadata("intervention_mode") = If(options.Mode, "")
                End If

                items(frame) = New SampleInfo With {
                    .ID = id,
                    .sample_name = id,
                    .sample_info = If(options.GroupName, ""),
                    .batch = 1,
                    .injectionOrder = frame + 1,
                    .color = If(options.Color, "#D62728"),
                    .shape = If(options.Shape, "21"),
                    .metadata = metadata
                }
            Next

            Return items
        End Function

        ''' <summary>
        ''' 写出 sidecar：先试真实 <see cref="SampleInfo"/> 序列化并校验，
        ''' 不通过则回退到字段完全一致的镜像 DTO。
        ''' </summary>
        Private Sub WriteSidecar(trace As SteadyStateTrace, options As TraceExportOptions, path As String)
            Dim items As SampleInfo() = BuildSampleInfo(trace, options)
            Dim json As String = Nothing

            Try
                json = items.GetJson(indent:=True)
            Catch ex As Exception
                ' SampleInfo 带一个带参的默认属性，不同序列化器对它的处理并不一致；
                ' 这里失败不算致命，交给下面的校验 + 回退分支
                json = Nothing
            End Try

            If Not ValidateSidecar(json, items) Then
                json = MirrorOf(items).GetJson(indent:=True)
            End If

            Call File.WriteAllText(path, json, Utf8NoBom)
        End Sub

        ''' <summary><see cref="SampleInfo"/> 的字段镜像（仅在真实类型序列化不可用时使用）。</summary>
        Private Class SampleInfoDto

            Public Property ID As String
            Public Property sample_name As String
            Public Property sample_info As String
            Public Property batch As Integer
            Public Property injectionOrder As Integer
            Public Property metadata As Dictionary(Of String, String)
            Public Property color As String
            Public Property shape As String

        End Class

        Private Function MirrorOf(items As SampleInfo()) As SampleInfoDto()
            Dim result(items.Length - 1) As SampleInfoDto

            For i As Integer = 0 To items.Length - 1
                result(i) = New SampleInfoDto With {
                    .ID = items(i).ID,
                    .sample_name = items(i).sample_name,
                    .sample_info = items(i).sample_info,
                    .batch = items(i).batch,
                    .injectionOrder = items(i).injectionOrder,
                    .metadata = items(i).metadata,
                    .color = items(i).color,
                    .shape = items(i).shape
                }
            Next

            Return result
        End Function

        ''' <summary>校验 sidecar：必须是数组、数量一致、ID 非空、<c>metadata.time</c> 可解析。</summary>
        Private Function ValidateSidecar(json As String, items As SampleInfo()) As Boolean
            If json.StringEmpty(True) Then
                Return False
            End If

            Try
                Using doc As JsonDocument = JsonDocument.Parse(json)
                    If doc.RootElement.ValueKind <> JsonValueKind.Array Then
                        Return False
                    End If
                    If doc.RootElement.GetArrayLength() <> items.Length Then
                        Return False
                    End If

                    For i As Integer = 0 To items.Length - 1
                        Dim element As JsonElement = doc.RootElement(i)
                        Dim id As JsonElement

                        If Not element.TryGetProperty(NameOf(SampleInfo.ID), id) Then
                            Return False
                        End If
                        If Not String.Equals(id.GetString(), items(i).ID, StringComparison.Ordinal) Then
                            Return False
                        End If

                        Dim meta As JsonElement

                        If Not element.TryGetProperty("metadata", meta) OrElse meta.ValueKind <> JsonValueKind.Object Then
                            Return False
                        End If

                        Dim time As JsonElement

                        If Not meta.TryGetProperty("time", time) Then
                            Return False
                        End If

                        Dim parsed As Double

                        If Not Double.TryParse(time.GetString(), Globalization.NumberStyles.Float,
                                               Globalization.CultureInfo.InvariantCulture, parsed) Then
                            Return False
                        End If
                    Next
                End Using

                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' <summary>矩阵标签：把角色 / 扰动 / 阻尼写进去，方便离线消费方识别。</summary>
        Private Function MatrixTag(trace As SteadyStateTrace, options As TraceExportOptions) As String
            Dim perturb As String = "control"

            If Not String.IsNullOrEmpty(options.TargetGene) Then
                perturb = $"{If(options.Mode, "Perturbation")}:{options.TargetGene}"
            End If

            Return $"fuzzygene steady-state trace ({If(options.Role, "trace")}, {perturb}, " &
                   $"{trace.NGene} genes x {trace.NFrames} frames, omega={trace.Relaxation:G3})"
        End Function

        ''' <summary>导出前的形状检查：宁可失败也不写出半成品。</summary>
        Private Sub CheckShape(trace As SteadyStateTrace)
            If trace.NFrames = 0 Then
                Throw New InvalidOperationException("the trace does not contain any frame")
            End If
            If trace.NGene = 0 Then
                Throw New InvalidOperationException("the trace does not describe any gene")
            End If

            For frame As Integer = 0 To trace.NFrames - 1
                Dim state As Double() = trace.States(frame)

                If state Is Nothing OrElse state.Length <> trace.NGene Then
                    Throw New InvalidDataException(
                        $"frame {frame} has {If(state Is Nothing, 0, state.Length)} value(s) " &
                        $"but the trace describes {trace.NGene} genes")
                End If
            Next
        End Sub

        Private Function Num(value As Double) As String
            If Double.IsNaN(value) Then
                Return "NaN"
            End If
            If Double.IsInfinity(value) Then
                Return If(value > 0, "Infinity", "-Infinity")
            End If

            Return value.ToString("R", Globalization.CultureInfo.InvariantCulture)
        End Function

        Private ReadOnly Property Utf8NoBom As UTF8Encoding
            Get
                Return New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False)
            End Get
        End Property

    End Module

End Namespace
