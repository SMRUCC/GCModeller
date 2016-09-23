#Region "Microsoft.VisualBasic::6febb92e2534240c55a1c1569c2b3a44, ..\interops\visualize\Cytoscape\Cytoscape\Graph\Xgmml\NetworkMetadata.vb"

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

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.RDF
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace CytoscapeGraphView.XGMML

    Public Class InnerRDF

        <XmlElement("rdf-Description")> Public Property meta As NetworkMetadata

        Public Overrides Function ToString() As String
            Return meta.GetJson
        End Function
    End Class

    Public Class NetworkMetadata : Inherits RDFEntity

        ''' <summary>
        ''' 节点之间互作的类型
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("dc-type")> Public Property InteractionType As String = "Protein-Protein Interaction"
        <XmlElement("dc-description")> Public Property Description As String = "N/A"
        <XmlElement("dc-identifier")> Public Property Identifer As String = "N/A"
        <XmlElement("dc-date")> Public Property [Date] As String

        ''' <summary>
        ''' 网络模型的名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("dc-title")> Public Property Title As String = "Default Network Title"
        <XmlElement("dc-source")> Public Property Source As String = "http://GCModeller.org/"
        <XmlElement("dc-format")> Public Property Format As String = "Cytoscape-XGMML"

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
