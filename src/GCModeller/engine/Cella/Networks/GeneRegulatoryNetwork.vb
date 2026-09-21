' ============================================================
' GeneRegulatoryNetwork.vb - 基因转录调控网络
' ============================================================
' 采用 GEARS 图神经网络建模：
'
'   1. 以「转录因子 TF → 靶基因」的调控关系作为先验知识，直接充当 GNN 的
'      图结构（GeneRegulatoryGraph 把每条先验边建成一条带关系类型的有向边，
'      Activation / Repression 决定消息符号）。
'   2. 用内置仿真器合成伪 Perturb-seq 样本完成训练；也支持注入实测样本。
'   3. 仿真循环里调用 Model.PredictDelta(controlExpr, pertFlag) 做**连续**
'      推理，而不是 GEARS.Predict 的离散敲除接口 —— 因为细胞里转录因子的
'      活性是信号转导通路的连续输出。
'
' 连续推理的编码约定（与训练分布对齐）：
'   训练时 flag=1 总是伴随着一个大幅偏离的 z-score（敲除 z≈-2、过表达 z≈+3）。
'   因此把 TF 活性 a∈[0,1] 映射为 z = (a-0.5)*4 ∈ [-2,+2]，
'   flag = min(1, |z|/2)。a=0.5 即野生型（z=0, flag=0）。
' ============================================================

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.GEARS

