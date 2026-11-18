# =====================================================================
# FuzzyGene R# api smoke test
# ---------------------------------------------------------------------
# 验证 workbench/R#/biosystem/fuzzyGene.vb 新增的轨迹相关 API：
#   fuzzygene_strongest_perturbation / fuzzygene_trace_convergence /
#   fuzzygene_relaxation / fuzzygene_export_trace / fuzzygene_trace_perturbation
#
# 运行方式：
#   R#.exe Z:\test\fuzzygene_api_test.R --attach G:\GCModeller --debug=none
#
# 注意：包命名空间是大小写敏感的 FuzzyGene（不是 fuzzygene）。
# 返回的 mutant / wildtype 是 .NET 的 HTS.DataFrame.Matrix 对象，
# 直接交给 geneExpression::write.expr_matrix 落盘，不要对它调用 dim()。
# =====================================================================

imports "FuzzyGene" from "biosystem";
imports "geneExpression" from "phenotype_kit";

print("=== FuzzyGene R# api test ===");

# ---------------------------------------------------------------------
# 1. 载入真实模型包
# ---------------------------------------------------------------------
let model = FuzzyGene::load_fuzzygene("G:/vcc2026/model/model.zip");

print("--- model ---");
print(model);

# ---------------------------------------------------------------------
# 2. 候选排序：只在 6 个候选上评估（控制耗时）
# ---------------------------------------------------------------------
let gene = FuzzyGene::fuzzygene_strongest_perturbation(model, candidates = 6, mode = "knockout");

print("--- strongest perturbation target ---");
print(gene);

# ---------------------------------------------------------------------
# 3. 默认求解参数下的收敛诊断（omega = 1，来自模型的 config.json）
# ---------------------------------------------------------------------
let diag = FuzzyGene::fuzzygene_trace_convergence(model, gene, mode = "knockout");

print("--- convergence summary (model default) ---");
print(diag$summary);

# ---------------------------------------------------------------------
# 4. 打开阻尼后重跑：omega = 0.5，最多 60 步
# ---------------------------------------------------------------------
model = FuzzyGene::fuzzygene_relaxation(model, omega = 0.5, steps = 60);

let diag2 = FuzzyGene::fuzzygene_trace_convergence(model, gene, mode = "knockout");

print("--- convergence summary (omega = 0.5, steps = 60) ---");
print(diag2$summary);

# ---------------------------------------------------------------------
# 5. 导出矩阵 CSV + sampleinfo sidecar + 扰动清单
# ---------------------------------------------------------------------
let files = FuzzyGene::fuzzygene_export_trace(model, gene, "Z:/test/fuzzygene-trace", mode = "knockout", dt = 1);

print("--- exported files ---");
print(files);

# ---------------------------------------------------------------------
# 6. 内存中的两个矩阵 + 终态变化表
# ---------------------------------------------------------------------
let trace = FuzzyGene::fuzzygene_trace_perturbation(model, gene, mode = "knockout");

print("--- trace summary ---");
print(trace$summary);

print("--- changes table shape ---");
print(dim(trace$changes));

# ---------------------------------------------------------------------
# 7. list slot 里的 Matrix 能否直接交给下游落盘
# ---------------------------------------------------------------------
print("--- write mutant matrix through geneExpression::write.expr_matrix ---");

let ok = geneExpression::write.expr_matrix(trace$mutant, file = "Z:/test/fuzzygene-trace/mutant_from_rsharp.csv", id = "geneID");

print(ok);

print("=== done ===");
