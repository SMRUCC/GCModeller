Imports System

''' <summary>
''' 虚拟细胞工作流的自检入口：按子命令分派到各个 DebugAudit 审计模块。
''' </summary>
''' <remarks>
''' <code>
''' VirtualCellular.test conv     # 流式稀疏矩阵转换
''' VirtualCellular.test model    # 模型 Save/Load
''' VirtualCellular.test pert     # 虚拟扰动
''' </code>
''' 退出码为失败断言数，0 表示全部通过。
''' </remarks>
Public Module Program

    ''' <summary>入口。</summary>
    Public Function Main(args As String()) As Integer
        Dim command As String = If(args Is Nothing OrElse args.Length = 0, "conv", args(0).ToLowerInvariant())

        Select Case command
            Case "conv"
                Return ConverterAudit.Run()
            Case "meta"
                Return MetaAudit.Run(If(args.Length > 1, args(1), Nothing))
            Case "model", "pert"
                Call Console.WriteLine($"   [audit] '{command}' is not implemented yet")
                Return 1
            Case Else
                Call Console.WriteLine("usage: VirtualCellular.test <conv|meta|model|pert> [path]")
                Return 1
        End Select
    End Function

End Module
