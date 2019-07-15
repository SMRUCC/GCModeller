#Region "Microsoft.VisualBasic::89fb3ef5440c16e492d029ed0f8189f3, WebCloud\SMRUCC.HTTPInternal\Core\FileSystem\FileNode.vb"

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

    '     Class FileNode
    ' 
    '         Properties: File, isDirectory, MySelf
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataStructures.Tree

Namespace Core.Cache

    Public Class FileNode : Inherits TreeNodeBase(Of FileNode)
        Implements ITreeNode(Of FileNode)

        Public ReadOnly Property File As CachedFile

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="name">FileName</param>
        Public Sub New(name As String)
            MyBase.New(name)
        End Sub

        Public ReadOnly Property isDirectory As Boolean
            Get
                Return ChildNodes.Count > 0
            End Get
        End Property

        Public Overrides ReadOnly Property MySelf As FileNode
            Get
                Return Me
            End Get
        End Property
    End Class
End Namespace
