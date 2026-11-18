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
#     FuzzyGene::fuzzygene_relaxation         改阻尼系数 / 迭代预算
#     FuzzyGene::fuzzygene_export_trace       把两条轨迹写成 5 个文件
#     FuzzyGene::fuzzygene_trace_perturbation 在内存里拿两个矩阵 + 变化表
#
# 【运行方式】
#
#   cd G:\GCModeller\src\R-sharp\App\net10.0
#   .\R#.exe Z:\test\fuzzygene_api_test.R --attach G:\GCModeller --debug=none
#
#   前置条件：用 Rsharp_app_release|x64 配置编译过解决方案，使
#   G:\GCModeller\assembly\net10.0\biosystem.dll 是最新的。
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
# 【产物清单】导出目录 Z:\test\fuzzygene-trace\
# =====================================================================
#
#   trace.mutant.csv                 突变型轨迹矩阵
#                                    （本次：18534 行 = 1 行表头 + 18533 个基因；
#                                      62 列 = 1 列 geneID + 61 帧 mut_t000…mut_t060；
#                                      约 22 MB）
#   trace.wildtype.csv               野生型（未扰动）轨迹矩阵，同形状、同行的顺序
#   trace.mutant.sampleinfo.json     突变型矩阵的「列元数据」，每列一条 SampleInfo
#   trace.wildtype.sampleinfo.json   野生型矩阵的列元数据
#   trace.perturbation.json          本次实验的清单：扰动定义 + 求解参数 + 收敛诊断
#   mutant_from_rsharp.csv           用 geneExpression::write.expr_matrix 写的同一份
#                                    突变型矩阵，用来证明内存里的 Matrix 与磁盘上的一致
#
# =====================================================================
# 【三种文件的格式】
# =====================================================================
#
# 1) 轨迹矩阵 CSV —— 宽表（wide / matrix 布局）
#
#       geneID,mut_t000,mut_t001,...,mut_t060
#       "TSPAN6",0.19391,0.12165,...,0.16783
#       "TNMD",0.01024,0.00988,...,0.01102
#       ...
#
#    - 第 1 列 geneID：基因符号，行顺序与模型内部的基因顺序一致（18533 行）
#    - 第 2 列起每一列是一个**迭代帧**：mut_t000 是起点（已施加敲除），
#      mut_t060 是第 60 步迭代的结果
#    - 数值是**原始表达量**（不是 z-score，也不是 log2 倍数变化），
#      用 .NET 的往返精度写出（约 17 位有效数字），回读误差 < 1e-15
#    - 行标签带双引号，列名不带，可直接被 read.csv(row.names = 1) 读入
#    - wildtype 文件的列名是 wt_t000…wt_t060，行顺序与 mutant 完全一致，
#      所以两个矩阵可以逐元素相减
#
# 2) sampleinfo JSON —— 数组，每个元素描述矩阵的一列（一个 SampleInfo）
#
#       [
#         {
#           "ID": "mut_t000",              # 必须与 CSV 的列名逐字相同
#           "injectionOrder": 1,           # 帧序号（从 1 开始）
#           "batch": 1,
#           "metadata": {
#             "time": "0",                 # = iteration * dt，下游按它展开时间轴
#             "iteration": "0",            # 迭代步号（真正有物理含义的那个）
#             "role": "mutant",            # mutant / wildtype
#             "group": "mutant:Knockout:HSD17B3",
#             "omega": "0.5",              # 该次求解的阻尼系数
#             "tolerance": "1E-06",        # 收敛阈值
#             "converged": "0",            # 整条轨迹是否收敛（每帧相同）
#             "lagged": "0",               # 是否滞后（时序）模型
#             "n_genes": "18533",
#             "target_gene": "HSD17B3",
#             "perturbation_mode": "Knockout",
#             "clamped": "1",              # 该基因在这一帧是否被钳制
#             "max_delta": "46.80",        # 产生这一帧的那一步的最大变化量
#                                          # 第 0 帧为空串（起点没有变化量）
#             "perturbed_genes": "[\"HSD17B3\"]",   # 仅突变型有，GEARS 约定
#             "intervention_mode": "Knockout"       # 仅突变型有，GEARS 约定
#           },
#           "sample_name": "mut_t000",
#           "sample_info": "mutant:Knockout:HSD17B3",
#           "color": "#D62728",            # 绘图建议色
#           "shape": "21"
#         },
#         ...
#       ]
#
#    注意：
#      - 键名大小写敏感，必须是 "time"（小写）；下游找不到会静默当 0 处理
#      - metadata 是字符串字典，数值都以字符串存放
#      - 野生型那份的 target_gene / perturbation_mode 是空串、clamped 是 "0"，
#        并且**没有** perturbed_genes / intervention_mode 两个键
#
# 3) trace.perturbation.json —— 单个对象，整次实验的清单
#
#       {
#         "format": "fuzzygene steady-state trace",
#         "created_at": "2026-11-18 16:38:00",
#         "target_gene": "HSD17B3",
#         "perturbation_mode": "Knockout",
#         "clamp_value": "0",            # 施加给靶基因的表达量（敲除 = 0）
#         "dt": "1",
#         "n_genes": 18533,
#         "mutant_iterations": 60,
#         "mutant_converged": false,
#         "mutant_max_deltas": [46.80, 19.05, ...],   # 每一步的最大变化量
#         "wildtype_iterations": 60,
#         "wildtype_converged": false,
#         "wildtype_max_deltas": [46.80, 19.05, ...],
#         "omega": 0.5,
#         "tolerance": 1E-06,
#         "lagged": false,
#         "note": "frame index is a damped-iteration step, not biological time",
#         "files": [ ... ]
#       }
#
#    max_delta 是求解器在**那一步**观察到的最大单步变化量（z 空间）。
#    它的数量级直接告诉你收敛状况：tolerance = 1e-6，而这里的最小值仍有
#    19 左右 —— 差了 7 个数量级，说明这 60 帧是持续振荡，不是收敛过程。
#
#    另一个值得注意的现象：mutant_max_deltas 与 wildtype_max_deltas 的
#    **前 6 步完全相同**（46.80, 19.05, 47.81, 27.70, 19.12, 26.14），
#    从第 7 步起才出现 1e-4 量级的微小差异。含义是：敲除 HSD17B3 的影响
#    要经过几跳才扩散到「当前变化最大的那个基因」，而且幅度相对系统自身
#    的整体振荡很小。换句话说，单看 mutant 曲线会觉得「扰动后剧烈变化」，
#    其实那大半是系统本来就有的漂移 —— 这正是必须把 wildtype 一起导出的
#    原因，也说明了「扰动效应 = mutant − wildtype」这个减法不能省。
#
# =====================================================================
# 【最重要的一条解读规则】
# =====================================================================
#
#   列号 = 阻尼迭代步，**不是生物学时间**。
#
#   求解器做的是 z ← z + omega * (F(z) - z)：
#     - omega = 1   时退化成 z ← F(z)（反复套用调控函数；静态模型下
#                   第 k 帧只受「k 跳以内」的基因影响，所以帧间差异反映
#                   的是**调控图上的距离**，不是时间）
#     - omega < 1   时是带阻尼的不动点求解，曲线形状会随 omega 变，
#                   但收敛时终点不变
#     - 只有 IsLagged = true（时序模型）且 omega = 1 时，一帧才等于
#       一个采样间隔，这时 dt 才应该填真实的采样时长
#
#   所以：
#     * 可以说「这个基因比那个基因早 1 帧开始变化」= 它离扰动源更近一跳
#     * 不可以说「这个基因在 6 小时后上调 2 倍」
#     * 判读顺序永远是：先看 converged，再看两条曲线的差，最后才看单条曲线
#
# =====================================================================

