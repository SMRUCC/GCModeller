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
     * @param x the gene expression matrix (rows = genes, columns = samples)
     * @param prior an optional prior regulatory network created by the ``bnlearn`` package (``prior_edges``);
     *  when omitted the candidate regulators are selected by correlation only.
     * 
     * + default value Is ``null``.
     * @param config the configuration created by ``fuzzygene::fuzzygene_config``; ``Nothing`` uses the defaults.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment (unused)
     * 
     * + default value Is ``null``.
     * @return an untrained model object
   */
   function build_fuzzygene(x: object, prior?: object, config?: object, env?: object): object;
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
    * >  let w = fuzzygene::fuzzygene_regulation_matrix(model, layout = "compact");
    * >  let c = fuzzygene::fuzzygene_confidence_matrix(model, layout = "compact");
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
    * >  ``config`` argument of ``fuzzygene::build_fuzzygene``. Structure and training options are
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
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return a multi line coverage summary, or an R# error object when the context is rejected
   */
   function fuzzygene_context(fuzzygene: object, context: string, valueColumn?: string, minCoverage?: number, env?: object): string;
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
    * >  let w = fuzzygene::fuzzygene_regulation_matrix(model, layout = "compact");
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
    * >  or failed instead of aborting the whole run. Use ``fuzzygene::fuzzygene_report`` to inspect
    * >  the per gene diagnostics.
    * 
     * @param fuzzygene the untrained model created by ``fuzzygene::build_fuzzygene``
     * @param env the R# runtime environment used to report errors
     * 
     * + default value Is ``null``.
     * @return the trained model object, or an R# error object when training fails
   */
   function train(fuzzygene: object, env?: object): object;
}
