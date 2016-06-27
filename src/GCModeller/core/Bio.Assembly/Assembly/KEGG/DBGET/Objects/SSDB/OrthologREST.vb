Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.KEGG.WebServices
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.Language

Namespace Assembly.KEGG.DBGET.bGetObject.SSDB

    ''' <summary>
    ''' 蛋白质直系同源比对blastp结果
    ''' </summary>
    ''' <remarks>
    ''' 在直系同源的数据被下载下来之后，这个对象会被直接保存为Xml文档
    ''' </remarks>
    <XmlType("Ortholog")>
    Public Class OrthologREST : Inherits ClassObject

        Public Const URL As String = "http://www.kegg.jp/ssdb-bin/ssdb_best?org_gene={0}:{1}"

        <XmlAttribute> Public Property KEGG_ID As String
        <XmlAttribute> Public Property Definition As String
        <XmlElement> Public Property Sequence As String
        ''' <summary>
        ''' 直系同源的蛋白质比对结果
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property Orthologs As SShit()

        Public Const SEPRATOR As String = "------------------------------------------------------------------ -------------------------------------------------------------"

        ' <INPUT TYPE="checkbox" NAME="ckid" VALUE="xca:xccb100_1230" CHECKED><A HREF="http://www.kegg.jp/dbget-bin/www_bget?xca:xccb100_1230" TARGET="_blank">xca:xccb100_1230</A> type IV pilus response regulator PilH  <A HREF="http://www.kegg.jp/dbget-bin/www_bget?ko:K02658"  TARGET="_blank">K02658</a>     120      771 (  378)     182    1.000    120     &lt;-&gt; <a href='/ssdb-bin/ssdb_ortholog_view?org_gene=xcb:XC_1184&org=xca&threshold=&type=' target=_ortholog>66</a>
        Public Const REGEX_ORTHO_ITEM As String = "<INPUT TYPE=""checkbox"" .+? target=_ortholog>\d+</a>"

        Public Overrides Function ToString() As String
            Return Definition
        End Function

        Public Shared Function Download(locusTag As QueryEntry) As OrthologREST
            Dim html As String = String.Format(URL, locusTag.SpeciesId, locusTag.LocusId).GET
            Dim tokens = Strings.Split(html, SEPRATOR)
            Dim hits As String() = Regex.Matches(tokens.Last, REGEX_ORTHO_ITEM, RegexICSng).ToArray

            Dim Ortholog As New OrthologREST With {
                .Orthologs = LinqAPI.Exec(Of String, SShit)(hits) <= Function(s) SShit.CreateObject(s),
                .KEGG_ID = locusTag.ToString
            }

            Dim Fa As FASTA.FastaToken = WebRequest.FetchSeq(locusTag)

            If Not Fa Is Nothing Then
                Ortholog.Definition = Fa.Title
                Ortholog.Sequence = Fa.SequenceData
            End If

            Return Ortholog
        End Function
    End Class
End Namespace