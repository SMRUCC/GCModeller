#Region "Microsoft.VisualBasic::f35b64e99199f26edb8198e13830cb81, RDotNET.Extensions.VisualBasic\Server\GC.vb"

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

    ' Module RDotNetGC
    ' 
    '     Function: Allocate
    ' 
    '     Sub: [Do], Add, Exclude
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

''' <summary>
''' Garbage Collection
''' </summary>
Public Module RDotNetGC

    Dim objects As New List(Of String)

    Public ReadOnly Property numObjects As Integer
        Get
            Return objects.Count
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Add(name As String)
        Call objects.Add(name)
    End Sub

    ''' <summary>
    ''' 主要是用于脚本化编程, 这个函数在分配了一个新的变量之后, 会将这个变量加入到GC列表之中
    ''' </summary>
    ''' <returns></returns>
    Friend Function Allocate() As String
        Dim name As String = App.NextTempName
        Call objects.Add(name)
        Return name
    End Function

    ''' <summary>
    ''' 从GC队列之中删除所给定的变量, 因为脚本化编程的api总是自动将新的临时变量添加进入gc队列的
    ''' 所以会需要使用这个函数来移除一些不想要被删除的对象
    ''' </summary>
    ''' <param name="names"></param>
    Public Sub Exclude(ParamArray names As String())
        For Each name As String In names
            Call objects.Remove(name)
        Next
    End Sub

    ''' <summary>
    ''' 一次性的将R环境之中的通过<see cref="Add"/>方法所添加的对象进行删除
    ''' </summary>
    Public Sub [Do]()
        Dim names = base.c(objects.ToArray, stringVector:=True)

        Call base.rm(list:=names)
        Call base.rm(list:=names.Rstring)
        Call base.gc()
        Call objects.Clear()
    End Sub
End Module

