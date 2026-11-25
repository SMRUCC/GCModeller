---
name: Marked LaTeX 公式渲染插件化支持
overview: 在核心运行时新增公式渲染抽象接口，由 MathDraw 实现 LaTeX→MathML→PNG data URI 渲染，由 markdown 项目以可选插件方式消费，最后在 Marked WinForms 阅读器中组装预览渲染流程。
todos:
  - id: add-core-math-interface
    content: 新增核心公式渲染接口并定义返回契约
    status: completed
  - id: implement-mathdraw-plugin
    content: 在 MathDraw 实现 LaTeX 图片渲染插件
    status: completed
    dependencies:
      - add-core-math-interface
  - id: add-markdown-plugin-hook
    content: 为 markdown 渲染链增加可选公式钩子
    status: completed
    dependencies:
      - add-core-math-interface
  - id: wire-marked-preview
    content: 在 Marked 预览流程注入公式插件
    status: completed
    dependencies:
      - implement-mathdraw-plugin
      - add-markdown-plugin-hook
  - id: extend-debugaudit-tests
    content: 扩展 DebugAudit 测试并生成审计报告
    status: completed
    dependencies:
      - wire-marked-preview
  - id: verify-build-regressions
    content: 构建验证依赖边界与公式显示效果
    status: completed
    dependencies:
      - extend-debugaudit-tests
---

## User Requirements

为现有 Markdown 阅读器增加 LaTeX 数学公式显示能力：块级公式与行内公式都应在预览中可见，公式以清晰图片形式随文档排版展示。解析与渲染模块通过统一抽象以可选方式接入公式能力，未接入时保留原有文本显示行为。阅读器负责在最终预览流程中启用公式渲染实现。测试需使用核心运行时提供的调试审计模块生成报告。

## Product Overview

阅读器预览中的 `$...、公式围栏以及 `$... 将显示为排版后的数学公式；块级公式居中成行，行内公式随正文文字对齐。公式渲染失败时回退为原始 LaTeX 文本，不影响其他 Markdown 内容显示。

## Core Features

- 支持块级公式：`$... 与 `math/tex/latex/equation` 围栏。
- 支持行内公式：`$...，并避免与代码、链接等语法冲突。
- 公式作为内联图片数据嵌入预览页面，避免本地路径与跨域加载问题。
- 公式能力为可选插件：未启用时公式语法不被特殊处理。
- 渲染失败、公式为空或不合法时安全降级为文本。

[object Object]]]>