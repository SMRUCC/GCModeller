Imports System.Xml.Serialization
Imports SMRUCC.genomics.DatabaseServices.StringDB.MIF25
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace StringDB.SimpleCsv

    Public Class PitrNode : Inherits DataVisualization.Network.FileStream.NetworkEdge

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

        <XmlElement("Node")> Public Property Nodes As StringDB.SimpleCsv.PitrNode()

        Public Function GetConfidence(Node1 As String, Node2 As String) As Double
            Dim LQuery = (From edge As PitrNode
                          In Nodes
                          Where edge.Contains(Node1) AndAlso
                              edge.Contains(Node2)
                          Select edge).ToArray
            If LQuery.IsNullOrEmpty Then
                Return 0
            Else
                Return LQuery.First.Confidence
            End If
        End Function

        Public Shared Function Compile(DBDir As String) As Network
            Return New Network With {
                .Nodes = EntrySet.ExtractNetwork(DBDir)
            }
        End Function
    End Class
End Namespace