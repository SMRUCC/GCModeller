// export R# package module type define for javascript/typescript language
//
//    imports "FuzzyGene" from "biosystem";
//
// ref=biosystem.fuzzyGene@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * FuzzyGene: the neuro-fuzzy (Wilson/Takagi-Sugeno ANFIS) gene regulatory network toolkit.
 * 
 * > This R# package module wraps the ``FuzzyGene`` sub-system. Every target gene is described by
 * >  its own first-order Takagi-Sugeno fuzzy model whose inputs are 3-6 candidate regulators;
 * >  the rules of that model can be read as biological statements ("IF TF-A is High AND
 * >  TF-B is Low THEN gene-G is activated"), and the analytic partial derivatives of the model
 * >  give a signed, weighted regulatory edge table.
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
 * > - ``fuzzygene_config``: create the hyper-parameter configuration object
 * >  ([FuzzyGeneConfig](cref:T:SMRUCC.genomics.Analysis.FuzzyGene.Config.FuzzyGeneConfig), registered as the
 * >  ``fuzzygene_config`` R# type);
 * > - ``build_fuzzygene``: build an untrained model from a gene expression matrix and an optional
 * >  prior regulatory network;
 * > - ``train``: fit every target gene (static route, or the lagged `x(t+1) = F(x(t))` route
 * >  when the matrix carries multiple time points);
 * > - ``fuzzygene_rules`` / ``fuzzygene_edges``: export the interpretable IF-THEN rules and the
 * >  signed regulatory edge table;
 * > - ``fuzzygene_trace_perturbation`` / ``fuzzygene_export_trace``: record the whole-genome
 * >  steady-state trajectory of one virtual perturbation (mutant and wildtype), either as two
 * >  matrices in memory or as CSV + ``SampleInfo`` sidecar files on disk;
 * > - ``fuzzygene_trace_convergence``: the solver diagnostics of that run (iteration vs
 * >  ``max_delta``), which tells whether the flat tail of a curve is a real steady state;
 * > - ``fuzzygene_rank_perturbations`` / ``fuzzygene_strongest_perturbation``: rank candidate
 * >  perturbations by predicted response strength;
 * > - ``fuzzygene_hybrid_config``: chainable mutator of the solver (Newton / Anderson / legacy /
 * >  advance) and the hybrid layers (PBN regularisation, ensemble samples, FCM warm start,
 * >  spectral radius calibration);
 * > - ``fuzzygene_pbn`` / ``fuzzygene_ensemble``: the qualitative probabilistic boolean layer
 * >  (CoD table, selected rules) and the structural uncertainty of a perturbation (mean, standard
 * >  deviation, confidence interval, sign agreement, boolean attractor periods);
 * > - ``fuzzygene_modules``: the coarse grained fuzzy cognitive map layer (module assignment,
 * >  bounded module steady state, per gene warm start and surrogate);
 * > - ``fuzzygene_solver_report``: exit reason, free residual, constraint violation, saturation
 * >  fractions and attractor period of a single perturbation, which separates "converged" from
 * >  "ran out of steps" / "oscillates" / "hit the support bounds";
 * > - ``fuzzygene_relaxation``: change the damping factor / iteration budget of the damped
 * >  fallback, which is what you want when a genome scale model keeps oscillating instead of
 * >  converging;
 * > 
 * > 
 * >  
 * > 
 * > 
 * >  A trained [FuzzyGeneModel](cref:T:SMRUCC.genomics.Analysis.FuzzyGene.FuzzyGeneModel) implements the
 * >  `InsilicoPerturbationExperiment` interface, so the ``knockouts``, ``knockdown`` and
 * >  ``overexpress`` api of the ``bnlearn`` package can be applied to it directly for in silico
 * >  perturbation experiments (partial knockdown is natively supported because the fuzzy
 * >  membership functions describe graded regulation). The generic ``writeBin`` handler of this
 * >  module persists a model into a zip based binary stream, and ``readBin.fuzzygene`` restores
 * >  it back to a fully usable model object.
*/
declare namespace FuzzyGene {
   /**
    * Build an untrained neuro-fuzzy gene regulatory network model.
    * 
    * > The expression matrix provides the training data: when it carries ``TimePoints`` with at
    * >  least three distinct values the model learns the lagged map `x(t+1) = F(x(t))`,
    * >  otherwise it learns the static map `x_g = F(regulators)`. The prior network supplies
    * >  TF-to-target edges that are used to order (not to force) the candidate inputs.
    * 
     * @param x the gene expression matrix: either the ``Matrix`` object returned by
     *  ``geneExpression::load.expr`` (rows = genes, columns = samples) or an already converted
     *  ``GeneExpressionData``. Both are accepted because the plain ``load.expr`` result is a
     *  ``Matrix``; the old signature only took ``GeneExpressionData``, so every script that piped
     *  ``load.expr()`` straight into ``build_fuzzygene`` failed with an InvalidCastException.
     * @param prior an optional prior regulatory network created by the ``bnlearn`` package (``prior_edges``);
     *  when omitted the candidate regulators are selected by correlation only.
     * 
     * + default value Is ``null``.
     * @param config the configuration created by ``FuzzyGene::fuzzygene_config``; ``Nothing`` uses the defaults.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return an untrained model object
   */
   function build_fuzzygene(x: any, prior?: object, config?: object, env?: object): object;
   /**
    * Run every analysis once and write the tables to a folder.
    * 
    * > Writes into the folder:
    * >  ``analysis.sensitivity.csv`` (omega + step sweeps),
    * >  ``analysis.lag.csv``, ``analysis.spearman.csv``,
    * >  ``analysis.dose_response.csv``, ``analysis.dose_time_surface.csv``,
    * >  ``analysis.combination.csv`` (when both genes are given) and
    * >  ``analysis.report.json`` (grid definition, invariant flags, run count, elapsed time).
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  This is the cheap way to get everything: calling the four analysis apis separately would
    * >  repeat the dose sweep twice. Expect one to two minutes on an 18k gene model with the
    * >  default grids.
    * >  
    * > 
    * > 
    * >  
    * > ```
    * >  let files = FuzzyGene::fuzzygene_analysis_report(
    * >      model, "TPPP3", geneA = "HSD17B3", geneB = "TPPP3", dir = "/path/analysis");
    * >  print(files);
    * >  
    * > ```
    * 
     * @param fuzzygene a trained (or restored) model
     * @param gene the target gene of the single perturbation analyses
     * @param geneA the first gene of the combination analysis; empty skips that part
     * 
     * + default value Is ``null``.
     * @param geneB the second gene of the combination analysis; empty skips that part
     * 
     * + default value Is ``null``.
     * @param dir the output folder (created when missing)
     * 
     * + default value Is ``null``.
     * @param mode ``knockout`` (default), ``knockdown`` or ``overexpression``
     * 
     * + default value Is ``'knockout'``.
     * @param omegas relaxation grid; ``NULL`` uses ``c(0.2, 0.5, 0.8, 1.0)``
     * 
     * + default value Is ``null``.
     * @param stepsList step grid of the invariance check; ``NULL`` uses ``c(50, 200, 500)``
     * 
     * + default value Is ``null``.
     * @param doses dose grid; ``NULL`` uses 5 evenly spaced points
     * 
     * + default value Is ``null``.
     * @param sweepSteps fixed iteration budget of the omega / lag analyses
     * 
     * + default value Is ``50``.
     * @param doseSteps iteration budget of the dose analyses
     * 
     * + default value Is ``60``.
     * @param combinationSteps iteration budget of the combination analysis
     * 
     * + default value Is ``60``.
     * @param top number of genes kept in the dose and combination tables
     * 
     * + default value Is ``50``.
     * @param surfaceTop number of genes kept in the dose-time surface
     * 
     * + default value Is ``20``.
     * @param lagRatio relative trigger threshold of the lag definition
     * 
     * + default value Is ``0.1``.
     * @param tolerance relative deviation threshold of the invariant flags
     * 
     * + default value Is ``0.001``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a character vector of the written file paths
   */
   function fuzzygene_analysis_report(fuzzygene: object, gene: string, geneA?: string, geneB?: string, dir?: string, mode?: string, omegas?: number, stepsList?: number, doses?: number, sweepSteps?: object, doseSteps?: object, combinationSteps?: object, top?: object, surfaceTop?: object, lagRatio?: number, tolerance?: number, env?: object): string;
   /**
    * Non-additive (synergy / antagonism) analysis of a two gene combination.
    * 
    * > Additive expectation is `control + (A - control) + (B - control)`; the non additive
    * >  effect is `joint - additive`. Positive means synergy, negative means antagonism.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  Returns a named list:
    * >  
    * > 
    * > 
    * >  
    * > - ``summary``: one row with the two genes and ``max_abs_non_additive``;
    * > - ``table``: the full 18k gene table ``gene`` / ``single_a`` / ``single_b`` /
    * >  ``joint`` / ``additive`` / ``non_additive``;
    * > - ``top``: the top genes ranked by ``|non_additive|``;
    * > - ``trajectory``: the frame level version ``iteration`` / ``gene`` /
    * >  ``additive`` / ``joint`` / ``non_additive``, which shows whether the deviation appears
    * >  together with the single perturbations or only later.
    * 
     * @param fuzzygene a trained (or restored) model
     * @param geneA the first target gene
     * @param geneB the second target gene; must differ from ``geneA``
     * @param mode ``knockout`` (default), ``knockdown`` or ``overexpression``
     * 
     * + default value Is ``'knockout'``.
     * @param steps iteration budget of each solve
     * 
     * + default value Is ``60``.
     * @param top number of genes kept in ``top`` and ``trajectory``
     * 
     * + default value Is ``50``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a named list of data.frames
   */
   function fuzzygene_combination(fuzzygene: object, geneA: string, geneB: string, mode?: string, steps?: object, top?: object, env?: object): object;
   /**
    * Export the confidence matrix of a trained model (same shape as the weight matrix).
    * 
    * > Rows and columns are exactly the same as in ``fuzzygene_regulation_matrix`` (same layout,
    * >  same order), only the element value changes: `W[i, j]` becomes the confidence of the
    * >  edge `j -> i`, that is the prior confidence when the edge is supported by the prior
    * >  network and the absolute expression correlation `|Pearson|` otherwise.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  Weight and confidence answer two different questions ("how strong / which direction" vs
    * >  "how trustworthy"), so exporting both matrices and using weight for the edge width and
    * >  confidence for the edge opacity or a cutoff is the usual combination:
    * >  
    * > ```
    * >  let w = FuzzyGene::fuzzygene_regulation_matrix(model, layout = "compact");
    * >  let c = FuzzyGene::fuzzygene_confidence_matrix(model, layout = "compact");
    * >  geneExpression::write.expr_matrix(w, file = "/path/weight.csv", id = "geneID");
    * >  geneExpression::write.expr_matrix(c, file = "/path/confidence.csv", id = "geneID");
    * >  
    * > ```
    * 
     * @param fuzzygene a trained model
     * @param layout ``square`` (default), ``regulators`` or ``compact``
     * 
     * + default value Is ``'square'``.
     * @param minConfidence edges with a smaller confidence are exported as zero; 0 keeps all
     * 
     * + default value Is ``0``.
     * @param genes restrict the exported rows to these target genes; empty means all
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a Matrix object (rows = targets, columns = regulators, non negative values)
   */
   function fuzzygene_confidence_matrix(fuzzygene: object, layout?: string, minConfidence?: number, genes?: string, env?: object): object;
   /**
    * Create a FuzzyGene modelling configuration object.
    * 
    * > The returned object (registered as the ``fuzzygene_config`` R# type) is accepted by the
    * >  ``config`` argument of ``FuzzyGene::build_fuzzygene``. Structure and training options are
    * >  inherited from the shared ANFIS configuration; this api exposes the subset that matters for
    * >  gene regulatory network inference.
    * 
     * @param maxRegulators how many candidate regulators (ANFIS inputs) are kept per target gene. The rule count of a
     *  grid partition is `m^n`, so this is the primary guard against the rule explosion;
     *  3-6 is the practically useful range. Default 4.
     * 
     * + default value Is ``4``.
     * @param selection regulator selection strategy: ``prior-first`` (default, prior edges by confidence then
     *  correlation), ``correlation`` (data driven only) or ``prior`` (prior edges only).
     * 
     * + default value Is ``'prior-first'``.
     * @param minCorrelation minimum |Pearson| for a gene to be taken as a correlation based regulator. Default 0.3.
     * 
     * + default value Is ``0.3``.
     * @param mode fit route: ``auto`` (default, lagged when the matrix has at least ``minTimePoints`` distinct
     *  time points), ``static`` or ``lagged``.
     * 
     * + default value Is ``'auto'``.
     * @param mfCount number of membership functions per input. Default 3.
     * 
     * + default value Is ``3``.
     * @param mfKind membership function shape: ``gaussian``, ``bell``, ``sigmoid``, ``triangular`` or
     *  ``trapezoidal``. Default ``gaussian``.
     * 
     * + default value Is ``'gaussian'``.
     * @param maxEpoch maximum training epochs per target gene. Default 100.
     * 
     * + default value Is ``100``.
     * @param learningRate learning rate of the premise parameter optimizer. Default 0.02.
     * 
     * + default value Is ``0.02``.
     * @param pruneThreshold rules whose average normalized firing strength falls below this value are pruned after
     *  training; 0 disables pruning. Default 0.01.
     * 
     * + default value Is ``0.01``.
     * @param maxGenes upper bound on the number of modelled genes (highest variance first, prior TFs always kept);
     *  0 keeps everything. Default 2000.
     * 
     * + default value Is ``2000``.
     * @param maxTrainRows maximum number of training rows per target gene; 0 uses every row. The dominant cost of the
     *  hybrid learning is the least squares normal equations, which scale as
     *  `rows x P^2`, so capping the rows is the cheapest large speedup: a few thousand rows
     *  are already far more than the few hundred consequent parameters need.
     *  Default 0 (all rows); 3000-5000 is the recommended setting for genome scale data.
     * 
     * + default value Is ``0``.
     * @param maxCandidateGenes size limit of the candidate pool used by the correlation screening
     *  (prior TFs plus the highest variance genes); 0 screens every gene. The screening cost is
     *  `O(genes^2 x rows)`, which at genome scale becomes comparable with the training itself.
     *  Default 0; 2000-4000 is the recommended setting for genome scale data.
     * 
     * + default value Is ``0``.
     * @param minBestCorrelation skip a target gene when it has no prior regulator and its best |Pearson| against the
     *  candidates is below this value (such models have R2 close to 0). 0 disables the filter.
     * 
     * + default value Is ``0``.
     * @param noveltyQuota fraction (0-1) of the regulator slots reserved for genes that are **not** in the prior
     *  network of that target, i.e. for links discovered from the expression data.
     *  ``0`` (default) keeps the old "prior first" behaviour, where prior TFs can occupy every
     *  slot and the model can only repeat the prior. ``0.2``-``0.4`` is a sensible range:
     *  with K = 6 and ``0.34`` two slots are always given to data driven regulators.
     *  Ignored when ``selection = "prior-only"``.
     * 
     * + default value Is ``0``.
     * @param rulePartition rule generation strategy: ``auto`` (default), ``grid`` or ``scatter``.
     *  ``grid`` places m membership functions per input and enumerates `m^K` rules, so the rule
     *  count explodes with the input count. ``scatter`` clusters the training samples (kmeans) and
     *  creates one rule per cluster, each with its own gaussian center/width per input - the number of
     *  rules no longer depends on K, which is what makes ``maxRegulators = 18`` feasible.
     *  ``auto`` keeps ``grid`` while `m^K <= maxRules` and `m^K*(K+1) <= maxConsequentParams`,
     *  otherwise it switches to ``scatter``.
     * 
     * + default value Is ``'auto'``.
     * @param scatterRules number of clusters (= rules) of the scatter partition. Default 48.
     * 
     * + default value Is ``48``.
     * @param scatterInputs maximum number of antecedents per scatter rule. Default 8.
     * 
     * + default value Is ``8``.
     * @param scatterMinInputs minimum number of antecedents per scatter rule. Default 2.
     * 
     * + default value Is ``2``.
     * @param scatterZThreshold an input joins a cluster rule when the cluster center deviates from the global mean by at
     *  least this many standard deviations. Default 1.
     * 
     * + default value Is ``1``.
     * @param scatterSeed random seed of the clustering, keeps the structure reproducible. Default 0.
     * 
     * + default value Is ``0``.
     * @param maxRules rule budget per target gene. Default 128.
     * 
     * + default value Is ``128``.
     * @param maxConsequentParams consequent parameter budget per target gene. Default 512.
     * 
     * + default value Is ``512``.
     * @param standardize z-score the expression matrix before fitting. Default TRUE.
     * 
     * + default value Is ``true``.
     * @param propagationSteps maximum fixed point iterations of a perturbation. Default 50.
     * 
     * + default value Is ``50``.
     * @param relaxation damping factor of the fixed point iteration. Default 0.5.
     * 
     * + default value Is ``0.5``.
     * @param knockdownResidual fraction of the baseline expression retained after a virtual knockdown. Default 0.2.
     * 
     * + default value Is ``0.2``.
     * @param overexpressionSigmas overexpression level in training standard deviations above the baseline. Default 3.
     * 
     * + default value Is ``3``.
     * @param significanceZScore |z| threshold used to flag a gene as significantly changed. Default 2.
     * 
     * + default value Is ``2``.
     * @param degreeOfParallelism per-gene training parallelism; 0 = all cores, 1 = serial.
     * 
     * + default value Is ``0``.
     * @param minTimePoints distinct time points required to switch to the lagged route. Default 3.
     * 
     * + default value Is ``3``.
     * @param env the R# runtime environment (unused, kept for error reporting symmetry)
     * 
     * + default value Is ``null``.
     * @return a validated configuration object
   */
   function fuzzygene_config(maxRegulators?: object, selection?: string, minCorrelation?: number, mode?: string, mfCount?: object, mfKind?: string, maxEpoch?: object, learningRate?: number, pruneThreshold?: number, maxGenes?: object, maxTrainRows?: object, maxCandidateGenes?: object, minBestCorrelation?: number, noveltyQuota?: number, rulePartition?: string, scatterRules?: object, scatterInputs?: object, scatterMinInputs?: object, scatterZThreshold?: number, scatterSeed?: object, maxRules?: object, maxConsequentParams?: object, standardize?: boolean, propagationSteps?: object, relaxation?: number, knockdownResidual?: number, overexpressionSigmas?: number, significanceZScore?: number, degreeOfParallelism?: object, minTimePoints?: object, env?: object): object;
   /**
    * Inject a transcriptome context as the initial condition of the virtual perturbation.
    * 
    * > The context replaces the control baseline: both the wildtype and the mutant state are
    * >  propagated **from the given condition**, so ``foldchange`` is read as
    * >  "mutant vs wildtype inside this context" instead of "mutant vs the training mean".
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  The table (csv or tsv) must carry at least one gene column and one expression column.
    * >  All non-numeric columns are treated as candidate gene keys and the one that matches the
    * >  most model genes is used, so a ``gene_name,gene_id,mean_counts`` pseudobulk table works
    * >  out of the box. Rows whose expression value is not a finite number are skipped (never
    * >  silently turned into zero), and model genes that are absent from the table keep the
    * >  training baseline.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  Note that a fitted static map is usually contractive: after it converges, the state no
    * >  longer depends on where the propagation started. The context therefore matters most for
    * >  (i) the reference state used by fold changes, (ii) non-converged / transient responses
    * >  and (iii) lagged (time series) models.
    * 
     * @param fuzzygene a trained model
     * @param context the context table path (csv/tsv) with a gene column and an expression column
     * @param valueColumn the name of the expression column; empty uses the last column of the table
     * 
     * + default value Is ``null``.
     * @param minCoverage the minimum fraction (0-1) of model genes that must be covered by the table,
     *  0 disables the check
     * 
     * + default value Is ``0``.
     * @param align 
     * + default value Is ``null``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a multi line coverage summary, or an R# error object when the context is rejected
   */
   function fuzzygene_context(fuzzygene: object, context: any, valueColumn?: string, minCoverage?: number, align?: object, env?: object): string;
   /**
    * Inject the result of an **upstream perturbation** as the control baseline (iterative perturbation).
    * 
    * > After this call the mutant state of the upstream experiment becomes the reference state of the
    * >  model, so the next perturbation reports "effect of B given that A is already knocked out"
    * >  instead of "effect of B from the original baseline".
    * >  
    * > ```
    * >  let round1 = model |> knockouts(c("A"));           # A 失活
    * >  print(FuzzyGene::fuzzygene_context_upstream(model, round1[[1]]));
    * >  let round2 = model |> knockouts(c("B"));           # 在 A 失活的背景上敲 B
    * >  
    * > ```
    * > 
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  ``useMutant = TRUE`` (default) takes ``MutantMeans``; ``FALSE`` takes ``WildtypeMeans``, which
    * >  restores the reference frame of the upstream round. An upstream result flagged as
    * >  ``Undefined`` (the target gene was not modelled) is rejected instead of being silently applied.
    * 
     * @param fuzzygene a trained model
     * @param upstream an `InterventionResult`, e.g. one element of `knockouts(...)`
     * @param useMutant take the mutant (perturbed) state instead of the wildtype one. Default TRUE.
     * 
     * + default value Is ``true``.
     * @param minCoverage minimum fraction (0-1) of model genes the upstream result must cover; 0 disables the check
     * 
     * + default value Is ``0``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a multi line coverage summary, or an R# error object when the context is rejected
   */
   function fuzzygene_context_upstream(fuzzygene: object, upstream: object, useMutant?: boolean, minCoverage?: number, env?: object): string;
   /**
    * Dose-response curve of one virtual perturbation.
    * 
    * > Dose `d` runs from 0 (pin the gene at its baseline) to 1 (full knockout); the clamped
    * >  value is `baseline x (1 - d)`. Note that `d = 0` is not the same as "no
    * >  perturbation": the gene is held fixed while the wildtype trajectory drifts away from the
    * >  baseline, which is why its own fold change is 0 at `d = 0` but other genes can still
    * >  move.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  Only the top genes are returned (ranked by ``|z|`` at the largest dose) to keep the
    * >  data.frame small; pass ``top = 0`` for all 18k genes.
    * >  
    * > 
    * > 
    * >  
    * > ```
    * >  let r = FuzzyGene::fuzzygene_dose_response(model, "TPPP3", doses = c(0, 0.5, 1));
    * >  print(r);
    * >  
    * > ```
    * 
     * @param fuzzygene a trained (or restored) model
     * @param gene the target gene of the virtual perturbation
     * @param doses dose points in [0, 1]; ``NULL`` uses 5 evenly spaced points
     * 
     * + default value Is ``null``.
     * @param steps iteration budget of each solve
     * 
     * + default value Is ``60``.
     * @param top number of genes to return; 0 means all
     * 
     * + default value Is ``50``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a long data.frame ``dose`` / ``clamp_value`` / ``gene`` / ``fold_change`` / ``percent_change`` / ``z_score`` / ``significant``
   */
   function fuzzygene_dose_response(fuzzygene: object, gene: string, doses?: number, steps?: object, top?: object, env?: object): object;
   /**
    * Dose x iteration response surface of one virtual perturbation.
    * 
    * > Every cell is `mutant(t) - control(t)` at the same frame, i.e. the drift of the system
    * >  itself has already been subtracted. Only the top genes (ranked by ``|z|`` at the largest
    * >  dose) are kept, so the cube stays small enough to plot as a contour or a 3D surface.
    * 
     * @param fuzzygene a trained (or restored) model
     * @param gene the target gene of the virtual perturbation
     * @param doses dose points in [0, 1]; ``NULL`` uses 5 evenly spaced points
     * 
     * + default value Is ``null``.
     * @param steps iteration budget of each solve
     * 
     * + default value Is ``60``.
     * @param top number of genes to keep
     * 
     * + default value Is ``20``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a long data.frame ``dose`` / ``iteration`` / ``gene`` / ``delta``
   */
   function fuzzygene_dose_time_surface(fuzzygene: object, gene: string, doses?: number, steps?: object, top?: object, env?: object): object;
   /**
    * Export the signed regulatory edge table of a trained model.
    * 
    * > The edge weight is the average analytic partial derivative
    * >  `∂x_target/∂x_regulator` over the training support: positive means activation,
    * >  negative means repression. ``confidence`` is the prior confidence when the edge is
    * >  supported by the prior network, otherwise the absolute expression correlation.
    * 
     * @param fuzzygene a trained model
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a data.frame with columns source, target, weight, regulation, confidence and prior
   */
   function fuzzygene_edges(fuzzygene: object, env?: object): object;
   /**
    * Ensemble (resampled boolean structure) prediction of virtual perturbations.
    * 
    * > Each sample rebuilds the network by drawing one rule per target gene from the
    * >  probabilistic boolean network according to its CoD derived probability, and then solves
    * >  the continuous perturbation again. The spread of the resulting states therefore measures
    * >  **structural uncertainty** (which regulatory logic the data supports), not numerical noise.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  Returns a named list:
    * >  
    * > 
    * > 
    * >  
    * > - ``table``: one row per (perturbation, gene) with ``mean`` / ``sd`` /
    * >  ``lower`` (5%) / ``upper`` (95%) / ``sign_agreement`` / ``high_fraction`` / ``reference``;
    * > - ``summary``: one row per perturbation with ``samples`` /
    * >  ``converged_fraction`` / ``degenerate`` / ``notes``;
    * > - ``attractors``: the boolean attractor period distribution
    * >  (``perturbation`` / ``period`` / ``count``), zero rows when no cycle was detected.
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  ``sign_agreement`` is the single most useful column: about 0.5 means the direction of that
    * >  gene's response is decided by the model noise, so it must not be reported as a finding.
    * 
     * @param fuzzygene a trained model built with ``pbnRegularization != "none"``
     * @param genes the genes to perturb
     * @param mode ``knockout`` (default), ``knockdown`` or ``overexpression``
     * 
     * + default value Is ``'knockout'``.
     * @param samples how many PBN resamples; <= 0 falls back to the configured value
     * 
     * + default value Is ``16``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a named list of data.frames
   */
   function fuzzygene_ensemble(fuzzygene: object, genes: string, mode?: string, samples?: object, env?: object): object;
   /**
    * Export the two trajectory matrices of one virtual perturbation to a folder.
    * 
    * > Writes, into **dir**:
    * >  
    * > 
    * > 
    * >  
    * > - ``trace.mutant.csv`` / ``trace.wildtype.csv``: gene x iteration matrices;
    * > - ``*.sampleinfo.json``: one `SampleInfo` row per column (``time`` / ``iteration``
    * >  / ``omega`` / ``converged`` / ``perturbed_genes`` ...), ready for the ExperimentDesigner
    * >  pipeline;
    * > - ``trace.perturbation.json``: the run summary plus the whole ``max_delta`` sequence.
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  ``dt`` only affects ``metadata$time`` of the sidecar (`time = iteration * dt`); it does
    * >  not change the matrix. Leave it at 1 unless the model is a lagged (time series) model and
    * >  the sampling interval is known.
    * >  
    * > 
    * > 
    * >  
    * > ```
    * >  let files = FuzzyGene::fuzzygene_export_trace(model, "TPPP3", "/path/trace", mode = "knockout");
    * >  print(files);
    * >  
    * > ```
    * 
     * @param fuzzygene a trained (or restored) model
     * @param gene the target gene of the virtual perturbation
     * @param dir the output folder (created when missing)
     * @param mode ``knockout`` (default), ``knockdown`` or ``overexpression``
     * 
     * + default value Is ``'knockout'``.
     * @param steps maximum iteration steps; 0 uses the model configuration
     * 
     * + default value Is ``0``.
     * @param dt time unit of one iteration frame, used by the sidecar only
     * 
     * + default value Is ``1``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a character vector of the written file paths
   */
   function fuzzygene_export_trace(fuzzygene: object, gene: string, dir: string, mode?: string, steps?: object, dt?: number, env?: object): string;
   /**
    * Chainable mutator of the solver and hybrid layer options.
    * 
    * > The existing ``fuzzygene_config`` already carries 30 optional arguments; adding the
    * >  solver / PBN / FCM knobs there would make it unusable and would break every script that
    * >  relies on positional arguments. This function mutates a configuration object in place
    * >  instead, so the whole thing stays readable:
    * >  
    * > 
    * > 
    * >  
    * > ```
    * >  let opts = FuzzyGene::fuzzygene_config(maxRegulators = 6, mfCount = 3);
    * >  opts = FuzzyGene::fuzzygene_hybrid_config(opts,
    * >      solver = "newton", pbnRegularization = "weighted", ensembleSamples = 16);
    * >  
    * > ```
    * > 
    * >  
    * > 
    * > 
    * >  Defaults are chosen so that calling this function with only ``config`` changes nothing
    * >  important: the Newton root finder is already the default solver, and the stability
    * >  calibration stays off (the root finder does not need a contractive map, while calibrating
    * >  a strongly cyclic fit can drop R2 from 0.86 to 0.014 — that has to be an explicit choice).
    * 
     * @param config the configuration created by ``fuzzygene_config``
     * @param solver ``newton`` (default), ``anderson``, ``legacy`` (the old damped iteration) or ``advance``
     *  (lagged/time series only: plain time stepping, no steady state claim)
     * 
     * + default value Is ``'newton'``.
     * @param maxNewtonIterations 
     * + default value Is ``20``.
     * @param andersonDepth 
     * + default value Is ``5``.
     * @param calibration ``none`` (default), ``spectral`` (scale the cyclic part until the spectral radius of the
     *  assembled Jacobian is below ``1 - epsilon``) or ``row-l1`` (the conservative row norm bound)
     * 
     * + default value Is ``'none'``.
     * @param calibrationEpsilon target margin ``1 - epsilon`` of the spectral radius. Default 0.05.
     * 
     * + default value Is ``0.05``.
     * @param calibrationMinScale lower bound of one calibration pass, keeps the model from collapsing. Default 0.05.
     * 
     * + default value Is ``0.05``.
     * @param clampInputToSupport clamp the antecedent inputs to the training domain of each regulator. Turning this off
     *  re-enables the unbounded extrapolation of the outermost rules. Default TRUE.
     * 
     * + default value Is ``true``.
     * @param clampOutputToSupport clamp the predicted expression to the training support. Default TRUE.
     * 
     * + default value Is ``true``.
     * @param maxRegulatorCollinearity upper bound of the pairwise |Pearson| between the selected regulators; 1 disables the
     *  constraint (default). Lowering it to 0.95 improves the conditioning of the least squares
     *  step at the price of selecting fewer data driven regulators.
     * 
     * + default value Is ``1``.
     * @param pbnRegularization ``none`` (default), ``filter`` (drop correlation candidates whose CoD is too low) or
     *  ``weighted`` (order the candidates by CoD). Enabling it also makes
     *  ``fuzzygene_ensemble`` available.
     * 
     * + default value Is ``'none'``.
     * @param pbnMinCod minimum CoD for a candidate parent set. Default 0.05.
     * 
     * + default value Is ``0.05``.
     * @param pbnWeight weight of the CoD term when ``pbnRegularization = "weighted"``. Default 1.
     * 
     * + default value Is ``1``.
     * @param pbnMaxParents maximum parents enumerated per target gene. Default 2.
     * 
     * + default value Is ``2``.
     * @param pbnMaxGenes gene limit of the qualitative layer; above it the PBN layer is skipped. Default 2000.
     * 
     * + default value Is ``2000``.
     * @param pbnCandidateSize candidate pool size per target gene (prior TFs plus the top correlated genes). Default 5.
     * 
     * + default value Is ``5``.
     * @param ensembleSamples default sample count of ``fuzzygene_ensemble``: how many times the PBN rules are resampled.
     *  0 (default) keeps the deterministic single solve.
     * 
     * + default value Is ``0``.
     * @param modeBiasWeight how strongly a sampled boolean mode pulls the continuous solve; 0 (default) only reports
     *  the boolean consistency without changing the numerical result.
     * 
     * + default value Is ``0``.
     * @param fcmWarmStart enable the module level fuzzy cognitive map: it provides a bounded warm start for the
     *  per gene root finding and a surrogate answer when the solver detects a limit cycle.
     *  Default FALSE.
     * 
     * + default value Is ``false``.
     * @param fcmModules number of modules of the coarse grained layer. Default 48.
     * 
     * + default value Is ``48``.
     * @param fcmAlpha slope of the sigmoid activation. Default 1.
     * 
     * + default value Is ``1``.
     * @param fcmMaxSteps propagation budget of the module layer. Default 200.
     * 
     * + default value Is ``200``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return the same configuration object, so the call can be chained
   */
   function fuzzygene_hybrid_config(config: object, solver?: string, maxNewtonIterations?: object, andersonDepth?: object, calibration?: string, calibrationEpsilon?: number, calibrationMinScale?: number, clampInputToSupport?: boolean, clampOutputToSupport?: boolean, maxRegulatorCollinearity?: number, pbnRegularization?: string, pbnMinCod?: number, pbnWeight?: number, pbnMaxParents?: object, pbnMaxGenes?: object, pbnCandidateSize?: object, ensembleSamples?: object, modeBiasWeight?: number, fcmWarmStart?: boolean, fcmModules?: object, fcmAlpha?: number, fcmMaxSteps?: object, env?: object): object;
   /**
    * The configuration attached to a model (identical to ``config.json`` inside the package).
    * 
    * > Useful to confirm what the model was actually fitted with, especially the rule budget
    * >  (``maxRules`` / ``maxConsequentParams``) and ``noveltyQuota``. Remember that
    * >  ``maxRegulators`` / ``mfCount`` are only upper bounds: the real structure is reduced by the
    * >  rule budget, which is reported on the ``structure:`` line of ``fuzzygene_report``.
    * 
     * @param fuzzygene a trained (or restored) model
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return the configuration object
   */
   function fuzzygene_model_config(fuzzygene: object, env?: object): object;
   /**
    * The coarse grained module layer (FCM): module assignment, bounded steady state and warm start.
    * 
    * > Returns a named list:
    * >  
    * > 
    * > 
    * >  
    * > - ``table``: ``gene`` / ``module`` / ``baseline``, the module assignment
    * >  computed at fit time (only present when ``fcmWarmStart = TRUE``);
    * > - ``modules``: ``module`` / ``n_genes`` / ``activation``, the bounded
    * >  activation of the module level fuzzy cognitive map on the training baseline;
    * > - ``initial``: ``gene`` / ``warm_start`` / ``surrogate``, the per gene
    * >  initial state derived from the module state (this is what the root finder starts from).
    * > - ``summary``: ``available`` / ``module_count`` / ``converged`` /
    * >  ``steps`` / ``period`` / ``notes``.
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  The module layer is bounded by construction, so it always terminates with either a fixed
    * >  point or a detected limit cycle. It is a **rough** skeleton: the module average erases the
    * >  individual genes, which is why the default is off and the surrogate must never replace the
    * >  per gene prediction.
    * 
     * @param fuzzygene a trained model built with ``fcmWarmStart = TRUE``
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a named list of data.frames
   */
   function fuzzygene_modules(fuzzygene: object, env?: object): object;
   /**
    * The qualitative (probabilistic boolean) layer of a trained model: CoD table and rules.
    * 
    * > Returns a named list:
    * >  
    * > 
    * > 
    * >  
    * > - ``cod``: one row per (target, regulator) pair, columns ``target`` / ``regulator`` /
    * >  ``cod`` / ``rank``; ``cod`` is the coefficient of determination of that parent set on the
    * >  binarized matrix, i.e. how much of the boolean state of the target the regulator set
    * >  explains;
    * > - ``rules``: ``target`` / ``parents`` / ``n_parents`` / ``cod``, the selected rule of each
    * >  target gene (parents joined by ``+``);
    * > - ``targets``: ``target`` / ``rules`` / ``best_cod``, one row per analysed gene;
    * > - ``summary``: one row with ``has_pbn`` / ``targets_analysed`` / ``skipped`` / ``genes`` /
    * >  ``warnings``.
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  CoD is a **consistency** measure after binarization, not causal evidence: a rule can have
    * >  a high CoD simply because the target and the parent are both driven by a third gene.
    * >  Use it to suppress pure correlation edges and to report structure uncertainty, not as a
    * >  substitute for perturbation data.
    * 
     * @param fuzzygene a trained model that was built with ``pbnRegularization != "none"``
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a named list of data.frames
   */
   function fuzzygene_pbn(fuzzygene: object, env?: object): object;
   /**
    * Rank candidate perturbations by their predicted response strength.
    * 
    * > Running a perturbation for every gene of a genome scale model takes hours, so only
    * >  **candidates** genes (an evenly spaced sample of the modelled genes, no
    * >  random numbers involved) are evaluated.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  The score is the largest `|Z|` among the genes that were not clamped: the Z score
    * >  of the target itself is the imposed shift, not a model prediction. Note that with
    * >  ``clampToSupport`` enabled the largest `|Z|` often saturates at the same value for many
    * >  candidates; the ``affected`` column (number of significant genes) is the tie breaker, and
    * >  is a better ranking criterion in that case.
    * 
     * @param fuzzygene a trained (or restored) model
     * @param candidates number of candidate genes to evaluate; 0 means all modelled genes
     * 
     * + default value Is ``24``.
     * @param mode ``knockout`` (default), ``knockdown`` or ``overexpression``
     * 
     * + default value Is ``'knockout'``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a data.frame ``gene`` / ``max_abs_z`` / ``affected`` / ``elapsed_ms``, best first
   */
   function fuzzygene_rank_perturbations(fuzzygene: object, candidates?: object, mode?: string, env?: object): object;
   /**
    * Export the signed regulation weight matrix of a trained model.
    * 
    * > Rows are target genes, columns are regulators, and the element
    * >  `W[i, j]` is the signed influence of regulator `j` on target `i`
    * >  (the average analytic partial derivative `dx_target/dx_regulator`):
    * >  positive means activation, negative means repression, zero means no modelled edge.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  The result is a standard ``Matrix`` object, so it can be written directly with
    * >  ``geneExpression::write.expr_matrix()`` or fed into downstream network analysis:
    * >  
    * > ```
    * >  let w = FuzzyGene::fuzzygene_regulation_matrix(model, layout = "compact");
    * >  geneExpression::write.expr_matrix(w, file = "/path/to/network_weight.csv", id = "geneID");
    * >  
    * > ```
    * > 
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  Layouts: ``square`` (strict N x N, rows and columns are all model genes),
    * >  ``regulators`` (columns limited to genes that really appear as regulators) and
    * >  ``compact`` (rows = targets with at least one edge, columns = regulators).
    * >  For a genome scale model the square layout is `N^2` doubles
    * >  (about 2.7 GB for 18,533 genes), so prefer ``compact``/``regulators``
    * >  together with the ``genes`` row filter.
    * 
     * @param fuzzygene a trained model
     * @param layout ``square`` (default), ``regulators`` or ``compact``
     * 
     * + default value Is ``'square'``.
     * @param minWeight edges with a smaller absolute weight are exported as zero; 0 keeps all
     * 
     * + default value Is ``0``.
     * @param genes restrict the exported rows to these target genes; empty means all
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a Matrix object (rows = targets, columns = regulators)
   */
   function fuzzygene_regulation_matrix(fuzzygene: object, layout?: string, minWeight?: number, genes?: string, env?: object): object;
   /**
    * Set the damping factor, the iteration budget and the tolerance used by the steady-state solver.
    * 
    * > These three values belong to the damped fallback path
    * >  (`z <- z + omega * (F(z) - z)`). The default solver is a Levenberg-Marquardt root
    * >  finder for `F(z) - z = 0`, and it is used whenever it can reduce the residual; the
    * >  damping factor only matters when it falls back (or when
    * >  ``solver = "legacy"`` / ``"anderson"`` is selected through
    * >  ``fuzzygene_hybrid_config``). With ``omega = 1`` the fallback is the plain repeated
    * >  application of `F`, which on a genome scale model often keeps oscillating instead of
    * >  converging; ``omega = 0.5`` is the usual compromise.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  Always rerun the trajectory after changing these values, and check
    * >  ``summary$exit_reason``: only ``Converged`` means the flat tail is a real steady state;
    * >  ``MaxIterations`` is a truncated plateau and ``LimitCycle`` means the system oscillates.
    * >  
    * > 
    * > 
    * >  
    * > ```
    * >  model = FuzzyGene::fuzzygene_relaxation(model, omega = 0.5, steps = 80);
    * >  let t = FuzzyGene::fuzzygene_trace_perturbation(model, "TPPP3");
    * >  print(t$summary);
    * >  
    * > ```
    * 
     * @param fuzzygene a trained (or restored) model
     * @param omega damping factor in (0, 1]; 0.5 is the recommended value
     * 
     * + default value Is ``0.5``.
     * @param steps maximum iteration steps, must be positive
     * 
     * + default value Is ``50``.
     * @param tolerance convergence threshold on the per step max change, must be positive
     * 
     * + default value Is ``1E-06``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return the same model object, so the call can be chained
   */
   function fuzzygene_relaxation(fuzzygene: object, omega?: number, steps?: object, tolerance?: number, env?: object): object;
   /**
    * Human readable training report of a fitted model.
    * 
    * 
     * @param fuzzygene a trained model
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a multi line summary, or an R# error object when no training result is attached
   */
   function fuzzygene_report(fuzzygene: object, env?: object): string;
   /**
    * Drop the injected context and restore the training baseline.
    * 
    * 
     * @param fuzzygene a trained model
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return `True` on success
   */
   function fuzzygene_reset_context(fuzzygene: object, env?: object): boolean;
   /**
    * Export the IF-THEN rules of every fitted target gene.
    * 
    * > Each line reads like
    * >  `rule3 [12.34%] IF TF-A IS high AND TF-B IS low THEN gene-G = 1.2*TF-A + -0.8*TF-B + 0.1`：
    * >  the bracketed number is the average normalized firing strength on the training set, the
    * >  coefficients are the local linear conclusions of that rule.
    * 
     * @param fuzzygene a trained model
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a character vector, one element per rule (with per gene section headers)
   */
   function fuzzygene_rules(fuzzygene: object, env?: object): string;
   /**
    * Model self-check: omega invariance, step invariance and lag ordering robustness.
    * 
    * > Returns a named list:
    * >  
    * > 
    * > 
    * >  
    * > - ``omega``: data.frame ``value`` / ``converged`` / ``iterations`` /
    * >  ``last_max_delta`` / ``max_abs_deviation`` / ``max_relative_deviation``; the deviation
    * >  columns are measured against ``reference_omega`` (see ``summary``);
    * > - ``steps``: the same shape for the iteration budget sweep, measured
    * >  against the largest step count;
    * > - ``lag``: wide table ``gene`` + one column per omega, each cell is the
    * >  frame index at which that gene first reaches 10% (``lagRatio``) of its own final change;
    * > - ``spearman``: wide table of pairwise Spearman rho between the omega runs,
    * >  computed on the genes that respond under every omega;
    * > - ``summary``: one row data.frame with the two invariant flags, the
    * >  reference values, the responding gene count, the mean rho and the run count.
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  Read ``summary$omega_invariant`` first. ``FALSE`` means the end state still depends on the
    * >  damping factor, i.e. the iteration has not converged and the trajectory is not a steady
    * >  state. That is a property of the model, not a bug of this function.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  Cost: about ``2 x (length(omegas) + length(stepsList) + length(omegas))`` solves. With the
    * >  default grids on an 18k gene model this takes roughly one minute.
    * 
     * @param fuzzygene a trained (or restored) model
     * @param gene the target gene of the virtual perturbation
     * @param mode ``knockout`` (default), ``knockdown`` or ``overexpression``
     * 
     * + default value Is ``'knockout'``.
     * @param omegas relaxation values to sweep, e.g. ``c(0.2, 0.5, 0.8, 1.0)``
     * 
     * + default value Is ``null``.
     * @param stepsList iteration budgets to sweep, e.g. ``c(50, 200, 500)``
     * 
     * + default value Is ``null``.
     * @param sweepSteps fixed iteration budget used by the omega sweep and the lag analysis
     * 
     * + default value Is ``50``.
     * @param referenceOmega omega used as the comparison baseline; ``NaN`` picks the value closest to 0.5
     * 
     * + default value Is ``NaN``.
     * @param tolerance relative deviation threshold of the invariant flags
     * 
     * + default value Is ``0.001``.
     * @param lagRatio relative trigger threshold of the lag definition
     * 
     * + default value Is ``0.1``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a named list of data.frames
   */
   function fuzzygene_sensitivity(fuzzygene: object, gene: string, mode?: string, omegas?: number, stepsList?: number, sweepSteps?: object, referenceOmega?: number, tolerance?: number, lagRatio?: number, env?: object): object;
   /**
    * Solver diagnostics of one virtual perturbation (exit reason, residuals, saturation).
    * 
    * > One row with both the mutant and the wildtype run:
    * >  ``exit_reason`` / ``iterations`` / ``free_residual`` / ``clamp_residual`` /
    * >  ``out_of_support`` / ``pinned`` / ``attractor_period`` / ``omega`` / ``tolerance`` /
    * >  ``lagged`` / ``elapsed_ms``, plus the same fields prefixed with ``control_``.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  Read ``exit_reason`` first. Only ``Converged`` means the flat tail of the curve is a real
    * >  steady state; ``MaxIterations`` is a truncated plateau, ``LimitCycle`` means the system
    * >  oscillates (the period mean is the honest answer), ``SupportSaturated`` means the support
    * >  clamp, not the model, produced the numbers, and ``Diverged`` means there is no answer.
    * >  ``clamp_residual`` is a diagnostic only: the clamped gene is by construction not at a fixed
    * >  point of the map, so it never goes to zero and must not be used as a convergence test.
    * 
     * @param fuzzygene a trained (or restored) model
     * @param gene the target gene of the virtual perturbation
     * @param mode ``knockout`` (default), ``knockdown`` or ``overexpression``
     * 
     * + default value Is ``'knockout'``.
     * @param steps maximum iteration steps; 0 uses the model configuration
     * 
     * + default value Is ``0``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a one row data.frame
   */
   function fuzzygene_solver_report(fuzzygene: object, gene: string, mode?: string, steps?: object, env?: object): object;
   /**
    * The single gene whose perturbation produces the strongest predicted response.
    * 
    * 
     * @param fuzzygene a trained (or restored) model
     * @param candidates number of candidate genes to evaluate; 0 means all modelled genes
     * 
     * + default value Is ``24``.
     * @param mode ``knockout`` (default), ``knockdown`` or ``overexpression``
     * 
     * + default value Is ``'knockout'``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return the gene name, or an R# error object when nothing could be evaluated
   */
   function fuzzygene_strongest_perturbation(fuzzygene: object, candidates?: object, mode?: string, env?: object): string;
   /**
    * Solver diagnostics of one virtual perturbation, without building the two large matrices.
    * 
    * > Same solve as ``fuzzygene_trace_perturbation`` but only the ``convergence`` /
    * >  ``control_convergence`` / ``summary`` elements are returned. Use it when all you need is the
    * >  `log10(max_delta)` versus iteration plot: constructing the 18k x 250 matrices is the
    * >  expensive part.
    * 
     * @param fuzzygene a trained (or restored) model
     * @param gene the target gene of the virtual perturbation
     * @param mode ``knockout`` (default), ``knockdown`` or ``overexpression``
     * 
     * + default value Is ``'knockout'``.
     * @param steps maximum iteration steps; 0 uses the model configuration
     * 
     * + default value Is ``0``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a named list with ``convergence``, ``control_convergence`` and ``summary``
   */
   function fuzzygene_trace_convergence(fuzzygene: object, gene: string, mode?: string, steps?: object, env?: object): object;
   /**
    * Record the whole-genome steady-state trajectory of one virtual perturbation.
    * 
    * > Returns a named list:
    * >  
    * > 
    * > 
    * >  
    * > - ``mutant``: a Matrix (rows = all genes, columns = iteration frames ``mut_t000``...);
    * > - ``wildtype``: the same shape for the unperturbed baseline;
    * > - ``convergence``: a data.frame ``iteration`` / ``max_delta`` of the mutant run;
    * > - ``control_convergence``: the same for the wildtype run;
    * > - ``changes``: a data.frame ``gene`` / ``fold_change`` / ``percent_change`` / ``z_score`` /
    * >  ``significant`` of the final state;
    * > - ``summary``: a one row data.frame with the perturbation definition and the solver diagnostics.
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  ``mutant`` and ``wildtype`` are .NET `HTS.DataFrame.Matrix` objects (rows = genes,
    * >  columns = frames), not R matrices, so ``dim()`` returns 1 on them. Pass them straight to a
    * >  downstream consumer instead:
    * >  
    * > ```
    * >  let trace = FuzzyGene::fuzzygene_trace_perturbation(model, "TPPP3");
    * >  geneExpression::write.expr_matrix(trace$mutant, file = "/path/mutant.csv", id = "geneID");
    * >  
    * > ```
    * > 
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  The column index is a solver iteration, not biological time. The default solver
    * >  finds the constrained fixed point of ``F(z) - z = 0`` (Levenberg-Marquardt, see
    * >  ``fuzzygene_hybrid_config(solver = ...)``), so the frames are Newton steps and the number of
    * >  frames is small (single digit for a well posed network). Only when the solver falls back to
    * >  the damped iteration does ``omega`` shape the curve (``fuzzygene_relaxation``), and only a
    * >  lagged (time series) model in ``advance`` mode makes one frame equal to one sampling interval.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  Read ``convergence`` / ``summary$converged`` before interpreting the curve: a flat tail
    * >  produced by running out of ``steps`` is a truncated plateau, not a steady state.
    * >  
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  ``wildtype`` is the control run: it starts from the same baseline and is iterated to its own
    * >  fixed point, so ``foldchange`` is now "mutant fixed point minus wildtype fixed point" instead
    * >  of "mutant minus the training mean". The training mean is generally not a fixed point
    * >  of a nonlinear fuzzy map, so the previous definition mixed the perturbation effect with the
    * >  model's own drift; the wildtype trajectory still exposes that drift, which is worth checking
    * >  (a large drift means the fit is inconsistent with its own steady state).
    * 
     * @param fuzzygene a trained (or restored) model
     * @param gene the target gene of the virtual perturbation
     * @param mode ``knockout`` (default), ``knockdown`` or ``overexpression``
     * 
     * + default value Is ``'knockout'``.
     * @param steps maximum iteration steps; 0 uses the model configuration
     * 
     * + default value Is ``0``.
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a named list of matrices, data.frames and the run summary
   */
   function fuzzygene_trace_perturbation(fuzzygene: object, gene: string, mode?: string, steps?: object, env?: object): object;
   /**
    * Load a model from a ``*.fuzzygene.zip`` package.
    * 
    * 
     * @param file the package path
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a trained model object ready for inference and virtual perturbation
   */
   function load_fuzzygene(file: string, env?: object): object;
   /**
    * Save a trained model into a ``*.fuzzygene.zip`` package.
    * 
    * > ``writeBin`` is the generic R# way to do the same thing on a connection; this api is a
    * >  convenience wrapper for scripts that already have a file path.
    * 
     * @param fuzzygene a trained model
     * @param file the output package path
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return `True` when the package has been written
   */
   function save_fuzzygene(fuzzygene: object, file: string, env?: object): boolean;
   /**
    * Train the neuro-fuzzy model: one Takagi-Sugeno network per target gene.
    * 
    * > Training is per gene and therefore parallelizable; a gene that cannot be fitted (constant
    * >  expression, no candidate regulator, ill conditioned least squares) is recorded as skipped
    * >  or failed instead of aborting the whole run. Use ``FuzzyGene::fuzzygene_report`` to inspect
    * >  the per gene diagnostics.
    * 
     * @param fuzzygene the untrained model created by ``FuzzyGene::build_fuzzygene``
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return the trained model object, or an R# error object when training fails
   */
   function train(fuzzygene: object, env?: object): object;
}
