#Region "Microsoft.VisualBasic::7c6942dc4709339b7a5034f78f5d21a4, ..\R.Bioconductor\RDotNET.Extensions.VisualBasic\Services\RPackage\Graphics\gplots\DendrogramAPI.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataStructures.BinaryTree

Namespace SymbolBuilder.packages.gplots

    Public Module DendrogramAPI

        <Extension>
        Public Function ClusterParts(Of T)(tree As BinaryTree(Of T)) As Dictionary(Of String, String())
            Return tree.ClusterParts(AddressOf IsLeaf, AddressOf IsLeafX, AddressOf GetEntities)
        End Function

        Public Function IsLeaf(Of T)(x As TreeNode(Of T)) As Boolean
            Throw New NotImplementedException
        End Function

        Public Function IsLeafX(Of T)(x As TreeNode(Of T)) As Boolean
            Throw New NotImplementedException
        End Function

        Public Function GetEntities(Of T)(x As TreeNode(Of T)) As String()
            Throw New NotImplementedException
        End Function
    End Module
End Namespace
