#Region "Microsoft.VisualBasic::1d0f8c8c9e6d84e6fd30537675659677, modules\Knowledge_base\Knowledge_base\PubMed\MedlineCitation\Article.vb"

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

    '   Total Lines: 173
    '    Code Lines: 137
    ' Comment Lines: 0
    '   Blank Lines: 36
    '     File Size: 5.22 KB


    '     Class PMID
    ' 
    '         Properties: ID, Version
    ' 
    '         Function: ToString
    ' 
    '     Class Article
    ' 
    '         Properties: Abstract, ArticleDate, ArticleTitle, AuthorList, ELocationID
    '                     Journal, Language, Pagination, PublicationTypeList, PubModel
    ' 
    '         Function: ToString
    ' 
    '     Class PublicationTypeList
    ' 
    '         Properties: PublicationType
    ' 
    '     Class AuthorList
    ' 
    '         Properties: Authors, CompleteYN
    ' 
    '         Function: GenericEnumerator, GetEnumerator, ToString
    ' 
    '     Class Author
    ' 
    '         Properties: AffiliationInfo, ForeName, Initials, LastName, ValidYN
    ' 
    '         Function: ToString
    ' 
    '     Class AffiliationInfo
    ' 
    '         Properties: Affiliation
    ' 
    '         Function: ToString
    ' 
    '     Class AbstractText
    ' 
    '         Properties: Label, NlmCategory, Text
    ' 
    '         Function: ToString
    ' 
    '     Class Abstract
    ' 
    '         Properties: AbstractText, CopyrightInformation
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Class ELocationID
    ' 
    '         Properties: EIdType, ValidYN, Value
    ' 
    '         Function: ToString
    ' 
    '     Class Pagination
    ' 
    '         Properties: MedlinePgn
    ' 
    '     Class Journal
    ' 
    '         Properties: ISOAbbreviation, ISSN, JournalIssue, Title
    ' 
    '         Function: ToString
    ' 
    '     Class ISSN
    ' 
    '         Properties: ID, IssnType
    ' 
    '         Function: ToString
    ' 
    '     Class JournalIssue
    ' 
    '         Properties: CitedMedium, Issue, PubDate, Volume
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PubMed

    Public Class PMID

        <XmlAttribute>
        Public Property Version As String
        <XmlText>
        Public Property ID As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class Article
        <XmlAttribute>
        Public Property PubModel As String
        Public Property Journal As Journal
        Public Property ArticleTitle As String
        Public Property Pagination As Pagination

        <XmlElement("ELocationID")>
        Public Property ELocationID As ELocationID()
        Public Property Abstract As Abstract
        Public Property AuthorList As AuthorList
        Public Property Language As String
        Public Property PublicationTypeList As PublicationTypeList
        Public Property ArticleDate As PubDate

        Public Overrides Function ToString() As String
            Return ArticleTitle
        End Function
    End Class

    Public Class PublicationTypeList

        <XmlElement("PublicationType")>
        Public Property PublicationType As RegisterObject()
    End Class

    Public Class AuthorList : Implements Enumeration(Of Author)

        <XmlAttribute>
        Public Property CompleteYN As String
        <XmlElement(NameOf(Author))>
        Public Property Authors As Author()

        Public Overrides Function ToString() As String
            Return Authors.Select(Function(a) a.ToString).GetJson
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of Author) Implements Enumeration(Of Author).GenericEnumerator
            If Not Authors Is Nothing Then
                For Each author As Author In Authors
                    Yield author
                Next
            End If
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of Author).GetEnumerator
            Yield GenericEnumerator()
        End Function
    End Class

    Public Class Author
        <XmlAttribute>
        Public Property ValidYN As String
        Public Property LastName As String
        Public Property ForeName As String
        Public Property Initials As String
        Public Property AffiliationInfo As AffiliationInfo

        Public Overrides Function ToString() As String
            Dim disp$ = $"{Initials} {ForeName} {LastName}"
            If AffiliationInfo Is Nothing Then
                disp &= $" ({AffiliationInfo.Affiliation})"
            End If
            Return disp
        End Function
    End Class

    Public Class AffiliationInfo
        Public Property Affiliation As String

        Public Overrides Function ToString() As String
            Return Affiliation
        End Function
    End Class

    Public Class AbstractText

        <XmlAttribute> Public Property Label As String
        <XmlAttribute> Public Property NlmCategory As String

        <XmlText>
        Public Property Text As String

        Public Overrides Function ToString() As String
            Return Text
        End Function
    End Class

    Public Class Abstract

        <XmlElement("AbstractText")>
        Public Property AbstractText As AbstractText()
        Public Property CopyrightInformation As String

        Sub New()
        End Sub

        Sub New(text As String)
            AbstractText = {New AbstractText With {.Text = text}}
        End Sub

        Public Overrides Function ToString() As String
            Return AbstractText _
                .SafeQuery _
                .Select(Function(a) a.Text) _
                .JoinBy(vbCrLf)
        End Function
    End Class

    Public Class ELocationID
        <XmlAttribute> Public Property EIdType As String
        <XmlAttribute> Public Property ValidYN As String

        <XmlText>
        Public Property Value As String

        Public Overrides Function ToString() As String
            Return EIdType & ": " & Value
        End Function
    End Class

    Public Class Pagination
        Public Property MedlinePgn As String
    End Class

    Public Class Journal
        Public Property ISSN As ISSN
        Public Property JournalIssue As JournalIssue
        Public Property Title As String
        Public Property ISOAbbreviation As String

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class

    Public Class ISSN
        <XmlAttribute>
        Public Property IssnType As String
        <XmlText>
        Public Property ID As String

        Public Overrides Function ToString() As String
            Return $"[{IssnType}] {ID}"
        End Function
    End Class

    Public Class JournalIssue
        <XmlAttribute>
        Public Property CitedMedium As String
        Public Property Volume As String
        Public Property Issue As String
        Public Property PubDate As PubDate
    End Class
End Namespace
