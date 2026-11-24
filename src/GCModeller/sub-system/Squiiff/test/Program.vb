Imports System

Module Program

    ''' <summary>
    ''' 测试项目入口：先做有限差分梯度校验（护栏），再跑端到端演示。
    ''' </summary>
    ''' <param name="args">命令行参数（未使用）。</param>
    ''' <returns>进程退出码；全部通过为 0，失败为 1。</returns>
    Function Main(args As String()) As Integer
        Console.OutputEncoding = System.Text.Encoding.UTF8

        Try
            Dim gradientsOk As Boolean = GradientCheck.Run()

            If Not gradientsOk Then
                Console.WriteLine()
                Console.WriteLine("梯度校验未通过，终止演示（反向传播有误时训练结果无意义）。")

                Return 1
            End If

            Dim demoOk As Boolean = SquiiffDemo.Run()

            Return If(demoOk, 0, 1)
        Catch ex As Exception
            Console.WriteLine()
            Console.WriteLine("演示执行失败:")
            Console.WriteLine(ex.ToString())

            Return 1
        End Try
    End Function

End Module
