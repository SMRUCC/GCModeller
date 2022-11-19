#Region "Microsoft.VisualBasic::83fbf93fd3e06f5ab2109df97e02f32a, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\SSDB\Models\SShit.vb"

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

    '   Total Lines: 129
    '    Code Lines: 114
    ' Comment Lines: 0
    '   Blank Lines: 15
    '     File Size: 6.30 KB


    '     Class BlastnHit
    ' 
    '         Properties: Bits, Definition, Eval, KO, LocusId
    ' 
    '         Function: ToString
    ' 
    '     Class SShit
    ' 
    '         Properties: Best, Bits, Coverage, Entry, EntryUrl
    '                     Identity, KO, Length, Margin, Overlap
    '                     SWScore
    ' 
    '         Function: CreateObject, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace Assembly.KEGG.DBGET.bGetObject.SSDB

    Public Class BlastnHit
        Public Property LocusId As String
        Public Property Definition As String
        Public Property KO As String
        Public Property Bits As Double
        Public Property Eval As Double

        Public Overrides Function ToString() As String
            Return $"[{KO}]{LocusId}  =>  {Definition}"
        End Function
    End Class

    Public Class SShit

        Public Property Entry As QueryEntry
        Public Property EntryUrl As String
        Public Property KO As KeyValuePair
        <XmlAttribute> Public Property Length As String
        <XmlAttribute> Public Property SWScore As String
        <XmlAttribute> Public Property Margin As String
        <XmlAttribute> Public Property Bits As String
        <XmlAttribute> Public Property Identity As String
        <XmlAttribute> Public Property Overlap As String
        Public Property Best As KeyValuePair

        Public ReadOnly Property Coverage As Double
            Get
                Return Val(Overlap) / Val(Length)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Entry.ToString
        End Function

        Public Const REGEX_VALUE_ARRAY As String = "</a>\s+\d+\s+\d+\(\s+\d+\).+<a href='.+' target=_ortholog>.+</a>"

        Public Shared Function CreateObject(strData As String) As SShit
            Dim ResultItem As SShit = New SShit
            Dim Tokens = Strings.Split(strData, "   ")
            Dim EntryValue As String = Strings.Split(Tokens.First, "  ").First  'Regex.Match(strData, "VALUE="".+?""( [^>]+)?><A HREF="".+?"" TARGET="".+?"">.+?</A> .+?<A").Value
            ResultItem.EntryUrl = EntryValue.href
            Dim EntryTemp As String() = New String() {Regex.Match(EntryValue, "VALUE="".+?""", RegexOptions.IgnoreCase).Value, Strings.Split(EntryValue, "</A>").Last}
            EntryTemp(0) = Mid(EntryTemp(0), 8)
            EntryTemp(0) = Mid(EntryTemp(0), 1, Len(EntryTemp(0)) - 1)
            If InStr(EntryTemp(1), "<A HREF") Then
                EntryTemp(1) = Strings.Split(EntryTemp(1), "<A HREF").First
            End If
            EntryTemp(1) = EntryTemp(1).Trim
            Dim TempChunk As String() = EntryTemp(0).Split(CChar(":"))
            ResultItem.Entry = New KEGG.WebServices.QueryEntry With {
                .description = EntryTemp(1),
                .speciesID = TempChunk(0),
                .locusID = TempChunk(1)
            }

            Dim strTemp As String = Regex.Match(strData, "<A HREF=""[^>]+?""  TARGET=""_blank"">K.+?</a>", RegexOptions.IgnoreCase).Value
            If Not String.IsNullOrEmpty(strTemp) Then
                TempChunk = New String() {strTemp.href, Regex.Match(strTemp, ">[^>]+?</a>").Value}
                TempChunk(1) = Mid(TempChunk(1), 2)
                TempChunk(1) = Mid(TempChunk(1), 1, Len(TempChunk(1)) - 4)
                ResultItem.KO = New KeyValuePair With {
                    .Key = TempChunk(0),
                    .Value = TempChunk(1)
                }

                Dim strValue As String() = (From n As String
                                            In (From item As Match In Regex.Matches(strData, "</a>[^<]+<a", RegexOptions.IgnoreCase) Select item.Value).ToArray.Last.Split
                                            Where Not String.IsNullOrEmpty(n.Trim)
                                            Select n).ToArray
                Dim BestValue As String = (From item As Match
                                           In Regex.Matches(strData, "<a href='.+?' target=_ortholog>\d+</a>", RegexOptions.IgnoreCase)
                                           Select item.Value).ToArray.Last
                Dim p As i32 = 1

                ResultItem.Length = strValue(++p)
                ResultItem.SWScore = strValue(++p) : p += 1
                ResultItem.Margin = Val(strValue(++p))
                ResultItem.Bits = strValue(++p)
                ResultItem.Identity = strValue(++p)
                ResultItem.Overlap = strValue(++p)
                ResultItem.Best = New KeyValuePair With {
                    .Key = strValue(p).Replace("&lt;", "<").Replace("&gt;", ">"),
                    .Value = Regex.Match(BestValue, ">\d+</a>").Value
                }
                ResultItem.Best.Value = Mid(ResultItem.Best.Value, 2)
                ResultItem.Best.Value = Mid(ResultItem.Best.Value, 1, Len(ResultItem.Best.Value) - 4)
            Else
                ResultItem.KO = New KeyValuePair With {
                    .Key = "",
                    .Value = ""
                }
                strData = Regex.Match(strData, "\s+\d+\d+\d+.+").Value

                Dim strValue As String() = (From n In strData.Split Where Not String.IsNullOrEmpty(n.Trim) Select n).ToArray
                Dim BestValue As String = (From item As Match In Regex.Matches(strData, "<a href='.+?' target=_ortholog>\d+</a>", RegexOptions.IgnoreCase) Select item.Value).ToArray.Last
                Dim p As i32 = 0

                ResultItem.Length = strValue(++p)
                ResultItem.SWScore = strValue(++p) : p += 1
                If String.Equals(ResultItem.SWScore, "(") Then
                    ResultItem.SWScore = ""
                    p -= 1
                End If
                ResultItem.Margin = Val(strValue(++p))
                ResultItem.Bits = strValue(++p)
                ResultItem.Identity = strValue(++p)
                ResultItem.Overlap = strValue(++p)
                ResultItem.Best = New KeyValuePair With {
                    .Key = strValue(p).Replace("&lt;", "<").Replace("&gt;", ">"),
                    .Value = Regex.Match(BestValue, ">\d+</a>").Value
                }
                ResultItem.Best.Value = Mid(ResultItem.Best.Value, 2)
                ResultItem.Best.Value = Mid(ResultItem.Best.Value, 1, Len(ResultItem.Best.Value) - 4)
            End If

            Return ResultItem
        End Function
    End Class
End Namespace
