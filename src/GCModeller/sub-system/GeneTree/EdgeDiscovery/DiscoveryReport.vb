Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 结构发现阶段的汇总结果
''' </summary>
Public Class DiscoveryReport

    ''' <summary>本次训练使用的发现模式</summary>
    Public Property Mode As EdgeDiscoveryMode = EdgeDiscoveryMode.AracneOnly

    ''' <summary>调用方给定的候选基因集合</summary>
    Public Property Candidates As String() = New String() {}

    ''' <summary>实际执行了发现流程的靶基因数量</summary>
    Public Property TargetsExamined As Integer

    ''' <summary>实际参与相关性筛选的候选调控者数量（已剔除常量化、不在矩阵中的基因）</summary>
    Public Property CandidatesScreened As Integer

    ''' <summary>所有被检验过的候选边（含接受与拒绝）</summary>
    Public Property Edges As DiscoveryEdge() = New DiscoveryEdge() {}

    ''' <summary>被接受的新边数量</summary>
    ''' <returns></returns>
    Public ReadOnly Property Accepted As Integer
        Get
            Return Edges.SafeQuery.Count(Function(e) e.isAccepted)
        End Get
    End Property

    ''' <summary>被拒绝的候选数量</summary>
    ''' <returns></returns>
    Public ReadOnly Property Rejected As Integer
        Get
            Return Edges.Length - Accepted
        End Get
    End Property

    ''' <summary>被接受边的 ΔR² 均值</summary>
    ''' <returns></returns>
    Public ReadOnly Property MeanDeltaR2 As Double
        Get
            Dim accepted As DiscoveryEdge() = Edges.SafeQuery.Where(Function(e) e.isAccepted).ToArray

            If accepted.Length = 0 Then
                Return 0.0R
            Else
                Return accepted.Average(Function(e) e.deltaR2)
            End If
        End Get
    End Property

    ''' <summary>
    ''' 把发现结果写成 TSV 表格
    ''' </summary>
    ''' <param name="filePath">输出文件路径</param>
    Public Sub Write(filePath As String)
        Dim folder As String = Path.GetDirectoryName(filePath)

        If Not String.IsNullOrEmpty(folder) AndAlso Not Directory.Exists(folder) Then
            Call Directory.CreateDirectory(folder)
        End If

        Using writer As New StreamWriter(filePath, False, Encoding.UTF8)
            Call WriteTo(writer)
        End Using
    End Sub

    ''' <summary>
    ''' 把发现结果写到一个文本流（存档与外部文件共用同一份格式）
    ''' </summary>
    ''' <param name="writer">目标写入器</param>
    Public Sub WriteTo(writer As TextWriter)
        Call writer.WriteLine("# GeneTree residual two-stage edge discovery")
        Call writer.WriteLine($"# mode: {Mode}")
        Call writer.WriteLine($"# candidates: {Candidates.Length}, screened: {CandidatesScreened}, targets: {TargetsExamined}")
        Call writer.WriteLine($"# accepted: {Accepted}, rejected: {Rejected}, mean_delta_r2: {MeanDeltaR2.ToString("G4")}")
        Call writer.WriteLine("gene,regulator,source,status,correlation,delta_r2,r2_before,r2_after,splits,sensitivity,reason")

        For Each edge As DiscoveryEdge In Edges.SafeQuery
            Call writer.WriteLine(String.Join(",",
                csv(edge.target),
                csv(edge.regulator),
                "discovered",
                csv(edge.status),
                edge.correlation.ToString("G6"),
                edge.deltaR2.ToString("G6"),
                edge.r2Before.ToString("G6"),
                edge.r2After.ToString("G6"),
                CStr(edge.splits),
                edge.sensitivity.ToString("G6"),
                csv(If(edge.reason, ""))))
        Next
    End Sub

    Private Shared Function csv(text As String) As String
        If text Is Nothing Then
            Return ""
        ElseIf text.IndexOf(","c) >= 0 OrElse text.IndexOf(""""c) >= 0 Then
            Return """" & text.Replace("""", """""") & """"
        Else
            Return text
        End If
    End Function

    Public Overrides Function ToString() As String
        Return $"discovery={Mode}, candidates={Candidates.Length}, screened={CandidatesScreened}, " &
               $"targets={TargetsExamined}, accepted={Accepted}, rejected={Rejected}, " &
               $"mean dR2={MeanDeltaR2.ToString("G4")}"
    End Function
End Class
