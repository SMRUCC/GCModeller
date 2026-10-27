# Figure legends — moGSVA applied to TCGA-BRCA

All panels are English-labelled and sized for a 183 mm double-column layout.
Each legend gives the panel content, the statistic shown and the sample size,
so the text can be pasted into a manuscript with minimal editing.

Common data description (may be shortened once, in the Methods section):
*Multi-omics GSVA (moGSVA) scores for 335 KEGG pathways in 460 primary
TCGA-BRCA tumours, computed from copy-number alteration (CNA, 22,232
features), DNA methylation (16,111), transcriptome (20,431) and proteome
(181) layers. Layer-specific enrichment scores were computed inside each
layer (GSVA, Gaussian kernel), fused with the feature-balanced weighting
scheme (lambda = 0.25) and filtered to gene sets with 5-500 mapped members.
Abbreviations: ES, enrichment score; ES_raw, weighted average before the
consistency reward; ES_multi, fused score after the reward; BH,
Benjamini-Hochberg.*

---

## Figure 1 | Input landscape and pathway coverage across four omics layers

**A,** Number of molecular features assayed in each layer (logarithmic axis);
all layers share the same 460 tumours. **B,** Number of KEGG pathway members
actually measured per layer (violin, log1p axis) with the box plot inside;
labels below the axis give the number of pathways with at least one measured
member out of 335. **C,** Number of layers contributing to each pathway after
the size filter: 156 pathways (46.6%) are supported by all four layers, 175
(52.2%) by three, 3 by two and 1 by a single layer. **D,** Distribution of the
per-pathway fusion weights for pathways supported by at least two layers
(box, outliers as dots, open diamond = mean); the feature-balanced rule
w = 1/log2(2+|S_k|) gives a mean weight of 0.295 for CNA, methylation and
transcriptome and 0.252 for the proteome. The proteome is the coverage-limiting
layer (158/335 pathways, median 0 members per pathway).

## Figure 2 | The fused multi-omics pathway activity landscape

**A,** Distribution of ES_multi across the 460 tumours for the ten most
variable pathways (violin; open dot = median; dashed line = 0). Distributions
are frequently bimodal, reflecting tumour subsets rather than symmetric noise.
**B,** Per-pathway standard deviation before (ES_raw) and after the consistency
reward (ES_multi); points on the diagonal would indicate no attenuation, and the
median reduction is 9.5% of the SD. **C,** Heatmap of the 40 most variable
pathways (rows) across all 460 tumours (columns), row-scaled to z-scores and
clipped at +/-2.5. Columns are hierarchically clustered and split into three
groups; rows are split into three clusters. Annotation bars on top encode ER
and PR status (immunohistochemistry), HER2 status and the IHC-derived subtype.
The bottom block of pathways (immune and inflammatory gene sets) is coordinately
activated in a subset of tumours that is enriched for ER-negative disease.

## Figure 3 | Cross-omics agreement and its effect on the activity scores

**A,** Distribution of the per-sample sign agreement a, the mean pairwise sign
agreement of the contributing layers; 50.0% of the 154,100 pathway x tumour
observations have a = -1/3, 23.3% have a = 0, 26.3% are fully concordant
(a = 1) and 0.4% are fully contradictory (a = -1). **B,** Composition of a
stratified by the number of contributing layers, showing that the attainable
values of a are quantised by K (K(K-1)/2 + 1 distinct values), so agreement is
not directly comparable across pathways with different coverage. **C,** Median
per-pathway Pearson correlation between the per-omics enrichment scores of each
layer pair (numbers below the diagonal coefficient give the number of pathways
used). Methylation and transcriptome are the most concordant pair (median
r = 0.32); proteome scores are essentially uncorrelated with the other layers
(median r between -0.03 and 0.01), so the layers carry partly independent
information. **D,** Pathway dispersion against mean agreement (one point per
pathway; line = linear fit, band = 95% CI); pathways whose layers agree vary
more across tumours (Spearman rho = 0.60), which is the intended behaviour of
the reward term.

## Figure 4 | Decomposing the fused score into per-omics contributions

