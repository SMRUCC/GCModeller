# ============================================================
# VirtualCellular R# demo（原子化 api + 复用 intervention 包）
#
# 流程：
#   1. load_expr        —— 数据加载
#   2. model.build      —— 纯计算（不落盘）
#   3. model.save       —— 纯 IO
#   4. model.open       —— 纯 IO（复用，不再解析 CSV）
#   5. as_perturbation  —— 纯计算（包装成 InsilicoPerturbationExperiment）
#   6. intervention::overexpress / make_exports —— 复用既有扰动与批量导出
#   7. overexpress_genes + export.de / export.summary —— 联合过表达与单次导出
#
# 运行：
#   cd E:\codebuddy\GCModeller\src\R-sharp\App\net10.0
#   & ".\R#.exe" "E:\codebuddy\GCModeller\test\virtualcellular_demo.R" --attach "E:\codebuddy\GCModeller" --debug=none --silent
#
# 前置：
#   dotnet build "E:\codebuddy\GCModeller\src\workbench\R#\biosystem\biosystem.vbproj" -c Rsharp_app_release -p:Platform=x64
# ============================================================

imports ["VirtualCellular", "intervention"] from "biosystem";

let mini = "E:/codebuddy/GCModeller/src/GCModeller/sub-system/CellularAI/VirtualCellular.Demo/bin/Debug/net10.0/mini";
let work = "E:/codebuddy/GCModeller/test";

print("[STEP] 1/7 load expression matrix");
let exp = VirtualCellular::load_expr(
    paste(mini, "expression.csv", sep = "/"),
    paste(mini, "sampleinfo.csv", sep = "/"));

print("[STEP] 2/7 build model (pure compute)");
let model = VirtualCellular::model.build(exp, seed = 20261126);
let genes = VirtualCellular::model.genes(model);

print(length(genes));

if (length(genes) < 100) {
    stop("[FAIL] the gene axis is too short");
}

print("[STEP] 3/7 save model package (pure io)");
VirtualCellular::model.save(model, paste(work, "virtualcellular-model.zip", sep = "/"));

print("[STEP] 4/7 reopen model package (pure io)");
let cached = VirtualCellular::model.open(paste(work, "virtualcellular-model.zip", sep = "/"));
let perts = VirtualCellular::model.perturbations(cached);

print(perts);

if (length(perts) < 1) {
    stop("[FAIL] the model has no pseudo perturbation");
}

print("[STEP] 5/7 wrap as perturbation experiment");
let pert = VirtualCellular::as_perturbation(cached, cells = 40, seed = 20261126);

print("[STEP] 6/7 reuse intervention::overexpress + make_exports");
let results = intervention::overexpress(pert, c("BATF", "BATF3"));

print(length(results));
intervention::make_exports(results, paste(work, "virtualcellular-batch", sep = "/"));

print("[STEP] 7/7 joint overexpression + atomic export");
let joint = VirtualCellular::overexpress_genes(pert, c("BATF", "BATF3"));
let n = length(VirtualCellular::perturbation.significant(joint));

print(n);

VirtualCellular::export.de(joint, paste(work, "virtualcellular-joint", "de.csv", sep = "/"));
VirtualCellular::export.summary(joint, paste(work, "virtualcellular-joint", "summary.csv", sep = "/"));

writeLines(
    c(
        paste("[PASS] genes =", length(genes)),
        paste("[PASS] perturbations =", length(perts)),
        paste("[PASS] intervention::overexpress returned =", length(results)),
        paste("[PASS] joint significant =", n)
    ),
    con = paste(work, "virtualcellular_demo_report.txt", sep = "/"));

print("[PASS] virtual cellular demo finished");
