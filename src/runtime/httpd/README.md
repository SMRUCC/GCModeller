# httpd — Flute HTTP Server

`httpd` 是 SMRUCC/GCModeller 的 Web 运行时，包含两个部分：

| 项目 | 类型 | 说明 |
|------|------|------|
| `src\Flute\Flute.NET5.vbproj` | 类库（程序集 `Flute.Http`） | 一个轻量级的纯托管 HTTP 服务器实现：TCP 监听、HTTP/1.1 解析、路由、静态文件系统、WebSocket(RFC6455)、HTTP 长轮询、文件上传、会话与配置 |
| `src\HTTP_SERVER\Fluteway.vbproj` | 命令行程序（`Fluteway.exe`） | 基于 `Flute.Http` 的 CLI 宿主：静态站点托管（`--listen`）、动态加载 Web App 模块（`/run`）、Apache 日志解析（`/parse_apache`） |

两个项目均为 **Visual Basic .NET / `net10.0`**，依赖 [sciBASIC#](https://github.com/xieguigang/sciBASIC) 的 `Microsoft.VisualBasic.Core`。

```bash
# 构建（同时产出 Flute.Http.dll 与 Fluteway.exe）
dotnet build src\HttpCore.sln -c Release

# 也可以构建完整仓库（额外包含模板编译、Sitemap、R# 互操作等子项目）
dotnet build src\WWWFlute.sln -c Release
```

产物路径：`src\HTTP_SERVER\bin\Release\net10.0\Fluteway.exe`（Windows）或 `Fluteway.dll`（Linux/macOS，`dotnet Fluteway.dll ...`）。

---

## 一、`Flute.NET5.vbproj` → 程序集 `Flute.Http`

### 1.1 项目概览

```xml
<RootNamespace>Flute.Http</RootNamespace>
<AssemblyName>Flute.Http</AssemblyName>
<TargetFrameworks>net10.0</TargetFrameworks>
<Platforms>AnyCPU;ARM32;x64;ARM64</Platforms>
<GeneratePackageOnBuild>True</GeneratePackageOnBuild>   <!-- 输出到 ..\..\.nuget\ -->
```

只依赖 `sciBASIC#\Microsoft.VisualBasic.Core\src\Core.vbproj`，不依赖 ASP.NET / Kestrel / IIS —— 整个 HTTP 协议栈（请求行解析、Header 解析、分块/表单/多部分体解析、WebSocket 帧编解码）都是自己实现的，因此它可以被直接内嵌到任意 .NET 桌面程序、R# 脚本宿主或 GCModeller 服务中，无需端口预留或管理员权限配置。

### 1.2 目录与模块划分

```
src\Flute\
├─ Http\                         服务器核心
│  ├─ HttpServer.vb              抽象服务器基类：TcpListener + 线程池/信号量 + 生命周期
│  ├─ HttpSocket.vb              具体服务器：把请求派发给 IAppHandler / AppHandler 委托
│  ├─ HttpProcessor.vb           单连接处理器：解析请求行/Header、WS 升级、长轮询、CORS 预检、POST 缓冲
│  ├─ Core\
│  │  ├─ HttpRouter.vb           反射式路由表（特性路由 + {name} 模板路由 + 手动注册）
│  │  └─ IHttpAppModule.vb       动态 Web App 模块契约（供 Fluteway /run 使用）
│  ├─ HttpStream\                PostReader / HttpMultipart / HttpPostedFile / StreamElement
│  ├─ WebSocket\                 WebSocketManager、Connection、Frame(RFC6455)、独立 WebSocketServer
│  ├─ LongPoll\                  LongPollManager、Connection、委托与消息类型
│  └─ Options\Preflight.vb       CORS 预检（OPTIONS）处理
├─ HttpMessage\                  报文模型
│  ├─ HttpRequest.vb / HttpPOSTRequest.vb / HttpResponse.vb / Content.vb / JsonResponse.vb
│  └─ Protocol\                  HttpHeader（请求/响应头常量 + WS 常量）、Cookies、HttpError、HttpMethods（路由特性）、WebForm
├─ FileSystem\                   虚拟文件系统
│  ├─ FileSystem.vb              物理目录 + 内存缓存/映射文件的统一视图
│  ├─ WebFileSystem.vb           WebFileSystemListener：静态资源托管（含目录穿越防护、大文件流式传输）
│  └─ FileObject\                FileObject / MemoryCachedFile / VirtualMappedFile
├─ Configuration\                Configuration.vb（服务器配置）、Session.vb（会话配置）
├─ HttpDriver.vb                 链式构造器：按 HTTP 方法注册处理器，快速拼装一个 HttpSocket
├─ ServerComponent.vb            携带 Configuration 的组件基类
├─ SessionManager.vb             会话（cookie `flute_session` + 可覆盖的存储）
├─ HttpLogEntry.vb               Apache Combined Log Format 解析
└─ Extensions.vb                 FaviconZip / TransferBinary / SuccessMsg / FailureMsg
```

### 1.3 核心类型速查

| 类型 | 命名空间 | 作用 |
|------|----------|------|
| `HttpServer` | `Flute.Http.Core` | 抽象基类。`Run()` 阻塞监听，`Shutdown()` 优雅停机，`WebSocket` / `LongPoll` 两个连接管理器 |
| `HttpSocket` | `Flute.Http.Core` | 常用实现。构造时传入 `AppHandler` 委托或 `IAppHandler`，分别处理 GET / POST / 其他方法 |
| `HttpRouter` | `Flute.Http.Core` | `IAppHandler` 实现：先查静态文件系统，再走 CLR 路由；未命中返回 404 |
| `HttpProcessor` | `Flute.Http.Core` | 每连接一个实例：解析请求 → 判定 WebSocket 升级 / 长轮询 → 分发 GET/POST/PUT/PATCH/OPTIONS/其他 |
| `HttpRequest` | `Flute.Http.Core.Message` | `HTTPMethod` / `URL` / `HttpHeaders` / `Remote` / `RouteData` / `Argument(name)` / `GetCookies()` |
| `HttpPOSTRequest` | `Flute.Http.Core.Message` | 继承 `HttpRequest`，额外暴露 `POSTData`（`Form`、`Objects`、`files`） |
| `HttpResponse` | `Flute.Http.Core.Message` | `WriteJSON` / `WriteHTML` / `WriteHeader` / `SendFile` / `SendData` / `Redirect` / `WriteError` / `SetCookies` / `AddCustomHttpHeader` |
| `PostReader` | `Flute.Http.Core.HttpStream` | 解析 `application/x-www-form-urlencoded`、`application/json`、`multipart/form-data` |
| `HttpPostedFile` | `Flute.Http.Core.HttpStream` | 上传文件（先落临时文件），`SaveAs(path)` 落盘 |
| `WebFileSystemListener` | `Flute.Http.FileSystem` | 静态资源托管；`MountFs` 后可被 `HttpRouter` 复用 |
| `Configuration` | `Flute.Http.Configurations` | 服务器级配置，可 `Load/Save` 为 ini |
| `SessionManager` | （根命名空间） | 会话读写，默认内存存储，可继承覆盖 |

### 1.4 一次请求的生命周期

```
TcpListener.AcceptTcpClient
      │  受 SemaphoreSlim 限流（默认 = CPU 核心数）
      ▼
HttpProcessor.Process()
      ├─ parseRequest() + readHeaders()          ← 请求行 / Header（大小写不敏感字典）
      ├─ isWebSocketRequest()   → handleWebSocketUpgrade()   （RFC6455 握手，仅当路由表已注册该路径）
      ├─ isLongPollRequest()    → handleLongPoll()           （挂起等待 push，仅当路由表已注册该路径）
      └─ 按方法分发
            GET                     → srv.handleGETRequest()
            POST                    → srv.handlePOSTRequest(body)
            PUT / PATCH（带正文）   → 同样以 HttpPOSTRequest 派发
            OPTIONS + CORS 预检     → Preflight.HandlePreflightRequest()（204）
            其它                    → srv.handleOtherMethod()（含 OPTIONS /ctrl/kill 远程停机）
                  │
                  ▼
        HttpSocket 构造 HttpRequest / HttpPOSTRequest + HttpResponse
                  ▼
        IAppHandler.AppHandler(request, response)     ← HttpRouter 或你自己的委托
```

要点：

- **POST 正文先落临时文件**（上限 `Configuration.max_post_size`，默认 16 MB），再由 `PostReader` 按 `Content-Type` 解析，避免大 body 常驻内存。
- **响应头自动补齐** `Content-Type` / `Connection`（遵循客户端 `Connection: close`）/ `Date` / `Server` / `X-Powered-By`，并可叠加自定义头。
- 静态文件：`WebFileSystemListener` 以 1 MB 为阈值，小文件整包发送、大文件流式拷贝。
- **目录穿越防护**：路径中含 `..`、Windows 盘符或 UNC 前缀时直接返回 403。
- 请求以 `/` 结尾时自动补 `index.html`；路径会做 `UrlDecode`。

### 1.5 路由与控制器的契约

路由特性定义在 `Flute.Http.Core.Message.HttpHeader`（`HttpMethods.vb`）：

```vb
<HttpGet("/api/hello")>
<HttpPost("/api/submit")>
<HttpPut("/api/item/{id}")>
<HttpDelete("/api/item/{id}")>
```

`HttpRouter.RegisterController(instance)` 的扫描规则：

1. 只扫描 **Public 实例方法**；
2. 方法必须被上述四个特性之一标注；
3. 签名必须严格是 `Sub(request As HttpRequest, response As HttpResponse)`（第一个参数可为 `HttpPOSTRequest` 等派生类，第二个必须可赋值给 `HttpResponse`）。**签名不符的方法会被跳过并输出 warning，不会被注册。**
4. URL 中的 `{name}` 片段会被编译成正则，注册为**模板路由**；否则注册为**精确路由**。匹配时 URL 两端斜杠被裁剪，比较大小写不敏感。
5. 命中模板路由时，捕获值通过 `request.RouteData("name")` 读取（已 `UrlDecode`）；精确路由时 `RouteData` 为 `Nothing`。

除了特性路由，也可以手动注册委托（`HttpSocket.AppHandler`）：

```vb
router.Register("GET", "/api/health", Sub(req As HttpRequest, rep As HttpResponse)
                                          Call rep.WriteJSON(New With {.ok = True})
                                      End Sub)
```

处理器抛出的异常会被路由器捕获，返回 `500` 并把真实异常写入日志。

### 1.6 WebSocket（RFC6455）

```vb
Dim server As New HttpSocket(AddressOf HttpHandler, port:=8080, configs:=settings)

' 内置回声处理器，或自定义 IWebSocketHandler / 事件委托
Call server.WebSocket.Route("/ws/echo", WebSocketHandler.Echo)
Call server.WebSocket.Route("/ws/chat",
    Sub(conn As WebSocketConnection, msg As WebSocketMessage)
        Call conn.SendText(msg.Text)
    End Sub)

' 广播
Call server.WebSocket.Broadcast("/ws/chat", "hello everyone")
```

- 只有在路由表中注册过的路径才会接受 `Upgrade: websocket` 握手，否则按普通 HTTP 请求处理（便于返回 404）。
- 支持文本/二进制、分片重组、Ping/Pong、Close 握手、子协议协商（`websocket_subprotocols` 配置，逗号分隔）。
- 只想跑纯 WebSocket 服务时用 `WebSocketServer`（`HttpSocket` 的薄封装，非 WS 请求返回 400）。

### 1.7 HTTP 长轮询（Long Polling）

```vb
Call server.LongPoll.Route("/poll/messages")     ' 注册后该路径的 GET 会被挂起
' ... 任意线程推送：
Dim n As Integer = server.LongPoll.PushText("/poll/messages", "hello")   ' 返回送达客户端数
```

`LongPollManager` 是线程安全的：内部用 `ConcurrentDictionary` 维护待处理连接，支持 `Push/PushText/PushJSON/PushBinary`（按路径）与 `Broadcast*`（全站）、`GetPendingCount(path)`、`CloseAll()`（停机时唤醒所有挂起线程）。可通过 `Configuration.longpoll_timeout`（默认 30 s）与 `longpoll_max_connections`（默认 1000）控制。

### 1.8 会话与配置

```vb
Dim session As New SessionManager(request.GetCookies(), settings)
If session.SetCookie Then response.SetCookies(SessionManager.CookieName, session.Id)
session.SaveSession("user", "alice")
```

`SessionManager` 默认把值放在进程内存（`ConcurrentDictionary`），可覆写 `GetSession` / `SaveSession` 接入持久化存储（仓库中 `src\SessionManager\Flute.SessionManager.vbproj` 即文件存储实现）。

`Configuration` 关键字段（`[configuration]` 段，可 ini 序列化）：

| 字段 | 默认值 | 说明 |
|------|--------|------|
| `x_powered_by` | `HttpProcessor.VBS_platform` | `X-Powered-By` 响应头 |
| `silent` | `True` | 关闭调试日志 |
| `request_timeout` | `30000` | 请求读取超时（ms） |
| `shutdown_token` | `""` | 远程停机令牌；为空则禁用 `/ctrl/kill` |
| `cors_allow_origin` / `_methods` / `_headers` | `*` / `POST, GET, OPTIONS` / `X-PINGOTHER, Content-Type` | CORS |
| `websocket_enabled` / `websocket_subprotocols` / `websocket_max_message_size` | `True` / `""` / 16 MB | WebSocket |
| `longpoll_enabled` / `longpoll_timeout` / `longpoll_max_connections` | `True` / 30000 / 1000 | 长轮询 |
| `max_post_size` | 16 MB | 单个请求体上限 |
| `session` | `New Session` | 会话子配置（`session_id_prefix`、`session_store`、`session_enable`） |

### 1.9 内嵌到自己的程序

```vb
Imports Flute.Http.Core
Imports Flute.Http.Core.Message

Dim app As New HttpSocket(
    app:=Sub(req As HttpRequest, rep As HttpResponse)
             Call rep.WriteHTML("<h1>hello flute</h1>")
         End Sub,
    port:=8080,
    configs:=Configuration.Default()
)

Call app.Run()      ' 阻塞当前线程
```

或用 `HttpDriver` 按方法注册（未注册的方法自动 501）：

```vb
Dim server As New HttpDriver(Configuration.Default()) _
    .AddResponseHeader("X-App", "demo") _
    .HttpMethod("GET", AddressOf OnGet) _
    .HttpMethod("POST", AddressOf OnPost) _
    .GetSocket(8080)
```

---

## 二、`Fluteway.vbproj` → `Fluteway.exe` 命令行工具

```xml
<OutputType>Exe</OutputType>
<AssemblyName>Fluteway</AssemblyName>
<RootNamespace>Fluteway</RootNamespace>
<TargetFramework>net10.0</TargetFramework>
```

入口 `src\HTTP_SERVER\Program.vb`，采用 sciBASIC 的 `RunCLI` 框架：

```vb
Public Function Main() As Integer
    Return GetType(Program).RunCLI(App.CommandLine, executeEmpty:=AddressOf listenCurrentFolder)
End Function
```

> **不带任何参数运行** `Fluteway` 等价于 `Fluteway --listen`，以**当前工作目录**为 `wwwroot`、端口 `80` 启动静态服务器。

### 2.1 命令总览

| 命令 | 说明 |
|------|------|
| `--listen` | 启动本地静态 Web 服务器 |
| `/run` | 反射加载一个 Web App 程序集（控制器类库）并启动服务器 |
| `/parse_apache` | 解析 Apache access/error 日志为 CSV |

通用帮助：`Fluteway ?`、`Fluteway ??<命令名>`、`Fluteway /i` 进入交互模式、`Fluteway /CLI.dev` 生成 CLI 管道开发脚手架。

### 2.2 `--listen`：静态站点托管

```bash
Fluteway --listen [/wwwroot <directory_path> --attach <other_directory_path> --parent <parent_process_id> /port <http_port, default=80>]
```

| 参数 | 默认 | 说明 |
|------|------|------|
| `/wwwroot` | 当前工作目录 | 站点根目录 |
| `/port` | `80` | 监听端口 |
| `--attach` | 无 | 附加目录；若路径是目录则递归挂载其中的文件（当前仅目录分支生效） |
| `--parent` | 无 | 父进程 ID（当前实现中绑定逻辑被注释，参数保留但尚未生效） |

行为补充：

- 启动前先做端口可用性检查，被占用则返回 `500` 并提示 `local tcp port(=N) is in used!`。
- 该命令内置一个**长轮询演示端点**：`GET /poll/messages`（挂起等待推送）与 `POST /push`（取 `message` 表单项或 `?message=` 查询参数，向所有挂起连接推送文本，返回 `{"ok":true,"delivered":n,"message":"..."}`）。
- 其余请求全部交给静态文件处理器。

示例：

```bash
Fluteway --listen /wwwroot "D:/site/public" /port 8080
# → http://localhost:8080/
```

### 2.3 `/run`：加载动态 Web App（重点）

```bash
Fluteway /run --app <app.dll> [--listen <port, default=80> --wwwroot <directory_path> --data <data_directory> --config <config.ini> --max-post-size <bytes> --base-url <http://host>]
```

| 参数 | 默认 | 说明 |
|------|------|------|
| `--app` | **必填** | Web App 程序集（`.dll`）路径，相对路径按当前目录解析 |
| `--listen` | `80` | 监听端口（注意这里是 `--listen <端口号>`，不是静态服务器命令的 `/port`） |
| `--wwwroot` | 当前工作目录 | 静态资源根目录 |
| `--data` | `<cwd>/data` | 数据目录；若存在 `<data>/packages` 会自动挂载为 `/packages/` 静态目录 |
| `--config` | 无 | ini 风格配置文件（`key=value`，`#` / `;` 注释），**优先级低于命令行参数** |
| `--max-post-size` | 16 MB | 单个请求体上限（字节） |
| `--base-url` | 无 | 对外访问基址，仅作为配置项传给 App 模块 |

执行流程：

1. 校验 `--app` 与文件存在性（缺失 → `404`）；
2. 合并配置：先读 `--config` 文件，再用命令行参数覆盖；
3. 端口占用检查（占用 → `500`）；
4. `Assembly.LoadFrom(app.dll)`（失败 → `500` 并记录日志）；
5. 建立 `WebFileSystemListener(wwwroot)` 并 `router.MountFs(...)`；
6. 挂载 `<data>/packages` → `/packages/`（若目录存在）；
7. 遍历程序集中**可加载的类型**，识别控制器 → `Activator.CreateInstance` → `router.RegisterController(...)`；若控制器实现了 `IHttpAppModule`，再调用 `module.Mount(router, configs)`；
8. 一个控制器都没找到 → `404`；
9. 以 `HttpSocket(router, port, configs:=settings)` 启动并阻塞。

**退出码**：`0` 正常退出；`404` 参数缺失 / 文件不存在 / 无控制器；`500` 端口被占用 / 程序集加载失败。

示例：

```bash
Fluteway /run --app "D:/apps/MyWebApp/bin/Release/net10.0/MyWebApp.dll" ^
               --listen 8080 ^
               --wwwroot "D:/apps/MyWebApp/wwwroot" ^
               --data    "D:/apps/MyWebApp/data" ^
               --config  "D:/apps/MyWebApp/app.ini" ^
               --max-post-size 67108864
```

### 2.4 `/parse_apache`：日志转 CSV

```bash
Fluteway /parse_apache --log <apache_access/error.log> [--save <save.csv>]
```

按 **Apache Combined Log Format** 正则解析，产出列为 `RemoteIp, Ident, RemoteUser, LogTime, HttpMethod, RequestUrl, HttpProtocol, StatusCode, ResponseBytes, Referer, UserAgent, RawLog`；未指定 `--save` 时把日志文件同名改后缀为 `.csv`。

### 2.5 作为子进程嵌入（.NET 互操作）

仓库内 `src\HTTP_SERVER\Svr\` 提供了把 `Fluteway.exe` 当作后台服务拉起的封装：

```vb
' Interop.vb
Dim cli As CLI.Fluteway = Interop.CreateServer()            ' 从 App.HOME 定位 Fluteway.exe
Dim args As String = cli.GetlistenCommandLine(wwwroot, port:=8080)

' HttpServices.vb：自动挑选可用端口并后台启动，Dispose 时杀掉子进程
Using http As HttpServices = New HttpServices("D:/site/public").StartHttp()
    Console.WriteLine(http.port)
End Using
```

---

## 三、如何为 `/run` 构建一个 Web App

`/run` 加载的是**普通 .NET 类库（controller class library）**——不需要实现任何特殊接口，只要写出符合约定的控制器类即可；需要更精细控制时才实现 `IHttpAppModule`。

### 3.1 宿主如何识别控制器

`Program.vb` 中的 `isHttpController(type)` 判定：

| 条件 | 结果 |
|------|------|
| 类型非 class / 是抽象类 | 不是控制器 |
| 实现 `IHttpAppModule` | **是**控制器 |
| 任一 Public 实例方法带 `<HttpGet>` / `<HttpPost>` / `<HttpPut>` / `<HttpDelete>` | **是**控制器 |

识别后宿主会 `Activator.CreateInstance(type)`，**因此控制器必须有一个公共的无参构造函数**；构造失败的类型只会被跳过并打印 warning，不会导致启动失败。

方法级要求（见 `HttpRouter.matchSignature`）：

```vb
Public Sub 方法名(request As HttpRequest, response As HttpResponse)
```

- 必须是 `Sub`（返回 `Void`）；
- 必须是 **Public 实例**方法（静态方法不参与扫描）；
- 第一个参数可以是 `HttpRequest` 或其派生类（如 `HttpPOSTRequest`）；
- 第二个参数必须可赋值给 `HttpResponse`。

### 3.2 最小可用工程

**① 项目文件 `MyWebApp.vbproj`**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Library</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>MyWebApp</RootNamespace>
    <AssemblyName>MyWebApp</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <!-- 也可以改为 <PackageReference Include="Flute.Http" ... /> -->
    <ProjectReference Include="..\Flute\Flute.NET5.vbproj" />
  </ItemGroup>

</Project>
```

**② 控制器 `HelloController.vb`**

```vb
Imports Flute.Http.Core.Message
Imports Flute.Http.Core.Message.HttpHeader

Public Class HelloController

    ' GET http://host/api/hello
    <HttpGet("/api/hello")>
    Public Sub Hello(request As HttpRequest, response As HttpResponse)
        Call response.WriteJSON(New With {
            .code = 0,
            .info = "hello flute"
        })
    End Sub

    ' GET http://host/api/user/42  →  RouteData("id") = "42"
    <HttpGet("/api/user/{id}")>
    Public Sub GetUser(request As HttpRequest, response As HttpResponse)
        Dim id As String = request.RouteData("id")

        Call response.WriteJSON(New With {
            .id = id,
            .name = "user-" & id
        })
    End Sub

    ' GET http://host/api/search?q=abc
    <HttpGet("/api/search")>
    Public Sub Search(request As HttpRequest, response As HttpResponse)
        Dim q As String = request("q").DefaultValue

        Call response.WriteJSON(New With {.query = q})
    End Sub

End Class
```

**.vbproj 与 `/run` 使用的目标框架要一致**（这里都是 `net10.0`），否则 `Assembly.LoadFrom` 可能因依赖解析失败而报 `500`。

### 3.3 处理 POST / 表单 / JSON / 文件上传

`HttpSocket` 会把 POST 请求构造为 `HttpPOSTRequest`，把第一个参数声明成它即可：

```vb
Imports Flute.Http.Core.Message
Imports Flute.Http.Core.Message.HttpHeader

Public Class FormController

    ' application/x-www-form-urlencoded 或 application/json
    <HttpPost("/api/login")>
    Public Sub Login(request As HttpPOSTRequest, response As HttpResponse)
        ' 取值顺序：URL 查询串 → 表单字段 → JSON 字段
        Dim user As String = request("user").DefaultValue
        Dim pwd As String = request("password").DefaultValue

        If user = "admin" AndAlso pwd = "123456" Then
            Call response.WriteJSON(New With {.code = 0, .info = "ok"})
        Else
            Call response.WriteJSON(New With {.code = 401, .info = "bad credential"})
        End If
    End Sub

    ' multipart/form-data 上传
    <HttpPost("/api/upload")>
    Public Sub Upload(request As HttpPOSTRequest, response As HttpResponse)
        Dim saved As New List(Of String)

        For Each field In request.POSTData.files
            For Each file In field.Value
                Dim target As String = IO.Path.Combine(App.CurrentDirectory, "data", file.FileName)

                Call file.SaveAs(target)
                Call saved.Add(target)
            Next
        Next

        Call response.WriteJSON(New With {.code = 0, .info = saved})
    End Sub

End Class
```

常用读取方式：

| 数据 | 访问方式 |
|------|----------|
| 查询串 / 表单 / JSON 字段 | `request("name").DefaultValue`（`HttpPOSTRequest` 会依次查 query → Form → Objects） |
| 表单集合 | `request.POSTData.Form`（`NameValueCollection`） |
| JSON 解析结果 | `request.POSTData.Objects`（`Dictionary(Of String, Object)`） |
| 上传文件 | `request.POSTData.files`（`Dictionary(Of String, List(Of HttpPostedFile))`） |
| Cookie | `request.GetCookies()` |
| 客户端 IP | `request.Remote` |
| 请求头 | `request.HttpHeaders`（大小写不敏感） |

需要自定义 JSON 解析行为时，可在自建宿主中给 `HttpSocket` 传入 `PostReader.JSONParser` 委托（`Fluteway /run` 使用默认解析器）。

### 3.4 进阶：实现 `IHttpAppModule`

当你需要在挂载时读取宿主配置、或注册**非特性**的路由（匿名委托、按运行时数据生成路由）时，实现该接口：

```vb
Imports Flute.Http.Core
Imports Flute.Http.Core.Message

Public Class AppModule
    Implements IHttpAppModule

    Private wwwroot As String
    Private dataDir As String

    ' 宿主在 RegisterController 之后调用；config 即 /run 合并出的配置字典
    Public Sub Mount(router As HttpRouter,
                     config As IReadOnlyDictionary(Of String, String)) Implements IHttpAppModule.Mount

        wwwroot = If(config.ContainsKey("wwwroot"), config("wwwroot"), App.CurrentDirectory)
        dataDir = If(config.ContainsKey("data"), config("data"), "data")

        ' 1) 注册本类的特性路由
        Call router.RegisterController(Me)

        ' 2) 注册手写委托路由
        Call router.Register("GET", "/api/health", AddressOf Health)
        Call router.Register("GET", "/pkg/{id}/index.json", AddressOf PackageIndex)

        ' 3) 需要静态资源时，可通过 router.FileSystem 访问宿主挂载的 wwwroot
    End Sub

    <HttpGet("/")>
    Public Sub Index(request As HttpRequest, response As HttpResponse)
        Call response.WriteHTML("<h1>MyWebApp is running</h1>")
    End Sub

    Private Sub Health(request As HttpRequest, response As HttpResponse)
        Call response.WriteJSON(New With {.ok = True, .wwwroot = wwwroot, .data = dataDir})
    End Sub

    Private Sub PackageIndex(request As HttpRequest, response As HttpResponse)
        Call response.WriteJSON(New With {.id = request.RouteData("id")})
    End Sub

End Class
```

`Mount` 的 `config` 字典中可用的键（键名大小写不敏感）：

| 键 | 来源 | 含义 |
|----|------|------|
| `app` | `--app` | 程序集路径 |
| `listen` | `--listen` | 监听端口 |
| `wwwroot` | `--wwwroot` | 静态资源根目录 |
| `data` | `--data` | 数据目录 |
| `base-url` | `--base-url` | 对外基址 |
| `max-post-size` | `--max-post-size` | 请求体上限 |
| `config` | `--config` | 配置文件路径 |

> 注意：`Mount` 目前只暴露 `HttpRouter` 与配置字典。若你的 App 需要 **WebSocket / 长轮询端点**（`server.WebSocket.Route` / `server.LongPoll.Route`），需要自建宿主：直接创建 `HttpSocket` 并注册端点，参考 `test\websocket_test\Program.vb`。

### 3.5 静态资源与目录约定

```
MyWebApp\
├─ wwwroot\                 ← --wwwroot：html/css/js/图片，路由未命中时回退到这里
│  ├─ index.html
│  └─ assets\...
└─ data\                    ← --data
   └─ packages\            ← 若存在，自动以 /packages/ 前缀对外静态发布
```

请求到来时 `HttpRouter.AppHandler` 的判定顺序是：

1. `wfs.CheckResourceFileExists(request)` 命中 → 直接作为静态文件返回；
2. 否则进入 CLR 路由（精确路由 → 模板路由 → 404）。

**因此 wwwroot 下的真实文件会遮蔽同名控制器路由**，规划 URL 时要注意避免冲突。

### 3.6 构建与运行

```bash
# 1) 构建 Web App
cd MyWebApp
dotnet build -c Release
# → bin\Release\net10.0\MyWebApp.dll

# 2) 构建 Fluteway
cd <repo>\src
dotnet build HttpCore.sln -c Release
# → HTTP_SERVER\bin\Release\net10.0\Fluteway.exe

# 3) 运行
Fluteway.exe /run --app "MyWebApp\bin\Release\net10.0\MyWebApp.dll" --listen 8080 --wwwroot "MyWebApp\wwwroot" --data "MyWebApp\data"
```

启动成功后控制台会打印类似：

```
controller mounted: MyWebApp.HelloController
http server started: http://localhost:8080/ (wwwroot=..., data=...)
```

### 3.7 排查清单

| 现象 | 原因与处理 |
|------|-----------|
| `missing required argument: --app <app.dll>`（404） | 未传 `--app` |
| `application module not found: ...`（404） | 路径写错或不是绝对路径；相对路径按**当前工作目录**解析 |
| `failed to load application module: ...`（500） | 目标框架不一致，或依赖（如 `Flute.Http.dll`）不在程序集旁边；把 Web App 的 `bin` 整个目录拷过去 |
| `no http controller found in '...'`（404） | 类不是 `Public`、方法是 `Shared`、方法签名不是 `Sub(HttpRequest, HttpResponse)`，或控制器没有无参构造函数 |
| `skip controller 'Xxx': ...`（warning） | 该类型被识别为控制器但实例化失败，其余控制器仍会正常挂载 |
| 路由一直 404 | 检查 wwwroot 下是否存在同名静态文件（会优先命中）、URL 两端斜杠、HTTP 方法是否匹配 |
| `local tcp port(=N) is in used!`（500） | 换 `--listen` 端口 |

---

## 四、仓库中的其他子项目

| 项目 | 说明 |
|------|------|
| `src\FluteBuild\FluteBuild.vbproj` | 模板编译 CLI：`/compile /view <模板目录> /wwwroot <输出目录> [--listen]`，把 `.vbhtml` 模板与 Markdown 批量渲染为静态 HTML，`--listen` 下监听目录变化自动重建 |
| `src\VBScript\Flute.Template.vbproj` | `VBHtml` 模板引擎（服务端脚本插值），被 FluteBuild 使用 |
| `src\Sitemap\Sitemap.vbproj` | 站点爬虫与分析工具：静态扫描、主题提取、页面评分、sitemap 生成 |
| `src\SessionManager\Flute.SessionManager.vbproj` | `Flute.Http` 会话的持久化（文件）实现 |

## 五、测试

```bash
dotnet run --project test\http_integration_test\HttpIntegrationTest.vbproj   # 启动 Fluteway.exe 做端到端 HTTP 验证
dotnet run --project test\websocket_test\WebSocketTest.vbproj                 # 进程内 RFC6455 协议一致性测试
```

## 六、许可

GNU General Public License v3（GPL3），详见 [LICENSE](./LICENSE)。
