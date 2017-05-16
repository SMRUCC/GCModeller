#Region "Microsoft.VisualBasic::67231c73814b369e78a91eebb2fa69bd, ..\GCModeller\models\Networks\Network.Regulons\RegulatesGraph\GraphNode.vb"

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

Imports Microsoft.VisualBasic.Data.visualize.Network

Namespace RegulatesGraph

    Public Class Entity : Inherits FileStream.Node

        ''' <summary>
        ''' 代谢途径  => 有多少个基因
        ''' 调控位点家族 => 调控多少个基因
        ''' </summary>
        ''' <returns></returns>
        Public Property Size As Integer
    End Class

    Public Class PathwayRegulates : Inherits FileStream.NetworkEdge

        ''' <summary>
        ''' 位点是通过这个基因列表来调控代谢途径的
        ''' </summary>
        ''' <returns></returns>
        Public Property Regulates As String

        Sub New()
            Me.Interaction = "regulates"
        End Sub
    End Class
End Namespace
