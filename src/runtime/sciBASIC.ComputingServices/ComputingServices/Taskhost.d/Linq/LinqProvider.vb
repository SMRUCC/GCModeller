#Region "Microsoft.VisualBasic::45e89a333a2883f26931ffc29cc7e4d5, ..\sciBASIC.ComputingServices\ComputingServices\Taskhost.d\Linq\LinqProvider.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Scripting.InputHandler
Imports Microsoft.VisualBasic.Serialization.JSON
Imports sciBASIC.ComputingServices.ComponentModel

Namespace TaskHost

    ''' <summary>
    ''' 执行得到数据集合然后分独传输数据元素
    ''' </summary>
    ''' 
    <Protocol(GetType(TaskProtocols))>
    Public Class LinqProvider : Inherits IHostBase
        Implements IDisposable

        ReadOnly _arrayType As Type
        ReadOnly _type As Type
        ReadOnly _source As Iterator

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="type">Element's <see cref="System.Type">type</see> in the <paramref name="source"/></param>
        Sub New(source As IEnumerable, type As Type)
            Call MyBase.New(Net.GetFirstAvailablePort(-1))

            _type = type
            _source = New Iterator(source)
            __host.Responsehandler = AddressOf New ProtocolHandler(Me).HandleRequest
            _arrayType = type.MakeArrayType

            __svrTask = __svrThread.BeginInvoke(Nothing, Nothing)  ' 避免崩溃的情况产生
        End Sub

        ''' <summary>
        ''' 使用线程可能会在出现错误的时候导致应用程序崩溃，所以在这里使用begineInvoke好了
        ''' </summary>
        Dim __svrThread As Action = Sub() Call __host.Run()
        Dim __svrTask As IAsyncResult

        ''' <summary>
        ''' 当前的这个数据源服务是否已经正确的开启了？
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsOpen As Boolean
            Get
                Return New AsynInvoke(Portal).Ping >= 0
            End Get
        End Property

        ''' <summary>
        ''' Linq数据源的集合的类型信息
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property BaseType As Type
            Get
                Return _arrayType
            End Get
        End Property

        Public Overrides ReadOnly Property Portal As IPEndPoint
            Get
                Return Me.GetPortal
            End Get
        End Property

        Public Shared Function CreateObject(Of T)(source As IEnumerable(Of T)) As LinqProvider
            Return New LinqProvider(source, GetType(T))
        End Function

        Public Function GetReturns() As Rtvl
            Return New Rtvl(Portal, GetType(IPEndPoint))
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="n">
        ''' 个数小于或者等于1，就只会返回一个对象；
        ''' 个数大于1的，会读取相应数量的元素然后返回一个集合类型
        ''' </param>
        ''' <param name="readDone"></param>
        ''' <returns></returns>
        Public Function Moves(n As Integer, Optional ByRef readDone As Boolean = False) As Object
            If n <= 1 Then
                Dim value As Object = _source.Current
                readDone = _source.MoveNext()
                Return value
            Else
                Dim list As New List(Of Object)
                For i As Integer = 0 To n - 1
                    Call list.Add(_source.Current)
                    readDone = _source.MoveNext
                Next
                Return [DirectCast](list.ToArray, _type)
            End If
        End Function

        <Protocol(TaskProtocols.MoveNext)>
        Private Function __moveNext(CA As Long, args As RequestStream, remote As System.Net.IPEndPoint) As RequestStream
            Dim readEnds As Boolean
            Dim value As Object = Moves(1, readEnds)
            Dim json As String = JsonContract.GetObjectJson(value, _type)
            Dim flag As Long = If(Not readEnds, Protocols.TaskProtocols.ReadsDone, HTTP_RFC.RFC_OK)
            Return New RequestStream(flag, flag, json)
        End Function

        <Protocol(TaskProtocols.Reset)>
        Private Function __reset(CA As Long, args As RequestStream, remote As System.Net.IPEndPoint) As RequestStream
            Call _source.Reset()
            Return NetResponse.RFC_OK
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call _source.Free
                    Call __host.Free
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
