#Region "Microsoft.VisualBasic::7476763617e251053b2c8b664ec73c95, modules\Knowledge_base\ncbi_kb\BITS\p.vb"

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


    ' Code Statistics:

    '   Total Lines: 84
    '    Code Lines: 55 (65.48%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 29 (34.52%)
    '     File Size: 2.42 KB


    '     Class Paragraph
    ' 
    '         Properties: bold, italic, links, related_object, text
    '                     xref
    ' 
    '         Function: GetTextContent, ToString
    ' 
    '     Class xref
    ' 
    '         Properties: ref_type, rid, text
    ' 
    '     Class Italic
    ' 
    '         Properties: toggle
    ' 
    '         Function: ToString
    ' 
    '     Class Bold
    ' 
    '         Function: ToString
    ' 
    '     Class ExtLink
    ' 
    '         Properties: ext_link_type, href, text
    ' 
    '         Function: ToString
    ' 
    '     Class RelatedObject
    ' 
    '         Properties: document_id, document_type, link_type, source_id, text
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace BITS
    Public Class Paragraph

        <XmlText> Public Property text As String()

        <XmlElement("related-object")>
        Public Property related_object As RelatedObject()

        <XmlElement> Public Property bold As Bold()

        <XmlElement("ext-link")> Public Property links As ExtLink()
        <XmlElement("italic")> Public Property italic As Italic()

        <XmlElement> Public Property xref As xref()

        Public Function GetTextContent() As String
            Return text.JoinBy(" ")
        End Function

        Public Overrides Function ToString() As String
            Return text.JoinBy(" ")
        End Function

    End Class

    Public Class xref

        <XmlAttribute("ref-type")> Public Property ref_type As String
        <XmlAttribute> Public Property rid As String
        <XmlText> Public Property text As String

    End Class

    Public Class Italic : Inherits Paragraph

        <XmlAttribute> Public Property toggle As String

        Public Overrides Function ToString() As String
            Return GetTextContent()
        End Function

    End Class

    Public Class Bold : Inherits Paragraph

        Public Overrides Function ToString() As String
            Return GetTextContent()
        End Function

    End Class

    <XmlType("ext-link")>
    Public Class ExtLink

        <XmlAttribute("ext-link-type")> Public Property ext_link_type As String
        <XmlAttribute("href", [Namespace]:=xlink)> Public Property href As String

        Public Const xlink As String = "http://www.w3.org/1999/xlink"

        <XmlText> Public Property text As String

        Public Overrides Function ToString() As String
            Return $"[{text}]({href})"
        End Function
    End Class

    <XmlType("related-object")>
    Public Class RelatedObject

        <XmlAttribute("link-type")> Public Property link_type As String
        <XmlAttribute("source-id")> Public Property source_id As String
        <XmlAttribute("document-id")> Public Property document_id As String
        <XmlAttribute("document-type")> Public Property document_type As String

        <XmlText> Public Property text As String

        Public Overrides Function ToString() As String
            Return source_id
        End Function

    End Class
End Namespace
