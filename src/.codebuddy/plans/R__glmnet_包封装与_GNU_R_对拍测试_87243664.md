---
name: R# glmnet 包封装与 GNU R 对拍测试
overview: 在 Rlapack 的 glmnet.vb 空壳模块中，基于底层 GLMNet 程序集封装出与 GNU R glmnet 对齐的 R# 函数接口（glmnet/cv.glmnet/predict/coef/deviance 及结果字段访问、print/summary/as.data.frame），随后以 Rsharp_app_release|x64 编译解决方案，并在 test/glmnet 中用 GNU R 黄金数据脚本 + R# demo 脚本 + GNU R compare.R 完成六家族与 CV 的 CSV 对拍报告。
todos:
  - id: verify-rsharp-api
    content: 使用 [subagent:code-explorer] 复核 R# 导出签名、矩阵转换与泛型注册约定
    status: completed
  - id: implement-core-api
    content: 在 glmnet.vb 实现初始化、入参转换及 glmnet/cv.glmnet 接口
    status: completed
    dependencies:
      - verify-rsharp-api
  - id: implement-methods
    content: 实现 predict/coef/deviance 与 print/summary/as.data.frame 及字段访问
    status: completed
    dependencies:
      - implement-core-api
  - id: write-golden-r
    content: 编写 GNU R 黄金数据脚本，导出六家族输入、拟合与 CV 基准 CSV
    status: completed
  - id: write-demo-scripts
    content: 编写 R# demo_fit/demo_cv/demo_predict 脚本输出同构结果 CSV
    status: completed
    dependencies:
      - implement-core-api
      - write-golden-r
  - id: write-compare-report
    content: 编写 GNU R compare.R 按分级容差断言并生成 CSV/TXT 报告
    status: completed
    dependencies:
      - write-golden-r
      - write-demo-scripts
  - id: build-and-verify
    content: 编译 Rsharp_app_release|x64 并运行全链路对拍，修正偏差与文档
    status: completed
    dependencies:
      - implement-methods
      - write-demo-scripts
      - write-compare-report
---

## 用户要求

在已完成的底层 `GLMNet` 程序集之上，于 R# 包模块 `glmnet.vb` 中封装出与 GNU R `glmnet` 对齐的脚本函数接口；编写 GNU R 黄金数据脚本、R# demo 脚本与对比脚本并统一放入 `test/glmnet`；随后以 `Rsharp_app_release|x64` 配置通过 dotnet 编译 `R_system.NET5.slnx`，用更新后的 `App/net10.0/R#.exe` 运行 R# 测试脚本完成对拍。

已确认的范围与约束：

- 接口范围为核心完整：`glmnet`、`cv.glmnet`、`predict`、`coef`、`deviance`，并支持结果字段访问与 `print`/`summary`/`as.data.frame`；不实现 `glmnet.control`、`assess.glmnet`、`plot`。
- 对拍覆盖 gaussian / binomial / poisson / multinomial / cox / mgaussian 六个家族及交叉验证，数据源为 GNU R glmnet 内置示例数据集。
- 结果以「GNU R compare 脚本 + CSV/文本报告」落地：R# 脚本输出 CSV，GNU R 脚本按容差断言并生成对比报告。
- 运行环境固定：`C:\Program Files\R\R-4.5.0\bin\Rscript.exe`（R 4.5.0 + glmnet 5.1）与 `G:\GCModeller\src\R-sharp\App\net10.0\R#.exe`。

## 产品概述

为 R# 语言补齐一套与 GNU R `glmnet` 同语义的弹性网/套索建模 API：R# 脚本可直接读入矩阵或数据框拟合整条 lambda 正则化路径、执行 K 折交叉验证、在新数据上预测、取系数与偏差，并以与 R 一致的字段名访问结果。同时提供一套可离线复跑的对拍工程，用同一份输入数据分别由 GNU R 与 R# 计算并逐项比对，输出可读的数值偏差报告，用于验证封装与底层算法的一致性。

## 核心特性

