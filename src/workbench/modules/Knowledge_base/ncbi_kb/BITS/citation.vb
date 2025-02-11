#Region "Microsoft.VisualBasic::9e95fb555e5928abd11f5dc714859b57, modules\Knowledge_base\ncbi_kb\BITS\citation.vb"

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

    '   Total Lines: 163
    '    Code Lines: 118 (72.39%)
    ' Comment Lines: 5 (3.07%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 40 (24.54%)
    '     File Size: 5.42 KB


    '     Class RefList
    ' 
    '         Properties: id, ref
    ' 
    '         Function: CreateCitation, GetCitations
    ' 
    '     Class ref
    ' 
    '         Properties: element_citation, id, mixed_citation
    ' 
    '         Function: ToString
    ' 
    '     Class personGroup
    ' 
    '         Properties: names
    ' 
    '         Function: GenericEnumerator
    ' 
    '     Class MixedCitation
    ' 
    '         Properties: annotation, collab, etal, fpage, issue
    '                     lpage, person_group, pub_id, publication_type, source
    '                     string_names, title, volume, year
    ' 
    '     Class PubId
    ' 
    '         Properties: id, pub_id_type
    ' 
    '         Function: ToString
    ' 
    '     Class StringName
    ' 
    '         Properties: given_names, name_style, surname
    ' 
    '         Function: ToString
    ' 
    '     Class Annotation
    ' 
    '         Properties: p
    ' 
    '         Function: GetContentText
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace BITS

    Public Class RefList

        <XmlAttribute("id")> Public Property id As String
        <XmlElement> Public Property ref As ref()

        Public Iterator Function GetCitations() As IEnumerable(Of Citation)
            For Each r As ref In ref.SafeQuery
                Dim cite As Citation = CreateCitation(r)

                If Not cite Is Nothing Then
                    Yield cite
                End If
            Next
        End Function

        Public Shared Function CreateCitation(r As ref) As Citation
            Dim authors As String()
            Dim cite As MixedCitation

            If r.mixed_citation IsNot Nothing Then
                cite = r.mixed_citation
                authors = cite.string_names _
                    .SafeQuery _
                    .Select(Function(name) name.ToString) _
                    .ToArray
            ElseIf r.element_citation IsNot Nothing Then
                cite = r.element_citation
                authors = cite.person_group _
                    .AsEnumerable _
                    .Select(Function(name) name.ToString) _
                    .ToArray
            Else
                Return Nothing
            End If

            Dim doi = cite.pub_id.SafeQuery.Where(Function(p) p.pub_id_type = "doi").FirstOrDefault?.id
            Dim pmid = cite.pub_id.SafeQuery.Where(Function(p) p.pub_id_type = "pmid").FirstOrDefault?.id

            Dim citation As New Citation With {
                .authors = authors,
                .abstract = cite.annotation _
                    .SafeQuery _
                    .Select(Function(a) a.GetContentText) _
                    .JoinBy(vbCrLf & vbCrLf),
                .doi = doi,
                .pubmed_id = pmid,
                .fpage = cite.fpage,
                .lpage = cite.lpage,
                .journal = cite.source,
                .title = cite.title?.GetTextContent(),
                .volume = cite.volume,
                .year = cite.year
            }

            If citation.authors.IsNullOrEmpty AndAlso citation.title.StringEmpty AndAlso citation.journal.StringEmpty Then
                ' needs to be parsed from the text?
                Call Citation.TryParse(cite.GetTextContent, citation)
            End If

            Return citation
        End Function

    End Class

    Public Class ref

        <XmlAttribute> Public Property id As String

        <XmlElement("mixed-citation")>
        Public Property mixed_citation As MixedCitation

        <XmlElement("element-citation")>
        Public Property element_citation As MixedCitation

        Public Overrides Function ToString() As String
            Return id
        End Function

    End Class

    Public Class personGroup : Implements Enumeration(Of StringName)

        <XmlElement("name")> Public Property names As StringName()

        Public Iterator Function GenericEnumerator() As IEnumerator(Of StringName) Implements Enumeration(Of StringName).GenericEnumerator
            If names Is Nothing Then
                Return
            End If

            For Each name As StringName In Me.names
                Yield name
            Next
        End Function
    End Class

    Public Class MixedCitation : Inherits Paragraph

        <XmlAttribute("publication-type")> Public Property publication_type As String
        <XmlElement> Public Property annotation As Annotation()
        <XmlElement("string-name")> Public Property string_names As StringName()
        <XmlElement("person-group")> Public Property person_group As personGroup
        Public Property etal As String

        ''' <summary>
        ''' the article-title
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("article-title")> Public Property title As Paragraph
        Public Property source As String
        Public Property year As String
        Public Property volume As String
        Public Property fpage As String
        Public Property lpage As String
        <XmlElement("pub-id")> Public Property pub_id As PubId()

        Public Property collab As String
        Public Property issue As String

    End Class

    Public Class PubId

        <XmlAttribute("pub-id-type")>
        Public Property pub_id_type As String

        <XmlText> Public Property id As String

        Public Overrides Function ToString() As String
            Return id
        End Function

    End Class

    <XmlType("string-name")>
    Public Class StringName

        <XmlAttribute("name-style")> Public Property name_style As String
        Public Property surname As String
        <XmlElement("given-names")> Public Property given_names As String

        Public Overrides Function ToString() As String
            Return surname & " " & given_names
        End Function

    End Class

    Public Class Annotation

        <XmlElement> Public Property p As Paragraph()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetContentText() As String
            Return p.Select(Function(pi) pi.GetTextContent).JoinBy(vbCrLf & vbCrLf)
        End Function

    End Class
End Namespace