''' <summary>
''' 采用 GEARS 图神经网络所构建的基因表达调控网络系统
''' </summary>
Public Class GeneRegulatoryNetwork : Inherits SubNetwork

    ''' <summary>底层 GEARS 模型；推理无副作用，可跨细胞共享</summary>
    Public ReadOnly Property Gears As GEARS
        Get
            Return _gears
        End Get
    End Property

    Private _gears As GEARS

    ReadOnly geneNames As String()
    ReadOnly n As Integer
    Private wildtypeMean As Double()
    Private wildtypeSD As Double()

    ''' <summary>转录因子基因索引 → 信号通道索引</summary>
    ReadOnly tfSignalLink As Couple()
    ''' <summary>代谢物效应物索引 → 受其调控的转录因子基因索引</summary>
    ReadOnly effectorLink As Couple()

    ReadOnly relaxationTau As Double

    ''' <summary>训练损失曲线</summary>
    Public ReadOnly Property LossCurve As Double()
        Get
            Return If(_gears Is Nothing, Nothing, _gears.LossCurve)
        End Get
    End Property

    ''' <summary>图结构中实际生效的先验边数（可用于校验先验网络是否被正确映射）</summary>
    Public ReadOnly Property NumPriorEdges As Integer
        Get
            Return If(_gears Is Nothing, 0, _gears.GraphData.NumPriorEdges)
        End Get
    End Property

    Private Structure Couple
        Public src As Integer
        Public dst As Integer
    End Structure

    Sub New(cell As VirtualCella, blueprint As CellaBlueprint, Optional gearsModel As GEARS = Nothing)
        Call MyBase.New(cell, NameOf(GeneRegulatoryNetwork))

        geneNames = blueprint.Genes
        n = geneNames.Length
        relaxationTau = blueprint.RnaRelaxationTau

        If blueprint.Expression IsNot Nothing Then
            _gears = If(gearsModel, New GEARS(blueprint.Expression, blueprint.Prior, blueprint.GearsConfig))
        Else
            _gears = gearsModel
        End If

        If _gears Is Nothing Then
            wildtypeMean = New Double(n - 1) {}
            wildtypeSD = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                wildtypeSD(i) = 1.0
            Next
        Else
            wildtypeMean = _gears.WildtypeMeans
            wildtypeSD = _gears.WildtypeSDs

            ' 回填到蓝图，后续细胞可以复用同一个已训练模型
            blueprint.Gears = _gears
        End If

        tfSignalLink = BuildTfSignalLink(blueprint)
        effectorLink = BuildEffectorLink(blueprint)
    End Sub

    ''' <summary>TF 基因 ← 信号通道</summary>
    Private Function BuildTfSignalLink(blueprint As CellaBlueprint) As Couple()
        Dim state As CellularState = cell.State
        Dim links As New List(Of Couple)

        For Each tf As String In blueprint.GetTFGenes().SafeQuery
            Dim geneIdx As Integer = -1
            Dim signalIdx As Integer = -1

            If state.GeneIndex.TryGetValue(tf, geneIdx) AndAlso state.SignalIndex.TryGetValue(tf, signalIdx) Then
                links.Add(New Couple With {.src = signalIdx, .dst = geneIdx})
            End If
        Next

        Return links.ToArray()
    End Function

    ''' <summary>代谢物效应物 → TF 基因</summary>
    Private Function BuildEffectorLink(blueprint As CellaBlueprint) As Couple()
        Dim state As CellularState = cell.State
        Dim links As New List(Of Couple)

        For Each pair In blueprint.Effectors.SafeQuery
            Dim metaboliteIdx As Integer = -1
            Dim geneIdx As Integer = -1

            If state.MetaboliteIndex.TryGetValue(pair.Key, metaboliteIdx) AndAlso
                state.GeneIndex.TryGetValue(pair.Value, geneIdx) Then

                links.Add(New Couple With {.src = metaboliteIdx, .dst = geneIdx})
            End If
        Next

        Return links.ToArray()
    End Function

    ' ==================== 训练 ====================

    ''' <summary>
    ''' 用内置仿真器合成伪 Perturb-seq 样本并训练 GNN
    ''' </summary>
    Public Function Train(Optional epochs As Integer = -1) As Double()
        If _gears Is Nothing Then
            Return {}
        End If

        If epochs > 0 AndAlso _gears.Options IsNot Nothing Then
            _gears.Options.Epochs = epochs
        End If

        If _gears.TrainingSamples.IsNullOrEmpty Then
            Call _gears.GenerateTrainingSamples()
        End If

        Dim curve As Double() = _gears.Train()

        ' 训练后基线统计量可能被重算，同步一次缓存
        wildtypeMean = _gears.WildtypeMeans
        wildtypeSD = _gears.WildtypeSDs

        Return curve
    End Function

    ' ==================== 推进 ====================

    Public Overrides Sub Tick(dt As Double)
        If _gears Is Nothing OrElse n = 0 Then
            Return
        End If

        Dim state As CellularState = cell.State
        Dim mrna As Double() = state.mRNA
        Dim signal As Double() = state.Signal
        Dim metabolite As Double() = state.Metabolite
        Dim xNorm As Double() = New Double(n - 1) {}
        Dim flag As Double() = New Double(n - 1) {}

        ' ---- 1. 输入侧：当前表达谱的 z-score ----
        For i As Integer = 0 To n - 1
            Dim sd As Double = System.Math.Max(wildtypeSD(i), 0.000001)

            xNorm(i) = (mrna(i) - wildtypeMean(i)) / sd
        Next

        ' ---- 2. 转录因子活性 → 扰动标记（与训练分布对齐的编码）----
        For Each link As Couple In tfSignalLink
            Dim activity As Double = signal(link.src)

            If Double.IsNaN(activity) Then
                activity = 0.5
            End If

            activity = Clamp(activity, 0.0, 1.0)

            Dim z As Double = (activity - 0.5) * 4.0

            xNorm(link.dst) = z
            flag(link.dst) = System.Math.Max(flag(link.dst), System.Math.Min(1.0, System.Math.Abs(z) / 2.0))
        Next

        ' ---- 3. 代谢物效应物 → 转录因子活性 ----
        For Each link As Couple In effectorLink
            Dim level As Double = metabolite(link.src)
            Dim activity As Double = 1.0 / (1.0 + System.Math.Exp(-level))
            Dim z As Double = (activity - 0.5) * 4.0

            flag(link.dst) = System.Math.Max(flag(link.dst), System.Math.Min(1.0, System.Math.Abs(z) / 2.0))
        Next

        ' ---- 4. GNN 推理 + 松弛到预测目标 ----
        Dim deltaNorm As Double()
        Dim ok As Boolean = True

        Try
            deltaNorm = _gears.Model.PredictDelta(xNorm, flag)
        Catch ex As Exception
            ' 单次推理失败不应该让整个培养体系崩溃：跳过本步即可
            Call CountFailure()
            ok = False
            deltaNorm = Nothing
        End Try

        If Not ok OrElse deltaNorm Is Nothing Then
            Return
        End If

        Dim alpha As Double = 1.0 - System.Math.Exp(-dt / System.Math.Max(relaxationTau, 0.000001))
        Dim clipped As Boolean = False

        For i As Integer = 0 To n - 1
            Dim sd As Double = System.Math.Max(wildtypeSD(i), 0.000001)
            Dim delta As Double = deltaNorm(i) * sd

            ' 数值安全：单次预测的偏移量不超过 3 倍标准差
            If delta > 3.0 * sd Then
                delta = 3.0 * sd
                clipped = True
            ElseIf delta < -3.0 * sd Then
                delta = -3.0 * sd
                clipped = True
            End If

            ' GEARS 的还原约定是「扰动后表达 = 基线 + Δ」，因此预测目标应当以
            ' 野生型基线为锚点，而不是在当前值上继续累加 —— 后者会让闭环仿真
            ' 单调漂移（实测会一路衰减到 0）。这里让 mRNA 松弛到预测目标。
            Dim target As Double = wildtypeMean(i) + delta

            If target < 0.0 Then
                target = 0.0
            End If

            mrna(i) = mrna(i) + (target - mrna(i)) * alpha
        Next

        If clipped Then
            Call CountFailure()
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Shared Function Clamp(x As Double, lo As Double, hi As Double) As Double
        If Double.IsNaN(x) Then Return lo
        If x < lo Then Return lo
        If x > hi Then Return hi
        Return x
    End Function

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Dim state As CellularState = cell.State
        Dim mrna As Double() = state.mRNA
        Dim sum As Double = 0.0
        Dim max As Double = 0.0

        For i As Integer = 0 To mrna.Length - 1
            sum += mrna(i)

            If mrna(i) > max Then
                max = mrna(i)
            End If
        Next

        Dim stats As New Dictionary(Of String, Double) From {
            {"genes", n},
            {"prior_edges", NumPriorEdges},
            {"mrna_total", sum},
            {"mrna_max", max}
        }

        If Not LossCurve.IsNullOrEmpty Then
            stats("loss_first") = LossCurve(0)
            stats("loss_last") = LossCurve(LossCurve.Length - 1)
        End If

        Return stats
    End Function

End Class
