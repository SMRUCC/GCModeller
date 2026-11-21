require(GCModeller);

imports "FuzzyGene" from "biosystem";

# 临时验证：吸引子语义（stabilized_by / attractor_period / 周期-尾段均值）

let model = FuzzyGene::load_fuzzygene("G:\vcc2026\model\model.zip");
let gene = "B3GNT2";

print("--- solver report (production model, FCM off) ---");

let r = FuzzyGene::fuzzygene_solver_report(model, gene, mode = "knockout", steps = 120);

print(r);

print("[INFO] stabilized_by:");
print(r$stabilized_by);
print("[INFO] exit_reason:");
print(r$exit_reason);
print("[INFO] attractor_period:");
print(r$attractor_period);

print("--- trace summary ---");

let t = FuzzyGene::fuzzygene_trace_convergence(model, gene, mode = "knockout", steps = 30);

print(t$summary);

print("[PASS] the attractor semantics is reachable");
