Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' 一次代谢网络模拟的完整输出：代谢物浓度轨迹、逐反应通量轨迹与液态时间常数轨迹
''' </summary>
''' <remarks>
''' <list type="bullet">
''' <item><description><see cref="Concentrations"/>：形状 (T × m)</description></item>
''' <item><description><see cref="Fluxes"/>：形状 (T × r)</description></item>
''' <item><description><see cref="Tau"/>：形状 (T × m)，即 τ^sys 轨迹，是 LNN 的可解释性输出</description></item>
''' </list>
''' </remarks>
Public Class MetabolicTrajectory

    ''' <summary>时间网格（可不规则）</summary>
    Public Property Times As Double()

    ''' <summary>内部代谢物 id（列顺序）</summary>
    Public Property MetaboliteIds As String()

    ''' <summary>反应 id（列顺序）</summary>
    Public Property ReactionIds As String()

    ''' <summary>浓度轨迹 (T × m)</summary>
    Public Property Concentrations As Tensor

    ''' <summary>通量轨迹 (T × r)</summary>
    Public Property Fluxes As Tensor

    ''' <summary>系统时间常数 τ^sys 轨迹 (T × m)</summary>
    Public Property Tau As Tensor

    Public ReadOnly Property Steps As Integer
        Get
            Return If(Times Is Nothing, 0, Times.Length)
        End Get
    End Property

    Public ReadOnly Property MetaboliteCount As Integer
        Get
            Return If(MetaboliteIds Is Nothing, 0, MetaboliteIds.Length)
        End Get
    End Property

    Public ReadOnly Property ReactionCount As Integer
        Get
            Return If(ReactionIds Is Nothing, 0, ReactionIds.Length)
        End Get
    End Property

