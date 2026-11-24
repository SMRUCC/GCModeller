Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Namespace GRN

    ''' <summary>
    ''' The fitted parameter set of the S-system model.
    ''' (拟合得到的S-system参数集合：alpha / beta / G / H)
    ''' </summary>
    ''' <remarks>
    ''' 其中 <see cref="G"/>(i)(j) 为合成项之中基因 j 对基因 i 的调控弹性 g_ij，
    ''' <see cref="H"/>(i)(j) 为降解项之中的 h_ij；净调控弹性取 g_ij - h_ij。
    ''' </remarks>
    Public Class SSystemParameters

        ''' <summary>
        ''' 规整化之后的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Property GeneNames As String()
        ''' <summary>
        ''' 原始的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalNames As String()

        ''' <summary>
        ''' 每一个基因的合成速率常数 alpha
        ''' </summary>
        ''' <returns></returns>
        Public Property alpha As Double()
        ''' <summary>
        ''' 每一个基因的降解速率常数 beta
        ''' </summary>
        ''' <returns></returns>
        Public Property beta As Double()
        ''' <summary>
        ''' 合成项的动力学阶矩阵，第一维是靶基因、第二维是调控因子
        ''' </summary>
        ''' <returns></returns>
        Public Property G As Double()()
        ''' <summary>
        ''' 降解项的动力学阶矩阵，第一维是靶基因、第二维是调控因子
        ''' </summary>
        ''' <returns></returns>
        Public Property H As Double()()

        ''' <summary>
        ''' 每一个基因的斜率拟合残差平方和
        ''' </summary>
        ''' <returns></returns>
        Public Property SlopeSSE As Double()
        ''' <summary>
        ''' 每一个基因的斜率拟合均方根误差
        ''' </summary>
        ''' <returns></returns>
        Public Property SlopeRMSE As Double()
        ''' <summary>
        ''' 每一个基因的观测斜率的总平方和（用于计算拟合的 R²）
        ''' </summary>
        ''' <returns></returns>
        Public Property SlopeSST As Double()
        ''' <summary>
        ''' 每一个基因的候选调控因子（矩阵行号）；为空表示该基因使用了全部基因作为候选
        ''' </summary>
        ''' <returns></returns>
        Public Property Candidates As Integer()()
        ''' <summary>
        ''' 每一个基因的参数求解是否收敛
        ''' </summary>
        ''' <returns></returns>
        Public Property Converged As Boolean()
        ''' <summary>
        ''' 是否执行过第二阶段的轨迹精修
        ''' </summary>
        ''' <returns></returns>
        Public Property TrajectoryRefined As Boolean = False

        ''' <summary>
        ''' 基因的数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property nGenes As Integer
            Get
                If GeneNames Is Nothing Then Return 0 Else Return GeneNames.Length
            End Get
        End Property

        ''' <summary>
        ''' 全部基因的平均拟合均方根误差
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MeanRMSE As Double
            Get
                If SlopeRMSE.IsNullOrEmpty Then Return 0 Else Return SlopeRMSE.Average
            End Get
        End Property

        ''' <summary>
        ''' 第 <paramref name="i"/> 个基因斜率拟合的决定系数 R²（残差平方和 / 总平方和）
        ''' </summary>
        ''' <param name="i"></param>
        ''' <returns></returns>
        Public Function RSquared(i As Integer) As Double
            Dim sst As Double = 0
            Dim sse As Double = 0

            If Not SlopeSST.IsNullOrEmpty AndAlso i < SlopeSST.Length Then
                sst = SlopeSST(i)
            End If
            If Not SlopeSSE.IsNullOrEmpty AndAlso i < SlopeSSE.Length Then
                sse = SlopeSSE(i)
            End If

            If sst <= 1.0E-12 Then
                Return 0
            End If

            Dim r2 As Double = 1 - sse / sst

            If Double.IsNaN(r2) OrElse Double.IsInfinity(r2) Then
                Return 0
            End If

            Return r2
        End Function

        ''' <summary>
        ''' 第 <paramref name="i"/> 个基因的候选调控因子的矩阵行号；没有记录的时候返回全部基因
        ''' </summary>
        ''' <param name="i"></param>
        ''' <returns></returns>
        Public Function CandidateOf(i As Integer) As Integer()
            If Not Candidates.IsNullOrEmpty AndAlso i < Candidates.Length AndAlso Not Candidates(i) Is Nothing Then
                Return Candidates(i)
            End If

            Return Enumerable.Range(0, nGenes).ToArray
        End Function

        ''' <summary>
        ''' 计算净调控弹性矩阵 E = G - H
        ''' </summary>
        ''' <returns></returns>
        Public Function NetElasticity() As Double()()
            Dim n As Integer = nGenes
            Dim E As Double()() = New Double(n - 1)() {}

            For i As Integer = 0 To n - 1
                E(i) = New Double(n - 1) {}

                For j As Integer = 0 To n - 1
                    E(i)(j) = G(i)(j) - H(i)(j)
                Next
            Next

            Return E
        End Function

        ''' <summary>
        ''' 深拷贝当前的参数集合
        ''' </summary>
        ''' <returns></returns>
        Public Function Clone() As SSystemParameters
            Return New SSystemParameters With {
                .GeneNames = CType(GeneNames.Clone(), String()),
                .OriginalNames = CType(OriginalNames.Clone(), String()),
                .alpha = CType(alpha.Clone(), Double()),
                .beta = CType(beta.Clone(), Double()),
                .G = G.Select(Function(row) CType(row.Clone(), Double())).ToArray,
                .H = H.Select(Function(row) CType(row.Clone(), Double())).ToArray,
                .SlopeSSE = CType(SlopeSSE.Clone(), Double()),
                .SlopeRMSE = CType(SlopeRMSE.Clone(), Double()),
                .SlopeSST = If(SlopeSST Is Nothing, Nothing, CType(SlopeSST.Clone(), Double())),
                .Candidates = If(Candidates Is Nothing, Nothing, Candidates.Select(Function(row) CType(row.Clone(), Integer())).ToArray),
                .Converged = CType(Converged.Clone(), Boolean()),
                .TrajectoryRefined = TrajectoryRefined
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"S-system [{nGenes} genes], mean slope RMSE = {MeanRMSE.ToString("G6")}"
        End Function

        ''' <summary>
        ''' 生成一个人类可读的参数摘要
        ''' </summary>
        ''' <returns></returns>
        Public Function Summary() As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine($"S-system parameters: {nGenes} genes")
            Call sb.AppendLine($"mean slope RMSE    : {MeanRMSE.ToString("G6")}")
            Call sb.AppendLine()

            For i As Integer = 0 To nGenes - 1
                Call sb.AppendLine($"  {GeneNames(i)}' = {alpha(i).ToString("G4")}*[prod] - {beta(i).ToString("G4")}*[prod]")

                For j As Integer = 0 To nGenes - 1
                    Dim e As Double = G(i)(j) - H(i)(j)

                    If std.Abs(e) > 0 Then
                        Call sb.AppendLine($"      <- {GeneNames(j)}  g={G(i)(j).ToString("G4")}, h={H(i)(j).ToString("G4")}, net={e.ToString("G4")}")
                    End If
                Next
            Next

            Return sb.ToString
        End Function
    End Class
End Namespace
