---
name: MLR_XMLDocs
overview: 为 runtime/sciBASIC#/Data_science/Mathematica/Math/DataFittings/Linear/MLR 文件夹下三个 VB.NET 源文件（Error.vb、LinearFitting.vb、MLRFit.vb）中的所有类、结构体、属性、函数、模块补充完整的 XML 文档注释，使其文档覆盖率达到 100%。
todos:
  - id: doc-error
    content: 为 Error.vb 的 Structure [Error] 各成员补全 XML 注释
    status: completed
  - id: doc-linearfitting
    content: 为 LinearFitting.vb 的 Module 全部函数补全/补完 XML 注释
    status: completed
  - id: doc-mlrofit
    content: 为 MLRFit.vb 的 Class MLRFit 缺失与空 XML 注释补全
    status: completed
  - id: verify-docs
    content: 核对三个文件 XML 注释覆盖率与签名一致性
    status: completed
    dependencies:
      - doc-error
      - doc-linearfitting
      - doc-mlrofit
---

## 用户需求

完善 `runtime\sciBASIC#\Data_science\Mathematica\Math\DataFittings\Linear\MLR` 文件夹下三个 VB.NET 源代码中每一个公开对象（类/结构/模块/属性/方法/函数）的 XML 文档注释（`''' <summary>`、`''' <param>`、`''' <returns>`），使覆盖率达到 100%。

## 产品概述

针对 Multivariate 命名空间下的多元线性回归模块进行文档补全，仅补充 XML 注释节点，不改动任何逻辑代码、不改动文件头 #Region 自动生成的统计信息块。

## 核心功能

- 为 Error.vb 中 `Structure [Error]` 的属性 X/Y/Yfit、方法 ToString、共享迭代器 RunTest 补全 XML 注释
- 为 LinearFitting.vb 中 `Module LinearFittingAlgorithm` 的各函数（含已存在骨架的扩展方法 LinearFitting、CurveScale，以及 left/right/ConfidenceInterval 等）补全空 summary、param、returns
- 为 MLRFit.vb 中 `Class MLRFit` 缺失 summary 的属性（N、SSE、SST）与空 summary、空 param/returns、GetY 方法、共享 LinearFitting 方法补全 XML 注释
- 保持与项目一致的中英混合简洁注释风格，语义准确（参考 IFitError、IFitted、MultivariatePolynomial 接口定义）

## 技术栈

- 语言：VB.NET（.NET Framework 科学计算库 sciBASIC#）
- 文档标准：Microsoft XML Documentation Comments（`'''` 三引号注释）
- 相关类型引用：`Microsoft.VisualBasic.Math.LinearAlgebra.Vector`、`GeneralMatrix`、`NumericMatrix`、`Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.GeneralMatrix`、`Formula`、`MultivariatePolynomial`、`IFitError`、`IFitted`

## 实现方案

采用"就地补全 XML 注释节点"策略：仅向现有对象上方插入/填充 `''' <summary>`、`''' <param name="...">`、`''' <returns>` 等三引号注释，不修改任何可执行代码，不触碰文件头部 `#Region` 自动生成的 Code Statistics 块。注释风格沿用项目既有的中英混合简洁风格（参考 IFitted.vb、MultivariatePolynomial.vb）。

关键决策与依据：

- 不引入新工具或脚本生成注释，手工结合源码语义与接口契约（IFitError 定义 Y/Yfit；IFitted 定义 R2/Polynomial/ErrorTest/GetY）编写，确保注释与签名严格一致，避免自动生成导致的语义错误。
- 对已有骨架注释（如 LinearFitting 扩展方法的 X/f 矩阵示例、CurveScale 的公式）予以保留并补完缺失的 summary/returns，不重复造轮子。
- 对空 `''' <summary>` 节点（如 MLRFit.N、SSE、Fx 的 param x、GetY 等）填充准确描述。

性能与可靠性：纯注释改动，零运行时影响；编译期可经 VB 编译器 `/doc` 校验无 warning。

## 实现注意

- 仅修改 XML 注释节点，保持缩进与既有注释风格（中英混合、简洁）。
- 不修改 `#Region` 头到 `#End Region` 之间的版权/统计信息。
- 对 `Implements` 接口成员，summary 应与接口定义语义一致（如 R2=相关系数、Polynomial=多项式）。
- 对每个 `param` 与 `returns` 节点确保名称与签名参数/返回完全一致，避免编译器 XML 校验告警。

## 架构设计

本任务为纯文档补全，不改动架构与数据流。MLR 模块现有结构：

- `MLRFit`（Class，实现 IFitted）：多元线性回归结果模型
- `LinearFittingAlgorithm`（Module）：提供拟合入口、曲线升维、置信区间计算
- `Error`（Structure，实现 IFitError）：单点拟合误差记录与测试迭代器

三者协同流程：`LinearFittingAlgorithm.LinearFitting` → 构造 `MLRFit` → 调用 `Error.RunTest` 生成误差数组写入 `MLRFit.ErrorTest`。

## 目录结构

```
runtime/sciBASIC#/Data_science/Mathematica/Math/DataFittings/Linear/MLR/
├── Error.vb          # [MODIFY] 为 Structure [Error] 的属性 X/Y/Yfit、ToString、Shared RunTest 补全 XML 注释
├── LinearFitting.vb  # [MODIFY] 为 Module LinearFittingAlgorithm 全部函数补全/补完 XML 注释（含已有骨架）
└── MLRFit.vb         # [MODIFY] 为 Class MLRFit 缺失/空 summary 的属性与方法补全 XML 注释
```