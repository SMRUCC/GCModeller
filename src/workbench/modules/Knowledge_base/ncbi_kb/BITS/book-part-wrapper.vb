#Region "Microsoft.VisualBasic::4413477979a8417b08336ac034a2738b, modules\Knowledge_base\ncbi_kb\BITS\book-part-wrapper.vb"

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

    '   Total Lines: 101
    '    Code Lines: 55 (54.46%)
    ' Comment Lines: 27 (26.73%)
    '    - Xml Docs: 88.89%
    ' 
    '   Blank Lines: 19 (18.81%)
    '     File Size: 3.43 KB


    '     Class BookPartWrapper
    ' 
    '         Properties: book_meta, book_part, content_type, dtd_version, from_where
    '                     id
    ' 
    '         Function: (+2 Overloads) GetCitations, PreprocessingXml, ToString
    ' 
    '     Class BookMeta
    ' 
    '         Properties: book_id, book_title_group
    ' 
    '     Class bookTitleGroup
    ' 
    '         Properties: book_title, subtitle
    ' 
    '     Class bookId
    ' 
    '         Properties: book_id_type, id
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace BITS

    ''' <summary>
    ''' the xml definition of the ncbi book data
    ''' </summary>
    ''' <remarks>
    ''' A set of Book Interchange Tag Suite (BITS)
    ''' DTD modules was written as the basis for
    ''' publishing, interchange, and repository
    ''' book DTDs, with the intention that DTDs for
    ''' specific purposes, such as this Book
    ''' DTD, would be developed based on them.
    ''' 
    ''' This Book Interchange DTD has been developed
    ''' from the ANSI/NISO JATS Z39.96 DTD modules,
    ''' in the approved manner, making changes to the
    ''' declarations in those modules by over-riding
    ''' Parameter Entity contents. These overrides
    ''' are defined in the three BITS book
    ''' customization modules:
    ''' 
    ''' ```
    '''     %bookcustom-classes.ent;
    '''     %bookcustom-mixes.ent;
    '''     %bookcustom-models.ent;
    ''' ```
    ''' 
    ''' which are called from this DTD file.
    ''' </remarks>
    <XmlType("book-part-wrapper")>
    Public Class BookPartWrapper

        <XmlAttribute("id")> Public Property id As String
        <XmlAttribute("content-type")> Public Property content_type As String
        <XmlAttribute("from-where")> Public Property from_where As String
        <XmlAttribute("dtd-version")> Public Property dtd_version As String

        <XmlElement("book-meta")> Public Property book_meta As BookMeta

        <XmlElement("book-part")> Public Property book_part As BookPart

        Public Overrides Function ToString() As String
            Return id
        End Function

        Public Shared Function PreprocessingXml(xml As String) As String
            If xml Is Nothing Then
                Return ""
            Else
                Return xml.StringReplace("[<]break\s*/[>]", vbCrLf)
            End If
        End Function

        Public Iterator Function GetCitations() As IEnumerable(Of Citation)
            For Each section As section In book_part.body.sections
                For Each cite As Citation In GetCitations(section)
                    Yield cite
                Next
            Next
        End Function

        Private Shared Iterator Function GetCitations(section As section) As IEnumerable(Of Citation)
            If Not section.ref_list Is Nothing Then
                For Each cite As Citation In section.ref_list.GetCitations
                    Yield cite
                Next
            End If

            If Not section.sections.IsNullOrEmpty Then
                For Each sec As section In section.sections
                    For Each cite As Citation In GetCitations(sec)
                        Yield cite
                    Next
                Next
            End If
        End Function
    End Class

    Public Class BookMeta

        <XmlElement("book-id")> Public Property book_id As bookId
        <XmlElement("book-title-group")> Public Property book_title_group As bookTitleGroup

    End Class

    Public Class bookTitleGroup

        <XmlElement("book-title")> Public Property book_title As String
        Public Property subtitle As String

    End Class

    Public Class bookId

        <XmlAttribute("book-id-type")> Public Property book_id_type As String
        <XmlText> Public Property id As String

    End Class
End Namespace
