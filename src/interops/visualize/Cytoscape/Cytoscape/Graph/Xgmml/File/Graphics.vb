#Region "Microsoft.VisualBasic::fa99d054671721837d93fab4111bc83e, visualize\Cytoscape\Cytoscape\Graph\Xgmml\File\Graphics.vb"

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

    '     Class Graphics
    ' 
    '         Properties: ScaleFactor
    ' 
    '         Function: DefaultValue
    ' 
    '     Class GraphAttribute
    ' 
    '         Properties: RDF
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml

Namespace CytoscapeGraphView.XGMML.File

    Public Class Graphics : Inherits AttributeDictionary

        Public ReadOnly Property ScaleFactor As Double
            Get
                Dim attr = Me("NETWORK_SCALE_FACTOR")
                If attr Is Nothing Then
                    Return 1
                Else
                    Return Val(attr.Value)
                End If
            End Get
        End Property

        Public Shared Function DefaultValue() As Graphics
            Dim attrs As Attribute() = {
                Attribute.StringValue([NameOf].ATTR_NETWORK_GRAPHICS_HEIGHT, "892.0"),
                Attribute.StringValue([NameOf].ATTR_NETWORK_GRAPHICS_CENTER_Z_LOCATION, "0.0"),
                Attribute.StringValue([NameOf].ATTR_NETWORK_GRAPHICS_EDGE_SELECTION, "true"),
                Attribute.StringValue([NameOf].ATTR_NETWORK_GRAPHICS_BACKGROUND_PAINT, "#EBE8E1"),
                Attribute.StringValue([NameOf].ATTR_NETWORK_GRAPHICS_NETWORK_WIDTH, "1554.0"),
                Attribute.StringValue([NameOf].ATTR_NETWORK_GRAPHICS_CENTER_Y_LOCATION, "104.00000095367432"),
                Attribute.StringValue([NameOf].ATTR_NETWORK_GRAPHICS_NODE_SELECTION, "true"),
                Attribute.StringValue([NameOf].ATTR_NETWORK_GRAPHICS_SCALE_FACTOR, "3.4140904692008895"),
                Attribute.StringValue([NameOf].ATTR_NETWORK_GRAPHICS_NETWORK_DEPTH, "0.0"),
                Attribute.StringValue([NameOf].ATTR_NETWORK_GRAPHICS_TITLE, "")
            }

            Return New Graphics With {
                .attributes = attrs
            }
        End Function
    End Class

    ''' <summary>
    ''' 全局的属性
    ''' </summary>
    Public Class GraphAttribute : Inherits Attribute

        ''' <summary>
        ''' RDF的描述数据
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("RDF", [Namespace]:=RDFEntity.XmlnsNamespace)>
        Public Property RDF As InnerRDF

    End Class
End Namespace
