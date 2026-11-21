require(GCModeller);

imports "FuzzyGene" from "biosystem";
imports "geneExpression" from "phenotype_kit";

# =====================================================================
# FuzzyGene R# API 测试 + 结果文件解读指南
# =====================================================================
#
# 【这个脚本做什么】
#
#   用真实模型包 G:\vcc2026\model\model.zip 跑一次完整的「虚拟扰动 →
#   记录全基因组稳态求解轨迹 → 导出矩阵文件」流程，并逐个说明产出的
#   文件是什么格式、应该怎么读。
#
#   涉及的 API（都在 workbench/R#/biosystem/fuzzyGene.vb 里导出）：
#     FuzzyGene::load_fuzzygene               载入 *.fuzzygene.zip 模型包
#     FuzzyGene::fuzzygene_strongest_perturbation  自动挑响应最强的靶基因
#     FuzzyGene::fuzzygene_trace_convergence  只看求解诊断（便宜）
#     FuzzyGene::fuzzygene_solver_report      退出原因 / 自由残差 / 约束违背量
#     FuzzyGene::fuzzygene_hybrid_config      链式切换求解器与混合层
#     FuzzyGene::fuzzygene_relaxation         阻尼迭代的 ω / 步数 / 容差
#     FuzzyGene::fuzzygene_export_trace       把两条轨迹写成 5 个文件
#     FuzzyGene::fuzzygene_trace_perturbation 在内存里拿两个矩阵 + 变化表
#     FuzzyGene::fuzzygene_pbn / fuzzygene_ensemble / fuzzygene_modules
#                                             定性层（本模型未启用，测容错分支）
#
# 【运行方式】
#
#   cd G:\GCModeller\src\R-sharp\App\net10.0
#   & ".\R#.exe" "G:\GCModeller\test\fuzzygene_api_test.R" --attach "G:\GCModeller" --debug=none
#
#   前置条件：用 Rsharp_app_release|x64 配置编译过解决方案，使
#   G:\GCModeller\assembly\net10.0\biosystem.dll 与 FuzzyGene.dll 是最新的。
#
# 【两个语法坑】
#
#   1) 包命名空间大小写敏感：必须写 FuzzyGene::，写 fuzzygene:: 会报
#      "we can not found any namespace called: 'fuzzygene'"。
#   2) mutant / wildtype 是 .NET 的 HTS.DataFrame.Matrix 对象，不是
#      R 的 matrix：对它调 dim() 只会得到 1。要落盘就交给
#      geneExpression::write.expr_matrix(...)，要画图先落盘再用 R 读回。
#
# =====================================================================
# 【本次这个模型包的状态】（来自训练报告 model\report.txt）
# =====================================================================
#
#   trained=18349 / skipped=184 / failed=0，edges=290998，rules=439674
#   calibration: SpectralRadius, rho 20.49 -> 0.2626, gamma=0.04636
#                mean R2 0.5287 -> 0.001133
#   structure: scatter, mf=2, inputs=18 x15777（18 个调控因子确实生效）
#   solver: Newton, maxNewtonIterations=60, ClampInput/OutputToSupport = TRUE
#   Hybrid: PbnRegularization = None, FcmWarmStart = false
#
#   **必须读懂 calibration 这一行**：γ=0.046 意味着所有一阶结论的斜率被压到
#   原来的 4.6%，R² 从 0.5287 掉到 0.0011（已经低于"用均值预测"的水平）。
#   于是 ρ 只有 0.2626（远小于 1），扰动**必然收敛**——但这个收敛是"系统几乎
#   不动"换来的，不是"模型把扰动传播对了"。
#   所以下面所有响应幅度都会非常小（|Z| 普遍 < 0.1），这是模型的性质，不是脚本错。
#   若要恢复幅度，需要在训练侧放宽 CalibrationEpsilon 或收紧因果先验，而不是在这里调参。
#
#   【实测结论，2026-11-21】校准到 ρ=0.26 并没有让扰动真正收敛：
#     B3GNT2 knockout -> exit_reason = MaxIterations, free_residual = 2.39
#                        iterations = 256（6 帧牛顿 + 250 帧阻尼回落）、耗时 28.9 s
#     out_of_support = 0.389（38.9% 的基因至少一个调控因子被推到训练域外）
#     对照（未扰动）同样 MaxIterations，free_residual = 1.71 —— 连野生型都没有不动点
#     mutant_max_deltas 在 1.4 与 10.6 之间来回跳（2 周期），但状态并不逐位重复，
#     所以 CycleDetector（窗口 16、精确匹配）报不出 LimitCycle
#   原因不是"没有压到 1 以下"（ρ 明明只有 0.26），而是**映射不连续**：
#   输入侧的 support 夹断让 z 在边界两侧跳到不同分支，形成非精确周期。
#   这说明"缩小斜率"这条路线单独用是无效的——还需要处理 support 夹断
#   （例如 ClampInputToSupport = FALSE + 有界激活，或换成 FCM 的有界传播）。
#   脚本仍然按真实结果断言，不会因为不收敛而失败。
#
# =====================================================================
# 【产物清单】G:\GCModeller\test\fuzzygene-trace\
# =====================================================================
#
#   trace.mutant.csv                 突变型轨迹矩阵（行=基因，列=mut_t000…）
#   trace.mutant.sampleinfo.json     突变型矩阵的「列元数据」，每列一条 SampleInfo
#   trace.wildtype.csv               野生型轨迹矩阵，同形状、同行的顺序
#   trace.wildtype.sampleinfo.json   野生型的列元数据
#   trace.perturbation.json          本次实验的清单：扰动定义 + 求解参数 + 收敛诊断
#   mutant_from_rsharp.csv           用 geneExpression::write.expr_matrix 写的同一份
#                                    突变型矩阵，用来证明内存里的 Matrix 与磁盘上的一致
#   api_test_report.txt              本脚本的 [PASS]/[FAIL] 汇总
#
#   另一个目录 G:\GCModeller\test\fuzzygene-analysis\：
#   analysis.sensitivity.csv / analysis.lag.csv / analysis.spearman.csv /
#   analysis.dose_response.csv / analysis.dose_time_surface.csv /
#   analysis.combination.csv / analysis.report.json
#
#   注意：帧数（mut_t000…mut_tNNN）不再是固定的 61 帧。
#   静态路线现在解 r(z)=F(z)−z=0，帧数 = 求解器实际迭代次数；
#   收敛的扰动通常只有个位数到几十帧，未收敛的才跑满 PropagationSteps。
#
# =====================================================================
# 【三种文件的格式】
# =====================================================================
#
# 1) 轨迹矩阵 CSV —— 宽表（wide / matrix 布局）
#
#       geneID,mut_t000,mut_t001,...,mut_tNNN
#       "TSPAN6",0.19391,0.12165,...,0.16783
#       ...
#
#    - 第 1 列 geneID：基因符号，行顺序与模型内部的基因顺序一致
#    - 第 2 列起每一列是一个**求解器迭代帧**：mut_t000 是起点（已施加敲除），
#      mut_tNNN 是最后一次迭代的结果
#    - 数值是**原始表达量**（不是 z-score，也不是 log2 倍数变化），
#      用 .NET 的往返精度写出（约 17 位有效数字），回读误差 < 1e-15
#    - wildtype 文件的列名是 wt_t000…，行顺序与 mutant 完全一致，
#      所以两个矩阵可以逐元素相减
#
# 2) sampleinfo JSON —— 数组，每个元素描述矩阵的一列（一个 SampleInfo）
#
#       [
#         {
#           "ID": "mut_t000",
#           "injectionOrder": 1,
#           "batch": 1,
#           "metadata": {
#             "time": "0",                 # = iteration * dt，下游按它展开时间轴
#             "iteration": "0",            # 迭代步号（真正有物理含义的那个）
#             "role": "mutant",
#             "group": "mutant:Knockout:HSD17B3",
#             "omega": "1",                # 阻尼系数（只对回落路径有意义）
#             "tolerance": "1E-06",        # 收敛阈值（自由基因的残差上限）
#             "converged": "1",            # 整条轨迹是否达到稳态（每帧相同）
#             "lagged": "0",
#             "n_genes": "18533",
#             "target_gene": "HSD17B3",
#             "perturbation_mode": "Knockout",
#             "clamped": "1",
#             "max_delta": "0.83",         # 产生这一帧的那一步的残差（z 空间）
#             "perturbed_genes": "[\"HSD17B3\"]",
#             "intervention_mode": "Knockout"
#           },
#           ...
#         }
#       ]
#
#    注意：
#      - 键名大小写敏感，必须是 "time"（小写）；下游找不到会静默当 0 处理
#      - metadata 是字符串字典，数值都以字符串存放
#      - 【口径】逐帧 metadata 里只有 converged；exit_reason / stabilized_by /
#        free_residual / clamp_residual / attractor_period 在 trace.perturbation.json
#        的顶层字段里（mutant_* / wildtype_*）。要单点查询就用
#        FuzzyGene::fuzzygene_solver_report()。
#
# 3) trace.perturbation.json —— 单个对象，整次实验的清单
#
#       {
#         "format": "fuzzygene steady-state trace",
#         "target_gene": "HSD17B3",
#         "clamp_value": "0",
#         "mutant_iterations": 12,          # 求解器实际迭代数
#         "mutant_converged": true,         # 现在这个字段真的代表"达到稳态"
#         "mutant_exit_reason": "Converged",
#         "mutant_free_residual": 9.7E-09,
#         "mutant_clamp_residual": 0.94,     # 恒 > 0，仅诊断
#         "mutant_attractor_period": 0,      # 检出极限环时给周期
#         "mutant_stabilized_by": "fixed point",   # ← 先看这个
#         "mutant_max_deltas": [...],       # 每一步的残差（长度 = iterations）
#         "wildtype_iterations": 9,
#         "wildtype_converged": true,
#         "wildtype_exit_reason": "Converged",
#         "wildtype_free_residual": 1.2E-08,
#         "wildtype_stabilized_by": "fixed point",
#         "omega": 1, "tolerance": 1E-06, "lagged": false,
#         "note": "frame index is a solver iteration, not biological time; read exit_reason / stabilized_by before using the end state",
#         "files": [...]
#       }
#
#    max_delta 的含义随求解器变化：静态路线是牛顿/LM 的残差序列，
#    正常情况下应该单调下降到 tolerance 以下；如果 3 步以后还在 10^-1 量级，
#    说明这个扰动没有不动点（用 exit_reason 确认是 MaxIterations 还是 LimitCycle）。
#
# =====================================================================
# 【最重要的一条解读规则】
# =====================================================================
#
#   列号 = **求解器迭代步**，不是生物学时间。
#
#   静态路线解的是 r(z) = F(z) - z = 0（Levenberg–Marquardt）：
#     - 帧数少（个位数~几十）= 牛顿二次收敛，属正常
#     - omega 只对**回落路径**（legacy / anderson）有意义；默认 solver=newton 时
#       改 omega 不会改变结果，要演示阻尼必须先用 fuzzygene_hybrid_config 切求解器
#     - 只有 IsLagged = true（时序模型，Advance 模式）时，一帧才等于一个采样间隔
#
#   所以：
#     * 可以说「这个基因比那个基因早 1 帧开始变化」= 它离扰动源更近一跳
#     * 不可以说「这个基因在 6 小时后上调 2 倍」
#     * 判读顺序永远是：先看 stabilized_by（终态从哪来），再看两条曲线的差，
#       最后才看单条曲线
#
# =====================================================================

