require(GCModeller);

imports "taxonomy_kit" from "metagenomics_kit";

let taxlist = c("[Bifidobacterium [indicum] DSM 20214 = LMG 11587] k__Bacteria;p__Bacillati;c__Actinomycetota;o__Actinomycetes;f__Bifidobacteriales;g__Bifidobacteriaceae;s__Bifidobacterium.",
"[Xanthomonas euvesicatoria pv. allii CFBP 6369] k__Bacteria;p__Pseudomonadati;c__Pseudomonadota;o__Gammaproteobacteria;f__Lysobacterales;g__Lysobacteraceae;s__Xanthomonas.");

print(as.data.frame(biom_string.parse(taxlist)));