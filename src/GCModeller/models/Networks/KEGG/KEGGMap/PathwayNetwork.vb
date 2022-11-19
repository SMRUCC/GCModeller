#Region "Microsoft.VisualBasic::d5e983bcc0fceb189cb942ecc3ee43b1, GCModeller\models\Networks\KEGG\KEGGMap\PathwayNetwork.vb"

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

    '   Total Lines: 45
    '    Code Lines: 31
    ' Comment Lines: 7
    '   Blank Lines: 7
    '     File Size: 1.48 KB


    ' Module PathwayNetwork
    ' 
    '     Function: BuildNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module PathwayNetwork

    ''' <summary>
    ''' 在这里创建的是map与map之间通过所拥有的交集代谢物来创建边连接
    ''' 生成一幅网络概览图
    ''' </summary>
    ''' <param name="ref"></param>
    ''' <param name="nodeValue"></param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildNetwork(ref As IEnumerable(Of Map), nodeValue As Action(Of Node)) As NetworkGraph
        Dim graph As New NetworkGraph
        Dim maps As Map() = ref.ToArray

        For Each map As Map In maps
            Call nodeValue(graph.CreateNode(map.ID))
        Next

        For Each A As Map In maps
            Dim compoundsA = A _
                .GetMembers _
                .Where(Function(id) id.IsPattern("C\d+")) _
                .ToArray

            For Each B In maps.Where(Function(bb) Not A Is bb)
                With B.GetMembers _
                    .Where(Function(id) id.IsPattern("C\d+")) _
                    .Intersect(compoundsA) _
                    .ToArray

                    If Not .IsNullOrEmpty Then
                        Dim edge As Edge = graph.CreateEdge(A.ID, B.ID)
                        edge.Weight = .Length
                    End If
                End With
            Next
        Next

        Return graph
    End Function
End Module
