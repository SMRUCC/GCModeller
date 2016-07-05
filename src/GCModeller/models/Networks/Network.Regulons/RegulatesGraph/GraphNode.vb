#Region "Microsoft.VisualBasic::5373ba9e78596a5cb398268cb1811238, ..\GCModeller\models\Networks\Network.Regulons\RegulatesGraph\GraphNode.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataVisualization.Network
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView

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
            Me.InteractionType = "regulates"
        End Sub
    End Class
End Namespace
