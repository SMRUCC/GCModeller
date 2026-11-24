---
name: vCella 单菌与菌群优化 demo（G:\fermenter\data）
overview: 用 Release|x64 构建生产版 vCella，在 G:\fermenter\data 下建立"单菌株"与"合成菌群"两个可整体拷贝、一键运行的优化 demo（含 GEM 模型、上下文映射、JSON 预设、cmd 批处理），跑出真实结果后撰写 README 使用说明与两份结果解读报告。
todos:
  - id: build-release-tool
    content: 用 Release|x64 构建 vCella，裁剪 *.pdb 后拷入 data\bin，并用 --help 双命令门禁
    status: in_progress
  - id: prepare-demo-layout
    content: 创建 single/consortium 目录，复制 GEM 模型与 db_context.ini，写入两份 preset.json
    status: completed
    dependencies:
      - build-release-tool
  - id: probe-and-freeze
    content: 小预算探针验证 S_Acetoin 边界池可解析，实测耗时后冻结单菌 3/20、群落 2/8 与快层档位
    status: completed
    dependencies:
      - prepare-demo-layout
  - id: write-cmd-scripts
    content: 编写两个 run.cmd 与 run-all.cmd，全相对路径、UTF-8、errorlevel 排错提示、结束 pause
    status: completed
    dependencies:
      - probe-and-freeze
  - id: run-both-demos
    content: 运行单菌与菌群 demo 至 exit 0，采集 result 报告文件、日志与实际耗时
    status: completed
    dependencies:
      - write-cmd-scripts
  - id: write-docs
    content: 用 [subagent:code-explorer] 核对字段与文件名后，写 README.md 与两份结果解读报告
    status: completed
    dependencies:
      - run-both-demos
  - id: verify-portability
    content: 校验脚本无绝对路径、外部目录可调用、文档数字与 result 产物逐项对上
    status: completed
    dependencies:
      - write-docs
---

## 用户需求

基于 `vCella` 已有的两个优化命令，在 `G:\fermenter\data` 下产出**两个可整体拷贝、一键复现**的代谢网络优化 demo，并配套文档。优化目标统一为最大化合成 `S_Acetoin`。

## 产品概述

`G:\fermenter\data` 将成为一个自包含的"优化引擎演示包"：一份生产版 `vCella` 二进制 + 两个 demo（单菌株、三物种合成菌群）+ 一键批处理 + 使用说明与结果解读报告。复制整个 `data` 目录到任意 Windows 机器即可复跑，不依赖 `G:\GCModeller` 与本机 `N:` 盘路径。

## 核心功能

### 1. 生产版工具载荷

