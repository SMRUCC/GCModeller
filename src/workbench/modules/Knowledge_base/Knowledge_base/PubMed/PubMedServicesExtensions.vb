#Region "Microsoft.VisualBasic::633176936f5cca61b8fa7dfdca9e8f3f, Knowledge_base\Knowledge_base\PubMed\PubMedServicesExtensions.vb"

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

    '     Module PubMedServicesExtensions
    ' 
    '         Function: GetArticleInfo, QueryPubmed
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports i32 = Microsoft.VisualBasic.Language.i32

Namespace PubMed

    <HideModuleName>
    Public Module PubMedServicesExtensions

        ''' <summary>
        ''' > https://www.ncbi.nlm.nih.gov/pmc/tools/developers/
        ''' 
        ''' Include two parameters that help to identify your service or application to our servers:
        '''
        ''' + ``tool`` should be the name Of the application, As a String value With no internal spaces, And
        ''' + ``email`` should be the e-mail address Of the maintainer Of the tool, And should be a valid e-mail address.
        ''' 
        ''' </summary>
        ReadOnly tool_info As New Dictionary(Of String, String) From {
            {"tool", "GCModeller-workbench-pubmed-repository"},
            {"email", "xie.guigang@gcmodeller.org"}
        }

        ' https://www.ncbi.nlm.nih.gov/portal/utils/file_backend.cgi?
        '    Db=pubmed&
        '    HistoryId=NCID_1_58281527_130.14.18.97_5555_1567045772_2177445283_0MetA0_S_HStore&
        '    QueryKey=10&
        '    Sort=&
        '    Filter=all&
        '    CompleteResultCount=90807&
        '    Mode=file&
        '    View=xml&
        '    p$l=Email&
        '    portalSnapshot=%2Fprojects%2Fentrez%2Fpubmed%2FPubMedGroup@1.146&
        '    BaseUrl=&
        '    PortName=live&
        '    RootTag=PubmedArticleSet&
        '    DocType=PubmedArticleSet%20PUBLIC%20%22-%2F%2FNLM%2F%2FDTD%20PubMedArticle,%201St%20January%202019%2F%2FEN%22%20%22https://dtd.nlm.nih.gov/ncbi/pubmed/out/pubmed_190101.dtd%22&
        '    FileName=&
        '    ContentType=xml

        ''' <summary>
        ''' Example
        ''' 
        ''' ```
        ''' https://www.ncbi.nlm.nih.gov/pubmed/?term=22007635&amp;report=xml
        ''' ```
        ''' </summary>
        ''' <param name="term"></param>
        ''' <returns></returns>
        Public Function GetArticleInfo(term As String) As PubmedArticle
            Dim url$ = $"https://www.ncbi.nlm.nih.gov/pubmed/?term={term}&report=xml"
            Dim html$ = url.GET(headers:=tool_info)
            Dim xml$ = html _
                .GetBetween("<pre>", "</pre>") _
                .Replace("&lt;", "<") _
                .Replace("&gt;", ">")
            Dim info As PubmedArticle = xml.CreateObjectFromXmlFragment(Of PubmedArticle)

            Return info
        End Function

        Const eSearch$ = "https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi"

        Public Iterator Function QueryPubmed(term$, Optional pageSize% = 2000) As IEnumerable(Of String)
            ' https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi?db=pubmed&term=microRNA&retmax=1000&retstart=1001
            Dim query As eSearchResult = ($"{eSearch}?db=pubmed&term={term.UrlEncode}&retmax={pageSize}") _
                .GET(headers:=tool_info) _
                .LoadFromXml(Of eSearchResult)
            Dim start As i32 = 0

            For Each id As String In query.IdList.AsEnumerable
                Yield id
            Next

            Do While start < query.Count
                query = ($"{eSearch}?db=pubmed&term={term.UrlEncode}&retmax={pageSize}&retstart={start = start + pageSize}") _
                    .GET(headers:=tool_info) _
                    .LoadFromXml(Of eSearchResult)

                For Each id As String In query.IdList.AsEnumerable
                    Yield id
                Next
            Loop
        End Function
    End Module
End Namespace
