Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Terminal
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' 通过这个模块从KEGG服务器上下载人类基因组数据
    ''' </summary>
    Public Module KEGGgenomeFetch

        ''' <summary>
        ''' 使用基因编号来下载基因注释数据
        ''' </summary>
        ''' <param name="id$">人类基因的编号</param>
        ''' <returns></returns>
        Public Function DownloadHSA(id$) As HumanGene
            Return DownloadURL($"http://www.kegg.jp/dbget-bin/www_bget?hsa:{id}")
        End Function

        ''' <summary>
        ''' 从指定页面的url下载基因数据
        ''' </summary>
        ''' <param name="url$"></param>
        ''' <returns></returns>
        <Extension> Public Function DownloadURL(url$) As HumanGene
            Return New WebForm(url).ParseModel
        End Function

        ''' <summary>
        ''' 从KEGG网页表单之中解析出基因的注释数据
        ''' </summary>
        ''' <param name="html"></param>
        ''' <returns></returns>
        <Extension> Public Function ParseModel(html As WebForm) As HumanGene
            Dim hsa As New HumanGene With {
                .AA = html.GetText("AA seq"),
                .NT = html.GetText("NT seq"),
                .Position = html.GetText("Position"),
                .Entry = html.GetText(NameOf(HumanGene.Entry)).Split.First,
                .OtherDBs = html("Other DBs").FirstOrDefault.__otherDBs,
                .Pathway = PathwayWebParser.__parseHTML_ModuleList(html("Pathway").FirstOrDefault, LIST_TYPES.Pathway),
                .GeneName = html.GetText("Gene name"),
                .Disease = __parseHTML_ModuleList(html.GetValue("Disease").FirstOrDefault, LIST_TYPES.Disease),
                .DrugTarget = html("Drug target").FirstOrDefault.__drugTarget,
                .Modules = __parseHTML_ModuleList(html.GetValue("Module").FirstOrDefault, LIST_TYPES.Module)
            }

            With hsa
                .AA = .AA.Trim.lTokens.Skip(1).JoinBy(ASCII.LF)
                .NT = .NT.Replace(InternalWebFormParsers.DBGET, "") _
                    .StripBlank _
                    .lTokens _
                    .Skip(1) _
                    .JoinBy(ASCII.LF)
            End With

            Return hsa
        End Function

        <Extension> Private Function __drugTarget(html$) As KeyValuePair()
            Dim out As New List(Of KeyValuePair)
            Dim divs = html.DivInternals.SlideWindows(2, offset:=2)

            If Trim(html).StringEmpty Then
                Return Nothing
            End If

            For Each pair In divs
                out += New KeyValuePair With {
                    .Key = pair(0) _
                        .StripHTMLTags(stripBlank:=True) _
                        .Trim(":"c),
                    .Value = pair(1) _
                        .StripHTMLTags(stripBlank:=True)
                }
            Next

            Return out
        End Function

        ''' <summary>
        ''' 从KEGG数据库之中下载人类基因组的注释数据
        ''' </summary>
        ''' <param name="EXPORT$"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function DownloadHumanGenome(geneIDs As IEnumerable(Of String), EXPORT$) As String()
            Dim list$() = geneIDs.ToArray
            Dim failures As New List(Of String)

            Using progress As New ProgressBar("Download genes of human genome...",, True)
                Dim tick As New ProgressProvider(list.Length)
                Dim path As New Value(Of String)
                Dim ETA$

                For Each id As String In list
                    If Not (path = $"{EXPORT}/{id}.xml").FileExists(True) Then
                        Try
                            Call DownloadHSA(id).SaveAsXml(path,,)
                            Call Thread.Sleep(1500)
                        Catch ex As Exception
                            failures += id
                        End Try
                    End If

                    ETA = $"ETA={tick.ETA(progress.ElapsedMilliseconds)}"
                    progress.SetProgress(tick.StepProgress, detail:=ETA)
                Next
            End Using

            Return failures
        End Function
    End Module
End Namespace