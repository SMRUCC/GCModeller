#Region "Microsoft.VisualBasic::24df4dd17070aae51e4f9e23821aea88, ..\interops\visualize\Cytoscape\Cytoscape\API\ImportantNodes\LDM.vb"

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

Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace API.ImportantNodes

    ''' <summary>
    ''' 从footprint之中导出来的Cytoscape的网络数据文件
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Regulations
        Public Property Regulator As String
        Public Property ORF As String
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
