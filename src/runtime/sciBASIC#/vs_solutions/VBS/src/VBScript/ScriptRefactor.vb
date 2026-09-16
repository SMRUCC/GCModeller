Imports System.Collections.Generic
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine

Namespace Script

    ''' <summary>
    ''' 把脚本源代码重构为可以直接在内存中编译的完整 VB.NET 源代码。
    ''' </summary>
    ''' <remarks>
    ''' 一次重构过程对应一个对象实例。重构被拆分为两个阶段:
    ''' <list type="number">
    ''' <item><b>扫描</b>: 由 <see cref="ScriptStructure.Scan"/> 完成, 得到与发射方式无关的
    ''' 静态结构(头部语句 / 类型定义块 / 顶层函数块 / 顶层语句槽位);</item>
    ''' <item><b>发射</b>: 由本类型的 <c>BuildCode</c> 把结构组装为固定的
    ''' <c>Namespace + Module + Main</c> 容器代码(顶层函数重写为匿名函数并按依赖落位)。</item>
    ''' </list>
    ''' 工程代码发射由 <c>ProjectCodeBuilder</c> 复用同一份 <see cref="ScriptStructure"/> 完成。
    ''' </remarks>
    Public Class ScriptRefactor

        ReadOnly _syntax As ScriptStructure
        ReadOnly magics As New List(Of String)
        ReadOnly _metadata As ScriptMetadata

        ''' <summary>被 #include 引入的其它脚本所贡献的头部 Imports</summary>
        ReadOnly _includeHeaders As New List(Of String)

        ''' <summary>被 #include 引入的其它脚本所贡献的类型定义块</summary>
        ReadOnly _includeTypes As New List(Of String)

        ''' <summary>创建一个脚本重构器</summary>
        ''' <param name="metadata">脚本头部指令解析得到的程序集元数据(可以为Nothing)</param>
        ''' <param name="magics">需要注入到 VBScriptHostMagics 模块的魔法方法源码</param>
        ''' <param name="includes">被 #include 引入的其它脚本所贡献的代码(可以为Nothing)</param>
        Sub New(metadata As ScriptMetadata, magics As IEnumerable(Of String), Optional includes As IncludeSet = Nothing)
            Me._metadata = metadata
            Me._syntax = New ScriptStructure()
            Call Me.magics.AddRange(magics)

            If includes IsNot Nothing Then
                Call _includeHeaders.AddRange(includes.HeaderImports)
                Call _includeTypes.AddRange(includes.TypeBlocks)
            End If
        End Sub

        ' ==================================================================
        ' 阶段1: 文本级预处理
        ' ==================================================================

        ''' <summary>
        ''' 移除 #include 元数据行, 展开命令行参数语法、let 声明与元组分解语法。
        ''' 预处理是纯文本变换, 因此可以被运行期与工程期两条发射路径共用。
        ''' </summary>
        Public Shared Function PreprocessText(source As String) As String
            Dim code As String = Regex.Replace(source, "^\s*#include\s+""[^""]*""\s*$", "", RegexOptions.IgnoreCase Or RegexOptions.Multiline)

            ' ?"--a" => args("--a")
            code = Regex.Replace(code, "\?""(?<name>[^""]+)""", "args(""${name}"")")
            ' let x = ... => Dim x As Object = ... (不会改写 LINQ 查询之中的 Let 子句)
            code = LetStatement.Expand(code)
            code = TupleDestructuring.Expand(code)

            Return code
        End Function

        ' ==================================================================
        ' 阶段2: 扫描 + 发射
        ' ==================================================================

        ''' <summary>
        ''' 对脚本源代码进行重构, 生成运行期可直接编译的完整代码。
        ''' </summary>
        Public Function Refactor(source As String) As String
            Return RefactorPreprocessed(PreprocessText(source))
        End Function

        ''' <summary>
        ''' 对<b>已经过 <see cref="PreprocessText"/> 处理</b>的脚本代码进行重构。
        ''' </summary>
        Public Function RefactorPreprocessed(code As String) As String
            Dim syntax As ScriptStructure = ScriptStructure.Scan(code)

            Call syntax.ResolveFunctionSlots()

            Return BuildCode(syntax)
        End Function

        ''' <summary>组装为 固定Namespace + Module + Main 的完整可编译代码</summary>
        Private Function BuildCode(syntax As ScriptStructure) As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine("Option Strict Off")
            Call sb.AppendLine("Option Explicit On")
            Call sb.AppendLine("Option Infer On")
            Call sb.AppendLine()

            Dim headers As String() = MergeHeaders(syntax)

            For Each header As String In headers
                Call sb.AppendLine(header)
            Next

            If headers.Length > 0 Then
                Call sb.AppendLine()
            End If

            Call sb.AppendLine($"Imports {GetType(CommandLine).Namespace}")
            Call sb.AppendLine($"Imports Microsoft.VisualBasic")
            Call sb.AppendLine($"Imports System.Linq")
            Call sb.AppendLine($"Imports System")
            Call sb.AppendLine($"Imports System.Collections")
            Call sb.AppendLine($"Imports System.Collections.Generic")
            Call sb.AppendLine($"Imports System.Data")
            Call sb.AppendLine($"Imports System.Diagnostics")
            Call sb.AppendLine($"Imports System.Threading.Tasks")
            Call sb.AppendLine($"Imports System.Xml.Linq")

            Call AppendAssemblyAttributes(sb)

            Call sb.AppendLine($"Namespace {NamespaceName}")

            Call sb.AppendLine("     Module VBScriptHostMagics")

            For Each magic As String In magics
                Call sb.AppendLine(magic)
            Next

            Call sb.AppendLine("     End Module")


            Call sb.AppendLine($"    Module {ModuleName}")
            Call sb.AppendLine()
            Call sb.AppendLine($"        Public Function {MainName}(args As CommandLine) As Integer")

            ' 槽位 -1: 不依赖任何顶层变量与其它顶层函数的匿名函数, 放在最前面
            Call AppendFunctions(sb, syntax, -1)

            For i As Integer = 0 To syntax.Slots.Count - 1
                For Each stmt As String In syntax.Slots(i).Statements
                    Call sb.AppendLine("            " & stmt)
                Next

                ' 挂在语句之后的匿名函数: 它捕获的变量到这里已经声明完毕
                Call AppendFunctions(sb, syntax, i)
            Next

            Call sb.AppendLine()
            Call sb.AppendLine("            Return 0")
            Call sb.AppendLine("        End Function")
            Call sb.AppendLine("    End Module")

            ' 类型定义块直接作为顶层命名空间的成员。
            ' 注意: 不能嵌套在 Module 之内 —— VB 不允许在 Module 内部再次声明 Module
            ' (BC30617), 而被 #include 引入的脚本常常会贡献 Module 定义。
            For Each typeBlock As String In MergeTypeBlocks(syntax)
                Call sb.AppendLine()

                Dim lines As String() = typeBlock.Split(vbLf)

                For i As Integer = 0 To lines.Length - 1
                    If i = 0 Then
                        Call sb.AppendLine("    " & NormalizeTypeAccess(lines(i)))
                    Else
                        Call sb.AppendLine("    " & lines(i))
                    End If
                Next
            Next

            Call sb.AppendLine("End Namespace")

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 顶层类型只允许 <c>Friend</c>/<c>Public</c>, 因此把类型声明行上的
        ''' <c>Private</c> 规范化为 <c>Friend</c>(仅作用于类型声明行, 不影响类型成员)。
        ''' </summary>
        Private Shared Function NormalizeTypeAccess(declaration As String) As String
            Return Regex.Replace(declaration, "^\s*Private\s+", "Friend ", RegexOptions.IgnoreCase)
        End Function

        ''' <summary>
        ''' 合并"脚本自身"与"被 #include 引入的脚本"所贡献的头部 Imports/Option 语句(去重, 保持顺序)。
        ''' </summary>
        Private Function MergeHeaders(syntax As ScriptStructure) As String()
            Dim list As New List(Of String)
            Dim added As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each header As String In _includeHeaders
                If added.Add(header.Trim()) Then
                    Call list.Add(header)
                End If
            Next

            For Each header As String In syntax.Headers
                If added.Add(header.Trim()) Then
                    Call list.Add(header)
                End If
            Next

            Return list.ToArray()
        End Function

        ''' <summary>
        ''' 合并"脚本自身"与"被 #include 引入的脚本"所贡献的类型定义块。
        ''' 被引入脚本的类型定义块被直接复制到主脚本的顶层命名空间之中。
        ''' </summary>
        Private Function MergeTypeBlocks(syntax As ScriptStructure) As String()
            Dim list As New List(Of String)

            Call list.AddRange(syntax.TypeBlocks)
            Call list.AddRange(_includeTypes)

            Return list.ToArray()
        End Function

        ''' <summary>
        ''' 把脚本头部指令声明的 assembly 级特性输出到生成代码之中。
        ''' 按照 VB 语法要求, assembly 特性必须位于 Imports 之后、Namespace 之前。
        ''' </summary>
        Private Sub AppendAssemblyAttributes(sb As StringBuilder)
            If _metadata Is Nothing Then
                Return
            End If

            Dim attrs As String() = _metadata.BuildAttributes()

            If attrs.Length = 0 Then
                Return
            End If

            Call sb.AppendLine()

            For Each attr As String In attrs
                Call sb.AppendLine(attr)
            Next

            Call sb.AppendLine()
        End Sub

        ''' <summary>把落在指定槽位上的全部顶层函数块输出到Main之中</summary>
        Private Sub AppendFunctions(sb As StringBuilder, syntax As ScriptStructure, slot As Integer)
            For Each func As ScriptFunctionBlock In syntax.Functions
                If func.Slot <> slot Then
                    Continue For
                End If

                For Each line As String In func.Lines
                    Call sb.AppendLine("            " & line)
                Next

                Call sb.AppendLine()
            Next
        End Sub
    End Class
End Namespace
