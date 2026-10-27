# moGSVA result audit

Generated: 2026-10-28 00:32:31
Result directory: `G:/GCModeller/test/demo/HTS/GSVA/brca_tcga_pub2015_mogsva`
Raw data directory: `K:/brca_tcga_pub2015/multipleOmics`

## 1. Export integrity

- **PASS** no missing values in the score matrices — 335 pathways x 460 tumours, all finite
- **PASS** pathway axes aligned — scores / raw_scores / consistency share one row order
- **PASS** sample axis matches samples.tsv — 460 tumour ids in identical order
- **PASS** unique pathway ids — 335 distinct KEGG gene sets
- **PASS** fused scores bounded — range [-0.758, 0.779]
- **PASS** pre-reward scores bounded — range [-0.758, 0.779]
- **PASS** gene-set size filter respected — member counts in [5, 493] (MinSize 5, MaxSize 500)

## 2. Feature-balanced weighting rule

- **PASS** weights follow 1/log2(2+|S_k|) normalised per pathway — 1156 pathway x omics weights, max |observed - predicted| = 1.11e-16
- **PASS** weights sum to one within each pathway — max deviation 2.22e-16 over 335 pathways

Mean weight by layer (multi-layer pathways where the layer is measured):

| Layer | mean w | median w |
|---|---|---|
| Copy number alteration | 0.275 | 0.322 |
| Methylation | 0.283 | 0.333 |
| Transcriptomics | 0.274 | 0.320 |
| Proteomics | 0.363 | 0.361 |

## 3. Fusion algebra

- **PASS** ES_multi = ES_raw x (1 + lambda*a)/(1 + lambda) — max |predicted - exported| = 1.11e-16 (lambda = 0.25)
- **PASS** reward never amplifies a score — max |ES_multi| = 0.779 <= max |ES_raw| = 0.779
- **PASS** sum of per-omics contributions equals ES_raw — max |sum(c_k) - ES_raw| = 5.55e-17 over 154,100 observations
- **PASS** contribution shares are normalised — max |sum(|share|) - 1| = 3.33e-16 over 154,100 observations

## 4. Cross-omics sign agreement

- **PASS** consistency matrix reproduces pairwise sign agreement — max |recomputed - exported| = 0.00e+00 over 153,640 observations
- **PASS** attainable agreement values are discrete — 6 distinct values: -1, -0.3333, -0.1667, 0, 0.5, 1

## 5. Permutation test and multiple testing

- **PASS** empirical p-values sit on the 1/(B+1) grid — implied B = 1000, smallest attainable p = 0.0010
- **PASS** q-values are Benjamini-Hochberg of the exported p-values — max deviation 2.78e-17 over 200 tests

Tested pathways: 200 of 335 (60%); smallest q = 0.057; 11 at q < 0.10; 13 at q < 0.25.

| Rank | Pathway | statistic | P | q |
|---|---|---|---|---|
| 1 | Retinol metabolism | 0.279 | 0.0010 | 0.057 |
| 2 | Ribosome | 0.386 | 0.0010 | 0.057 |
| 3 | Viral protein interaction with cytokine and cytokine receptor | 0.281 | 0.0010 | 0.057 |
| 4 | Metabolism of xenobiotics by cytochrome P450 | 0.227 | 0.0020 | 0.057 |
| 5 | Drug metabolism - cytochrome P450 | 0.256 | 0.0020 | 0.057 |
| 6 | Proteasome | 0.283 | 0.0020 | 0.057 |
| 7 | Neuroactive ligand-receptor interaction | 0.171 | 0.0020 | 0.057 |
| 8 | Citrate cycle (TCA cycle) | 0.295 | 0.0030 | 0.067 |
| 9 | Protein export | 0.320 | 0.0030 | 0.067 |
| 10 | Oxidative phosphorylation | 0.204 | 0.0040 | 0.080 |

## 6. Ablation against the pooled baseline

Common pathways: 285; overall Pearson r = 0.481; per-pathway median r = 0.469 (IQR 0.39-0.55).

| Layer | r with pooled baseline | r with fused score | pathways |
|---|---|---|---|
| Copy number alteration | 0.52 | 0.46 | 285 |
| Methylation | -0.20 | 0.64 | 283 |
| Transcriptomics | 0.65 | 0.68 | 285 |
| Proteomics | 0.16 | 0.82 | 111 |

## 7. Downstream association

ER contrast (324 ER+ vs 101 ER-): 204 of 335 pathways at FDR < 0.05; 61 higher in ER+, 143 higher in ER-.
Overall survival (61 events, 459 tumours): 2 of 335 pathways at FDR < 0.05; smallest P = 1.71e-05 (q = 0.006).

Pathways that separate overall survival at FDR < 0.05:

| Pathway | log-rank P | q |
|---|---|---|
| Synaptic vesicle cycle | 1.71e-05 | 0.006 |
| Endometrial cancer | 2.37e-04 | 0.040 |

## 8. Structural caveats worth reporting

- 1 pathway(s) are supported by a single layer: Metabolic pathways.
- Proteome coverage is the limiting factor: 158 of 335 pathways.
- Pathway dispersion declines with gene-set size (Spearman rho = -0.54).
- The feature-balanced rule gives the proteome the largest weight (mean 0.363) because its member counts are the smallest; the proteome is the single largest contributor in 33.6% of observations.
