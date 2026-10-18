// export R# package module type define for javascript/typescript language
//
//    imports "GPS" from "biosystem";
//
// ref=biosystem.gpsTools@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * GPS: the GNN + Transformer based gene regulatory network modelling toolkit.
 * 
 * > This R# package module wraps the [GPSModel](cref:T:SMRUCC.genomics.Analysis.GPS.GPSModel) of the ``GPS`` sub-system.
 * >  The model fuses a local message passing branch (edge type aware aggregation over the
 * >  prior regulatory network) with a global multi head attention branch (Graphormer style
 * >  shortest path bias), encodes the obtained gene representations over time with a GRU
 * >  and finally decodes the expression of every single gene with its own linear readout.
 * >  
 * > 
 * > 
 * >  
 * > 
 * > 
 * >  The workflow exposed by this module is:
 * >  
 * > 
 * > 
 * >  
 * > - ``gps_config``: create the hyper-parameter configuration object
 * >  ([GPSConfig](cref:T:SMRUCC.genomics.Analysis.GPS.Config.GPSConfig), registered as the ``gps_config`` R# type);
 * > - ``build_gps``: build a model object from a time series gene expression matrix and a
 * >  prior regulatory network object;
 * > - ``train``: run the masked reconstruction pre-training followed by the next-step
 * >  prediction fine-tuning, and attach the training diagnostics to the returned object.
 * > 
 * > 
 * >  
 * > 
 * > 
 * >  A trained [GPSModel](cref:T:SMRUCC.genomics.Analysis.GPS.GPSModel) implements the
 * >  [InsilicoPerturbationExperiment](cref:T:SMRUCC.genomics.Analysis.BNLearn.InsilicoPerturbationExperiment) interface,
 * >  so the ``knockouts``, ``knockdown`` and ``overexpress`` api of the ``bnlearn`` package
 * >  can be applied to it directly for in silico perturbation experiments. The generic
 * >  ``writeBin`` handler of this module persists a model into a zip based binary stream,
 * >  and ``readBin.modular_bayesian`` restores it back to a fully usable model object.
*/
declare namespace GPS {
   /**
    * Build a GPS (GNN + Transformer) gene regulatory network model.
    * 
    * > The constructor performs the following preparation steps once and keeps the results,
    * >  so they are reused by every training epoch and by the virtual perturbation analysis:
    * >  
    * > 
    * > 
    * >  
    * > 1. preprocessing: CPM normalization, ``log2(x + 1)`` transformation, per gene z-score and
    * >  the optional high variance gene filter;
    * > 2. graph construction: the prior ``TF -> target`` edges are mapped onto the modelled genes,
    * >  optionally extended with co-expression edges;
    * > 3. static graph encoding: shortest path distance buckets, Laplacian positional encoding and
    * >  the in/out degree buckets;
    * > 4. the strictly time ordered train / validation / test split of the sliding windows.
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  The returned object is untrained; call ``gps::train`` before any prediction or
    * >  perturbation analysis. It can also be persisted with the ``writeBin`` handler and
    * >  restored with ``readBin.modular_bayesian``.
    * 
     * @param x the time series gene expression matrix object: rows are genes and columns are samples,
     *  and every sample must carry a time point label (for example the matrix loaded by
     *  ``geneExpression::load.expr``). At least three distinct time points are required.
     * @param prior the prior regulatory network object (for example created by
     *  ``bnlearn::as.prior_net``). Gene pairs that are not described by this network are kept as
     *  isolated nodes and still participate in the global attention branch.
     * @param config the hyper-parameter configuration created by ``gps::gps_config``; when ``Nothing`` the
     *  default [GPSConfig](cref:T:SMRUCC.genomics.Analysis.GPS.Config.GPSConfig) values are used. Note that the window length may be
     *  clipped when the input has too few time points.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment
     * 
     * + default value Is ``null``.
     * @return an untrained [GPSModel](cref:T:SMRUCC.genomics.Analysis.GPS.GPSModel) object
   */
   function build_gps(x: object, prior: object, config?: object, env?: object): object;
   /**
    * Create a GPS model hyper-parameter configuration object.
    * 
    * > The returned [GPSConfig](cref:T:SMRUCC.genomics.Analysis.GPS.Config.GPSConfig) object (registered as the ``gps_config`` R# type)
    * >  is accepted by the ``config`` argument of the ``gps::build_gps`` api and controls the
    * >  whole pipeline: data preprocessing, graph encoding, the GPS fusion backbone, the GRU
    * >  time encoder, the training schedule and the default behaviour of the virtual
    * >  perturbation analysis.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  Every argument is optional and carries exactly the same default value as the
    * >  corresponding property of [GPSConfig](cref:T:SMRUCC.genomics.Analysis.GPS.Config.GPSConfig), so calling ``gps_config()`` without
    * >  any argument is equivalent to ``new("gps_config")``. The assembled object is validated
    * >  by [GPSConfig.Validate()](cref:M:SMRUCC.genomics.Analysis.GPS.Config.GPSConfig.Validate) before it is returned: an invalid combination (for
    * >  example ``nHeads`` not dividing ``dModel``, ``window`` smaller than 2, or
    * >  ``trainRatio + validationRatio`` greater than or equal to 1) raises an
    * >  [ArgumentException](cref:T:System.ArgumentException) instead of failing later during training.
    * 
     * @param maxGenes maximum number of genes used for modelling; ``0`` disables the high variance filter and
     *  keeps every gene of the input matrix. When the limit is exceeded the most variable genes
     *  are kept, together with all transcription factors of the prior network. Default 2000,
     *  because the global attention layer scales as O(N^2).
     * 
     * + default value Is ``2000``.
     * @param cpmNormalize apply a CPM library size normalization to every sample column before modelling.
     *  Default ``True``.
     * 
     * + default value Is ``true``.
     * @param logTransform apply the ``log2(x + 1)`` variance stabilizing transformation after the CPM normalization.
     *  Default ``True``.
     * 
     * + default value Is ``true``.
     * @param zScore standardize every gene across the samples (per gene z-score) so that genes with very
     *  different expression levels share a comparable scale. Default ``True``.
     * 
     * + default value Is ``true``.
     * @param peDim dimension of the Laplacian positional encoding, that is, the number of the smallest
     *  non-trivial Laplacian eigenvectors that are projected into the node embedding. ``0``
     *  disables the positional encoding. Default 16.
     * 
     * + default value Is ``16``.
     * @param spdMaxDist maximum shortest path distance that receives its own attention bias bucket; longer and
     *  disconnected gene pairs are merged into dedicated buckets. The bucket count is
     *  ``spdMaxDist + 2``. Default 5.
     * 
     * + default value Is ``5``.
     * @param coexpressionTopK number of co-expression edges that are added for every gene (the GEARS style covariance
     *  graph); ``0`` keeps the prior regulatory edges only. Default 0.
     * 
     * + default value Is ``0``.
     * @param minCoexpression minimum absolute Pearson correlation required for a co-expression edge. Only used when
     *  ``coexpressionTopK`` is greater than zero. Default 0.7.
     * 
     * + default value Is ``0.7``.
     * @param dModel hidden dimension of the model (``d_model``). Must be divisible by ``nHeads``. Default 128.
     * 
     * + default value Is ``128``.
     * @param nLayers number of stacked GPS fusion layers, that is, the number of message passing hops that can
     *  be captured. Default 3.
     * 
     * + default value Is ``3``.
     * @param nHeads number of attention heads of the global attention branch. Must divide ``dModel``.
     *  Default 8.
     * 
     * + default value Is ``8``.
     * @param ffnDim hidden dimension of the feed forward network inside every GPS layer. Default 256.
     * 
     * + default value Is ``256``.
     * @param dropout dropout rate applied to the GRU output representation; must be in ``[0, 1)``.
     *  Default 0.1.
     * 
     * + default value Is ``0.1``.
     * @param geneEmbedDim dimension of the learnable gene identity embedding. Default 32.
     * 
     * + default value Is ``32``.
     * @param useSpdBias enable the Graphormer style shortest path attention bias. Switching it off is the
     *  ablation setting for "global attention without topology awareness". Default ``True``.
     * 
     * + default value Is ``true``.
     * @param useGlobalAttention enable the global attention branch; ``False`` degenerates the model into a pure GNN.
     *  Default ``True``.
     * 
     * + default value Is ``true``.
     * @param useLocalMpnn enable the local message passing branch; ``False`` degenerates the model into a pure
     *  Transformer. Default ``True``.
     * 
     * + default value Is ``true``.
     * @param window length of the input time window (``w``); at least 2 and at most the number of distinct
     *  time points minus one. Default 5.
     * 
     * + default value Is ``5``.
     * @param gruHidden hidden dimension of the GRU time encoder. Default 128.
     * 
     * + default value Is ``128``.
     * @param useUncertainty add a softplus variance head and switch the main loss to the Gaussian negative log
     *  likelihood, so the model reports a per-gene uncertainty together with the prediction.
     *  Default ``False``.
     * 
     * + default value Is ``false``.
     * @param pretrainEpochs number of epochs of the masked reconstruction pre-training stage; ``0`` skips it.
     *  Default 30.
     * 
     * + default value Is ``30``.
     * @param finetuneEpochs maximum number of epochs of the next-step prediction fine-tuning stage. Default 100.
     * 
     * + default value Is ``100``.
     * @param learningRate Adam learning rate used by both training stages. Default 0.0001.
     * 
     * + default value Is ``0.0001``.
     * @param autofocusGamma exponent increment of the autofocus loss, the total exponent is ``2 + autofocusGamma``;
     *  larger values put more weight on strongly changed genes. Default 1.0.
     * 
     * + default value Is ``1``.
     * @param lambdaDir weight of the direction aware loss term. Default 0.5.
     * 
     * + default value Is ``0.5``.
     * @param lambdaL1 weight of the L1 sparse regularization term. Default 0.0001.
     * 
     * + default value Is ``0.0001``.
     * @param lambdaPrior weight of the prior distance regularization term; it is only meaningful for a learnable
     *  adjacency matrix and therefore defaults to 0 in the current fixed ``A_prior`` version.
     *  Default 0.
     * 
     * + default value Is ``0``.
     * @param gradientClip global L2 norm clip threshold applied to the gradients before every Adam step;
     *  ``0`` disables the clipping. Default 1.0.
     * 
     * + default value Is ``1``.
     * @param earlyStopPatience number of consecutive epochs without validation improvement before the fine-tuning stage
     *  stops and rolls back to the best parameters; ``0`` disables early stopping. Default 15.
     * 
     * + default value Is ``15``.
     * @param maskRatio fraction of genes whose last time frame is masked during the pre-training stage; must be
     *  in ``[0, 1)``. Default 0.15.
     * 
     * + default value Is ``0.15``.
     * @param printEvery print the loss every ``printEvery`` epochs; ``0`` keeps the training silent. Default 5.
     * 
     * + default value Is ``5``.
     * @param trainRatio fraction of the sliding windows used for training; the split is always performed in time
     *  order, never randomly. Default 0.7.
     * 
     * + default value Is ``0.7``.
     * @param validationRatio fraction of the sliding windows used for validation (early stopping).
     *  ``trainRatio + validationRatio`` must be smaller than 1. Default 0.15.
     * 
     * + default value Is ``0.15``.
     * @param seed random seed of the parameter initialization, the dropout masks and the pre-training masks;
     *  a fixed seed makes the whole run reproducible. Default 2024.
     * 
     * + default value Is ``2024``.
     * @param significanceZScore a gene is flagged as significantly changed when the absolute Z-score of its perturbation
     *  effect exceeds this threshold. Default 0.5.
     * 
     * + default value Is ``0.5``.
     * @param cascadeSteps default number of recursive steps used by the temporal cascade simulation of a virtual
     *  perturbation. Default 5.
     * 
     * + default value Is ``5``.
     * @param env 
     * + default value Is ``null``.
     * @return a validated [GPSConfig](cref:T:SMRUCC.genomics.Analysis.GPS.Config.GPSConfig) object
   */
   function gps_config(maxGenes?: object, cpmNormalize?: boolean, logTransform?: boolean, zScore?: boolean, peDim?: object, spdMaxDist?: object, coexpressionTopK?: object, minCoexpression?: number, dModel?: object, nLayers?: object, nHeads?: object, ffnDim?: object, dropout?: number, geneEmbedDim?: object, useSpdBias?: boolean, useGlobalAttention?: boolean, useLocalMpnn?: boolean, window?: object, gruHidden?: object, useUncertainty?: boolean, pretrainEpochs?: object, finetuneEpochs?: object, learningRate?: number, autofocusGamma?: number, lambdaDir?: number, lambdaL1?: number, lambdaPrior?: number, gradientClip?: number, earlyStopPatience?: object, maskRatio?: number, printEvery?: object, trainRatio?: number, validationRatio?: number, seed?: object, significanceZScore?: number, cascadeSteps?: object, env?: object): object;
   /**
    * Train the GPS model and return it together with the training diagnostics.
    * 
    * > Training runs in two stages: the masked reconstruction pre-training stage, in which the
    * >  last time frame of a random subset of genes is masked, followed by the next-step
    * >  prediction fine-tuning stage, which uses the autofocus loss plus the direction aware
    * >  loss. The best validation parameters are restored when early stopping triggers.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  The evaluation metrics are computed on the time ordered test windows, and the
    * >  "persistence" baseline (the last observed frame is used as the prediction) is reported
    * >  side by side, so the contribution of the graph and the attention can be judged. When the
    * >  model has not surpassed that baseline yet, an explanatory note is appended to the report;
    * >  increasing ``finetune_epochs`` usually helps.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  The returned object carries four attributes: ``training``
    * >  ([GPSTrainingResult](cref:T:SMRUCC.genomics.Analysis.GPS.Training.GPSTrainingResult) with the loss curves), ``metrics``
    * >  ([EvaluationMetrics](cref:T:SMRUCC.genomics.Analysis.GPS.Training.EvaluationMetrics) of the model), ``baseline``
    * >  ([EvaluationMetrics](cref:T:SMRUCC.genomics.Analysis.GPS.Training.EvaluationMetrics) of the persistence baseline) and ``report``
    * >  (the formatted multi line text).
    * 
     * @param gps the untrained model created by ``gps::build_gps``
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return the trained model object with the ``training``, ``metrics``, ``baseline`` and ``report``
     *  attributes attached; an R# error object is returned instead when the training fails or
     *  when the produced loss curve is not finite.
   */
   function train(gps: object, env?: object): object;
}
