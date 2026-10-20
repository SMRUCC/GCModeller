Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.GPS
Imports SMRUCC.genomics.Analysis.GPS.Config
Imports SMRUCC.genomics.Analysis.GPS.Training
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' GPS: the GNN + Transformer based gene regulatory network modelling toolkit.
''' </summary>
''' <remarks>
''' <para>
''' This R# package module wraps the <see cref="GPSModel"/> of the ``GPS`` sub-system.
''' The model fuses a local message passing branch (edge type aware aggregation over the
''' prior regulatory network) with a global multi head attention branch (Graphormer style
''' shortest path bias), encodes the obtained gene representations over time with a GRU
''' and finally decodes the expression of every single gene with its own linear readout.
''' </para>
''' <para>
''' The workflow exposed by this module is:
''' </para>
''' <list type="bullet">
''' <item><description>
''' ``gps_config``: create the hyper-parameter configuration object
''' (<see cref="GPSConfig"/>, registered as the ``gps_config`` R# type);
''' </description></item>
''' <item><description>
''' ``build_gps``: build a model object from a time series gene expression matrix and a
''' prior regulatory network object;
''' </description></item>
''' <item><description>
''' ``train``: run the masked reconstruction pre-training followed by the next-step
''' prediction fine-tuning, and attach the training diagnostics to the returned object.
''' </description></item>
''' </list>
''' <para>
''' A trained <see cref="GPSModel"/> implements the
''' <see cref="SMRUCC.genomics.Analysis.BNLearn.InsilicoPerturbationExperiment"/> interface,
''' so the ``knockouts``, ``knockdown`` and ``overexpress`` api of the ``bnlearn`` package
''' can be applied to it directly for in silico perturbation experiments. The generic
''' ``writeBin`` handler of this module persists a model into a zip based binary stream,
''' and ``readBin.modular_bayesian`` restores it back to a fully usable model object.
''' </para>
''' </remarks>
<Package("GPS")>
<RTypeExport("gps", GetType(GPSModel))>
<RTypeExport("gps_config", GetType(GPSConfig))>
Module gpsTools

    ''' <summary>
    ''' Register the custom binary serialization handlers of the GPS package.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Two generic handlers are registered on the R# runtime:
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>
    ''' ``writeBin`` for <see cref="GPSModel"/>: the model object is written into the
    ''' connection supplied by the ``con`` argument as a zip archive (configuration,
    ''' expression matrix, prior network, model parameters and training curves are stored
    ''' as separate entries).
    ''' </description></item>
    ''' <item><description>
    ''' ``readBin.modular_bayesian`` for <see cref="Stream"/>: the stream is parsed as a
    ''' GPS model package and a fully reconstructed model object is returned, including the
    ''' derived structures (regulatory graph, graph encoding, sliding window dataset and the
    ''' virtual perturbation engine).
    ''' </description></item>
    ''' </list>
    ''' </remarks>
    Sub Main()
        Call RInternal.generic.add("writeBin", GetType(GPSModel), AddressOf saveModel)
        Call RInternal.generic.add("readBin.gps", GetType(Stream), AddressOf loadModel)
    End Sub

    ''' <summary>
    ''' R# ``writeBin`` handler: persist a GPS model into the given connection.
    ''' </summary>
    ''' <param name="model">the trained GPS model to be serialized</param>
    ''' <param name="args">the R# named argument list; the ``con`` argument supplies the output stream</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns><c>True</c> when the model has been written successfully</returns>
    Private Function saveModel(model As GPSModel, args As list, env As Environment) As Object
        Dim con As Stream = args!con

        Try
            Call model.Save(con)
            Call con.Flush()
        Catch ex As Exception

        End Try

        Return True
    End Function

    ''' <summary>
    ''' R# ``readBin.modular_bayesian`` handler: restore a GPS model from a connection.
    ''' </summary>
    ''' <param name="s">the stream that contains a GPS model package</param>
    ''' <param name="args">the R# named argument list (unused)</param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>the reconstructed <see cref="GPSModel"/> object</returns>
    Private Function loadModel(s As Stream, args As list, env As Environment) As Object
        Return GPSFile.Load(s)
    End Function

    ''' <summary>
    ''' Create a GPS model hyper-parameter configuration object.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The returned <see cref="GPSConfig"/> object (registered as the ``gps_config`` R# type)
    ''' is accepted by the ``config`` argument of the ``gps::build_gps`` api and controls the
    ''' whole pipeline: data preprocessing, graph encoding, the GPS fusion backbone, the GRU
    ''' time encoder, the training schedule and the default behaviour of the virtual
    ''' perturbation analysis.
    ''' </para>
    ''' <para>
    ''' Every argument is optional and carries exactly the same default value as the
    ''' corresponding property of <see cref="GPSConfig"/>, so calling ``gps_config()`` without
    ''' any argument is equivalent to ``new("gps_config")``. The assembled object is validated
    ''' by <see cref="GPSConfig.Validate"/> before it is returned: an invalid combination (for
    ''' example ``nHeads`` not dividing ``dModel``, ``window`` smaller than 2, or
    ''' ``trainRatio + validationRatio`` greater than or equal to 1) raises an
    ''' <see cref="ArgumentException"/> instead of failing later during training.
    ''' </para>
    ''' </remarks>
    ''' <param name="maxGenes">
    ''' maximum number of genes used for modelling; ``0`` disables the high variance filter and
    ''' keeps every gene of the input matrix. When the limit is exceeded the most variable genes
    ''' are kept, together with all transcription factors of the prior network. Default 2000,
    ''' because the global attention layer scales as O(N^2).
    ''' </param>
    ''' <param name="cpmNormalize">
    ''' apply a CPM library size normalization to every sample column before modelling.
    ''' Default ``True``.
    ''' </param>
    ''' <param name="logTransform">
    ''' apply the ``log2(x + 1)`` variance stabilizing transformation after the CPM normalization.
    ''' Default ``True``.
    ''' </param>
    ''' <param name="zScore">
    ''' standardize every gene across the samples (per gene z-score) so that genes with very
    ''' different expression levels share a comparable scale. Default ``True``.
    ''' </param>
    ''' <param name="peDim">
    ''' dimension of the Laplacian positional encoding, that is, the number of the smallest
    ''' non-trivial Laplacian eigenvectors that are projected into the node embedding. ``0``
    ''' disables the positional encoding. Default 16.
    ''' </param>
    ''' <param name="spdMaxDist">
    ''' maximum shortest path distance that receives its own attention bias bucket; longer and
    ''' disconnected gene pairs are merged into dedicated buckets. The bucket count is
    ''' ``spdMaxDist + 2``. Default 5.
    ''' </param>
    ''' <param name="coexpressionTopK">
    ''' number of co-expression edges that are added for every gene (the GEARS style covariance
    ''' graph); ``0`` keeps the prior regulatory edges only. Default 0.
    ''' </param>
    ''' <param name="minCoexpression">
    ''' minimum absolute Pearson correlation required for a co-expression edge. Only used when
    ''' ``coexpressionTopK`` is greater than zero. Default 0.7.
    ''' </param>
    ''' <param name="dModel">
    ''' hidden dimension of the model (``d_model``). Must be divisible by ``nHeads``. Default 128.
    ''' </param>
    ''' <param name="nLayers">
    ''' number of stacked GPS fusion layers, that is, the number of message passing hops that can
    ''' be captured. Default 3.
    ''' </param>
    ''' <param name="nHeads">
    ''' number of attention heads of the global attention branch. Must divide ``dModel``.
    ''' Default 8.
    ''' </param>
    ''' <param name="ffnDim">
    ''' hidden dimension of the feed forward network inside every GPS layer. Default 256.
    ''' </param>
    ''' <param name="dropout">
    ''' dropout rate applied to the GRU output representation; must be in ``[0, 1)``.
    ''' Default 0.1.
    ''' </param>
    ''' <param name="geneEmbedDim">
    ''' dimension of the learnable gene identity embedding. Default 32.
    ''' </param>
    ''' <param name="useSpdBias">
    ''' enable the Graphormer style shortest path attention bias. Switching it off is the
    ''' ablation setting for "global attention without topology awareness". Default ``True``.
    ''' </param>
    ''' <param name="useGlobalAttention">
    ''' enable the global attention branch; ``False`` degenerates the model into a pure GNN.
    ''' Default ``True``.
    ''' </param>
    ''' <param name="useLocalMpnn">
    ''' enable the local message passing branch; ``False`` degenerates the model into a pure
    ''' Transformer. Default ``True``.
    ''' </param>
    ''' <param name="window">
    ''' length of the input time window (``w``); at least 2 and at most the number of distinct
    ''' time points minus one. Default 5.
    ''' </param>
    ''' <param name="gruHidden">
    ''' hidden dimension of the GRU time encoder. Default 128.
    ''' </param>
    ''' <param name="useUncertainty">
    ''' add a softplus variance head and switch the main loss to the Gaussian negative log
    ''' likelihood, so the model reports a per-gene uncertainty together with the prediction.
    ''' Default ``False``.
    ''' </param>
    ''' <param name="pretrainEpochs">
    ''' number of epochs of the masked reconstruction pre-training stage; ``0`` skips it.
    ''' Default 30.
    ''' </param>
    ''' <param name="finetuneEpochs">
    ''' maximum number of epochs of the next-step prediction fine-tuning stage. Default 100.
    ''' </param>
    ''' <param name="learningRate">
    ''' Adam learning rate used by both training stages. Default 0.0001.
    ''' </param>
    ''' <param name="autofocusGamma">
    ''' exponent increment of the autofocus loss, the total exponent is ``2 + autofocusGamma``;
    ''' larger values put more weight on strongly changed genes. Default 1.0.
    ''' </param>
    ''' <param name="lambdaDir">
    ''' weight of the direction aware loss term. Default 0.5.
    ''' </param>
    ''' <param name="lambdaL1">
    ''' weight of the L1 sparse regularization term. Default 0.0001.
    ''' </param>
    ''' <param name="lambdaPrior">
    ''' weight of the prior distance regularization term; it is only meaningful for a learnable
    ''' adjacency matrix and therefore defaults to 0 in the current fixed ``A_prior`` version.
    ''' Default 0.
    ''' </param>
    ''' <param name="gradientClip">
    ''' global L2 norm clip threshold applied to the gradients before every Adam step;
    ''' ``0`` disables the clipping. Default 1.0.
    ''' </param>
    ''' <param name="earlyStopPatience">
    ''' number of consecutive epochs without validation improvement before the fine-tuning stage
    ''' stops and rolls back to the best parameters; ``0`` disables early stopping. Default 15.
    ''' </param>
    ''' <param name="maskRatio">
    ''' fraction of genes whose last time frame is masked during the pre-training stage; must be
    ''' in ``[0, 1)``. Default 0.15.
    ''' </param>
    ''' <param name="trainRatio">
    ''' fraction of the sliding windows used for training; the split is always performed in time
    ''' order, never randomly. Default 0.7.
    ''' </param>
    ''' <param name="validationRatio">
    ''' fraction of the sliding windows used for validation (early stopping).
    ''' ``trainRatio + validationRatio`` must be smaller than 1. Default 0.15.
    ''' </param>
    ''' <param name="seed">
    ''' random seed of the parameter initialization, the dropout masks and the pre-training masks;
    ''' a fixed seed makes the whole run reproducible. Default 2024.
    ''' </param>
    ''' <param name="significanceZScore">
    ''' a gene is flagged as significantly changed when the absolute Z-score of its perturbation
    ''' effect exceeds this threshold. Default 0.5.
    ''' </param>
    ''' <param name="cascadeSteps">
    ''' default number of recursive steps used by the temporal cascade simulation of a virtual
    ''' perturbation. Default 5.
    ''' </param>
    ''' <param name="denseAttentionMaxNodes">
    ''' gene count up to which the exact dense attention (an ``N×N`` score matrix) and the exact
    ''' Laplacian positional encoding are used. Above this threshold the model switches to the
    ''' sparse candidate attention (``O(N·c·d)``) and to the random walk positional encoding, which
    ''' is what makes the whole genome scale (``N`` around 60 thousand) trainable. Default 1024.
    ''' </param>
    ''' <param name="attentionNeighbors">
    ''' number of attention candidates per gene in the sparse mode (including the gene itself):
    ''' self + graph neighbourhood (budget truncated BFS) + hub genes + random long range links.
    ''' Default 32.
    ''' </param>
    ''' <param name="attentionRandomLinks">
    ''' number of random long range links per gene in the sparse mode; the slots left over by a
    ''' small graph neighbourhood are filled with additional random links, so even a prior network
    ''' without any edge still keeps a global receptive field. Default 8.
    ''' </param>
    ''' <param name="attentionHubGenes">
    ''' number of high degree genes included as global hubs in every candidate set of the sparse
    ''' mode. Default 8.
    ''' </param>
    ''' <param name="knockdownResidual">
    ''' the fraction of the wild type expression level that is kept for the target gene of an in
    ''' silico knockdown experiment: the intervention value is <c>gene mean * knockdownResidual</c>.
    ''' <c>0.2</c> corresponds to the CRISPRi setting of the Virtual Cell Challenge (more than 80
    ''' percent on target knockdown). Default 0.2.
    ''' </param>
    ''' <returns>a validated <see cref="GPSConfig"/> object</returns>
    ''' <exception cref="ArgumentException">the assembled configuration is not valid</exception>
    <ExportAPI("gps_config")>
    <RApiReturn(GetType(GPSConfig))>
    Public Function gps_config(Optional maxGenes As Integer = 2000,
                               Optional cpmNormalize As Boolean = True,
                               Optional logTransform As Boolean = True,
                               Optional zScore As Boolean = True,
                               Optional peDim As Integer = 16,
                               Optional spdMaxDist As Integer = 5,
                               Optional coexpressionTopK As Integer = 0,
                               Optional minCoexpression As Double = 0.7,
                               Optional dModel As Integer = 128,
                               Optional nLayers As Integer = 3,
                               Optional nHeads As Integer = 8,
                               Optional ffnDim As Integer = 256,
                               Optional dropout As Double = 0.1,
                               Optional geneEmbedDim As Integer = 32,
                               Optional useSpdBias As Boolean = True,
                               Optional useGlobalAttention As Boolean = True,
                               Optional useLocalMpnn As Boolean = True,
                               Optional window As Integer = 5,
                               Optional gruHidden As Integer = 128,
                               Optional useUncertainty As Boolean = False,
                               Optional pretrainEpochs As Integer = 30,
                               Optional finetuneEpochs As Integer = 100,
                               Optional learningRate As Double = 0.0001,
                               Optional autofocusGamma As Double = 1.0,
                               Optional lambdaDir As Double = 0.5,
                               Optional lambdaL1 As Double = 0.0001,
                               Optional lambdaPrior As Double = 0.0,
                               Optional gradientClip As Double = 1.0,
                               Optional earlyStopPatience As Integer = 15,
                               Optional maskRatio As Double = 0.15,
                               Optional trainRatio As Double = 0.7,
                               Optional validationRatio As Double = 0.15,
                               Optional seed As Integer = 2024,
                               Optional significanceZScore As Double = 0.5,
                               Optional cascadeSteps As Integer = 5,
                               Optional CheckpointIntervalEpochs As Integer = 1,
                               Optional denseAttentionMaxNodes As Integer = 1024,
                               Optional attentionNeighbors As Integer = 32,
                               Optional attentionRandomLinks As Integer = 8,
                               Optional attentionHubGenes As Integer = 8,
                               Optional knockdownResidual As Double = 0.2,
                               Optional env As Environment = Nothing) As Object

        Dim config As New GPSConfig With {
            .MaxGenes = maxGenes,
            .CpmNormalize = cpmNormalize,
            .LogTransform = logTransform,
            .ZScore = zScore,
            .PeDim = peDim,
            .SpdMaxDist = spdMaxDist,
            .CoexpressionTopK = coexpressionTopK,
            .MinCoexpression = minCoexpression,
            .DModel = dModel,
            .NLayers = nLayers,
            .NHeads = nHeads,
            .FfnDim = ffnDim,
            .Dropout = dropout,
            .GeneEmbedDim = geneEmbedDim,
            .UseSpdBias = useSpdBias,
            .UseGlobalAttention = useGlobalAttention,
            .UseLocalMpnn = useLocalMpnn,
            .Window = window,
            .GruHidden = gruHidden,
            .UseUncertainty = useUncertainty,
            .PretrainEpochs = pretrainEpochs,
            .FinetuneEpochs = finetuneEpochs,
            .LearningRate = learningRate,
            .AutofocusGamma = autofocusGamma,
            .LambdaDir = lambdaDir,
            .LambdaL1 = lambdaL1,
            .LambdaPrior = lambdaPrior,
            .GradientClip = gradientClip,
            .EarlyStopPatience = earlyStopPatience,
            .MaskRatio = maskRatio,
            .TrainRatio = trainRatio,
            .ValidationRatio = validationRatio,
            .Seed = seed,
            .SignificanceZScore = significanceZScore,
            .CascadeSteps = cascadeSteps,
            .CheckpointIntervalEpochs = CheckpointIntervalEpochs,
            .DenseAttentionMaxNodes = denseAttentionMaxNodes,
            .AttentionNeighbors = attentionNeighbors,
            .AttentionRandomLinks = attentionRandomLinks,
            .AttentionHubGenes = attentionHubGenes,
            .KnockdownResidual = knockdownResidual
        }

        If Not config.Validate() Then
            Return RInternal.debug.stop(
                "invalid GPS configuration: please check that nHeads divides dModel, " &
                "window is at least 2, dropout is in [0, 1), maskRatio is in [0, 1), " &
                "trainRatio + validationRatio is smaller than 1, every dimension is positive, " &
                "denseAttentionMaxNodes is at least 1, attentionNeighbors is at least 2 and " &
                "attentionRandomLinks + attentionHubGenes is smaller than attentionNeighbors.", env)
        End If

        Return config
    End Function

    ''' <summary>
    ''' Build a GPS (GNN + Transformer) gene regulatory network model.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The constructor performs the following preparation steps once and keeps the results,
    ''' so they are reused by every training epoch and by the virtual perturbation analysis:
    ''' </para>
    ''' <list type="number">
    ''' <item><description>
    ''' preprocessing: CPM normalization, ``log2(x + 1)`` transformation, per gene z-score and
    ''' the optional high variance gene filter;
    ''' </description></item>
    ''' <item><description>
    ''' graph construction: the prior ``TF -> target`` edges are mapped onto the modelled genes,
    ''' optionally extended with co-expression edges;
    ''' </description></item>
    ''' <item><description>
    ''' static graph encoding: shortest path distance buckets, Laplacian positional encoding and
    ''' the in/out degree buckets;
    ''' </description></item>
    ''' <item><description>
    ''' the strictly time ordered train / validation / test split of the sliding windows.
    ''' </description></item>
    ''' </list>
    ''' <para>
    ''' The returned object is untrained; call ``gps::train`` before any prediction or
    ''' perturbation analysis. It can also be persisted with the ``writeBin`` handler and
    ''' restored with ``readBin.modular_bayesian``.
    ''' </para>
    ''' </remarks>
    ''' <param name="x">
    ''' the time series gene expression matrix object: rows are genes and columns are samples,
    ''' and every sample must carry a time point label (for example the matrix loaded by
    ''' ``geneExpression::load.expr``). At least three distinct time points are required.
    ''' </param>
    ''' <param name="prior">
    ''' the prior regulatory network object (for example created by
    ''' ``bnlearn::as.prior_net``). Gene pairs that are not described by this network are kept as
    ''' isolated nodes and still participate in the global attention branch.
    ''' </param>
    ''' <param name="config">
    ''' the hyper-parameter configuration created by ``gps::gps_config``; when ``Nothing`` the
    ''' default <see cref="GPSConfig"/> values are used. Note that the window length may be
    ''' clipped when the input has too few time points.
    ''' </param>
    ''' <param name="env">the R# runtime environment</param>
    ''' <returns>an untrained <see cref="GPSModel"/> object</returns>
    <ExportAPI("build_gps")>
    <RApiReturn(GetType(GPSModel))>
    Public Function build_gps(x As GeneExpressionData,
                              Optional prior As PriorNetwork = Nothing,
                              Optional config As GPSConfig = Nothing,
                              Optional env As Environment = Nothing) As GPSModel

        Return New GPSModel(x, prior, config)
    End Function

    ''' <summary>
    ''' Train the GPS model and return it together with the training diagnostics.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Training runs in two stages: the masked reconstruction pre-training stage, in which the
    ''' last time frame of a random subset of genes is masked, followed by the next-step
    ''' prediction fine-tuning stage, which uses the autofocus loss plus the direction aware
    ''' loss. The best validation parameters are restored when early stopping triggers.
    ''' </para>
    ''' <para>
    ''' The evaluation metrics are computed on the time ordered test windows, and the
    ''' "persistence" baseline (the last observed frame is used as the prediction) is reported
    ''' side by side, so the contribution of the graph and the attention can be judged. When the
    ''' model has not surpassed that baseline yet, an explanatory note is appended to the report;
    ''' increasing ``finetune_epochs`` usually helps.
    ''' </para>
    ''' <para>
    ''' The returned object carries four attributes: ``training``
    ''' (<see cref="GPSTrainingResult"/> with the loss curves), ``metrics``
    ''' (<see cref="EvaluationMetrics"/> of the model), ``baseline``
    ''' (<see cref="EvaluationMetrics"/> of the persistence baseline) and ``report``
    ''' (the formatted multi line text).
    ''' </para>
    ''' </remarks>
    ''' <param name="gps">the untrained model created by ``gps::build_gps``</param>
    ''' <param name="env">the R# runtime environment used to report errors</param>
    ''' <returns>
    ''' the trained model object with the ``training``, ``metrics``, ``baseline`` and ``report``
    ''' attributes attached; an R# error object is returned instead when the training fails or
    ''' when the produced loss curve is not finite.
    ''' </returns>
    <ExportAPI("train")>
    <RApiReturn(GetType(GPSModel))>
    Public Function train(gps As GPSModel, Optional checkpoint As String = Nothing, Optional env As Environment = Nothing) As Object
        ' ---- 3. 训练 ----
        Dim sw As Stopwatch = Stopwatch.StartNew()

        gps.CheckpointPath = checkpoint

        Dim training As GPSTrainingResult = gps.Train(resumeFromCheckpoint:=Not checkpoint.StringEmpty(, True))
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

    ' ==================== 模型元信息：供 R# 脚本做基因对齐与反标准化 ====================

    ''' <summary>
    ''' Get the gene ids of the trained GPS model, in the modelling order.
    ''' </summary>
    ''' <param name="gps">the trained model object created by ``GPS::build_gps`` or restored by ``readBin``.</param>
    ''' <returns>
    ''' a character vector of the gene id (Ensembl gene id for the human model), the order of
    ''' this vector is the row order of every expression profile produced by the model.
    ''' </returns>
    ''' <remarks>
    ''' The gene order of the model is <b>not</b> necessarily the same as the order of the input
    ''' expression matrix, so any external profile must be aligned against this vector instead of
    ''' against the raw input file.
    ''' </remarks>
    <ExportAPI("gene_names")>
    <RApiReturn(GetType(String))>
    Public Function gene_names(gps As GPSModel) As Object
        Return CType(gps.GeneNames.Clone(), String())
    End Function

    ''' <summary>
    ''' Get the per gene mean of the preprocessed expression data (log-CPM scale).
    ''' </summary>
    ''' <param name="gps">the trained model object.</param>
    ''' <returns>a numeric vector, one value per modelled gene.</returns>
    ''' <remarks>
    ''' Together with ``GPS::gene_sds`` this vector defines the z-score normalization of the
    ''' model: <c>z = (log2(cpm + 1) - mean) / sd</c>. External context profiles must be
    ''' normalized with exactly these two vectors before they are injected via ``GPS::set_context``.
    ''' </remarks>
    <ExportAPI("gene_means")>
    <RApiReturn(GetType(Double))>
    Public Function gene_means(gps As GPSModel) As Object
        Return CType(gps.GeneMeans.Clone(), Double())
    End Function

    ''' <summary>
    ''' Get the per gene standard deviation of the preprocessed expression data (log-CPM scale).
    ''' </summary>
    ''' <param name="gps">the trained model object.</param>
    ''' <returns>a numeric vector, one value per modelled gene.</returns>
    <ExportAPI("gene_sds")>
    <RApiReturn(GetType(Double))>
    Public Function gene_sds(gps As GPSModel) As Object
        Return CType(gps.GeneSDs.Clone(), Double())
    End Function

    ''' <summary>
    ''' Get the raw library size of every training sample (the denominator of the CPM normalization).
    ''' </summary>
    ''' <param name="gps">the trained model object.</param>
    ''' <returns>
    ''' a numeric vector of the library sizes, one value per training sample; it is exported so
    ''' that the caller can reason about the count scale of the model output.
    ''' </returns>
    <ExportAPI("library_sizes")>
    <RApiReturn(GetType(Double))>
    Public Function library_sizes(gps As GPSModel) As Object
        If gps.LibrarySizes Is Nothing Then
            Return New Double() {}
        End If

        Return CType(gps.LibrarySizes.Clone(), Double())
    End Function

    ''' <summary>
    ''' Get the gene alignment and the de-normalization parameters of the model as a tuple list.
    ''' </summary>
    ''' <param name="gps">the trained model object.</param>
    ''' <returns>
    ''' a list with the slots ``gene_names``, ``means``, ``sds`` and ``library_sizes``.
    ''' </returns>
    <ExportAPI("gene_stats")>
    <RApiReturn(GetType(Double))>
    Public Function gene_stats(gps As GPSModel) As Object
        Dim slots As New Dictionary(Of String, Object) From {
            {"gene_names", CType(gps.GeneNames.Clone(), String())},
            {"means", CType(gps.GeneMeans.Clone(), Double())},
            {"sds", CType(gps.GeneSDs.Clone(), Double())},
            {"library_sizes", If(gps.LibrarySizes Is Nothing, New Double() {}, CType(gps.LibrarySizes.Clone(), Double()))}
        }

        Return New list(slots)
    End Function

    ' ==================== 上下文条件化 ====================

    ''' <summary>
    ''' Inject an external cell context into the model as the control baseline of the following
    ''' in silico perturbation experiments.
    ''' </summary>
    ''' <param name="gps">the trained model object.</param>
    ''' <param name="genes">
    ''' a character vector of the gene id of the context profile; the ids must live in the same
    ''' identifier space as the model (Ensembl gene id for the human model).
    ''' </param>
    ''' <param name="logcpm">
    ''' a numeric vector of the <c>log2(CPM + 1)</c> expression value of the context, in the same
    ''' order as <paramref name="genes"/>. The caller is responsible for the library size
    ''' normalization and for the log transform; the model only applies its own z-score.
    ''' </param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' TRUE when the context has been applied; an R# error message object is returned when the
    ''' gene ids do not overlap the modelled genes at all.
    ''' </returns>
    ''' <remarks>
    ''' Genes of the model that are missing from <paramref name="genes"/> keep their own mean
    ''' (that is a z-score of zero), so a context is allowed to cover only the genes of the
    ''' sequencing panel.
    ''' </remarks>
    <ExportAPI("set_context")>
    Public Function set_context(gps As GPSModel,
                                <RRawVectorArgument(TypeCodes.string)> genes As Object,
                                <RRawVectorArgument(TypeCodes.double)> logcpm As Object,
                                Optional env As Environment = Nothing) As Object
        Dim geneIds As String() = CLRVector.asCharacter(genes)
        Dim values As Double() = CLRVector.asNumeric(logcpm)

        If geneIds.Length <> values.Length Then
            Return RInternal.debug.stop(
                $"the size of the genes vector ({geneIds.Length}) does not match the size of the logcpm vector ({values.Length})", env)
        End If

        Try
            Call gps.SetContext(geneIds, values)
        Catch ex As Exception
            Return RInternal.debug.stop(ex.Message, env)
        End Try

        Return True
    End Function

    ''' <summary>
    ''' Restore the control baseline of the model back to the training derived one.
    ''' </summary>
    ''' <param name="gps">the trained model object.</param>
    ''' <returns>TRUE</returns>
    <ExportAPI("reset_context")>
    Public Function reset_context(gps As GPSModel) As Object
        Call gps.ResetContext()

        Return True
    End Function

    ''' <summary>
    ''' Get or set the fraction of the wild type expression that is kept by an in silico knockdown.
    ''' </summary>
    ''' <param name="gps">the trained model object.</param>
    ''' <param name="residual">
    ''' the residual fraction in the range ``[0,1)``; a negative value (the default) means "only
    ''' read the current value without modifying it".
    ''' </param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' the currently effective residual fraction; ``0`` makes a knockdown identical to a knockout
    ''' and ``0.2`` is the CRISPRi setting used by the Virtual Cell Challenge.
    ''' </returns>
    <ExportAPI("knockdown_residual")>
    Public Function knockdown_residual(gps As GPSModel,
                                      Optional residual As Double = -1,
                                      Optional env As Environment = Nothing) As Object
        If residual >= 0 Then
            Try
                gps.KnockdownResidual = residual
            Catch ex As Exception
                Return RInternal.debug.stop(ex.Message, env)
            End Try
        End If

        Return gps.KnockdownResidual
    End Function

    ''' <summary>
    ''' Get or set whether the perturbation channel is enabled.
    ''' </summary>
    ''' <param name="gps">the trained model object.</param>
    ''' <param name="enabled">
    ''' ``1`` enables, ``0`` disables; a negative value (the default) only reads the current state.
    ''' </param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' the currently effective flag. When enabled, the change of the perturbed gene is injected
    ''' into the input window of its downstream targets along the prior regulatory edges, so a
    ''' single gene knockdown does not have to survive several rounds of degree normalisation.
    ''' </returns>
    <ExportAPI("perturbation_channel")>
    Public Function perturbation_channel(gps As GPSModel,
                                         Optional enabled As Integer = -1,
                                         Optional env As Environment = Nothing) As Object
        If enabled >= 0 Then
            gps.PerturbationChannel = enabled > 0
        End If

        Return gps.PerturbationChannel
    End Function

    ''' <summary>
    ''' Get or set the injection gain of the perturbation channel.
    ''' </summary>
    ''' <param name="gps">the trained model object.</param>
    ''' <param name="gain">
    ''' the non negative gain; a negative value (the default) only reads the current value.
    ''' ``0`` is equivalent to disabling the channel.
    ''' </param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>the currently effective gain.</returns>
    <ExportAPI("perturbation_channel_gain")>
    Public Function perturbation_channel_gain(gps As GPSModel,
                                              Optional gain As Double = -1,
                                              Optional env As Environment = Nothing) As Object
        If gain >= 0 Then
            Try
                gps.PerturbationChannelGain = gain
            Catch ex As Exception
                Return RInternal.debug.stop(ex.Message, env)
            End Try
        End If

        Return gps.PerturbationChannelGain
    End Function

    ''' <summary>
    ''' Get or set the co-regulation fallback of the perturbation channel.
    ''' </summary>
    ''' <param name="gps">the trained model object.</param>
    ''' <param name="enabled">
    ''' ``1`` enables, ``0`` disables; a negative value (the default) only reads the current state.
    ''' </param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' the currently effective flag. When enabled, perturbed genes without outgoing edges
    ''' (all non transcription factors in a TF to target prior) spread their change to the genes
    ''' that share the same upstream regulators.
    ''' </returns>
    <ExportAPI("perturbation_channel_coregulation")>
    Public Function perturbation_channel_coregulation(gps As GPSModel,
                                                      Optional enabled As Integer = -1,
                                                      Optional env As Environment = Nothing) As Object
        If enabled >= 0 Then
            gps.PerturbationChannelCoRegulation = enabled > 0
        End If

        Return gps.PerturbationChannelCoRegulation
    End Function

    ''' <summary>
    ''' The edge normalisation mode that the graph convolution actually uses.
    ''' </summary>
    ''' <param name="gps">the trained model object.</param>
    ''' <returns>
    ''' ``mean`` (convex averaging, the legacy behaviour), ``sqrt`` (the default, keeps more of the
    ''' perturbation magnitude across hops) or ``sum`` (no normalisation).
    ''' </returns>
    <ExportAPI("graph_normalization")>
    Public Function graph_normalization(gps As GPSModel) As Object
        Return gps.GraphNormalization.ToString().ToLowerInvariant()
    End Function

    ' ==================== 精简结果导出 ====================

    ''' <summary>
    ''' Write the mutant (and the control) expression profiles of a set of in silico perturbation
    ''' results as csv matrix files.
    ''' </summary>
    ''' <param name="results">
    ''' a collection of the <see cref="InterventionResult"/> object, for example the value returned
    ''' by the ``knockouts`` / ``knockdown`` / ``overexpress`` api of the ``bnlearn`` package.
    ''' </param>
    ''' <param name="file">
    ''' the output file path of the mutant matrix (``mutant_means_matrix.csv``), rows are genes and
    ''' columns are the intervention conditions.
    ''' </param>
    ''' <param name="controlFile">
    ''' the output file path of the control matrix; when it is nothing the file
    ''' ``wildtype_means_matrix.csv`` is written next to <paramref name="file"/>.
    ''' </param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' TRUE when both matrix files have been written; an R# error message object is returned when
    ''' the input can not be treated as a collection of <see cref="InterventionResult"/>.
    ''' </returns>
    ''' <remarks>
    ''' The generated csv files use exactly the same layout as the ones produced by the
    ''' ``make_exports`` api (four comment lines followed by a ``Gene,&lt;condition&gt;...`` header),
    ''' but only the two matrices that are needed by the downstream submission builder are written;
    ''' ``make_exports`` writes about eight tables per run which is several gigabytes for a few
    ''' hundred conditions.
    ''' </remarks>
    <ExportAPI("export_profiles")>
    Public Function export_profiles(<RRawVectorArgument(GetType(InterventionResult))> results As Object,
                                    file As String,
                                    Optional controlFile As String = Nothing,
                                    Optional env As Environment = Nothing) As Object

        Dim pull As PipeIterator(Of InterventionResult) = pipeline.Stream(Of InterventionResult)(results, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim res As InterventionResult() = pull.ToArray()
        Dim m As Integer = res.Length

        If m = 0 Then
            Return RInternal.debug.stop("the intervention result collection is empty", env)
        End If

        Dim geneNames As String() = res(0).GeneNames
        Dim n As Integer = geneNames.Length

        If String.IsNullOrEmpty(controlFile) Then
            controlFile = Path.Combine(Path.GetDirectoryName(file), "wildtype_means_matrix.csv")
        End If

        Dim folder As String = Path.GetDirectoryName(file)

        If Not String.IsNullOrEmpty(folder) AndAlso Not Directory.Exists(folder) Then
            Call Directory.CreateDirectory(folder)
        End If

        ' ---- 1. mutant 矩阵 ----
        Call WriteMatrix(file, res, geneNames, n, m, mutant:=True)
        ' ---- 2. control 矩阵 ----
        Call WriteMatrix(controlFile, res, geneNames, n, m, mutant:=False)

        Return True
    End Function

    ''' <summary>
    ''' Write one ``[genes x conditions]`` expression matrix as a csv file.
    ''' </summary>
    ''' <param name="file">the output csv file path</param>
    ''' <param name="res">the intervention results, one column per element</param>
    ''' <param name="geneNames">the modelled gene names (row names)</param>
    ''' <param name="n">the number of the genes</param>
    ''' <param name="m">the number of the conditions</param>
    ''' <param name="mutant">
    ''' TRUE writes <see cref="InterventionResult.MutantMeans"/>, FALSE writes
    ''' <see cref="InterventionResult.WildtypeMeans"/> (the control prediction is shared by every
    ''' condition, so only a single ``control`` column is written).
    ''' </param>
    Private Sub WriteMatrix(file As String, res As InterventionResult(),
                            geneNames As String(), n As Integer, m As Integer, mutant As Boolean)

        Dim columns As String()

        If mutant Then
            columns = res.Select(Function(r) $"{r.Spec.GeneName}_{r.Spec.Mode}").ToArray()
        Else
            columns = New String() {"control"}
            m = 1
        End If

        Using writer As New StreamWriter(file, False, New UTF8Encoding(False))
            If mutant Then
                Call writer.WriteLine("# Mutant expression means: rows=genes, columns=intervention conditions")
                Call writer.WriteLine("#")
                Call writer.WriteLine("# Metric: MutantMean")
            Else
                Call writer.WriteLine("# Wildtype expression means: rows=genes, columns=intervention conditions")
                Call writer.WriteLine("#")
                Call writer.WriteLine("# Metric: WildtypeMean")
            End If

            Call writer.WriteLine($"# Genes: {n}, Conditions: {m}")

            Dim line As New StringBuilder()
            Dim ci As CultureInfo = CultureInfo.InvariantCulture

            Call line.Append("Gene")

            For Each name As String In columns
                Call line.Append(","c).Append(name)
            Next

            Call writer.WriteLine(line.ToString())

            For i As Integer = 0 To n - 1
                Call line.Clear()
                Call line.Append(geneNames(i))

                If mutant Then
                    For k As Integer = 0 To m - 1
                        Call line.Append(","c).Append(res(k).MutantMeans(i).ToString("G9", ci))
                    Next
                Else
                    Call line.Append(","c).Append(res(0).WildtypeMeans(i).ToString("G9", ci))
                End If

                Call writer.WriteLine(line.ToString())
            Next

            Call writer.Flush()
        End Using
    End Sub
End Module
