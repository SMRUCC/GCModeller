#Region "Microsoft.VisualBasic::b476b27534f941a4a1682a53cddc8e2e, visualize\Cytoscape\Cytoscape\Graph\Xgmml\NetworkMetadata.vb"

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

    '     Class InnerRDF
    ' 
    '         Properties: meta
    ' 
    '         Function: ToString
    ' 
    '     Class NetworkMetadata
    ' 
    '         Properties: [Date], Description, Format, Identifer, InteractionType
    '                     Source, Title
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
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
