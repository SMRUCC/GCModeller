#Region "Microsoft.VisualBasic::521577941829e5e4dd9f08d3d20f8c5a, modules\Knowledge_base\ncbi_kb\PubMed\PubmedArticleSet.vb"

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

    '   Total Lines: 69
    '    Code Lines: 46 (66.67%)
    ' Comment Lines: 6 (8.70%)
    '    - Xml Docs: 66.67%
    ' 
    '   Blank Lines: 17 (24.64%)
    '     File Size: 2.63 KB


    '     Class PubmedArticleSet
    ' 
    '         Properties: PubmedArticle
    ' 
    '         Function: Escape, LoadStream, ParseArticleXml, ProcessXmlDocument
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
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

End Namespace
