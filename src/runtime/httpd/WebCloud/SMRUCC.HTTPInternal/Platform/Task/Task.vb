#Region "Microsoft.VisualBasic::26b40332255239bee1ef2a564c64338d, ..\httpd\WebCloud\SMRUCC.HTTPInternal\Platform\Task\Task.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Platform

    Public MustInherit Class Task : Implements IDisposable
        Implements IReadOnlyId

        Protected Friend _innerTaskPool As TaskPool
        Protected current As Integer

        Dim _callback As Action

        ''' <summary>
        ''' 任务的编号
        ''' </summary>
        ''' <returns></returns>
        Public Property uid As String Implements IReadOnlyId.Identity
        Public ReadOnly Property Complete As Boolean

        Sub New(callback As Action)
            _callback = callback
            Complete = False
        End Sub

        ''' <summary>
        ''' 提供了对任务执行过程<see cref="RunTask"/>之中的任务内容的描述
        ''' </summary>
        ''' <returns></returns>
        Protected MustOverride Function contents() As String()

        ''' <summary>
        ''' Public interface for invoke this task.
        ''' (需要在这个过程对象之中实现具体的任务过程)
        ''' </summary>
        Public MustOverride Sub RunTask()

        Public Function GetProgress() As TaskProgress
            Dim o As New TaskProgress With {
                .current = current,
                .progress = contents()
            }
            Return o
        End Function

        Public Overrides Function ToString() As String
            Return $" {uid} -> [{contents(current)}]"
        End Function

        Public Function Start() As Task
            _Complete = False
            Call RunTask()
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

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).                   
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
