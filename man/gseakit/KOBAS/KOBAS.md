# KOBAS

### KOBAS toolkit
 
 The KOBAS (KEGG Orthology Based Annotation System) enrichment analysis toolkit.
 This package provides the VB.NET port of the KOBAS 3.0 enrichment calculation
 algorithms:
 
 + ``kobas.gsea``: GSEA analysis of a expression matrix against a gene set collection,
 + ``kobas.identify``: four statistical enrichment tests (hypergeometric / Fisher /
   binomial / chi-square) for a foreground gene list against a background gene list,
 + ``kobas.plage`` / ``kobas.gage``: SNBEA style pathway level analysis,
 + ``kobas.term_similarity`` / ``kobas.term_cluster``: term similarity and clustering
   of the enrichment result terms,
 + ``kobas.fdr``: multiple testing correction (BH / BY / q-value),
 + ``kobas.fisher_test`` / ``kobas.hyper_test`` / ``kobas.binom_test`` / ``kobas.chisq_test``:
   the exact statistical tests used by the enrichment analysis.
> The translated algorithms follow the standard GSEA / Fisher definitions rather than
>  the original KOBAS implementation where the original code deviates from the standard
>  definition (for example the miss step of the running enrichment score or the
>  construction of the 2x2 contingency table). See the remarks of the individual
>  functions for the detailed notes.

+ [read.KEGGpathway](KOBAS/read.KEGGpathway.1) Read KOBAS enrichment output result table file
+ [kobas.background](KOBAS/kobas.background.1) Load a gene set collection from a GMT file as a GSEA background model.
+ [kobas.gsea](KOBAS/kobas.gsea.1) ### Run the KOBAS style GSEA analysis
+ [kobas.identify](KOBAS/kobas.identify.1) ### Run the KOBAS style over-representation (enrichment) analysis
+ [kobas.read_annotate](KOBAS/kobas.read_annotate.1) Read a KOBAS ``annotate`` output file as a ``dataframe``.
+ [kobas.fdr](KOBAS/kobas.fdr.1) Multiple testing correction of the p-values.
+ [kobas.plage](KOBAS/kobas.plage.1) ### PLAGE pathway level analysis (KOBAS SNBEA)
+ [kobas.gage](KOBAS/kobas.gage.1) ### GAGE style gene set enrichment analysis (KOBAS SNBEA)
+ [kobas.term_similarity](KOBAS/kobas.term_similarity.1) Compute the pairwise similarity matrix of the given term list.
+ [kobas.term_cluster](KOBAS/kobas.term_cluster.1) Cluster the given terms by the pairwise similarity.
+ [kobas.fisher_test](KOBAS/kobas.fisher_test.1) The Fisher exact test (greater / enrichment tail) of a 2x2 contingency table.
+ [kobas.hyper_test](KOBAS/kobas.hyper_test.1) The hypergeometric test (upper tail) of a term.
+ [kobas.binom_test](KOBAS/kobas.binom_test.1) The exact binomial test (upper tail) of a term.
+ [kobas.chisq_test](KOBAS/kobas.chisq_test.1) The chi-square test (Yates continuity correction) of a 2x2 contingency table.
