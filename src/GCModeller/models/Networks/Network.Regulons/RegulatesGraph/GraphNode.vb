#Region "Microsoft.VisualBasic::645364469f50e49c391de0e745d0e3f4, models\Networks\Network.Regulons\RegulatesGraph\GraphNode.vb"

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

    '     Class Entity
    ' 
    '         Properties: Size
    ' 
    '     Class PathwayRegulates
    ' 
    '         Properties: Regulates
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream

Namespace RegulatesGraph

    Public Class Entity : Inherits Node

        ''' <summary>
        ''' 代谢途径  => 有多少个基因
        ''' 调控位点家族 => 调控多少个基因
        ''' </summary>
        ''' <returns></returns>
        Public Property Size As Integer
    End Class

    Public Class PathwayRegulates : Inherits NetworkEdge

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
