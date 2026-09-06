Imports System.IO
Imports System.IO.Compression
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.BNLearn.ModularNetwork
Imports SMRUCC.genomics.Analysis.BNLearn.ModularNetwork.WGCNA
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Module WGCNADemo

    ''' <summary>
    ''' 基于 WGCNA 模块划分训练多个 BNLearn 子网络，并在整合后的全局网络上
    ''' 执行全局虚拟扰动（雅可比线性传播 + 级联采样传播）。
    ''' 结果写出为 TSV（基因 × 扰动源响应矩阵 + 每源明细）并打印 Top 变化基因摘要。
    ''' </summary>
    Sub Run()
        ' 1. 读取数据
        ' 注意：直接读取完整表达矩阵，避免用 WGCNA 基因列表做 Matrix 索引过滤导致
        ' rownames 与数据行数不一致（rownames 被缩减而数据行未同步缩减）。
        ' 模块子集的提取交由 pipeline 的 GeneExpressionData.GetSubMatrix 按模块基因自动完成。
        Dim modules As GeneModuleColor() = WGCNA.ReadModuleAssignment("K:\hsa\WGCNA_output-demo\gene_module_assignment.csv")
        Dim subMat As Matrix = Matrix.LoadData("K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv", tqdm_wrap:=True)

        Dim exprData = BnIO.ReadGeneExpressionMatrix(subMat)

        ' 2. 构建 WGCNA 子网络 + 全局扰动流水线
        Dim pipeline As New ModularNetworkPipeline() With {
            .NormalizeData = True,
            .NSamples = 5000,
            .RandomSeed = 42,
            .MaxSteps = 30,
            .HubTopN = 20,
            .CrossModuleCorThreshold = 0.3,
            .CrossGeneCorThreshold = 0.4,
            .CrossScale = 0.5
        }
        ' 结构学习参数（与 BNLearnWorkflow 一致）
        pipeline.StructureParams.MaxIterations = 500
        pipeline.Learn(modules, exprData)

        Dim source As String() = pipeline.GetModuleHubSources

        ' 3. 方法一（默认）：雅可比矩阵多步线性传播
        pipeline.Propagation = PropagationMethod.Jacobian
        Dim jacResults = pipeline.InsilicoPerturbation(source, Intervention.InterventionMode.Knockout).ToArray
        Dim outDirJac = App.HOME & "/output/wgcna_global_perturbation/jacobian"
        Call pipeline.SaveResults(jacResults, outDirJac)

        ' 4. 方法二：级联采样跨模块传播（对前若干代表源演示，避免全量过慢）
        pipeline.Propagation = PropagationMethod.CascadeSampling
        Dim demoSources = jacResults.Take(5).Select(Function(r) r.SourceGene).ToArray()
        Dim casResults = pipeline.InsilicoPerturbation(demoSources, Intervention.InterventionMode.Knockout).ToArray
        Dim outDirCas = App.HOME & "/output/wgcna_global_perturbation/cascade"
        Call pipeline.SaveResults(casResults, outDirCas)

        Call Console.WriteLine("[WGCNADemo] 全局虚拟扰动流程完成。雅可比方法源数={0}, 级联方法源数={1}", jacResults.Count, casResults.Count)
        Call Console.WriteLine("[WGCNADemo] 结果目录: " & App.HOME & "/output/wgcna_global_perturbation/")
    End Sub

    ' ============================================================
    ' 模型持久化（zip 导出 / 载入）往返一致性测试
    ' 运行方式：test.exe persistence
    ' ============================================================

    ''' <summary>持久化测试累积的失败项</summary>
    Private failures As New List(Of String)

    ''' <summary>打印一条断言结果并累积失败项</summary>
    Private Sub Check(name As String, ok As Boolean, Optional detail As String = "")
        If ok Then
            Call Console.WriteLine($"  [PASS] {name}")
        Else
            Call Console.WriteLine($"  [FAIL] {name} {detail}")
            Call failures.Add(name)
        End If
    End Sub

    ''' <summary>
    ''' 两个 double 向量的最大绝对差；长度不一致或出现单边 NaN 时返回 -1（表示不可比）。
    ''' </summary>
    Private Function MaxAbsDiff(a As Double(), b As Double()) As Double
        If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length Then
            Return -1
        End If

        Dim d As Double = 0

        For i = 0 To a.Length - 1
            If Double.IsNaN(a(i)) OrElse Double.IsNaN(b(i)) Then
                If Not (Double.IsNaN(a(i)) AndAlso Double.IsNaN(b(i))) Then
                    Return -1
                End If
            ElseIf Not a(i).Equals(b(i)) Then
                Dim diff As Double = If(a(i) > b(i), a(i) - b(i), b(i) - a(i))

                If diff > d Then d = diff
            End If
        Next

        Return d
    End Function

    ''' <summary>读取 zip 条目的解压后字节</summary>
    Private Function EntryBytes(entry As ZipArchiveEntry) As Byte()
        Using ms As New MemoryStream()
            Using s As Stream = entry.Open()
                Call s.CopyTo(ms)
            End Using

            Return ms.ToArray()
        End Using
    End Function

    ''' <summary>比对两个 zip 内各条目的解压内容，返回不一致 / 缺失 / 多出的条目名</summary>
    Private Function CompareZip(fileA As String, fileB As String) As List(Of String)
        Dim diff As New List(Of String)

        Using zipA As ZipArchive = ZipFile.OpenRead(fileA)
            Using zipB As ZipArchive = ZipFile.OpenRead(fileB)
                For Each entry In zipA.Entries
                    Dim other As ZipArchiveEntry = zipB.GetEntry(entry.FullName)

                    If other Is Nothing Then
                        Call diff.Add(entry.FullName & "(missing)")
                    ElseIf Not EntryBytes(entry).SequenceEqual(EntryBytes(other)) Then
                        Call diff.Add(entry.FullName)
                    End If
                Next

                For Each entry In zipB.Entries
                    If zipA.GetEntry(entry.FullName) Is Nothing Then
                        Call diff.Add(entry.FullName & "(extra)")
                    End If
                Next
            End Using
        End Using

        Return diff
    End Function

    ''' <summary>
    ''' ModularNetworkPipeline 模型持久化往返测试：
    ''' 训练 → 跑一批虚拟扰动作为基线 → 导出 zip → 载入 → 逐一断言状态与扰动结果一致 →
    ''' 再导出一次并与首个 zip 逐条目比对，验证完全无损往返。
    ''' </summary>
    ''' <param name="moduleFile">WGCNA 模块划分结果 csv</param>
    ''' <param name="exprFile">全局表达矩阵 csv</param>
    Sub RunPersistenceTest(Optional moduleFile As String = "K:\hsa\WGCNA_output-demo\gene_module_assignment.csv",
                           Optional exprFile As String = "K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv")
        Call failures.Clear()

        ' ---- 1. 读取数据 ----
        Call Console.WriteLine("=== [1] 读取数据 ===")

        Dim modules As GeneModuleColor() = WGCNA.ReadModuleAssignment(moduleFile)
        Dim subMat As Matrix = Matrix.LoadData(exprFile, tqdm_wrap:=True)
        Dim exprAll = BnIO.ReadGeneExpressionMatrix(subMat)

        ' 预先把表达矩阵收窄到 WGCNA 涉及的基因：pipeline 内部本来就只保留模块基因，
        ' 先裁剪可以显著降低 Standardize 与后续建模的内存/时间开销
        Dim wgcnaGenes As String() = modules.Select(Function(m) m.geneID).Distinct().ToArray()
        Dim exprData As GeneExpressionData = exprAll.GetSubMatrix(wgcnaGenes)

        If exprData Is Nothing Then
            Call Console.WriteLine("[WGCNADemo] 表达矩阵中没有任何 WGCNA 基因，测试中止。")
            Return
        End If

        Call Console.WriteLine($"  表达矩阵: {exprData.NGene} 基因 × {exprData.NSample} 样本")

        ' ---- 2. 训练 ----
        Call Console.WriteLine("=== [2] 训练流水线 ===")

        Dim pipeline As New ModularNetworkPipeline() With {
            .NormalizeData = True,
            .NSamples = 2000,
            .RandomSeed = 42,
            .MaxSteps = 20,
            .HubTopN = 20,
            .CrossModuleCorThreshold = 0.3,
            .CrossGeneCorThreshold = 0.4,
            .CrossScale = 0.5
        }

        pipeline.StructureParams.MaxIterations = 20
        pipeline.Learn(modules, exprData)

        Dim sources As String() = pipeline.GetModuleHubSources.Take(3).ToArray()

        Call Console.WriteLine($"  扰动源: {String.Join(", ", sources)}")

        ' ---- 3. 保存前的扰动基线 ----
        Call Console.WriteLine("=== [3] 生成保存前的扰动基线 ===")

        pipeline.Propagation = PropagationMethod.Jacobian
        Dim beforeJac = pipeline.InsilicoPerturbation(sources, Intervention.InterventionMode.Knockout).ToArray()

        pipeline.Propagation = PropagationMethod.CascadeSampling
        Dim beforeCas = pipeline.InsilicoPerturbation(sources.Take(1), Intervention.InterventionMode.Knockout).ToArray()

        ' ---- 4. 导出 / 载入 ----
        Call Console.WriteLine("=== [4] 导出与载入模型 ===")

        Dim outDir As String = Path.Combine(App.HOME, "output/wgcna_pipeline_model")
        Dim zip1 As String = Path.Combine(outDir, "model.zip")
        Dim zip2 As String = Path.Combine(outDir, "model.roundtrip.zip")

        Call Directory.CreateDirectory(outDir)

        Using fs As New FileStream(zip1, FileMode.Create, FileAccess.ReadWrite)
            Call pipeline.SaveModel(fs)
            Call fs.Flush()
        End Using

        Dim loaded As ModularNetworkPipeline

        Using fs As New FileStream(zip1, FileMode.Open, FileAccess.Read)
            loaded = ModularNetworkPipeline.LoadModel(fs)
        End Using

        ' ---- 5. 一致性断言 ----
        Call Console.WriteLine("=== [5] 一致性断言 ===")

        ' 5.1 模块 hub 源（覆盖 _moduleHubs 的还原）
        Dim hub1 As String() = pipeline.GetModuleHubSources.ToArray()
        Dim hub2 As String() = loaded.GetModuleHubSources.ToArray()

        Check("模块 hub 源集合一致", hub1.SequenceEqual(hub2), $"({hub1.Length} vs {hub2.Length})")

        ' 5.2 全部标量参数
        Check("Propagation 一致", pipeline.Propagation = loaded.Propagation, $"({pipeline.Propagation} vs {loaded.Propagation})")
        Check("MaxSteps 一致", pipeline.MaxSteps = loaded.MaxSteps)
        Check("Tolerance 一致", pipeline.Tolerance.Equals(loaded.Tolerance))
        Check("NSamples 一致", pipeline.NSamples = loaded.NSamples)
        Check("RandomSeed 一致", pipeline.RandomSeed = loaded.RandomSeed)
        Check("NormalizeData 一致", pipeline.NormalizeData = loaded.NormalizeData)
        Check("HubTopN 一致", pipeline.HubTopN = loaded.HubTopN)
        Check("CrossModuleCorThreshold 一致", pipeline.CrossModuleCorThreshold.Equals(loaded.CrossModuleCorThreshold))
        Check("CrossGeneCorThreshold 一致", pipeline.CrossGeneCorThreshold.Equals(loaded.CrossGeneCorThreshold))
        Check("CrossScale 一致", pipeline.CrossScale.Equals(loaded.CrossScale))

        ' 5.3 雅可比传播（覆盖 _genes / _gIndex / _A 的还原）
        loaded.Propagation = PropagationMethod.Jacobian
        Dim afterJac = loaded.InsilicoPerturbation(sources, Intervention.InterventionMode.Knockout).ToArray()

        Check("Jacobian 扰动源序列一致",
              beforeJac.Select(Function(r) r.SourceGene).SequenceEqual(afterJac.Select(Function(r) r.SourceGene)))

        Dim jacStepsOk As Boolean = (beforeJac.Length > 0) AndAlso (beforeJac.Length = afterJac.Length)
        Dim jacDiff As Double = -1

        If jacStepsOk Then
            jacDiff = 0

            For i = 0 To beforeJac.Length - 1
                If beforeJac(i).Steps <> afterJac(i).Steps Then
                    jacStepsOk = False
                End If

                Dim d As Double = MaxAbsDiff(beforeJac(i).Effects, afterJac(i).Effects)

                If d < 0 Then
                    jacDiff = -1
                ElseIf d > jacDiff AndAlso jacDiff >= 0 Then
                    jacDiff = d
                End If
            Next
        End If

        Check("Jacobian 传播步数一致", jacStepsOk)
        Check("Jacobian 效应向量逐元素一致", jacStepsOk AndAlso jacDiff = 0, $"maxDiff={jacDiff}")

        ' 5.4 级联采样传播（覆盖 _globalNet / _exprStd 的还原）
        loaded.Propagation = PropagationMethod.CascadeSampling
        Dim afterCas = loaded.InsilicoPerturbation(sources.Take(1), Intervention.InterventionMode.Knockout).ToArray()
        Dim casDiff As Double = -1

        If beforeCas.Length > 0 AndAlso beforeCas.Length = afterCas.Length Then
            casDiff = 0

            For i = 0 To beforeCas.Length - 1
                Dim d As Double = MaxAbsDiff(beforeCas(i).Effects, afterCas(i).Effects)

                If d < 0 Then
                    casDiff = -1
                ElseIf d > casDiff AndAlso casDiff >= 0 Then
                    casDiff = d
                End If
            Next
        End If

        Check("Cascade 效应向量逐元素一致", casDiff = 0, $"maxDiff={casDiff}")

        ' 5.5 载入后再导出一次，逐条目比对（验证完全无损往返）
        Using fs As New FileStream(zip2, FileMode.Create, FileAccess.ReadWrite)
            Call loaded.SaveModel(fs)
            Call fs.Flush()
        End Using

        Dim diffEntries As List(Of String) = CompareZip(zip1, zip2)

        Check("载入后再导出的 zip 内容逐条目一致",
              diffEntries.Count = 0,
              If(diffEntries.Count > 0, String.Join(", ", diffEntries), ""))

        ' ---- 6. 汇总 ----
        Call Console.WriteLine("========================================")

        If failures.Count = 0 Then
            Call Console.WriteLine("[WGCNADemo] 持久化往返测试全部通过: PASS")
        Else
            Call Console.WriteLine($"[WGCNADemo] 持久化往返测试失败 {failures.Count} 项: {String.Join("; ", failures)}")
        End If

        Call Console.WriteLine($"[WGCNADemo] 模型文件: {zip1} ({New FileInfo(zip1).Length} bytes)")
    End Sub
End Module
