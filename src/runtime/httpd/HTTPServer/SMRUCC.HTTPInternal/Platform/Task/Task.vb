#Region "Microsoft.VisualBasic::26b40332255239bee1ef2a564c64338d, ..\httpd\HTTPServer\SMRUCC.HTTPInternal\Platform\Task\Task.vb"

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

Namespace Platform

    Public Class Task : Implements System.IDisposable
        Implements IReadOnlyId

        Protected Friend _innerTaskPool As TaskPool

        Dim _task As Action
        Dim _callback As Action

        ''' <summary>
        ''' 任务的编号
        ''' </summary>
        ''' <returns></returns>
        Public Property uid As String Implements IReadOnlyId.Identity
        Public ReadOnly Property Complete As Boolean

        Sub New(task As Action, callback As Action)
            _task = task
            _callback = callback
            Complete = True
        End Sub

        Public Function Start() As Task
            _Complete = False
            Call _task()
            Call _callback()
            _Complete = True
            Return Me
        End Function

        ''' <summary>
        ''' 获取当前的这个任务对象在队列之中的等待位置
        ''' </summary>
        ''' <returns></returns>
        Public Function GetQueuePos() As Integer
            Return _innerTaskPool._taskQueue.IndexOf(Me)
        End Function

        Public Overrides Function ToString() As String
            Return uid
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    _task = Nothing
                    _callback = Nothing
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
