# ============================================================
# VirtualCellular R# demo
# 目的：用 R# 脚本复现 VirtualCellular.Demo 的流程
#       （装配模型 → 保存 → 打开 → BATF / BATF3 / 联合过表达 → 导出）
#
# 运行：
#   cd E:\codebuddy\GCModeller\src\R-sharp\App\net10.0
#   & ".\R#.exe" "E:\codebuddy\GCModeller\test\virtualcellular_demo.R" --attach "E:\codebuddy\GCModeller" --debug=none
#
# 前置：
#   dotnet build "E:\codebuddy\GCModeller\src\workbench\R#\biosystem\biosystem.vbproj" -c Rsharp_app_release -p:Platform=x64
# ============================================================

imports "VirtualCellular" from "biosystem";

print("[STEP] 1/5 build model from the expression matrix");
let model = VirtualCellular::model.build(
    "E:/codebuddy/GCModeller/src/GCModeller/sub-system/CellularAI/VirtualCellular.Demo/bin/Debug/net10.0/mini/expression.csv",
    "E:/codebuddy/GCModeller/src/GCModeller/sub-system/CellularAI/VirtualCellular.Demo/bin/Debug/net10.0/mini/sampleinfo.csv",
    "E:/codebuddy/GCModeller/test/virtualcellular-model.zip",
    seed = 20261126);

print("[CHECK] gene axis");
let genes = VirtualCellular::model.genes(model);
print(length(genes));

if (length(genes) < 100) {
    stop("[FAIL] the gene axis is too short");
}

print("[CHECK] pseudo perturbation axis");
let perts = VirtualCellular::model.perturbations(model);
print(perts);

if (length(perts) < 1) {
    stop("[FAIL] the model has no pseudo perturbation");
}

print("[STEP] 2/5 reopen the model package (no csv parsing)");
let cached = VirtualCellular::model.open("E:/codebuddy/GCModeller/test/virtualcellular-model.zip");
print(length(VirtualCellular::model.genes(cached)));

print("[STEP] 3/5 single gene overexpression: BATF");
let n1 = VirtualCellular::overexpress(
    cached, c("BATF"),
    "E:/codebuddy/GCModeller/test/virtualcellular-out",
    cells = 40, seed = 20261126);

print(n1);

if (n1 <= 0) {
    stop("[FAIL] BATF overexpression produced no significant gene");
}

print("[STEP] 4/5 single gene overexpression: BATF3");
let n2 = VirtualCellular::overexpress(
    cached, c("BATF3"),
    "E:/codebuddy/GCModeller/test/virtualcellular-out",
    cells = 40, seed = 20261126);

print(n2);

print("[STEP] 5/5 joint overexpression: BATF + BATF3");
let n3 = VirtualCellular::overexpress(
    cached, c("BATF", "BATF3"),
    "E:/codebuddy/GCModeller/test/virtualcellular-out",
    cells = 40, seed = 20261126);

print(n3);

writeLines(
    c(
        paste("[PASS] genes =", length(genes)),
        paste("[PASS] perturbations =", length(perts)),
        paste("[PASS] BATF significant =", n1),
        paste("[PASS] BATF3 significant =", n2),
        paste("[PASS] BATF+BATF3 significant =", n3)
    ),
    con = "E:/codebuddy/GCModeller/test/virtualcellular_demo_report.txt");

print("[PASS] virtual cellular demo finished");
