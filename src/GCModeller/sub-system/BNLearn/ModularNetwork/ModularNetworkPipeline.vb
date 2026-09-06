' ============================================================
' WGCNASubnetworkPipeline.vb
' ------------------------------------------------------------
' 依据 DBNBlocks.md 文档思路："分而治之训练、合而为一扰动"。
'
' 流程：
'   1) 基于 WGCNA 模块划分（GeneModuleColor[]）把基因切分为若干模块；
'   2) 对每个模块子集独立训练静态高斯贝叶斯子网络（结构学习 + 参数学习）；
'   3) 把各子网的回归系数拼成块对角全局系数矩阵 A，并（关键）补全
'      模块间边（用模块 eigengene 相关 + hub 基因间相关），得到整合的
'      全局网络（含模块内 + 模块间边），并统一学习全局 CPD；
'   4) 在整合后的全局网络上做虚拟扰动传播，支持两种方法：
'        - Jacobian（默认）：沿 A^k 多步线性传播至收敛；
'        - CascadeSampling：在全局网络上做多步 do-演算（DynamicIntervention）。
'   5) 导出全局扰动响应矩阵（gene × perturbation）TSV + 控制台摘要。
' ============================================================

Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.BNLearn.ModularNetwork.WGCNA
Imports SMRUCC.genomics.Analysis.BNLearn.StructureLearning

