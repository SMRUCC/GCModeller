#Region "Microsoft.VisualBasic::b86ded1325461ec363c9cff911f8526b, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\WebQuery\ReactionQuery.vb"

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

    '   Total Lines: 172
    '    Code Lines: 128
    ' Comment Lines: 16
    '   Blank Lines: 28
    '     File Size: 7.09 KB


    '     Class ReactionQuery
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DownloadFrom, getCommonNames, innerOrthParser, orthologyParser, parsingEquation
    '                   rxnUrl, trimComments, webFormParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.DBGET.WebQuery

    Public Class ReactionQuery : Inherits WebQuery(Of String)

        Const dataURL As String = "http://www.kegg.jp/dbget-bin/www_bget?rn:{0}"

        Public Sub New(<CallerMemberName> Optional cache As String = Nothing, Optional interval As Integer = -1, Optional offline As Boolean = False)
            MyBase.New(url:=AddressOf rxnUrl,
                       contextGuid:=Function(id) id,
                       parser:=AddressOf DownloadFrom,
                       prefix:=Function(id) id.Trim.Last.ToString,
                       cache:=cache,
                       interval:=interval,
                       offline:=offline
                   )
        End Sub

        Private Shared Function rxnUrl(rxnId As String) As String
            Return String.Format(dataURL, rxnId)
        End Function

        ''' <summary>
        ''' 从指定的URL页面下载代谢过程的数据模型
        ''' </summary>
        ''' <param name="resource">URL或者页面文本</param>
        ''' <returns></returns>
        Public Shared Function DownloadFrom(resource As String, Optional schema As Type = Nothing) As Reaction
            Dim webForm As New WebForm(resource)

            If webForm.Count = 0 Then
                Return Nothing
            Else
                Return webFormParser(Of Reaction)(webForm)
            End If
        End Function

        ''' <summary>
        ''' 从网页模型之中解析出代谢模型数据
        ''' </summary>
        ''' <typeparam name="ReactionType"></typeparam>
        ''' <param name="WebForm"></param>
        ''' <returns></returns>
        Friend Shared Function webFormParser(Of ReactionType As Reaction)(WebForm As WebForm) As ReactionType
            Dim rn As ReactionType = Activator.CreateInstance(Of ReactionType)()

            On Error Resume Next

            rn.ID = WebForm.GetValue("Entry").FirstOrDefault.Strip_NOBR.StripHTMLTags.StripBlank.Split.First
            rn.Comments = trimComments(WebForm.GetValue("Comment").FirstOrDefault) _
                .Strip_NOBR _
                .StripBlank _
                .TrimNewLine _
                .Replace("Comment ", "") ' Comment标记没有被去除干净？

            rn.Definition = WebForm.GetValue("Definition") _
                .FirstOrDefault _
                .Strip_NOBR _
                .Replace("<=", "&lt;=") _
                .StripHTMLTags _
                .StripBlank _
                .Replace("&lt;=", "<=")
            rn.Pathway = WebForm.parseList(WebForm.GetValue("Pathway").FirstOrDefault, "<a href="".+?"">.+?</a>")
            rn.Module = WebForm.parseList(WebForm.GetValue("Module").FirstOrDefault, "<a href="".+?"">.+?</a>")
            rn.CommonNames = getCommonNames(WebForm.GetValue("Name").FirstOrDefault)
            rn.Equation = parsingEquation(WebForm.GetValue("Equation").FirstOrDefault)
            rn.Orthology = orthologyParser(WebForm.GetValue("Orthology").FirstOrDefault)
            rn.Class = WebForm.parseList(WebForm.GetValue("Reaction class").FirstOrDefault, "<a href="".+?"">.+?</a>")

            Dim ecTemp As String = WebForm _
                .GetValue("Enzyme") _
                .FirstOrDefault
            rn.Enzyme = r.Matches(ecTemp, "\d+(\.\d+)+") _
                .ToArray _
                .Distinct _
                .ToArray

            Return rn
        End Function

        Private Shared Function orthologyParser(s As String) As OrthologyTerms
            Dim ms As String() = r.Matches(s, "K\d+<.+?\[EC.+?\]", RegexOptions.IgnoreCase).ToArray
            Dim result = ms _
                .Select(AddressOf innerOrthParser) _
                .ToArray

            Return New OrthologyTerms With {
                .Terms = result
            }
        End Function

        ''' <summary>
        ''' K01509&lt;/a> adenosinetriphosphatase [EC:&lt;a href="/dbget-bin/www_bget?ec:3.6.1.3">3.6.1.3&lt;/a>]
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Private Shared Function innerOrthParser(s As String) As [Property]
            Dim t As String() = r.Split(s, "<[/]?a>", RegexOptions.IgnoreCase)
            Dim KO As String = t.ElementAtOrDefault(Scan0)
            Dim def As String = t.ElementAtOrDefault(1).Split("["c).First.Trim
            Dim EC As String = r.Match(s, "\d+(\.\d+)+").Value

            Return New [Property] With {
                .name = KO,
                .value = EC,
                .comment = def.StripHTMLTags
            }
        End Function

        Private Shared Function trimComments(html As String) As String
            If String.IsNullOrEmpty(html) Then
                Return ""
            End If

            Dim links As KeyValuePair(Of String, String)() = r _
                .Matches(html, "<a href="".+?"">.+?</a>") _
                .ToArray(Function(m) New KeyValuePair(Of String, String)(m, m.GetValue))
            Dim sb As New StringBuilder(html)

            For Each l As KeyValuePair(Of String, String) In links
                Call sb.Replace(l.Key, l.Value)
            Next
            Call sb.Replace("<br>", "")

            Return sb.ToString.StripHTMLTags
        End Function

        Private Shared Function parsingEquation(strData As String) As String
            Dim sb As New StringBuilder(strData)
            Dim link As KeyValuePair(Of String, String)

            For Each m As Match In Regex.Matches(strData, "<a href="".+?"">.+?</a>")
                link = New KeyValuePair(Of String, String)(m.Value, m.Value.GetValue)
                sb.Replace(link.Key, link.Value)
            Next

            Dim s$ = sb.ToString

            s = s.Replace(Regex.Match(s, "<nobr>.+</nobr>").Value, "")
            s = s.Replace("<=", "&lt;=")
            s = s.Replace("<-", "&lt;-")
            s = s.StripHTMLTags
            s = s.Replace("&lt;", "<")
            s = s.Replace("&gt;", ">")
            s = s.StripBlank

            Return s
        End Function

        Private Shared Function getCommonNames(str As String) As String()
            Return LinqAPI.Exec(Of String) <=
 _
                From line As String
                In Strings.Split(str.Strip_NOBR, "<br>")
                Let s = line.StripHTMLTags.StripBlank
                Where Not String.IsNullOrEmpty(s)
                Select s

        End Function
    End Class
End Namespace
