require(GCModeller);

imports "CUDA" from "MLkit";
imports "GPS" from "biosystem";
imports "bnlearn" from "biosystem";
imports "geneExpression" from "phenotype_kit";
imports "sampleInfo" from "phenotype_kit";

CUDA::register_cuda_tensor("fp32");

let x = geneExpression::load.expr("G:\GCModeller\src\GCModeller\sub-system\demo\TestData1\gene_expression_matrix.csv");
let samples = read.sampleinfo("G:\GCModeller\src\GCModeller\sub-system\demo\TestData1\samplesinfo.csv");
let GRN = read.csv("G:\GCModeller\src\GCModeller\sub-system\demo\TestData1\regulatory_network_prior.csv", row.names = NULL, check.names = FALSE);

x <- as.time_serials(x, samples, time.label = "time");

let net = prior_network(
    TF = GRN$TF,
    target_gene = GRN$TargetGene,
    regulation_type = GRN$RegulationType,
    confidence = GRN$Confidence,
    evidence = GRN$Evidence
) |> as.prior_net()
;
let gps = x 
|> build_gps(net, config = gps_config())
|> train();

cat(attr(gps, "report"));

str(as.list(attr(gps,"metrics")));
str(as.list(attr(gps,"baseline")));
str(as.list(attr(gps,"training")));

writeBin(gps, con = here("demo_gps.zip"));

c(gps |> knockouts(c("bkdR", "fadR")),
  gps |> overexpress(c("ccpA","metK")),
  gps |> knockdown(c("pksM","spo0A"))
)
|> make_exports(dir = here("demo_intervention"), 
    pathway_info = read_pathway_knowledge("G:\GCModeller\src\GCModeller\sub-system\demo\TestData1\pathway_info.json")
)
;