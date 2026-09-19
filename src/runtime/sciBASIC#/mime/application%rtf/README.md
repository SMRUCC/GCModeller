# RTF 与 Office MathML 文档模型

## 引言：为什么还需要 RTF？

在富文本格式层出不穷的今天，RTF（Rich Text Format）依然有它不可替代的位置：它是**纯文本**，可以用任意编辑器打开与 diff；它被 Word、LibreOffice、写字板等几乎全部办公软件原生支持；它不需要引入 OOXML 那样庞大的对象模型，就能生成一份带字体、颜色与样式的文档。

`Microsoft.VisualBasic.MIME.RTF` 正是 `sciBASIC#` 中负责这一层的轻量实现，由两部分组成：

1. **RTF 文档对象模型** —— 逐步追加带样式的文本，最终输出 `.rtf` 文件；
2. **Office / OMML XML 模型** —— 可序列化的 Office 数学标记（OMML）与文档属性模型，用于 Word XML 往返。

## 设计目标

- **够用就好**：类模型刻意保持精简，只有「文档 / 格式区域 / 字体」三个核心概念。
- **增量构建**：`AppendText` / `AppendLine` 每次调用都可以携带自己的字体样式，文档在追加过程中被组织为若干「格式区域」。
- **无损序列化**：Office / OMML 模型全部是可序列化的 XML 模型，便于与报告生成流水线对接。

## 核心特性

- 通过 `AppendText` / `AppendLine` 增量构建文档，每次调用可选择自带字体样式；
- 在文本缓冲之上跟踪**样式区域**，可通过 `SetFormat` 重新应用或拆分格式；
- 通过 `Save` 输出 RTF 1.x 文档（字体表、颜色表、生成器元数据）；
- 提供可序列化的 **Office / OMML XML 模型** —— 文档属性、Word 文档设置、数学属性与 html head，用于 Word XML 往返。

## 命名空间地图

| 命名空间 | 职责 |
|---|---|
| `Microsoft.VisualBasic.MIME.RTF`（根） | RTF 文档模型：`Rtf`、`FormatedRegion`、`Font` |
| `Microsoft.VisualBasic.MIME.RTF.Models` | 共享字体数据模型 `Font`（字族、字号、粗体、斜体、下划线、颜色） |
| `Microsoft.VisualBasic.MIME.RTF.Omml` | Office / OMML XML 模型：`HTML`、`DocumentXmlProperty`、`DocumentProperties`、`WordDocument`、`OfficeDocumentSettings`、`mathPr`、`Paragraph`、`Font`、`StyleTokens` |

## 关键类型与 API

- `Microsoft.VisualBasic.MIME.RTF.Rtf` —— 文档对象模型：追加带样式的文本并写出 `.rtf` 文件；
- `Microsoft.VisualBasic.MIME.RTF.Font` —— 文本区域的字体样式，同时负责生成 RTF 样式标记（`\b`、`\i`、`\fs`、`\ul`）；
- `Models.Font` —— 被 `Font` 继承的纯字体数据（字族、字号、粗体、斜体、下划线、颜色）；
- `FormatedRegion` —— 文档中的一个样式化文本片段：记录自身的起止偏移并渲染自己的 RTF 文本；
- `Omml.HTML` —— 携带 Word / VML / OMML 命名空间声明的 Office html/xml 文档根模型；
- `Omml.DocumentXmlProperty` / `Omml.DocumentProperties` —— 可序列化为 Office XML 的 `xml` 与属性块；
- `Omml.WordDocument` / `Omml.OfficeDocumentSettings` / `Omml.mathPr` —— Word 文档设置、兼容性与数学属性模型。

## 快速上手

```vbnet
Imports System.Drawing
Imports Microsoft.VisualBasic.MIME.RTF

Dim doc As New Rtf("Cambria", 11, Color.Black)

Call doc.AppendLine("Hello sciBASIC#")
Call doc.AppendLine("styled text",
                    New Font(size:=11, Bold:=True, Name:="Cambria",
                             Italic:=False, Underline:=False, Color:=Color.Red))
Call doc.Save("demo.rtf")
```

## 实现要点

- **样式区域模型**：文档内部维护文本缓冲与格式区域列表；`SetFormat` 在修改已有区域样式时会按需**拆分区域**，从而保证任意位置都能被重新着色或加粗。
- **RTF 标记生成**：`Font` 负责把字体属性翻译为 RTF 控制字（`\b`、`\i`、`\fs`、`\ul`），`Rtf.Save` 汇总字体表、颜色表与生成器元数据后落盘。
- **OMML 与报告流水线**：`Omml.*` 模型与 `sciBASIC#` 的报告生成（Word XML 往返）共享，因此公式与文档元数据可以在 `.docx` 与 RTF 之间迁移。

## 与 sciBASIC# 生态的关系

本包是 `mime` 家族的一员，与 `Microsoft.VisualBasic.MIME.Office.WordDocument`（.docx）、`Excel`、`PDF` 等同属文档输出能力；其 OMML 模型被报告生成流水线复用。

## 包信息

- Assembly：`Microsoft.VisualBasic.MIME.RTF`
- TargetFramework：`net4.8`（随 Windows 桌面工具链使用）
- Tags：`scibasic;rtf;rich-text-format;omml;office-math;document-model;word`
- 说明：本项目为旧式（非 SDK）工程，`PackageTags` / `PackageReleaseNotes` / `PackageReadmeFile` 与旧式 `Tags` / `ReleaseNotes` 属性同时提供，以兼容不同的打包方式。

## 许可证

GPL-3.0-or-later
