#Region "Microsoft.VisualBasic::03a50492fe2e8a313020230617709063, visualize\Cytoscape\Cytoscape\Graph\Xgmml\Graph.vb"

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

'     Class Graph
' 
'         Properties: Attributes, Directed, documentVersion, Edges, Graphics
'                     ID, Label, NetworkMetaData, Nodes, Size
' 
'         Function: (+2 Overloads) CreateObject, DeleteDuplication, ExistEdge, (+2 Overloads) GetNode, GetSize
'                   Load, (+2 Overloads) Save
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.application
Imports Microsoft.VisualBasic.Text

Namespace CytoscapeGraphView.XGMML.File

    ''' <summary>
    ''' The Cytoscape software XML format network visualization model.(Cytoscape软件的网络XML模型文件)
    ''' </summary>
    ''' <remarks>请注意，由于在Cytoscape之中，每一个Xml元素都是小写字母的，所以在这个类之中的所有的Xml序列化的标记都不可以再更改大小写了</remarks>
    <XmlRoot("graph", Namespace:="http://www.cs.rpi.edu/XGMML")>
    Public Class Graph : Implements ISaveHandle

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

        <XmlElement("att")> Public Property attributes As GraphAttribute()
        <XmlElement("graphics")> Public Property graphics As Graphics
        <XmlElement("node")> Public Property Nodes As XGMMLnode()
            Get
                If _nodeList.IsNullOrEmpty Then
                    Return New XGMMLnode() {}
                End If
                Return _nodeList.Values.ToArray
            End Get
            Set(value As XGMMLnode())
                If value.IsNullOrEmpty Then
                    _nodeList = New Dictionary(Of String, XGMMLnode)
                Else
                    _nodeList = value.ToDictionary(Function(obj) obj.label)
                End If
            End Set
        End Property

        <XmlElement("edge")> Public Property Edges As XGMMLedge()

        Dim _nodeList As Dictionary(Of String, XGMMLnode)

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

        Public Sub New()
            xmlns = New XmlSerializerNamespaces

            xmlns.Add("cy", xmlnsCytoscape)
            xmlns.Add("rdf", rdf_xml.RDF.XmlnsNamespace)
            xmlns.Add("xlink", "http://www.w3.org/1999/xlink")
            xmlns.Add("dc", xmlns_dc)
        End Sub
#End Region

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Label">Synonym</param>
        ''' <returns></returns>
        Public Function GetNode(Label As String) As XGMMLnode
            Dim Node As XGMMLnode = Nothing
            Call _nodeList.TryGetValue(Label, Node)
            Return Node
        End Function

        Public Function GetNode(ID As Long) As XGMMLnode
            Return LinqAPI.DefaultFirst(Of XGMMLnode) <=
                From node As XGMMLnode
                In Me._nodeList.Values
                Where node.id = ID
                Select node
        End Function

        ''' <summary>
        ''' 使用这个方法才能够正确的加载一个cytoscape的网络模型文件
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Load(path As String) As Graph
            Dim graph As Graph = path.LoadXml(Of Graph)(preprocess:=AddressOf RDFXml.TrimRDF)
            Return graph
        End Function

        Public Function GetSize(Optional Scale As Double = 1) As Size
            Dim Max_X As Integer = (From node In Nodes.AsParallel Select node.graphics.x).Max * (Scale + 1)
            Dim Max_Y As Integer = (From node In Nodes.AsParallel Select node.graphics.y).Max * (Scale + 1)

            Return New Size(Max_X, Max_Y)
        End Function

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

        ''' <summary>
        ''' 创建一个初始默认的网络文件
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function CreateObject() As Graph
            Dim Graph As New Graph With {
                .label = "",
                .id = "",
                .directed = "1",
                .NetworkMetaData = New NetworkMetadata
            }
            Return Graph
        End Function

        Public Function ExistEdge(Edge As XGMMLedge) As Boolean
            Return Not (GetNode(Edge.source) Is Nothing OrElse GetNode(Edge.target) Is Nothing)
        End Function

        ''' <summary>
        ''' Creates a default cytoscape network model xml file with specific title and description.(创建一个初始默认的网络文件)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function CreateObject(Title As String, Type As String, Optional Description As String = "") As Graph
            Dim Graph As Graph = Graph.CreateObject

            Graph.label = Title
            Graph.NetworkMetaData.title = Title.Replace("<", "[").Replace(">", "]")
            Graph.NetworkMetaData.type = Type.Replace("<", "[").Replace(">", "]")
            Graph.NetworkMetaData.description = Description.Replace("<", "[").Replace(">", "]")
            Return Graph
        End Function

        Public Function DeleteDuplication() As Graph
            Dim sw As Stopwatch = Stopwatch.StartNew

            Call $"{NameOf(Edges)}:={Edges.Count } in the network model...".__DEBUG_ECHO
            Me.Edges = Distinct(Me.Edges)
            Call $"{NameOf(Edges)}:={Edges.Count } left after remove duplicates in {sw.ElapsedMilliseconds}ms....".__DEBUG_ECHO
            Return Me
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
            Return WriteXml(Me.GetXml, Encodings.UTF8.CodePage, FilePath)
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace
