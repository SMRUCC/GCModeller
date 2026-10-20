// export R# package module type define for javascript/typescript language
//
//    imports "linear" from "TRNtoolkit";
//
// ref=TRNtoolkit.Linear@TRNtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The linear (elastic net) gene regulatory network builder and the in silico
 *  perturbation toolkit that runs on top of the fitted linear models.
 * 
 * > This package provides two groups of the R# api:
 * >  
 * >  + ``elastic_net`` / ``coef``: fit a ``regulator -> target`` linear regulatory
 * >    network from a gene expression matrix, and export the coefficient matrix;
 * >  + ``elastic_model`` / ``set_baseline`` / ``intervene`` / ``profiles_matrix``:
 * >    build an in silico perturbation engine from the fitted network, condition it
 * >    on a given cell context, and run the RNAi style knockdown experiments.
 * >  
 * >  The perturbation engine implements the `InsilicoPerturbationExperiment`
 * >  interface, so the ``knockouts`` / ``overexpress`` / ``knockdown`` api of the
 * >  ``bnlearn`` package can be used on the object that is created by
 * >  ``elastic_model`` directly.
*/
declare namespace linear {
   /**
    * Export the coefficient matrix of the fitted network.
    * 
    * 
     * @param grn the fitted linear GRN model
     * @return a matrix whose rows are the target genes and columns are the regulators,
     *  an element is the regression coefficient of the ``regulator -> target`` edge.
   */
   function coef(grn: object): object;
   /**
    * Build the in silico perturbation engine from a fitted linear GRN model.
    * 
    * 
     * @param grn the fitted [GeneRegulatoryNetwork](cref:T:SMRUCC.genomics.Model.Network.Regulons.LinearRegression.GeneRegulatoryNetwork) object, it is the
     *  ``grn`` slot of the ``elastic_net`` result.
     * @param opts the perturbation options, a default configuration is used when this argument is not given
     * 
     * + default value Is ``null``.
     * @param strict in the strict mode an error will be thrown when the target gene of the perturbation
     *  is not found in the model; in the non strict mode a warning will be printed and the
     *  wildtype expression will be returned with the ``Undefined`` flag marked as TRUE.
     *  For a batch run of hundreds of the perturbations it is recommended to disable it and
     *  to check the ``MissingGenes`` field of the result.
     * 
     * + default value Is ``true``.
     * @param scale the numeric space of the expression matrix that was used to fit the network:
     *  ``linear`` (default) or ``log2cpm`` (i.e. ``log2(x + 1)``). For a log transformed
     *  matrix this argument is required, otherwise the knockdown magnitude will be
     *  over-estimated by about 2~3 times.
     * 
     * + default value Is ``'linear'``.
     * @param propagation ``cascade`` (default) propagates the perturbation to the steady state,
     *  ``onehop`` only keeps the direct effects,
     *  ``delta`` propagates the change only (recommended for the context conditioned
     *  prediction: it is numerically identical to "mutant minus the matched control").
     * 
     * + default value Is ``'cascade'``.
     * @param steps the max cascade iteration steps, a value that is less than 1 means "use the default"
     * 
     * + default value Is ``-1``.
     * @param damping the damping factor of the cascade iteration, a value that is less than or equals to 0 means "use the default"
     * 
     * + default value Is ``-1``.
     * @param knockdown_residual the fraction of the wildtype expression that is kept after a knockdown,
     *  a negative value means "use the default" (0.2, i.e. about 80% knockdown).
     * 
     * + default value Is ``-1``.
     * @param env the R# runtime environment object
     * 
     * + default value Is ``null``.
     * @return an [ElasticNet](cref:T:SMRUCC.genomics.Analysis.CellPhenotype.ElasticNet) engine which implements the
     *  [InsilicoPerturbationExperiment](cref:T:SMRUCC.genomics.Analysis.BNLearn.InsilicoPerturbationExperiment) interface.
   */
   function elastic_model(grn: object, opts?: object, strict?: boolean, scale?: string, propagation?: string, steps?: object, damping?: number, knockdown_residual?: number, env?: object): object;
   /**
    * Fit a linear (elastic net) gene regulatory network from the expression matrix.
    * 
    * 
     * @param x the gene expression matrix, rows are genes and columns are samples
     * @param candidates the max number of the candidate regulators for each target gene, the candidates
     *  are picked by the absolute pearson correlation with the target gene.
     * 
     * + default value Is ``1000``.
     * @param maxreg the max number of the non-zero regulation edges that each target gene can keep
     * 
     * + default value Is ``10``.
     * @param l1_ratio the L1 mixing ratio of the elastic net, in the range [0, 1]
     * 
     * + default value Is ``0.7``.
     * @param alphas the number of the alpha steps in the regularization path
     * 
     * + default value Is ``40``.
     * @param mutual only keep the reversible (mutual) regulation pairs
     * 
     * + default value Is ``false``.
     * @param TF an optional gene id vector of the transcript factors, these genes are always
     *  included in the candidate regulator pool (they are not affected by the
     *  **candidates** truncation). Non TF genes are still able to be the
     *  regulators, so that the knockdown of a non TF gene still has downstream responses.
     * 
     * + default value Is ``null``.
     * @param standardize z-score the expression vectors before the fitting
     * 
     * + default value Is ``false``.
     * @param min_abs_r the minimal absolute pearson correlation for a gene to be a candidate regulator
     * 
     * + default value Is ``0``.
     * @return a list object with two slots: ``grn`` (the fitted [GeneRegulatoryNetwork](cref:T:SMRUCC.genomics.Model.Network.Regulons.LinearRegression.GeneRegulatoryNetwork))
     *  and ``edges`` (the [RegulationEdge](cref:T:SMRUCC.genomics.Model.Network.Regulons.LinearRegression.RegulationEdge) collection).
   */
   function elastic_net(x: object, candidates?: object, maxreg?: object, l1_ratio?: number, alphas?: number, mutual?: boolean, TF?: any, standardize?: boolean, min_abs_r?: number): object|object;
   /**
    * Run the perturbation experiment for a set of genes and save the profiles as a csv file.
    * 
    * > The first column is the gene id, the second column is the control (wildtype) profile,
    * >  and the following columns are the mutant profiles of the given perturbations.
    * 
     * @param model the perturbation engine that is created via ``elastic_model``
     * @param genes the target gene ids
     * @param file the output csv file, its folder will be created automatically
     * @param mode ``knockout``, ``knockdown`` (default) or ``overexpress``
     * 
     * + default value Is ``'knockdown'``.
     * @param env the R# runtime environment object
     * 
     * + default value Is ``null``.
     * @return TRUE when the file has been written successfully
   */
   function export_profiles(model: object, genes: any, file: string, mode?: string, env?: object): boolean;
   /**
    * Get the gene ids of the fitted network, in the row order of the expression matrix.
    * 
    * 
     * @param model the perturbation engine that is created via ``elastic_model``
     * @return a character vector of the gene ids
   */
   function gene_names(model: object): string;
   /**
    * Run one in silico perturbation experiment and return the raw result object.
    * 
    * 
     * @param model the perturbation engine that is created via ``elastic_model``
     * @param gene the target gene id
     * @param mode ``knockout``, ``knockdown``, ``overexpress`` or ``custom``
     * 
     * + default value Is ``'knockout'``.
     * @param value the absolute expression value in the model space, only used by the ``custom`` mode.
     * 
     * + default value Is ``0``.
     * @param env the R# runtime environment object
     * 
     * + default value Is ``null``.
     * @return an [InterventionResult](cref:T:SMRUCC.genomics.Analysis.BNLearn.Intervention.InterventionResult) object; use ``as.list`` to access the fields
     *  (the ``$`` indexing is not supported on this object).
   */
   function intervene(model: object, gene: string, mode?: string, value?: number, env?: object): object;
   /**
    * Get the implied library size of every training sample of the fitted network.
    * 
    * > This vector is the bridge between an external raw count matrix and the model space:
    * >  normalize the counts with ``y = counts / sum(counts) * median(library_sizes)`` and
    * >  then apply ``x = log2(y + pseudo)``.
    * 
     * @param model the perturbation engine that is created via ``elastic_model``
     * @param pseudo the pseudo count offset that was used by the log transformation, default 1
     * 
     * + default value Is ``1``.
     * @return a numeric vector, one value per training sample, i.e. ``sum(2^x - pseudo)`` of the
     *  expression matrix that was used to fit the network.
   */
   function library_sizes(model: object, pseudo?: number): number;
   /**
    * Run the perturbation experiment for a set of genes and collect the profiles into a matrix.
    * 
    * > This api is the memory friendly replacement of the ``make_exports`` api for a batch run:
    * >  it does not generate the ten difference analysis tables for every condition.
    * 
     * @param model the perturbation engine that is created via ``elastic_model``
     * @param genes the target gene ids, one column will be generated for each of them
     * @param mode ``knockout``, ``knockdown`` (default) or ``overexpress``
     * 
     * + default value Is ``'knockdown'``.
     * @param env the R# runtime environment object
     * 
     * + default value Is ``null``.
     * @return a matrix of the ``[gene x condition]`` mutant means. The genes that can not be found in
     *  the model are skipped, so the column number equals the number of the matched genes.
   */
   function profiles_matrix(model: object, genes: any, mode?: string, env?: object): object;
   /**
    * Set the wildtype baseline (the cell context) of the perturbation engine.
    * 
    * 
     * @param model the perturbation engine that is created via ``elastic_model``
     * @param genes the gene ids of the given expression profile, the matching against the model is
     *  case insensitive and the genes that are not modelled are ignored.
     * @param values the baseline expression values, they must be in the model space (i.e. the same
     *  transformation that was used to fit the network, for example ``log2(CPM + 1)``).
     * @param env the R# runtime environment object
     * 
     * + default value Is ``null``.
     * @return the number of the genes that have been matched and written
   */
   function set_baseline(model: object, genes: any, values: any, env?: object): any;
   /**
    * Get the currently effective wildtype (baseline) expression vector of the model.
    * 
    * 
     * @param model the perturbation engine that is created via ``elastic_model``
     * @return a numeric vector, one value per gene, in the same order as ``gene_names``.
     *  Before ``set_baseline`` is called this is the sample mean of the training matrix.
   */
   function wildtype_means(model: object): number;
}
