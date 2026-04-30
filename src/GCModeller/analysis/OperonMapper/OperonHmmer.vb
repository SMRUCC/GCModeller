Imports Microsoft.VisualBasic.DataMining.HiddenMarkovChain
Imports Microsoft.VisualBasic.DataMining.HiddenMarkovChain.Models
Imports SMRUCC.genomics.Model.OperonMapper.ContextModel

Public Class OperonHmmer

    ''' <summary>
    ''' 将 FeatureScores 中的所有特征离散化为单一的观测符号 (全特征版)
    ''' 注意：此方法会产生大量组合符号，可能导致HMM状态空间爆炸
    ''' </summary>
    ''' <param name="features">计算得到的特征得分对象</param>
    ''' <returns>离散化后的观测符号字符串，例如 "S_C_S_E_0_N"</returns>
    Public Shared Function DiscretizeAllFeatures(features As FeatureScores) As String
        ' 1. 基因间距离 - 基于论文的 U40, U200, O200
        Dim distSym As Char
        If features.IntergenicDistance < 40 Then
            distSym = "S"c ' Short (强Operon指示)
        ElseIf features.IntergenicDistance <= 200 Then
            distSym = "M"c ' Medium (模糊区)
        Else
            distSym = "L"c ' Long (强Boundary指示)
        End If

        ' 2. 邻域保守性 - 该得分为负对数似然，越小越保守
        Dim neighborSym As Char
        If features.NeighborhoodConservation < 1.0 Then
            neighborSym = "C"c ' Conserved (保守，倾向Operon)
        ElseIf features.NeighborhoodConservation <= 5.0 Then
            neighborSym = "N"c ' Neutral
        Else
            neighborSym = "U"c ' Unconserved (不保守)
        End If

        ' 3. 系统发育距离 - Hamming距离，越小越相关
        Dim phyloSym As Char
        If features.PhylogeneticDistance <= 5 Then
            phyloSym = "S"c ' Similar
        ElseIf features.PhylogeneticDistance <= 20 Then
            phyloSym = "M"c ' Medium
        Else
            phyloSym = "D"c ' Different
        End If

        ' 4. 基因长度比 - ln(up/down)，靠近0表示长度相近
        Dim lengthSym As Char
        If features.LengthRatio < -0.5 Then
            lengthSym = "D"c ' Downstream longer
        ElseIf features.LengthRatio <= 0.5 Then
            lengthSym = "E"c ' Equal (长度相近，论文发现Boundary对常在此区)
        Else
            lengthSym = "U"c ' Upstream longer
        End If

        ' 5. GO功能相似性 - 共同术语数量
        Dim goSym As String
        If features.GOSimilarity = 0 Then
            goSym = "0" ' None
        ElseIf features.GOSimilarity <= 3 Then
            goSym = "L" ' Low
        Else
            goSym = "H" ' High
        End If

        ' 6. DNA基序频率 (提取论文中最关键的 TTTTT 基序作为代表)
        ' 论文指出 TTTTT 在 Boundary 对中显著富集
        Dim motifSym As Char
        Dim tttttScore As Double = If(features.Motifs.ContainsKey("Motif_TTTTT"), features.Motifs("Motif_TTTTT"), 0.0)
        If tttttScore < 0.5 Then
            motifSym = "L"c ' Lower than expected (倾向Operon)
        ElseIf tttttScore <= 2.0 Then
            motifSym = "N"c ' Normal
        Else
            motifSym = "H"c ' Higher than expected (倾向Boundary)
        End If

        ' 组合符号: 距离_邻域_发育_长度_GO_Motif
        Return $"{distSym}_{neighborSym}_{phyloSym}_{lengthSym}_{goSym}_{motifSym}"
    End Function

    ''' <summary>
    ''' 将 FeatureScores 降维离散化 (推荐用于HMM)
    ''' 将特征分为物理、进化、功能三组，组内综合打分后再离散化
    ''' </summary>
    ''' <param name="features">计算得到的特征得分对象</param>
    ''' <returns>离散化后的观测符号字符串，例如 "P1_E1_F1"</returns>
    Public Shared Function DiscretizeFeaturesReduced(features As FeatureScores) As String
        ' ==========================================
        ' 1. 物理特征组: 基因间距离 + 长度比 + Motif (论文Table 3&4显示距离是关键分隔符)
        ' ==========================================
        Dim physSym As String
        If features.IntergenicDistance < 40 Then
            ' 距离很短，极大概率是Operon
            physSym = "P1" ' Physical-Operon
        ElseIf features.IntergenicDistance > 200 Then
            ' 距离很长，极大概率是Boundary
            physSym = "P3" ' Physical-Boundary
        Else
            ' 模糊区 (40-200)，利用长度比和Motif辅助判断
            Dim isBoundary As Boolean = False
            ' 论文发现Boundary对的长度比通常接近0 (长度相近)
            If Math.Abs(features.LengthRatio) < 0.2 Then isBoundary = True
            ' 论文发现TTTTT在Boundary中富集
            Dim tttttScore As Double = If(features.Motifs.ContainsKey("Motif_TTTTT"), features.Motifs("Motif_TTTTT"), 0.0)
            If tttttScore > 1.5 Then isBoundary = True

            physSym = If(isBoundary, "P2b", "P2a") ' Physical-Medium(Boundary倾向) / Physical-Medium(Operon倾向)
        End If

        ' ==========================================
        ' 2. 进化特征组: 邻域保守性 + 系统发育距离 (论文指出这两者信息重叠，见表4)
        ' ==========================================
        Dim evoSym As String
        Dim evoScore As Integer = 0
        ' 发育距离小，得1分
        If features.PhylogeneticDistance <= 10 Then evoScore += 1
        ' 邻域保守性高(得分低)，得1分
        If features.NeighborhoodConservation < 2.0 Then evoScore += 1

        Select Case evoScore
            Case 2 : evoSym = "E1" ' Evolutionary-StrongOperon (共进化强烈)
            Case 1 : evoSym = "E2" ' Evolutionary-Medium
            Case Else : evoSym = "E3" ' Evolutionary-Weak/Boundary
        End Select

        ' ==========================================
        ' 3. 功能特征组: GO相似性 (论文指出GO在U200组贡献了5-7%的精度提升)
        ' ==========================================
        Dim funcSym As String
        If features.GOSimilarity >= 2 Then
            funcSym = "F1" ' Functional-Related
        ElseIf features.GOSimilarity = 1 Then
            funcSym = "F2" ' Functional-Weak
        Else
            funcSym = "F3" ' Functional-Unrelated
        End If

        ' 最终组合：例如 "P1_E1_F1" 表示 物理极似Operon + 进化强烈相关 + 功能相关
        Return $"{physSym}_{evoSym}_{funcSym}"
    End Function

    Public Iterator Function Predict(chromosome As IEnumerable(Of FeatureScores), HMModel As HMM) As IEnumerable(Of ODBOperon)
        ' 将连续特征转换为离散观测序列
        Dim obsSequenceStrings As New List(Of String)
        Dim geneTuple As New List(Of (UpstreamID As String, DownstreamID As String))

        For Each fs As FeatureScores In chromosome
            obsSequenceStrings.Add(DiscretizeFeaturesReduced(fs))
            geneTuple.Add((fs.upstreamID, fs.downstreamID))
        Next

        ' 构建HMM需要的Chain对象
        Dim obSequence As New Chain(Function(a, b) a = b) With {
            .obSequence = obsSequenceStrings.ToArray()
        }

        ' 6. 使用 Viterbi 算法解码最可能的隐状态序列 (核心预测步骤)
        ' 输出将是类似 {"Operon", "Operon", "Boundary", "Operon", "Operon"} 的序列
        Dim viterbiResult As viterbiSequence = HMModel.viterbiAlgorithm(obSequence)
        Dim stats As String() = viterbiResult.stateSequence
        Dim op_members As New List(Of String)
        Dim opId As Integer = 1

        For i As Integer = 0 To stats.Length - 1
            If stats(i) = Operon Then
                ' 如果是新的Operon起点，加入Upstream；否则Upstream已经在列表里了
                If op_members.Count = 0 Then
                    op_members.Add(geneTuple(i).UpstreamID)
                End If
                ' 无论是否是起点，都加入Downstream（如果与上一个的Downstream不同，即防止极短重复）
                If op_members.Last <> geneTuple(i).DownstreamID Then
                    op_members.Add(geneTuple(i).DownstreamID)
                End If
            ElseIf stats(i) = Boundary Then
                ' 遇到边界，输出当前积累的Operon
                If op_members.Count >= 2 Then
                    Yield New ODBOperon With {
                        .koid = "OP_" & opId,
                        .op = op_members.ToArray
                    }
                    opId += 1
                End If
                op_members.Clear()
            End If
        Next

        ' 处理序列末尾剩余的Operon
        If op_members.Count >= 2 Then
            Yield New ODBOperon With {
                .koid = "OP_" & opId,
                .op = op_members.ToArray
            }
        End If
    End Function

    Const Operon As String = NameOf(Operon)
    Const Boundary As String = NameOf(Boundary)

    Public Function TrainModel(chromosomes As IEnumerable(Of IEnumerable(Of FeatureScores))) As HMM
        ' 将连续特征转换为离散观测序列
        Dim obsSequenceStrings As New List(Of String)

        For Each chr As IEnumerable(Of FeatureScores) In chromosomes
            For Each fs As FeatureScores In chr
                Call obsSequenceStrings.Add(DiscretizeFeaturesReduced(fs))
            Next
            ' 可以考虑在这里加入分隔符，如果HMM框架支持多序列训练的话
        Next

        ' 构建HMM需要的Chain对象
        Dim obSequence As New Chain(Function(a, b) a = b) With {
            .obSequence = obsSequenceStrings.ToArray()
        }
        ' 假设使用降维版本，共 4*3*3 = 36 种可能符号
        Dim observables As Observable() = CreateObservables.ToArray
        ' 1. 定义隐状态
        ' 状态1: Operon (属于同一操纵子)
        ' 状态2: Boundary (操纵子边界)
        Dim hiddenStates As StatesObject() = {
            New StatesObject With {.state = Operon, .prob = {0.85, 0.15}}, ' 转移概率: [Operon -> Operon, Operon -> Boundary]
            New StatesObject With {.state = Boundary, .prob = {0.9, 0.1}}  ' 转移概率: [Boundary -> Operon, Boundary -> Boundary]
        }
        ' 4. 构建HMM模型
        Dim HMModel As New HMM(hiddenStates, observables, init:={0.7, 0.3})
        ' 如果状态从 Operon 变为 Boundary，或者从 Boundary 变为 Operon，则存在边界

        ' ========================================================
        ' ' 使用 Baum-Welch 算法进行无监督训练
        ' ========================================================
        ' 基于一整条染色体的基因对观测序列，
        ' 让模型自己学习最优参数
        Dim maximizedModel As HMM = HMModel.baumWelchAlgorithm(obSequence)
        Return maximizedModel
    End Function

    Private Iterator Function CreateObservables() As IEnumerable(Of Observable)
        ' 循环生成所有可能的组合
        Dim pStates = {"P1", "P2a", "P2b", "P3"}
        Dim eStates = {"E1", "E2", "E3"}
        Dim fStates = {"F1", "F2", "F3"}

        Dim rawProbOperon As New List(Of Double)
        Dim rawProbBoundary As New List(Of Double)
        Dim symbols As New List(Of String)

        ' 1. 计算原始权重 (基于生物学先验知识)
        For Each p As String In pStates
            For Each e As String In eStates
                For Each f As String In fStates
                    Call symbols.Add($"{p}_{e}_{f}")

                    ' 给出Operon下的先验权重 (不要求和为1，后面会归一化)
                    Dim weightOp As Double = 1.0 ' 基础权重
                    If p = "P1" Then weightOp *= 5.0 ' 物理距离短，强支持
                    If p = "P3" Then weightOp *= 0.1 ' 物理距离长，强反对
                    If e = "E1" Then weightOp *= 2.0 ' 进化共保守，支持
                    If f = "F1" Then weightOp *= 1.5 ' 功能相关，支持

                    ' 给出Boundary下的先验权重
                    Dim weightBd As Double = 1.0
                    If p = "P3" Then weightBd *= 5.0
                    If p = "P1" Then weightBd *= 0.1
                    If e = "E3" Then weightBd *= 2.0
                    If p = "P2b" Then weightBd *= 1.8 ' 模糊区的Boundary倾向

                    rawProbOperon.Add(weightOp)
                    rawProbBoundary.Add(weightBd)
                Next
            Next
        Next

        ' 2. 归一化，确保概率和严格为 1 (必须步骤，否则HMM数学基础崩溃)
        Dim sumOp = rawProbOperon.Sum()
        Dim sumBd = rawProbBoundary.Sum()

        For i As Integer = 0 To symbols.Count - 1
            Yield New Observable With {
                .obs = symbols(i),
                .prob = {rawProbOperon(i) / sumOp, rawProbBoundary(i) / sumBd}
            }
        Next
    End Function

End Class
