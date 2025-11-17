#Region "Microsoft.VisualBasic::6a7c15a34a7267be3114b8bf60695c8f, modules\Knowledge_base\ncbi_kb\PubMed\PubmedArticle.vb"

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

    '   Total Lines: 262
    '    Code Lines: 169 (64.50%)
    ' Comment Lines: 57 (21.76%)
    '    - Xml Docs: 91.23%
    ' 
    '   Blank Lines: 36 (13.74%)
    '     File Size: 10.91 KB


    '     Class PubmedArticle
    ' 
    '         Properties: MedlineCitation, PMID, PubmedData
    ' 
    '         Function: GetAbstractText, GetArticleDoi, GetAuthors, GetJournal, GetMeshTerms
    '                   GetPublishYear, GetTitle, ToString
    ' 
    '     Class KeywordList
    ' 
    '         Properties: Keywords, Owner
    ' 
    '         Function: GenericEnumerator, ToString
    ' 
    '     Class Keyword
    ' 
    '         Properties: Keyword, MajorTopicYN
    ' 
    '         Function: ToString
    ' 
    '     Class MedlineCitation
    ' 
    '         Properties: Article, ChemicalList, CitationSubset, DateCompleted, DateCreated
    '                     DateRevised, KeywordList, MedlineJournalInfo, MeshHeadingList, Owner
    '                     PMID, Status
    ' 
    '         Function: ToString
    ' 
    '     Class MeshHeading
    ' 
    '         Properties: DescriptorName, QualifierName
    ' 
    '         Function: ToString
    ' 
    '     Class PubmedData
    ' 
    '         Properties: ArticleIdList, History, PublicationStatus
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PubMed

    ''' <summary>
    ''' PubMed® comprises more than 36 million citations for biomedical literature from MEDLINE, 
    ''' life science journals, and online books. Citations may include links to full text content 
    ''' from PubMed Central and publisher web sites.
    ''' </summary>
    ''' <remarks>
    ''' A single pubmed article object inside a xml metadata file
    ''' </remarks>
    Public Class PubmedArticle

        ''' <summary>
        ''' MEDLINE®/PubMed® Journal Article Citation Format
        ''' </summary>
        ''' <returns></returns>
        Public Property MedlineCitation As MedlineCitation
        Public Property PubmedData As PubmedData

        Public ReadOnly Property PMID As String
            Get
                If MedlineCitation IsNot Nothing Then
                    Return MedlineCitation.PMID.ID
                End If

                Return Nothing
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return MedlineCitation.ToString
        End Function

        ''' <summary>
        ''' get article title
        ''' </summary>
        ''' <returns>
        ''' this function will returns a title string with html tag unescaped
        ''' </returns>
        Public Function GetTitle() As String
            If MedlineCitation IsNot Nothing Then
                If MedlineCitation.Article IsNot Nothing Then
                    Return MedlineCitation.Article.ArticleTitle.TrimNewLine.Replace("&lt;", "<")
                End If
            End If

            Return Nothing
        End Function

        Public Function GetJournal() As String
            If MedlineCitation IsNot Nothing Then
                If MedlineCitation.Article IsNot Nothing Then
                    If MedlineCitation.Article.Journal IsNot Nothing Then
                        Return MedlineCitation.Article.Journal.Title
                    End If
                End If
            End If

            Return Nothing
        End Function

        Public Iterator Function GetAuthors() As IEnumerable(Of String)
            If MedlineCitation IsNot Nothing Then
                If MedlineCitation.Article IsNot Nothing Then
                    For Each author As Author In MedlineCitation.Article.AuthorList.AsEnumerable
                        If Not author Is Nothing Then
                            Yield $"{author.ForeName} {author.LastName}"
                        End If
                    Next
                End If
            End If
        End Function

        Public Function GetArticleDoi() As String
            If MedlineCitation IsNot Nothing Then
                If MedlineCitation.Article IsNot Nothing Then
                    If Not MedlineCitation.Article.ELocationID.IsNullOrEmpty Then
                        Return MedlineCitation.Article.ELocationID.KeyItem("doi")?.Value
                    End If
                End If
            End If
            If PubmedData IsNot Nothing Then
                Dim doi = PubmedData.ArticleIdList.Where(Function(a) a.IdType.TextEquals("doi")).FirstOrDefault

                If Not doi Is Nothing Then
                    Return doi.ID
                End If
            End If

            Return "-"
        End Function

        Public Function GetPublishYear() As Integer
            If MedlineCitation IsNot Nothing Then
                If MedlineCitation.Article IsNot Nothing Then
                    If MedlineCitation.Article.Journal IsNot Nothing Then
                        If MedlineCitation.Article.Journal.JournalIssue IsNot Nothing Then
                            If MedlineCitation.Article.Journal.JournalIssue.PubDate IsNot Nothing Then
                                Return MedlineCitation.Article.Journal.JournalIssue.PubDate.Year
                            End If
                        End If
                    End If
                    If MedlineCitation.Article.ArticleDate IsNot Nothing Then
                        Return MedlineCitation.Article.ArticleDate.Year
                    End If
                End If
            End If

            Return 0
        End Function

        ''' <summary>
        ''' get the article abstract text
        ''' </summary>
        ''' <returns>
        ''' this function will returns a text content of the article abstract with html tag unescaped.
        ''' </returns>
        Public Function GetAbstractText() As String
            If MedlineCitation IsNot Nothing Then
                If MedlineCitation.Article IsNot Nothing Then
                    If MedlineCitation.Article.Abstract IsNot Nothing AndAlso Not MedlineCitation.Article.Abstract.AbstractText.IsNullOrEmpty Then
                        Return MedlineCitation.Article _
                            .Abstract _
                            .AbstractText _
                            .Select(Function(a) a.Text) _
                            .JoinBy(vbCrLf) _
                            .Replace("&lt;", "<")
                    End If
                End If
            End If

            Return Nothing
        End Function

        Public Iterator Function GetMeshTerms() As IEnumerable(Of NamedValue(Of String))
            If MedlineCitation IsNot Nothing Then
                If MedlineCitation.MeshHeadingList IsNot Nothing Then
                    For Each term In MedlineCitation.MeshHeadingList
                        If term.DescriptorName IsNot Nothing Then
                            Yield New NamedValue(Of String)(term.DescriptorName.UI, term.DescriptorName.Value)
                        End If
                    Next
                End If
                If MedlineCitation.ChemicalList IsNot Nothing Then
                    For Each term In MedlineCitation.ChemicalList
                        If term.NameOfSubstance IsNot Nothing Then
                            Yield New NamedValue(Of String)(term.NameOfSubstance.UI, term.NameOfSubstance.Value)
                        End If
                    Next
                End If
            End If
        End Function

        Public Iterator Function GetOtherTerms() As IEnumerable(Of String)
            If MedlineCitation IsNot Nothing Then
                If MedlineCitation.KeywordList IsNot Nothing Then
                    For Each term As Keyword In MedlineCitation.KeywordList.AsEnumerable
                        Yield term.Keyword
                    Next
                End If
            End If
        End Function

    End Class

    Public Class KeywordList : Implements Enumeration(Of Keyword)

        <XmlAttribute>
        Public Property Owner As String
        <XmlElement("Keyword")>
        Public Property Keywords As Keyword()

        Public Overrides Function ToString() As String
            Return Keywords.SafeQuery.Select(Function(kw) kw.Keyword).GetJson
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of Keyword) Implements Enumeration(Of Keyword).GenericEnumerator
            If Not Keywords Is Nothing Then
                For Each word In Keywords
                    Yield word
                Next
            End If
        End Function
    End Class

    Public Class Keyword

        <XmlAttribute>
        Public Property MajorTopicYN As String
        <XmlText>
        Public Property Keyword As String

        Sub New()
        End Sub

        Sub New(term As String)
            Keyword = term
        End Sub

        Public Overrides Function ToString() As String
            Return Keyword
        End Function
    End Class

    ''' <summary>
    ''' MEDLINE®/PubMed® Journal Article Citation Format
    ''' </summary>
    ''' <remarks>
    ''' The National Library of Medicine® (NLM®) uses the ANSI/NISO Z39.29-2005 (R2010) Bibliographic 
    ''' References standard as the basis for the format of MEDLINE/PubMed citations to journal articles. 
    ''' The National Information Standards Organization (NISO) is a non-profit association accredited 
    ''' by the American National Standards Institute (ANSI) to identify, develop, maintain and publish 
    ''' technical standards in the area of library and information science.
    '''
    ''' Guidance on the NLM interpretation of this national standard can be found in Citing Medicine: 
    ''' the NLM Style Guide For Authors, Editors, And Publishers, available via the National Center For 
    ''' Biotechnology Information (NCBI) Bookshelf.  
    '''
    ''' PubMed citations In the Summary (text) display format are compatible With ANSI/NISO Z39.29-2005 
    ''' (R2010). This format Is useful For a list Of references Or a bibliography. Here Is a sample 
    ''' citation In the Summary (text) display format:
    '''
    ''' ```
    ''' Freedman SB, Adler M, Seshadri R, Powell EC. Oral ondansetron For gastroenteritis In a pediatric 
    ''' emergency department. N Engl J Med. 2006 Apr 20;354(16):1698-705. PubMed PMID: 16625009.
    ''' ```
    '''
    ''' The last element In the citation above identifies the unique identification number In PubMed (PMID).
    '''
    ''' The NLM citation format Is also the foundation For the reference style approved by the Recommendations
    ''' For the Conduct, Reporting, Editing And Publication Of Scholarly Work In Medical Journals, a product 
    ''' Of the International Committee Of Medical Journal Editors (ICMJE). Please refer To Section IV.A.3.g.ii, 
    ''' References Style And Format, Of the PDF version Of the ICMJE Recommendations. The ICMJE Web site also 
    ''' lists the journals that follow the ICMJE Recommendations. NLM hosts And maintains a Web page 
    ''' featuring sample citations extracted from Or based On Citing Medicine For easy use by the ICMJE 
    ''' audience.
    ''' </remarks>
    Public Class MedlineCitation
        <XmlAttribute> Public Property Status As String
        <XmlAttribute> Public Property Owner As String

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

        Public Overrides Function ToString() As String
            Return Article.ToString
        End Function
    End Class

    Public Class MeshHeading

        Public Property DescriptorName As RegisterObject
        <XmlElement("QualifierName")>
        Public Property QualifierName As RegisterObject()

        Sub New()
        End Sub

        Sub New(keyword As String)
            DescriptorName = New RegisterObject With {.Value = keyword}
        End Sub

        Public Overrides Function ToString() As String
            Return DescriptorName.ToString
        End Function
    End Class

    Public Class PubmedData

        Public Property History As History
        Public Property PublicationStatus As String
        Public Property ArticleIdList As ArticleId()

        Public Overrides Function ToString() As String
            Return PublicationStatus
        End Function

    End Class
End Namespace
