' ---------------------------------------------------------------------------
' ToolRegistry —— 工具注册表（"框架负责做"的那一半）
'
' readme 里的那句话："模型负责'想'，框架负责'做'，两者以 token 序列为契约。"
' 本类就是"做"的那一半：把工具名映射到真实的 .NET 委托，并在调用之前做三层校验。
'
' 三层校验分别对应 readme 里列举的三类常见失败模式：
'
'   1. 幻觉工具名 —— schema 里不存在的函数名。约束解码保证不了这一点（工具名不在
'      参数对象的约束范围内），必须由框架校验并把错误信息回传给模型自我修正；
'   2. 参数类型/枚举不匹配 —— 纯 prompt 方案下最常见。有了约束解码这类问题基本消失，
'      但工具仍应校验（万一路径上被注入了未经约束的文本）；
'   3. 循环调用 —— 由严格单调的上下文（上一轮的工具结果被回填）自然缓解，
'      再由 <see cref="AgentLoop"/> 的最大轮次上限兜底。
'
' 任何一层校验失败都不抛异常，而是返回一段结构化的错误字符串 —— 因为这段字符串
' 会被当作"工具结果"回填进上下文，让模型有机会自我修正。
' ---------------------------------------------------------------------------

Imports System.Text

Namespace LLM

    ''' <summary>一个已注册的工具：名称 + 说明 + 参数 schema + 真实实现。</summary>
    Public Class ToolDefinition

        Public Property Name As String
        Public Property Description As String
        Public Property Schema As JsonSchema
        ''' <summary>真实实现：接收解析后的参数表，返回结果字符串。</summary>
        Public Property Handler As Func(Of Dictionary(Of String, String), String)

        Public Overrides Function ToString() As String
            Return $"{Name} - {Description}"
        End Function

    End Class

    ''' <summary>工具注册表：承载"外部代码执行解析"职责。</summary>
    Public Class ToolRegistry

        Private ReadOnly _tools As New List(Of ToolDefinition)

        ''' <summary>已注册的工具（按注册顺序）。</summary>
        Public ReadOnly Property Tools As IList(Of ToolDefinition)
            Get
                Return _tools
            End Get
        End Property

        ''' <summary>注册一个工具；同名工具会被拒绝，避免静默覆盖。</summary>
        Public Function Register(name As String, description As String, schema As JsonSchema,
                                 handler As Func(Of Dictionary(Of String, String), String)) As ToolDefinition

            If String.IsNullOrEmpty(name) Then Throw New ArgumentException("工具名不能为空")
            If Find(name) IsNot Nothing Then Throw New ArgumentException($"工具 '{name}' 已经注册过")

            Dim tool As New ToolDefinition With {
                .Name = name,
                .Description = description,
                .Schema = If(schema, JsonSchema.Empty()),
                .Handler = handler
            }

            _tools.Add(tool)

            Return tool
        End Function

        ''' <summary>按名查找工具；不存在时返回 <see langword="Nothing"/>。</summary>
        Public Function Find(name As String) As ToolDefinition
            If name Is Nothing Then Return Nothing

            For Each t In _tools
                If String.Equals(t.Name, name, StringComparison.Ordinal) Then Return t
            Next

            Return Nothing
        End Function

        ''' <summary>工具名列表（供"工具名约束"或提示信息使用）。</summary>
        Public ReadOnly Property Names As String()
            Get
                Return _tools.Select(Function(t) t.Name).ToArray()
            End Get
        End Property

        ''' <summary>
        ''' 渲染注入 prompt 的工具清单（readme 里"把 Schema 变成 token"的第一步）。
        ''' </summary>
        Public Function RenderToolCatalog() As String
            Dim text As New StringBuilder()

            Call text.AppendLine("Available tools:")

            For Each t In _tools
                Call text.AppendLine($"- {t.Name}: {t.Description}")
                Call text.Append(t.Schema.RenderPrompt())
            Next

            Call text.AppendLine()
            Call text.AppendLine("To call a tool, output exactly:")
            Call text.AppendLine("  " & ToolCallProtocol.CallsBeginMarker & ToolCallProtocol.CallBeginMarker &
                                 ToolCallProtocol.CallTypeFunction & ToolCallProtocol.SepMarker & "{tool_name}")
            Call text.AppendLine("  ```json")
            Call text.AppendLine("  {""argument"": value, ...}")
            Call text.AppendLine("  ```")
            Call text.AppendLine("  " & ToolCallProtocol.CallEndMarker & ToolCallProtocol.CallsEndMarker)
            Call text.AppendLine("Otherwise answer the user directly in plain text.")

            Return text.ToString()
        End Function

        ''' <summary>
        ''' 渲染<b>紧凑</b>工具清单：只保留"签名 + 枚举取值 + 调用格式"。
        ''' </summary>
        ''' <remarks>
        ''' 与 <see cref="RenderToolCatalog"/> 的关系：后者是给人看的完整说明书（含逐参数的
        ''' 自然语言描述），前者是给模型看的、尽量省 token 的版本。小模型的上下文窗口很窄，
        ''' 工具清单会占掉相当一部分预算，因此训练与推理都统一使用紧凑版，保证分布一致。
        ''' </remarks>
        Public Function RenderCompactCatalog() As String
            Dim text As New StringBuilder()

            Call text.AppendLine("tools:")

            For Each t In _tools
                Dim args = String.Join(", ", t.Schema.Properties.Select(Function(p) p.Name))

                Call text.AppendLine($"- {t.Name}({args}): {t.Description}")

                For Each p In t.Schema.Properties
                    If p.EnumValues IsNot Nothing AndAlso p.EnumValues.Length > 0 Then
                        Call text.AppendLine($"    {p.Name} one of: {String.Join(" | ", p.EnumValues)}")
                    End If
                Next
            Next

            Call text.AppendLine("to call a tool output:")
            Call text.AppendLine(ToolCallProtocol.CallsBeginMarker & ToolCallProtocol.CallBeginMarker &
                                 ToolCallProtocol.CallTypeFunction & ToolCallProtocol.SepMarker & "NAME")
            Call text.AppendLine("```json")
            Call text.AppendLine("{arguments}")
            Call text.AppendLine("```")
            Call text.AppendLine(ToolCallProtocol.CallEndMarker & ToolCallProtocol.CallsEndMarker)

            Return text.ToString()
        End Function

        ''' <summary>
        ''' 执行一次工具调用。
        ''' </summary>
        ''' <param name="toolCall">解析出来的调用</param>
        ''' <returns>工具的真实返回值，或者一段"为什么没能调用"的结构化错误说明</returns>
        Public Function Invoke(toolCall As ToolCall) As String
            If toolCall Is Nothing Then Return ErrorOf("the tool call is empty")

            Dim tool = Find(toolCall.Name)

            If tool Is Nothing Then
                ' 失败模式 1：幻觉工具名。把可用清单回传，让模型自我修正。
                Return ErrorOf($"tool '{toolCall.Name}' does not exist; available tools: {String.Join(", ", Names)}")
            End If

            Dim args = If(toolCall.Arguments, New Dictionary(Of String, String)())

            ' 失败模式 2：参数缺失 / 枚举越界
            For Each p In tool.Schema.Properties
                Dim has As Boolean = args.ContainsKey(p.Name)

                If p.Required AndAlso Not has Then
                    Return ErrorOf($"tool '{tool.Name}' requires argument '{p.Name}'")
                End If

                If Not has Then Continue For

                If p.EnumValues IsNot Nothing AndAlso p.EnumValues.Length > 0 Then
                    If Array.IndexOf(p.EnumValues, args(p.Name)) < 0 Then
                        Return ErrorOf($"argument '{p.Name}' must be one of: {String.Join(" | ", p.EnumValues)}")
                    End If
                End If
            Next

            Try
                Return tool.Handler(args)
            Catch ex As Exception
                Return ErrorOf($"tool '{tool.Name}' failed: {ex.Message}")
            End Try
        End Function

        ''' <summary>
        ''' 把失败原因包装成一段 JSON 错误对象。
        ''' </summary>
        ''' <remarks>
        ''' 用 JSON 而不是自然语言，是因为这段文本会被当作"工具结果"回填进上下文，
        ''' 保持与成功结果同构可以让模型更容易学到"出错时该如何纠正"。
        ''' </remarks>
        Private Shared Function ErrorOf(message As String) As String
            Dim q = Chr(34)

            Return "{" & q & "error" & q & ": " & q & message.Replace(q, "'") & q & "}"
        End Function

    End Class

End Namespace
