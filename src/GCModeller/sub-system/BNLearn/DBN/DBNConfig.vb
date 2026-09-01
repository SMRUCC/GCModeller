#Region "Microsoft.VisualBasic::b71a9665585a6b34c4ba22cf95799ee7, sub-system\BNLearn\DBN\DBNConfig.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 52
    '    Code Lines: 13 (25.00%)
    ' Comment Lines: 24 (46.15%)
    '    - Xml Docs: 95.83%
    ' 
    '   Blank Lines: 15 (28.85%)
    '     File Size: 2.19 KB


    '     Class DBNConfig
    ' 
    '         Properties: BasalTranscriptionRate, HighThreshold, HighTranscriptionRate, LowThreshold, LowTranscriptionRate
    '                     OnlineLearningRate, Seed, SmoothingAlpha, UseMultinomialSampling
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace DBN


    ' ==================== DBN Configuration ====================

    ''' <summary>
    ''' Configuration options for the Dynamic Bayesian Network.
    ''' Controls discretization thresholds, smoothing, transcription rate mapping, etc.
    ''' </summary>
    Public Class DBNConfig

        ''' <summary>
        ''' Smoothing parameter for parameter learning (Dirichlet prior concentration).
        ''' Larger values give more weight to the topology-based prior.
        ''' When alpha = 0: pure Maximum Likelihood Estimation (data only).
        ''' When alpha is large: prior dominates (topology only).
        ''' Default = 1.0 (Laplace smoothing with topology prior).
        ''' </summary>
        Public Property SmoothingAlpha As Double = 1.0

        ''' <summary>
        ''' If true, sample from the probability distribution (stochastic prediction).
        ''' If false, take the most likely state (deterministic, argmax).
        ''' Default = false (deterministic).
        ''' </summary>
        Public Property UseMultinomialSampling As Boolean = False

        ''' <summary>Lower threshold for discretization (values below this = "Low")</summary>
        Public Property LowThreshold As Double = 0.33

        ''' <summary>Upper threshold for discretization (values above this = "High")</summary>
        Public Property HighThreshold As Double = 0.66

        ''' <summary>Transcription rate for "High" expression state</summary>
        Public Property HighTranscriptionRate As Double = 1.0

        ''' <summary>Transcription rate for "Medium" expression state (basal)</summary>
        Public Property BasalTranscriptionRate As Double = 0.5

        ''' <summary>Transcription rate for "Low" expression state</summary>
        Public Property LowTranscriptionRate As Double = 0.0

        ''' <summary>Random seed for reproducible stochastic sampling</summary>
        Public Property Seed As Integer = 42

        ''' <summary>Learning rate for online parameter updates (exponential moving average)</summary>
        Public Property OnlineLearningRate As Double = 0.1

        ''' <summary>
        ''' 单个节点允许的最大父节点数（拓扑构建阶段的兜底保护）。
        ''' 
        ''' CPT 的行数为各父节点状态数之积（默认 3 态即 3^P），父节点数不受限时
        ''' 规模会指数爆炸（实测模块内 hub 基因的父节点数可达 39）。
        ''' 
        ''' 默认值 8 的另一层意义是**模型判别力**：激活得分用 noisy-OR 组合，
        ''' 当父节点数为 P 时，单个父节点从 Medium 变为 Low 只会让
        ''' activationScore 从 1-0.5^P 变为 1-0.5^(P-1)，P 越大变化越小
        ''' （P=39 时差异约 1e-12），单基因敲降将完全无法产生可观测的响应。
        ''' 取 8 既符合转录调控 fan-in 的一般规模，也让每个父节点保持足够影响权重。
        ''' </summary>
        Public Property MaxParents As Integer = 8

        ''' <summary>
        ''' 单个节点 CPT 允许"完整展开"的最大行数。
        ''' 
        ''' 不超过该阈值（默认 10000，即 3 态下 P&lt;=8）时按原逻辑展开全表；
        ''' 超过时改为按需计算（惰性 CPT，见 ConditionalProbabilityTable.OnDemandProvider）：
        ''' 拓扑先验分布本身就是父状态的纯函数，查询时现场计算的结果与全表展开完全一致，
        ''' 但内存占用从 O(3^P) 降为 O(实际访问过的配置数)。
        ''' </summary>
        Public Property MaxCPTRows As Integer = 10000

        ''' <summary>
        ''' 惰性 CPT 的记忆化缓存上限：单个节点最多缓存多少个"现场计算过"的父配置。
        ''' 超过后不再写入缓存，每次查询直接计算，保证稀疏缓存不会无限增长
        ''' （父节点多时每个 key 字符串可达数百字节，缓存条目数需要设上限）。
        ''' </summary>
        Public Property MaxCPTCacheRows As Integer = 2000

        ''' <summary>
        ''' 计算惰性节点的边缘分布时使用的蒙特卡洛采样数（配置空间过大时无法枚举）。
        ''' </summary>
        Public Property MarginalSampleSize As Integer = 4096

        ''' <summary>
        ''' 每个节点（基因）的离散化阈值：key = 节点 ID，value = (low_threshold, high_threshold)。
        ''' 
        ''' 默认阈值 <see cref="LowThreshold"/> / <see cref="HighThreshold"/>（0.33 / 0.66）是按
        ''' "已归一化到 [0,1] 的数据"设计的；而时间序列常常是原始 log1p 表达值（量级 0~10+），
        ''' 此时几乎所有基因都会被判为 High，导致学习到的 CPT 与推理证据都严重偏向 High。
        ''' 
        ''' 由训练流程按数据的经验分位数填好该字典后，参数学习（LearnParameters）与
        ''' 推理（PredictNextState）都会经 GetThresholds 命中这里，保证两侧使用同一套阈值。
        ''' </summary>
        Public Property NodeThresholds As New Dictionary(Of String, Tuple(Of Double, Double))

        ''' <summary>
        ''' 激活得分判定为"高表达"的下限（默认 0.58）。
        ''' 
        ''' 得分由 noisy-OR 组合得到，父节点数为 P 时单个父节点由 Medium 变 Low
        ''' 只会让得分变动约 0.5^P（P=8 时约 0.06）。若沿用较宽的区间（0.34/0.66，
        ''' 区间宽度 0.32），单基因敲降带来的得分变化远不足以跨越区间，
        ''' 所有基因都会被钉死在 Medium，虚拟扰动也就观测不到任何响应。
        ''' 默认取 0.42/0.58（区间宽度 0.16），使单基因扰动能够跨越状态边界。
        ''' </summary>
        Public Property ActivationHighThreshold As Double = 0.53

        ''' <summary>
        ''' 激活得分判定为"低表达"的上限（默认 0.42），见 <see cref="ActivationHighThreshold"/>。
        ''' </summary>
        Public Property ActivationLowThreshold As Double = 0.47

        ''' <summary>
        ''' 级联传播触发下游模块整体状态偏置的阈值（默认 0.02）。
        ''' 
        ''' 传播量是"模块内基因状态相对基线的平均变化"（状态值 Low=0/Medium=1/High=2）。
        ''' 单基因敲降通常只影响其直接靶标（约占模块基因的 1%），对应量级约 0.01；
        ''' 沿用原实现的 0.1 会让级联永远不触发，扰动效应被完全局限在本模块内。
        ''' </summary>
        Public Property CascadeStateThreshold As Double = 0.02

        ''' <summary>
        ''' 自适应阈值所使用的低分位数（默认 0.33，即约 1/3 的样本落入 Low）。
        ''' </summary>
        Public Property QuantileLow As Double = 0.33

        ''' <summary>
        ''' 自适应阈值所使用的高分位数（默认 0.66，即约 1/3 的样本落入 High）。
        ''' </summary>
        Public Property QuantileHigh As Double = 0.66

    End Class


End Namespace
