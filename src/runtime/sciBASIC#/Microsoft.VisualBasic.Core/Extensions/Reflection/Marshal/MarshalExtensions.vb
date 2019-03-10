﻿#Region "Microsoft.VisualBasic::a810b6c4ba107323a3ec2d0a908a1d7c, Microsoft.VisualBasic.Core\Extensions\Reflection\Marshal\MarshalExtensions.vb"

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

    ' Module MarshalExtensions
    ' 
    '     Function: (+2 Overloads) MarshalAs
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Emit.Marshal

''' <summary>
''' 
''' </summary>
Public Module MarshalExtensions

    ''' <summary>
    ''' Read unmanaged memory using memory pointer.
    ''' </summary>
    ''' <typeparam name="T">
    ''' <see cref="Integer"/>, <see cref="Char"/>, <see cref="Short"/>, <see cref="Long"/>, <see cref="Single"/>, <see cref="Byte"/>, <see cref="IntPtr"/>, <see cref="Double"/>
    ''' </typeparam>
    ''' <param name="p"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' ```vbnet
    ''' Public Shared Sub Copy(source As IntPtr, destination() As Integer, startIndex As Integer, length As Integer)
    ''' Public Shared Sub Copy(source As IntPtr, destination() As Char, startIndex As Integer, length As Integer)
    ''' Public Shared Sub Copy(source As IntPtr, destination() As Short, startIndex As Integer, length As Integer)
    ''' Public Shared Sub Copy(source As IntPtr, destination() As Long, startIndex As Integer, length As Integer)
    ''' Public Shared Sub Copy(source As IntPtr, destination() As Single, startIndex As Integer, length As Integer)
    ''' Public Shared Sub Copy(source As IntPtr, destination() As Byte, startIndex As Integer, length As Integer)
    ''' Public Shared Sub Copy(source As IntPtr, destination() As IntPtr, startIndex As Integer, length As Integer)
    ''' Public Shared Sub Copy(source As IntPtr, destination() As Double, startIndex As Integer, length As Integer)
    ''' ```
    ''' </remarks>
    <Extension> Public Function MarshalAs(Of T)(p As System.IntPtr, chunkSize As Integer) As IntPtr(Of T)
        Select Case GetType(T)
            Case GetType(Integer) : Return DirectCast(CType(New [Integer](p, chunkSize), Object), IntPtr(Of T))
            Case GetType(Char) : Return DirectCast(CType(New [Char](p, chunkSize), Object), IntPtr(Of T))
            Case GetType(Short) : Return DirectCast(CType(New [Short](p, chunkSize), Object), IntPtr(Of T))
            Case GetType(Long) : Return DirectCast(CType(New [Long](p, chunkSize), Object), IntPtr(Of T))
            Case GetType(Single) : Return DirectCast(CType(New [Single](p, chunkSize), Object), IntPtr(Of T))
            Case GetType(Byte) : Return DirectCast(CType(New [Byte](p, chunkSize), Object), IntPtr(Of T))
            Case GetType(System.IntPtr) : Return DirectCast(CType(New IntPtr(p, chunkSize), Object), IntPtr(Of T))
            Case GetType(Double) : Return DirectCast(CType(New [Double](p, chunkSize), Object), IntPtr(Of T))
            Case Else
                Throw New MemberAccessException(GetType(T).FullName & " is not a valid memory region!")
        End Select
    End Function

    ''' <summary>
    ''' 方便进行数组操作的一个函数
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="raw"></param>
    ''' <param name="p"></param>
    ''' <returns></returns>
    <Extension> Public Function MarshalAs(Of T)(ByRef raw As T(), Optional p As System.IntPtr = Nothing) As IntPtr(Of T)
        Return New IntPtr(Of T)(raw, p)
    End Function
End Module
