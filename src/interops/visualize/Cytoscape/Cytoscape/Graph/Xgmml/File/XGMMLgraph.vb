#Region "Microsoft.VisualBasic::1eb15b47aab97d0de147c6d4f0c76c36, visualize\Cytoscape\Cytoscape\Graph\Xgmml\File\XGMMLgraph.vb"

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

    '     Class XGMMLgraph
    ' 
    '         Properties: attributes, directed, documentVersion, edges, graphics
    '                     id, label, networkMetadata, nodes, Size
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) CreateObject, GetNodeIndex, GetSize, (+2 Overloads) Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports Microsoft.VisualBasic.Text

Namespace CytoscapeGraphView.XGMML.File

    ''' <summary>
    ''' The Cytoscape software XML format network visualization model.(Cytoscape软件的网络XML模型文件)
    ''' </summary>
    ''' <remarks>请注意，由于在Cytoscape之中，每一个Xml元素都是小写字母的，所以在这个类之中的所有的Xml序列化的标记都不可以再更改大小写了</remarks>
    <XmlRoot("graph", Namespace:="http://www.cs.rpi.edu/XGMML")>
    Public Class XGMMLgraph : Implements ISaveHandle

#Region "Assembly File Public Properties"

        <XmlAttribute("id")> Public Property id As String
        ''' <summary>
        ''' The brief title information of this cytoscape network model.(这个Cytoscape网络模型文件的摘要标题信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute("label")> Public Property label As String
        ''' <summary>
        ''' The edges between these nodes have the direction from one node to another node?
        ''' (这个网络模型文件之中的相互作用的节点之间的边是否是具有方向性的)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute("directed")> Public Property directed As String
        <XmlAttribute("documentVersion", [Namespace]:=xmlnsCytoscape)>
        Public Property documentVersion As String = "3.0"

        ''' <summary>
        ''' 在这个属性里面会自动设置Graph对象的属性列表里面的数据
        ''' </summary>
        ''' <returns></returns>
        <XmlIgnore>
        Public ReadOnly Property networkMetadata As NetworkMetadata
            Get
                Return attributes _
                    .Where(Function(a) a.name = "networkMetadata") _
                    .FirstOrDefault() _
                    .RDF _
                    .meta
            End Get
        End Property

        ''' <summary>
        ''' the graph attributes
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("att")> Public Property attributes As GraphAttribute()
        <XmlElement("graphics")> Public Property graphics As Graphics
        <XmlElement("node")> Public Property nodes As XGMMLnode()
        <XmlElement("edge")> Public Property edges As XGMMLedge()

        <XmlNamespaceDeclarations()>
        Public xmlns As XmlSerializerNamespaces

        ''' <summary>
        ''' cy:xxx
        ''' </summary>
        Public Const xmlnsCytoscape$ = "http://www.cytoscape.org"
        ''' <summary>
        ''' dc:xxx
        ''' </summary>
        Public Const xmlns_dc$ = "http://purl.org/dc/elements/1.1/"

        Public ReadOnly Property Size As Size
            Get
                Dim width = Me.graphics("NETWORK_WIDTH")
                Dim height = Me.graphics("NETWORK_HEIGHT")

                If width Is Nothing OrElse height Is Nothing Then
                    Return GetSize()
                Else
                    Return New Size(Val(width.Value), Val(height.Value))
                End If
            End Get
        End Property

        Public Sub New()
            xmlns = New XmlSerializerNamespaces

            xmlns.Add("cy", xmlnsCytoscape)
            xmlns.Add("rdf", RDFEntity.XmlnsNamespace)
            xmlns.Add("xlink", "http://www.w3.org/1999/xlink")
            xmlns.Add("dc", xmlns_dc)
        End Sub
#End Region

        Public Function GetSize(Optional Scale As Double = 1) As Size
            Dim Max_X As Integer = (From node In nodes.AsParallel Select node.graphics.x).Max * (Scale + 1)
            Dim Max_Y As Integer = (From node In nodes.AsParallel Select node.graphics.y).Max * (Scale + 1)

            Return New Size(Max_X, Max_Y)
        End Function

        Public Function GetNodeIndex() As GraphIndex
            Return New GraphIndex(Me)
        End Function

        ''' <summary>
        ''' 创建一个初始默认的网络文件
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function CreateObject() As XGMMLgraph
            Dim g As New XGMMLgraph With {
                .label = "",
                .id = "",
                .directed = "1",
                .attributes = {NetworkMetadata.createAttribute}
            }
            Return g
        End Function

        ''' <summary>
        ''' Creates a default cytoscape network model xml file with specific title and description.(创建一个初始默认的网络文件)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function CreateObject(Title As String, Type As String, Optional Description As String = "") As XGMMLgraph
            Dim Graph As XGMMLgraph = XGMMLgraph.CreateObject

            Graph.label = Title
            Graph.networkMetadata.title = Title.Replace("<", "[").Replace(">", "]")
            Graph.networkMetadata.type = Type.Replace("<", "[").Replace(">", "]")
            Graph.networkMetadata.description = Description.Replace("<", "[").Replace(">", "]")
            Return Graph
        End Function

        ''' <summary>
        ''' Save this cytoscape network visualization model using this function.
        ''' (请使用这个方法进行Cytoscape网络模型文件的保存)
        ''' </summary>
        ''' <param name="FilePath">The file path of the xml file saved location.</param>
        ''' <param name="encoding">The text encoding of saved text file.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Save(FilePath As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Return RDFXml.WriteXml(Me, Encodings.UTF8.CodePage, FilePath)
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace
