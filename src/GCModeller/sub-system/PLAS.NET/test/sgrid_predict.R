require(GCModeller);

imports "ssGrid" from "biosystem";
imports "geneExpression" from "phenotype_kit";

let network = readBin(here("hsa-sgrid.zip"), what = "ssgrid_network");
let targets = read.csv(here("pert_counts.csv"), row.names = NULL)$target_gene ;

print("inspect of the targets:");
print(targets);

for(let id in c("A","B","C")) {
    let ctx  = sgrid_context(network, here( `pseudobulk_context_${id}.csv`));
    let resp = perturb(network, targets, type = "knockdown", magnitude = 0.8,
                    context = ctx, export = here(`predict/${id}`));
    write.csv(as.data.frame(resp), file = here(`predict/response_${id}.csv`));
}

