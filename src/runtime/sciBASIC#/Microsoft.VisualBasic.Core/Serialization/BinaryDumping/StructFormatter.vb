﻿#Region "Microsoft.VisualBasic::3772c04517d8acef596a4a90d56e9935, Microsoft.VisualBasic.Core\Serialization\BinaryDumping\StructFormatter.vb"

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

    '     Module StructFormatter
    ' 
    '         Function: DeSerialize, GetSerializeBuffer, Load, Serialize
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary

Namespace Serialization.BinaryDumping

    Public Module StructFormatter

        ''' <summary>
        ''' Save a structure type object into a binary file.(使用二进制序列化保存一个对象)
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="obj"></param>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function Serialize(Of T)(obj As T, path As String) As Boolean
            Dim buffer As Byte() = obj.GetSerializeBuffer
            Return buffer.FlushStream(path)
        End Function

        <Extension> Public Function GetSerializeBuffer(Of T)(obj As T) As Byte()
            Dim IFormatter As IFormatter = New BinaryFormatter()
            Dim Stream As New IO.MemoryStream()
            Call IFormatter.Serialize(Stream, obj)
            Dim buffer As Byte() = Stream.ToArray
            Return buffer
        End Function

        <Extension> Public Function DeSerialize(Of T)(bytes As Byte()) As T
            Dim obj As Object = (New BinaryFormatter).[Deserialize](New MemoryStream(bytes))
            Return DirectCast(obj, T)
        End Function

        ''' <summary>
        ''' Load a strucutre object from the file system by using binary serializer deserialize.
        ''' (使用反二进制序列化从指定的文件之中加载一个对象)
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function Load(Of T)(path As String) As T
            If Not FileIO.FileSystem.FileExists(path) Then
                Return Activator.CreateInstance(Of T)()
            End If
            Using Stream As Stream = New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
                Dim IFormatter As IFormatter = New BinaryFormatter()
                Dim obj As T = DirectCast(IFormatter.Deserialize(Stream), T)
                Return obj
            End Using
        End Function
    End Module
End Namespace
