Imports System.Collections.Generic

Namespace Search

    ''' <summary>
    ''' The progress notification of a running topology search.
    ''' (运行中的拓扑搜索的进度通知)
    ''' </summary>
    Public Class SearchProgress

        ''' <summary>The number of topologies that have been evaluated so far.</summary>
        Public Property Completed As Integer

        ''' <summary>The total number of topologies to evaluate.</summary>
        Public Property Total As Integer

        ''' <summary>The identifier of the topology that was evaluated last.</summary>
        Public Property CurrentTopologyId As Integer

        ''' <summary>The number of functional circuits that were found so far.</summary>
        Public Property FunctionalSoFar As Integer

        ''' <summary>The elapsed wall clock time of the search so far.</summary>
        Public Property Elapsed As System.TimeSpan

    End Class

    ''' <summary>
    ''' The report of a finished topology search: the summary statistics, the
    ''' motif Venn counts, the clustering diagnostics, the design table and the
    ''' paths of the written product files.
    ''' (已完成的拓扑搜索的报告：汇总统计、基序 Venn 计数、聚类诊断、设计表与产物文件路径)
    ''' </summary>
    Public Class SearchReport

        ''' <summary>The output directory of the search products.</summary>
        Public Property OutputDir As String = ""

        ''' <summary>The number of topologies that were evaluated in this run.</summary>
        Public Property TopologiesEvaluated As Integer

        ''' <summary>The number of topologies that were reloaded from the checkpoints.</summary>
        Public Property TopologiesResumed As Integer

        ''' <summary>The total number of topologies covered by the report.</summary>
        Public Property TopologiesTotal As Integer

        ''' <summary>The total number of simulated circuits.</summary>
        Public Property CircuitsSimulated As Long

        ''' <summary>The total number of ill behaved circuits.</summary>
        Public Property IllBehavedCircuits As Long

        ''' <summary>The number of topologies with at least one functional circuit.</summary>
        Public Property FunctionalTopologies As Integer

        ''' <summary>The number of over represented topologies, i.e. the topologies above the Q threshold.</summary>
        Public Property OverRepresentedTopologies As Integer

        ''' <summary>
        ''' The motif Venn counts of the over represented functional topologies,
        ''' the population of the necessity claim of the paper.
        ''' (过度代表功能拓扑的基序 Venn 计数，即论文必要性论断的总体)
        ''' </summary>
        Public Property VennOverRepresented As (nfblbOnly As Integer, ifflpOnly As Integer, both As Integer, neither As Integer, total As Integer)

        ''' <summary>
        ''' The motif Venn counts of every topology with at least one functional
        ''' circuit, the broad necessity check.
        ''' (全部至少有一个功能电路的拓扑的基序 Venn 计数，即广义必要性检查)
        ''' </summary>
        Public Property VennFunctional As (nfblbOnly As Integer, ifflpOnly As Integer, both As Integer, neither As Integer, total As Integer)

        ''' <summary>The number of Hamming clusters of the clustered topology set, -1 when the clustering was disabled.</summary>
        Public Property ClusterCount As Integer = -1

        ''' <summary>The silhouette score of the Hamming clustering, NaN when the clustering was disabled.</summary>
        Public Property Silhouette As Double = Double.NaN

        ''' <summary>The rows of the design table experiment.</summary>
        Public Property DesignTable As New List(Of Analysis.RobustnessAnalyzer.DesignTableRow)()

        ''' <summary>The topologies with the highest Q values, at most twenty of them.</summary>
        Public Property TopTopologies As New List(Of Assessment.TopologyResult)()

        ''' <summary>The wall clock time of the search.</summary>
        Public Property Elapsed As System.TimeSpan

        ''' <summary>The artifact name to file path map of the written products.</summary>
        Public Property ArtifactPaths As New Dictionary(Of String, String)()

        ''' <summary>
        ''' The effective number of parallel threads of the search.
        ''' (搜索的实际并行线程数)
        ''' </summary>
        Public Property Parallelism As Integer

        ''' <summary>
        ''' The functional thresholds and the protocol of the search, formatted
        ''' for the report header.
        ''' (搜索的功能阈值与协议，格式化为报告头部)
        ''' </summary>
        Public Property Configuration As String = ""

    End Class

End Namespace