- 用 `Release|x64` 配置编译 `vCella.vbproj`，得到生产模式命令行工具；
- 把产物整目录裁剪后放入 `data\bin\`（剔除 `*.pdb`），两个 demo 共用这一份；
- 门禁：`bin\vCella.exe --help /optimize` 与 `--help /optimize-consortium` 均正常返回。

### 2. 单菌株优化 demo（`data\single`）

- 模型：`Acetobacter pasteurianus IFO 3283-01 IFO 3283.xml`（自 `N:\GMNDesigner\Acetoin\models_test2\GEMs` 复制）；
- 上下文映射：同源 `db_context.ini`；
- 目标：`S_Acetoin`，目标池为**边界（分泌）池**，指标为均值滴度；
- 参数档位：时间步 3、评估预算 20，含全细胞复核与稳健性复核各 1 个候选；
- 产出：`candidates.csv`、`pareto.csv`、`best_design.json`、`config.json`、`stage_summary.md`，以及 `--report` 的审计报告。

### 3. 合成菌群优化 demo（`data\consortium`）

- 模型：3 个 GEM（Acetobacter pasteurianus、Lentilactobacillus buchneri、Limosilactobacillus fermentum），覆盖"生产者 + 交叉喂养伙伴"，且 `S_Acetoin` 在这些模型中命中最多；
- 目标：**共享培养基**中的 `S_Acetoin`，群落目标为总滴度；
- 参数档位：时间步 2、评估预算 8、三物种、确定性模式、共存阈值 5%；快层档位按实测耗时在"开环近似"与"跳过"之间冻结；
- 产出：`community.csv`（逐物种丰度/贡献）、`crossfeeding.csv`（交叉喂养边）、带 species 列的 `candidates.csv`、schema 2.0 的 `best_design.json`、`stage_summary.md` 与审计报告。

### 4. 一键运行脚本

- `data\run-all.cmd` 顺序执行两个 demo；每个 demo 各有 `run.cmd`；
- 全部使用相对路径，脚本内先切到自身目录，中文输出不乱码，失败时给出排错提示（缺 .NET 10 运行时、模型缺失、目标化合物解析失败），结束时列出结果文件并等待按键。

### 5. 文档

- `data\README.md`：工具总览（6 个命令）、两个优化命令的参数表与默认值、JSON 预设的字段与覆盖语义、输出文件清单、耗时参考、复现步骤与常见问题；
- `data\reports\single-strain.md`：单菌实验设置、野生型基线、最佳设计与推荐方案、逐阶段摘要、通路机理解读、局限；
- `data\reports\consortium.md`：群落实验设置、群落 fold、逐物种丰度与贡献、交叉喂养网络、跨物种编辑解读、共存约束结论、局限与提升建议。

## 运行效果

双击 `run-all.cmd` 后终端实时打印：命令头部、模型/目标/时间网格、阶段进度、阶段摘要表、最佳设计与推荐方案、耗时与报告目录；结束后 `result\` 下出现完整报告文件，与文档中的数字一一对应。

## 技术栈

沿用现有工程，不引入新依赖：

- 语言/框架：VB.NET，`net10.0`；构建用 `dotnet build`
- 宿主项目：`G:\GCModeller\src\GCModeller\engine\vCella\vCella.vbproj`（`Release|x64` 的 `OutputPath=../App/`）
- 命令行框架：`Microsoft.VisualBasic.CommandLine`（`RunCLI` + `ExportAPI`/`Usage`）
- 配置：`System.Text.Json`（`OptimizationPreset` / `ConsortiumPreset`）
- 运行环境：Windows + .NET 10 运行时（框架依赖部署）

## 实施策略

**总原则：只在 `G:\fermenter\data` 下新增文件；对 `G:\GCModeller` 的唯一写入是 Release 构建产物；`N:` 盘模型只读复制。**

1. **构建门禁先做**：`dotnet build .\vCella\vCella.vbproj -c Release -p:Platform=x64` 必须 0 error。若 MSBuild 未识别 x64 平台（产物不在 `engine\App\net10.0`），退化为 `-c Release`（产物在 `vCella\bin\Release\net10.0`），并在 README 注明实际构建配置——不影响 demo 的可复现性。
2. **载荷裁剪**：拷整目录到 `data\bin`，删除 `*.pdb`（约 42.9 MB → 约 30 MB），保留 `*.dll.config`、`*.json`、`runtimes\`。
3. **目标池必须选对**：`S_Acetoin` 是分泌产物，只存在于边界集合。单菌用 `--pool boundary`（`internal` 池读不到它）；群落默认 `shared`。这是本任务最容易踩错的一点。
4. **路径双轨制**：`ContextMap`/`MediumFile`/`Species[].Model` 由预设加载器按 **config 文件所在目录** 解析（可写相对路径）；`OutputDirectory` 则按 **当前工作目录** 解析——因此预设里不写 `OutputDirectory`，改由 `run.cmd` 用 `--output=result` 指定（脚本已 `cd /d "%~dp0"`）。
5. **成本校准后冻结参数**：GEM 规模下单次评估可达数十秒，必须先探针实测再定档，避免 demo 超过 15 分钟。
6. **文档事后写**：所有数字、文件名、耗时都从真实 `result\` 产物中提取，禁止凭空估计。

关键决策与理由：

- **不修改 `vCella` 任何源码**：本轮是"用工具 + 出 demo + 出文档"，改工具会把风险引入已验证的生产链路。
- **`data\bin` 共享二进制**：两个 demo 各带一份会让体积翻倍，而复制整个 `data` 目录已满足"整体搬运"的诉求。
- **群落只放 3 个模型**：目录内模型数即物种数，天然稳定，无需依赖 `--species` 截断；3 物种能同时展示"生产者 + 交叉喂养伙伴"的群落效应。

## 目录结构

```
G:\fermenter\data\
├── README.md                     # [NEW] 工具使用说明（中文）
├── run-all.cmd                   # [NEW] 顺序执行两个 demo
├── bin\                          # [NEW] Release|x64 产物（剔除 *.pdb，约 30 MB）
│   ├── vCella.exe / vCella.dll / vCella.runtimeconfig.json / vCella.deps.json
│   └── <依赖 dll + *.dll.config + runtimes\>
├── single\
│   ├── models\Acetobacter pasteurianus IFO 3283-01 IFO 3283.xml   # [COPY] 22.5 MB，只读复制
│   ├── config\db_context.ini                                      # [COPY] 上下文映射（介质配方 + 被动通透）
│   ├── preset.json                                                # [NEW] OptimizationPreset
│   ├── run.cmd                                                    # [NEW] 一键运行
│   └── result\                                                    # 运行时生成（*.optimize 目录）
├── consortium\
│   ├── models\Acetobacter pasteurianus ... .xml                   # [COPY]
│   ├── models\Lentilactobacillus buchneri ATCC 4005.xml            # [COPY]
│   ├── models\Limosilactobacillus fermentum LMT2-75.xml            # [COPY]
│   ├── config\db_context.ini                                      # [COPY]
│   ├── preset.json                                                # [NEW] ConsortiumPreset
│   ├── run.cmd                                                    # [NEW] 一键运行
│   └── result\                                                    # 运行时生成（*.consortium-optimize 目录）
└── reports\
    ├── single-strain.md                                           # [NEW] 单菌结果解读
    └── consortium.md                                              # [NEW] 菌群结果解读
