#Region "Microsoft.VisualBasic::59253114aa9e8b8df373f94adde3ab9d, ..\sciBASIC.ComputingServices\ComputingServices\Asymmetric\Protocols.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.Reflection

Namespace Asymmetric

    Public Module Protocols

        Public Const OAuth As String = "--oauth"

        Public Enum Protocols As Long
            FolkInstance = 893579348435
            ''' <summary>
            ''' 关闭目标物理机上面的一个指定编号的实例进程
            ''' </summary>
            ShutdownInstance
            ''' <summary>
            ''' 主节点获取目标物理机的当前的系统负载，请注意系统负载的计算方法是可以被复写的，这个是为了适应不同的需求
            ''' </summary>
            GetLoad
            ''' <summary>
            ''' 物理机上面的服务节点在启动之后向主节点进行自身网络位置的注册
            ''' </summary>
            NodeRegister

            ''' <summary>
            ''' 主节点向目标物理主机请求所有在运行之中的服务进程实例
            ''' </summary>
            GetInstanceList
        End Enum

        Public ReadOnly Property ProtocolEntry As Long =
            New Protocol(GetType(Protocols)).EntryPoint

        Public Function FolkInstance(CLI As String) As RequestStream
            Return New RequestStream(ProtocolEntry, Protocols.FolkInstance, CLI)
        End Function

        Public Function GetInstanceList() As RequestStream
            Return New RequestStream(ProtocolEntry, Protocols.GetInstanceList, "GET")
        End Function

        ''' <summary>
        ''' 每一台物理主机上面只有一个宿主服务，所以其端口号固定
        ''' </summary>
        Public Const ParasitiferSvr As Integer = 54032
        ''' <summary>
        ''' 中心节点服务
        ''' </summary>
        Public Const MasterSvr As Integer = 4568

        Public Function GetLoad() As RequestStream
            Return New RequestStream(ProtocolEntry, Protocols.GetLoad, "GET")
        End Function

        Public Class RegisterPost
            <XmlAttribute> Public Property IPAddress As String
            <XmlAttribute> Public Property uid As Long
        End Class

        Public Function NodeRegister(IPAddress As String, OAuth As Net.SSL.Certificate) As RequestStream
            Dim post As New RegisterPost With {.IPAddress = IPAddress, .uid = OAuth.uid}
            Return New RequestStream(ProtocolEntry, Protocols.NodeRegister, post.GetXml)
        End Function

        Public Function GetPostData(request As RequestStream) As RegisterPost
            Return request.LoadObject(Of RegisterPost)(AddressOf LoadFromXml)
        End Function
    End Module
End Namespace