imports "FuzzyGene" from "biosystem";
imports "geneExpression" from "phenotype_kit";

print("=== FuzzyGene R# api test ===");

# ---------------------------------------------------------------------
# 1. 载入模型包
# ---------------------------------------------------------------------
#
#   load_fuzzygene 读的是 *.fuzzygene.zip：
#     manifest.json     基因名、均值/标准差、稳态基线、支持域、边表
#     config.json       训练配置（含 Relaxation / PropagationSteps /
#                       Tolerance / ClampToSupport）
#     models/0000.json… 每个靶基因一个 TSK 模糊模型
#
#   本次这个包：18533 个基因、18379 个有模型、57.7 万条边、30.8 万条规则。
#   注意 config.json 里 Relaxation = 1、PropagationSteps = 250，
#   也就是默认**没有阻尼**、最多迭代 250 步。
#
let model = FuzzyGene::load_fuzzygene("G:/vcc2026/model/model.zip");

print("--- model ---");
print(model);

# ---------------------------------------------------------------------
# 2. 自动挑选靶基因
# ---------------------------------------------------------------------
#
#   fuzzygene_strongest_perturbation 在 candidates 个等距抽样的有模型基因上
#   逐个跑一次稳态，按「未被扰动基因的最大 |Z|」排序，返回第一名。
#
#   为什么候选是抽样：18533 个基因逐个跑一次约 0.3-1 秒，全量要好几个小时。
#   candidates = 6 只是为了让这个演示脚本跑得快；正式分析建议 24-50。
#
#   本次选出 HSD17B3（换成 24 个候选时结果可能不同，因为候选池变了）。
#
let gene = FuzzyGene::fuzzygene_strongest_perturbation(model, candidates = 6, mode = "knockout");

