' /********************************************************************************/
'
'  Rockhopper 细菌 RNA-seq 分析工具 —— 运行期共享状态与日志输出
'
'  该模块用于替代原始 Java 移植代码中对全局 `Output(...)` / `verbose` 的隐式引用。
'  原始代码将这些符号定义在 `Namespace Java` 的 `Module CLI_API` 中，随该模块失效而缺失，
'  这里集中提供一个线程安全的轻量输出通道，供 Core / Alignment / Assembly 等底层模块使用。
'
' /********************************************************************************/

Imports System.IO

Namespace Core

    ''' <summary>
    ''' 运行期共享状态，保存 Rockhopper 执行过程中的全局参数与输出通道。
    ''' </summary>
    ''' <remarks>
    ''' 复刻自原始 Rockhopper Java 版的 <c>verbose</c> 全局变量与 <c>Output()</c> 方法语义：
    ''' 输出同时写入标准输出与 summary.txt（若已配置）。
    ''' </remarks>
    Public Module Logging

        ''' <summary>
        ''' 是否输出 verbose 级别的详细信息（对应命令行 <c>-v true</c>）。
        ''' </summary>
        Public Property Verbose As Boolean = False

        ''' <summary>
        ''' 汇总输出文件（summary.txt）的写入器，为 Nothing 时仅输出到控制台。
        ''' </summary>
        Public Property SummaryWriter As TextWriter = Nothing

        Private ReadOnly _sync As New Object()

        ''' <summary>
        ''' 输出一段文本到控制台，并在 <see cref="SummaryWriter"/> 存在时同步写入 summary.txt。
        ''' </summary>
        Public Sub Output(text As String)
            If SummaryWriter IsNot Nothing Then
                SyncLock _sync
                    SummaryWriter.Write(text)
                    SummaryWriter.Flush()
                End SyncLock
            End If

            Console.Write(text)
        End Sub

        ''' <summary>
        ''' 输出一行文本（等价于 <see cref="Output"/> 后追加换行）。
        ''' </summary>
        Public Sub OutputLine(text As String)
            Call Output(text & vbLf)
        End Sub

    End Module

End Namespace
