#Region "Microsoft.VisualBasic::4c62b148d67950d545f0fef8b5d932e4, ..\interops\visualize\Cytoscape\Cytoscape\Graph\Cyjs.vb"

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

Imports System.Text
Imports System.Web.Script.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.Abstract
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML

Namespace CytoscapeGraphView.Cyjs

    ''' <summary>
    ''' 网络模型的JSON文档格式，WebApp的网络数据模型的json文件
    ''' </summary>
    Public Class Cyjs : Implements ISaveHandle

        Public Property format_version As String = "1.0"
        Public Property generated_by As String = "GCModeller-3.2.1"
        Public Property target_cytoscapejs_version As String = "~2.1"
        Public Property data As Data
        Public Property elements As Network

        Public Function ToGraphModel() As Graph
            If elements Is Nothing Then
                elements = New Network
            End If

            Dim Graph = Serialization.Export(Of NodeData, EdgeData)(
                elements.nodes.ToArray(Function(x) x.data),
                elements.edges.ToArray(Function(x) x.data),
                data.shared_name)
            Return Graph
        End Function

        Public Function ToNetworkGraph() As NetworkGraph

        End Function

        Public Overrides Function ToString() As String
            Return __json()
        End Function

        Protected Function __json() As String
            Dim json As New StringBuilder(GetJson())

            If Me.elements Is Nothing Then
                elements = New Network
            End If
            If Me.elements.nodes Is Nothing Then
                Me.elements.nodes = New Node() {}
            End If
            If Me.elements.edges Is Nothing Then
                Me.elements.edges = New Edge() {}
            End If

            For Each node In Me.elements.nodes
                Call CytoscapeGraphView.Cyjs.Data.Replace(json, node.data)
            Next
            For Each edge In Me.elements.edges
                Call CytoscapeGraphView.Cyjs.Data.Replace(json, edge.data)
            Next

            Return json.ToString
        End Function

        Public Function Save(Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean Implements ISaveHandle.Save
            Return __json.SaveTo(Path, encoding)
        End Function

        Public Function Save(Optional Path As String = "", Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(Path, encoding.CodePage)
        End Function
    End Class

    Public Class Data : Implements IDynamicsProperty

        Public Property selected As Boolean
        Public Property __Annotations As String()
        Public Property shared_name As String = ""
        Public Property SUID As String
        Public Property name As String = ""
        Public Property id As String

        <ScriptIgnore>
        Public Property attributes As Dictionary(Of String, String) Implements IDynamicsProperty.attributes
        Public Property DynamicsSlot As String Implements IDynamicsProperty.DynamicsSlot

        Sub New()
            DynamicsSlot = CStr(Me.GetHashCode)
        End Sub

        Public Function GetAttrJson() As String
            Dim jbr As New StringBuilder

            For Each attr In attributes
                Call jbr.AppendLine($"""{attr.Key}"" : ""{attr.Value}""")
            Next

            Return jbr.ToString
        End Function

        Public Shared Function Replace(ByRef jbr As StringBuilder, docNode As Data) As StringBuilder
            Dim key As String = $"""{NameOf(DynamicsSlot)}"" : ""{CStr(docNode.GetHashCode)}"""
            Call jbr.Replace(key, docNode.GetAttrJson)
            Return jbr
        End Function
    End Class

    Public Class Network
        Public Property nodes As Node()
        Public Property edges As Edge()
    End Class

    Public Class Edge
        Public Property data As EdgeData
        Public Property selected As Boolean
    End Class

    Public Class EdgeData : Inherits Data
        Implements INetworkEdge

        Public Property source As String Implements IInteraction.source
        Public Property target As String Implements IInteraction.target
        Public Property Confidence As Double Implements INetworkEdge.value
        Public Property EdgeBetweenness As Double
        Public Property interaction As String Implements INetworkEdge.Interaction
        Public Property shared_interaction As String
        Public Property SelfLoop As Boolean
    End Class

    Public Class Node
        Public Property data As NodeData
        Public Property position As position
        Public Property selected As Boolean
    End Class

    Public Class NodeData : Inherits Data
        Implements INode
        Implements IDynamicsProperty

        Public Property NeighborhoodConnectivity As Double
        Public Property NumberOfDirectedEdges As Integer
        Public Property Stress As Integer
        Public Property SelfLoops As Integer
        Public Property IsSingleNode As Boolean
        Public Property PartnerOfMultiEdgedNodePairs As Integer
        Public Property NodeType As String Implements INode.NodeType
        Public Property Degree As Integer
        Public Property TopologicalCoefficient As Double
        Public Property BetweennessCentrality As Double
        Public Property Radiality As Double
        Public Property Eccentricity As Integer
        Public Property NumberOfUndirectedEdges As Integer
        Public Property ClosenessCentrality As Double
        Public Property AverageShortestPathLength As Double
        Public Property ClusteringCoefficient As Double
        Public Property Size As Integer

        Public Property Identifer As String Implements INode.ID
            Get
                Return name
            End Get
            Set(value As String)
                name = value
            End Set
        End Property
    End Class

    Public Class position
        Public Property x As Double
        Public Property y As Double
    End Class

    Public Interface IDynamicsProperty
        Property DynamicsSlot As String
        Property attributes As Dictionary(Of String, String)
    End Interface
End Namespace
