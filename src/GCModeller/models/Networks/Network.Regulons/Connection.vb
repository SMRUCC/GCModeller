#Region "Microsoft.VisualBasic::3ba80f912ff14a0db9e773a92a652304, models\Networks\Network.Regulons\Connection.vb"

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


    ' Code Statistics:

    '   Total Lines: 16
    '    Code Lines: 10 (62.50%)
    ' Comment Lines: 3 (18.75%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (18.75%)
    '     File Size: 637 B


    ' Class Connection
    ' 
    '     Properties: cor, gene1, gene2, interaction, is_directly
    '                 pval
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.GraphTheory.SparseGraph

''' <summary>
''' the correlation connection between two genes
''' </summary>
Public Class Connection : Implements IInteraction, INetworkEdge

    Public Property gene1 As String Implements IInteraction.source
    Public Property gene2 As String Implements IInteraction.target
    Public Property is_directly As Boolean
    Public Property cor As Double Implements INetworkEdge.value
    Public Property pval As Double
    Public Property interaction As String Implements INetworkEdge.Interaction

End Class
