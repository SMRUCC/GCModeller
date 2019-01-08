Imports Microsoft.VisualBasic.Parallel

Namespace Platform

    ''' <summary>
    ''' 某一项任务会需要被重复执行
    ''' </summary>
    Public Class TaskWorker

        ReadOnly taskQueue As New ThreadQueue
        ReadOnly doTask As Func(Of Boolean)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="task">
        ''' 这是一个类似于迭代器的函数, 通过调用这个函数来执行具体的任务内容, 然后执行完毕之后通过这个函数的返回值来了解是否还有新的任务
        ''' 如果函数返回true, 则说明还有新的任务
        ''' 如果返回false, 则说明没有新的任务, 则函数在执行完毕之后会退出循环
        ''' </param>
        Sub New(task As Func(Of Boolean))
            doTask = task
        End Sub

        Public Sub Push()
            Call taskQueue.AddToQueue(AddressOf runInternal)
        End Sub

        Private Sub runInternal()
            Do While doTask()
                ' 只要返回true, 则这个循环会一直执行下去
            Loop
        End Sub
    End Class
End Namespace