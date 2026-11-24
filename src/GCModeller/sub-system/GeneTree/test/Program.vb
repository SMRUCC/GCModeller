Imports System

''' <summary>
''' GeneTree 演示/测试程序的入口
''' </summary>
Module Program

    ''' <summary>
    ''' 进程入口：运行 GeneTree 虚拟扰动演示
    ''' </summary>
    ''' <param name="args">命令行参数，详见 <see cref="GeneTreeDemo.Usage"/></param>
    ''' <returns>进程退出码；成功为 0，参数错误为 2，运行失败为 1</returns>
    Function Main(args As String()) As Integer
        Try
            Return GeneTreeDemo.Run(args)
        Catch ex As Exception
            Console.WriteLine()
            Console.WriteLine("演示执行失败:")
            Console.WriteLine(ex.ToString())

            Return 1
        End Try
    End Function

End Module
