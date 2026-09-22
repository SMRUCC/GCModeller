require(GCModeller);

imports "gene_quantification" from "rnaseq";
imports "geneExpression" from "phenotype_kit";

let counts = read_featureCounts("F:\datapool\20260312\20260424\gene_counts.txt");
let expr = convert_to_tpm(counts);

let [tpm, fpkm] = expression_data(expr);

write.expr_matrix(tpm, file = "F:\datapool\20260312\20260424\gene_tpm.csv");
write.expr_matrix(fpkm, file = "F:\datapool\20260312\20260424\gene_fpkm.csv");

counts = counts_matrix(counts);

write.expr_matrix(deseq2_norm(counts), file = "F:\datapool\20260312\20260424\gene_deseq2.csv");
write.expr_matrix(edgeR_norm(counts), file = "F:\datapool\20260312\20260424\gene_edger.csv");