```

## 关键实现细节

### 1. 预设文件（`--config`）

单菌 `single\preset.json`：

```
{
  "TargetCompound": "S_Acetoin",
  "TargetPool": "boundary",
  "Metric": "mean",
  "Steps": 3,
  "TimeStep": 1.0,
  "EvaluationBudget": 20,
  "ContextMap": "config\\db_context.ini",
  "MaxKnockouts": 2,
  "MaxKcatEdits": 2,
  "KcatLevels": [0.5, 2.0],
  "ScanLevels": [0.5, 2.0],
  "WholeCellTopN": 1,
  "RobustnessTopN": 1,
  "RobustnessMediumFactors": [1.0],
  "RobustnessKcatJitter": 0.0,
  "RequireTrained": false
}
```

群落 `consortium\preset.json`：在公共字段基础上追加

```
{
  "TargetCompound": "S_Acetoin",
  "TargetPool": "shared",
  "Metric": "mean",
  "Steps": 2,
  "TimeStep": 1.0,
  "EvaluationBudget": 8,
  "ContextMap": "config\\db_context.ini",
  "MaxKnockouts": 2,
  "MaxKcatEdits": 2,
  "SurrogateMode": "<按实测冻结>",
  "SurrogateIterations": 2,
  "Objective": "community",
  "MinSpeciesAbundance": 0.05,
  "WholeCellTopN": 1,
  "RobustnessTopN": 1,
  "CheckSpeciesDropout": false,
  "Deterministic": true
}
```

`Cells`（接种数）与 `SpeciesLimit`（物种上限）由 `run.cmd` 显式传 `--cells 1`，不写进预设，避免与目录内模型数耦合。

### 2. `run.cmd` 模板要点

```
@echo off
chcp 65001 >nul
setlocal
cd /d "%~dp0"
if not exist result mkdir result
echo [cmd] ..\bin\vCella.exe /optimize --model="models\..." --config=preset.json --output=result --report
..\bin\vCella.exe /optimize --model="models\..." --config=preset.json --output=result --report > "result\run.log" 2>&1
if errorlevel 1 ( ... 打印排错提示 ... ) else ( ... 列出报告文件 ... )
pause
```

约束：只用相对路径（`..\bin`、`models`、`config`、`result`），不出现 `G:\`、`N:\`；`run-all.cmd` 用 `call` 串行调用两个脚本。

### 3. 成本校准（参数冻结）

1. 探针：单菌 `--steps 1 --budget 1`、群落 `--steps 1 --budget 1 --surrogate-mode open`，记录单次评估耗时 T 与 WT 录制耗时；
2. 估算：`总时长 ≈ WT 录制 + budget × 物种数 × T`；
3. 以 ≤15 分钟为约束，在 `open`（能捕捉交叉喂养，较慢）与 `none`（走慢层批量管线，较快）之间二选一，必要时下调 `budget`（8 → 4/6）；
4. 冻结后回写 `preset.json`，并把实测耗时写进 README。

### 4. 结果采集清单

| demo | schema | 文件 |
| --- | --- | --- |
| 单菌 | `cella-optimization/1.0` | `candidates.csv`、`pareto.csv`、`best_design.json`、`config.json`、`stage_summary.md`、`optimize_report.txt` |
| 群落 | `cella-optimization/2.0` | `community.csv`、`candidates.csv`、`crossfeeding.csv`、`best_design.json`、`config.json`、`stage_summary.md`、`consortium_report.txt` |


文档引用前用 [subagent:code-explorer] 核对 `DesignReport.Save` / `CommunityDesignReport.Save` 的实际文件名与 `OptimizationPreset` 的字段名，避免文档与产物不符。

### 5. 性能与可靠性

- 单次运行 3–15 分钟，后台执行 + 轮询日志，避免前端超时；
- 全部评估走确定性路径（群落 `Deterministic=true`），同一机器重复运行结果可复现；
- 报告写入 `result\`，重复运行前不清空（保留历史），README 说明如何清理。

### 6. 风险与应对

| 风险 | 应对 |
| --- | --- |
| `S_Acetoin` 不在该模型的边界集合（转运通道缺失） | 探针阶段即可发现（banner 的 `target compound` 与 `wild-type value`）；备选：改 `--target auto` 或换 `Lentilactobacillus buchneri`，并在 README 记录原因 |
| 未训练模板 → 结果仅具链路验证意义 | 运行时会打印醒目 WARNING；两份解读报告必须显著标注该前提 |
| 3 物种 + 开环快层超时 | 按 4 档位校准，必要时降 `budget` 或改 `none`，README 写明降档原因与提升方法 |
| `Release | x64` 平台未生效 | 退化 `-c Release`，产物改从 `vCella\bin\Release\net10.0` 拷贝 |
| 目标机无 .NET 10 运行时 | README 前置条件给出 `dotnet --list-runtimes` 自检命令与安装指引 |
| 双击后窗口一闪而过 | 脚本结尾 `pause`，且 `cd /d "%~dp0"` 保证定位正确 |


## 验收标准

1. `Release|x64` 构建 0 error；`data\bin\vCella.exe --help /optimize`、`/optimize-consortium` 均 exit 0。
2. 单菌 `run.cmd` exit 0；banner 显示 `target compound : S_Acetoin` 且野生型值 > 0；`result\` 下 6 个报告文件齐备。
3. 群落 `run.cmd` exit 0；`community.csv` 含 3 个物种行且丰度合计约 1；`crossfeeding.csv` 非空；`best_design.json` schema 为 `2.0`。
4. 可移植性：从 `C:\` 等外部目录调用 `data\single\run.cmd` 能正常启动；`Select-String -Pattern "G:\\|N:\\"` 在所有 `.cmd` 中零命中。
5. 文档一致性：README 与两份报告里的数字、文件名、命令都能在 `result\` 中对应；耗时栏与实测一致。
6. 两份报告均显式标注"未训练模板 → 结果仅供链路验证"（若运行时触发该警告）。

## Agent Extensions

### SubAgent

- **code-explorer**
- Purpose: 在撰写 README 与两份解读报告前，核对 `DesignReport.Save` 与 `CommunityDesignReport.Save` 实际落盘的文件名、`OptimizationPreset` / `ConsortiumPreset` 的完整字段名与默认值、以及 `--help /optimize` 与 `--help /optimize-consortium` 的真实参数文本。
- Expected outcome: 一份"字段名 + 文件名 + 默认值"对照清单，确保文档中的参数表、输出文件清单与实际产物完全一致，不出现凭记忆写出的错误字段。