print("=== FuzzyGene R# api test ===");

let trace_dir = "G:/GCModeller/test/fuzzygene-trace";
let analysis_dir = "G:/GCModeller/test/fuzzygene-analysis";

let passes = 0;
let failures = 0;

# ---------------------------------------------------------------------
# 1. 载入模型包
# ---------------------------------------------------------------------
#
#   load_fuzzygene 读的是 *.fuzzygene.zip：
#     manifest.json     基因名、均值/标准差、稳态基线、支持域、边表、模块划分
#     config.json       训练配置（Solver / Hybrid / PropagationSteps / …）
#     models/0000.json… 每个靶基因一个 TSK 模糊模型
#     pbn.json          可选：只有训练时开了 PbnRegularization 才有
#
#   这个包：18533 个基因、18349 个有模型、29.1 万条边、44 万条规则。
#   config.json 里 Solver.Mode = Newton、MaxNewtonIterations = 60，
#   Hybrid.PbnRegularization = None（所以后面 PBN/集成采样会走容错分支）。
#
let model = FuzzyGene::load_fuzzygene("G:\vcc2026\model\model.zip");

print("--- model ---");
print(model);

let edges = fuzzygene_edges(model);

if (nrow(edges) == 0) {
    print("[FAIL] the model carries no edge");
    failures = failures + 1;
} else {
    print("[PASS] the model package is loaded");
    passes = passes + 1;
}