print("--- strongest perturbation target ---");
print(gene);

# ---------------------------------------------------------------------
# 3. 默认参数下的收敛诊断
# ---------------------------------------------------------------------
#
#   fuzzygene_trace_convergence 跑一次和导出完全相同的求解，
#   但**只返回诊断**，不构造那两个 18533 × 61 的大矩阵（省内存、省时间）。
#
#   返回 list 三个元素：
#     $convergence          data.frame(iteration, max_delta)  突变型
#     $control_convergence  data.frame(iteration, max_delta)  野生型
#     $summary              data.frame 单行，9 列：
#                             gene / mode / clamp_value / iterations /
#                             converged / omega / tolerance / lagged / elapsed_ms
#
#   本次结果：iterations = 250、converged = FALSE、omega = 1
#   也就是「用模型默认的无阻尼设置跑满 250 步仍未收敛」。
#
let diag = FuzzyGene::fuzzygene_trace_convergence(model, gene, mode = "knockout");

print("--- convergence summary (model default) ---");
print(diag$summary);

# ---------------------------------------------------------------------
# 4. 打开阻尼后重新求解
# ---------------------------------------------------------------------
#
#   fuzzygene_relaxation 改三个求解参数，**并返回同一个模型对象**（可链式）：
#     omega      阻尼系数，取值 (0, 1]，0.5 是稳定性与速度的常用折中
#     steps      最大迭代步数
#     tolerance  收敛阈值（单步最大变化量低于它就停）
#
#   改完必须重新跑轨迹，之前拿到的矩阵/诊断都不再对应当前参数。
#
#   注意这里用的是赋值而不是 let：model 已经在第 1 步声明过了，
#   fuzzygene_relaxation 返回的就是同一个对象，赋值只是让语义更直观。
#
model = FuzzyGene::fuzzygene_relaxation(model, omega = 0.5, steps = 300);

let diag2 = FuzzyGene::fuzzygene_trace_convergence(model, gene, mode = "knockout");

print("--- convergence summary (omega = 0.5, steps = 60) ---");
print(diag2$summary);

