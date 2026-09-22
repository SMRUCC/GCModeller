require(GCModeller);

imports "mixOmics" from "phenotype_kit";
imports "geneExpression" from "phenotype_kit";
imports "bioModels.TRN" from "cytoscape";

let otu_16s = read.csv(here("16s\\count.csv"), row.names = 1, check.names = FALSE);
let mrm = read.csv(here("MRM.csv"), row.names = 1, check.names = FALSE);
let metab = read.csv(here("lcms_all_sample_data.csv"), row.names = 1, check.names = FALSE);

colnames(mrm) = gsub(colnames(mrm),"_","-");
rownames(mrm) = make.unique(rownames(mrm));
rownames( metab) = make.unique(rownames( metab));

otu_16s = load.expr(otu_16s);
mrm = load.expr(mrm);
metab = load.expr(metab);

let mrm_cor = mixOmics::mine(otu_16s, mrm) |> connections(cor_cutoff = 0);
# let lcms_cor = mixOmics::mine(otu_16s, metab, strict = FALSE)|> connections(cor_cutoff = 0);

print(as.data.frame(mrm_cor));

# print(as.data.frame(lcms_cor));
write.csv(as.data.frame(mrm_cor), file = here("16s+MRM_Spearman+MIC.csv"), row.names = FALSE);