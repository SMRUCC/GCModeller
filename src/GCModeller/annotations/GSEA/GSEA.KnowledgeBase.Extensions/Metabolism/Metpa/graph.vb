#Region "Microsoft.VisualBasic::b52465a5b8005cbb6b181270e1861357, G:/GCModeller/src/GCModeller/annotations/GSEA/GSEA.KnowledgeBase.Extensions//Metabolism/Metpa/graph.vb"

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

    '   Total Lines: 47
    '    Code Lines: 31
    ' Comment Lines: 4
    '   Blank Lines: 12
    '     File Size: 1.29 KB


    '     Class graph
    ' 
    '         Properties: edges, name, nodes
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) Create
    ' 
    '     Class graphList
    ' 
    '         Properties: graphs
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports g = Microsoft.VisualBasic.Data.visualize.Network.Graph.NetworkGraph

Namespace Metabolism.Metpa

    Public Class graph

        Public Property nodes As Node()
        Public Property edges As NetworkEdge()
        Public Property name As String

        Sub New()
        End Sub

        ''' <summary>
        ''' create a network graph model from the table data
        ''' </summary>
        ''' <returns></returns>
        Public Function Create() As g
            Dim network As New NetworkTables With {
                .edges = edges,
                .nodes = nodes
            }
            Dim g As g = network.CreateGraph

            Return g
        End Function

        Public Shared Function Create(g As g, name As String) As graph
            Dim table As NetworkTables = g.Tabular({"*"}, meta:=New MetaData)
            Dim graph As New graph With {
                .edges = table.edges,
                .nodes = table.nodes,
                .name = name
            }

            Return graph
        End Function

    End Class

    Public Class graphList

        Public Property graphs As Dictionary(Of String, graph)

    End Class
End Namespace
