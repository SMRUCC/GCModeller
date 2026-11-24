Imports SMRUCC.genomics.Analysis.FuzzyGene.Data
Imports SMRUCC.genomics.Analysis.FuzzyGene.IO
Imports SMRUCC.genomics.BioSystem.Intervention

Namespace Perturbation

    ''' <summary>
    ''' 一次虚拟扰动的完整轨迹：标准干预结果 + 突变型轨迹 + 野生型（对照）轨迹。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 轨迹<b>不放进</b> <see cref="InterventionResult"/>：那个类型是 BNLearn / GPS /
    ''' CellPhenotype 共用的契约，加字段会波及它们的序列化与语义。这里用包装类型把
    ''' 「结论」与「过程」分开。
    ''' </para>
    ''' <para>
    ''' <see cref="Control"/> 不是可选的装饰：<see cref="AnfisPerturbationEngine.ControlSteadyState"/>
    ''' 目前直接返回训练基线（而不是不动点），所以突变型轨迹从基线出发时，
    ''' 前一两帧的跳变里混着「基线本身偏离不动点」的系统漂移。
    ''' 只有把两条曲线放在一起看，才能把漂移与扰动效应区分开。
    ''' </para>
    ''' </remarks>
    Public Class PerturbationTrace

        ''' <summary>标准干预结果（终态、FoldChange、Z 分数、显著性）。</summary>
        Public Property Result As InterventionResult

        ''' <summary>突变型（施加钳制）的求解轨迹。</summary>
        Public Property Mutant As SteadyStateTrace

        ''' <summary>野生型（不施加钳制）的求解轨迹。</summary>
        Public Property Control As SteadyStateTrace

        ''' <summary>被扰动的靶基因。</summary>
        Public Property TargetGene As String

        ''' <summary>干预模式。</summary>
        Public Property Mode As InterventionMode

        ''' <summary>施加的钳制值（原始表达量）。</summary>
        Public Property ClampValue As Double

        ''' <summary>完成这次分析（含求解）所花的墙钟时间，单位毫秒。</summary>
        Public Property ElapsedMs As Double

        ''' <summary>
        ''' 把两条轨迹各写成一个矩阵 CSV 与同名 <c>*.sampleinfo.json</c>，并写扰动清单。
        ''' </summary>
        ''' <param name="dir">输出目录</param>
        ''' <param name="dt">一帧对应的时间单位数（只影响 <c>metadata("time")</c>）</param>
        ''' <returns>实际写出的全部文件路径</returns>
        Public Function Export(dir As String, Optional dt As Double = 1.0) As String()
            Dim files As New List(Of String)
            Dim mutantOptions As TraceExportOptions = Nothing

            If Mutant IsNot Nothing Then
                mutantOptions = New TraceExportOptions With {
                    .Role = "mutant",
                    .SamplePrefix = "mut_t",
                    .GroupName = $"mutant:{Mode}:{TargetGene}",
                    .TargetGene = TargetGene,
                    .Mode = Mode.ToString(),
                    .ClampValue = ClampValue,
                    .Dt = dt,
                    .Color = "#D62728",
                    .Shape = "21"
                }

                Call files.AddRange(TraceExporter.Export(Mutant, dir, mutantOptions))
            End If

            If Control IsNot Nothing Then
                Dim controlOptions As New TraceExportOptions With {
                    .Role = "wildtype",
                    .SamplePrefix = "wt_t",
                    .GroupName = "wildtype",
                    .TargetGene = Nothing,
                    .Mode = Nothing,
                    .ClampValue = Double.NaN,
                    .Dt = dt,
                    .Color = "#1F77B4",
                    .Shape = "24"
                }

                Call files.AddRange(TraceExporter.Export(Control, dir, controlOptions))
            End If

            If mutantOptions IsNot Nothing Then
                Call files.Add(TraceExporter.WriteManifest(dir, mutantOptions, Mutant, Control, files.ToArray()))
            End If

            Return files.ToArray()
        End Function

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"perturbation trace: {Mode}:{TargetGene} -> {ClampValue:G4}, mutant={Mutant}, control={Control}"
        End Function

    End Class

    ''' <summary>
    ''' 候选扰动基因的响应强度排名条目。
    ''' </summary>
    ''' <remarks>
    ''' 评分只取<b>未被扰动</b>的基因里最大的 <c>|Z|</c>：被钳制的那个基因的 Z 分数
    ''' 是「人为施加值相对基线的偏离」，不是模型的推断，把它算进评分会让
    ''' 排名永远选中「本身就波动最大」的基因，失去意义。
    ''' </remarks>
    Public Class PerturbationCandidate

        ''' <summary>候选靶基因。</summary>
        Public Property GeneName As String

        ''' <summary>未被扰动基因的最大绝对 Z 分数。</summary>
        Public Property MaxAbsZScore As Double

        ''' <summary>显著变化的基因数量。</summary>
        Public Property NAffected As Integer

        ''' <summary>完成这次候选评估所花的墙钟时间，单位毫秒。</summary>
        Public Property ElapsedMs As Double

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"{GeneName}: max|Z|={MaxAbsZScore:G4}, affected={NAffected}"
        End Function

    End Class

End Namespace