**A,** Relative contribution of each layer to ES_multi for the 20 most variable
pathways supported by at least two layers (mean |contribution share| over
tumours, normalised to 100% per pathway). None of these 20 pathways has
proteomic coverage. **B,** Share of all 154,100 pathway x tumour observations in
which each layer is the single largest contributor: transcriptome 34.3%,
proteome 25.7%, methylation 21.7%, CNA 18.2%. The proteome dominates a quarter
of the observations despite covering only 158 pathways, because its small
member counts receive larger feature-balanced weights. **C,** Share of tumours
in which each layer dominates, for the 24 most variable pathways, split by the
leading layer (numbers = percentage of tumours; grey cells = below 5%). The
leading layer is pathway specific: DNA replication and the proteasome are
transcriptome-dominated (74% and 68% of tumours), whereas systemic lupus
erythematosus and phosphate/phosphonate metabolism are methylation- and
CNA-dominated (58% and 53%).

## Figure 5 | Ablation against a naive pooled single-omics GSVA

**A,** Two-dimensional binning (90 x 90) of ES_multi against the pooled
baseline that runs GSVA on the concatenated matrix (285 pathways scored by both
methods, 131,100 observations). Dashed line = identity, solid line = linear fit;
overall Pearson r = 0.51. **B,** Distribution of the per-pathway Pearson
correlation between the two scores (median 0.50, IQR 0.43-0.57), showing that
the discrepancy is pathway specific rather than a global rescaling. **C,**
Per-pathway correlation of each score with the layer-specific enrichment scores
(box plots; number of pathways below the layer name). The pooled baseline
reproduces the transcriptome (median r = 0.65) but is uncorrelated with, or
anti-correlated to, methylation (median r = -0.20) and almost uninformative for
the proteome (0.16), whereas the fused score recovers every layer (CNA 0.48,
methylation 0.65, transcriptome 0.71, proteome 0.65). **D,** Representative
pathways with the lowest (maturity onset diabetes of the young, r = 0.13) and
highest (phosphonate and phosphinate metabolism, r = 0.86) agreement between the
two methods.

## Figure 6 | Permutation-based significance and the determinants of power

**A,** Observed test statistic max_j |ES_multi| for the 15 strongest of the 40
pathways that entered the permutation test (100 permutations; colour = -log10 P).
No pathway reaches FDR < 0.05: the smallest P is 0.0099 and the corresponding
BH q is 0.198, which is the resolution limit of 100 permutations
(1/(B+1) = 0.0099). **B,** Cumulative distribution of the empirical P values
against the uniform expectation; the excess of small P values is modest.
**C,** The test statistic scales with pathway dispersion (Pearson r = 0.59), so
power is concentrated in the pathways that vary most across tumours. **D,**
Pathway dispersion is itself set by gene-set size: SD decreases with the total
number of mapped members (Spearman rho = -0.68; points coloured by the number of
contributing layers), identifying small gene sets as the noisiest units and the
main caveat for pathway-level inference.

## Figure 7 | Clinical context of the fused pathway activity

**A,** ES_multi by ER status (immunohistochemistry) for the six pathways with
the strongest ER contrast (box plots, whiskers at 1.5 IQR; 324 ER-positive vs
101 ER-negative tumours; two-sided Wilcoxon test, BH-adjusted q shown on the
right). **B,** Volcano plot of the ER contrast for all 335 pathways
(x = difference in median ES_multi, ER+ minus ER-; y = -log10 q; dashed line =
FDR 0.05). 220 of 335 pathways differ at FDR < 0.05, and 151 of these are
higher in ER-negative tumours, consistent with the more disordered pathway
activity of ER-negative disease. **C,** Mean ES_multi across IHC-defined
subtypes for the 18 pathways with the strongest ER effect (numbers below the
subtype names give group sizes; 156 tumours with missing or equivocal HER2 are
not classified and are omitted). **D,** Overall survival of the pathway with the
strongest ER contrast (Hedgehog signalling), split at the median ES_multi
(229 tumours per group, 61 deaths; log-rank P = 0.0084, Cox HR = 0.50,
95% CI 0.30-0.85); ticks mark censored patients.

---

### Notes on reproducibility

* Figure source: `fig1_input_landscape.R` … `fig7_clinical_association.R`,
  shared settings in `00_common.R`, batch entry point `run_all.R`.
* All numbers quoted above are reproduced by `stats_for_legends.R`.
* Colour conventions: one colour per omics layer throughout (CNA #4DBBD5,
  methylation #E64B35, transcriptome #3C5488, proteome #00A087); red/blue in the
  clinical panels always encodes ER-positive / ER-negative status.
* p-values are two-sided unless stated otherwise. No adjustment is applied to the
  survival log-rank test, which is exploratory and based on a single
  pre-specified pathway.
