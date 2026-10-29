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
   function fuzzygene_config(maxRegulators?: object, selection?: string, minCorrelation?: number, mode?: string, mfCount?: object, mfKind?: string, maxEpoch?: object, learningRate?: number, pruneThreshold?: number, maxGenes?: object, maxTrainRows?: object, maxCandidateGenes?: object, minBestCorrelation?: number, maxRules?: object, maxConsequentParams?: object, standardize?: boolean, propagationSteps?: object, relaxation?: number, knockdownResidual?: number, overexpressionSigmas?: number, significanceZScore?: number, degreeOfParallelism?: object, minTimePoints?: object, env?: object): object;
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
