#Region "Microsoft.VisualBasic::ea59a5e11880b4d0f4759ad740bb81fe, ..\sciBASIC.ComputingServices\ComputingServices\FileSystem\Protocols\FileURI.vb"

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

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Net

Namespace FileSystem.Protocols

    ''' <summary>
    ''' Represents a local file or remote file its location on the network.
    ''' (表示一个本地文件或者网络上面的文件的位置)
    ''' </summary>
    Public Class FileURI

        ''' <summary>
        ''' The services portal of this remote filesystem object.
        ''' (远程文件服务对象的服务接口)
        ''' </summary>
        ''' <returns></returns>
        Public Property EntryPoint As IPEndPoint
        ''' <summary>
        ''' The reference location of this file system object on the target machine.
        ''' (目标机器(<see cref="EntryPoint"/>)上面的文件系统的引用的位置)
        ''' </summary>
        ''' <returns></returns>
        Public Property File As String

        ''' <summary>
        ''' Is this file system object is a local file?.(是否为本地文件)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsLocal As Boolean
            Get
                Return EntryPoint Is Nothing
            End Get
        End Property

        Sub New(uri As String)
            Dim addr As String = Regex.Match(uri, "^\d+@\d+(\.\d+){3}[:]\/\/").Value
            If String.IsNullOrEmpty(addr) Then  '本地文件
                File = uri
                EntryPoint = Nothing
            Else ' 远程文件
                File = uri.Replace(addr, "")
                Dim Tokens As String() = addr.Split(":"c).First.Split("@"c)
                EntryPoint = New IPEndPoint(Tokens(1), Scripting.CTypeDynamic(Of Integer)(Tokens(Scan0)))
            End If
        End Sub

        Sub New(path As String, portal As IPEndPoint)
            File = path
            EntryPoint = portal
        End Sub

        Sub New()
        End Sub

        ''' <summary>
        ''' {port}@{ipaddress}://C:\xxx\xxx.file
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            If EntryPoint Is Nothing Then
                Return File
            Else
                Return $"{EntryPoint.Port}@{EntryPoint.IPAddress}://{File}"
            End If
        End Function

        ''' <summary>
        ''' port@remote_IP://hash+&lt;path>
        ''' </summary>
        ''' <param name="uri"></param>
        ''' <param name="remote"></param>
        ''' <param name="handle"></param>
        Public Shared Sub FileStreamParser(uri As String, ByRef remote As IPEndPoint, ByRef handle As FileHandle)
            Dim file As New FileURI(uri)
            remote = file.EntryPoint
            uri = file.File
            Dim hash As String = Regex.Match(uri, "^\d+\+").Value
            uri = Mid(uri, hash.Length + 1)
            handle = New FileHandle With {
                .FileName = uri,
                .HashCode = Scripting.CTypeDynamic(Of Integer)(hash)
            }
        End Sub
    End Class
End Namespace
