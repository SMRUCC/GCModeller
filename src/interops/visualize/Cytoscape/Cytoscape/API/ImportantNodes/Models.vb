﻿#Region "Microsoft.VisualBasic::d1b1b8683cc688673b964634a5b47140, visualize\Cytoscape\Cytoscape\API\ImportantNodes\Models.vb"

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

    '   Total Lines: 46
    '    Code Lines: 25 (54.35%)
    ' Comment Lines: 10 (21.74%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 11 (23.91%)
    '     File Size: 1.48 KB


    '     Class Regulations
    ' 
    '         Properties: ORF, Regulator
    ' 
    '     Class RankRegulations
    ' 
    '         Properties: GeneCluster, RankScore, Regulators
    ' 
    '         Function: ToString
    ' 
    '     Class NodeRank
    ' 
    '         Properties: Nodes, Rank
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.GraphTheory.SparseGraph
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace API.ImportantNodes

    ''' <summary>
    ''' 从footprint之中导出来的Cytoscape的网络数据文件
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Regulations : Implements IInteraction

        Public Property Regulator As String Implements IInteraction.source
        Public Property ORF As String Implements IInteraction.target

    End Class

    Public Class RankRegulations

        ''' <summary>
        ''' 得分越高越重要
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RankScore As Integer
        Public Property Regulators As String()
        Public Property GeneCluster As String()

        Public Overrides Function ToString() As String
            Return String.Format("Rank.Score={0}; GeneCluster={1}; Regulators={2}", RankScore, String.Join(", ", GeneCluster), String.Join(", ", Regulators))
        End Function
    End Class

    Public Class NodeRank

        Public Property Rank As Integer

        <Collection("ImportantNodes")>
        Public Property Nodes As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
