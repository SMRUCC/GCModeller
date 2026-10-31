// export R# package module type define for javascript/typescript language
//
//    imports "ARACNE" from "TRNtoolkit";
//
// ref=TRNtoolkit.ARACNETools@TRNtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * ARACNE (Algorithm for the Reconstruction of Accurate Cellular Networks) gene
 *  regulatory network inference.
 * 
 * > The pipeline is: ``mi -> threshold -> DPI -> direction``, and the transcription
 * >  factor (TF) prior knowledge is used in four places:
 * >  
 * >  + the computation space is reduced from ``N x N`` to ``TF x N``;
 * >  + the ``TF -> target`` direction is assigned from the TF list;
 * >  + the curated ``TF -> target`` evidence relaxes the MI threshold and protects
 * >    the supported edges from the top-K pruning;
 * >  + the curated evidence also protects the supported edges from the DPI filtering,
 * >    and the protection strength is graded by the prior confidence.
 * >  
 * >  The api groups:
 * >  
 * >  + ``read_tf`` / ``read_prior``: load the prior knowledge (the TF annotation table and
 * >    the curated ``TF -> target`` table) directly from the data files;
 * >  + ``aracne``: run the whole inference from an expression matrix;
 * >  + ``aracne_options`` / ``infer``: build the options object, then run the inference
 * >    in two steps (useful when the same options are reused for several matrices);
 * >  + ``edges`` / ``regulons`` / ``summary`` / ``threshold`` / ``dpi_stats`` /
 * >    ``mi_matrix`` / ``as_graph`` / ``write_edges``: inspect and export the network.
