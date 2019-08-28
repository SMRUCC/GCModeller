#Region "Microsoft.VisualBasic::fedb7b213dc5c9b6c0162d89577d5ffe, Bio.Repository\Indexing\NamesIndex.vb"

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

    ' Class NamesIndex
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetValueByName
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree

''' <summary>
''' 对对象名称建立索引
''' </summary>
''' <typeparam name="T"></typeparam>
Public Class NamesIndex(Of T)

    ReadOnly tree As BinaryTree(Of String, T)

    Public Sub New(repository As NamesFactory(Of T))
        Dim tree As New AVLTree(Of String, T)(AddressOf String.Compare)

        For Each obj As T In repository.PopulateObjects
            For Each name As String In repository.GetNames(obj)
                Call tree.Add(name, obj, False)
            Next
        Next

        Me.tree = tree.root
    End Sub

    Public Function GetValueByName(name As String) As IEnumerable(Of T)
        Dim node As BinaryTree(Of String, T) = tree.Find(name, AddressOf String.Compare)

        If Not node Is Nothing Then
            Return node!values
        Else
            Return {}
        End If
    End Function
End Class
