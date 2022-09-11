#Region "Microsoft.VisualBasic::fe79ea1b6f5fd8bb87b00579d677ae0c, modules\Knowledge_base\Knowledge_base\PubMed\PubmedArticle.vb"

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

    '   Total Lines: 63
    '    Code Lines: 54
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 2.21 KB


    '     Class PubmedArticle
    ' 
    '         Properties: MedlineCitation, PubmedData
    ' 
    '     Class KeywordList
    ' 
    '         Properties: Keywords, Owner
    ' 
    '         Function: GenericEnumerator, GetEnumerator
    ' 
    '     Class Keyword
    ' 
    '         Properties: Keyword, MajorTopicYN
    ' 
    '     Class MedlineCitation
    ' 
    '         Properties: Article, ChemicalList, CitationSubset, DateCompleted, DateCreated
    '                     DateRevised, KeywordList, MedlineJournalInfo, MeshHeadingList, Owner
    '                     PMID, Status
    ' 
    '     Class MeshHeading
    ' 
    '         Properties: DescriptorName, QualifierName
    ' 
    '     Class PubmedData
    ' 
    '         Properties: ArticleIdList, History, PublicationStatus
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace PubMed

    Public Class PubmedArticle
        Public Property MedlineCitation As MedlineCitation
        Public Property PubmedData As PubmedData
    End Class

    Public Class KeywordList : Implements Enumeration(Of Keyword)
        <XmlAttribute>
        Public Property Owner As String
        <XmlElement("Keyword")>
        Public Property Keywords As Keyword()

        Public Iterator Function GenericEnumerator() As IEnumerator(Of Keyword) Implements Enumeration(Of Keyword).GenericEnumerator
            If Not Keywords Is Nothing Then
                For Each word In Keywords
                    Yield word
                Next
            End If
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of Keyword).GetEnumerator
            Yield GenericEnumerator()
        End Function
    End Class

    Public Class Keyword
        <XmlAttribute>
        Public Property MajorTopicYN As String
        <XmlText>
        Public Property Keyword As String
    End Class

    Public Class MedlineCitation
        Public Property Status As String
        Public Property Owner As String
        Public Property PMID As PMID
        Public Property DateCreated As PubDate
        Public Property DateCompleted As PubDate
        Public Property DateRevised As PubDate
        Public Property Article As Article
        Public Property MedlineJournalInfo As MedlineJournalInfo
        Public Property ChemicalList As Chemical()
        Public Property CitationSubset As String
        Public Property MeshHeadingList As MeshHeading()
        Public Property KeywordList As KeywordList
    End Class

    Public Class MeshHeading
        Public Property DescriptorName As RegisterObject
        <XmlElement("QualifierName")>
        Public Property QualifierName As RegisterObject()
    End Class

    Public Class PubmedData
        Public Property History As History
        Public Property PublicationStatus As String
        Public Property ArticleIdList As ArticleId()
    End Class
End Namespace