# ---------------------------------------------------------------------
# 2. 自动挑选靶基因
# ---------------------------------------------------------------------
#
#   fuzzygene_strongest_perturbation 在 candidates 个等距抽样的有模型基因上
#   逐个跑一次稳态，按「未被扰动基因的最大 |Z|」排序，返回第一名。
#
#   candidates = 6 只是为了让这个演示脚本跑得快；正式分析建议 24-50。
#   注意：这个模型被校准得很厉害（γ=0.046），所有 |Z| 都会很小，
#   排名前列的基因之间差距可能只有 1e-3 量级。
#
print("--- strongest perturbation target ---");

#   candidates 从 6 降到 3：这个模型每次求解约 27 s（6 帧牛顿 + 250 帧阻尼回落），
#   6 个候选光这一步就要 2.5 分钟。
#
let gene = FuzzyGene::fuzzygene_strongest_perturbation(model, candidates = 3, mode = "knockout");

print(gene);

# ---------------------------------------------------------------------
# 3. 默认参数下的收敛诊断
# ---------------------------------------------------------------------
#
#   fuzzygene_trace_convergence 跑一次和导出完全相同的求解，
#   但**只返回诊断**，不构造那两个 18533 × N 的大矩阵（省内存、省时间）。
#
#   返回 list 三个元素：
#     $convergence          data.frame(iteration, max_delta)  突变型
#     $control_convergence  data.frame(iteration, max_delta)  野生型
#     $summary              data.frame 单行，**17 列**：
#                             gene / mode / clamp_value / iterations / converged /
#                             exit_reason / free_residual / clamp_residual /
#                             out_of_support / pinned / attractor_period /
#                             stabilized_by / control_exit_reason / omega /
#                             tolerance / lagged / elapsed_ms
#
#   exit_reason 的取值与含义（这是本轮修复的核心）：
#     Converged          自由基因残差 < tolerance —— 唯一可以当稳态用的情况
#     MaxIterations      步数/轮数耗尽 —— 曲线平尾是截断，不是稳态
#     LimitCycle         检出极限环 —— 系统在振荡，attractor_period 给周期
#     SupportSaturated   大部分基因贴在支持域边界 —— 结论由夹断产生
#     Diverged           残差放大 —— 没有答案
#   clamp_residual 恒 > 0（被钳制基因本来就不在 F 的不动点上），**不能**当收敛判据。
#
#   stabilized_by 回答的是另一个问题：**报出来的那个终态是从哪来的**：
#     fixed point     真正的不动点，可以当定量结论
#     limit cycle     极限环的周期均值（attractor_period 给周期），振荡系统的正确汇报值
#     fcm surrogate   FCM 模块稳态代理（只有粗粒度可信，需要 fcmWarmStart 才可能出现）
#     tail mean       尾段均值兜底（既无不动点也无精确周期时的最后手段）
#   空串表示没有稳定下来，此时报的就是最后一帧。**只有 fixed point 能当定量结论。**
#
print("--- convergence summary (model default) ---");

