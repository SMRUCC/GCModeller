// export R# package module type define for javascript/typescript language
//
//    imports "KOBAS" from "gseakit";
//
// ref=gseakit.KOBAS@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * ### KOBAS toolkit
 *  
 *  The KOBAS (KEGG Orthology Based Annotation System) enrichment analysis toolkit.
 *  This package provides the VB.NET port of the KOBAS 3.0 enrichment calculation
 *  algorithms:
 *  
 *  + ``kobas.gsea``: GSEA analysis of a expression matrix against a gene set collection,
 *  + ``kobas.identify``: four statistical enrichment tests (hypergeometric / Fisher /
 *    binomial / chi-square) for a foreground gene list against a background gene list,
 *  + ``kobas.plage`` / ``kobas.gage``: SNBEA style pathway level analysis,
 *  + ``kobas.term_similarity`` / ``kobas.term_cluster``: term similarity and clustering
 *    of the enrichment result terms,
 *  + ``kobas.fdr``: multiple testing correction (BH / BY / q-value),
 *  + ``kobas.fisher_test`` / ``kobas.hyper_test`` / ``kobas.binom_test`` / ``kobas.chisq_test``:
 *    the exact statistical tests used by the enrichment analysis.
 * 
 * > The translated algorithms follow the standard GSEA / Fisher definitions rather than
 * >  the original KOBAS implementation where the original code deviates from the standard
 * >  definition (for example the miss step of the running enrichment score or the
 * >  construction of the 2x2 contingency table). See the remarks of the individual
 * >  functions for the detailed notes.
