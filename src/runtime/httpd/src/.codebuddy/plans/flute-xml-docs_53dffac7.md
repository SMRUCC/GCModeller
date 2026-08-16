---
name: flute-xml-docs
overview: 为 Flute 文件夹内所有 .vb 源文件中每一个公开的（Public/Friend）类型与成员（Class/Module/Structure/Interface/Enum 及其 Public/Friend 的 Property/Function/Sub/Field/Constructor/Event/Operator）按照其实际功能与代码逻辑补全 XML 文档注释（''' <summary> 等），已注释的保持或完善，未注释的补齐，确保覆盖完整、风格与现有注释一致。
todos:
  - id: doc-root-files
    content: 补全根目录 ServerComponent.vb、HttpDriver.vb、Extensions.vb、SessionManager.vb 的公开对象 XML 注释
    status: completed
  - id: doc-configuration
    content: 补全 Configuration/Configuration.vb 与 Configuration/Session.vb 的公开对象 XML 注释
    status: completed
  - id: doc-http-core
    content: 补全 Http/HttpProcessor.vb、HttpServer.vb、HttpSocket.vb、Http/Options/Preflight.vb 的公开对象 XML 注释
    status: completed
  - id: doc-websocket-longpoll
    content: 查漏补缺 Http/WebSocket/ 与 Http/LongPoll/ 共 8 个文件的公开对象 XML 注释
    status: completed
  - id: doc-httpstream
    content: 补全 Http/HttpStream/ 下 PostReader.vb、HttpPostedFile.vb、HttpMultipart.vb、StreamElement.vb 的公开对象 XML 注释
    status: completed
  - id: doc-httpmessage
    content: 补全 HttpMessage/ 下 HttpRequest.vb、HttpPOSTRequest.vb、HttpResponse.vb、JsonResponse.vb、Content.vb 及 Protocol/ 下 HttpHeader.vb、Cookies.vb、HttpError.vb、WebForm.vb 的公开对象 XML 注释
    status: completed
  - id: doc-filesystem
    content: 补全 FileSystem/FileSystem.vb、WebFileSystem.vb 及 FileObject/FileObject.vb、MemoryCachedFile.vb、VirtualMappedFile.vb 的公开对象 XML 注释
    status: completed
  - id: build-verify
    content: 对 Flute 项目执行 dotnet build 验证编译通过且无新增错误
    status: completed
    dependencies:
      - doc-root-files
      - doc-configuration
      - doc-http-core
      - doc-websocket-longpoll
      - doc-httpstream
      - doc-httpmessage
      - doc-filesystem
---

## 用户需求

为 Flute 文件夹（`g:/GCModeller/src/runtime/httpd/src/Flute`）内全部 `.vb` 源文件中每一个公开（Public/Friend）类型与成员，按照其实际功能与代码逻辑补全 XML 文档注释。

## 产品概述

Flute 是一个用 VB.NET 编写的轻量级 HTTP 服务器核心库。当前代码库中存在大量公开类型与成员缺少 XML 文档注释（约 700+ 处公开对象，仅约 380 处有注释，覆盖不均衡）。本任务以"按代码逻辑写实注释"为目标，对齐现有注释风格，补齐缺失的 `''' <summary>` 及其 `<param>`/`<returns>`/`<remarks>`/`<example>` 等子标签，不改动任何运行逻辑。

## 核心功能

- 扫描 Flute 目录下所有 `.vb` 源文件（排除 `My Project/`、`bin/`、`obj/` 等生成目录）
- 为每一个 Public/Friend 的 Class、Module、Structure、Interface、Enum 编写 `<summary>` 类型级注释
- 为上述类型中每一个 Public/Friend 成员（Property、Function、Sub、Field、Constructor、Event、Operator）编写 `<summary>`，有参数则用 `<param>`，有返回值则用 `<returns>`，必要时补 `<remarks>`/`<example>`/`<exception>`
- 对已有但空泛/不准确的注释按真实代码逻辑完善；对已有且准确的注释保持不动
- 注释内容须基于实际代码行为，不臆造、不泛泛而谈

## 技术栈

- 语言：VB.NET（.NET 10.0）
- 注释规范：Visual Basic XML 文档注释（`'''` 三引号），标签包括 `<summary>`、`<param name="x">`、`<returns>`、`<remarks>`、`<example>`、`<exception>`、`<typeparam>`、`<value>`
- 工具：基于现有 `read_file`/`search_content` 的静态阅读，逐个文件补齐；可用 [subagent:code-explorer] 批量辅助定位缺注释的公开符号

## 实现方案

### 设计策略

按"文件分组、由核心到外围、先类型后成员"的顺序，逐文件补全 XML 注释。每个文件的处理流程为：

1. 读取文件全量内容，识别所有 Public/Friend 类型与成员
2. 比对现有注释，标出缺失或空泛处
3. 结合调用关系与代码逻辑撰写准确注释
4. 就地插入/完善注释，保留已有准确注释与文件头 `#Region`（GPL 头、Summaries 块）

### 关键技术决策

1. **注释风格对齐**：以 `WebFileSystem.vb`、`HttpResponse.vb`、`Configuration.vb`、`WebSocket*.vb`、`LongPoll*.vb` 为范本，统一使用英文 `<summary>`（与现有 380 处一致），参数说明用 `<param>`。
2. **仅注释公开面**：Private/Protected 成员（如 `HttpProcessor.m_webSocketHijacked`）不强制注释，但若其承载关键逻辑且与公开行为相关可酌情补充 `<remarks>`；受保护字段（如 `ServerComponent.settings`）因其对派生类可见，补充 `<summary>`。
3. **避免代码改动**：只新增/完善 `'''` 注释行，不动 `Imports`、方法体、签名、文件头 Region。
4. **分组执行降低风险**：将 34 个文件按模块划分为 8 个批次任务，每批集中处理相关文件，便于增量验证编译。

### 性能与可靠性

- 注释改动不改变 IL 行为，编译不应引入错误；每批完成后执行 `dotnet build` 仅验证（警告可接受，错误须清零，当前已有 1 个预存无关 Warning）。
- 使用 `<param>` 名称必须与形参严格一致，否则会产生 CS1573/BC42305 文档警告（虽有 Resharper 抑制，仍应尽量规范）。

## 实现注意事项

- 排除目录：`My Project/`、`bin/`、`obj/`、`Resources/`（生成或资源文件）。
- `ServerComponent.vb`、`HttpDriver.vb`、`Extensions.vb`、`SessionManager.vb`、`HttpSocket.vb`、`FileSystem/FileObject/*` 等文件注释覆盖极低，是重点补齐对象。
- `WebSocket*`、`LongPoll*` 系列已较完整，仅需查漏补缺。
- 枚举（Enum）成员（如 `WebSocketOpcode`、`WebSocketCloseCode`）若缺注释须逐个补 `<summary>`。
- 接口（如 `IWebSocketHandler`、`ILongPollHandler`、`IAppHandler`）的每个方法须有 `<summary>`，实现类可引用接口注释或补充自身说明。

## 架构设计

本任务为纯文档补全，不引入新架构。改动分布在 Flute 各 `.vb` 文件的注释区域，保持现有命名空间与类型结构不变。

## 目录结构

```
Flute/
├── ServerComponent.vb              # [MODIFY] MustInherit 基类，补类型与构造函数注释
├── HttpDriver.vb                  # [MODIFY] 驱动封装，补类型与公开方法注释
├── Extensions.vb                 # [MODIFY] 扩展方法，补类型与每个方法注释
├── SessionManager.vb             # [MODIFY] 会话管理，补类型与成员注释
├── Configuration/
│   ├── Configuration.vb          # [MODIFY] 配置类，查漏补缺（已有较好）
│   └── Session.vb                # [MODIFY] 会话对象，补类型与成员注释
├── Http/
│   ├── HttpProcessor.vb          # [MODIFY] 请求处理器，查漏补缺
│   ├── HttpServer.vb             # [MODIFY] 服务器基类，查漏补缺
│   ├── HttpSocket.vb             # [MODIFY] 具体服务器实现，补注释
│   ├── Options/Preflight.vb      # [MODIFY] CORS 预检，补注释
│   ├── WebSocket/                # [MODIFY] 5 个文件，查漏补缺
│   ├── LongPoll/                 # [MODIFY] 3 个文件，查漏补缺
│   └── HttpStream/               # [MODIFY] 4 个文件，补注释
├── HttpMessage/                  # [MODIFY] 5 个文件 + Protocol/ 4 个文件，补注释
└── FileSystem/                   # [MODIFY] 2 个文件 + FileObject/ 3 个文件，补注释
```

## 关键代码结构

无需新增代码；示例注释规范（以 `ServerComponent` 为例）：

```
''' <summary>
''' the base component class which carries the server wide configuration
''' instance for all of the derived http server components.
''' </summary>
Public MustInherit Class ServerComponent
    ''' <summary>
    ''' the shared server configuration instance
    ''' </summary>
    Protected ReadOnly settings As Configuration
    ''' <summary>
    ''' create a new server component with the given configuration
    ''' </summary>
    Sub New(settings As Configuration)
        Me.settings = settings
    End Sub
End Class
```