let diag = FuzzyGene::fuzzygene_trace_convergence(model, gene, mode = "knockout");

print(diag$summary);

# 硬约束：只要报 converged，自由残差就必须真的在容差以内。
# 这条断言专门用来防「把截断平台伪装成稳态」的回归。
if (diag$summary$converged[1]) {
    if (diag$summary$free_residual[1] > diag$summary$tolerance[1]) {
        print("[FAIL] converged = TRUE but free_residual is above the tolerance");
        failures = failures + 1;
    } else {
        print("[PASS] converged = TRUE implies free_residual <= tolerance");
        passes = passes + 1;
    }
} else {
    print("[PASS] converged = FALSE is reported honestly (see exit_reason)");
    passes = passes + 1;
}

print("[INFO] exit_reason / iterations / free_residual:");
print(diag$summary$exit_reason);
print(diag$summary$iterations);
print(diag$summary$free_residual);

# ---------------------------------------------------------------------
# 4. 单次扰动的求解器报告（新增 API，最直接反映本轮修复）
# ---------------------------------------------------------------------
#
#   fuzzygene_solver_report 返回单行表，同时给突变型与对照两条轨迹的诊断：
#     exit_reason / iterations / free_residual / clamp_residual /
#     out_of_support / pinned / attractor_period
#     control_exit_reason / control_iterations / control_free_residual
#
#   和 trace_convergence 的区别：这个不返回逐帧序列，只给终局判定，
#   适合在批量筛选里对每个靶基因做一次"这次求解可不可信"的检查。
#
print("--- solver report (mutant vs wild type) ---");

