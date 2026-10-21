Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Namespace GRN

    ''' <summary>
    ''' A directed regulatory interaction edge of the gene regulatory network.
    ''' (基因调控网络之中的一条有向调控边)
    ''' </summary>
    ''' <remarks>
    ''' 边的权重取净调控弹性 g_ij - h_ij：为正表示源基因激活靶基因，
    ''' 为负表示源基因抑制靶基因，零表示不存在调控关系。
    ''' </remarks>
    Public Class RegulatoryEdge

        ''' <summary>
        ''' 调控因子的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Property source As String
        ''' <summary>
        ''' 靶基因的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Property target As String
        ''' <summary>
        ''' 净调控弹性 g_ij - h_ij
        ''' </summary>
        ''' <returns></returns>
        Public Property weight As Double
        ''' <summary>
        ''' 合成项之中的动力学阶 g_ij
        ''' </summary>
        ''' <returns></returns>
        Public Property g As Double
        ''' <summary>
        ''' 降解项之中的动力学阶 h_ij
        ''' </summary>
        ''' <returns></returns>
        Public Property h As Double

        ''' <summary>
        ''' 调控的类型：激活或者抑制
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property interaction As String
            Get
                If weight > 0 Then
                    Return "activates"
                ElseIf weight < 0 Then
                    Return "inhibits"
                Else
                    Return "none"
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{source} {interaction} {target} (net={weight.ToString("G4")})"
        End Function
    End Class

    ''' <summary>
    ''' The network topology inference of the fitted S-system parameters.
    ''' (由拟合参数反推基因调控网络的拓扑结构)
    ''' </summary>
    ''' <remarks>
    ''' 参看 method.md 的步骤3：基因之间的真实相互作用是稀疏的，
    ''' 故这里通过阈值截断与每基因限边数来恢复网络的拓扑。
    ''' </remarks>
    Public Module NetworkTopology

        ''' <summary>
        ''' 对拟合参数施加稀疏化约束，并返回恢复出来的有向调控边集合
        ''' </summary>
        ''' <param name="parameters">拟合得到的参数集合，稀疏化之后的结果会被回写到该对象之中</param>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Public Function Sparsify(parameters As SSystemParameters, Optional options As FittingOptions = Nothing) As RegulatoryEdge()
            If parameters Is Nothing Then
                Throw New ArgumentNullException(NameOf(parameters))
            End If
            If options Is Nothing Then
                options = New FittingOptions()
            End If

            Dim n As Integer = parameters.nGenes
            Dim net As Double()() = parameters.NetElasticity()
            Dim keep As Boolean()() = New Boolean(n - 1)() {}
            Dim edges As New System.Collections.Generic.List(Of RegulatoryEdge)

            For i As Integer = 0 To n - 1
                keep(i) = New Boolean(n - 1) {}
            Next

            For i As Integer = 0 To n - 1
                Dim candidates As New System.Collections.Generic.List(Of Integer)

                For j As Integer = 0 To n - 1
                    If j = i AndAlso Not options.KeepSelfLoop Then
                        Continue For
                    End If
                    If std.Abs(net(i)(j)) < options.Sparsity Then
                        Continue For
                    End If

                    Call candidates.Add(j)
                Next

                ' 每基因限边数：按照净调控弹性的绝对值大小保留最强的若干条边
                If options.MaxEdgesPerGene > 0 AndAlso candidates.Count > options.MaxEdgesPerGene Then
                    Dim drop As Integer() = candidates _
                        .OrderBy(Function(j) std.Abs(net(i)(j))) _
                        .Take(candidates.Count - options.MaxEdgesPerGene) _
                        .ToArray

                    For Each j As Integer In drop
                        Call candidates.Remove(j)
                    Next
                End If

                For Each j As Integer In candidates
                    keep(i)(j) = True

                    Call edges.Add(New RegulatoryEdge With {
                        .source = parameters.GeneNames(j),
                        .target = parameters.GeneNames(i),
                        .g = parameters.G(i)(j),
                        .h = parameters.H(i)(j),
                        .weight = net(i)(j)
                    })
                Next
            Next

            ' 把没有保留的交互置零，使得后续的模型生成只包含稀疏化之后的边
            For i As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    If Not keep(i)(j) Then
                        parameters.G(i)(j) = 0
                        parameters.H(i)(j) = 0
                    End If
                Next
            Next

            Return edges.ToArray
        End Function

        ''' <summary>
        ''' 网络的稳定性诊断：稳态要求对数增益矩阵 E = G - H 的特征值实部全为负数。
        ''' 这里给出 Gershgorin 圆盘定理所推出的最大特征值实部上界。
        ''' </summary>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function StabilityDiagnosis(parameters As SSystemParameters) As Double
            Dim net As Double()() = parameters.NetElasticity()
            Dim n As Integer = net.Length
            Dim bound As Double = Double.NegativeInfinity

            For i As Integer = 0 To n - 1
                Dim radius As Double = 0

                For j As Integer = 0 To n - 1
                    If j <> i Then
                        radius += std.Abs(net(i)(j))
                    End If
                Next

                bound = std.Max(bound, net(i)(i) + radius)
            Next

            Return bound
        End Function

        ''' <summary>
        ''' 生成网络的稳定性诊断报告
        ''' </summary>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function StabilityReport(parameters As SSystemParameters) As String
            Dim bound As Double = StabilityDiagnosis(parameters)

            If bound < 0 Then
                Return $"The network is stable: the Gershgorin upper bound of the maximal eigenvalue real part is {bound.ToString("G4")} (< 0)."
            Else
                Return $"The network may be unstable: the Gershgorin upper bound of the maximal eigenvalue real part is {bound.ToString("G4")} (>= 0)."
            End If
        End Function

        ''' <summary>
        ''' 生成 Cytoscape 可以直接导入的 SIF 文本
        ''' </summary>
        ''' <param name="edges"></param>
        ''' <returns></returns>
        Public Function ToSIF(edges As IReadOnlyCollection(Of RegulatoryEdge)) As String
            If edges.IsNullOrEmpty Then
                Return ""
            End If

            Return edges _
                .Select(Function(e) $"{e.source}{vbTab}{e.interaction}{vbTab}{e.target}") _
                .JoinBy(vbLf)
        End Function
    End Module
End Namespace
