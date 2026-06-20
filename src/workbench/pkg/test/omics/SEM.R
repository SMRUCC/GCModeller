require(GCModeller);

imports "causal_modeling" from "phenotype_kit";

let data = read.csv(here("causal_modeling.csv"), row.names = 1, check.names = FALSE);
let path = causal_modeling::make_path(from = [], 
                                        to = []);

print(data, max.print = 6);