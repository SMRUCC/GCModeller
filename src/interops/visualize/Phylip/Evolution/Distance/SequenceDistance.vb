Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Models

Namespace Evolution.Distance

    ''' <summary>
    ''' 两两序列距离的估计模型
    ''' </summary>
    Public Enum DistanceModel
        ''' <summary>
        ''' p 距离：差异位点所占的比例（不做任何多重替换校正）
        ''' </summary>
        PDistance
        ''' <summary>
        ''' Poisson 校正：<c>d = -ln(1 - p)</c>，适用于蛋白质序列
        ''' </summary>
        PoissonCorrection
        ''' <summary>
        ''' Jukes-Cantor 校正：<c>d = -3/4 * ln(1 - 4/3 * p)</c>，适用于核酸序列
        ''' </summary>
        JukesCantor
    End Enum

    ''' <summary>
    ''' 由比对位点矩阵估计两两之间的进化距离，是距离法（UPGMA / NJ 以及 ML 初始树）的前置步骤。
    ''' </summary>
    Public Module SequenceDistance

        ''' <summary>
        ''' 当两条序列完全相同时所使用的最小距离，避免后续出现除零。
        ''' </summary>
        Public Const MinDistance As Double = 0.000001

        ''' <summary>
        ''' 计算整个字符矩阵的两两距离方阵。
        ''' </summary>
        Public Function PairwiseMatrix(matrix As CharacterMatrix,
                                       Optional model As DistanceModel = DistanceModel.PDistance) As DistanceMatrix

            Dim n As Integer = matrix.SequenceCount
            Dim mat(n - 1)() As Double

            For i As Integer = 0 To n - 1
                mat(i) = New Double(n - 1) {}
            Next

            For i As Integer = 0 To n - 2
                For j As Integer = i + 1 To n - 1
                    Dim d As Double = PairwiseDistance(matrix.States(i), matrix.States(j), model)
                    mat(i)(j) = d
                    mat(j)(i) = d
                Next
            Next

            Return DistanceMatrix.FromMatrix(matrix.Names, mat)
        End Function

        ''' <summary>
        ''' 计算两条（已编码的）序列之间的距离。
        ''' </summary>
        Public Function PairwiseDistance(a As Integer(), b As Integer(),
                                         Optional model As DistanceModel = DistanceModel.PDistance) As Double

            Dim diff As Integer = 0
            Dim comparable As Integer = 0

            For i As Integer = 0 To a.Length - 1
                If a(i) < 0 OrElse b(i) < 0 Then
                    Continue For
                End If

                comparable += 1

                If a(i) <> b(i) Then
                    diff += 1
                End If
            Next

            If comparable = 0 Then
                Return MinDistance
            End If

            Dim p As Double = diff / comparable

            Return Correction(p, model)
        End Function

        ''' <summary>
        ''' 将观测到的差异比例 p 依据给定的替换模型转换为进化距离。
        ''' </summary>
        Public Function Correction(p As Double, model As DistanceModel, Optional states As Integer = 20) As Double
            If p <= 0 Then
                Return MinDistance
            End If

            ' 完全饱和时校正值发散，这里做截断以避免得到 Infinity/NaN
            Const maxP As Double = 0.999999
            Dim pp As Double = If(p >= 1, maxP, p)

            Select Case model
                Case DistanceModel.PDistance
                    Return pp
                Case DistanceModel.PoissonCorrection
                    Return -Math.Log(1 - pp)
                Case DistanceModel.JukesCantor
                    Dim f As Double = states / (states - 1)
                    Return -(states - 1) / states * Math.Log(1 - f * pp)
                Case Else
                    Return pp
            End Select
        End Function
    End Module

End Namespace