- `glmnet(x, y, family, ...)`：六家族完整 lambda 路径拟合，支持 alpha、nlambda、lambda.min.ratio、自定义 lambda、weights、offset、standardize、intercept、penalty.factor、上下界、exclude、dfmax/pmax、thresh、maxit、type.gaussian / type.multinomial / type.logistic、standardize.response、relax 等 R 风格参数。
- `cv.glmnet(...)`：K 折交叉验证，支持 nfolds、显式 foldid、type.measure、grouped、keep、seed，输出 cvm/cvsd/cvup/cvlo、lambda.min、lambda.1se 与 foldid。
- `predict` / `coef` / `deviance`：支持 type（link/response/coefficients/nonzero/class）、数值或 `lambda.min`/`lambda.1se` 的 s、exact 重拟合、newoffset。
- 结果对象：以 R 风格字段名（`a0`、`beta`、`lambda`、`df`、`dev.ratio`、`nulldev`、`nzero`、`jerr`、`nobs`、`nvars`、`class`；CV 另有 `cvm`、`cvsd`、`lambda.min`、`lambda.1se`）暴露，并支持 `print`/`summary`/`as.data.frame`。
- 对拍工程：GNU R 导出六家族黄金 CSV（含 x/y/foldid/lambda/df/dev.ratio/系数/CV 曲线），R# demo 脚本读同一 CSV 并输出同构结果 CSV，GNU R compare 脚本按家族分级容差断言并生成文本与 CSV 报告。
- 构建与验证：一条 `Rsharp_app_release|x64` 编译命令产出到 `App/net10.0`，随后用 `R#.exe` 逐脚本运行并给出可核查的运行日志与报告。

## 技术栈选择

沿用现有工程栈，不引入新框架或 NuGet 依赖：

- 语言/运行时：VB.NET（.NET SDK 风格项目，`net10.0`），编译配置 `Rsharp_app_release|x64`。
- 底层算法：`G:\GCModeller\src\runtime\sciBASIC#\Data_science\MachineLearning\GLMNet\GLMNet.vbproj`（根命名空间 `Microsoft.VisualBasic.MachineLearning.GLMNet`），已由 `Rlapack.NET5.vbproj:145` 引用，无需改动项目文件。
- R# 互操作：`<Package>` / `<ExportAPI>` / `<RApiReturn>` / `<RTypeExport>`、`RRawVectorArgument`、`SMRUCC.Rsharp.Runtime.Environment`、`CLRVector.asNumeric`、`list`/`dataframe`、`RInternal.generic` / `makeDataframe` / `ConsolePrinter`。
- 数据交换：CSV（`utils::write.table` 与 R# `read.csv`/`write.csv`）。
- 对拍工具：GNU R 4.5.0 + glmnet 5.1（`Rscript.exe`）；构建工具 `dotnet build`。

## 实现策略

采用「薄适配层 + 稳定结果契约」：在 `glmnet.vb` 内只做三件事——R# 对象到 `Double()()` / `GlmnetOptions` 的转换、调用底层 `GlmnetSolver` / `GlmnetPredict`、把 CLR 结果装配为 R 风格 `list`；算法与数值口径完全交给已通过 R 对拍的底层库，避免在上层重实现或二次标定。

关键决策与权衡：

