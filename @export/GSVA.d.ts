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
    * Gene Set Variation Analysis for microarray and RNA-seq data
    *  
    *  Gene Set Variation Analysis (GSVA) is a non-parametric, unsupervised 
    *  method for estimating variation of gene set enrichment through the
    *  samples of a expression data set. GSVA performs a change in coordinate
    *  systems, transforming the data from a gene by sample matrix to a gene-set
    *  by sample matrix, thereby allowing the evaluation of pathway enrichment 
    *  for each sample. This new matrix of GSVA enrichment scores facilitates
    *  applying standard analytical methods like functional enrichment, 
    *  survival analysis, clustering, CNV-pathway analysis or cross-tissue 
    *  pathway analysis, in a pathway-centric manner.
    *  
    *  main function of the package which estimates activity
    *  scores For Each given gene-Set
    * 
    * > Hänzelmann S., Castelo R. and Guinney J. GSVA: gene set variation analysis
    * >  for microarray and RNA-Seq data. BMC Bioinformatics, 14:7, 2013.
    * 
     * @param expr A raw gene expression data matrix object
     * @param geneSet A gsea enrichment [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model
     * @param name_suffix append the pathway name to the map id as suffix?
     * 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function gsva(expr: any, geneSet: any, name_suffix?: boolean, env?: object): object;
   /**
    * convert to diff data from dataframe
    * 
    * 
     * @param diff -
     * @param pathId -
     * 
     * + default value Is ``'pathNames'``.
     * @param t -
     * 
     * + default value Is ``'t'``.
     * @param pvalue -
     * 
     * + default value Is ``'pvalue'``.
   */
   function matrix_to_diff(diff: object, pathId?: string, t?: string, pvalue?: string): object;
   /**
     * @param verbose default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function mo_layers(layers: any, verbose?: boolean, env?: object): object;
   /**
     * @param env default value Is ``null``.
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
     * @param env 
     * + default value Is ``null``.
     * @return A fully populated [MultiOmicsGsvaOptions](cref:T:SMRUCC.genomics.Analysis.HTS.GSVA.MultiOmicsGsvaOptions) object that can be
     *  passed to the `mogsva` function.
   */
   function mogsva_opts(score_method?: object, kcdf?: object, fusion_strategy?: object, consistency_lambda?: number, balance_feature_size?: boolean, min_size?: object, max_size?: object, ssgsea_alpha?: number, ssgsea_normalize?: boolean, permutations?: object, permutation_method?: object, permutation_pathways?: string, permutation_gene_limit?: object, max_permutation_pathways?: object, seed?: object, compute_pooled?: boolean, verbose?: boolean, env?: object): object;
}
