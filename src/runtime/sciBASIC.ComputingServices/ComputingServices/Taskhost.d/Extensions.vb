#Region "Microsoft.VisualBasic::c09c71724f161dea52376d2d406acd85, ..\sciBASIC.ComputingServices\ComputingServices\Taskhost.d\Extensions.vb"

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

Imports System.Runtime.CompilerServices

Namespace TaskHost

    Public Module Extensions

        ''' <summary>
        ''' Services running on a LAN?
        ''' </summary>
        Dim _local As Boolean = False

        ''' <summary>
        ''' 假若这个参数为真，则说明服务只是运行在局域网之中，则只会返回局域网的IP地址
        ''' 假若为假，则说明服务是运行在互联网上面的，则会查询主机的公共IP地址，调试的时候建议设置为真
        ''' </summary>
        ''' <returns></returns>
        Public Property EnvironmentLocal As Boolean
            Get
                Return _local
            End Get
            Set(value As Boolean)
                _local = value

                If _local Then
                    _IPAddress = Net.AsynInvoke.LocalIPAddress
                Else
                    _IPAddress = GetMyIPAddress()
                End If
            End Set
        End Property

        Public ReadOnly Property IPAddress As String

        Sub New()
#If DEBUG Then
            EnvironmentLocal = True
#Else
            EnvironmentLocal = False
#End If
        End Sub

        Public ReadOnly Property PublicShared As System.Reflection.BindingFlags =
            System.Reflection.BindingFlags.Public Or
            System.Reflection.BindingFlags.Static

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="type"></param>
        ''' <param name="name">NameOf</param>
        ''' <returns></returns>
        <Extension> Public Function [AddressOf](type As Type, name As String) As InvokeInfo
            Dim method = type.GetMethod(name, bindingAttr:=PublicShared)
            Dim info As New InvokeInfo With {
                .assm = FileIO.FileSystem.GetFileInfo(type.Assembly.Location).Name,
                .Name = method.Name,
                .FullIdentity = type.FullName
            }
            Return info
        End Function

        <Extension> Public Function Invoke(info As InvokeInfo, host As TaskHost) As Object
            If host Is Nothing Then
                Return TaskInvoke.TryInvoke(info)
            Else
                Dim rtvl = host.Invoke(info)
                Dim entry = info.GetMethod
                Return rtvl.GetValue(entry.ReturnType)
            End If
        End Function

        ''' <summary>
        ''' DirectCast
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="info"></param>
        ''' <param name="host"></param>
        ''' <returns></returns>
        <Extension> Public Function Invoke(Of T)(info As InvokeInfo, host As TaskHost) As T
            Return DirectCast(info.Invoke(host), T)
        End Function

        Public Function AddressEquals(a As Object, b As Object) As Boolean
            Return ObjectAddress.AddressOf(a) = b
        End Function
    End Module
End Namespace
