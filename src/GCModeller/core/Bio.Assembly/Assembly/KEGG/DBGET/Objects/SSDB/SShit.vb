Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.KEGG.WebServices
Imports Microsoft.VisualBasic.ComponentModel

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
            ResultItem.EntryUrl = EntryValue.Get_href
            Dim EntryTemp As String() = New String() {Regex.Match(EntryValue, "VALUE="".+?""", RegexOptions.IgnoreCase).Value, Strings.Split(EntryValue, "</A>").Last}
            EntryTemp(0) = Mid(EntryTemp(0), 8)
            EntryTemp(0) = Mid(EntryTemp(0), 1, Len(EntryTemp(0)) - 1)
            If InStr(EntryTemp(1), "<A HREF") Then
                EntryTemp(1) = Strings.Split(EntryTemp(1), "<A HREF").First
            End If
            EntryTemp(1) = EntryTemp(1).Trim
            Dim TempChunk As String() = EntryTemp(0).Split(CChar(":"))
            ResultItem.Entry = New KEGG.WebServices.QueryEntry With {
                .Description = EntryTemp(1),
                .SpeciesId = TempChunk(0),
                .LocusId = TempChunk(1)
            }

            Dim strTemp As String = Regex.Match(strData, "<A HREF=""[^>]+?""  TARGET=""_blank"">K.+?</a>", RegexOptions.IgnoreCase).Value
            If Not String.IsNullOrEmpty(strTemp) Then
                TempChunk = New String() {strTemp.Get_href, Regex.Match(strTemp, ">[^>]+?</a>").Value}
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
                Dim p As Integer = 1

                ResultItem.Length = strValue(p.MoveNext)
                ResultItem.SWScore = strValue(p.MoveNext) : p += 1
                ResultItem.Margin = Val(strValue(p.MoveNext))
                ResultItem.Bits = strValue(p.MoveNext)
                ResultItem.Identity = strValue(p.MoveNext)
                ResultItem.Overlap = strValue(p.MoveNext)
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
                Dim p As Integer = 0

                ResultItem.Length = strValue(p.MoveNext)
                ResultItem.SWScore = strValue(p.MoveNext) : p += 1
                If String.Equals(ResultItem.SWScore, "(") Then
                    ResultItem.SWScore = ""
                    p -= 1
                End If
                ResultItem.Margin = Val(strValue(p.MoveNext))
                ResultItem.Bits = strValue(p.MoveNext)
                ResultItem.Identity = strValue(p.MoveNext)
                ResultItem.Overlap = strValue(p.MoveNext)
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