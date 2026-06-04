require(GCModeller);
require(jsonlite);

imports "bnlearn" from "phenotype_kit";
imports "geneExpression" from "phenotype_kit";

let exprData = geneExpression::load.expr(here("expression_data.csv"));
let regs = read.csv(here("transcript_regulations.csv"));
let bios = jsonlite::fromJSON(here("biological_background.json"), what = "knowledges");

print(regs);

let priorNet = bnlearn::prior_network(TF = regs$TF,
    target_gene = regs$TargetGene,
    regulation_type = regs$RegulationType,
    confidence = regs$Confidence,
    evidence = regs$Evidence
);
let model = bnlearn(exprData, priorNet, max_itrs = 100);
let results = c(
    model |> knockouts([""]),
    model |> overexpress([""]),
    model |> knockdown([""])
);

make_exports(results, dir = here("bnlearn_results"), pathway_info = bios);