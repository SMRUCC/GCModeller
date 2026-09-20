Imports SMRUCC.genomics.Analysis.Squiiff.IO
Imports std = System.Math

Namespace Training

    ''' <summary>单个训练轮次（epoch）的汇总记录。</summary>
    Public Class EpochRecord

        Public Property Epoch As Integer
        Public Property Steps As Integer

        ''' <summary>本轮平均噪声预测损失。</summary>
        Public Property DiffusionLoss As Double

        ''' <summary>本轮平均 KL 正则项。</summary>
        Public Property KLLoss As Double

        ''' <summary>本轮平均总损失（<c>diffusion + β·KL</c>）。</summary>
        Public Property TotalLoss As Double

        ''' <summary>本轮最后一步的学习率。</summary>
        Public Property LearningRate As Double

        ''' <summary>本轮平均（裁剪前）全局梯度范数。</summary>
        Public Property GradientNorm As Double

        ''' <summary>本轮耗时（秒）。</summary>
        Public Property ElapsedSeconds As Double
    End Class

    ''' <summary>训练历史：逐轮损失曲线、最佳轮次与耗时统计，可导出 CSV 供绘图。</summary>
    Public Class TrainingHistory

        Private ReadOnly _records As New List(Of EpochRecord)
        Private _bestLoss As Double = Double.MaxValue
        Private _bestEpoch As Integer = 0

        Public ReadOnly Property Records As List(Of EpochRecord)
            Get
                Return _records
            End Get
        End Property

        ''' <summary>训练总耗时（秒）。</summary>
        Public Property TotalSeconds As Double

        ''' <summary>达成的最佳总损失。</summary>
        Public ReadOnly Property BestLoss As Double
            Get
                Return _bestLoss
            End Get
        End Property

        ''' <summary>最佳损失对应的轮次。</summary>
        Public ReadOnly Property BestEpoch As Integer
            Get
                Return _bestEpoch
            End Get
        End Property

        ''' <summary>记录一轮结果并维护最佳轮次。</summary>
        Public Sub Add(record As EpochRecord)
            _records.Add(record)

            If record.TotalLoss < Me._bestLoss Then
                Me._bestLoss = record.TotalLoss
                Me._bestEpoch = record.Epoch
            End If
        End Sub

        ''' <summary>用最近 <paramref name="window"/> 轮的损失判断是否已不再改善。</summary>
        Public Function IsStagnant(window As Integer, tolerance As Double) As Boolean
            If window < 2 OrElse _records.Count < window Then Return False

            Dim best As Double = Double.MaxValue
            Dim last = _records(_records.Count - 1).TotalLoss

            For i As Integer = _records.Count - window To _records.Count - 1
                best = std.Min(best, _records(i).TotalLoss)
            Next

            Return last > best + tolerance
        End Function

        ''' <summary>导出损失曲线 CSV。</summary>
        Public Sub SaveCsv(path As String)
            Dim header = New String() {"epoch", "steps", "diffusion_loss", "kl_loss", "total_loss", "learning_rate", "grad_norm", "elapsed_seconds"}
            Dim rows As New List(Of Double())

            For Each r In _records
                rows.Add(New Double() {r.Epoch, r.Steps, r.DiffusionLoss, r.KLLoss, r.TotalLoss, r.LearningRate, r.GradientNorm, r.ElapsedSeconds})
            Next

            Call ResultWriter.WriteNumericTable(path, header, rows)
        End Sub

        ''' <summary>绘制终端损失曲线（ASCII），便于无绘图环境下直观查看收敛情况。</summary>
        Public Function RenderCurve(Optional width As Integer = 62, Optional height As Integer = 12) As String
            If _records.Count = 0 Then Return "（无训练记录）"

            Dim losses = _records.Select(Function(r) r.TotalLoss).ToArray()
            Dim low = losses.Min()
            Dim high = losses.Max()

            ' 首轮损失通常远高于后续，做对数可视缩放会使曲线可读性更好
            Dim logLow = std.Log(std.Max(low, 1.0E-12))
            Dim logHigh = std.Log(std.Max(high, 1.0E-12))
            If logHigh - logLow < 1.0E-09 Then logHigh = logLow + 1.0

            Dim canvas(height - 1, width - 1) As Char
            For r As Integer = 0 To height - 1
                For c As Integer = 0 To width - 1
                    canvas(r, c) = " "c
                Next
            Next

            For c As Integer = 0 To width - 1
                Dim index = CInt(std.Round(c * (losses.Length - 1) / std.Max(1, width - 1)))
                Dim level = (std.Log(std.Max(losses(index), 1.0E-12)) - logLow) / (logHigh - logLow)
                Dim row = height - 1 - CInt(std.Round(level * (height - 1)))
                canvas(std.Max(0, std.Min(height - 1, row)), c) = "*"c
            Next

            Dim sb As New Text.StringBuilder()
            sb.AppendLine($"  损失曲线（对数纵轴，范围 {low:G4} ~ {high:G4}）")

            Dim buffer(width - 1) As Char
            For r As Integer = 0 To height - 1
                For c As Integer = 0 To width - 1
                    buffer(c) = canvas(r, c)
                Next

                sb.AppendLine("   |" & New String(buffer))
            Next

            sb.AppendLine("   +" & New String("-"c, width))
            sb.AppendLine($"    epoch 1{"",12}epoch {_records(_records.Count - 1).Epoch}")

            Return sb.ToString()
        End Function
    End Class
End Namespace
