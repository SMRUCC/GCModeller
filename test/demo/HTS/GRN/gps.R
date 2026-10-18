require(GCModeller);

imports "CUDA" from "MLkit";
imports "GPS" from "biosystem";
imports "bnlearn" from "biosystem";
imports "geneExpression" from "phenotype_kit";
imports "sampleInfo" from "phenotype_kit";

CUDA::register_cuda_tensor("fp64");
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
|> build_gps(net, config = gps_config(
    maxGenes = 0,
    cpmNormalize = TRUE,
    logTransform = TRUE,
    zScore = TRUE,
    peDim = 8,
    spdMaxDist = 4,
    coexpressionTopK = 0,
    minCoexpression = 0.7,
    dModel = 32,
    nLayers = 3,
    nHeads = 4,
    ffnDim = 64,
    dropout = 0.05,
    geneEmbedDim = 32,
    useSpdBias = TRUE,
    useGlobalAttention = TRUE,
    useLocalMpnn = TRUE,
    window = 4,
    gruHidden = 128,
    useUncertainty = FALSE,
    pretrainEpochs = 30,
    finetuneEpochs = 100,
    learningRate = 0.01,
    autofocusGamma = 1,
    lambdaDir = 0.5,
    lambdaL1 = 0.0001,
    lambdaPrior = 0,
    gradientClip = 1,
    earlyStopPatience = 4,
    maskRatio = 0.15,
    printEvery = 5,
    trainRatio = 0.6,
    validationRatio = 0.2,
    seed = 2024,
    significanceZScore = 0.5,
    cascadeSteps = 5    
))
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