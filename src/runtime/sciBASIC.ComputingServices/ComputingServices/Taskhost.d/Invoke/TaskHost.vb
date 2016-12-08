#Region "Microsoft.VisualBasic::c0cba50390a4eccad66b31d5f91d63ba, ..\sciBASIC.ComputingServices\ComputingServices\Taskhost.d\Invoke\TaskHost.vb"

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
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Serialization.JSON
Imports sciBASIC.ComputingServices.ComponentModel
Imports sciBASIC.ComputingServices.FileSystem

Namespace TaskHost

    ''' <summary>
    ''' Using this object to running the method on the remote machine.
    ''' (由于是远程调用，所以运行的环境可能会很不一样，所以在设计程序的时候请尽量
    ''' 避免或者不要使用模块变量，以免出现难以调查的BUG)
    ''' </summary>
    Public Class TaskRemote : Implements IRemoteSupport
        Implements INamedValue

        Dim _remote As IPEndPoint

        Public ReadOnly Property FileSystem As FileSystemHost Implements IRemoteSupport.FileSystem

        Private Property Key As String Implements INamedValue.Key
            Get
                Return _remote.IPAddress
            End Get
            Set(value As String)
                _remote.IPAddress = value
            End Set
        End Property

        Sub New(remote As IPEndPoint)
            _remote = remote
            FileSystem = New FileSystemHost(GetFirstAvailablePort)
        End Sub

        Sub New(remote As String, port As Integer)
            Call Me.New(New IPEndPoint(remote, port))
        End Sub

        ''' <summary>
        ''' Start the application on the remote host.(相当于Sub，调用远程的命令行程序，只会返回0或者错误代码)
        ''' </summary>
        ''' <param name="exe">Exe file path</param>
        ''' <param name="args"></param>
        ''' <returns></returns>
        Public Function Shell(exe As String, args As String) As Integer
            Dim func As Func(Of String, String, Integer) = AddressOf Protocols.Shell
            Return Invoke(Of Integer)(func, {exe, args})
        End Function

        ''' <summary>
        ''' Running the function delegate pointer on the remote machine. 
        ''' 
        ''' *****************************************************************************************************
        ''' NOTE: Performance issue, this is important! if the function pointer its returns type is a collection, 
        ''' then you should using the method <see cref="TaskRemote.AsLinq(Of T)([Delegate], Object())"/> to running 
        ''' your code on the remote. Or a large json data will be return back through network in one package, 
        ''' this may cause a serious performance problem both on your server and your local client.
        ''' (本地服务器通过这个方法调用远程主机上面的函数，假若目标函数的返回值类型是一个集合，
        ''' 请使用<see cref="TaskRemote.AsLinq(Of T)([Delegate], Object())"/>方法，否则集合之中的所有数据都将会一次性返回，
        ''' 这个可能会导致严重的性能问题)
        ''' </summary>
        ''' <param name="target"></param>
        ''' <param name="args"></param>
        ''' <returns></returns>
        Public Function Invoke(target As [Delegate], ParamArray args As Object()) As Object
            Dim params As InvokeInfo = InvokeInfo.CreateObject(target, args)
            Dim rtvl As Rtvl = Invoke(params)
            Dim obj As Object = rtvl.GetValue(target)
            Return obj
        End Function

        Public Function Invoke(info As InvokeInfo) As Rtvl
            Dim value As String = JsonContract.GetJson(info)
            Dim req As RequestStream = New RequestStream(ProtocolEntry, TaskProtocols.Invoke, value)
            Dim rep As RequestStream = New AsynInvoke(_remote).SendMessage(req)
            Dim rtvl As Rtvl = JsonContract.LoadObject(Of Rtvl)(rep.GetUTF8String)
            Return rtvl
        End Function

        Public Function Invoke(Of T)(target As [Delegate], ParamArray args As Object()) As T
            Dim value As Object = Invoke(target, args)

            If value Is Nothing Then
                Return Nothing
            Else
                Return DirectCast(value, T)
            End If
        End Function

        ''' <summary>
        ''' If your function pointer returns type is a collection, then using this method is recommended.
        ''' (执行远程机器上面的代码，然后返回数据查询接口)
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="target">远程机器上面的函数指针</param>
        ''' <param name="args"></param>
        ''' <returns></returns>
        Public Function AsLinq(Of T)(target As [Delegate], ParamArray args As Object()) As ILinq(Of T)
            Dim params As InvokeInfo = InvokeInfo.CreateObject(target, args)
            Dim jparam As String = params.GetJson
            Dim req As New RequestStream(ProtocolEntry, TaskProtocols.InvokeLinq, jparam)
            Dim rep As RequestStream = New AsynInvoke(_remote).SendMessage(req)
            Dim svr As IPEndPoint = rep.GetUTF8String.LoadObject(Of IPEndPoint)
            Return New ILinq(Of T)(svr)
        End Function
    End Class
End Namespace