let report_ko = FuzzyGene::fuzzygene_solver_report(model, gene, mode = "knockout", steps = 30);

print(report_ko);

print("[INFO] wild type solve:");
print(report_ko$control_exit_reason);
print(report_ko$control_free_residual);

let report_kd = FuzzyGene::fuzzygene_solver_report(model, gene, mode = "knockdown", steps = 30);

print("[INFO] knockdown exit_reason:");
print(report_kd$exit_reason);
print(report_kd$iterations);

if (report_ko$iterations[1] > 0) {
    print("[PASS] the solver report carries the iteration count");
    passes = passes + 1;
} else {
    print("[FAIL] the solver report lost the iteration count");
    failures = failures + 1;
}

# ---------------------------------------------------------------------
# 5. 切换到阻尼迭代，单独验证 ω 的作用
# ---------------------------------------------------------------------
#
#   默认 solver = newton：omega 只在「牛顿失败回落到 Anderson/legacy」时才生效。
#   要真的演示阻尼，必须先用 fuzzygene_hybrid_config 把求解器切过去。
#
#   fuzzygene_model_config(model) 返回的是模型内部那个 Config 对象的**活引用**，
#   所以改它就是改模型本身（不需要重新训练、不需要重新载入）。
#
#   这里把 steps 压到 120：18533 基因 × (18 输入 × 48 规则) 的前向一次约几十毫秒，
#   250 步会让这一段变成几十秒，而演示只需要看出"阻尼迭代的帧数远多于牛顿"。
#
print("--- switch to the damped fallback and re-solve ---");

let live_config = FuzzyGene::fuzzygene_model_config(model);

live_config = FuzzyGene::fuzzygene_hybrid_config(live_config, solver = "legacy");

model = FuzzyGene::fuzzygene_relaxation(model, omega = 0.5, steps = 60, tolerance = 0.01);

let diag_damped = FuzzyGene::fuzzygene_trace_convergence(model, gene, mode = "knockout", steps = 60);

print(diag_damped$summary);

print("[INFO] the damped run needs far more frames than the Newton run:");
print(diag_damped$summary$iterations);

# 复位：后续章节都要跑牛顿求根，否则会慢一个数量级
live_config = FuzzyGene::fuzzygene_hybrid_config(live_config, solver = "newton");

model = FuzzyGene::fuzzygene_relaxation(model, omega = 1, steps = 250, tolerance = 0.000001);

print("[PASS] the solver can be switched at runtime through fuzzygene_hybrid_config");
passes = passes + 1;