1. **返回 R# `list` 而非裸 CLR 对象**：用户要求 `fit$lambda`、`fit$beta`、`cv$lambda.min` 这类字段访问；裸 CLR 对象在 R# 中主要走 `[obj]::member`， `语义不稳定。`list `可精确复刻 GNU R 对象字段名，也便于 `write.csv`。同时在 `list` 中保留一个内部槽（如 `.__fit__` / `.__cv__`）持有原生 `GlmnetFit`/`GlmnetCVFit`，供 `predict`/`coef`/`deviance` 无损取回，避免把三维 `beta` 反复序列化。
2. **`predict`/`coef`/`deviance` 用 `<ExportAPI>` 直接导出并按首参 `Object` 分派**：仓库无 `generic.add("predict"/"coef")` 惯例（现例 `math.vb:980`、`Linear.vb:849` 均为直接导出），遵循既有模式；同时用 `<RTypeExport>` + `generic.add("summary")` + `makeDataframe.addHandler` + `AttachConsoleFormatter` 注册到原生类型，使 `print`/`summary`/`as.data.frame` 在持有原生对象时同样可用，并提供接受 `list` 的显式 `print.glmnet`/`summary.glmnet`/`as.data.frame.glmnet` 辅助导出。
3. **初始化用 `<RInitialize>` 静态 `Main`（或模块 `Sub New()`）**：R# 在导入包时反射调用 `ImportsPackage.runMain`（`ImportsPackage.vb:296-312`），确定性优于改动 `zzz.vb`；这样可把改动面收敛到单个文件，降低对其他 Rlapack 包初始化的影响。
4. **命名冲突规避**：底层命名空间末段为 `GLMNet`，而本模块名为 `glmnet`，VB 大小写不敏感（底层 README 与 `GlmnetSolver.vb` 备注已明示）。实现使用 `Imports` 别名（如 `Imports glmnetLib = Microsoft.VisualBasic.MachineLearning.GLMNet`）或全限定名引用 `GlmnetFit`/`GlmnetOptions` 等类型，并在首次编译即验证解析无歧义。
5. **参数命名对齐 R**：VB 参数用下划线（`lambda_min_ratio`、`type_measure`、`penalty_factor`、`lambda_min`），R# 运行时自动把 `_` 映射为 `.`（`RMethodInfo.vb:436-441`），因此脚本里 `lambda.min.ratio = ...` 与 `lambda_min_ratio = ...` 均可匹配；不虚构 `<RName>` 属性。
6. **索引口径换算**：R 的 `foldid` 与 `exclude` 为 1-based，底层 `GlmnetOptions.FoldId` / `Exclude` 为 0-based，统一在适配层减 1；`type_measure`/`type`/`family` 字符串分别映射到 `CVMeasure`/`PredictionType`/`FamilyRegistry.Parse` 可接受的名称，非法值经 `RInternal.debug.stop` 返回可读错误而非抛出裸异常。

复杂度与瓶颈：拟合本身是 O(n·p·nlambda·iter) 的坐标下降，由底层承担；适配层额外开销仅为一次性 O(n·p) 的矩阵转换与 O(nlambda·p·k) 的结果装配，属线性、可忽略。需在 `predict`/`coef` 热路径避免重复把 `list` 反序列化成矩阵，直接复用内部槽的原生对象；不要把整条 `beta` 或输入矩阵写入日志。

## 实现要点（防止回归）

- **复用既有转换范式**：照 `Rlapack\stats.vb:1440-1461` 的 `getMatrix`（`Rdataframe.forEachRow` + `REnv.TryCastGenericArray`、`INumericMatrix.ArrayPack`）与 `Rlapack\RMatrix.vb:625-675` 的 `matrix_extractor`（dataframe 逐列 `CLRVector.asNumeric`、GeneralMatrix、list、`Double()()`、vector）写一个模块内私有的 `toMatrix(x, env)`；`y` 用 `CLRVector.asNumeric` 后按家族整理成 `n x k` 的 `Double()()`（cox 两列 time/status，multinomial 单列标签，mgaussian 多列），与 `Internals\GlmnetData.vb:8-11` 契约一致。
- **统一参数构造**：写一个 `buildOptions(...)` 把全部 R 风格可选参数映射到单个 `GlmnetOptions`，`glmnet` 与 `cv.glmnet` 共用；自定义 `lambda` 需显式校验严格递减正数（底层 `Validate` 已覆盖，但先做 R# 侧类型错误提示）。
- **CV 对拍去噪**：R 侧导出 `*_foldid.csv`，R# 侧必须读入并换算为 0-based 后传入 `FoldId`，否则随机分折会带来不可比噪声；同时把 R 的实际 lambda 网格作为 `options.Lambda` 传入以逐步对齐（底层 README 的 fdev 早停规则会使默认 100 步被截断）。
- **结果装配字段名固定**，供 demo 与 compare 脚本共同依赖：

| 槽名 | 内容 | 形状 |
| --- | --- | --- |
| `a0` | 截距 | 每步 × 响应 |
| `beta` | 系数 | 每步 × 特征 × 响应（多维时展开为 `feature.response` 列） |
| `lambda`, `df`, `dev.ratio`, `nzero`, `deviance`, `converged`, `iterations` | 路径指标 | 长度 = 步数 |
| `nulldev`, `nobs`, `nvars`, `nresponses`, `jerr`, `family`, `class` | 标量元信息 | — |
| `cvm`, `cvsd`, `cvup`, `cvlo`, `foldid`, `lambda.min`, `lambda.1se`, `index.min`, `index.1se`, `name` | 仅 CV | — |
| `.__fit__` / `.__cv__` | 原生 CLR 对象（内部槽，不写入 CSV） | — |


