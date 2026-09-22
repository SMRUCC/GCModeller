require(GCModeller);

imports "kmers" from "metagenomics_kit";

let samples = "Z:\bayes_abundance.json" |> readText() |> JSON::json_decode();
let m = as.abundance_matrix(samples);
