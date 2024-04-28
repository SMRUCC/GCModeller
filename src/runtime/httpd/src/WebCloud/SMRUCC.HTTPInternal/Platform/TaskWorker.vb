#Region "Microsoft.VisualBasic::225b66d041c5e9cede5cfd5a5289aac1, WebCloud\SMRUCC.HTTPInternal\Platform\TaskWorker.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Class TaskWorker
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: Push, runInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