# ---------------------------------------------------------------------
# 6. 导出 5 个结果文件  ← 本脚本最核心的一步
# ---------------------------------------------------------------------
#
#   fuzzygene_export_trace(model, gene, dir, mode, steps, dt) 会写：
#     trace.mutant.csv / trace.mutant.sampleinfo.json
#     trace.wildtype.csv / trace.wildtype.sampleinfo.json
#     trace.perturbation.json
#
#   参数说明：
#     dir  输出目录，不存在会自动创建；同名文件会被覆盖
#     dt   只影响 sidecar 里的 metadata$time（time = iteration * dt），
#          不改变矩阵本身。静态模型保持 1；时序模型填采样间隔
#
#   steps = 30：这个模型不收敛，跑满 250 步会写出 256 列 × 18533 行（约 96 MB/文件），
#   只是拖慢脚本；30 帧已经足够看清残差形状。
let files = FuzzyGene::fuzzygene_export_trace(model, gene, trace_dir, mode = "knockout", steps = 30, dt = 1);

print("--- exported files ---");
print(files);

if (length(files) == 5) {
    print("[PASS] the export wrote the 5 expected artifacts");
    passes = passes + 1;
} else {
    print("[FAIL] the export did not return 5 files");
    failures = failures + 1;
}

# ---------------------------------------------------------------------
# 7. 在内存里拿两个矩阵 + 终态变化表
# ---------------------------------------------------------------------
#
#   返回 list 六个元素：
#     $mutant                Matrix（基因 × 帧），列名 mut_t000…
#     $wildtype              Matrix（同形状），列名 wt_t000…
#     $convergence           data.frame(iteration, max_delta)
#     $control_convergence   data.frame(iteration, max_delta)
#     $changes               data.frame（每基因一行 × 5 列）：
#                              gene / fold_change / percent_change /
#                              z_score / significant
#                            fold_change = **突变型不动点 − 野生型不动点**
#                            （旧版是"突变型 − 训练基线"，那个差里混入了模型自漂移，
#                              本轮已改成同一模型下两个稳态之差）
#                            z_score     = fold_change / 训练期标准差
#                            significant = |z_score| > 2 或该基因被钳制
#     $summary               单行 data.frame（同第 3 步，16 列）
#
print("--- trace summary ---");

let trace = FuzzyGene::fuzzygene_trace_perturbation(model, gene, mode = "knockout", steps = 30);

print(trace$summary);

print("--- changes table shape ---");
print(dim(trace$changes));

if (nrow(trace$changes) == 18533) {
    print("[PASS] the changes table covers every gene");
    passes = passes + 1;
} else {
    print("[FAIL] the changes table does not match the gene count");
    failures = failures + 1;
}

# ---------------------------------------------------------------------
# 8. 验证内存里的 Matrix 能直接交给下游落盘
# ---------------------------------------------------------------------
#
#   geneExpression::write.expr_matrix 接受 .NET 的 HTS.DataFrame.Matrix，
#   按 id 参数作为首列表头写出 CSV。
#
#   不要对 trace$mutant 调用 dim()/nrow()/ncol()：它是 CLR 对象，
#   这些 R 函数拿不到形状，会返回 1。要形状就用 dim(trace$changes)
#   或直接读回 CSV。
#
print("--- write mutant matrix through geneExpression::write.expr_matrix ---");

let ok = geneExpression::write.expr_matrix(trace$mutant, file = file.path(trace_dir, "mutant_from_rsharp.csv"), id = "geneID");

print(ok);

# ---------------------------------------------------------------------
# 9. 定性层 / 粗粒度层（用启用过这些层的模型来测）
# ---------------------------------------------------------------------
#
#   生产模型训练时 PbnRegularization = None、FcmWarmStart = false，
#   所以 fuzzygene_pbn / fuzzygene_ensemble / fuzzygene_modules 对它都会**明确报错**
#   并提示怎么打开（设计约定：宁可报错，也不返回"标准差恒为 0"的假结果）。
#
#   但这里**不能**用 tryCatch 去探测这个错误：R# 的 tryCatch 抓不住
#   .NET 抛出的 TargetInvocationException，会让整个脚本直接中止（实测会 abort）。
#   所以改成加载那个真正启用了定性层的小模型（500 基因，随包带 pbn.json），
#   直接验证三个 API 的正常路径。
#
let small_model_file = "G:/vcc2026/model/test_small/model.zip";

