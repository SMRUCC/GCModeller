#Region "Microsoft.VisualBasic::352eec02af01503445e92e504f5ad498, ..\httpd\HTTPServer\SMRUCC.HTTPInternal\Platform\Task\TaskPool.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports Microsoft.VisualBasic.Parallel.Linq

Namespace Platform

    Public Class TaskPool : Implements System.IDisposable

        Protected Friend ReadOnly _taskQueue As New Queue(Of Task)

        ''' <summary>
        ''' 允许同时运行的任务的数量
        ''' </summary>
        ''' <returns></returns>
        Public Property NumThreads As Integer
        Public Property TimeInterval As Integer = 1000

        ReadOnly TaskPool As New List(Of AsyncHandle(Of Task))
        ReadOnly _runningTask As New List(Of Task)

        ''' <summary>
        ''' 当不存在的时候，说明正在运行，或者已经运行完毕了
        ''' </summary>
        ''' <param name="uid"></param>
        ''' <returns></returns>
        Public Function GetTask(uid As String) As Task
            Dim LQuery = (From x As Task In _taskQueue
                          Where String.Equals(uid, x.uid, StringComparison.OrdinalIgnoreCase)
                          Select x).FirstOrDefault
            Return LQuery
        End Function

        Public Function TaskRunning(uid As String) As Boolean
            Dim task As Task = (From x As Task
                            In _runningTask
                                Where String.Equals(uid, x.uid, StringComparison.OrdinalIgnoreCase)
                                Select x).FirstOrDefault
            If task Is Nothing Then
                Return False
            Else
                Return Not task.Complete
            End If
        End Function

        Sub New()
            Call RunTask(AddressOf __taskInvoke)
        End Sub

        Public Function Queue(task As Task) As Integer
            Call _taskQueue.Enqueue(task)
            task._innerTaskPool = Me
            Return _taskQueue.Count
        End Function

        Public Overrides Function ToString() As String
            Dim s As String = _taskQueue.Count & " tasks in queue..."
            Call s.__DEBUG_ECHO
            Return s
        End Function

        Private Sub __taskInvoke()
            If NumThreads <= 0 Then
                NumThreads = LQuerySchedule.CPU_NUMBER * 2
            End If

            Do While Not disposedValue
                If TaskPool.Count < NumThreads AndAlso _taskQueue.Count > 0 Then  ' 向任务池里面添加新的并行任务
                    Dim task As Task = _taskQueue.Dequeue
                    Call TaskPool.Add(New AsyncHandle(Of Task)(AddressOf task.Start).Run)
                    Call _runningTask.Add(task)
                End If

                Dim LQuery = (From task As AsyncHandle(Of Task)
                          In TaskPool
                              Where task.IsCompleted ' 在这里获得完成的任务
                              Select task).ToArray
                For Each completeTask As AsyncHandle(Of Task) In LQuery
                    Dim task As Task = completeTask.GetValue
                    Call TaskPool.Remove(completeTask)  '  将完成的任务从任务池之中移除然后获取返回值
                    Call _runningTask.Remove(task)
                Next

                Call Threading.Thread.Sleep(TimeInterval)
            Loop

            Call (From task In TaskPool.AsParallel  ' 等待剩余的计算任务完成计算过程
                  Let cli As Task = task.GetValue
                  Select cli).ToArray
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
