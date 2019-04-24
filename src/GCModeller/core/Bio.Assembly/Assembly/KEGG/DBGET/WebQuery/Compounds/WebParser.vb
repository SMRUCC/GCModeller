Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.DBGET.WebQuery.Compounds

    Module WebParser

        ''' <summary>
        ''' 使用KEGG compound页面的url来下载代谢物数据
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function DownloadCompoundFrom(url As String) As Compound
            Return New WebForm(url).ParseCompound
        End Function

        <Extension>
        Public Function ParseCompound(html As WebForm) As Compound
            Dim links As DBLinks = GetDBLinks(html.GetValue("Other DBs").FirstOrDefault)
            Dim cpd As New Compound(links) With {
                .Entry = Regex.Match(html.GetValue("Entry").FirstOrDefault, "[GC]\d+").Value,
                .CommonNames = GetCommonNames(html.GetValue("Name").FirstOrDefault()),
                .Formula = html.GetValue("Formula").FirstOrDefault.Strip_NOBR.StripHTMLTags(stripBlank:=True),
                .reactionId = html _
                    .GetValue("Reaction") _
                    .FirstOrDefault _
                    .GetLinks() _
                    .Where(Function(s) Not s.IsShowAllLink) _
                    .ToArray,
                .Enzyme = html.GetValue("Enzyme").FirstOrDefault.GetLinks.Where(Function(s) Not s.IsShowAllLink).ToArray,
                .Pathway = html.GetValue("Pathway") _
                    .FirstOrDefault _
                    .__parseHTML_ModuleList(LIST_TYPES.Pathway) _
                    .Select(Function(s) String.Format("[{0}] {1}", s.name, s.text)) _
                    .ToArray _
                    .__parseNamedData,
                .Module = html.GetValue("Module") _
                    .FirstOrDefault _
                    .__parseHTML_ModuleList(LIST_TYPES.Module) _
                    .Select(Function(s) String.Format("[{0}] {1}", s.name, s.text)) _
                    .ToArray _
                    .__parseNamedData,
                .MolWeight = Val(html.GetValue("Mol weight").FirstOrDefault.Strip_NOBR.StripHTMLTags(stripBlank:=True)),
                .ExactMass = Val(html.GetValue("Exact mass").FirstOrDefault.Strip_NOBR.StripHTMLTags(stripBlank:=True)),
                .Remarks = html _
                    .GetValue("Remark") _
                    .Select(Function(s) s.Strip_NOBR.StripHTMLTags(stripBlank:=True).TrimNewLine) _
                    .ToArray
            }
            Return cpd
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Friend Function __parseNamedData(strData As String()) As NamedValue()
            Dim LQuery = LinqAPI.Exec(Of NamedValue) _
 _
                () <= From s As String
                      In strData
                      Let Id As String = Regex.Match(s, "\[.+?\]", RegexICSng).Value
                      Let value As String = s.Replace(Id, "").Trim
                      Select New NamedValue With {
                          .name = Id,
                          .text = value
                      }

            Return LQuery
        End Function

        Const FLAG As String = "[a-z0-9-_.]+"
        Const regexpDBLink As String = FLAG & ": (\s*<a href="".+?"">" & FLAG & "</a>\s*)+"

        Friend Function GetDBLinks(html$) As DBLinks
            If String.IsNullOrEmpty(html) Then
                Return Nothing
            End If

            Dim t$() = html.GetTablesHTML
            'Dim LQuery As DBLink() = t _
            '    .SlideWindows(winSize:=2, offset:=2) _
            '    .Where(Function(w) w.Length >= 2) _
            '    .Select(Function(s)
            '                Return s(0).StripHTMLTags(stripBlank:=True).Trim(":"c).Trim.TryParse(s(1))
            '            End Function) _
            '    .IteratesALL _
            '    .ToArray
            Dim LQuery As DBLink() = t _
                .Select(Function(linkTable)
                            Dim tr = linkTable.GetRowsHTML(0)
                            Dim tuple = tr.GetColumnsHTML
                            Dim name = tuple(0).StripHTMLTags(True).Trim(":"c, " "c)
                            Dim id$ = tuple.ElementAtOrDefault(1) _
                                           .StripHTMLTags(True) _
                                           .Trim

                            Return New DBLink(name, id)
                        End Function) _
                .ToArray

            Return New DBLinks(LQuery)
        End Function

        <Extension> Private Function TryParse(DBName$, values$) As DBLink()
            Dim IDs$() = values _
                .StripHTMLTags(stripBlank:=True) _
                .Split _
                .Select(AddressOf Trim) _
                .Where(Function(s) Not s.StringEmpty) _
                .ToArray
            Dim LQuery$ = LinqAPI.DefaultFirst(Of String) _
 _
                () <= From prefixName As String
                      In DBLinks.PrefixDB
                      Where InStr(DBName, prefixName, CompareMethod.Text) > 0
                      Select prefixName

            DBName = If(String.IsNullOrEmpty(LQuery), DBName, LQuery)

            Return IDs.Select(Function(ID$)
                                  Return New DBLink With {
                                      .DBName = DBName,
                                      .Entry = ID
                                  }
                              End Function) _
                      .ToArray
        End Function

        Friend Function GetCommonNames(str$) As String()
            If String.IsNullOrEmpty(str) Then
                Return New String() {}
            End If

            Dim buf As String() = str.Strip_NOBR.HtmlLines
            Dim names = LinqAPI.Exec(Of String) _
 _
                () <= From s As String
                      In buf
                      Let line As String = s _
                          .StripHTMLTags(stripBlank:=True) _
                          .Trim(";"c, " "c)
                      Where Not String.IsNullOrEmpty(line)
                      Select line

            Return names
        End Function
    End Module
End Namespace