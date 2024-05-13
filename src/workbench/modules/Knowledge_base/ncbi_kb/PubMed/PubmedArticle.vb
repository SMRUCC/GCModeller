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
    '    Code Lines: 51
    ' Comment Lines: 42
    '   Blank Lines: 10
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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace PubMed

    ''' <summary>
    ''' PubMed® comprises more than 36 million citations for biomedical literature from MEDLINE, 
    ''' life science journals, and online books. Citations may include links to full text content 
    ''' from PubMed Central and publisher web sites.
    ''' </summary>
    Public Class PubmedArticle

        ''' <summary>
        ''' MEDLINE®/PubMed® Journal Article Citation Format
        ''' </summary>
        ''' <returns></returns>
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
    End Class

    Public Class Keyword
        <XmlAttribute>
        Public Property MajorTopicYN As String
        <XmlText>
        Public Property Keyword As String
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