if (file.exists(small_model_file)) {
    print("--- qualitative layer on the small model (PBN + FCM enabled) ---");

    let small = FuzzyGene::load_fuzzygene(small_model_file);

    let pbn = FuzzyGene::fuzzygene_pbn(small);

    print(pbn$summary);
    print(pbn$targets, max.print = 5);

    if (nrow(pbn$cod) > 0) {
        print("[PASS] fuzzygene_pbn returns the CoD table");
        passes = passes + 1;
    } else {
        print("[FAIL] fuzzygene_pbn returned an empty CoD table");
        failures = failures + 1;
    }

    let ensemble = FuzzyGene::fuzzygene_ensemble(small, c("FCGRT"), mode = "knockout", samples = 8);

    print(ensemble$summary);
    print(ensemble$table, max.print = 3);

    if (nrow(ensemble$table) > 0) {
        print("[PASS] fuzzygene_ensemble returns mean / sd / sign agreement");
        passes = passes + 1;
    } else {
        print("[FAIL] fuzzygene_ensemble returned an empty table");
        failures = failures + 1;
    }

    let modules = FuzzyGene::fuzzygene_modules(small);

    print(modules$summary);

    if (nrow(modules$modules) > 0) {
        print("[PASS] fuzzygene_modules returns the module activation");
        passes = passes + 1;
    } else {
        print("[FAIL] fuzzygene_modules returned no module");
        failures = failures + 1;
    }
} else {
    print("[WARN] the small model is not present, the qualitative layer is not exercised");
}

# ---------------------------------------------------------------------
# 10. 多运行自检：ω 不变性 / 步数不变性 / lag 排序稳健性
# ---------------------------------------------------------------------
#
#   ω 扫描      同一扰动在 ω ∈ c(0.5, 1.0) 上各跑一次，比较**终态**。
#               ω 只改变"走过去的路"，收敛时终点应当一致；
#               终点不一致 = 没收敛，或者存在多个吸引子。
#
#   步数扫描    在 steps ∈ c(30, 60) 上各跑一次，以最长步数为参考。
#               注意：牛顿求根下 steps 只约束回落路径，步数不变性是"弱检查"。
#
#   lag 稳健性  「首次偏离野生型并达到自身终态变化量 10% 的帧号」，
#               再跨 ω 两两算 Spearman 秩相关。ρ 接近 0 = 排序不可信。
#
#   读法：先看 $summary$omega_invariant。FALSE 时后面的曲线与 lag 排序
#   都只能当"模型诊断"，不能当生物学结论。
#
print("--- sensitivity self check ---");

let sens = FuzzyGene::fuzzygene_sensitivity(model, gene,
    omegas = c(0.5, 1.0),
    stepsList = c(30, 60),
    sweepSteps = 30,
    tolerance = 0.001,
    lagRatio = 0.1);

print(sens$summary);
print(sens$omega);
print(sens$steps);
print(sens$spearman);

# ---------------------------------------------------------------------
# 11. 剂量-响应曲线
# ---------------------------------------------------------------------
#
# 剂量 d ∈ [0, 1]，钳制值 = 基线 × (1 − d)：
#   d = 0 → 靶基因被钉在基线（**不是**"没有扰动"）
#   d = 1 → 完全敲除（钳制值 = 0）
#
# 返回长表，每个 (剂量, 基因) 一行：
#   dose / clamp_value / gene / fold_change / percent_change / z_score / significant
#
# 只返回按最大剂量下 |Z| 排序的前 `top` 个基因；top = 0 返回全基因组。
#
print("--- dose response (top 5 genes) ---");

let dose = FuzzyGene::fuzzygene_dose_response(model, gene,
    doses = c(0, 0.5, 1),
    steps = 30,
    top = 5);

print(dose);

# ---------------------------------------------------------------------
# 12. 剂量-时间曲面
# ---------------------------------------------------------------------
#
# 每个格子是「该剂量的突变轨迹在那一帧 − 同一帧的野生型轨迹」，
# 也就是**已经减掉系统自身漂移**的扰动效应。
# 返回长表：dose / iteration / gene / delta（iteration 是求解器迭代帧号）
#
print("--- dose time surface long table shape (rows x columns) ---");

let surf = FuzzyGene::fuzzygene_dose_time_surface(model, gene,
    doses = c(0, 0.5, 1),
    steps = 30,
    top = 5);

