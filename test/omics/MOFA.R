require(GCModeller);

# demo code for Multi-Omics Factor Analysis-a framework for unsupervised integration of multi-omics data sets

imports "MOFA" from "phenotype_kit";
imports "geneExpression" from "phenotype_kit";

let rna_seqs = load.expr(here("rnaseqs_days.csv"));
let mrm_metab = load.expr(here("data\\meanTable.csv"));

let model = create_mofa(rna_seqs, mrm_metab)
|> run_mofa()
;
let result = model |> MOFA::reconstruct();

print(elbo_trace(model));

write.expr_matrix(result$rna_seqs, file = here("MOFA/rna_seqs.csv"));
write.expr_matrix(result$mrm_metab, file = here("MOFA/mrm_metab.csv"));