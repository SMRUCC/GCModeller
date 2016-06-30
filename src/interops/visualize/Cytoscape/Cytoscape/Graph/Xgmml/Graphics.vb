Imports System.Xml.Serialization

Namespace CytoscapeGraphView.XGMML

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
                .Attributes = attrs
            }
        End Function
    End Class

    Public Class GraphAttribute : Inherits Attribute

        ''' <summary>
        ''' RDF的描述数据
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("rdf-RDF")> Public Property RDF As InnerRDF
    End Class
End Namespace