# ---------------------------------------------------------------------
# 5. 导出 5 个结果文件  ← 本脚本最核心的一步
# ---------------------------------------------------------------------
#
#   fuzzygene_export_trace(model, gene, dir, mode, steps, dt) 会写：
#
#     trace.mutant.csv               突变型矩阵（行=基因，列=mut_t000…mut_t060）
#     trace.mutant.sampleinfo.json   突变型列元数据
#     trace.wildtype.csv             野生型矩阵（行=基因，列=wt_t000…wt_t060）
#     trace.wildtype.sampleinfo.json 野生型列元数据
#     trace.perturbation.json        整次实验清单（含两条 max_delta 序列）
#
#   参数说明：
#     dir  输出目录，不存在会自动创建；同名文件会被覆盖
#     dt   只影响 sidecar 里的 metadata$time（time = iteration * dt），
#          不改变矩阵本身。静态模型保持 1；时序模型填采样间隔
#
#   返回：5 个文件的绝对路径（字符向量），顺序与上面一致。
#
let files = FuzzyGene::fuzzygene_export_trace(model, gene, "Z:/test/fuzzygene-trace", mode = "knockout", dt = 1);

print("--- exported files ---");
print(files);

# ---------------------------------------------------------------------
# 6. 在内存里拿两个矩阵 + 终态变化表
# ---------------------------------------------------------------------
#
#   返回 list 六个元素：
#     $mutant                Matrix（18533 基因 × 61 帧），列名 mut_t000…
#     $wildtype              Matrix（同形状），列名 wt_t000…
#     $convergence           data.frame(iteration, max_delta)
#     $control_convergence   data.frame(iteration, max_delta)
#     $changes               data.frame(18533 行 × 5 列)：
#                              gene / fold_change / percent_change /
#                              z_score / significant
#                            fold_change = 突变型终态 − 对照基线（原始表达量之差）
#                            z_score     = fold_change / 训练期标准差
#                            significant = |z_score| > 2 或该基因被钳制
#     $summary               单行 data.frame（同第 3 步）
#
#   $changes 是「终态结论」，最适合直接排序看 top 变化基因；
#   $mutant / $wildtype 是「过程」，用来画曲线或热图。
#
let trace = FuzzyGene::fuzzygene_trace_perturbation(model, gene, mode = "knockout");

print("--- trace summary ---");
print(trace$summary);

print("--- changes table shape ---");
print(dim(trace$changes));

# ---------------------------------------------------------------------
# 7. 验证内存里的 Matrix 能直接交给下游落盘
# ---------------------------------------------------------------------
#
#   geneExpression::write.expr_matrix 接受 .NET 的 HTS.DataFrame.Matrix，
#   按 id 参数作为首列表头写出 CSV。
#
#   这一步的产物 mutant_from_rsharp.csv 与第 5 步的 trace.mutant.csv
#   内容**逐字节相同**（MD5 一致），说明 list slot 里的 Matrix 就是
#   底层那个对象，没有被截断或转置。
#
#   注意：不要对 trace$mutant 调用 dim()/nrow()/ncol()，它是 CLR 对象，
#   这些 R 函数拿不到形状，会返回 1。要形状就用 dim(trace$changes)
#   或直接读回 CSV。
#
print("--- write mutant matrix through geneExpression::write.expr_matrix ---");

let ok = geneExpression::write.expr_matrix(trace$mutant, file = "Z:/test/fuzzygene-trace/mutant_from_rsharp.csv", id = "geneID");

print(ok);

print("=== done ===");

# =====================================================================
# 【怎么把结果用起来】—— 参考，不在本脚本里执行
# =====================================================================
#
#   R 里读入并比较突变型与野生型（apply 到真实数据前先改路径）：
#
#     mut <- read.csv("Z:/test/fuzzygene-trace/trace.mutant.csv", row.names = 1, check.names = FALSE)
#     wt  <- read.csv("Z:/test/fuzzygene-trace/trace.wildtype.csv", row.names = 1, check.names = FALSE)
#     info <- jsonlite::fromJSON("Z:/test/fuzzygene-trace/trace.mutant.sampleinfo.json")
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
#   画曲线之前务必先确认 converged：如果是 FALSE，曲线尾部的平台是
#   「跑满步数被截断」，不是稳态，不能当结论用。
#
# =====================================================================
