# Citrate Synthesis: Oxaloacetate + Acetyl-CoA → Citrate
# Isomerization (Aconitase Reaction): Citrate → Isocitrate
# Oxidative Decarboxylation (Isocitrate Dehydrogenation): Isocitrate + NAD⁺ → α-Ketoglutarate + CO₂ + NADH
# Second Decarboxylation (α-Ketoglutarate Dehydrogenation): α-Ketoglutarate + NAD⁺ + CoA-SH → Succinyl-CoA + CO₂ + NADH
# Substrate-Level Phosphorylation (Succinyl-CoA Synthetase): Succinyl-CoA + Pi + GDP → Succinate + GTP + CoA-SH
# Succinate Dehydrogenation: Succinate + FAD → Fumarate + FADH₂
# Fumarate Hydration (Fumarase): Fumarate + H₂O → L-Malate
# Malate Dehydrogenation: L-Malate + NAD⁺ → Oxaloacetate + NADH
let TCA_cycle = [
  # metabolite       synthesis                                                  consumption                
  acetyl_coA      -> 0                                                          - (oxaloacetate ^ 0.5) * (acetyl_coA ^ 0.5),      
  CO2             -> ((isocitrate ^ 0.5) * (NAD ^ 0.5)) + 
                     ((a_ketoglutarate ^ 0.3) * (NAD ^ 0.3) * (coA_sh ^ 0.3))   - 0,                          
  citrate         -> ((oxaloacetate^0.5) * (acetyl_coA^0.5))                    - (citrate),              
  coA_sh          -> ((succinyl_coA^ 0.3) * ( Pi^0.3) * ( GDP^ 0.3))            - ((a_ketoglutarate ^ 0.3) * (NAD ^ 0.3) * (coA_sh ^ 0.3)), 
  FAD             -> 0                                                          - ((succinate ^ 0.5 ) * ( FAD ^ 0.5)),                
  FADH2           -> ((succinate^0.5) * (FAD ^ 0.5))                            - 0,                 
  fumarate        -> ((succinate ^ 0.5 ) *( FAD ^ 0.5))                         - ((fumarate ^0.5) * (H2O ^ 0.5)),                  
  GDP             -> 0                                                          - ((succinyl_coA ^ 0.3) * (Pi ^ 0.3) * (GDP ^ 0.3)),         
  GTP             -> ((succinyl_coA ^ 0.5) *( Pi^0.5) *( GDP^0.5))              - 0,                    
  H2O             -> 0                                                          - ((fumarate ^0.5) * (H2O ^ 0.5)),                     
  isocitrate      -> (citrate)                                                  - ((isocitrate ^ 0.5) * (NAD ^ 0.5)),                      
  l_malate        -> ((fumarate ^0.5) * (H2O ^ 0.5))                            - ((l_malate^0.5) * (NAD ^ 0.5)),                       
  NAD             -> 0                                                          - ((isocitrate ^ 0.5) * (NAD ^ 0.5)) + 
                                                                                  ((a_ketoglutarate ^ 0.3) * (NAD ^ 0.3) * (coA_sh ^ 0.3)) + 
                                                                                  ((l_malate^0.5) * (NAD ^ 0.5)),                       
  NADH            -> ((isocitrate^0.5) *( NAD^0.5)) + 
                     ((a_ketoglutarate^0.3) *( NAD ^ 0.3) * (coA_sh ^ 0.3)) + 
                     ((l_malate^0.5) * (NAD ^ 0.5))                             - 0,                         
  oxaloacetate    -> ((l_malate^0.5) * (NAD ^ 0.5))                             - ((oxaloacetate^0.5) * (acetyl_coA^0.5)),          
  Pi              -> 0                                                          - ((succinyl_coA ^ 0.3) * (Pi ^ 0.3) * (GDP ^ 0.3)),     
  succinate       -> ((succinyl_coA ^ 0.3) * (Pi ^ 0.3) * (GDP ^ 0.3))          - ((succinate ^ 0.5 ) *( FAD ^ 0.5)),   
  succinyl_coA    -> ((a_ketoglutarate ^ 0.3) * (NAD ^ 0.3) * (coA_sh ^ 0.3))   - ((succinyl_coA ^ 0.3) * (Pi ^ 0.3) * (GDP ^ 0.3)),       
  a_ketoglutarate -> ((isocitrate ^ 0.5) * (NAD ^ 0.5))                         - ((a_ketoglutarate ^ 0.3) * (NAD ^ 0.3) * (coA_sh ^ 0.3)) 
];

