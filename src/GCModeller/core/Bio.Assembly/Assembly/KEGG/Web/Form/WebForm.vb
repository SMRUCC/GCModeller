#Region "Microsoft.VisualBasic::89326d0dcb6557d4718f82f9d4664da2, GCModeller\core\Bio.Assembly\Assembly\KEGG\Web\Form\WebForm.vb"

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

    '   Total Lines: 314
    '    Code Lines: 223
    ' Comment Lines: 40
    '   Blank Lines: 51
    '     File Size: 12.61 KB


    '     Class WebForm
    ' 
    '         Properties: AllLinksWidget, Count, Keys, References, Values
    '                     WebPageTitle
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ContainsKey, GetEnumerator, GetEnumerator1, getHtml, GetRaw
    '                   GetValue, parseList, parseListInternal, RegexReplace, RemoveHrefLink
    '                   ToString, TryGetValue
    ' 
    '         Sub: (+2 Overloads) Dispose, ParseRefList
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Option Strict Off

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.WebServices.InternalWebFormParsers

    ''' <summary>
    ''' KEGG 网页表格的数据解析方法，在Value之中可能会有重复的Key数据出现
    ''' </summary>
    ''' <remarks></remarks>
    Public Class WebForm : Implements IReadOnlyDictionary(Of String, String())
        Implements IDisposable

        ''' <summary>
        ''' Entry, {trim_formatted, non-process}
        ''' </summary>
        Dim _strData As SortedDictionary(Of String, NamedValue(Of String)())
        Dim _url As String

        Public ReadOnly Property WebPageTitle As String
        Public Property AllLinksWidget As AllLinksWidget

        ''' <summary>
        ''' 这个构造函数同时支持url或者文本内容
        ''' </summary>
        ''' <param name="resource"></param>
        Sub New(resource As String)
            Dim html As String = getHtml(resource)
            Dim tokens As String() = r.Split(html, "<th class="".+?"" align="".+?""").Skip(1).ToArray
            Dim tmp As String() = LinqAPI.Exec(Of String) <=
 _
                From strValue As String
                In tokens
                Let value As String = r.Match(strValue, "<nobr>.+?</nobr>.+", RegexOptions.Singleline).Value.Trim
                Where Not String.IsNullOrEmpty(value)
                Select value

            Me._WebPageTitle = html.HTMLTitle
            Me._strData = New SortedDictionary(Of String, NamedValue(Of String)())

            Dim fields = LinqAPI.Exec(Of NamedValue(Of String())) <=
 _
                From strValue As String
                In tmp
                Let Key As String = r.Match(strValue, "<nobr>.+?</nobr>").Value
                Let Value As String = RegexReplace(strValue.Replace(Key, ""), WebForm.HtmlFormatControl)
                Select New NamedValue(Of String()) With {
                    .Name = Key.GetValue,
                    .Value = {Value.TrimNewLine, strValue}
                }
            Dim allKeys As IEnumerable(Of String) = From item As NamedValue(Of String())
                                                    In fields
                                                    Let KeyValue As String = item.Name
                                                    Select KeyValue
                                                    Distinct
            For Each Key As String In allKeys
                Dim vals As NamedValue(Of String)() = LinqAPI.Exec(Of NamedValue(Of String)) <=
 _
                    From item As NamedValue(Of String())
                    In fields
                    Where String.Equals(Key, item.Name)
                    Select New NamedValue(Of String) With {
                        .Name = item.Value(Scan0),
                        .Value = item.Value(1)
                    }

                Call _strData.Add(Key, vals)
            Next

            AllLinksWidget = InternalWebFormParsers.AllLinksWidget.InternalParser(html)
            Call ParseRefList(html)
        End Sub

        Private Shared Function getHtml(res As String) As String
            Dim html$

            If res.IsURLPattern Then
                html = res.GET
            Else
                html = res
            End If

            Return html _
                .Replace("&nbsp;", " ") _
                .Replace("&gt;", ">") _
                .Replace("&lt;", "<")
        End Function

        Private Sub ParseRefList(Page As String)
            Dim list As String() = Strings.Split(Page, "<nobr>Reference</nobr></th>").Skip(1).ToArray
            Me.References = bGetObject.Reference.References(list)
        End Sub

        ''' <summary>
        ''' Reference list of this biological object
        ''' </summary>
        ''' <returns></returns>
        Public Property References As bGetObject.Reference()

        Private Shared ReadOnly HtmlFormatControl As String() = {
            "<td .+?>",
            "<div .+?>",
            "</th>|</div>|</td>|</tr>|<tr>|<tbody>|<div>|</tbody>|</table>|<nobr>|</nobr>",
            "<table .+?>"
        }

        Protected Friend Shared Function parseList(html$, splitRegx$) As NamedValue()
            If String.IsNullOrEmpty(html) Then
                Return {}
            Else
                Return parseListInternal(html, splitRegx)
            End If
        End Function

        Private Shared Function parseListInternal(html$, splitRegx$) As NamedValue()
            Dim componentList As New List(Of NamedValue)
            Dim bufs As String() = LinqAPI.Exec(Of String) <=
 _
                From m As Match
                In r.Matches(html, splitRegx)
                Select m.Value
                Distinct

            For i As Integer = 0 To bufs.Length - 2
                Dim p1 As Integer = InStr(html, bufs(i))
                Dim p2 As Integer = InStr(html, bufs(i + 1))
                Dim strTemp As String = Mid(html, p1, p2 - p1)

                Dim entry As String = r.Match(strTemp, splitRegx).Value
                Dim cps_Describ As String = strTemp.Replace(entry, "").Trim

                entry = entry.GetValue
                cps_Describ = WebForm.RemoveHrefLink(cps_Describ)

                componentList += New NamedValue With {
                    .name = entry,
                    .text = cps_Describ
                }
            Next

            If bufs.Length > 0 Then
                Dim p As Integer = InStr(html, bufs.Last)
                html = Mid(html, p)
                Dim last As New NamedValue With {
                    .name = r.Match(html, splitRegx).Value,
                    .text = WebForm.RemoveHrefLink(html.Replace(.name, "").Trim)
                }

                last.name = last.name.GetValue

                Call componentList.Add(last)
            End If

            For Each cpd As NamedValue In componentList
                cpd.name = cpd.name.StripHTMLTags.Trim({ASCII.TAB, ASCII.CR, ASCII.LF, " "c})
                cpd.text = cpd.text.StripHTMLTags.Trim({ASCII.TAB, ASCII.CR, ASCII.LF, " "c})
            Next

            Return componentList _
                .Where(Function(li) li.name <> "DBGET") _
                .ToArray
        End Function

        ''' <summary>
        ''' 将符合目标规则的字符串替换为空字符串
        ''' </summary>
        ''' <param name="strData"></param>
        ''' <param name="ExprCollection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function RegexReplace(strData As String, ExprCollection As String()) As String
            For Each strItem As String In ExprCollection
                strData = r.Replace(strData, strItem, "")
            Next
            Return strData
        End Function

        ''' <summary>
        ''' 获取某一个字段的数据
        ''' </summary>
        ''' <param name="KeyWord">网页的表格之中的最左端的字段名</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetValue(KeyWord As String) As String()
            If _strData.ContainsKey(KeyWord) Then
                Return _strData.Item(KeyWord).Select(Function(x) x.Value).ToArray
            Else
                Return New String() {""}
            End If
        End Function

        Public Function GetRaw(Keyword As String) As String()
            If _strData.ContainsKey(Keyword) Then
                Return _strData.Item(Keyword).Select(Function(x) x.Value).ToArray
            Else
                Return New String() {""}
            End If
        End Function

        Public Overrides Function ToString() As String
            Return WebPageTitle
        End Function

        Const HREF As String = "<a href="".+?"">.+?</a>"

        Public Shared Function RemoveHrefLink(strValue As String) As String
            If String.IsNullOrEmpty(strValue) Then
                Return ""
            End If

            Dim sBuilder As New StringBuilder(strValue)
            Dim Links = From m As Match
                        In Regex.Matches(strValue, HREF)
                        Select Original = m.Value,
                            Value = m.Value.GetValue

            For Each LinkItem In Links
                Call sBuilder.Replace(LinkItem.Original, LinkItem.Value)
            Next

            Return sBuilder.ToString
        End Function

