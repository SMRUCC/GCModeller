Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.Uniprot.Web

    <PackageNamespace("Uniprot.WebServices")>
    Public Module WebServices

        Const UNIPROT_QUERY As String = "http://www.uniprot.org/uniprot/?query=name%3A{0}+AND+taxonomy%3A{1}&sort=score"

        ''' <summary>
        ''' Create a protein query url. 
        ''' </summary>
        ''' <param name="geneId"></param>
        ''' <param name="taxonomy"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Query.Create", Info:="Create a protein query url. ")>
        Public Function CreateQuery(<Parameter("Gene.ID")> geneId As String, taxonomy As String) As String
            Return String.Format(UNIPROT_QUERY, geneId, taxonomy)
        End Function

        Const UNIPROT_FASTA_DOWNLOAD_URL As String = "http://www.uniprot.org/uniprot/{0}.fasta"

        ''' <summary>
        ''' Download a protein sequence fasta data from http://www.uniprot.org/ using a specific <paramref name="UniprotId"></paramref>. （从http://www.uniprot.org/网站之上下载一条蛋白质序列）
        ''' </summary>
        ''' <param name="UniprotId">The uniprot id of a protein sequence.(蛋白质在Uniprot数据库之中的编号)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Protein.Download", Info:="Download a protein sequence fasta data from http://www.uniprot.org/ using a specific UniprotId.")>
        Public Function DownloadProtein(UniprotId As String) As FASTA.FastaToken
            Dim url As String = String.Format(UNIPROT_FASTA_DOWNLOAD_URL, UniprotId)
            Dim html As String = url.GET
            Return FASTA.FastaToken.TryParse(html)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="url">CreateQuery(geneId, taxonomy)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("ListEntries")>
        Public Function GetEntries(url As String) As Entry
            Dim pageContent As String = url.GET
            Return Nothing
        End Function
    End Module
End Namespace