- **错误与日志**：类型/维度/空值一律 `Return RInternal.debug.stop(msg, env)` 或 `Message.InCompatibleType(...)`；`Return Nothing` 表示 R NULL；警告用 `env.AddMessage`。日志只打印步数、family、lambda 首尾与 `jerr`，不倾倒矩阵；不为每步打日志。
- **影响范围控制**：只改 `glmnet.vb`（必要时 `zzz.vb` 一行），不修改底层 `GLMNet` 项目、不修改 `Rlapack.NET5.vbproj`（引用已存在）、不重构 `stats.vb`/`math.vb`；新增文件仅在 `test\glmnet` 下。保留 `glmnet` 包名与现有 `<Package("glmnet")>` 声明不变以维持命名空间兼容。
- **编译与运行**：`dotnet build G:\GCModeller\src\R-sharp\R_system.NET5.slnx -c Rsharp_app_release -p:Platform=x64`；该配置下 `Rlapack` 的 `OutputPath` 为 `../../App\`，产物落到 `App\net10.0`。若整解决方案因 slnx/SDK 受阻，退而单独 build `Rlapack.NET5.vbproj` 同配置。运行用 `"G:\GCModeller\src\R-sharp\App\net10.0\R#.exe" "<脚本绝对路径>"`（脚本路径为第一个位置参数），可加 `--sink` 保存日志。
- **报告口径**：按底层 README 实测偏差分级设置容差——`lambda`/`nulldev`/`df` 近似严格；`dev.ratio` 用相对容差（gaussian ~1e-5、binomial/poisson ~1e-3、multinomial ~2e-2）；系数 gaussian ~1e-3、poisson ~1e-2、binomial 放宽到 ~3.0 且以 dev.ratio/CV 曲线为主判据；`cvm/cvsd` 用相对容差；`lambda.min` 比较网格位置并允许若干步偏差。
- **用户规则处理**：本次测试主体为 R#/GNU R 脚本，报告由 `compare.R` 生成 CSV/文本，不新增 VB.NET console 测试工程，故不使用 `DebugAudit.vb`；若实施中确需新增 console 对拍程序，须改用 `DebugAudit` 记录结果并生成报告。

## 架构设计

```mermaid
flowchart LR
  subgraph GNU_R["GNU R 4.5.0 + glmnet 5.1"]
    A[export_golden.R] --> B[(data/*.csv 黄金数据)]
    B --> C[compare.R] --> D[(report/* 对比报告)]
  end
  subgraph Rsharp["R# 运行时 App/net10.0"]
    E[test/glmnet/*.R demo 脚本] --> F[glmnet.vb 适配层<br/>Package: glmnet]
    F --> G[GLMNet.dll<br/>GlmnetSolver / GlmnetPredict]
    G --> H[R 风格 list 结果]
    H --> I[(result/*.csv)]
    I --> C
  end
  J[dotnet build<br/>R_system.NET5.slnx<br/>Rsharp_app_release|x64] --> Rsharp
  B --> E
```

数据流：`Rscript.exe export_golden.R <data-dir>` → 生成输入与基准 CSV → `R#.exe demo_*.R` 读同一 CSV 调 `glmnet` 包 → 结果写 `result\*.csv` → `Rscript.exe compare.R <data-dir> <result-dir> <report-dir>` 逐字段断言并输出报告。适配层内部链路：`toMatrix` → `buildOptions` → `GlmnetSolver.Fit/CrossValidate` 或 `GlmnetPredict.Predict/Coef/Deviance` → `toRList`。

## 目录结构

本实现以单个 VB 模块为改动核心，测试脚本与数据全部收敛到 `test\glmnet`：

```
G:/GCModeller/src/R-sharp/
├── Library/Rlapack/
│   ├── glmnet.vb                       # [MODIFY] 主实现。由空壳扩展为完整包：<Package("glmnet")> + <RTypeExport>；
│   │                                   #   私有 toMatrix/toResponse/buildOptions/toRList/unwrap 辅助；
│   │                                   #   导出 glmnet / cv.glmnet / predict / coef / deviance /
│   │                                   #   print.glmnet / summary.glmnet / as.data.frame.glmnet；
│   │                                   #   <RInitialize> Main 注册 summary、makeDataframe、ConsolePrinter。
│   │                                   #   用命名空间别名规避 glmnet 模块名与 GLMNet 命名空间的 VB 大小写冲突。
│   ├── zzz.vb                          # [MODIFY/可选] 仅当放弃 <RInitialize> 时，在 onLoad 中加一行 Call glmnet.Main()；优先不改。
│   └── Rlapack.NET5.vbproj             # [不改] 已含 GLMNet.vbproj 引用与 Rsharp_app_release|x64 输出路径，仅核对。
├── R_system.NET5.slnx                  # [不改] 已含 Rsharp_app_release 与 x64 及 Rlapack 项目配置。
├── App/net10.0/                        # [产物] 编译后 Rlapack.dll / R#.exe 更新位置，不手工编辑。
└── test/glmnet/
    ├── R/
    │   ├── check_version.R             # [NEW] 核验 R 4.5.0 / glmnet 5.1 与默认参数，参考底层 test/R/check_version.R。
    │   ├── export_golden.R             # [NEW] 生成六家族黄金数据：*_x.csv、*_y.csv、*_foldid.csv、
    │   │                               #   *_fit.csv（step/lambda/a0/df/dev.ratio/nzero/nulldev）、
    │   │                               #   *_beta.csv、*_cv.csv；移植并适配底层 test/R/export_golden.R，
    │   │                               #   set.seed(1)、显式 foldid、write.table(sep=",", row.names=FALSE, quote=FALSE)。
    │   └── compare.R                   # [NEW] 读取 data/ 与 result/ 同构 CSV，按家族分级容差逐字段断言，
    │                                   #   输出 PASS/FAIL 明细 CSV 与人类可读文本报告；非零退出码表示失败。
    ├── demo_fit.R                      # [NEW] R# 脚本：imports "glmnet" from "Rlapack"; 逐家族 read.csv → glmnet(...) →
    │                                   #   print(fit)/summary/as.data.frame → write.csv 到 result/*_fit.csv 与 *_beta.csv。
    ├── demo_cv.R                       # [NEW] R# 脚本：读 *_foldid.csv 并换算为 0-based 传入 cv.glmnet，
    │                                   #   输出 result/*_cv.csv（lambda/cvm/cvsd/lambda.min/lambda.1se）。
    ├── demo_predict.R                  # [NEW] R# 脚本：验证 predict(link/response/class)、coef(lambda.min/lambda.1se/exact)、
    │                                   #   deviance 与 fit$lambda / fit$beta / cv$lambda.min 字段访问，写 result/*_predict.csv。
    ├── run-tests.cmd                   # [NEW] 串联：Rscript check_version → export_golden → dotnet build →
    │                                   #   R#.exe 三个 demo 脚本 → Rscript compare；用绝对路径与引号。
    ├── README.md                       # [NEW] 环境、命令顺序、目录约定、容差说明与故障排查。
    ├── data/                           # [NEW 生成] export_golden.R 产出的黄金 CSV（输入 + 基准）。
    ├── result/                         # [NEW 生成] R# demo 脚本产出的同构 CSV。
    └── report/                         # [NEW 生成] compare.R 产出的对比报告 CSV/TXT。
```

## Agent Extensions

### SubAgent

- **code-explorer**
- Purpose: 在动手前复核 R# 包导出与互操作细节（`<ExportAPI>`/`<RApiReturn>`/`<RTypeExport>` 的准确签名、`list.add` 与 `dataframe` 构造、`CLRVector.asNumeric`/`REnv.TryCastGenericArray` 用法、`generic.add`/`makeDataframe.addHandler`/`<RInitialize>` 的调用契约），并确认 `test/glmnet` 目录仍为空、底层 `GLMNet` 签名未变化。
- Expected outcome: 产出一份带文件与行号证据的核对结论，使 `glmnet.vb` 的签名、初始化方式与返回结构一次编译通过，避免虚构 API 或与既有 Rlapack 包约定冲突。