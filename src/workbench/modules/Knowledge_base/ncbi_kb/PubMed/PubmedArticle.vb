#Region "Microsoft.VisualBasic::87591f4ea57c1219700b58b1acc72a34, modules\Knowledge_base\ncbi_kb\PubMed\PubmedArticle.vb"

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

'   Total Lines: 103
'    Code Lines: 51 (49.51%)
' Comment Lines: 42 (40.78%)
'    - Xml Docs: 88.10%
' 
'   Blank Lines: 10 (9.71%)
'     File Size: 4.70 KB


'     Class PubmedArticle
' 
'         Properties: MedlineCitation, PubmedData
' 
'     Class KeywordList
' 
'         Properties: Keywords, Owner
' 
'         Function: GenericEnumerator
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

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace PubMed

    Public Class PubmedArticleSet

        <XmlElement("PubmedArticle")>
        Public Property PubmedArticle As PubmedArticle()

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="s">A stream of a large xml document file</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadStream(s As Stream, Optional tqdm As Boolean = True) As IEnumerable(Of PubmedArticle)
            Return s.LoadUltraLargeXMLDataSet(Of PubmedArticle)(preprocess:=AddressOf ProcessXmlDocument, tqdm:=tqdm)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseArticleXml(xml As String) As PubmedArticle
            Return xml.CreateObjectFromXmlFragment(Of PubmedArticle)(preprocess:=AddressOf ProcessXmlDocument)
        End Function

        Private Shared Function ProcessXmlDocument(s As String) As String
            Static articleTitle As New Regex("[<]ArticleTitle[>].*?[<]/ArticleTitle[>]", RegexICSng)
            Static abstractText As New Regex("[<]AbstractText[>].*?[<]/AbstractText[>]", RegexICSng)
            Static vernacularTitle As New Regex("[<]VernacularTitle[>].*?[<]/VernacularTitle[>]", RegexICSng)

            Dim sb As New StringBuilder(s)

            Call articleTitle.Replace(s, Function(m) Escape(m, sb))
            Call abstractText.Replace(s, Function(m) Escape(m, sb))
            Call vernacularTitle.Replace(s, Function(m) Escape(m, sb))

            Call sb.Replace(" < ", " &lt; ")

            Return sb.ToString
        End Function

        Private Shared Function Escape(m As Match, sb As StringBuilder) As String
            Dim str = m.Value.GetValue

            Static elementBegin As New Regex("[<][a-z0-9]+", RegexICSng)
            Static elementEnd As New Regex("[<]/[a-z0-9]+", RegexICSng)

            For Each tag As String In elementBegin _
                .Matches(str) _
                .EachValue _
                .JoinIterates(elementEnd.Matches(str).EachValue) _
                .Distinct

                Call sb.Replace(tag, tag.Replace("<", "&lt;"))
            Next

            Return ""
        End Function
    End Class

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

        Public Function GetTitle() As String
            If MedlineCitation IsNot Nothing Then
                If MedlineCitation.Article IsNot Nothing Then
                    Return MedlineCitation.Article.ArticleTitle
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

        Public Function GetAbstractText() As String
            If MedlineCitation IsNot Nothing Then
                If MedlineCitation.Article IsNot Nothing Then
                    If MedlineCitation.Article.Abstract IsNot Nothing AndAlso Not MedlineCitation.Article.Abstract.AbstractText.IsNullOrEmpty Then
                        Return MedlineCitation.Article.Abstract.AbstractText.Select(Function(a) a.Text).JoinBy(vbCrLf)
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
