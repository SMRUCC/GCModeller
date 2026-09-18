Namespace Script

    ''' <summary>
    ''' 向量化改写报告: 供 <c>--verbose</c> 模式输出改写规模与未能改写的可疑行。
    ''' </summary>
    Public Class VectorizationReport

        ''' <summary>发生改写的语句行数</summary>
        Public Property Rewritten As Integer

        ''' <summary>
        ''' 疑似应当改写但未能改写的行(例如跨物理行的表达式续行、语法不完整的行)。
        ''' 只有在这些行确实引用了已知向量变量时才会被记录。
        ''' </summary>
        Public ReadOnly Property Skipped As New List(Of String)

        ''' <summary>本次改写所识别到的向量变量名</summary>
        Public ReadOnly Property Vectors As New List(Of String)
    End Class

    ''' <summary>
    ''' 脚本向量化预处理阶段。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 本模块是 <see cref="ScriptRefactor.PreprocessText"/> 的一个阶段, 位置在
    ''' <c>let</c> 展开与元组分解之后。它把脚本之中「数值向量参与算术运算」的标量写法
    ''' 改写为等价的运行时 SIMD 调用, 从而让脚本作者不必手写 <c>For</c> 循环。
    ''' </para>
    ''' <para>
    ''' <b>为什么放在预处理阶段</b>: <see cref="ScriptParseResult.PreprocessedCode"/>
    ''' 是运行期(<see cref="ScriptRefactor"/>)与工程期(<see cref="ProjectCodeBuilder"/>)
    ''' 两条发射路径共用的中间产物, 因此一处接入即可让 <c>vbs run.vb</c> 与
    ''' <c>vbs make-project</c> 自动保持一致。
    ''' </para>
    ''' <para>
    ''' <b>开关</b>: 默认自动生效; 脚本头部写 <c>#no-vectorize</c> 可以按脚本关闭,
    ''' 命令行 <c>--no-vectorize</c> 可以按次关闭。两条指令行本身会被剔除, 不会进入生成代码;
    ''' <c>#vectorize</c> / <c>#vectorize off</c> 的形式亦可识别。
    ''' </para>
    ''' </remarks>
    Public Module Vectorization

        ''' <summary>关闭向量化的头部指令</summary>
        Public Const NoVectorizeDirective As String = "#no-vectorize"

        ''' <summary>重新打开向量化的头部指令</summary>
        Public Const VectorizeDirective As String = "#vectorize"

        ''' <summary>一行源码所携带的向量化开关指令</summary>
        Private Enum Directive
            None
            Enable
            Disable
        End Enum

        ''' <summary>
        ''' 对脚本代码做向量化改写。
        ''' </summary>
        ''' <param name="source">
        ''' 已经过 <see cref="ScriptRefactor.PreprocessText"/> 之中其它阶段处理的脚本代码
        ''' (即已完成 <c>#include</c> 剔除、<c>?参数</c> 展开、<c>let</c> 展开与元组分解展开)
        ''' </param>
        ''' <param name="enabled">
        ''' 命令行给出的默认开关; 脚本头部的 <c>#no-vectorize</c> / <c>#vectorize</c> 指令优先
        ''' </param>
        ''' <param name="report">可选的改写报告</param>
        Public Function Expand(source As String,
                               Optional enabled As Boolean = True,
                               Optional report As VectorizationReport = Nothing) As String

            If String.IsNullOrEmpty(source) Then
                Return source
            End If

            Dim lines As New List(Of String)
            Dim effective As Boolean = enabled

            ' ---- 1. 解析并剔除开关指令 ----
            For Each raw As String In source.LineTokens
                Select Case DirectiveOf(raw)
                    Case Directive.Disable
                        effective = False
                        Continue For
                    Case Directive.Enable
                        effective = True
                        Continue For
                    Case Else
                        Call lines.Add(raw)
                End Select
            Next

            If Not effective Then
                Return String.Join(vbCrLf, lines)
            End If

            ' ---- 2. 逐行改写 ----
            Dim rewriter As New VectorExpressionRewriter()
            Dim output As New List(Of String)

            For Each raw As String In lines
                Dim rewritten As String = rewriter.RewriteLine(raw, report)

                If Not String.Equals(rewritten, raw, StringComparison.Ordinal) Then
                    If report IsNot Nothing Then
                        report.Rewritten += 1
                    End If
                End If

                Call output.Add(rewritten)
            Next

            If report IsNot Nothing Then
                Call report.Vectors.AddRange(rewriter.VectorNames)
            End If

            Return String.Join(vbCrLf, output)
        End Function

        ''' <summary>解析一行是否为向量化开关指令</summary>
        Private Function DirectiveOf(line As String) As Directive
            Dim text As String = line.Trim()

            If Not text.StartsWith("#") Then
                Return Directive.None
            End If

            text = text.Substring(1).Trim().Replace(":", " ").ToLower()

            Select Case CollapseSpace(text)
                Case "no-vectorize", "vectorize off"
                    Return Directive.Disable
                Case "vectorize", "vectorize on"
                    Return Directive.Enable
                Case Else
                    Return Directive.None
            End Select
        End Function

        Private Function CollapseSpace(text As String) As String
            Return System.Text.RegularExpressions.Regex.Replace(text, "\s+", " ").Trim()
        End Function
    End Module
End Namespace
