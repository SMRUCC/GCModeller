require(GCModeller);

imports "S.system" from "simulators";

# Citrate Synthesis: Oxaloacetate + Acetyl-CoA → Citrate
# Isomerization (Aconitase Reaction): Citrate → Isocitrate
# Oxidative Decarboxylation (Isocitrate Dehydrogenation): Isocitrate + NAD⁺ → α-Ketoglutarate + CO₂ + NADH
# Second Decarboxylation (α-Ketoglutarate Dehydrogenation): α-Ketoglutarate + NAD⁺ + CoA-SH → Succinyl-CoA + CO₂ + NADH
# Substrate-Level Phosphorylation (Succinyl-CoA Synthetase): Succinyl-CoA + Pi + GDP → Succinate + GTP + CoA-SH
# Succinate Dehydrogenation: Succinate + FAD → Fumarate + FADH₂
# Fumarate Hydration (Fumarase): Fumarate + H₂O → L-Malate
# Malate Dehydrogenation: L-Malate + NAD⁺ → Oxaloacetate + NADH
let TCA_cycle = [
  # metabolite       synthesis               consumption                                           
  citrate         -> (oxaloacetate^0.5)      - (citrate ^ 0.5),                             
  fumarate        -> (succinate ^ 0.5)       - (fumarate ^0.5),                  
  isocitrate      -> (citrate ^ 0.5)         - (isocitrate ^ 0.5),               
  l_malate        -> (fumarate ^0.5)         - (l_malate^0.5),                      
  oxaloacetate    -> (l_malate^0.5)          - (oxaloacetate^0.5),          
  succinate       -> (succinyl_coA ^ 0.5)    - (succinate ^ 0.5 ) ,   
  succinyl_coA    -> (a_ketoglutarate ^ 0.5) - (succinyl_coA ^ 0.5),  
  a_ketoglutarate -> (isocitrate ^ 0.5)      - (a_ketoglutarate ^ 0.5)
];

let metabolites = list(
    citrate = 100, fumarate = 20, isocitrate = 20, 
    l_malate = 20, oxaloacetate = 20, 
    succinate = 20, succinyl_coA = 20, 
    a_ketoglutarate = 20
);

# open data connection for export to a csv table file
using data.driver as snapshot(relative_work("TCA-cycle.csv"), symbols = names(metabolites)) {
    data.driver
    |> kernel(S.script("TCA cycle"),strict=FALSE)
    |> environment(metabolites)
    |> s.system(TCA_cycle)
    |> run(ticks = 150, resolution = 0.005);
}

