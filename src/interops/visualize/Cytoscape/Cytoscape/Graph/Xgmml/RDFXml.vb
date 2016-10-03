#Region "Microsoft.VisualBasic::8cfaa2f3c70528425a0eef896175082d, ..\interops\visualize\Cytoscape\Cytoscape\Graph\Xgmml\RDFXml.vb"

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
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Text.Xml

Namespace CytoscapeGraphView.XGMML

    Module RDFXml

        Public Function TrimRDF(xml As String) As String
            Dim sb As New StringBuilder(xml)

            Call sb.Replace(" cy:", " cy-")
            Call sb.Replace("rdf:", "rdf-")
            Call sb.Replace("<dc:", "<dc-")
            Call sb.Replace("/dc:", "/dc-")

            xml = sb.ToString

            Return xml
        End Function

        Const XGMML As String = "http://www.cs.rpi.edu/XGMML"
        Const dc As String = "http://purl.org/dc/elements/1.1/"
        Const xlink As String = "http://www.w3.org/1999/xlink"
        Const rdf As String = "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
        Const cy As String = "http://www.cytoscape.org"

        Public Function WriteXml(graph As Graph, encoding As Encoding, path As String) As Boolean
            If graph.NetworkMetaData Is Nothing Then
                graph.NetworkMetaData = New NetworkMetadata With {
                    .about = "http://www.cytoscape.org/"
                }
            Else
                graph.NetworkMetaData.about = "http://www.cytoscape.org/"
            End If
            Return WriteXml(graph.GetXml, encoding, path)
        End Function

        Public Function WriteXml(xml As String, encoding As Encoding, path As String) As Boolean
            Dim doc As New XmlDoc(xml)

            doc.encoding = XmlEncodings.UTF8
            doc.standalone = True
            doc.version = "1.0"
            doc.xmlns.xmlns = XGMML
            doc.xmlns.xsd = ""
            doc.xmlns.xsi = ""
            doc.xmlns.Set(NameOf(dc), dc)
            doc.xmlns.Set(NameOf(xlink), xlink)
            doc.xmlns.Set(NameOf(rdf), rdf)
            doc.xmlns.Set(NameOf(cy), cy)

            Dim sb As New StringBuilder(doc.ToString)

            Call sb.Replace(" cy-", " cy:")
            Call sb.Replace("rdf-", "rdf:")
            Call sb.Replace("<dc-", "<dc:")
            Call sb.Replace("/dc-", "/dc:")

            Return sb.SaveTo(path, encoding)
        End Function
    End Module
End Namespace
