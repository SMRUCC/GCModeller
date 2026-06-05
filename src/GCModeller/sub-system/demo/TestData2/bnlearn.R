require(GCModeller);
require(jsonlite);

imports "bnlearn" from "phenotype_kit";
imports "geneExpression" from "phenotype_kit";

let exprData = geneExpression::load.expr(here("expression_data.csv"));
let regs = read.csv(here("transcript_regulations.csv"));
let bios = jsonlite::fromJSON(here("biological_background.json"), what = "knowledges");

print(regs, max.print = 6);
print([bios$pathway_nrps]::genes);
print([bios$pathway_quorum_sensing]::genes);
print([bios$pathway_tca_cycle]::genes);

let priorNet = bnlearn::prior_network(TF = regs$TF,
    target_gene = regs$TargetGene,
    regulation_type = regs$RegulationType,
    confidence = regs$Confidence,
    evidence = regs$Evidence
);
let model = bnlearn(exprData, priorNet, max_itrs = 250);
# let results = c(
#     model |> knockouts(["sigH", "yceF"]),
#     model |> overexpress(["srfAA","thrC"]),
#     model |> knockdown(["lutR","ytrJ"])
# );

let ko_pathway_nrps = model |> knockouts([bios$pathway_nrps]::genes);
let oe_pathway_quorum_sensing = model |> overexpress([bios$pathway_quorum_sensing]::genes);
let kd_pathway_tca_cycle = model |> knockdown([bios$pathway_tca_cycle]::genes);
let results = c(ko_pathway_nrps, oe_pathway_quorum_sensing, kd_pathway_tca_cycle );

make_exports(results, dir = here("bnlearn_results"), pathway_info = bios);
bnlearn::save_model(model, dir = here("bnlearn_model"));