Namespace ModularNetwork

    ''' <summary>
    ''' 基于 WGCNA 模块划分的贝叶斯子网络训练 + 全局虚拟扰动流水线
    ''' </summary>
    Public Class ModularNetworkPipeline

        ' ---- 全局扰动参数 ----
        ''' <summary>传播方法，默认 Jacobian（线性化雅可比多步传播）</summary>
        Public Property Propagation As PropagationMethod = PropagationMethod.Jacobian

        ''' <summary>最大传播步数（雅可比收敛上限 / 级联采样时间步数）</summary>
        Public Property MaxSteps As Integer = 50

        ''' <summary>雅可比收敛阈值：||e_{t+1}|| / ||e_t|| 小于该值即停止</summary>
        Public Property Tolerance As Double = 0.000001
        ''' <summary>参数学习与采样所用样本数</summary>
        Public Property NSamples As Integer = 10000

        ''' <summary>随机种子</summary>
        Public Property RandomSeed As Integer = 42

        ' ---- 训练参数（与 BNLearnWorkflow 风格一致） ----

        ''' <summary>
        ''' 是否对表达数据做标准化（z-score），默认 True
        ''' </summary>
        ''' <returns></returns>
        Public Property NormalizeData As Boolean = True

        ''' <summary>结构学习参数（算法/显著性阈值/最大父节点数/随机种子）</summary>
        Public Property StructureParams As New StructureLearningParams()

        ''' <summary>每个模块取 kME 最高的前 N 个基因作为模块接口（hub）</summary>
        Public Property HubTopN As Integer = 20

        ''' <summary>模块 eigengene 相关阈值：|cor| 超过才尝试补模块间边</summary>
        Public Property CrossModuleCorThreshold As Double = 0.3

        ''' <summary>hub 基因间相关阈值：|r| 超过才在对应基因间补跨模块边</summary>
        Public Property CrossGeneCorThreshold As Double = 0.4

        ''' <summary>跨模块边的初始权重缩放（最终由全局参数学习覆盖）</summary>
        Public Property CrossScale As Double = 0.5


        Dim model As BlockNetwork
        Dim infer As BlockPropagate

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetModuleHubSources() As String()
            Return model.GetModuleHubSources.ToArray
        End Function

        ' ============================================================
        ' 1. 主入口
        ' ============================================================

        ''' <summary>
        ''' 模块切分 → 子网络训练 → 全局矩阵拼接
        ''' </summary>
        ''' <param name="assignment">WGCNA 模块划分结果（geneID / moduleColor / kME）</param>
        ''' <param name="expr">全局表达矩阵（基因 × 样本）</param>
        ''' <returns></returns>
        Public Function Learn(assignment As GeneModuleColor(), expr As GeneExpressionData) As ModularNetworkPipeline
            model = New BlockNetwork(expr, normalizeData:=NormalizeData) With {
                .CrossGeneCorThreshold = CrossGeneCorThreshold,
                .CrossModuleCorThreshold = CrossModuleCorThreshold,
                .CrossScale = CrossScale,
                .HubTopN = HubTopN,
                .StructureParams = StructureParams
            }
            infer = New BlockPropagate With {
                .Model = model.Learn(assignment),
                .MaxSteps = MaxSteps,
                .Tolerance = Tolerance,
                .NSamples = NSamples,
                .RandomSeed = RandomSeed
            }

            Return Me
        End Function

        ''' <summary>
        ''' 各源基因全局扰动。
        ''' </summary>
        ''' <param name="sources">扰动源基因列表</param>
        ''' <returns>每个扰动源的全局扰动结果</returns>
        Public Iterator Function InsilicoPerturbation(sources As IEnumerable(Of String), mode As InterventionMode) As IEnumerable(Of GlobalPerturbationResult)
            ' 确定扰动源
            Dim srcList As New List(Of String)(sources.SafeQuery)

            Call $"[WGCNASubnetworkPipeline] 虚拟扰动实验的代表基因共 {srcList.Count} 个".debug

            For Each src As String In srcList
                Dim gi As Integer = model.GetGlobalIndex(src)
                If gi < 0 Then
                    Call $"[WGCNASubnetworkPipeline] 警告: 扰动源 '{src}' 不在表达矩阵中，跳过".debug
                    Continue For
                End If
                Dim r As GlobalPerturbationResult
                If Propagation = PropagationMethod.Jacobian Then
                    r = infer.PropagateJacobian(gi, mode)
                Else
                    r = infer.PropagateCascade(gi, mode)
                End If

                Yield r

                Call r.ToString().debug
            Next
        End Function

        ' ============================================================
        ' 8. 结果导出
        ' ============================================================

        ''' <summary>
        ''' 写出全局扰动响应矩阵（行=基因，列=各扰动源）与每个源的明细 TSV，并打印摘要。
        ''' </summary>
        Public Sub SaveResults(results As IReadOnlyCollection(Of GlobalPerturbationResult), outputDir As String)
            If Not Directory.Exists(outputDir) Then
                Directory.CreateDirectory(outputDir)
            End If

            ' 全局响应矩阵
            Dim sbMatrix As New StringBuilder()
            sbMatrix.Append("gene")
            For Each r In results
                sbMatrix.Append(vbTab).Append(r.SourceGene)
            Next
            sbMatrix.AppendLine()

            For i = 0 To model._genes.Length - 1
                sbMatrix.Append(model._genes(i))
                For Each r In results
                    sbMatrix.Append(vbTab).Append(r.Effects(i).ToString("F6"))
                Next
                sbMatrix.AppendLine()
            Next
            File.WriteAllText(Path.Combine(outputDir, "global_perturbation_responses.tsv"), sbMatrix.ToString())

            ' 每个源的明细
            For Each r In results
                Dim safe = New String(r.SourceGene.Where(Function(c) Char.IsLetterOrDigit(c)).ToArray())
                File.WriteAllText(Path.Combine(outputDir, "pert_" & safe & ".tsv"), r.ToTSV())
            Next

            ' 控制台摘要
            For Each r In results
                Console.WriteLine(r.ToString())
            Next
        End Sub

        ''' <summary>
        ''' zip 压缩包内的模型格式版本号
        ''' </summary>
        Private Const ModelFormatVersion As Integer = 1

        ''' <summary>
        ''' Save current model as zip archive file
        ''' </summary>
        ''' <param name="s">
        ''' 目标输出流，由调用方提供并负责释放，因此这里以 leaveOpen 的方式使用 <see cref="ZipArchive"/>。
        ''' </param>
        ''' 
        ''' zip 布局：
        ''' ```
        ''' meta.txt             version / genes / modules / subnets / samples / 各段存在性标志
        ''' settings.txt         流水线级别的扰动与训练开关
        ''' network.txt          BlockNetwork 训练参数（含结构学习参数）
        ''' propagate.txt        BlockPropagate 传播参数
        ''' genes.txt            全局基因名，行号即 GetGlobalIndex 的索引
        ''' A.bin                全局雅可比系数矩阵（二进制 nG × nG）
        ''' global/              全局聚合网络（nodes.txt / edges.tsv / cpt.tsv）
        ''' expr/                标准化训练表达矩阵（genes / samples / timepoints.bin / matrix.bin）
        ''' modules.tsv          模块色 \t 基因数 \t hub 数（行号即模块序）
        ''' modules/0000/        模块成员基因与 hub 基因
        ''' subnets/0000/        各模块训练子网络（nodes.txt / edges.tsv / cpt.tsv）
        ''' ```
        ''' 
        ''' 说明：
        ''' 1. 表达矩阵与雅可比矩阵 A 一律走二进制块（见 <see cref="WriteMatrix"/>）；
        ''' 2. A 与全局网络 CPD 在训练末尾本就相互自洽，这里仍显式落盘 A，
        '''    因为它是雅可比传播的唯一输入、也是本模型的核心产物；
        ''' 3. 原始未标准化表达矩阵（BlockNetwork._expr）不落盘——体积与标准化矩阵相当，
        '''    且仅在训练日志里被用到，载入时以标准化矩阵替代。
        Public Sub SaveModel(s As Stream)
            If s Is Nothing Then
                Throw New ArgumentNullException(NameOf(s))
            End If
            If model Is Nothing OrElse model._globalNet Is Nothing Then
                Throw New InvalidOperationException("当前流水线尚未调用 Learn 完成训练，没有可以被导出的模型。")
            End If

            Dim net As BlockNetwork = model
            Dim genes As String() = If(net._genes, New String() {})
            Dim std As GeneExpressionData = net._exprStd
            Dim modules As List(Of String) = net._moduleGenes.Keys.ToList()
            Dim subnets As List(Of BayesianNetwork) = If(net._subNets, New List(Of BayesianNetwork)())

            Using zip As New ZipArchive(s, ZipArchiveMode.Create, leaveOpen:=True)
                Call WriteText(zip, "meta.txt", Sub(w)
                                                    w.WriteLine($"version={ModelFormatVersion}")
                                                    w.WriteLine($"genes={genes.Length}")
                                                    w.WriteLine($"modules={modules.Count}")
                                                    w.WriteLine($"subnets={subnets.Count}")
                                                    w.WriteLine($"samples={If(std Is Nothing, 0, std.NSample)}")
                                                    w.WriteLine($"has_global={If(net._globalNet Is Nothing, 0, 1)}")
                                                    w.WriteLine($"has_expr={If(std Is Nothing, 0, 1)}")
                                                End Sub)

                Call WriteText(zip, "settings.txt", Sub(w)
                                                        w.WriteLine($"Propagation={Propagation.ToString()}")
                                                        w.WriteLine($"MaxSteps={MaxSteps}")
                                                        w.WriteLine($"Tolerance={Num(Tolerance)}")
                                                        w.WriteLine($"NSamples={NSamples}")
                                                        w.WriteLine($"RandomSeed={RandomSeed}")
                                                        w.WriteLine($"NormalizeData={NormalizeData}")
                                                    End Sub)

                Call WriteText(zip, "network.txt", Sub(w)
                                                       w.WriteLine($"HubTopN={net.HubTopN}")
                                                       w.WriteLine($"CrossModuleCorThreshold={Num(net.CrossModuleCorThreshold)}")
                                                       w.WriteLine($"CrossGeneCorThreshold={Num(net.CrossGeneCorThreshold)}")
                                                       w.WriteLine($"CrossScale={Num(net.CrossScale)}")
                                                       w.WriteLine($"Algorithm={net.StructureParams.Algorithm.ToString()}")
                                                       w.WriteLine($"Alpha={Num(net.StructureParams.Alpha)}")
                                                       w.WriteLine($"MaxParents={net.StructureParams.MaxParents}")
                                                       w.WriteLine($"TabuLength={net.StructureParams.TabuLength}")
                                                       w.WriteLine($"MaxIterations={net.StructureParams.MaxIterations}")
                                                       w.WriteLine($"BICPenalty={Num(net.StructureParams.BICPenalty)}")
                                                       w.WriteLine($"UseWhitelist={net.StructureParams.UseWhitelist}")
                                                       w.WriteLine($"UseBlacklist={net.StructureParams.UseBlacklist}")
                                                       w.WriteLine($"StructRandomSeed={net.StructureParams.RandomSeed}")
                                                   End Sub)

                Dim prop As BlockPropagate = infer

                Call WriteText(zip, "propagate.txt", Sub(w)
                                                         w.WriteLine($"MaxSteps={If(prop Is Nothing, MaxSteps, prop.MaxSteps)}")
                                                         w.WriteLine($"Tolerance={Num(If(prop Is Nothing, Tolerance, prop.Tolerance))}")
                                                         w.WriteLine($"NSamples={If(prop Is Nothing, NSamples, prop.NSamples)}")
                                                         w.WriteLine($"RandomSeed={If(prop Is Nothing, RandomSeed, prop.RandomSeed)}")
                                                     End Sub)

                Call WriteText(zip, "genes.txt", Sub(w)
                                                     For Each g As String In genes
                                                         w.WriteLine(Sanitize(g))
                                                     Next
                                                 End Sub)

                Call WriteMatrix(zip, "A.bin", net._A)
                Call WriteNet(zip, "global/", net._globalNet)

                If std IsNot Nothing Then
                    Call WriteText(zip, "expr/genes.txt", Sub(w)
                                                              For Each g As String In If(std.GeneNames, New String() {})
                                                                  w.WriteLine(Sanitize(g))
                                                              Next
                                                          End Sub)
                    Call WriteText(zip, "expr/samples.txt", Sub(w)
                                                                For Each n As String In If(std.SampleNames, New String() {})
                                                                    w.WriteLine(Sanitize(n))
                                                                Next
                                                            End Sub)
                    Call WriteDoubles(zip, "expr/timepoints.bin", std.TimePoints)
                    Call WriteMatrix(zip, "expr/matrix.bin", std.Matrix)
                End If

                Call WriteText(zip, "modules.tsv", Sub(w)
                                                       For Each color As String In modules
                                                           Dim nGene As Integer = net._moduleGenes(color).Count
                                                           Dim nHub As Integer = If(net._moduleHubs.ContainsKey(color), net._moduleHubs(color).Count, 0)

                                                           w.WriteLine(String.Join(vbTab, {
                                                               Sanitize(color),
                                                               nGene.ToString(CultureInfo.InvariantCulture),
                                                               nHub.ToString(CultureInfo.InvariantCulture)
                                                           }))
                                                       Next
                                                   End Sub)

                For i As Integer = 0 To modules.Count - 1
                    Dim dir As String = IndexDir("modules", i)
                    Dim color As String = modules(i)

                    Call WriteText(zip, dir & "genes.txt", Sub(w)
                                                               For Each g As String In net._moduleGenes(color)
                                                                   w.WriteLine(Sanitize(g))
                                                               Next
                                                           End Sub)
                    Call WriteText(zip, dir & "hubs.txt", Sub(w)
                                                              For Each g As String In If(net._moduleHubs.ContainsKey(color), net._moduleHubs(color), New List(Of String)())
                                                                  w.WriteLine(Sanitize(g))
                                                              Next
                                                          End Sub)
                Next

                For i As Integer = 0 To subnets.Count - 1
                    Call WriteNet(zip, IndexDir("subnets", i), subnets(i))
                Next
            End Using

            Call $"[ModularNetworkPipeline] 模型已导出: modules={modules.Count}, genes={genes.Length}, subnets={subnets.Count}".info
        End Sub

        ''' <summary>
        ''' load <see cref="ModularNetworkPipeline"/> model from a zip archive file.
        ''' </summary>
        ''' <param name="s">zip 压缩包输入流，由调用方提供并负责释放</param>
        ''' <returns>
        ''' 还原后的流水线对象：全局基因索引、雅可比矩阵 A、全局聚合网络及其 CPD、
        ''' 标准化表达矩阵、WGCNA 模块划分与 hub 基因、各模块子网络以及全部训练/传播参数
        ''' 均与保存前一致，可直接执行 <see cref="InsilicoPerturbation"/> 与 <see cref="SaveResults"/>。
        ''' </returns>
        Public Shared Function LoadModel(s As Stream) As ModularNetworkPipeline
            If s Is Nothing Then
                Throw New ArgumentNullException(NameOf(s))
            End If

            Dim pipeline As New ModularNetworkPipeline()
            Dim net As New BlockNetwork()

            Using zip As New ZipArchive(s, ZipArchiveMode.Read, leaveOpen:=True)
                Dim meta As Dictionary(Of String, String) = ReadMeta(GetEntry(zip, "meta.txt"))
                Dim verText As String = Nothing
                Dim version As Integer = 0

                If meta Is Nothing OrElse Not meta.TryGetValue("version", verText) Then
                    Throw New InvalidDataException("modular_pipe 模型文件缺少版本信息，可能不是有效的模型压缩包。")
                End If
                If Not Integer.TryParse(verText, NumberStyles.Integer, CultureInfo.InvariantCulture, version) OrElse
                    version <> ModelFormatVersion Then

                    Throw New InvalidDataException($"modular_pipe 模型文件版本不匹配：文件为 {verText}，当前程序支持 {ModelFormatVersion}。")
                End If

                ' ---- 流水线级标量 ----
                Dim cfg As Dictionary(Of String, String) = ReadMeta(GetEntry(zip, "settings.txt"))
                Dim methodText As String = GetValue(cfg, "Propagation")
                Dim method As PropagationMethod = PropagationMethod.Jacobian

                If methodText.Length > 0 AndAlso [Enum].TryParse(Of PropagationMethod)(methodText, True, method) Then
                    pipeline.Propagation = method
                End If

                pipeline.MaxSteps = GetInt(cfg, "MaxSteps", 50)
                pipeline.Tolerance = GetDouble(cfg, "Tolerance", 0.000001)
                pipeline.NSamples = GetInt(cfg, "NSamples", 10000)
                pipeline.RandomSeed = GetInt(cfg, "RandomSeed", 42)
                pipeline.NormalizeData = GetBool(cfg, "NormalizeData", True)

                ' ---- 网络训练参数（流水线与 BlockNetwork 上各持一份，保持与 Learn 一致的共享语义） ----
                Dim ncfg As Dictionary(Of String, String) = ReadMeta(GetEntry(zip, "network.txt"))

                pipeline.HubTopN = GetInt(ncfg, "HubTopN", 20)
                pipeline.CrossModuleCorThreshold = GetDouble(ncfg, "CrossModuleCorThreshold", 0.3)
                pipeline.CrossGeneCorThreshold = GetDouble(ncfg, "CrossGeneCorThreshold", 0.4)
                pipeline.CrossScale = GetDouble(ncfg, "CrossScale", 0.5)

                Dim algoText As String = GetValue(ncfg, "Algorithm")
                Dim algo As StructureAlgorithm = StructureAlgorithm.MMHC

                If algoText.Length > 0 AndAlso [Enum].TryParse(Of StructureAlgorithm)(algoText, True, algo) Then
                    pipeline.StructureParams.Algorithm = algo
                End If

                pipeline.StructureParams.Alpha = GetDouble(ncfg, "Alpha", 0.05)
                pipeline.StructureParams.MaxParents = GetInt(ncfg, "MaxParents", 5)
                pipeline.StructureParams.TabuLength = GetInt(ncfg, "TabuLength", 20)
                pipeline.StructureParams.MaxIterations = GetInt(ncfg, "MaxIterations", 500)
                pipeline.StructureParams.BICPenalty = GetDouble(ncfg, "BICPenalty", 1.0)
                pipeline.StructureParams.UseWhitelist = GetBool(ncfg, "UseWhitelist", True)
                pipeline.StructureParams.UseBlacklist = GetBool(ncfg, "UseBlacklist", True)
                pipeline.StructureParams.RandomSeed = GetInt(ncfg, "StructRandomSeed", 42)

                net.HubTopN = pipeline.HubTopN
                net.CrossModuleCorThreshold = pipeline.CrossModuleCorThreshold
                net.CrossGeneCorThreshold = pipeline.CrossGeneCorThreshold
                net.CrossScale = pipeline.CrossScale
                net.StructureParams = pipeline.StructureParams

                ' ---- 全局基因索引 ----
                net._genes = ReadNames(GetEntry(zip, "genes.txt"))
                net._gIndex = New Dictionary(Of String, Integer)()

                For i As Integer = 0 To net._genes.Length - 1
                    net._gIndex(net._genes(i)) = i
                Next

                ' ---- 全局雅可比矩阵与聚合网络 ----
                net._A = ReadMatrix(GetEntry(zip, "A.bin"))

                If GetBool(meta, "has_global", False) Then
                    net._globalNet = ReadNet(zip, "global/")
                End If

                ' ---- 标准化训练表达矩阵 ----
                If GetBool(meta, "has_expr", False) Then
                    Dim geneNames As String() = ReadNames(GetEntry(zip, "expr/genes.txt"))
                    Dim sampleNames As String() = ReadNames(GetEntry(zip, "expr/samples.txt"))
                    Dim times As Double() = ReadDoubles(GetEntry(zip, "expr/timepoints.bin"))
                    Dim matrix As Double(,) = ReadMatrix(GetEntry(zip, "expr/matrix.bin"))
                    Dim nS As Integer = GetInt(meta, "samples", sampleNames.Length)

                    If matrix Is Nothing Then
                        matrix = New Double(If(geneNames.Length > 1, geneNames.Length, 1) - 1,
                                            If(nS > 1, nS, 1) - 1) {}
                    End If
                    If times Is Nothing OrElse times.Length <> nS Then
                        times = Enumerable.Repeat(0.0, If(nS > 0, nS, 0)).ToArray()
                    End If

                    net._exprStd = New GeneExpressionData() With {
                        .GeneNames = geneNames,
                        .SampleNames = sampleNames,
                        .Matrix = matrix,
                        .TimePoints = times
                    }
                    ' 原始未标准化矩阵未落盘，这里以标准化矩阵顶替（仅影响 Learn 末尾的一句日志）
                    net._expr = net._exprStd
                End If

                ' ---- WGCNA 模块划分与 hub ----
                Dim moduleRows As String() = ReadLines(GetEntry(zip, "modules.tsv"))

                net._moduleGenes = New Dictionary(Of String, List(Of String))()
                net._moduleHubs = New Dictionary(Of String, List(Of String))()

                For i As Integer = 0 To moduleRows.Length - 1
                    Dim p As String() = moduleRows(i).Split(New String() {vbTab}, StringSplitOptions.None)

                    If p.Length = 0 Then Continue For

                    Dim color As String = p(0)
                    Dim dir As String = IndexDir("modules", i)

                    net._moduleGenes(color) = ReadNames(GetEntry(zip, dir & "genes.txt")).ToList()
                    net._moduleHubs(color) = ReadNames(GetEntry(zip, dir & "hubs.txt")).ToList()
                Next

                ' ---- 各模块训练子网络 ----
                Dim subnetCount As Integer = GetInt(meta, "subnets", 0)

                net._subNets = New List(Of BayesianNetwork)()

                For i As Integer = 0 To subnetCount - 1
                    Call net._subNets.Add(ReadNet(zip, IndexDir("subnets", i)))
                Next

                ' ---- 传播器 ----
                Dim pcfg As Dictionary(Of String, String) = ReadMeta(GetEntry(zip, "propagate.txt"))

                pipeline.model = net
                pipeline.infer = New BlockPropagate() With {
                    .Model = net,
                    .MaxSteps = GetInt(pcfg, "MaxSteps", pipeline.MaxSteps),
                    .Tolerance = GetDouble(pcfg, "Tolerance", pipeline.Tolerance),
                    .NSamples = GetInt(pcfg, "NSamples", pipeline.NSamples),
                    .RandomSeed = GetInt(pcfg, "RandomSeed", pipeline.RandomSeed)
                }
            End Using

            Call $"[ModularNetworkPipeline] 模型已载入: modules={net._moduleGenes.Count}, genes={If(net._genes Is Nothing, 0, net._genes.Length)}, subnets={net._subNets.Count}".info

            Return pipeline
        End Function

        ' ==================== zip 持久化辅助 ====================

        ''' <summary>模块 / 子网络在 zip 内的目录名（用序号命名，避免模块颜色中的特殊字符影响条目名）</summary>
        Private Shared Function IndexDir(kind As String, index As Integer) As String
            Return $"{kind}/{index.ToString("D4")}/"
        End Function

        ''' <summary>将一段文本写入 zip 包内的指定条目</summary>
        Private Shared Sub WriteText(zip As ZipArchive, name As String, write As Action(Of TextWriter))
            Dim entry As ZipArchiveEntry = zip.CreateEntry(name, CompressionLevel.Optimal)

            Using w As New StreamWriter(entry.Open())
                Call write(w)
            End Using
        End Sub

        ''' <summary>按条目名查找 zip 内的条目（路径分隔符统一为 /，大小写不敏感）</summary>
        ''' <returns>不存在时返回 Nothing，调用方按缺省值降级处理</returns>
        Private Shared Function GetEntry(zip As ZipArchive, name As String) As ZipArchiveEntry
            Dim target As String = name.Replace("\"c, "/"c)

            For Each e As ZipArchiveEntry In zip.Entries
                If String.Equals(e.FullName.Replace("\"c, "/"c), target, StringComparison.OrdinalIgnoreCase) Then
                    Return e
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>读取文本条目的全部非空行</summary>
        Private Shared Function ReadLines(entry As ZipArchiveEntry) As String()
            If entry Is Nothing Then
                Return New String() {}
            End If

            Dim lines As New List(Of String)

            Using sr As New StreamReader(entry.Open())
                Do While Not sr.EndOfStream
                    Dim line As String = sr.ReadLine()

                    If Not String.IsNullOrWhiteSpace(line) Then
                        lines.Add(line)
                    End If
                Loop
            End Using

            Return lines.ToArray()
        End Function

        ''' <summary>
        ''' 读取名称清单（节点名 / 基因名 / 样本名）。
        ''' 与 <see cref="ReadLines"/> 不同，这里保留空行，否则行号会与索引错位，
        ''' 仅剔除文件末尾换行所产生的那一个空行。
        ''' </summary>
        Private Shared Function ReadNames(entry As ZipArchiveEntry) As String()
            If entry Is Nothing Then
                Return New String() {}
            End If

            Dim lines As New List(Of String)

            Using sr As New StreamReader(entry.Open())
                Do While Not sr.EndOfStream
                    Call lines.Add(sr.ReadLine())
                Loop
            End Using

            If lines.Count > 0 AndAlso lines(lines.Count - 1).Length = 0 Then
                Call lines.RemoveAt(lines.Count - 1)
            End If

            Return lines.ToArray()
        End Function

        ''' <summary>读取 key=value 形式的元数据条目</summary>
        Private Shared Function ReadMeta(entry As ZipArchiveEntry) As Dictionary(Of String, String)
            Dim meta As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

            For Each line As String In ReadLines(entry)
                Dim i As Integer = line.IndexOf("="c)

                If i <= 0 Then Continue For

                meta(line.Substring(0, i).Trim()) = line.Substring(i + 1).Trim()
            Next

            Return meta
        End Function

        ''' <summary>
        ''' 写出一个贝叶斯网络的三段：nodes.txt（行号即节点索引）/ edges.tsv / cpt.tsv。
        ''' 全局聚合网络与各模块子网络共用这一对读写函数。
        ''' </summary>
        Private Shared Sub WriteNet(zip As ZipArchive, prefix As String, net As BayesianNetwork)
            Dim edges As List(Of (FromIdx As Integer, ToIdx As Integer)) =
                If(net.Nodes.Count > 0, net.GetEdges(), New List(Of (FromIdx As Integer, ToIdx As Integer))())

            Call WriteText(zip, prefix & "nodes.txt", Sub(w)
                                                          For Each node As BnNode In net.Nodes
                                                              w.WriteLine(Sanitize(node.Name))
                                                          Next
                                                      End Sub)

            Call WriteText(zip, prefix & "edges.tsv", Sub(w)
                                                          For Each e In edges
                                                              w.WriteLine($"{e.FromIdx}{vbTab}{e.ToIdx}")
                                                          Next
                                                      End Sub)

            Call WriteText(zip, prefix & "cpt.tsv", Sub(w)
                                                        For i As Integer = 0 To net.Nodes.Count - 1
                                                            Dim cpd As BnCPD = net.Nodes(i).CPD

                                                            If cpd IsNot Nothing Then
                                                                w.WriteLine(String.Join(vbTab, {
                                                                    i.ToString(CultureInfo.InvariantCulture),
                                                                    Num(cpd.Intercept),
                                                                    JoinNums(cpd.Coeffs),
                                                                    JoinInts(cpd.ParentIndices),
                                                                    Num(cpd.ResidualSD),
                                                                    Num(cpd.ResidualVariance),
                                                                    Num(cpd.RSquared),
                                                                    Num(cpd.BIC),
                                                                    cpd.NSamples.ToString(CultureInfo.InvariantCulture)
                                                                }))
                                                            End If
                                                        Next
                                                    End Sub)
        End Sub

        ''' <summary>读回 <see cref="WriteNet"/> 写出的贝叶斯网络（含全部 CPD）</summary>
        Private Shared Function ReadNet(zip As ZipArchive, prefix As String) As BayesianNetwork
            Dim net As New BayesianNetwork()

            For Each name As String In ReadNames(GetEntry(zip, prefix & "nodes.txt"))
                Call net.AddNode(name)
            Next

            For Each line As String In ReadLines(GetEntry(zip, prefix & "edges.tsv"))
                Dim p As String() = line.Split(New String() {vbTab}, StringSplitOptions.None)
                Dim fromIdx As Integer = 0, toIdx As Integer = 0

                If p.Length >= 2 AndAlso
                    Integer.TryParse(p(0), NumberStyles.Integer, CultureInfo.InvariantCulture, fromIdx) AndAlso
                    Integer.TryParse(p(1), NumberStyles.Integer, CultureInfo.InvariantCulture, toIdx) Then

                    Call net.AddEdge(fromIdx, toIdx)
                End If
            Next

            For Each line As String In ReadLines(GetEntry(zip, prefix & "cpt.tsv"))
                Dim p As String() = line.Split(New String() {vbTab}, StringSplitOptions.None)
                Dim idx As Integer = 0

                If p.Length < 9 Then Continue For
                If Not Integer.TryParse(p(0), NumberStyles.Integer, CultureInfo.InvariantCulture, idx) Then Continue For
                If idx < 0 OrElse idx >= net.Nodes.Count Then Continue For

                net.Nodes(idx).CPD = New BnCPD With {
                    .NodeIndex = idx,
                    .Intercept = ParseNum(p(1)),
                    .Coeffs = ParseNums(p(2)),
                    .ParentIndices = ParseInts(p(3)),
                    .ResidualSD = ParseNum(p(4)),
                    .ResidualVariance = ParseNum(p(5)),
                    .RSquared = ParseNum(p(6)),
                    .BIC = ParseNum(p(7)),
                    .NSamples = CInt(ParseNum(p(8)))
                }
            Next

            Return net
        End Function

        ''' <summary>
        ''' 以二进制块写出 double 向量（Int32 长度 + 数据体），避免大数组走文本解析。
        ''' </summary>
        Private Shared Sub WriteDoubles(zip As ZipArchive, name As String, values As Double())
            Dim entry As ZipArchiveEntry = zip.CreateEntry(name, CompressionLevel.Optimal)

            Using out As New BinaryWriter(entry.Open())
                If values Is Nothing Then
                    out.Write(0)
                Else
                    out.Write(values.Length)

                    For Each x As Double In values
                        out.Write(x)
                    Next
                End If
            End Using
        End Sub

        ''' <summary>读回 <see cref="WriteDoubles"/> 写出的 double 向量</summary>
        Private Shared Function ReadDoubles(entry As ZipArchiveEntry) As Double()
            If entry Is Nothing Then
                Return New Double() {}
            End If

            Using input As New BinaryReader(entry.Open())
                Dim n As Integer = input.ReadInt32()

                If n <= 0 Then
                    Return New Double() {}
                End If

                Dim buf As Double() = New Double(n - 1) {}

                For i As Integer = 0 To n - 1
                    buf(i) = input.ReadDouble()
                Next

                Return buf
            End Using
        End Function

        ''' <summary>
        ''' 以二进制块写出二维 double 矩阵：Int32 行数 + Int32 列数 + 行优先数据体。
        ''' 用于全局雅可比矩阵 A 与标准化表达矩阵（后者行列分别为基因与样本）。
        ''' </summary>
        Private Shared Sub WriteMatrix(zip As ZipArchive, name As String, m As Double(,))
            Dim entry As ZipArchiveEntry = zip.CreateEntry(name, CompressionLevel.Optimal)
            Dim nR As Integer = If(m Is Nothing, 0, m.GetLength(0))
            Dim nC As Integer = If(m Is Nothing, 0, m.GetLength(1))

            Using out As New BinaryWriter(entry.Open())
                out.Write(nR)
                out.Write(nC)

                For i As Integer = 0 To nR - 1
                    For j As Integer = 0 To nC - 1
                        out.Write(m(i, j))
                    Next
                Next
            End Using
        End Sub

        ''' <summary>读回 <see cref="WriteMatrix"/> 写出的矩阵，条目缺失或为空时返回 Nothing</summary>
        Private Shared Function ReadMatrix(entry As ZipArchiveEntry) As Double(,)
            If entry Is Nothing Then
                Return Nothing
            End If

            Using input As New BinaryReader(entry.Open())
                Dim nR As Integer = input.ReadInt32()
                Dim nC As Integer = input.ReadInt32()

                If nR <= 0 OrElse nC <= 0 Then
                    Return Nothing
                End If

                Dim m As Double(,) = New Double(nR - 1, nC - 1) {}

                For i As Integer = 0 To nR - 1
                    For j As Integer = 0 To nC - 1
                        m(i, j) = input.ReadDouble()
                    Next
                Next

                Return m
            End Using
        End Function

        ''' <summary>以 G17 无损格式写出数值（固定使用不变区域文化，避免受系统区域设置影响）</summary>
        Private Shared Function Num(d As Double) As String
            Return d.ToString("G17", CultureInfo.InvariantCulture)
        End Function

        ''' <summary>解析 <see cref="Num"/> 写出的数值，解析失败时返回 0 而不是抛出</summary>
        Private Shared Function ParseNum(s As String) As Double
            Dim d As Double = 0

            Call Double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, d)

            Return d
        End Function

        Private Shared Function JoinNums(values As Double()) As String
            If values Is Nothing Then Return ""
            Return String.Join(",", values.Select(Function(d) Num(d)))
        End Function

        Private Shared Function JoinInts(values As Integer()) As String
            If values Is Nothing Then Return ""
            Return String.Join(",", values.Select(Function(i) i.ToString(CultureInfo.InvariantCulture)))
        End Function

        Private Shared Function ParseNums(s As String) As Double()
            If String.IsNullOrWhiteSpace(s) Then Return New Double() {}
            Return s.Split(","c).Where(Function(x) x.Length > 0).Select(Function(x) ParseNum(x)).ToArray()
        End Function

        Private Shared Function ParseInts(s As String) As Integer()
            If String.IsNullOrWhiteSpace(s) Then Return New Integer() {}
            Return s.Split(","c).Where(Function(x) x.Length > 0).Select(Function(x) CInt(ParseNum(x))).ToArray()
        End Function

        ''' <summary>文本字段中不允许出现的字符：制表符、CR、LF（会破坏 TSV / 逐行文本格式）</summary>
        Private Shared ReadOnly IllegalChars As Char() = New Char() {ChrW(9), ChrW(13), ChrW(10)}

        ''' <summary>
        ''' 清理文本字段中的制表符与换行符，避免破坏 TSV / 逐行文本格式。
        ''' 先验网络动辄数十万条边，这里先做一次快速探测，不含非法字符时直接原样返回。
        ''' </summary>
        Private Shared Function Sanitize(s As String) As String
            If String.IsNullOrEmpty(s) Then Return ""
            If s.IndexOfAny(IllegalChars) < 0 Then Return s

            Return s.Replace(ChrW(9), " "c).Replace(ChrW(13), " "c).Replace(ChrW(10), " "c)
        End Function

        ''' <summary>取元数据字符串值，缺失时返回缺省值</summary>
        Private Shared Function GetValue(meta As Dictionary(Of String, String), key As String, Optional def As String = "") As String
            Dim value As String = Nothing

            If meta IsNot Nothing AndAlso meta.TryGetValue(key, value) Then
                Return value
            End If

            Return def
        End Function

        ''' <summary>取元数据的布尔值，兼容 True/False 与 1/0 两种写法</summary>
        Private Shared Function GetBool(meta As Dictionary(Of String, String), key As String, def As Boolean) As Boolean
            Dim s As String = GetValue(meta, key)
            Dim b As Boolean = def

            If Boolean.TryParse(s, b) Then
                Return b
            End If

            Dim n As Integer = 0

            If Integer.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, n) Then
                Return n <> 0
            End If

            Return def
        End Function

        Private Shared Function GetInt(meta As Dictionary(Of String, String), key As String, def As Integer) As Integer
            Dim n As Integer = def

            If Integer.TryParse(GetValue(meta, key), NumberStyles.Integer, CultureInfo.InvariantCulture, n) Then
                Return n
            End If

            Return def
        End Function

        Private Shared Function GetDouble(meta As Dictionary(Of String, String), key As String, def As Double) As Double
            Dim d As Double = def

            If Double.TryParse(GetValue(meta, key), NumberStyles.Float, CultureInfo.InvariantCulture, d) Then
                Return d
            End If

            Return def
        End Function
    End Class

End Namespace
