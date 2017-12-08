#Region "Microsoft.VisualBasic::5025b94f07600f6af59018e8f18edb83, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\ReactionWebAPI.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Module ReactionWebAPI

        Const URL As String = "http://www.kegg.jp/dbget-bin/www_bget?rn:{0}"
        Const KEGG_COMPOUND_ID As String = "[A-Z]+\d+"

        ''' <summary>
        ''' 使用ID来下载代谢过程的模型数据
        ''' </summary>
        ''' <param name="ID">编号格式为：``R\d+``，例如R00259</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Download(ID As String) As Reaction
            Return DownloadFrom(String.Format(URL, ID))
        End Function

        ''' <summary>
        ''' 从指定的URL页面下载代谢过程的数据模型
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        Public Function DownloadFrom(url As String) As Reaction
            Dim WebForm As New WebForm(url)

            If WebForm.Count = 0 Then
                Return Nothing
            Else
                Return __webFormParser(Of Reaction)(WebForm)
            End If
        End Function

        ''' <summary>
        ''' 从网页模型之中解析出代谢模型数据
        ''' </summary>
        ''' <typeparam name="ReactionType"></typeparam>
        ''' <param name="WebForm"></param>
        ''' <returns></returns>
        Friend Function __webFormParser(Of ReactionType As Reaction)(WebForm As WebForm) As ReactionType
            Dim rn As ReactionType = Activator.CreateInstance(Of ReactionType)()

            On Error Resume Next

            rn.Entry = WebForm.GetValue("Entry").FirstOrDefault.Strip_NOBR.StripHTMLTags.StripBlank.Split.First
            rn.Comments = __trimComments(WebForm.GetValue("Comment").FirstOrDefault) _
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
            rn.Pathway = WebForm.parseList(WebForm.GetValue("Pathway").FirstOrDefault, "<a href="".+?"">.+?</a>").ValueList
            rn.Module = WebForm.parseList(WebForm.GetValue("Module").FirstOrDefault, "<a href="".+?"">.+?</a>").ValueList
            rn.CommonNames = __getCommonNames(WebForm.GetValue("Name").FirstOrDefault)
            rn.Equation = __parsingEquation(WebForm.GetValue("Equation").FirstOrDefault)
            rn.Orthology = __orthologyParser(WebForm.GetValue("Orthology").FirstOrDefault)
            rn.Class = WebForm.parseList(WebForm.GetValue("Reaction class").FirstOrDefault, "<a href="".+?"">.+?</a>").ValueList

            Dim ecTemp As String = WebForm _
                .GetValue("Enzyme") _
                .FirstOrDefault
            rn.Enzyme = Regex.Matches(ecTemp, "\d+(\.\d+)+") _
                .ToArray _
                .Distinct _
                .ToArray

            Return rn
        End Function

        <Extension>
        Private Function ValueList(keys As IEnumerable(Of KeyValuePair)) As NamedValue()
            Return keys _
                .Select(Function(k)
                            Return New NamedValue With {
                                .name = k.Key,
                                .text = k.Value
                            }
                        End Function) _
                .ToArray
        End Function

        Private Function __orthologyParser(s As String) As OrthologyTerms
            Dim ms As String() = Regex.Matches(s, "K\d+<.+?\[EC.+?\]", RegexOptions.IgnoreCase).ToArray
            Dim result = ms _
                .Select(AddressOf __innerOrthParser) _
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
        Private Function __innerOrthParser(s As String) As [Property]
            Dim t As String() = Regex.Split(s, "<[/]?a>", RegexOptions.IgnoreCase)
            Dim KO As String = t.ElementAtOrDefault(Scan0)
            Dim def As String = t.ElementAtOrDefault(1).Split("["c).First.Trim
            Dim EC As String = Regex.Match(s, "\d+(\.\d+)+").Value

            Return New [Property] With {
                .name = KO,
                .value = EC,
                .Comment = def.StripHTMLTags
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="list"></param>
        ''' <param name="EXPORT"></param>
        ''' <returns>返回成功下载的对象的数目</returns>
        ''' <remarks></remarks>
        Public Function FetchTo(list As String(), EXPORT$) As String()
            Dim failures As New List(Of String)

            For Each ID As String In list
                Dim r As Reaction = Download(ID)

                If r Is Nothing Then
                    failures += ID
                Else
                    Dim path$ = String.Format("{0}/{1}.xml", EXPORT, ID)
                    Call r.GetXml.SaveTo(path)
                End If
            Next

            Return failures.ToArray
        End Function

        Private Function __trimComments(html As String) As String
            If String.IsNullOrEmpty(html) Then
                Return ""
            End If

            Dim links As KeyValuePair(Of String, String)() = Regex _
                .Matches(html, "<a href="".+?"">.+?</a>") _
                .ToArray(Function(m) New KeyValuePair(Of String, String)(m, m.GetValue))
            Dim sb As New StringBuilder(html)

            For Each l As KeyValuePair(Of String, String) In links
                Call sb.Replace(l.Key, l.Value)
            Next
            Call sb.Replace("<br>", "")

            Return sb.ToString.StripHTMLTags
        End Function

        Private Function __parsingEquation(strData As String) As String
            Dim sb As New StringBuilder(strData)

            For Each m As Match In Regex.Matches(strData, "<a href="".+?"">.+?</a>")
                Dim link As New KeyValuePair(Of String, String)(m.Value, m.Value.GetValue)
                Call sb.Replace(link.Key, link.Value)
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

        Private Function __getCommonNames(str As String) As String()
            Return LinqAPI.Exec(Of String) <=
 _
                From line As String
                In Strings.Split(str.Strip_NOBR, "<br>")
                Let s = line.StripHTMLTags.StripBlank
                Where Not String.IsNullOrEmpty(s)
                Select s

        End Function
    End Module
End Namespace
