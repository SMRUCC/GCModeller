Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.GPS
Imports SMRUCC.genomics.Analysis.GPS.Config
Imports SMRUCC.genomics.Analysis.GPS.Training
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' GNN+Transformer as GRN model
''' </summary>
<Package("GPS")>
<RTypeExport("gps", GetType(GPSModel))>
<RTypeExport("gps_config", GetType(GPSConfig))>
Module gpsTools

    Sub Main()
        Call RInternal.generic.add("writeBin", GetType(GPSModel), AddressOf saveModel)
        Call RInternal.generic.add("readBin.modular_bayesian", GetType(Stream), AddressOf loadModel)
    End Sub

    Private Function saveModel(model As GPSModel, args As list, env As Environment) As Object
        Dim con As Stream = args!con
        Call model.Save(con)
        Call con.Flush()
        Return True
    End Function

    Private Function loadModel(s As Stream, args As list, env As Environment) As Object
        Return GPSFile.Load(s)
    End Function

    <ExportAPI("gps_config")>
    Public Function gps_config() As GPSConfig
        Return New GPSConfig
    End Function

    <ExportAPI("build_gps")>
    <RApiReturn(GetType(GPSModel))>
    Public Function build_gps(x As GeneExpressionData, prior As PriorNetwork, Optional config As GPSConfig = Nothing, Optional env As Environment = Nothing) As GPSModel
        Return New GPSModel(x, prior, config)
    End Function

    <ExportAPI("train")>
    <RApiReturn(GetType(GPSModel))>
    Public Function train(gps As GPSModel, Optional env As Environment = Nothing) As Object
        ' ---- 3. 训练 ----
        Dim sw As Stopwatch = Stopwatch.StartNew()
        Dim training As GPSTrainingResult = gps.Train(gps.Config.PrintEvery)
        Dim report As New StringBuilder
        Dim plannedSteps As Integer = gps.Dataset.Train().Count * gps.Config.FinetuneEpochs

        sw.Stop()

        If Not gps.IsTrained Then Return RInternal.debug.stop("模型未完成训练", env)
        If training.TrainLoss.IsNullOrEmpty Then Return RInternal.debug.stop("训练损失曲线为空", env)
        If training.TrainLoss.Any(Function(v) v.IsNaNImaginary) Then Return RInternal.debug.stop("训练损失出现 NaN/Inf", env)

        report.AppendLine(
              $"[训练] 完成，用时 {sw.Elapsed.TotalSeconds:F1}s；" &
              $"预训练 {training.PretrainLoss.Count} 轮，微调 {training.FinetuneEpochs} 轮")

        If training.PretrainLoss.Count > 0 Then
            report.AppendLine(
                  $"  预训练损失 {training.PretrainLoss.First():G6} → {training.PretrainLoss.Last():G6}")
        End If

        report.AppendLine(
              $"  微调损失 {training.TrainLoss.First():G6} → {training.TrainLoss.Last():G6}" &
              If(training.ValidationLoss.Count > 0,
                 $"；验证 {training.ValidationLoss.First():G6} → {training.ValidationLoss.Last():G6}" &
                 $"（最优 epoch {training.BestEpoch}，{training.BestValidationLoss:G6}）",
                 ""))

        ' ---- 4. 评估 ----
        Dim metrics As EvaluationMetrics = gps.Evaluate()
        Dim baseline As EvaluationMetrics = gps.EvaluateBaseline()

        report.AppendLine($"[评估] 模型 {metrics}")
        report.AppendLine($"[评估] 基线 {baseline}")
        report.AppendLine(
              $"[评估] 图外基因：模型 MSE={metrics.IsolatedMSE:G4} R²={metrics.IsolatedR2:F4}；" &
              $"基线 MSE={baseline.IsolatedMSE:G4} R²={baseline.IsolatedR2:F4}")

        If metrics.MSE.IsNaNImaginary Then Return RInternal.debug.stop("评估未返回有效 MSE", env)

        If metrics.R2 < baseline.R2 Then
            report.AppendLine(
                  $"  说明：本次仅训练 {training.FinetuneEpochs} 轮（{plannedSteps} 步），" &
                  "模型尚未超过历史持续性基线；增大 --epochs 可继续改善。")
        End If

        Call Console.WriteLine(report.ToString)

        Dim model As New vbObject(gps)

        Call model.setAttribute("training", training)
        Call model.setAttribute("metrics", metrics)
        Call model.setAttribute("baseline", baseline)
        Call model.setAttribute("report", report.ToString)

        Return model
    End Function
End Module
