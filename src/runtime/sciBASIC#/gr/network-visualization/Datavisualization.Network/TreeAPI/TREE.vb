﻿#Region "Microsoft.VisualBasic::1046fc72d01ff1f8948d4a42120165af, gr\network-visualization\Datavisualization.Network\TreeAPI\TREE.vb"

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

    '     Enum NodeTypes
    ' 
    '         Leaf, LeafX, Path, ROOT
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class TreeNode
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class LeafX
    ' 
    '         Properties: LeafX
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetEntities
    ' 
    '     Class Leaf
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetEntities
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataStructures.BinaryTree
Imports Microsoft.VisualBasic.Linq

Namespace TreeAPI

    Public Enum NodeTypes
        Path
        Leaf
        LeafX
        ROOT
    End Enum

    Public MustInherit Class TreeNode : Inherits TreeNode(Of NodeTypes)

        Public MustOverride Function GetEntities() As String()

        Sub New(parent As String, myType As NodeTypes)
            Call MyBase.New(parent, myType)
        End Sub
    End Class

    Public Class LeafX : Inherits TreeNode

        Public Property LeafX As FileStream.NetworkEdge()

        Sub New(parent As String)
            Call MyBase.New(parent & "-LeafX", NodeTypes.LeafX)
        End Sub

        Public Overrides Function GetEntities() As String()
            Return LeafX.Select(Function(x) x.ToNode).ToArray
        End Function
    End Class

    Public Class Leaf : Inherits TreeNode

        Sub New(parent As String)
            Call MyBase.New(parent & "-Leaf", NodeTypes.Leaf)
        End Sub

        Public Overrides Function GetEntities() As String()
            Return Me.GetEnumerator.Select(Function(x) x.Name)
        End Function
    End Class
End Namespace