*/
declare namespace ARACNE {
   /**
    * Infer the ARACNE gene regulatory network from the expression matrix.
    * 
    * 
     * @param x the gene expression matrix, rows are the genes and columns are the samples
     * @param TF an optional gene id vector of the transcription factors. When it is given the
     *  computation space is reduced from ``N x N`` to ``TF x N``, and the edges are
     *  oriented from the TF to the targets; the prior network (the ``prior`` argument)
     *  is additionally used for the threshold relaxation and the DPI protection.
     * 
     * + default value Is ``null``.
     * @param estimator see the ``aracne_options`` api; default ``copula``
     * 
     * + default value Is ``'copula'``.
     * @param threshold see the ``aracne_options`` api; default ``permutation``
     * 
     * + default value Is ``'permutation'``.
     * @param mi_cutoff the fixed MI threshold, a positive value forces the ``fixed`` mode
     * 
     * + default value Is ``0``.
     * @param quantile the quantile of the permutation null distribution, default 0.95
     * 
     * + default value Is ``0.95``.
     * @param permutations the number of the gene pairs that are sampled for the null distribution
     * 
     * + default value Is ``0``.
     * @param dpi ``auto`` (default), ``standard``, ``tf`` or ``off``
     * 
     * + default value Is ``'auto'``.
     * @param epsilon the DPI tolerance, default 0.1
     * 
     * + default value Is ``0.1``.
     * @param direction ``tf`` (default) or ``undirected``
     * 
     * + default value Is ``'tf'``.
     * @param bins the bin number of the ``histogram`` estimator, default 4
     * 
     * + default value Is ``4``.
     * @param max_neighbors the top-K pruning of each regulator, 0 = no pruning
     * 
     * + default value Is ``0``.
     * @param max_genes keep only the top variant genes (the TF genes are always kept)
     * 
     * + default value Is ``0``.
     * @param min_nonzero the minimal non-zero ratio of the retained genes
     * 
     * + default value Is ``0``.
     * @param log2 apply ``log2(x + 1)`` before the inference
     * 
     * + default value Is ``false``.
     * @param rank spearman (rank) based copula instead of the pearson one
     * 
     * + default value Is ``false``.
     * @param standardize z-score the expression vectors before the inference
     * 
     * + default value Is ``false``.
     * @param prior the curated ``TF -> target`` evidence: the value of the ``bnlearn::prior_network``
     *  api (a [RegulatoryEdge](cref:T:SMRUCC.genomics.BioSystem.Core.RegulatoryEdge) vector or pipeline) can be passed directly.
     *  Only the edges whose confidence reaches **prior_min_confidence**
     *  are used, and the ``RegulationType`` (activator/inhibitor) is exported as the
     *  ``regulation`` column of the edge table.
     * 
     * + default value Is ``null``.
     * @param prior_min_confidence the eligibility threshold of the prior edges, default 0.45
     * 
     * + default value Is ``0.45``.
     * @param prior_factor the threshold relaxation factor at the full confidence, default 0.5
     * 
     * + default value Is ``0.5``.
     * @param prior_protection ``graded`` (default), ``sampled`` or ``off``
     * 
     * + default value Is ``'graded'``.
     * @param prior_strength the strength of the graded protection, default 0.5
     * 
     * + default value Is ``0.5``.
     * @param prior_tf_augment append the prior regulators into the TF set
     * 
     * + default value Is ``true``.
     * @param threads the max degree of parallelism, a value that is less than or equals to 0 means "auto"
     * 
     * + default value Is ``-1``.
     * @param seed the random seed, the result is reproducible for a fixed seed
     * 
     * + default value Is ``1234567``.
     * @param verbose print the statistics, the top regulons and the top edges
     * 
     * + default value Is ``true``.
     * @param env the R# runtime environment object
     * 
     * + default value Is ``null``.
     * @return the fitted [ARACNETools.aracneNetwork()](cref:M:TRNtoolkit.ARACNETools.aracneNetwork(SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix,System.Object,System.String,System.String,System.Double,System.Double,System.Int32,System.String,System.Double,System.String,System.Int32,System.Int32,System.Int32,System.Double,System.Boolean,System.Boolean,System.Boolean,System.Object,System.Double,System.Double,System.String,System.Double,System.Boolean,System.Int32,System.Int32,System.Boolean,SMRUCC.Rsharp.Runtime.Environment)) object. Use the ``edges``/``regulons``/
     *  ``summary``/``as_graph`` api to inspect it, and the ``write_edges`` api to export it.
   */
   function aracne(x: object, TF?: any, estimator?: string, threshold?: string, mi_cutoff?: number, quantile?: number, permutations?: object, dpi?: string, epsilon?: number, direction?: string, bins?: object, max_neighbors?: object, max_genes?: object, min_nonzero?: number, log2?: boolean, rank?: boolean, standardize?: boolean, prior?: any, prior_min_confidence?: number, prior_factor?: number, prior_protection?: string, prior_strength?: number, prior_tf_augment?: boolean, threads?: object, seed?: object, verbose?: boolean, env?: object): object;
   /**
    * Create an [AracneOptions](cref:T:SMRUCC.genomics.Model.Network.Regulons.ARACNE.AracneOptions) object from the scalar arguments.
    * 
    * 
     * @param estimator the mutual information estimator: ``copula`` (default, ``O(n)`` per gene pair),
     *  ``adaptive`` (the adaptive partitioning of the ARACNE paper), ``histogram``
     *  (equal frequency bins with the Miller-Madow correction) or ``kernel``
     *  (gaussian kernel density, ``O(n^2)``).
     * 
     * + default value Is ``'copula'``.
     * @param threshold ``permutation`` (default, the quantile of the null distribution that is created
     *  by shuffling the samples of each gene) or ``fixed`` (use **mi_cutoff**).
     * 
     * + default value Is ``'permutation'``.
     * @param mi_cutoff the fixed mutual information threshold; a positive value forces the ``fixed``
     *  mode even when **threshold** is ``permutation``.
     * 
     * + default value Is ``0``.
     * @param quantile the quantile of the permutation null distribution, default 0.95
     * 
     * + default value Is ``0.95``.
     * @param permutations the number of the gene pairs that are sampled for the null distribution,
     *  a value that is less than or equals to 0 means "auto".
     * 
     * + default value Is ``0``.
     * @param dpi ``auto`` (TF guided when a TF list is given, otherwise standard),
     *  ``standard``, ``tf`` (TF guided) or ``off``.
     * 
     * + default value Is ``'auto'``.
     * @param epsilon the DPI tolerance, default 0.1
     * 
     * + default value Is ``0.1``.
     * @param direction ``tf`` (default) or ``undirected``
     * 
     * + default value Is ``'tf'``.
     * @param bins the bin number of the ``histogram`` estimator, default 4
     * 
     * + default value Is ``4``.
     * @param max_neighbors the max number of the edges that each regulator can keep (top-K pruning),
     *  a value that is less than or equals to 0 means "no pruning".
     * 
     * + default value Is ``0``.
     * @param max_genes keep only the top variant genes; the genes of the TF list and of the prior
     *  network are always kept.
     * 
     * + default value Is ``0``.
     * @param min_nonzero the minimal ratio of the non-zero samples for a gene to be kept
     * 
     * + default value Is ``0``.
     * @param log2 apply a ``log2(x + 1)`` transformation before the inference
     * 
     * + default value Is ``false``.
     * @param rank use the ranks (spearman) instead of the values (pearson)
     * 
     * + default value Is ``false``.
     * @param standardize z-score the expression vectors before the inference
     * 
     * + default value Is ``false``.
     * @param prior_min_confidence the eligibility threshold of the curated prior edges, default 0.45 (i.e. Low and above)
     * 
     * + default value Is ``0.45``.
     * @param prior_factor the threshold relaxation factor at the full confidence, default 0.5;
     *  the prior edge of the confidence ``c`` uses ``tau * (1 - (1 - factor) * c)``.
     * 
     * + default value Is ``0.5``.
     * @param prior_protection ``graded`` (default: the DPI protection window is scaled by the confidence),
     *  ``sampled`` (a prior edge is fully protected with the probability ``p = c``)
     *  or ``off``.
     * 
     * + default value Is ``'graded'``.
     * @param prior_strength the strength of the graded protection, default 0.5
     * 
     * + default value Is ``0.5``.
     * @param prior_tf_augment append the regulators of the prior network into the TF set, so that a incomplete
     *  TF annotation does not lose the regulation edges.
     * 
     * + default value Is ``true``.
     * @param threads the max degree of parallelism, a value that is less than or equals to 0 means "auto"
     * 
     * + default value Is ``-1``.
     * @param seed the random seed of the permutation sampling and of the sampled protection
     * 
     * + default value Is ``1234567``.
     * @param silent do not print the progress messages
     * 
     * + default value Is ``true``.
     * @return the options object that can be used by the ``infer`` api
   */
   function aracne_options(estimator?: string, threshold?: string, mi_cutoff?: number, quantile?: number, permutations?: object, dpi?: string, epsilon?: number, direction?: string, bins?: object, max_neighbors?: object, max_genes?: object, min_nonzero?: number, log2?: boolean, rank?: boolean, standardize?: boolean, prior_min_confidence?: number, prior_factor?: number, prior_protection?: string, prior_strength?: number, prior_tf_augment?: boolean, threads?: object, seed?: object, silent?: boolean): object;
   /**
    * Convert the inferred network into a graph model, so that it can be exported as a
    *  network file or be visualized by the ``igraph`` package.
    * 
    * 
     * @param net the network that is created via the ``aracne`` api
     * @return a [NetworkGraph](cref:T:Microsoft.VisualBasic.Data.visualize.Network.Graph.NetworkGraph) object: the nodes are the genes of the network, the
     *  edges carry the ``interaction_type`` (``directed``/``mutual``/``undirected``),
     *  the ``prior`` flag and the ``regulation`` type (``activator``/``inhibitor``) metadata.
   */
   function as_graph(net: object): object;
   /**
    * Get the mutual information matrix of the inferred network (the alias of ``mi_matrix``).
    * 
    * 
     * @param net the network that is created via the ``aracne`` api
   */
   function coef(net: object): object;
   /**
    * Get the DPI (data processing inequality) filtering statistics of the network.
    * 
    * 
     * @param net the network that is created via the ``aracne`` api
     * @return a single row data frame with the columns: ``edges_before``, ``edges_after``,
     *  ``removed``, ``rescued`` (the prior protected edges), ``triplets`` and ``passes``;
     *  the ``rescued_high``, ``rescued_medium`` and ``rescued_low`` columns report the
     *  rescued edges by the prior confidence grade.
   */
   function dpi_stats(net: object): object;
   /**
    * Get the regulation edges of the inferred network as a data frame.
    * 
    * 
     * @param net the network that is created via the ``aracne`` api
     * @return a data frame with the columns: ``regulator``, ``target``, ``mi``, ``correlation``,
     *  ``interaction``, ``is_tf_source``, ``is_tf_target``, ``prior`` and ``regulation``.
     *  The rows are sorted by the mutual information in the descending order.
   */
   function edges(net: object): object;
   /**
    * Get the gene ids of the network, in the row order of the internal expression vectors.
    * 
    * 
     * @param net the network that is created via the ``aracne`` api
     * @return a character vector of the gene ids
   */
   function gene_names(net: object): string;
   /**
    * Run the inference with a pre-built [AracneOptions](cref:T:SMRUCC.genomics.Model.Network.Regulons.ARACNE.AracneOptions) object.
    * 
    * 
     * @param x the gene expression matrix, rows are the genes and columns are the samples
     * @param opts the options object that is created via the ``aracne_options`` api
     * 
     * + default value Is ``null``.
     * @return the fitted [ARACNETools.aracneNetwork()](cref:M:TRNtoolkit.ARACNETools.aracneNetwork(SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix,System.Object,System.String,System.String,System.Double,System.Double,System.Int32,System.String,System.Double,System.String,System.Int32,System.Int32,System.Int32,System.Double,System.Boolean,System.Boolean,System.Boolean,System.Object,System.Double,System.Double,System.String,System.Double,System.Boolean,System.Int32,System.Int32,System.Boolean,SMRUCC.Rsharp.Runtime.Environment)) object
   */
   function infer(x: object, opts?: object): object;
   /**
    * Get the mutual information matrix of the inferred network.
    * 
    * 
     * @param net the network that is created via the ``aracne`` api
     * @return a matrix whose rows are the regulators and columns are the targets, an element is
     *  the mutual information (bits) of the corresponding gene pair.
   */
   function mi_matrix(net: object): object;
   /**
    * Load the curated ``TF -> target`` prior network from a csv table.
    * 
    * 
     * @param file the file path of the curated prior table, the header must contain the ``TF`` and
     *  the ``TargetGene`` columns; the optional ``Confidence`` column (``High``/
     *  ``Medium``/``Low`` or a numeric value) and the optional ``RegulationType`` column
     *  (``Activator``/``Inhibitor``) are recognized as well.
     * @param min_confidence the eligibility threshold of the prior confidence, default 0.45
     * 
     * + default value Is ``0.45``.
     * @param env the R# runtime environment object
     * 
     * + default value Is ``null``.
     * @return a [RegulatoryEdge](cref:T:SMRUCC.genomics.BioSystem.Core.RegulatoryEdge) vector that can be passed to the ``prior`` argument of ``aracne``
   */
   function read_prior(file: string, min_confidence?: number, env?: object): object;
   /**
    * Load the transcription factor gene symbols from a TF annotation table
    *  (for example the ``Homo_sapiens_TF.txt`` of the AnimalTFDB style).
    * 
    * 
     * @param file the file path of the TF annotation table: a tab/csv separated table whose header
     *  contains the **column** name, or a plain text file with one gene
     *  symbol per line. The empty/``NA`` symbols are skipped and the result is unique
     *  in case insensitive.
     * @param column the column name of the gene symbol, default ``Symbol``
     * 
     * + default value Is ``'Symbol'``.
     * @param env the R# runtime environment object
     * 
     * + default value Is ``null``.
     * @return a character vector of the TF symbols
   */
   function read_tf(file: string, column?: string, env?: object): string;
   /**
    * Get the regulons (the target gene count of each regulator) of the network.
    * 
    * 
     * @param net the network that is created via the ``aracne`` api
     * @return a data frame with the columns ``regulator``, ``size`` (the number of the targets)
     *  and ``mean_mi`` (the mean mutual information of the regulon), sorted by the size
     *  in the descending order.
   */
   function regulons(net: object): object;
   /**
    * Get the inference statistics of the network.
    * 
    * 
     * @param net the network that is created via the ``aracne`` api
     * @return a single row data frame with the columns: ``genes_scanned``, ``genes``, ``samples``,
     *  ``tf``, ``pairs``, ``threshold``, ``edges_threshold``, ``edges_dpi``, ``edges``,
     *  ``prior_edges``, ``prior_supported``, ``prior_rescued``, ``seconds``.
   */
   function summary(net: object): object;
   /**
    * Get the transcription factors that are matched by the network (the regulators).
    * 
    * 
     * @param net the network that is created via the ``aracne`` api
     * @return a character vector of the TF gene ids
   */
   function tf_ids(net: object): string;
   /**
    * Get the significance threshold (``tau``) information of the network.
    * 
    * 
     * @param net the network that is created via the ``aracne`` api
     * @return a single row data frame with the columns: ``mode``, ``value``, ``quantile``,
     *  ``null_mean``, ``null_sd``, ``null_max``, ``samples``, ``replicates`` and ``signal_ratio``.
   */
   function threshold(net: object): object;
   /**
    * Export the regulation edges of the network into a csv file.
    * 
    * 
     * @param net the network that is created via the ``aracne`` api
     * @param file the output csv file, its folder will be created automatically
   */
   function write_edges(net: object, file: string): ;
}