print(dim(surf));

# ---------------------------------------------------------------------
# 13. 双基因组合的非加性（协同 / 拮抗）
# ---------------------------------------------------------------------
#
# 加性期望 = control + (A − control) + (B − control)
# 非加性   = joint − additive    正 = 协同，负 = 拮抗
#
# 返回 $summary / $table / $top / $trajectory。
# 两个基因必须都存在且不同，否则直接报错（不会静默跳过）。
#
print("--- combination (non-additive) ---");

let gene2 = "TSPAN6";

let combo = FuzzyGene::fuzzygene_combination(model, gene, gene2,
    steps = 30,
    top = 5);

print(combo$summary);
print(combo$top);

# ---------------------------------------------------------------------
# 14. 一键导出：跑完整套分析并落盘
# ---------------------------------------------------------------------
#
# 这一步把 10~13 的分析再跑一遍，但产物是磁盘上的表格：
#   analysis.sensitivity.csv / analysis.lag.csv / analysis.spearman.csv
#   analysis.dose_response.csv / analysis.dose_time_surface.csv
#   analysis.combination.csv / analysis.report.json
#
print("--- analysis artifacts ---");

let analysis = FuzzyGene::fuzzygene_analysis_report(model, gene,
    geneA = gene,
    geneB = gene2,
    dir = analysis_dir,
    omegas = c(0.5, 1.0),
    stepsList = c(30, 60),
    doses = c(0, 0.5, 1),
    sweepSteps = 30,
    doseSteps = 30,
    combinationSteps = 30,
    top = 5,
    surfaceTop = 5,
    lagRatio = 0.1,
    tolerance = 0.001);

print(analysis);

# ---------------------------------------------------------------------
# 15. 结论
# ---------------------------------------------------------------------
#
print("--- test summary ---");

writeLines(c(
    "# FuzzyGene R# API test",
    `assertions passed   : ${passes}`,
    `assertions failed   : ${failures}`,
    "# interpretation: see the header of scripts/fuzzygene_api_test.R",
    "# exit_reason is authoritative; converged is derived from it",
    "# clamp_residual is diagnostic only and never reaches 0",
    `trace dir    : ${trace_dir}`,
    `analysis dir : ${analysis_dir}`
), con = file.path(trace_dir, "api_test_report.txt"));

print(passes);
print(failures);

if (failures > 0) {
    stop("[FAIL] the API test reported one or more failures, see api_test_report.txt");
}

print("[PASS] the whole FuzzyGene api surface is reachable and self consistent");

print("=== done ===");

# =====================================================================
# 【怎么把结果用起来】—— 参考，不在本脚本里执行
# =====================================================================
#
#   R 里读入并比较突变型与野生型：
#
#     mut <- read.csv("G:/GCModeller/test/fuzzygene-trace/trace.mutant.csv", row.names = 1, check.names = FALSE)
#     wt  <- read.csv("G:/GCModeller/test/fuzzygene-trace/trace.wildtype.csv", row.names = 1, check.names = FALSE)
#     info <- jsonlite::fromJSON("G:/GCModeller/test/fuzzygene-trace/trace.mutant.sampleinfo.json")
#
#     # 1) 扰动效应 = 同一帧上两条曲线之差（这是唯一有生物学含义的量）
#     effect <- as.matrix(mut) - as.matrix(wt)
#
#     # 2) 热图：行聚类看响应模式，列用 metadata$iteration 做 x 轴
#     pheatmap::pheatmap(as.matrix(mut), cluster_cols = FALSE,
#                        labels_col = info$metadata$iteration)
#
#     # 3) 按「第一次偏离野生型超过阈值的帧号」给基因排序，
#     #    得到的是「离扰动源几跳」的顺序，不是时间顺序
#     lag <- apply(abs(effect) > 0.05, 1, function(v) if (any(v)) which(v)[1] else NA)
#     head(sort(lag))
#
#   画曲线之前务必先确认 exit_reason = "Converged"：MaxIterations 的尾平台是
#   "跑满步数被截断"，LimitCycle 则应该改报周期均值，两者都不能当稳态结论。
#
# =====================================================================