#Region "轨迹切片"

    ''' <summary>取某个代谢物的浓度时间序列</summary>
    Public Function ConcentrationOf(metaboliteId As String) As Double()
        Dim j = Array.IndexOf(MetaboliteIds, metaboliteId)

        If j < 0 Then
            Throw New ArgumentException($"未知代谢物：{metaboliteId}")
        End If

        Return Column(Concentrations, j)
    End Function

    ''' <summary>取某条反应的通量时间序列</summary>
    Public Function FluxOf(reactionId As String) As Double()
        Dim j = Array.IndexOf(ReactionIds, reactionId)

        If j < 0 Then
            Throw New ArgumentException($"未知反应：{reactionId}")
        End If

        Return Column(Fluxes, j)
    End Function

    ''' <summary>取某个代谢物的 τ^sys 时间序列</summary>
    Public Function TauOf(metaboliteId As String) As Double()
        Dim j = Array.IndexOf(MetaboliteIds, metaboliteId)

        If j < 0 Then
            Throw New ArgumentException($"未知代谢物：{metaboliteId}")
        End If

        Return Column(Tau, j)
    End Function

    Private Function Column(mat As Tensor, j As Integer) As Double()
        Dim n = mat.Shape(0)
        Dim out(n - 1) As Double

        For i = 0 To n - 1
            out(i) = mat(i, j)
        Next

        Return out
    End Function

#End Region

#Region "评估指标"

    ''' <summary>
    ''' 与观测浓度矩阵 (T × m) 的均方根误差
    ''' </summary>
    Public Function RMSE(observed As Tensor) As Double
        Return std.Sqrt(MeanSquared(observed))
    End Function

    ''' <summary>与观测浓度矩阵的均方误差</summary>
    Public Function MSE(observed As Tensor) As Double
        Return MeanSquared(observed)
    End Function

    ''' <summary>与观测浓度矩阵的平均绝对误差</summary>
    Public Function MAE(observed As Tensor) As Double
        Dim sum As Double = 0.0
        Dim n As Integer = 0

        For i = 0 To Concentrations.Shape(0) - 1
            For j = 0 To Concentrations.Shape(1) - 1
                sum += std.Abs(Concentrations(i, j) - observed(i, j))
                n += 1
            Next
        Next

        Return sum / std.Max(1, n)
    End Function

    Private Function MeanSquared(observed As Tensor) As Double
        Dim sum As Double = 0.0
        Dim n As Integer = 0

        For i = 0 To Concentrations.Shape(0) - 1
            For j = 0 To Concentrations.Shape(1) - 1
                Dim d = Concentrations(i, j) - observed(i, j)
                sum += d * d
                n += 1
            Next
        Next

        Return sum / std.Max(1, n)
    End Function

    ''' <summary>
    ''' 决定系数 R²（按列，即按代谢物计算后取平均，更能反映各代谢物的拟合质量）
    ''' </summary>
    Public Function R2(observed As Tensor) As Double
        Dim m = Concentrations.Shape(1)
        Dim T = Concentrations.Shape(0)
        Dim acc As Double = 0.0
        Dim counted As Integer = 0

        For j = 0 To m - 1
            Dim mean As Double = 0.0

            For i = 0 To T - 1
                mean += observed(i, j)
            Next
            mean /= T

            Dim ssRes As Double = 0.0
            Dim ssTot As Double = 0.0

            For i = 0 To T - 1
                Dim d = Concentrations(i, j) - observed(i, j)
                ssRes += d * d
                Dim dv = observed(i, j) - mean
                ssTot += dv * dv
            Next

            ' 观测为常数时无法定义 R²，跳过该列
            If ssTot > 0.0000000000001 Then
                acc += 1.0 - ssRes / ssTot
                counted += 1
            End If
        Next

        Return If(counted = 0, 0.0, acc / counted)
    End Function

    ''' <summary>
    ''' 整条轨迹上的平均稳态违反度 mean‖S·v‖₂
    ''' </summary>
    Public Function SteadyStateViolation(graph As MetabolicNetworkGraph) As Double
        Dim acc As Double = 0.0
        Dim T = Fluxes.Shape(0)

        For t = 0 To T - 1
            acc += graph.SteadyStateViolation(Row(Fluxes, t))
        Next

        Return acc / std.Max(1, T)
    End Function

    Private Function Row(mat As Tensor, i As Integer) As Tensor
        Dim width = mat.Shape(1)
        Dim v = New Tensor(width)

        For j = 0 To width - 1
            v(j) = mat(i, j)
        Next

        Return v
    End Function

#End Region

#Region "导出"

    ''' <summary>把浓度/通量/τ 轨迹分别写成 CSV（行=时间，列=代谢物或反应）</summary>
    Public Sub SaveCsv(directory As String, Optional prefix As String = "sim")
        If Not Directory.Exists(directory) Then
            Directory.CreateDirectory(directory)
        End If

        WriteMatrix(Path.Combine(directory, $"{prefix}_concentrations.csv"), MetaboliteIds, Concentrations)
        WriteMatrix(Path.Combine(directory, $"{prefix}_fluxes.csv"), ReactionIds, Fluxes)
        WriteMatrix(Path.Combine(directory, $"{prefix}_tau.csv"), MetaboliteIds, Tau)
    End Sub

    Private Sub WriteMatrix(path As String, headers As String(), mat As Tensor)
        Dim sb As New StringBuilder()
        Dim T = mat.Shape(0)
        Dim W = mat.Shape(1)

        sb.Append("time").Append(","c)
        sb.AppendLine(String.Join(",", headers.Select(Function(h) Quote(h))))

        For i = 0 To T - 1
            sb.Append(Times(i).ToString("G6"))

            For j = 0 To W - 1
                sb.Append(","c).Append(mat(i, j).ToString("G6"))
            Next

            sb.AppendLine()
        Next

        File.WriteAllText(path, sb.ToString())
    End Sub

    Private Shared Function Quote(text As String) As String
        If text Is Nothing Then Return ""

        If text.Contains(","c) OrElse text.Contains(""""c) Then
            Return """" & text.Replace("""", """""") & """"
        End If

        Return text
    End Function

#End Region

End Class
