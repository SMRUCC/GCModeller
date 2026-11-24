---
name: add-pathdto-metabolite-markers
overview: 在 PathDto 中新增 startMetabolite / targetMetabolite 两个字符串标记属性，由 Netwalk.Search 在组装候选通路时回填（起点取正向第一步的主前体 SMILES，目标取查询的目标 SMILES），并更新 BioCyc 测试项目的调试输出以展示这两个新字段。
todos:
  - id: add-pathdto-props
    content: 在 Pathway.vb 的 PathDto 中新增 StartMetabolite 与 TargetMetabolite 属性（JSON 键 start_metabolite / target_metabolite，附注释）
    status: completed
  - id: fill-netwalk-search
    content: 在 Netwalk.vb 的 Search 方法 PathDto 构造处回填 TargetMetabolite=targetSmiles、StartMetabolite=fwdSteps 首步主前体（含空列表兜底）
    status: completed
    dependencies:
      - add-pathdto-props
  - id: update-test-output
    content: 更新 BioCyc/test 的 MetabolicDemo.vb 与 PathwayFinderDemo.vb 中 PrintReport，逐条打印 startMetabolite 与 targetMetabolite 调试信息
    status: completed
    dependencies:
      - add-pathdto-props
  - id: build-verify
    content: 编译 MetabolicRouter 与 BioCyc/test 项目，确认无错误且新字段在输出与 JSON 中正确显示
    status: completed
    dependencies:
      - fill-netwalk-search
      - update-test-output
---

## 需求概述

优化代谢路径搜索结果的标注信息：为逆向合成通路搜索返回的每条候选路径补充起止标记。

## Product Overview

在 <code>PathReport.Paths</code> 的每个 <code>PathDto</code> 候选通路对象上新增两个字符串属性：

- <code>StartMetabolite</code>：该条通路的起始代谢物（正向生物合成的最上游起始原料，即逆合成最深一步的主前体，以 SMILES 表示）；
- <code>TargetMetabolite</code>：该条通路的最终目标化合物（本次查询的目标 SMILES）。

## Core Features

- <code>PathDto</code> 新增 <code>startMetabolite</code> / <code>targetMetabolite</code> 两个字符串属性，并纳入 JSON 序列化输出（遵循现有 snake_case 命名约定：<code>start_metabolite</code> / <code>target_metabolite</code>）
- 在搜索结果组装处自动回填两个字段：同一报告内 targetMetabolite 相同，startMetabolite 随各候选路径不同而不同
- 更新 BioCyc 测试项目（test.vbproj）中的调试输出代码（MetabolicDemo.vb / PathwayFinderDemo.vb 的 PrintReport），在控制台逐条打印 startMetabolite 与 targetMetabolite 供查看；PathwayFinderDemo 导出的 JSON 亦自动携带新字段
- 向后兼容：仅新增字段，不改动现有字段与搜索逻辑，不影响路径评分与排序结果

## 技术方案

### 技术栈

- 语言：VB.NET（.NET），与现有 MetabolicRouter 模块一致
- 序列化：System.Text.Json（<code>JsonPropertyName</code> 特性），沿用现有 DTO 模式

### 实现方式

1. **模型层**（Pathway.vb）：在 <code>PathDto</code> 中新增两个属性并附 XML 注释与 <code>JsonPropertyName</code>：

- <code>StartMetabolite</code> → JSON <code>start_metabolite</code>
- <code>TargetMetabolite</code> → JSON <code>target_metabolite</code>

2. **回填点**（Netwalk.vb 的 <code>Search(targetSmiles)</code>，108–128 行的 PathDto 构造循环）：

- <code>TargetMetabolite = targetSmiles</code>（该报告的查询目标，与 <code>report.Target</code> 一致）
- <code>StartMetabolite = fwdSteps(0).Substrates(0)</code>：<code>Scoring.AssembleForward</code> 将逆合成步骤反转为正向顺序，正向第一步的底物列表中主前体排在最前（Scoring.vb 注释明确「主前体在前」），即该通路的最上游起始代谢物；对 <code>fwdSteps</code> 为空或 <code>Substrates</code> 为空的边界情况返回空串兜底
- 该构造点是 MetabolicAdapter 与 BioCycAdapter 共用入口，一处修改两套适配器同时生效

3. **测试输出**（models\BioCyc\test）：

- MetabolicDemo.vb 的 <code>PrintReport</code>：在每条路径头部信息中追加打印「起始代谢物 / 目标代谢物」
- PathwayFinderDemo.vb 的 <code>PrintReport</code>：同样追加打印；其导出的 biocyc_pathways.json 经 JsonSerializer 自动包含新字段

### 关键代码结构

```
' Pathway.vb — PathDto 新增属性
''' <summary>该通路正向生物合成的起始代谢物（最上游主前体）的 SMILES。</summary>
<JsonPropertyName("start_metabolite")>
Public Property StartMetabolite As String

''' <summary>该通路的最终目标化合物的 SMILES。</summary>
<JsonPropertyName("target_metabolite")>
Public Property TargetMetabolite As String
```

### 性能与影响面

- 起始代谢物直接取自已组装好的 <code>fwdSteps</code>，无额外解析开销（O(1)）
- 仅新增字段赋值，不触碰 BeamSearch / Scoring / 评分排序逻辑，回归风险最小

## Agent Extensions

无（本次任务目标文件与调用链已全部定位，无需额外扩展）