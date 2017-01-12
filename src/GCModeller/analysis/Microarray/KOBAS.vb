Imports System.Collections.Specialized
Imports System.Text.RegularExpressions

''' <summary>
''' 做富集分析使用的
''' </summary>
Public Module KOBAS

    Public Function GenelistEnrichment(list$, SpeciesSearch$, Optional input_type$ = "id:uniprot") As String
        Dim args As New Dictionary(Of String, String())

        If list.FileExists(True) Then
            list = list.ReadAllText
        End If

        Call args.Add(NameOf(SpeciesSearch), {SpeciesSearch})
        Call args.Add(NameOf(input_type), {input_type})
        Call args.Add("input_seq1", {list})
        Call args.Add("kobasdb[]", {"G", "K"})
        Call args.Add("Run", {"Run"})

        Dim html$ = "http://kobas.cbi.pku.edu.cn/run_kobas.php".POST(args, Referer:="http://kobas.cbi.pku.edu.cn/anno_iden.php")
        Dim link As String = Regex.Match(html, "javascript:window.location.href='./download_file.php?type=run_kobas&userid=.+?'", RegexICSng).Value
        Dim tmp = App.GetAppSysTempFile

        link = "http://kobas.cbi.pku.edu.cn/" & link.GetStackValue("'", "'").Trim("."c, "/"c)
        link.DownloadFile(tmp)
        tmp = tmp.ReadAllText

        Return tmp
    End Function
End Module
