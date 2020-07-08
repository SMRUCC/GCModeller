#Region "Microsoft.VisualBasic::31fa3ad317a3f96fc84ee0b37de872d8, Network.BLAST\DataModels\Protein.vb"

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

    '     Class Protein
    ' 
    '         Properties: Family, Genome, LocusId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.Abstract

Namespace LDM

    ''' <summary>
    ''' 相当于网络之中的一个节点
    ''' </summary>
    Public Class Protein : Implements INode

        Public Property LocusId As String Implements INode.Id
        ''' <summary>
        ''' 家族分类
        ''' </summary>
        ''' <returns></returns>
        Public Property Family As String Implements INode.NodeType
        ''' <summary>
        ''' 这个蛋白质所处的基因组的名称
        ''' </summary>
        ''' <returns></returns>
        Public Property Genome As String
    End Class
End Namespace
