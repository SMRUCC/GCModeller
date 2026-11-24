require(GCModeller);

imports "causal_modeling" from "phenotype_kit";

let data = read.csv(here("causal_modeling.csv"), row.names = 1, check.names = FALSE);
let path = causal_modeling::make_path(paths = list(
    c("CHS_expr","Quercetin"),
    c("Quercetin","DPPH"),
    c("Quercetin","Starch"),
    c("CHS_expr","DPPH"),
    c("CHS_expr","Starch"),
    c("Starch","ParticleSize")
));
let model = as_causalmodel(data, path);

print(data, max.print = 6);

let [sem_result, sem_boot] = sem(model, boot = 500);

let path_coefficient = path_coefficient(sem_result);
let effect_decomposition = effect_decomposition(sem_result);
let significance_test = significance_test(sem_result, sem_boot);
let indirect_effect = indirect_effect(sem_result, sem_boot);

print(path_coefficient);
print(effect_decomposition);
print(significance_test);
print(indirect_effect);