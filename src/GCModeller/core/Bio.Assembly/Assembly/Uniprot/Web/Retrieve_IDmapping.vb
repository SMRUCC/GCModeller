Imports System.Collections.Specialized
Imports System.Threading
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.HtmlParser

Namespace Assembly.Uniprot.Web

    Public Module Retrieve_IDmapping

        Const yes$ = NameOf(yes)
        Const no$ = NameOf(no)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="uploadQuery"></param>
        ''' <param name="from"></param>
        ''' <param name="[to]"></param>
        ''' <param name="save$"></param>
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
            Dim html As String = url.POST(args, "http://www.uniprot.org/uploadlists/",)
            Dim query$ = html.HTMLTitle.Split.First
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
            url = $"http://www.uniprot.org/uniprot/?sort={query}&desc=&compress=yes&query={query}&fil=&format=tab&force=yes&columns=yourlist({uid}),id"

            Try
                Call url.DownloadFile(save.TrimSuffix & "-mappingTable.tsv.gz")
            Catch ex As Exception
                Call App.LogException(New Exception(url, ex))
            End Try

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
        Public Function MappingReader(path$) As Dictionary(Of String, String())
            Dim lines = path.ReadAllLines.Skip(1)
            Dim maps As Dictionary(Of String, String()) = lines _
                .Select(Function(l) l.Split(ASCII.TAB)) _
                .GroupBy(Function(x) x(0)) _
                .ToDictionary(Function(x) x.Key,
                              Function(x) x.Select(
                              Function(row) row(1)).Distinct.ToArray)
            Return maps
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