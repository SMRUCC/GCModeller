Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Module MetabolitesDBGet

        Const URL = "http://www.kegg.jp/dbget-bin/www_bget?cpd:{0}"

        ''' <summary>
        ''' 使用KEGG compound的编号来下载代谢物数据
        ''' </summary>
        ''' <param name="Id"></param>
        ''' <returns></returns>
        Public Function DownloadCompound(Id As String) As Compound
            Return DownloadCompoundFrom(url:=String.Format(URL, Id))
        End Function

        ''' <summary>
        ''' 使用KEGG compound页面的url来下载代谢物数据
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        Public Function DownloadCompoundFrom(url As String) As Compound
            Dim html As New WebForm(url)
            Dim links As DBLinks = GetDBLinks(html.GetValue("Other DBs").FirstOrDefault)
            Dim cpd As New Compound(links) With {
                .Entry = Regex.Match(html.GetValue("Entry").FirstOrDefault, "[GC]\d+").Value
            }
            cpd.CommonNames = GetCommonNames(html.GetValue("Name").FirstOrDefault())
            cpd.Formula = html.GetValue("Formula").FirstOrDefault.Replace("<br>", "")
            cpd.KEGG_reaction = GetReactionList(html.GetValue("Reaction").FirstOrDefault)
            cpd.Pathway = (From s As KeyValuePair In WebForm.parseList(html.GetValue("Pathway").FirstOrDefault, "<a href="".*/kegg-bin/show_pathway\?.+?"">.+?</a>") Select String.Format("[{0}] {1}", s.Key, s.Value)).ToArray
            cpd.Module = (From s As KeyValuePair In WebForm.parseList(html.GetValue("Module").FirstOrDefault, "<a href="".*/kegg-bin/show_module\?.+?"">.+?</a>") Select String.Format("[{0}] {1}", s.Key, s.Value)).ToArray
            cpd.MolWeight = Val(html.GetValue("Mol weight").FirstOrDefault)

            Return cpd
        End Function

        ''' <summary>
        ''' 下载指定编号集合的代谢物数据，并保存到指定的文件夹之中
        ''' </summary>
        ''' <param name="list">KEGG compound id list</param>
        ''' <param name="EXPORT"></param>
        ''' <returns>返回下载失败的对象的编号列表</returns>
        ''' <remarks></remarks>
        Public Function FetchTo(list As String(), EXPORT As String) As String()
            Dim failures As New List(Of String)
            Dim path$

            Call $"{list.Length} KEGG compounds are going to download!".__DEBUG_ECHO

            For Each cpdID As String In list

                path = String.Format("{0}/{1}.xml", EXPORT, cpdID)

                If Not path.FileExists Then
                    Dim CompoundData As Compound = DownloadCompound(cpdID)

                    If CompoundData Is Nothing Then
                        failures += cpdID
                    Else
                        Call CompoundData.GetXml.SaveTo(path)
                    End If
                End If
            Next

            Return failures
        End Function

        Const FLAG As String = "[a-z0-9-_.]+"
        Const REGEX_DBLINK As String = FLAG & ": (\s*<a href="".+?"">" & FLAG & "</a>\s*)+"

        Friend Function GetDBLinks(strData As String) As DBLinks
            If String.IsNullOrEmpty(strData) Then
                Return Nothing
            End If

            Dim Tokens As String() = (From m As Match
                                      In Regex.Matches(strData, REGEX_DBLINK, RegexOptions.IgnoreCase)
                                      Select m.Value).ToArray
            Dim LQuery As IEnumerable(Of DBLink()) = From s As String In Tokens Select TryParse(s)
            Return New DBLinks(LQuery.IteratesALL)
        End Function

        Private Function TryParse(str As String) As DBLink()
            Dim TempChunk As String() = Strings.Split(str, ": ")
            Dim DBName As String = TempChunk.First
            Dim Entries As String() = GetValues(TempChunk.Last)
            Dim LQuery As String = (From prefixName As String
                                    In ComponentModel.DBLinkBuilder.DBLinks.PrefixDB
                                    Where InStr(DBName, prefixName, CompareMethod.Text) > 0
                                    Select prefixName).FirstOrDefault

            DBName = If(String.IsNullOrEmpty(LQuery), DBName, LQuery)

            Return (From s As String
                    In Entries
                    Select New DBLink With {.DBName = DBName, .Entry = s}).ToArray
        End Function

        Private Function GetValues(str As String) As String()
            Dim buf As String() =
                Regex.Matches(str, "<a href="".+?"">.+?</a>") _
               .ToArray(AddressOf HtmlStrips.GetValue)
            Return buf
        End Function

        Friend Function GetReactionList(strData As String) As String()
            If String.IsNullOrEmpty(strData) Then
                Return New String() {}
            End If

            Dim buf As String() =
                Regex.Matches(strData, "<a href="".+?"">.+?</a>", RegexOptions.Singleline) _
               .ToArray(AddressOf HtmlStrips.GetValue)
            Return buf
        End Function

        Friend Function GetCommonNames(strData As String) As String()
            If String.IsNullOrEmpty(strData) Then
                Return New String() {}
            End If

            Dim buf As String() = Strings.Split(strData, "<br>")
            buf = (From s As String
                   In buf
                   Let strItem As String = s.Replace(";", "").Trim
                   Where Not String.IsNullOrEmpty(strItem)
                   Select strItem).ToArray

            Return buf
        End Function
    End Module
End Namespace