*/
declare namespace KOBAS {
   module kobas {
      /**
       * Load a gene set collection from a GMT file as a GSEA background model.
       * 
       * > The ``size`` property of the returned background model is the union size of all
       * >  gene set members, it is NOT the number of genes in the expression matrix. The
       * >  GSEA analysis always uses the number of genes in the ranked list as the
       * >  population size ``N``.
       * 
        * @param gmt the GMT file path.
        * @param min_size the minimum number of members of the gene set, gene sets smaller than this
        *  value will be removed. Default ``0`` means no filtering.
        * 
        * + default value Is ``0``.
        * @param max_size the maximum number of members of the gene set, gene sets larger than this
        *  value will be removed. Default ``2147483647`` means no filtering.
        * 
        * + default value Is ``2147483647``.
        * @param duplicate the strategy for the duplicated gene set id in the GMT file, could be
        *  ``error`` (default, throw an exception) or ``keep`` (keep the first one).
        * 
        * + default value Is ``'error'``.
        * @return a [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model.
      */
      function background(gmt: string, min_size?: object, max_size?: object, duplicate?: string): object;
      /**
       * The exact binomial test (upper tail) of a term.
       * 
       * 
        * @param m1 the number of the term members in the foreground list.
        * @param n1 the size of the foreground gene list.
        * @param m2 the number of the term members in the background.
        * @param n2 the size of the background gene list.
        * @return the p-value.
      */
      function binom_test(m1: object, n1: object, m2: object, n2: object): number;
      /**
       * The chi-square test (Yates continuity correction) of a 2x2 contingency table.
       * 
       * 
        * @param a the upper left cell.
        * @param b the upper right cell.
        * @param c the lower left cell.
        * @param d the lower right cell.
        * @return the p-value; the Fisher exact test will be applied automatically when any of the
        *  expected frequencies is less than 10.
      */
      function chisq_test(a: object, b: object, c: object, d: object): number;
      /**
       * Multiple testing correction of the p-values.
       * 
       * > The q-value is estimated with a fixed lambda (0.5) of the Storey's pi0 estimator,
       * >  the R ``qvalue`` package uses a smoothed lambda grid instead, so the result may
       * >  differ slightly from the R package.
       * 
        * @param pvalue the raw p-values.
        * @param method the correction method: ``BH`` (Benjamini and Hochberg, default), ``BY``
        *  (Benjamini and Yekutieli), ``QVALUE`` (Storey) or ``None``.
        * 
        * + default value Is ``'BH'``.
        * @return the adjusted p-values in the same order as the input.
      */
      function fdr(pvalue: number, method?: string): number;
      /**
       * The Fisher exact test (greater / enrichment tail) of a 2x2 contingency table.
       * 
       * 
        * @param a the upper left cell: the members of the term in the foreground list.
        * @param b the upper right cell: the foreground members that are not in the term.
        * @param c the lower left cell: the term members that are not in the foreground.
        * @param d the lower right cell: the members that are neither in the term nor in the foreground.
        * @return the p-value.
      */
      function fisher_test(a: object, b: object, c: object, d: object): number;
      /**
       * ### GAGE style gene set enrichment analysis (KOBAS SNBEA)
       *  
       *  Aggregate the gene level t statistics of each gene set into a gene set level
       *  z-score (``sum(t) / sqrt(n)``) and estimate the significance from the upper tail
       *  of the standard normal distribution.
       * 
       * > The returned p-value is the upper tail probability, that means the gene set is
       * >  enriched in the second phenotype group (the samples labeled with ``1``).
       * 
        * @param expr the expression matrix (matrix object, ``dataframe`` or expression matrix file path).
        * @param cls the phenotype class labels, same as the ``cls`` parameter of ``kobas.gsea``.
        * @param gmt the gene set collection: a [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model or a GMT file path.
        * @param min_size the minimum number of the mapped members of a gene set, default ``5``.
        * 
        * + default value Is ``5``.
        * @param max_size the maximum number of the mapped members of a gene set, default ``500``.
        * 
        * + default value Is ``500``.
        * @param fdr the multiple testing correction method, default ``BH``.
        * 
        * + default value Is ``'BH'``.
        * @param data_type the data type of the expression matrix: ``rseq`` (default) or ``ma``.
        * 
        * + default value Is ``'rseq'``.
        * @param normalize performs the data normalization before the analysis? Default ``TRUE``.
        * 
        * + default value Is ``true``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return a ``dataframe`` with the gene set id as the row names and the columns: ``name``,
        *  ``description``, ``size``, ``statistic``, ``pvalue``, ``p_adjust`` and ``genes``.
      */
      function gage(expr: any, cls: any, gmt: any, min_size?: object, max_size?: object, fdr?: string, data_type?: string, normalize?: boolean, env?: object): object;
      /**
       * ### Run the KOBAS style GSEA analysis
       *  
       *  Gene Set Enrichment Analysis with the KOBAS ranking and permutation scheme: the
       *  genes are ranked by the signal-to-noise ratio or the t statistics between the
       *  two phenotype groups, then a weighted Kolmogorov-Smirnov like running sum is
       *  evaluated for each gene set. The significance of the enrichment score is
       *  estimated by permuting the phenotype class labels.
       * 
       * > A positive ES means that the gene set is enriched in the first phenotype group
       * >  (the samples labeled with ``0``) because the KOBAS ranking score is
       * >  ``mean(group0) - mean(group1)``.
       * 
        * @param expr the expression matrix, rows are genes and columns are samples. The value could be
        *  a matrix object, a ``dataframe`` object (rows are genes and columns are samples),
        *  or a file path of the ``gct`` / KOBAS plain text expression matrix.
        * @param cls the phenotype class labels: could be a ``phenotype`` object, a ``CLS`` file path,
        *  an integer vector (``0`` for the first phenotype group and ``1`` for the second
        *  group), or a character vector with exactly two distinct class labels.
        * @param gmt the gene set collection: a [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model or a GMT file path.
        * @param method the gene ranking method: ``snr`` (signal to noise ratio, default) or ``ttest``.
        * 
        * + default value Is ``'snr'``.
        * @param weight the weight of the enrichment score, ``0`` for the unweighted Kolmogorov-Smirnov
        *  statistic, ``1`` (default) for the classic GSEA weighting and ``2`` for the
        *  over-weighted score. Any other positive value ``p`` will use ``|rank|^p``.
        * 
        * + default value Is ``1``.
        * @param permutations the number of the phenotype label permutations, default ``1000``. Set to ``0`` for
        *  skipping the significance estimation (then the ``pvalue`` and ``FDR`` columns of
        *  the result table will be ``NA`` and ``NES`` equals to ``ES``).
        * 
        * + default value Is ``1000``.
        * @param min_size the minimum number of the mapped members of a gene set, default ``15``.
        * 
        * + default value Is ``15``.
        * @param max_size the maximum number of the mapped members of a gene set, default ``500``.
        * 
        * + default value Is ``500``.
        * @param seed the random seed of the permutation, default ``20261026``.
        * 
        * + default value Is ``20261026``.
        * @param keep_running_sum keeps the running sum of each gene set in the result objects? Default ``FALSE``
        *  because the running sum occupies a lot of memory for large gene set collections.
        * 
        * + default value Is ``false``.
        * @param verbose prints the progress message? Default ``TRUE``.
        * 
        * + default value Is ``true``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return a ``dataframe`` with the gene set id as the row names and the columns:
        *  ``name``, ``description``, ``size``, ``ES``, ``NES``, ``pvalue``, ``FDR``,
        *  ``es_index``, ``leading_edge`` (the hit genes before/after the enrichment score
        *  position, joined by ``;``) and ``hit_genes``.
      */
      function gsea(expr: any, cls: any, gmt: any, method?: string, weight?: number, permutations?: object, min_size?: object, max_size?: object, seed?: object, keep_running_sum?: boolean, verbose?: boolean, env?: object): object;
      /**
       * The hypergeometric test (upper tail) of a term.
       * 
       * 
        * @param q the observed number of the term members in the foreground list.
        * @param m the number of the term members in the background.
        * @param n the total number of the genes in the background.
        * @param k the size of the foreground gene list.
        * @return the p-value.
      */
      function hyper_test(q: object, m: object, n: object, k: object): number;
      /**
       * ### Run the KOBAS style over-representation (enrichment) analysis
       *  
       *  Perform the over-representation analysis with the KOBAS statistical tests: the
       *  foreground gene list is mapped onto the given gene set collection, then the
       *  members of each term are compared with the background gene list by the
       *  hypergeometric test, the Fisher exact test, the binomial test or the chi-square
       *  test.
       * 
       * 
        * @param foreground the foreground gene list: a character vector of the gene ids, or the file path of
        *  a KOBAS ``annotate`` output file (the ``query`` column will be used).
        * @param background the background gene list: a character vector of the gene ids, or the file path of
        *  a KOBAS ``annotate`` output file. It must be a superset of the
        *  **foreground** for the Fisher and the chi-square test.
        * @param gmt the gene set collection: a [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model or a GMT file path.
        * @param method the statistical test: ``h`` for the Fisher exact test (default, also the
        *  hypergeometric test), ``b`` for the binomial test, ``c`` for the chi-square test
        *  and ``x`` for the frequency list only.
        * 
        * + default value Is ``'h'``.
        * @param fdr the multiple testing correction: ``BH`` (default), ``BY``, ``QVALUE`` or ``None``.
        * 
        * + default value Is ``'BH'``.
        * @param cutoff the minimum number of the background members of a term, terms with fewer
        *  background members are not used in the statistical test. Default ``5``.
        * 
        * + default value Is ``5``.
        * @param database the database label that will be written into the ``database`` column, default ``K``
        *  (KEGG PATHWAY).
        * 
        * + default value Is ``'K'``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return a ``dataframe`` with the term id as the row names and the columns:
        *  ``name``, ``database``, ``input_number``, ``background_number``, ``pvalue``,
        *  ``p_adjust`` and ``input`` (the enriched gene ids joined by ``;``).
      */
      function identify(foreground: any, background: any, gmt: any, method?: string, fdr?: string, cutoff?: object, database?: string, env?: object): object;
      /**
       * ### PLAGE pathway level analysis (KOBAS SNBEA)
       *  
       *  Score each gene set of the expression matrix with the PLAGE method (the first
       *  right singular vector of the row standardized gene set submatrix), then compare
       *  the activity scores between the two phenotype groups.
       * 
       * > The differential analysis uses the classic Welch two sample t-test instead of the
       * >  limma moderated t-test because there is no moderated t implementation in the
       * >  GCModeller core library, so the p-values are slightly more conservative than the
       * >  limma result.
       * 
        * @param expr the expression matrix (matrix object, ``dataframe`` or expression matrix file path).
        * @param cls the phenotype class labels, same as the ``cls`` parameter of ``kobas.gsea``.
        * @param gmt the gene set collection: a [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model or a GMT file path.
        * @param min_size the minimum number of the mapped members of a gene set, default ``5``.
        * 
        * + default value Is ``5``.
        * @param max_size the maximum number of the mapped members of a gene set, default ``500``.
        * 
        * + default value Is ``500``.
        * @param fdr the multiple testing correction method, default ``BH``.
        * 
        * + default value Is ``'BH'``.
        * @param data_type the data type of the expression matrix: ``rseq`` (RNA-seq read counts, default) or
        *  ``ma`` (microarray data).
        * 
        * + default value Is ``'rseq'``.
        * @param normalize performs the data normalization before the analysis? Default ``TRUE``: the RNA-seq
        *  data will be transformed by the log2 CPM with the prior count and the microarray
        *  data will be quantile normalized.
        * 
        * + default value Is ``true``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return a ``dataframe`` with the gene set id as the row names and the columns: ``name``,
        *  ``description``, ``size``, ``statistic``, ``pvalue``, ``p_adjust`` and ``genes``.
      */
      function plage(expr: any, cls: any, gmt: any, min_size?: object, max_size?: object, fdr?: string, data_type?: string, normalize?: boolean, env?: object): object;
      /**
       * Read a KOBAS ``annotate`` output file as a ``dataframe``.
       * 
       * 
        * @param file the ``annotate`` output file path.
        * @return a ``dataframe`` with the query id as the row names and the columns ``links``
        *  (the mapped gene ids joined by ``;``) and ``size``.
      */
      function read_annotate(file: string): object;
      /**
       * Cluster the given terms by the pairwise similarity.
       * 
       * > The cluster self test of the KOBAS algorithm requires more than 3 pairwise
       * >  similarities, that means a cluster should contain at least 4 terms.
       * 
        * @param terms a named ``list`` of the character vectors, see the ``terms`` parameter of
        *  ``kobas.term_similarity``.
        * @param threshold the similarity threshold of the term cluster, default ``0.35``.
        * 
        * + default value Is ``0.35``.
        * @param method the similarity method: ``kappa`` (default) or ``jaccard``.
        * 
        * + default value Is ``'kappa'``.
        * @param universe the gene universe size of the kappa method, default ``-1`` (auto).
        * 
        * + default value Is ``-1``.
        * @param pvalue an optional numeric vector of the p-values of the terms, it is used for scoring the
        *  term cluster (the geometric mean of ``-log10(p)``). Default ``NULL`` means ``NA``.
        * 
        * + default value Is ``null``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return a ``dataframe`` with the cluster index as the row names and the columns:
        *  ``size`` (the number of the terms in the cluster), ``score``, ``members`` (the term
        *  ids joined by ``;``) and ``index`` (the term index in the input term list).
      */
      function term_cluster(terms: object, threshold?: number, method?: string, universe?: object, pvalue?: any, env?: object): object;
      /**
       * Compute the pairwise similarity matrix of the given term list.
       * 
       * 
        * @param terms a named ``list`` of the character vectors, the name of each slot is the term id
        *  and the value is the member gene id vector of that term (for example the result
        *  of ``as.geneSet`` of a gsea background model).
        * @param method the similarity method: ``kappa`` (Cohen's kappa, default) or ``jaccard``.
        * 
        * + default value Is ``'kappa'``.
        * @param universe the size of the gene universe that is used by the kappa method. Default ``-1``
        *  means the union size of all of the given terms.
        * 
        * + default value Is ``-1``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return a ``dataframe`` of the similarity matrix, both the row names and the column names
        *  are the term ids and the diagonal values are ``1``.
      */
      function term_similarity(terms: object, method?: string, universe?: object, env?: object): object;
   }
   module read {
      /**
       * Read KOBAS enrichment output result table file
       * 
       * 
        * @param file -
      */
      function KEGGpathway(file: string): object;
   }
}
