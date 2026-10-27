// export R# package module type define for javascript/typescript language
//
//    imports "GSVA" from "gseakit";
//
// ref=gseakit.GSVA@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Gene Set Variation Analysis for microarray and RNA-seq data
 * 
*/
declare namespace GSVA {
   /**
    * different analysis of the gsva result
    * 
    * 
     * @param gsva the gsva analysis result
     * @param compares the [DataAnalysis](cref:T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.DataAnalysis) comparision collection
     * @return this function returns a array of the [GSVADiff](cref:T:SMRUCC.genomics.Visualize.GSVADiff) object which is compares the different of the gsva score via the limma function. 
     *  for get the different score of gsva score in raw [LimmaTable](cref:T:SMRUCC.genomics.Analysis.HTS.DataFrame.LimmaTable) array type, please use the ``geneExpression::limma`` function for analysis the gsva difference result across two condition groups.
   */
   function diff(gsva: object, compares: object): object;
   /**
    * Gene Set Variation Analysis (GSVA) for microarray and RNA-seq data.
    * 
    * > GSVA is a non-parametric, unsupervised method for estimating variation
    * >  of gene set enrichment through the samples of an expression data set.
    * >  It performs a change in coordinate systems, transforming the data from a
    * >  gene-by-sample matrix to a gene-set-by-sample matrix, thereby allowing
    * >  the evaluation of pathway enrichment for each sample. This new matrix of
    * >  GSVA enrichment scores facilitates applying standard analytical methods
    * >  such as functional enrichment, survival analysis, clustering,
    * >  CNV-pathway analysis or cross-tissue pathway analysis, in a
    * >  pathway-centric manner.
    * >  Hänzelmann S., Castelo R. and Guinney J. GSVA: gene set variation analysis
    * >  for microarray and RNA-Seq data. BMC Bioinformatics, 14:7, 2013.
    * 
     * @param expr A raw gene expression data matrix. Acceptable inputs are an
     *  [Matrix](cref:T:SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix) object or an R# `dataframe` whose rows are
     *  features (genes) and whose columns are samples.
     * @param geneSet A gene-set collection supplied either as a GSEA
     *  [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model or as anything that
     *  `BackgroundHelper.CastBackground` can coerce into one (for example a
     *  GMT file path or a list of gene-set objects).
     * @param method The enrichment scoring method:
     *  [Methods.gsva](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.Methods.gsva) (default, kernel based ES),
     *  [Methods.ssgsea](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.Methods.ssgsea), [Methods.zscore](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.Methods.zscore) or
     *  [Methods.plage](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.Methods.plage).
     * 
     * + default value Is ``null``.
     * @param kcdf The kernel density estimation type used to estimate the cumulative
     *  distribution function of each expression profile:
     *  [KCDFs.Gaussian](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.KCDFs.Gaussian) for continuous data,
     *  [KCDFs.Poisson](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.KCDFs.Poisson) for count data, or
     *  [KCDFs.none](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.KCDFs.none) for the empirical cumulative distribution
     *  function. Defaults to [KCDFs.none](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.KCDFs.none), i.e. the same behavior as
     *  the underlying function when no kernel is requested.
     * 
     * + default value Is ``null``.
     * @param min_sz Minimum gene-set size (number of mapped features). Gene sets with fewer
     *  mapped features are removed from the analysis. Default 1.
     * 
     * + default value Is ``1``.
     * @param max_sz Maximum gene-set size (number of mapped features). Gene sets with more
     *  mapped features are removed from the analysis. Default unlimited.
     * 
     * + default value Is ``2147483647``.
     * @param mxdiff When TRUE (default) the ES range is transformed to the interval
     *  [-1, 1] by using the difference between the largest and the smallest ES
     *  values; otherwise the original ES range is kept.
     * 
     * + default value Is ``true``.
     * @param tau Exponent (τ, default 1) applied to the weight of the cumulative
     *  distribution function when **method** is
     *  [Methods.gsva](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.Methods.gsva); higher values give more weight to the tails
     *  of the distribution.
     * 
     * + default value Is ``1``.
     * @param kernel Logical flag to use the Gaussian kernel to estimate the cumulative
     *  distribution function when **method** is
     *  [Methods.gsva](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.Methods.gsva). Ignored when a non-kernel
     *  **kcdf** is requested.
     * 
     * + default value Is ``true``.
     * @param rnaseq Logical flag indicating that **expr** is read count data
     *  from an RNA-seq experiment; this switches the kernel to a Poisson
     *  density when a kernel is used.
     * 
     * + default value Is ``false``.
     * @param abs_ranking When TRUE (default FALSE) the enrichment scores are computed using the
     *  absolute values of the ranking, which is appropriate for single-sample
     *  (ssGSEA-style) scoring.
     * 
     * + default value Is ``false``.
     * @param alpha The α (alpha) parameter (default 0.25) of the ssGSEA scoring method,
     *  controlling the weight of the random-walk deviation.
     * 
     * + default value Is ``0.25``.
     * @param normalize Logical flag (default TRUE) to normalize the enrichment scores so that
     *  each gene set has a mean enrichment of zero across samples.
     * 
     * + default value Is ``true``.
     * @param name_suffix Append the gene-set description (`names`) to the pathway identifier
     *  as a suffix in the returned matrix, e.g. `"hsa00010 - Glycolysis"`.
     * 
     * + default value Is ``false``.
     * @param verbose Logical flag (default FALSE) to emit progress and diagnostic messages
     *  during the calculation.
     * 
     * + default value Is ``false``.
     * @param env The R# runtime environment (used for error reporting).
     * 
     * + default value Is ``null``.
     * @return An [Matrix](cref:T:SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix) of GSVA enrichment scores with gene sets in
     *  rows and samples in columns, or a `Message` describing the error
     *  when the gene-set collection cannot be parsed.
   */
   function gsva(expr: any, geneSet: any, method?: object, kcdf?: object, min_sz?: object, max_sz?: object, mxdiff?: boolean, tau?: number, kernel?: boolean, rnaseq?: boolean, abs_ranking?: boolean, alpha?: number, normalize?: boolean, name_suffix?: boolean, verbose?: boolean, env?: object): object;
   /**
    * Convert the limma differential analysis result table into the GSVA difference objects.
    * 
    * 
     * @param diff the dataframe object of the limma differential analysis result.
     * @param pathId the column name of the pathway id in the given dataframe. Default ``pathNames``.
     * 
     * + default value Is ``'pathNames'``.
     * @param t the column name of the t statistics. Default ``t``.
     * 
     * + default value Is ``'t'``.
     * @param pvalue the column name of the p-value. Default ``pvalue``.
     * 
     * + default value Is ``'pvalue'``.
     * @return an array of the [GSVADiff](cref:T:SMRUCC.genomics.Visualize.GSVADiff) objects.
   */
   function matrix_to_diff(diff: object, pathId?: string, t?: string, pvalue?: string): object;
   /**
    * Assemble the multiple omics data layers into a [MultiOmicsDataset](cref:T:SMRUCC.genomics.Analysis.HTS.GSVA.Omics.MultiOmicsDataset) object
    *  for the multi-omics GSVA (moGSVA) analysis.
    * 
    * > The samples of all of the omics layers are aligned by the sample name: the intersection
    * >  of the sample names is used as the final sample axis of the multi-omics dataset.
    * 
     * @param layers the omics layer collection, each element could be an [OmicsLayer](cref:T:SMRUCC.genomics.Analysis.HTS.DataFrame.MultipleOmics.OmicsLayer) object or
     *  a tuple that could be cast into the omics layer model. Every layer should define its
     *  unique omics name and the expression matrix whose rows are the features of that omics.
     * @param verbose prints the progress and the diagnostic message of the layer alignment? Default ``TRUE``.
     * 
     * + default value Is ``true``.
     * @param env the R# runtime environment.
     * 
     * + default value Is ``null``.
     * @return a [MultiOmicsDataset](cref:T:SMRUCC.genomics.Analysis.HTS.GSVA.Omics.MultiOmicsDataset) object which contains the sample aligned omics layers,
     *  or an error message when the layer collection could not be assembled.
   */
   function mo_layers(layers: any, verbose?: boolean, env?: object): object;
   /**
    * Run the multi-omics GSVA (moGSVA) analysis on the given multi-omics dataset.
    * 
    * > The scoring of each omics layer is performed independently (the kernel density
    * >  estimation never mixes the features from the different omics), then the omics specific
    * >  enrichment scores are fused by the configured weight strategy.
    * 
     * @param x the multi-omics dataset object that is created by the ``mo_layers`` function.
     * @param geneSet the gene set collection, it could be a GSEA [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model, a GMT file
     *  path or other data that could be cast into the background model.
     * @param opts the running configuration object that is created by the ``mogsva_opts`` function.
     * @param env the R# runtime environment.
     * 
     * + default value Is ``null``.
     * @return a [MultiOmicsGsvaResult](cref:T:SMRUCC.genomics.Analysis.HTS.GSVA.MultiOmicsGsvaResult) object which contains the fused pathway activity
     *  matrix, the per-omics scores, the pathway weights, the omics contribution decomposition
     *  and the permutation test p-value table (when ``permutations`` is greater than zero).
   */
   function mogsva(x: object, geneSet: any, opts: object, env?: object): object;
   /**
    * Build a [MultiOmicsGsvaOptions](cref:T:SMRUCC.genomics.Analysis.HTS.GSVA.MultiOmicsGsvaOptions) configuration object that controls
    *  the whole multi-omics GSVA (moGSVA) pipeline: per-omics enrichment scoring,
    *  cross-omics weighted fusion with a consistency reward, and a gene-to-pathway
    *  permutation test for significance assessment.
    * 
    * > All parameters are optional and default to the values defined on the
    * >  [MultiOmicsGsvaOptions](cref:T:SMRUCC.genomics.Analysis.HTS.GSVA.MultiOmicsGsvaOptions) class, so calling this function with
    * >  no arguments yields the same default configuration that the moGSVA engine
    * >  uses.
    * 
     * @param score_method The enrichment scoring method applied independently inside each omics layer
     *  (recommended [Methods.gsva](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.Methods.gsva), the default). Other choices are
     *  ssGSEA, z-score and PLAGE.
     * 
     * + default value Is ``null``.
     * @param kcdf The kernel density estimation type used by the GSVA scoring step:
     *  [KCDFs.Gaussian](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.KCDFs.Gaussian) for continuous data (default),
     *  [KCDFs.Poisson](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.KCDFs.Poisson) for count data, or
     *  [KCDFs.none](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.KCDFs.none) for the empirical cumulative distribution function.
     * 
     * + default value Is ``null``.
     * @param fusion_strategy The cross-omics weighted fusion strategy. Defaults to
     *  [FusionMethods.Equal](cref:F:SMRUCC.genomics.Analysis.HTS.GSVA.Fusion.FusionMethods.Equal) (a plain average, i.e. baseline 1).
     *  Alternatives: variance weighting, information-entropy weighting,
     *  mutual-information consistency weighting and a balanced scheme that also
     *  applies the per-omics feature-count balancing term.
     * 
     * + default value Is ``null``.
     * @param consistency_lambda The hyper-parameter λ (lambda) of the cross-omics consistency reward term.
     *  A value of 0 disables the reward and reduces the fusion to a pure weighted
     *  average. Default 0.25.
     * 
     * + default value Is ``0.25``.
     * @param balance_feature_size Whether to superimpose the per-omics feature-count balancing term
     *  1/log2(2+|S_k|) so that omics layers with many mapped members cannot
     *  dominate the fused score. Off by default so that the equal-weight fusion
     *  stays strictly identical to the simple per-omics average.
     * 
     * + default value Is ``false``.
     * @param min_size Minimum gene-set size (number of mapped features) after the feature
     *  mapping step. Gene sets smaller than this are dropped. Default 5.
     * 
     * + default value Is ``5``.
     * @param max_size Maximum gene-set size (number of mapped features) after the feature
     *  mapping step. Gene sets larger than this are dropped. Default 500.
     * 
     * + default value Is ``500``.
     * @param ssgsea_alpha The α (alpha) parameter of the ssGSEA scoring path. Default 0.25.
     * 
     * + default value Is ``0.25``.
     * @param ssgsea_normalize Whether the ssGSEA scoring path normalizes the enrichment scores.
     *  Default TRUE.
     * 
     * + default value Is ``true``.
     * @param permutations Number of permutations for the gene-to-pathway permutation test.
     *  0 (the default) skips the significance assessment entirely.
     * 
     * + default value Is ``0``.
     * @param permutation_method The scoring method used to build the permutation null distribution.
     *  Defaults to ssGSEA (rank based) to avoid repeating the expensive
     *  kernel density estimation.
     * 
     * + default value Is ``null``.
     * @param permutation_pathways A subset of pathway identifiers to evaluate in the permutation test.
     *  When nothing is supplied the first **max_permutation_pathways**
     *  pathways from the fused result are used.
     * 
     * + default value Is ``null``.
     * @param permutation_gene_limit Upper bound on the number of features kept in the permutation view
     *  (0 means no trimming). When trimming is applied, the target pathway
     *  members are kept first and the remaining budget is filled with background
     *  features using a fixed seed.
     * 
     * + default value Is ``0``.
     * @param max_permutation_pathways Maximum number of pathways evaluated in the permutation test when no
     *  explicit pathway subset is supplied. Default 40.
     * 
     * + default value Is ``40``.
     * @param seed Random seed used for deterministic data derivation and permutation
     *  shuffling so that runs are fully reproducible. Default 20261026.
     * 
     * + default value Is ``20261026``.
     * @param compute_pooled Whether to also compute the ablation control "concatenate then single-omics
     *  GSVA" baseline. Default TRUE.
     * 
     * + default value Is ``true``.
     * @param verbose Whether to emit progress/diagnostic logging. Default TRUE.
     * 
     * + default value Is ``true``.
     * @param env The R# runtime environment (used for error reporting).
     * 
     * + default value Is ``null``.
     * @return A fully populated [MultiOmicsGsvaOptions](cref:T:SMRUCC.genomics.Analysis.HTS.GSVA.MultiOmicsGsvaOptions) object that can be
     *  passed to the `mogsva` function.
   */
   function mogsva_opts(score_method?: object, kcdf?: object, fusion_strategy?: object, consistency_lambda?: number, balance_feature_size?: boolean, min_size?: object, max_size?: object, ssgsea_alpha?: number, ssgsea_normalize?: boolean, permutations?: object, permutation_method?: object, permutation_pathways?: string, permutation_gene_limit?: object, max_permutation_pathways?: object, seed?: object, compute_pooled?: boolean, verbose?: boolean, env?: object): object;
}
