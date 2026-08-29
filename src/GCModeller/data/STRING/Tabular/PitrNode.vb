Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream

Namespace Tabular

    ''' <summary>
    ''' STRING网络的Csv数据文件存储
    ''' </summary>
    Public Class PitrNode : Inherits NetworkEdge

        <XmlAttribute("Node_a"), Column("fromNode")>
        Public Overrides Property FromNode As String
        <XmlAttribute("Node_b")> <Column("toNode")>
        Public Overrides Property ToNode As String
        <XmlAttribute("confidence")> <Column("confidence")>
        Public Overrides Property value As Double

        Public Overrides Function ToString() As String
            Return $"{FromNode} <---> {ToNode}; {value}"
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
End Namespace