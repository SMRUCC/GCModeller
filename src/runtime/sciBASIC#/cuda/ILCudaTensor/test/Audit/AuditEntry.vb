' ------------------------------------------------------------------------
' ILCudaTensor 审计入口
'
' 用法：
'   test.exe audit [--cpu-only] [--out <dir>] [--device <n>]
'
' 这是唯一需要从 Program.Main 分派进来的入口，便于保持既有 demo 行为不变。
' ------------------------------------------------------------------------

Namespace Audit

    ''' <summary>审计命令的参数解析与编排</summary>
    Public Module AuditEntry

        ''' <summary>执行审计，返回失败用例数（同时作为进程退出码）。</summary>
        Public Function Run(args As String()) As Integer
            Dim cpuOnly As Boolean = False
            Dim reportDir As String = "audit"
            Dim device As Integer = 0
            Dim i As Integer = 1

            While args IsNot Nothing AndAlso i < args.Length
                Select Case args(i).ToLowerInvariant()
                    Case "--cpu-only"
                        cpuOnly = True
                    Case "--out", "--report"
                        If i + 1 < args.Length Then
                            reportDir = args(i + 1)
                            i += 1
                        End If
                    Case "--device"
                        If i + 1 < args.Length Then
                            device = Integer.Parse(args(i + 1))
                            i += 1
                        End If
                    Case Else
                        Console.WriteLine($"未知参数: {args(i)}")
                End Select

                i += 1
            End While

            Console.WriteLine()
            Console.WriteLine("======================================================================")
            Console.WriteLine("  ILCudaTensor 融合核审计（三维池化 / 计数合成）")
            Console.WriteLine("======================================================================")

            Call AuditRunner.Setup(cpuOnly, device, reportDir)

            Call PoolingAudit.Run()
            Call SynthesisAudit.Run()

            Return AuditRunner.Flush()
        End Function
    End Module
End Namespace
