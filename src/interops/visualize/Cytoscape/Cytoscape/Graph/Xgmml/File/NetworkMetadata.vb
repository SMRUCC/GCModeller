#Region "Microsoft.VisualBasic::a2443f7a5cdc942f675b23b45d16b738, visualize\Cytoscape\Cytoscape\Graph\Xgmml\File\NetworkMetadata.vb"

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
    '         Properties: [date], description, format, identifer, source
    '                     title, type
    ' 
    '         Function: createAttribute, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace CytoscapeGraphView.XGMML.File

    <XmlType("RDF", [Namespace]:=RDFEntity.XmlnsNamespace)>
    Public Class InnerRDF

        <XmlElement("Description", [Namespace]:=RDFEntity.XmlnsNamespace)>
        Public Property meta As NetworkMetadata

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
        <XmlElement("type", [Namespace]:=XGMMLgraph.xmlns_dc)> Public Property type As String = "Protein-Protein Interaction"
        <XmlElement("description", [Namespace]:=XGMMLgraph.xmlns_dc)> Public Property description As String = "N/A"
        <XmlElement("identifier", [Namespace]:=XGMMLgraph.xmlns_dc)> Public Property identifer As String = "N/A"
        <XmlElement("date", [Namespace]:=XGMMLgraph.xmlns_dc)> Public Property [date] As String

        ''' <summary>
        ''' 网络模型的名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("title", [Namespace]:=XGMMLgraph.xmlns_dc)> Public Property title As String = "Default Network Title"
        <XmlElement("source", [Namespace]:=XGMMLgraph.xmlns_dc)> Public Property source As String = "http://GCModeller.org/"
        <XmlElement("format", [Namespace]:=XGMMLgraph.xmlns_dc)> Public Property format As String = "Cytoscape-XGMML"

        Friend Shared Function createAttribute(Optional title$ = "Default Network Title", Optional description$ = "GCModeller generated network model") As GraphAttribute
            Return New GraphAttribute With {
                .name = NameOf(XGMMLgraph.networkMetadata),
                .RDF = New InnerRDF With {
                    .meta = New NetworkMetadata With {
                        .about = "http://www.cytoscape.org/",
                        .title = title,
                        .description = description
                    }
                }
            }
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
