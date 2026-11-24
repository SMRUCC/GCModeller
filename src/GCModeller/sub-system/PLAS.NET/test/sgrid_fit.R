require(GCModeller);

imports "ssGrid" from "biosystem";
imports "geneExpression" from "phenotype_kit";

let t = read.csv(here("cell_pseudotime_mapping.csv"), row.names = NULL);

print(t,max.print = 6);
print(t$pseudotime);

let x = load.expr(here("pseudotime_ordered_expression_matrix_subset.csv"));
let opts    = sfit_options(sparsity = 0.01, max_edges = 8, max_regulators = 24, smooth = 9, l1 = 0, thread_tasks = 72);
let prior   = sgrid_prior(here("TF_target_prior_curated.csv"));
let network = sfit(x, opts, prior = prior, time = t$pseudotime);

writeBin(network , con = here("submatrix-sgrid.zip"));

print(sgrid_r2(network) |> mean);
write.csv(sgrid_coefficients(network), file = here("tmp/coef.csv"));
sgrid_save(network, here("tmp/network"));

stop();

let ctx  = sgrid_context(network, "/data/pseudobulk_context_A.csv");
let resp = perturb(network, c("ABCD1","ACLY"), type = "knockdown", magnitude = 0.8,
                   context = ctx, export = "/tmp/ko");
write.csv(as.data.frame(resp), file = "/tmp/ko/response.csv");
