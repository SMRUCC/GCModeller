#Region "Microsoft.VisualBasic::e5aa12769670367c856f2c52944451db, modules\Knowledge_base\ncbi_kb\BITS\book-part.vb"

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

    '   Total Lines: 58
    '    Code Lines: 34 (58.62%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 24 (41.38%)
    '     File Size: 1.39 KB


    '     Class BookPart
    ' 
    '         Properties: body, book_part_meta
    ' 
    '         Function: ToString
    ' 
    '     Class bookPartMeta
    ' 
    '         Properties: book_part_id, pub_history, related_object, title_group
    ' 
    '     Class pubHistory
    ' 
    '         Properties: [date]
    ' 
    '     Class [date]
    ' 
    '         Properties: date_type, day, month, year
    ' 
    '     Class TitleGroup
    ' 
    '         Properties: title
    ' 
    '     Class bookPartId
    ' 
    '         Properties: book_part_id_type, id
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace BITS

    <XmlType("book-part")>
    Public Class BookPart

        Public Property body As body

        <XmlElement("book-part-meta")>
        Public Property book_part_meta As bookPartMeta

        Public Overrides Function ToString() As String
            Return body.ToString
        End Function

    End Class

    Public Class bookPartMeta

        <XmlElement("book-part-id")> Public Property book_part_id As bookPartId
        <XmlElement("title-group")> Public Property title_group As TitleGroup
        <XmlElement("related-object")> Public Property related_object As RelatedObject()
        <XmlElement("pub-history")> Public Property pub_history As pubHistory

    End Class

    Public Class pubHistory

        Public Property [date] As [date]

    End Class

    Public Class [date]

        <XmlAttribute("date-type")> Public Property date_type As String

        Public Property day As String
        Public Property month As String
        Public Property year As String

    End Class

    Public Class TitleGroup

        Public Property title As String

    End Class

    Public Class bookPartId

        <XmlAttribute("book-part-id-type")> Public Property book_part_id_type As String

        <XmlText> Public Property id As String

    End Class

End Namespace
