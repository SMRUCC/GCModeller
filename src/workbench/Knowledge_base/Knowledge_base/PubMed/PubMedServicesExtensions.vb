#Region "Microsoft.VisualBasic::e61b71a58dae16a2e6e94e3dcdffc0b6, ..\workbench\Knowledge_base\Knowledge_base\PubMed\PubMedServicesExtensions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Public Module PubMedServicesExtensions

    ''' <summary>
    ''' > https://www.ncbi.nlm.nih.gov/pmc/tools/developers/
    ''' 
    ''' Include two parameters that help to identify your service or application to our servers:
    '''
    ''' + ``tool`` should be the name Of the application, As a String value With no internal spaces, And
    ''' + ``email`` should be the e-mail address Of the maintainer Of the tool, And should be a valid e-mail address.
    ''' </summary>
    ReadOnly tool_info As New Dictionary(Of String, String) From {
        {"tool", "GCModeller-workbench-pubmed-repository"},
        {"email", "xie.guigang@gcmodeller.org"}
    }

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
End Module

