# Citrate Synthesis: Oxaloacetate + Acetyl-CoA → Citrate
# Isomerization (Aconitase Reaction): Citrate → Isocitrate
# Oxidative Decarboxylation (Isocitrate Dehydrogenation): Isocitrate + NAD⁺ → α-Ketoglutarate + CO₂ + NADH
# Second Decarboxylation (α-Ketoglutarate Dehydrogenation): α-Ketoglutarate + NAD⁺ + CoA-SH → Succinyl-CoA + CO₂ + NADH
# Substrate-Level Phosphorylation (Succinyl-CoA Synthetase): Succinyl-CoA + Pi + GDP → Succinate + GTP + CoA-SH
# Succinate Dehydrogenation: Succinate + FAD → Fumarate + FADH₂
# Fumarate Hydration (Fumarase): Fumarate + H₂O → L-Malate
# Malate Dehydrogenation: L-Malate + NAD⁺ → Oxaloacetate + NADH
let TCA_cycle = [
    # metabolite       synthesis                                                                                          consumption                                                                                           
 acetyl_coA ->                                                     0    -  (oxaloacetate ^ 0.5) * (acetyl_coA ^ 0.5)     ,                                                                      
 CO2 ->              ((isocitrate ^ 0.5) * (NAD ^ 0.5)) + ((a_ketoglutarate ^ 0.3) * (NAD ^ 0.3) * (coA_sh ^ 0.3))   - 0  ,                                                                                                   
 Citrate          | (Oxaloacetate + Acetyl-CoA)                                                                         | (Citrate)                                                                                             
 CoA-SH           | (Succinyl-CoA + Pi + GDP)                                                                           | (α-Ketoglutarate + NAD⁺ + CoA-SH)                                                                     
 FAD              | NA                                                                                                  | (Succinate + FAD)                                                                                     
 FADH₂            | (Succinate + FAD)                                                                                   | NA                                                                                                    
 Fumarate         | (Succinate + FAD)                                                                                   | (Fumarate + H₂O)                                                                                      
 GDP              | NA                                                                                                  | (Succinyl-CoA + Pi + GDP)                                                                             
 GTP              | (Succinyl-CoA + Pi + GDP)                                                                           | NA                                                                                                    
 H₂O              | NA                                                                                                  | (Fumarate + H₂O)                                                                                      
 Isocitrate       | (Citrate)                                                                                           | (Isocitrate + NAD⁺)                                                                                   
 L-Malate         | (Fumarate + H₂O)                                                                                    | (L-Malate + NAD⁺)                                                                                     
 NAD⁺             | NA                                                                                                  | (Isocitrate + NAD⁺) (α-Ketoglutarate + NAD⁺ + CoA-SH) (L-Malate + NAD⁺)                               
 NADH             | (Isocitrate + NAD⁺) (α-Ketoglutarate + NAD⁺ + CoA-SH) (L-Malate + NAD⁺)                             | NA                                                                                                    
 Oxaloacetate     | (L-Malate + NAD⁺)                                                                                   | (Oxaloacetate + Acetyl-CoA)                                                                           
 Pi               | NA                                                                                                  | (Succinyl-CoA + Pi + GDP)                                                                             
 Succinate        | (Succinyl-CoA + Pi + GDP)                                                                           | (Succinate + FAD)                                                                                     
 Succinyl-CoA     | (α-Ketoglutarate + NAD⁺ + CoA-SH)                                                                   | (Succinyl-CoA + Pi + GDP)                                                                             
 α-Ketoglutarate  | (Isocitrate + NAD⁺)                                                                                 | (α-Ketoglutarate + NAD⁺ + CoA-SH)                                                                     
];

