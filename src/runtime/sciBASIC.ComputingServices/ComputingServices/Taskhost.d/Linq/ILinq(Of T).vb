#Region "Microsoft.VisualBasic::7d03b7943b9b5f79146892368adcc144, ..\sciBASIC.ComputingServices\ComputingServices\Taskhost.d\Linq\ILinq(Of T).vb"

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

Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace TaskHost

    ''' <summary>
    ''' Remote Linq source reader
    ''' </summary>
    Public Class ILinqReader : Implements IDisposable

        ''' <summary>
        ''' Element type in the source collection.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Type As Type
        ''' <summary>
        ''' Remote entry point
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Portal As IPEndPoint

        ReadOnly invoke As AsynInvoke
        ReadOnly req As New RequestStream(Protocols.ProtocolEntry, TaskProtocols.MoveNext)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="portal"></param>
        ''' <param name="type">JSON反序列化所需要的类型信息</param>
        Sub New(portal As IPEndPoint, type As Type)
            Me.Portal = portal
            Me.invoke = New AsynInvoke(portal)
            Me.Type = type
        End Sub

        ''' <summary>
        ''' 这个迭代器函数不会重置远程的数据源
        ''' </summary>
        ''' <param name="n">迭代器所返回来的元素数量，当小于1的时候会被自动重置为1个元素</param>
        ''' <returns></returns>
        Public Iterator Function Moves(n As Integer) As IEnumerable
            If n <= 1 Then
                n = 1
            End If

            For i As Integer = 0 To n - 1
                Dim rep As RequestStream = invoke.SendMessage(req)
                Dim json As String = rep.GetUTF8String
                Dim value As Object = JsonContract.LoadObject(json, Type)

                If rep.ProtocolCategory = TaskProtocols.ReadsDone Then
                    Exit For
                Else
                    Yield value
                End If
            Next
        End Function

        ''' <summary>
        ''' 使用这个迭代器函数查询会自动重置远程的数据源的位置到初始位置
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function AsQuerable() As IEnumerable
            Call invoke.SendMessage(Protocols.LinqReset)  ' resets the remote linq source read position

            Do While True
                Dim rep As RequestStream = invoke.SendMessage(req)
                Dim json As String = rep.GetUTF8String
                Dim value As Object = JsonContract.LoadObject(json, Type)

                If rep.ProtocolCategory = TaskProtocols.ReadsDone Then
                    Exit Do
                Else
                    Yield value
                End If
            Loop
        End Function

        ''' <summary>
        ''' Automatically free the remote resource.(释放远程主机上面的资源)
        ''' </summary>
        Private Sub __free()
            Dim uid As String = Portal.ToString
            Dim req As New RequestStream(ProtocolEntry, TaskProtocols.Free, uid)
            Call invoke.SendMessage(req)
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).

                    Call __free()
                    Call invoke.Free
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

    ''' <summary>
    ''' Remote LINQ source reader
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class ILinq(Of T) : Inherits ILinqReader
        Implements IDisposable
        Implements IEnumerable(Of T)

        ''' <summary>
        ''' Creates a linq source reader from the remote entry point
        ''' </summary>
        ''' <param name="portal"></param>
        Sub New(portal As IPEndPoint)
            Call MyBase.New(portal, GetType(T))
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Type.FullName}@{Portal.ToString}"
        End Function

        Public Overloads Iterator Function Moves(n As Integer) As IEnumerable(Of T)
            For Each x As Object In MyBase.Moves(n)
                Yield DirectCast(x, T)
            Next
        End Function

#Region "Implements IEnumerable(Of T)"

        ''' <summary>
        ''' 这个迭代器函数会自动重置远程数据源
        ''' </summary>
        ''' <returns></returns>
        Public Overloads Iterator Function AsQuerable() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            For Each x As Object In MyBase.AsQuerable
                If x Is Nothing Then
                    Yield x
                Else
                    Yield DirectCast(x, T)
                End If
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield AsQuerable()
        End Function
#End Region
    End Class
End Namespace
