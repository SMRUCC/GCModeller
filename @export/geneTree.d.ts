// export R# package module type define for javascript/typescript language
//
//    imports "geneTree" from "biosystem";
//
// ref=biosystem.geneTreeTools@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * XGBoost based gene regulatory network simulator and the in silico perturbation toolkit
 * 
 * > This package module is the R# front end of the ``GeneTree`` model: the ARACNE
 * >  inferred mutual information network is turned into a non linear dynamical system
 * >  by fitting one gradient boosting tree regressor per target gene
 * >  (`x_i = f_i(X_Reg(i))`), and the virtual perturbation experiment is then
 * >  performed by solving the fixed point of the fitted system with the damped
 * >  iteration `x_i^new = (1 - alpha) * x_i^old + alpha * f_i(X_Reg(i)^old)`.
 * >  
 * > 
 * > 
 * >  
 * >  
 * > 
 * > 
 * >  The typical workflow:
 * >  
 * > 
 * > 
 * >  
 * >  
 * > ```
 * >  imports ["geneTree", "bnlearn"] from "biosystem";
 * >  
 * >  let opts = geneTree::make_profile("fast", population = 100);
 * >  let grn = geneTree::new(aracne, expr, opts) |> fit_tree();
 * >  
 * >  # 相对训练时的野生型基线做虚拟敲除
 * >  let ko = grn |> knockouts(c("TP53", "MYC"));
 * >  
 * >  # 切换到指定的表达状态上下文之后再扰动（fix = TRUE 时上下文基因被钳制）
 * >  let state = grn |> geneTree::steady_state();
 * >  let perturbed = grn
 * >      |> geneTree::set_context(c("TP53"), c(0.0), fix = TRUE)
 * >      |> knockouts(c("EGFR"))
 * >  ;
 * >  
 * > ```
 * > 
 * >  
 * >  
 * > 
 * > 
 * >  The fitted [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) object implements the
 * >  [InsilicoPerturbationExperiment](cref:T:SMRUCC.genomics.BioSystem.InsilicoPerturbationExperiment) interface,
 * >  so the generic ``knockouts`` / ``overexpress`` / ``knockdown`` / ``make_exports``
 * >  api of the ``bnlearn`` package module can be used directly on it.
