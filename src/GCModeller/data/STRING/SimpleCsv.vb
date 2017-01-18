#Region "Microsoft.VisualBasic::d3cd421e1c9b6e2b0923dc5d7d276918, ..\GCModeller\data\ExternalDBSource\string-db\SimpleCsv.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language

Namespace SimpleCsv

    Public Class PitrNode : Inherits NetworkEdge

        <XmlAttribute("Node_a")> <Column("fromNode")>
        Public Overrides Property FromNode As String
        <XmlAttribute("Node_b")> <Column("toNode")>
        Public Overrides Property ToNode As String
        <XmlAttribute("confidence")> <Column("confidence")>
        Public Overrides Property Confidence As Double

        Public Overrides Function ToString() As String
            Return $"{FromNode} <---> {ToNode}; {Confidence}"
        End Function

        ''' <summary>
        ''' 返回空字符串表示标识符不是这个节点边两端的实体对象
        ''' </summary>
        ''' <param name="NodeId"></param>
        ''' <returns></returns>
        Public Function GetInteractNode(NodeId As String) As String
            If String.Equals(NodeId, FromNode) Then
                Return ToNode
            ElseIf String.Equals(NodeId, ToNode) Then
                Return FromNode
            Else
                Return ""
            End If
        End Function
    End Class

    <XmlRoot("Interaction_Network", Namespace:="http://code.google.com/p/genome-in-code/interaction_network/")>
    Public Class Network

        <XmlElement("Node")> Public Property Nodes As SimpleCsv.PitrNode()

        Public Function GetConfidence(Node1 As String, Node2 As String) As Double
            Dim LQuery As PitrNode = LinqAPI.DefaultFirst(Of PitrNode) <=
 _
                From edge As PitrNode
                In Nodes
                Where edge.Contains(Node1) AndAlso
                    edge.Contains(Node2)
                Select edge

            If LQuery Is Nothing Then
                Return 0
            Else
                Return LQuery.Confidence
            End If
        End Function

        Public Shared Function Compile([imports] As String) As Network
            Return New Network With {
                .Nodes = PSI.ExtractNetwork([imports])
            }
        End Function
    End Class
End Namespace
