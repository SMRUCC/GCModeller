require(GCModeller);

imports "CUDA" from "MLkit";
imports "GPS" from "biosystem";
imports "bnlearn" from "biosystem";
imports "geneExpression" from "phenotype_kit";
imports "sampleInfo" from "phenotype_kit";

let pathways = read_pathway_knowledge("G:\GCModeller\src\GCModeller\sub-system\demo\TestData1\pathway_info.json");
let gps = readBin(here("demo_gps.zip"), what = "gps");

c(gps |> knockouts(c("bkdR", "fadR")),
  gps |> overexpress(c("ccpA","metK")),
  gps |> knockdown(c("pksM","spo0A"))
)
|> make_exports(dir = here("demo_intervention"), 
    pathway_info = pathways
)
;