*/
declare namespace geneTree {
   /**
    * Get the candidate edges that were examined by the residual two-stage structure discovery.
    * 
    * > the ``delta_r2`` of an accepted edge is the validation R² gain that the edge brought
    * >  to its target gene model, the ``correlation`` column is the pearson correlation between
    * >  the target gene residual and the candidate gene expression.
    * 
     * @param tree the trained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param top_n the number of the records that are ordered by the R² gain in descending order, ``0``
     *  returns all of them.
     * 
     * + default value Is ``50``.
     * @param accepted_only when TRUE only the accepted new edges are returned.
     * 
     * + default value Is ``true``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a dataframe whose columns are ``gene``, ``regulator``, ``status``, ``correlation``,
     *  ``delta_r2``, ``r2_before``, ``r2_after``, ``splits``, ``sensitivity`` and ``reason``;
     *  an R# error message object is returned when the model does not contain any discovery
     *  result.
   */
   function discovered_edges(tree: object, top_n?: object, accepted_only?: boolean, env?: object): object;
   /**
    * Enable the residual two-stage structure discovery on a GeneTree configuration.
    * 
    * > The discovery runs in two stages:
    * >  
    * > 
    * > 
    * >  
    * > 1. only the ARACNE edges are used to fit the target gene model, and the residual
    * >  (label - prediction) is computed;
    * > 2. the candidates are ranked by the correlation with the residual, then they are added
    * >  into the feature set one by one; a candidate is accepted as a new regulation edge
    * >  only when the validation R² gain is above ``min_delta_r2`` and the feature is
    * >  actually used by the trees.
    * > 
    * > 
    * >  
    * > 
    * > 
    * >  the ARACNE edges are always kept, the accepted edges are appended after them, so the
    * >  topology of a discovery model is still comparable with the previous models.
    * 
     * @param config the configuration object created by the ``make_profile`` api (it is modified in
     *  place and returned, so it can be chained).
     * @param candidates a character vector of the candidate regulator gene id; the discovery only searches
     *  inside this set, it never guesses the candidate space by itself.
     *  
     *  to make a non-transcription-factor perturbation target able to propagate to its
     *  downstream genes, the target gene itself must be included in this candidate set.
     * @param max_per_gene the maximum number of the new edges that could be accepted for each target gene,
     *  default is 2.
     * 
     * + default value Is ``2``.
     * @param max_candidates the maximum number of the candidates that are tried for each target gene (each try
     *  costs one more model fitting), default is 5.
     * 
     * + default value Is ``5``.
     * @param min_correlation the minimal |pearson r| between the target gene residual and the candidate gene
     *  expression, default is 0.1.
     * 
     * + default value Is ``0.1``.
     * @param min_delta_r2 the minimal validation R² gain that is required for accepting one new edge,
     *  default is 0.002.
     * 
     * + default value Is ``0.002``.
     * @param sample_size the number of the cells that are used by the residual correlation screening,
     *  default is 1000, ``0`` uses all of the training cells.
     * 
     * + default value Is ``1000``.
     * @param max_genes only the first n target genes run the discovery, ``0`` means all of them (useful for
     *  a quick trial).
     * 
     * + default value Is ``0``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the input configuration object with ``EdgeDiscovery = ResidualTwoStage``; an R#
     *  error message object is returned when the candidate set is empty or the resulting
     *  configuration is invalid.
   */
   function discovery(config: object, candidates: any, max_per_gene?: object, max_candidates?: object, min_correlation?: number, min_delta_r2?: number, sample_size?: object, max_genes?: object, env?: object): object;
   /**
    * Get the split importance of the regulators, this table is the data driven
    *  "sign and weight" of the ARACNE network edges.
    * 
    * 
     * @param tree the trained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param top_n the number of the regulator records that are ordered by the sensitivity in
     *  descending order, ``0`` returns all of them.
     * 
     * + default value Is ``20``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a dataframe whose columns are ``gene``, ``regulator``, ``rank``, ``mi``,
     *  ``correlation``, ``splits`` and ``sensitivity``.
   */
   function feature_importance(tree: object, top_n?: object, env?: object): object;
   /**
    * Train the gradient boosting tree models of each target gene.
    * 
    * > the training is parallel over the target genes, the training progress is
    * >  written to the console when the ``verbose`` option of the configuration is on.
    * 
     * @param tree the untrained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the [TrainingReport](cref:T:SMRUCC.genomics.Analysis.GeneTree.TrainingReport) object: the R²/RMSE of each gene model, the
     *  comparison against the mean baseline and the split importance of each
     *  regulator; an R# error message object is returned when the training fails.
   */
   function fit_tree(tree: object, env?: object): object;
   /**
    * Get the gene ids of the model.
    * 
    * 
     * @param tree the trained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param env 
     * + default value Is ``null``.
     * @return a character vector of the gene id, the order is the row order of the expression matrix.
   */
   function gene_names(tree: object, env?: object): string;
   /**
    * Get the gene alignment and the wildtype baseline of the model.
    * 
    * > the ``means`` and ``sds`` vectors are estimated from the cells that are used
    * >  as the initial states of the fixed point iteration, they define the z-score
    * >  normalization of the perturbation result.
    * 
     * @param tree the trained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a list with the slots ``gene_names``, ``means``, ``sds``, ``population``,
     *  ``has_context`` and ``context_fix``.
   */
   function gene_stats(tree: object, env?: object): number;
   /**
    * Get the expression state context that is currently applied on the model.
    * 
    * 
     * @param tree the trained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param env 
     * + default value Is ``null``.
     * @return a list with the slots ``genes``, ``expression``, ``fix``, ``aligned``,
     *  ``clamp_genes``, ``note`` and ``unknown``(the gene ids that could not be mapped
     *  onto the model), the ``genes`` slot is an empty character vector when no context
     *  has been applied.
   */
   function get_context(tree: object, env?: object): number;
   /**
    * Attach the gene expression matrix to the regulation graph, the created model is
    *  not trained yet.
    * 
    * 
     * @param graph the regulation graph object that is created by the ``load_topology`` api.
     * @param expr the gene expression matrix object(rows are genes, columns are samples), which
     *  could be loaded from a csv table file via the ``geneExpression::load.expr`` api.
     *  the numeric values of the model training and the perturbation are all taken from
     *  this matrix(the ARACNE model archive stores the rank transformed vectors instead
     *  of the raw expression values).
     * @param config the configuration object created by the ``make_profile`` api.
     * 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
     * @return the untrained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
   */
   function load_expression(graph: object, expr: object, config?: object, env?: object): object;
   /**
    * Build the regulation graph model from the ARACNE inferred network.
    * 
    * 
     * @param aracne the ARACNE network object, which could be created by the ``aracne`` package
     *  module or loaded from the model zip archive via ``readBin``.
     * @param config the configuration object created by the ``make_profile`` api; the
     *  ``max_regulators`` and ``include_undirected_edges`` options determine the
     *  structure of the graph.
     * 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
     * @return the [GeneRegulationGraph](cref:T:SMRUCC.genomics.Analysis.GeneTree.GeneRegulationGraph) object: target gene -> regulators.
   */
   function load_topology(aracne: object, config?: object, env?: object): object;
   /**
    * Create the training/solving configuration profile of the GeneTree model.
    * 
    * 
     * @param profile the performance profile: ``fast`` (at most 1000 cells per model, top 10
     *  regulators, 40 boosting rounds), ``balanced`` (3000 cells, top 20 regulators,
     *  100 rounds) or ``full`` (all of the cells and all of the regulators).
     * 
     * + default value Is ``null``.
     * @param population the number of cells that are used as the independent initial states of the
     *  damped fixed point iteration, more cells give a more reliable population
     *  level statistic but cost more time.
     * 
     * + default value Is ``100``.
     * @param iterations the maximum iteration rounds of the fixed point solving.
     * 
     * + default value Is ``60``.
     * @param cells the maximum number of cells that is used by each gene model, ``0`` keeps the
     *  value of the selected profile.
     * 
     * + default value Is ``0``.
     * @param max_genes only the first n target genes(ordered by the regulator count in descending
     *  order) are trained, ``0`` means all of the target genes.
     * 
     * + default value Is ``0``.
     * @param max_regulators the number of the regulators that is kept for each target gene(the top n
     *  regulator ordered by the mutual information in descending order), a negative
     *  value keeps the value of the selected profile, ``0`` keeps all of them.
     * 
     * + default value Is ``-1``.
     * @param include_undirected 
     * + default value Is ``true``.
     * @param threads 
     * + default value Is ``-1``.
     * @param threads_per_model 
     * + default value Is ``-1``.
     * @return the [GeneTreeConfig](cref:T:SMRUCC.genomics.Analysis.GeneTree.GeneTreeConfig) configuration object.
   */
   function make_profile(profile?: object, population?: object, iterations?: object, cells?: object, max_genes?: object, max_regulators?: object, include_undirected?: boolean, threads?: object, threads_per_model?: object): object;
   /**
    * Create the GeneTree model from the ARACNE network and the gene expression matrix.
    * 
    * > this function is the combination of the ``load_topology`` and the
    * >  ``load_expression`` api, the model should be trained by the ``fit_tree`` api
    * >  before any virtual perturbation experiment.
    * 
     * @param aracne the ARACNE inferred network object.
     * @param expr the gene expression matrix object(rows are genes, columns are samples).
     * @param config the configuration object created by the ``make_profile`` api.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the untrained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object; an R# error message object
     *  is returned when the network or the expression matrix is invalid.
   */
   function new(aracne: object, expr: object, config?: object, env?: object): object;
   /**
    * Override the parallelism of a trained model (the perturbation solver) or of a
    *  configuration object (the model training).
    * 
    * > the training is parallel over the target genes, therefore the per model threads are
    * >  resolved to 1 automatically: nested parallelism (outer over genes plus inner tree
    * >  building) starves the .NET thread pool and the CPU utilization collapses to a few
    * >  cores even on a 128 threads server.
    * 
     * @param tree the GeneTree model or the configuration object.
     * @param threads the outer (per gene) degree of parallelism; ``<= 0`` restores the automatic mode
     *  which uses all of the available processors.
     * 
     * + default value Is ``-1``.
     * @param threads_per_model the TGBoost thread number of a single gene model: ``-1`` automatic (1 when the genes
     *  are trained in parallel), ``0`` sequential, ``>0`` explicit. It only affects the
     *  model training, it is ignored for the perturbation solving.
     * 
     * + default value Is ``-1``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the input object with the new parallelism settings.
   */
   function parallel_opts(tree: any, threads?: object, threads_per_model?: object, env?: object): any;
   /**
    * Set the expression value of a set of genes to the given values, i.e. run a
    *  ``Custom`` mode in silico intervention.
    * 
    * 
     * @param tree the trained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param genes a character vector of the target gene id.
     * @param expression a numeric vector of the expected expression value of each target gene, in the
     *  same order as **genes**; the genes are set to these values and
     *  are clamped during the whole iteration.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the [InterventionResult](cref:T:SMRUCC.genomics.BioSystem.Intervention.InterventionResult) object of the intervention, which could be
     *  exported via the ``make_exports`` api of the ``bnlearn`` package module;
     *  an R# error message object is returned when the sizes of the two vectors do not
     *  match or the prediction fails.
   */
   function perturb(tree: object, genes: any, expression: any, env?: object): object;
   /**
    * Run a combined in silico intervention on a set of genes at the same time.
    * 
    * > the ``Spec`` property of the result object is a synthetic intervention
    * >  specification whose gene name is the ``+`` joined id list.
    * 
     * @param tree the trained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param genes a character vector of the gene id that are perturbed together.
     * @param mode the intervention mode of all of the target genes: ``Knockout``, ``Knockdown``,
     *  ``Overexpression`` or ``Custom``.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return one [InterventionResult](cref:T:SMRUCC.genomics.BioSystem.Intervention.InterventionResult) object of the combined intervention, the
     *  non additive synergy/antagonism effect of the gene combination is kept;
     *  an R# error message object is returned when the prediction fails.
   */
   function perturb_combined(tree: object, genes: any, mode?: object, env?: object): object;
   /**
    * Get the per gene fitting quality table of the trained model.
    * 
    * 
     * @param tree the trained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a dataframe whose rows are the modelled genes and whose columns are ``kind``,
     *  ``n_regulators``, ``n_train``, ``n_validate``, ``r2``, ``rmse``,
     *  ``baseline_rmse``, ``improvement``, ``trees``, ``min_value`` and ``max_value``.
   */
   function report_table(tree: object, env?: object): object;
   /**
    * Remove the expression state context, the model is restored back to the wildtype
    *  baseline that was estimated during the model training.
    * 
    * 
     * @param tree the trained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param env 
     * + default value Is ``null``.
     * @return the input model object, so that it can be chained with the perturbation api.
   */
   function reset_context(tree: object, env?: object): object;
   /**
    * Inject an expression state context into the model, the context becomes the
    *  starting state as well as the differential analysis baseline of the following
    *  in silico perturbation experiments.
    * 
    * > genes that are not described in the context keep their own wildtype value, so a
    * >  context is allowed to cover only a subset of the network.
    * 
     * @param tree the trained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param genes a character vector of the gene id of the context; the ids must live in the same
     *  identifier space as the expression matrix that the model was trained on.
     * @param expression a numeric vector of the expression value of the context, in the same order as
     *  **genes**; the values may come from a different normalisation
     *  pipeline, they are mapped onto the model scale by the quantile alignment below.
     * @param fix when TRUE the context genes are clamped to the given values during the whole
     *  iteration(a gene knock-in like experiment); otherwise they are only used as
     *  the starting point and will be updated by the network like the other genes.
     *  
     *  clamping only makes sense for a small gene set: when the clamped gene number
     *  exceeds ``MaxClampRatio``(5% by default) of the network, the whole network would
     *  lose its degrees of freedom and the perturbation effect would vanish, so the
     *  library degrades the context to "starting state only" automatically and reports
     *  a warning here.
     * 
     * + default value Is ``false``.
     * @param align whether to run the quantile alignment of the context onto the model expression
     *  scale, default is TRUE: the context values are mapped onto the per gene
     *  distribution of the model population, so that the relative order of the genes is
     *  kept and no feature falls outside of the training range of its tree model.
     * 
     * + default value Is ``true``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the input model object that the context has been applied, so that it can be
     *  chained with ``knockouts`` / ``overexpress`` / ``knockdown``;
     *  an R# error message object is returned when the sizes of the two vectors do not
     *  match or no gene of the context could be mapped onto the model.
   */
   function set_context(tree: object, genes: any, expression: any, fix?: boolean, align?: boolean, env?: object): object;
   /**
    * Get the diagnostics of the last fixed point solving.
    * 
    * 
     * @param tree the trained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a list with the slots ``iterations``, ``converged``, ``converged_cells``,
     *  ``max_change``, ``population`` and ``history``(the maximum relative change of
     *  each iteration); an R# error message object is returned when no perturbation
     *  experiment has been run yet.
   */
   function solver_history(tree: object, env?: object): number;
   /**
    * Override the fixed point solving options of a trained model at runtime, so that
    *  the solver can be tuned without re-training the whole network.
    * 
    * > the control steady state and the last perturbation result are invalidated, so the
    * >  next perturbation experiment re-solves the control steady state with the new
    * >  options.
    * 
     * @param tree the trained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param iterations the soft iteration limit; a negative value keeps the current value.
     * 
     * + default value Is ``-1``.
     * @param damping the damping coefficient ``alpha`` in the range (0, 1]; a negative value keeps the
     *  current value. A larger alpha moves faster but is easier to oscillate.
     * 
     * + default value Is ``-1``.
     * @param tolerance the relative convergence tolerance; a negative value keeps the current value.
     * 
     * + default value Is ``-1``.
     * @param hard_cap the hard iteration limit which is used when the soft limit is not enough for
     *  convergence; a negative value keeps the current value, ``0`` restores the default
     *  (``max(iterations * 5, 300)``).
     * 
     * + default value Is ``-1``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the input model object that the solver options have been applied.
   */
   function solver_opts(tree: object, iterations?: object, damping?: number, tolerance?: number, hard_cap?: object, env?: object): object;
   /**
    * Solve the control steady state of the network, i.e. the "attractor" of the fitted
    *  dynamical system when no intervention is applied.
    * 
    * > the result is cached in the model object, the first call runs the fixed point
    * >  iteration and the later calls are cheap. When an expression context is applied,
    * >  the control steady state starts from the context state.
    * 
     * @param tree the trained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a list with the slots ``gene_names`` and ``means``: the difference between the
     *  mean and the wildtype baseline is the systematic drift of the tree model itself,
     *  which could be subtracted from the perturbation result.
   */
   function steady_state(tree: object, env?: object): number;
   /**
    * Get the summary statistics of the model training.
    * 
    * 
     * @param tree the trained [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) model object.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a list with the slots: ``genes``, ``models``, ``boosted_models``,
     *  ``constant_models``, ``source_nodes``, ``median_r2``, ``mean_r2``,
     *  ``informative_models``, ``cells``, ``population`` and ``fitted``.
   */
   function training_stats(tree: object, env?: object): number;
   /**
    * Export the training report as two readable table files.
    * 
    * 
     * @param report the [TrainingReport](cref:T:SMRUCC.genomics.Analysis.GeneTree.TrainingReport) object returned by the ``fit_tree`` api.
     * @param folder the output directory: the ``report.tsv``(fitting quality of each gene model) and
     *  the ``importance.tsv``(split importance of each regulator) files are written
     *  into this directory, it will be created if it is not exists.
     * @return TRUE when the table files have been written.
   */
   function write_report(report: object, folder: string): boolean;
}
