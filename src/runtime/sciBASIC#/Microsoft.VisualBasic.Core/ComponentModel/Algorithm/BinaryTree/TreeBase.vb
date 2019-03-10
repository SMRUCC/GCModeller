﻿#Region "Microsoft.VisualBasic::d0d38f7bbe88ad4665ca33c51f5a3d66, Microsoft.VisualBasic.Core\ComponentModel\Algorithm\BinaryTree\TreeBase.vb"

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

    '     Class TreeBase
    ' 
    '         Properties: root
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetAllNodes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace ComponentModel.Algorithm.BinaryTree

    ''' <summary>
    ''' 二叉树对象的通用模板
    ''' </summary>
    ''' <typeparam name="K"></typeparam>
    ''' <typeparam name="V"></typeparam>
    Public MustInherit Class TreeBase(Of K, V)

        ''' <summary>
        ''' The root node of this binary tree
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property root As BinaryTree(Of K, V)
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return _root
            End Get
        End Property

        Protected _root As BinaryTree(Of K, V)

        Protected ReadOnly compares As Comparison(Of K)
        Protected ReadOnly views As Func(Of K, String)
        Protected ReadOnly stack As New List(Of BinaryTree(Of K, V))

        ''' <summary>
        ''' Create an instance of the AVL binary tree.
        ''' </summary>
        ''' <param name="compares">Compare between two keys.</param>
        ''' <param name="views">Display the key as string</param>
        Sub New(compares As Comparison(Of K), Optional views As Func(Of K, String) = Nothing)
            Me.compares = compares
            Me.views = views
        End Sub

        ''' <summary>
        ''' 将整棵树销毁
        ''' </summary>
        Public Overridable Sub Clear()
            _root = Nothing
            stack.Clear()
        End Sub

        Public Function GetAllNodes() As BinaryTree(Of K, V)()
            Return stack.ToArray
        End Function
    End Class
End Namespace
