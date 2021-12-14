﻿#Region "Microsoft.VisualBasic::533cfb64eccbe479f112aaee5fba2a55, Data_science\Graph\Model\Tree\Tree.vb"

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

    ' Class Tree
    ' 
    '     Properties: Data
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    ' Class Tree
    ' 
    '     Properties: Data
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.Serialization

''' <summary>
''' Tree node with data..(可以直接被使用的树对象类型)
''' </summary>
''' <typeparam name="T"></typeparam>
''' 
<DataContract>
Public Class Tree(Of T, K) : Inherits AbstractTree(Of Tree(Of T, K), K)

    Public Property Data As T

    Sub New(Optional qualDeli$ = ".")
        MyBase.New(qualDeli)
    End Sub

    Sub New()
        Call MyBase.New(".")
    End Sub
End Class

''' <summary>
''' 使用字符串<see cref="String"/>作为键名的树节点
''' </summary>
''' <typeparam name="T"></typeparam>
''' <remarks>
''' 在这里如果直接继承<see cref="Tree(Of T, K)"/>类型的话，会导致child的类型错误
''' </remarks>
Public Class Tree(Of T) : Inherits AbstractTree(Of Tree(Of T), String)

    Public Property Data As T

    Sub New(Optional qualDeli$ = ".")
        MyBase.New(qualDeli)
    End Sub

    Public Function Add(child As Tree(Of T)) As Tree(Of T)
        Call Childs.Add(child.label, child)
        Return Me
    End Function
End Class
