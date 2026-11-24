Namespace Assessment

    ''' <summary>
    ''' The aggregated search result of one topology: its robustness Q value,
    ''' the well behavedness statistics of its circuits and the motif and
    ''' cluster assignments that the post processing adds.
    ''' (单个拓扑的聚合搜索结果：鲁棒性 Q 值、电路良态统计与后处理添加的基序和聚类归属)
    ''' </summary>
    Public Class TopologyResult

        ''' <summary>The identifier of the topology, its base 3 encoded sign vector.</summary>
        Public Property TopologyId As Integer

        ''' <summary>The sign vector of the topology as a symbol string.</summary>
        Public Property Signs As String

        ''' <summary>The number of active links of the topology.</summary>
        Public Property LinkCount As Integer

        ''' <summary>The number of simulated circuits of the topology.</summary>
        Public Property Simulated As Integer

        ''' <summary>The number of ill behaved circuits of the topology.</summary>
        Public Property IllBehaved As Integer

        ''' <summary>
        ''' The robustness Q value: the number of functional circuits, i.e. the
        ''' sampled parameter sets that land inside of the functional region.
        ''' (鲁棒性 Q 值：功能电路数量，即落入功能区的采样参数组数量)
        ''' </summary>
        Public Property Functional As Integer

        ''' <summary>The mean sensitivity of the functional circuits.</summary>
        Public Property MeanSensitivity As Double

        ''' <summary>The maximum sensitivity of the functional circuits.</summary>
        Public Property MaxSensitivity As Double

        ''' <summary>The mean precision of the functional circuits.</summary>
        Public Property MeanPrecision As Double

        ''' <summary>The mean steady state output O1 of the functional circuits.</summary>
        Public Property MeanO1 As Double

        ''' <summary>
        ''' Whether the topology owns a negative feedback loop with a buffering
        ''' node, assigned by the motif analyzer.
        ''' (拓扑是否含有带缓冲节点的负反馈回路 NFBLB，由基序分析器赋值)
        ''' </summary>
        Public Property HasNFBLB As Boolean

        ''' <summary>
        ''' Whether the topology owns an incoherent feedforward loop with a
        ''' proportioner node, assigned by the motif analyzer.
        ''' (拓扑是否含有带比例器节点的非相干前馈回路 IFFLP，由基序分析器赋值)
        ''' </summary>
        Public Property HasIFFLP As Boolean

        ''' <summary>
        ''' The Hamming cluster label of the topology, -1 when the topology was
        ''' not clustered.
        ''' (拓扑的 Hamming 聚类标签，未聚类时为 -1)
        ''' </summary>
        Public Property ClusterLabel As Integer = -1

        ''' <summary>
        ''' Whether the topology is over represented inside of the functional
        ''' region, i.e. whether its Q value exceeds the threshold.
        ''' (拓扑是否在功能区被过度代表，即 Q 值是否超过门槛)
        ''' </summary>
        ''' <param name="qThreshold">the Q threshold of the over representation</param>
        Public Function IsOverRepresented(qThreshold As Integer) As Boolean
            Return Functional > qThreshold
        End Function

        ''' <summary>
        ''' The Q value and the motif flags as a compact string.
        ''' </summary>
        Public Overrides Function ToString() As String
            Return $"[{TopologyId}] {Signs} links={LinkCount} Q={Functional} nfblb={HasNFBLB} ifflp={HasIFFLP}"
        End Function

    End Class

End Namespace
