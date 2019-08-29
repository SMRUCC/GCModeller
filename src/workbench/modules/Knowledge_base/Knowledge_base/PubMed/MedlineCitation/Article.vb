#Region "Microsoft.VisualBasic::4ac2b6ac754b0e596a47bd9f65b8bc91, Knowledge_base\Knowledge_base\PubMed\MedlineCitation\Article.vb"

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

    ' Class PMID
    ' 
    '     Properties: ID, Version
    ' 
    '     Function: ToString
    ' 
    ' Class Article
    ' 
    '     Properties: Abstract, ArticleDate, ArticleTitle, AuthorList, ELocationID
    '                 Journal, Language, Pagination, PublicationTypeList, PubModel
    ' 
    '     Function: ToString
    ' 
    ' Class PublicationTypeList
    ' 
    '     Properties: PublicationType
    ' 
    ' Class AuthorList
    ' 
    '     Properties: Authors, CompleteYN
    ' 
    ' Class Author
    ' 
    '     Properties: AffiliationInfo, ForeName, Initials, LastName, ValidYN
    ' 
    '     Function: ToString
    ' 
    ' Class AffiliationInfo
    ' 
    '     Properties: Affiliation
    ' 
    '     Function: ToString
    ' 
    ' Class Abstract
    ' 
    '     Properties: AbstractText
    ' 
    '     Function: ToString
    ' 
    ' Class ELocationID
    ' 
    '     Properties: EIdType, ValidYN, Value
    ' 
    '     Function: ToString
    ' 
    ' Class Pagination
    ' 
    '     Properties: MedlinePgn
    ' 
    ' Class Journal
    ' 
    '     Properties: ISOAbbreviation, ISSN, JournalIssue, Title
    ' 
    ' Class ISSN
    ' 
    '     Properties: ID, IssnType
    ' 
    ' Class JournalIssue
    ' 
    '     Properties: CitedMedium, Issue, PubDate, Volume
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
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
        Public Property ELocationID As ELocationID
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
        <XmlElement("PublicationType")> Public Property PublicationType As RegisterObject()
    End Class

    Public Class AuthorList
        <XmlAttribute>
        Public Property CompleteYN As String
        <XmlElement(NameOf(Author))>
        Public Property Authors As Author()
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

    Public Class Abstract
        Public Property AbstractText As String

        Public Overrides Function ToString() As String
            Return AbstractText
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
    End Class

    Public Class ISSN
        <XmlAttribute>
        Public Property IssnType As String
        <XmlText>
        Public Property ID As String
    End Class

    Public Class JournalIssue
        <XmlAttribute>
        Public Property CitedMedium As String
        Public Property Volume As String
        Public Property Issue As String
        Public Property PubDate As PubDate
    End Class
End Namespace