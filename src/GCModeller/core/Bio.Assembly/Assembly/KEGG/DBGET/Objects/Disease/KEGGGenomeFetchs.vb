Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' 通过这个模块从KEGG服务器上下载人类基因组数据
    ''' </summary>
    Public Module KEGGGenomeFetchs

        ''' <summary>
        ''' 使用基因编号来下载基因注释数据
        ''' </summary>
        ''' <param name="id$">人类基因的编号</param>
        ''' <returns></returns>
        Public Function DownloadHSA(id$) As HumanGene
            Throw New NotImplementedException
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
                .Disease = __parseHTML_ModuleList(html.GetValue("Disease").FirstOrDefault, LIST_TYPES.Disease)
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

        ''' <summary>
        ''' 从KEGG数据库之中下载人类基因组的注释数据
        ''' </summary>
        ''' <param name="EXPORT$"></param>
        ''' <returns></returns>
        Public Function DownloadHumanGenome(EXPORT$) As String()
            Throw New NotImplementedException
        End Function

    End Module
End Namespace