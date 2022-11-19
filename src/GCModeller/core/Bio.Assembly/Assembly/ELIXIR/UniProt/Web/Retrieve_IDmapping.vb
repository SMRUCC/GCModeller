#Region "Microsoft.VisualBasic::bb5a1053fc1093da187323ebb6d36b64, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\UniProt\Web\Retrieve_IDmapping.vb"

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

    '   Total Lines: 295
    '    Code Lines: 158
    ' Comment Lines: 103
    '   Blank Lines: 34
    '     File Size: 12.22 KB


    '     Module Retrieve_IDmapping
    ' 
    '         Function: GetMappingList, IDTypeParser, MappingReader, MappingsReader, SingleMappings
    '                   UniprotIDFilter
    ' 
    '         Sub: Mapping
    ' 
    '     Enum Formats
    ' 
    '         canonical, gff, isoform, list, mappingTable
    '         rdf, tab, txt, xlsx, xml
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Namespace Assembly.Uniprot.Web

    Public Module Retrieve_IDmapping

        ReadOnly idTypes As Dictionary(Of String, ID_types) =
            Enums(Of ID_types) _
            .ToDictionary(Function(id) id.Description)

        Public Function IDTypeParser(value$, Optional [default] As ID_types = ID_types.P_REFSEQ_AC) As ID_types
            value = value.ToUpper

            If idTypes.ContainsKey(value) Then
                Return idTypes(value)
            Else
                Return [default]
            End If
        End Function

        Const yes$ = NameOf(yes)
        Const no$ = NameOf(no)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="uploadQuery"></param>
        ''' <param name="from"></param>
        ''' <param name="[to]"></param>
        ''' <param name="save$">这是一个文件名来的</param>
        ''' <param name="compress$">
        ''' 假若这个参数为<see cref="yes"/>的话，下载的是一个``*.gz``格式的压缩文件
        ''' </param>
        ''' <param name="format"></param>
        Public Sub Mapping(uploadQuery As IEnumerable(Of String),
                           from As ID_types,
                           [to] As ID_types,
                           save$,
                           Optional compress$ = yes,
                           Optional format As Formats = Formats.xml)

            Dim args As New NameValueCollection

            Call args.Add(NameOf(from), from.ToString)
            Call args.Add(NameOf([to]), [to].ToString)
            Call args.Add(NameOf(uploadQuery), uploadQuery.JoinBy(vbLf))

            Dim url$ = "http://www.uniprot.org/uploadlists/"
            Dim html As String = url.POST(args, , "http://www.uniprot.org/uploadlists/",).html
            Dim query$ = html.HTMLTitle.Split.First

            ' 2017-3-7
            ' 由于IE内核的版本不同的原因，所返回来的html文本会有些差异，所以对于query编号的解析会有些差异
            ' 这里是处理win7老平台上面的query解析的操作
            If query = "mapping" Then
                query = Regex.Match(html, "new mappingResults\('.+?'\)", RegexICSng).Value
                query = query.GetStackValue("(", ")")
                query = query.Trim("'"c)
                query = "yourlist:" & query
            End If

            Dim uid$ = query.Split(":"c).Last

            Call Thread.Sleep(1000)

            ' http://www.uniprot.org/uniprot/
            ' query=yourlist:M20170110ACFE4208EAFA842A78A1B3BA7138A93D9543F8P
            ' sort=yourlist:M20170110ACFE4208EAFA842A78A1B3BA7138A93D9543F8P
            ' columns=yourlist(M20170110ACFE4208EAFA842A78A1B3BA7138A93D9543F8P),isomap(M20170110ACFE4208EAFA842A78A1B3BA7138A93D9543F8P),id,entry%20name,reviewed,protein%20names,genes,organism,length
            'url = "http://www.uniprot.org/uniprot/?"
            'url &= "query=" & query & "&"
            'url &= "sort=" & query & "&"
            'url &= $"columns=yourlist({uid}),isomap({uid}),id,entry%20name,reviewed,protein%20names,genes,organism,length"
            'html = url.GET()
            'html = Strings.Split(html, "UniProtKB Results").Last

            'Dim out As New Dictionary(Of String, List(Of String))
            'Dim table = html.GetTablesHTML.FirstOrDefault
            'Dim rows = table.GetRowsHTML

            'For Each row As String In rows.Skip(2)
            '    Dim columns$() = row.GetColumnsHTML
            '    Dim queryId$ = columns(1)
            '    Dim mapId$ = columns(3).GetValue

            '    If Not out.ContainsKey(queryId) Then
            '        Call out.Add(queryId, New List(Of String))
            '    End If

            '    Call out(queryId).Add(mapId)
            'Next

            ' http://www.uniprot.org/uniprot/?sort=yourlist:M20170111AAFB7E4D2F1D05654627429E83DA5CCEA02970F&desc=&compress=yes&query=yourlist:M20170111AAFB7E4D2F1D05654627429E83DA5CCEA02970F&fil=&format=tab&force=yes&columns=yourlist(M20170111AAFB7E4D2F1D05654627429E83DA5CCEA02970F),id
            ' http://www.uniprot.org/uniprot/?sort=yourlist:M20170307A7434721E10EE6586998A056CCD0537E86F2B0I&desc=&compress=yes&query=yourlist:M20170307A7434721E10EE6586998A056CCD0537E86F2B0I&fil=&format=tab&force=yes&columns=yourlist(M20170307A7434721E10EE6586998A056CCD0537E86F2B0I),id
            url = $"http://www.uniprot.org/uniprot/?sort={query}&desc=&compress=yes&query={query}&fil=&format=tab&force=yes&columns=yourlist({uid}),id"

            Try
                Call url.DownloadFile(save.TrimSuffix & "-mappingTable.tsv.gz")
            Catch ex As Exception
                Call App.LogException(New Exception(url, ex))
            End Try

            ' http://www.uniprot.org/uniprot/?sort=yourlist:M20170307A7434721E10EE6586998A056CCD0537E86F2B0I&desc=&compress=yes&query=yourlist:M20170307A7434721E10EE6586998A056CCD0537E86F2B0I&fil=&format=xml&force=yes
            url = $"http://www.uniprot.org/uniprot/?sort={query}&desc=&compress={compress}&query={query}&fil=&format={format}&force=yes"

            Try
                Call url.DownloadFile(save)
            Catch ex As Exception
                Call App.LogException(New Exception(url, ex))
            End Try
        End Sub

        ''' <summary>
        ''' 读取从uniprot上面所下载下来的id mapping表
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 可能每一行之中会存在多对多的情况，但是不需要担心，这个函数会自动处理这些非一对一的情况的
        ''' </remarks>
        <Extension>
        Public Function MappingReader(path As String) As Dictionary(Of String, String())
            If Not path.FileExists Then
                Return Nothing
            End If

            Dim lines = path.ReadAllLines.Skip(1)
            Dim maps As Dictionary(Of String, String()) = lines _
                .Select(Function(l) l.Split(ASCII.TAB)) _
                .Select(Function(t)
                            Dim mapped$() = t(1).Split(","c)
                            Return t(0) _
                                .Split _
                                .Select(Function(k)
                                            Return New KeyValuePair(Of String, String())(k, mapped)
                                        End Function)
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(x) x.Key) _
                .Select(Function(k)
                            Dim values$() = k.Select(Function(v) v.Value) _
                                .IteratesALL _
                                .Distinct _
                                .ToArray
                            Return k.Key.Split(","c) _
                                .Select(Function(key$)
                                            Return New KeyValuePair(Of String, String())(key, values)
                                        End Function)
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(g) g.Key) _
                .ToDictionary(Function(k) k.Key,
                              Function(g)
                                  Return g.Select(Function(v) v.Value) _
                                      .IteratesALL _
                                      .Distinct _
                                      .OrderBy(Function(id) id) _
                                      .ToArray
                              End Function)
            Return maps
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="mappings"></param>
        ''' <param name="initials">Q开头的编号一般都是reviewed状态的蛋白数据</param>
        ''' <returns></returns>
        <Extension>
        Public Function UniprotIDFilter(mappings As Dictionary(Of String, String()), Optional initials$ = "QPO") As Dictionary(Of String, String)
            Dim table As New Dictionary(Of String, String)

            For Each idMaps In mappings
                If idMaps.Value.Length = 1 Then
                    table(idMaps.Key) = idMaps.Value.First
                ElseIf idMaps.Value.IsNullOrEmpty Then
                    ' Do Nothing
                Else ' > 1
                    For Each init As Char In initials
                        Dim id = idMaps.Value.Where(Function(s) s.First = init).FirstOrDefault
                        If Not id.StringEmpty Then
                            table(idMaps.Key) = id
                            Exit For
                        End If
                    Next

                    If Not table.ContainsKey(idMaps.Key) Then
                        table(idMaps.Key) = idMaps.Value.First
                    End If
                End If
            Next

            Return table
        End Function

        ''' <summary>
        ''' 与<see cref="MappingReader"/>所不同的是，这个函数是读取一个文件夹之中的所有的
        ''' mapping table(``*.tsv``, ``*.tab``)作为一个mapping数据的整体来使用的
        ''' </summary>
        ''' <param name="DIR$"></param>
        ''' <returns></returns>
        Public Function MappingsReader(DIR$) As Dictionary(Of String, String())
            Dim mappings As Dictionary(Of String, String()) = DIR _
                .EnumerateFiles("*.tab", "*.tsv") _
                .Select(AddressOf Retrieve_IDmapping.MappingReader) _
                .IteratesALL _
                .GroupBy(Function(k) k.Key) _
                .ToDictionary(Function(k) k.Key,
                              Function(g)
                                  Return g.Select(Function(v) v.Value) _
                                      .IteratesALL _
                                      .Distinct _
                                      .ToArray
                              End Function)
            Return mappings
        End Function

        ''' <summary>
        ''' 假若在mapping表之中不存在重复的基因编号的话，可以使用这个函数
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 其实这个函数就是直接读取行数据然后依据TAB符号进行分割之后写入字典之中
        ''' </remarks>
        Public Function SingleMappings(path$) As Dictionary(Of String, String)
            Dim out As New Dictionary(Of String, String)

            For Each line As String In path.ReadAllLines.Skip(1)
                Dim t$() = line.Split(ASCII.TAB)
                Call out.Add(t(0), t(1))
            Next

            Return out
        End Function

        ''' <summary>
        ''' 得到mapping的所有的基因编号
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        Public Function GetMappingList(path$) As String()
            Return MappingReader(path).Values.Unlist.Distinct.ToArray
        End Function
    End Module

    Public Enum Formats
        ''' <summary>
        ''' FASTA (canonical)
        ''' </summary>
        canonical
        ''' <summary>
        ''' FASTA (canonical &amp; isoform)
        ''' </summary>
        isoform
        ''' <summary>
        ''' Tab-separated
        ''' </summary>
        tab
        ''' <summary>
        ''' Text
        ''' </summary>
        txt
        ''' <summary>
        ''' Excel
        ''' </summary>
        xlsx
        ''' <summary>
        ''' GFF
        ''' </summary>
        gff
        ''' <summary>
        ''' XML
        ''' </summary>
        xml
        ''' <summary>
        ''' Mapping Table
        ''' </summary>         
        mappingTable
        ''' <summary>
        ''' RDF/XML
        ''' </summary>
        rdf
        ''' <summary>
        ''' Target List
        ''' </summary>
        list
    End Enum
End Namespace
