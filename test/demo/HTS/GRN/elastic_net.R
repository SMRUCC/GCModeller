require(GCModeller);

imports "bnlearn" from "biosystem";
imports "geneExpression" from "phenotype_kit";
imports "linear" from "TRNtoolkit";

let opts = new("elastic_net.options");
let mat = load.expr("G:\GCModeller\src\GCModeller\sub-system\demo\TestData1\gene_expression_matrix.csv");
let grn = elastic_net(mat);
let model = elastic_model(grn$grn, opts);

c(model |> knockouts(c("bkdR", "fadR")),
  model |> overexpress(c("ccpA","metK")),
  model |> knockdown(c("pksM","spo0A"))
)
|> make_exports(dir = here("elastic_net_intervention"), 
    pathway_info = read_pathway_knowledge("G:\GCModeller\src\GCModeller\sub-system\demo\TestData1\pathway_info.json")
)
;