#Region "Implements IReadOnlyDictionary(Of String, String)"

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, String())) Implements IEnumerable(Of KeyValuePair(Of String, String())).GetEnumerator
            For Each Item As KeyValuePair(Of String, NamedValue(Of String)()) In Me._strData
                Yield New KeyValuePair(Of String, String())(Item.Key, Item.Value.Select(Function(obj) obj.Value).ToArray)
            Next
        End Function

        Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of String, String())).Count
            Get
                Return _strData.Count
            End Get
        End Property

        Public Function ContainsKey(key As String) As Boolean Implements IReadOnlyDictionary(Of String, String()).ContainsKey
            Return _strData.ContainsKey(key)
        End Function

        Default Public ReadOnly Property Item(key As String) As String() Implements IReadOnlyDictionary(Of String, String()).Item
            Get
                Return GetValue(key)
            End Get
        End Property

        Public ReadOnly Property Keys As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, String()).Keys
            Get
                Return _strData.Keys.ToArray
            End Get
        End Property

        Public Function TryGetValue(key As String, ByRef value As String()) As Boolean Implements IReadOnlyDictionary(Of String, String()).TryGetValue
            Dim raw As NamedValue(Of String)() = Nothing
            Dim f As Boolean = _strData.TryGetValue(key, raw)
            value = raw.Select(Function(obj) obj.Value).ToArray
            Return f
        End Function

        Public ReadOnly Property Values As IEnumerable(Of String()) Implements IReadOnlyDictionary(Of String, String()).Values
            Get
                Return _strData.Values.Select(Function(tuple) tuple.Values).ToArray
            End Get
        End Property

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
