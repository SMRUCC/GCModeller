Imports System

Module Program

    ''' <summary>
    ''' 测试项目入口：运行 GEARS 虚拟扰动演示
    ''' </summary>
    ''' <param name="args">命令行参数（未使用）</param>
    ''' <returns>进程退出码；成功为 0，异常为 1</returns>
    Function Main(args As String()) As Integer
        Try
            Call GEARSDemo.Run()

            Return 0
        Catch ex As Exception
            Console.WriteLine()
            Console.WriteLine("演示执行失败:")
            Console.WriteLine(ex.ToString())

            Return 1
        End Try
    End Function
End Module
