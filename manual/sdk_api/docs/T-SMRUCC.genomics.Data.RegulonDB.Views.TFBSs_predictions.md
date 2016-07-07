---
title: TFBSs_predictions
---

# TFBSs_predictions
_namespace: [SMRUCC.genomics.Data.RegulonDB.Views](N-SMRUCC.genomics.Data.RegulonDB.Views.html)_

TFBSs_predictions_v3.0.txt
 
 # Title: Computationally predicted transcription factor binding sites (TFBSs) using the evaluated weight matrix v3.0
 #
 #
 # Description Of the dataset:
 #
 # Computationally predicted transcription factor binding sites (TFBSs) In
 # upstream regions Of the Escherichia coli K-12 genome, based On version 8.8
 # Of RegulonDB.
 # We scanned all upstream regions Of every Single gene, from +50 To -400 Or
 # from +50 To the closest upstream ORF, whatever happens first.
 #
 #
 # Citation:
 #
 # Dataset provided And maintained by RegulonDB (PUBMED: #21051347) from the
 # original source published In: Medina-Rivera et al. Theoretical And
 # empirical quality assessment Of transcription factor-binding motifs. Nucleic Acids
 # Research (2011) vol. 39 (3) pp. 808-824 (PubMed: 20923783)
 #
 # Columns:
 # 1. - map name (eg: gene name),
 # 2. - feature type (site, ORF),
 # 3. - identifier(ex: GATA_box, Abf1_site)
 # 4. - strand (D For Direct, R For Reverse),
 # 5. - start position (may be negative)
 # 6. - End position (may be negative)
 # 7. - (Optional) sequence (ex: AAGATAAGCG)
 # 8. - (Optional) The weight Of a sequence segment
 # 9.- Pval. The significance Of the weight associated To Each site
 # 10.- ln_Pval
 # 11.- Significance. sig = -log(P-value)
 #
 # seq_idft_typeft_namestrandstartEndsequenceweightPvalln_PvalsigWminWmaxrankrank_pm




