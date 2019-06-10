Imports System.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Public Class CacheGenerator

    Public ReadOnly Property KO_list As String
    Public ReadOnly Property taxonomy As String
    Public ReadOnly Property counts As String

    Const KEGG_list$ = "KO.list"
    Const Taxonomy_data$ = "taxonomy.txt"
    Const gene_counts$ = "gene.counts"
    Const blank$ = "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="cache">
    ''' The cache data directory
    ''' </param>
    Sub New(cache As String)
        KO_list = $"{cache}/{KEGG_list}"
        taxonomy = $"{cache}/{Taxonomy_data}"
        counts = $"{cache}/{gene_counts}"
    End Sub

    Public Sub CopyTo(destination As String)
        With destination & "/"
            Call KO_list.FileCopy(.ByRef)
            Call taxonomy.FileCopy(.ByRef)
            Call counts.FileCopy(.ByRef)
        End With
    End Sub

    ''' <summary>
    ''' 函数返回来的是临时文件夹的路径位置
    ''' </summary>
    ''' <param name="UniProtXml"></param>
    ''' <returns></returns>
    Public Function ScanInternal(UniProtXml As IEnumerable(Of entry)) As CacheGenerator
        Using KO As StreamWriter = Me.KO_list.OpenWriter,
            taxon As StreamWriter = Me.taxonomy.OpenWriter,
            counts As StreamWriter = Me.counts.OpenWriter

            Call doScan(UniProtXml, KO, taxon, counts)

            Call KO.Flush()
            Call counts.Flush()
            Call taxon.Flush()
        End Using

        Return Me
    End Function

    Private Shared Sub doScan(UniProtXml As IEnumerable(Of entry), KO As StreamWriter, taxon As StreamWriter, counts As StreamWriter)
        ' KO 缓存文件示例
        ' 
        ' taxonomy_id KO
        ' taxonomy_id KO
        ' taxonomy_id KO
        '
        ' 
        Dim taxonomyIndex As New Dictionary(Of String, String)

        For Each protein As entry In UniProtXml _
            .Where(Function(org)
                       Return Not org.organism Is Nothing AndAlso
                              Not org.organism.dbReference Is Nothing
                   End Function)

            Dim taxonomy As organism = protein.organism
            Dim taxonID$ = taxonomy.dbReference.id

            ' 为了计算出覆盖度，需要知道基因组之中有多少个基因的记录
            ' 在这里直接记录下物种的编号，在后面分组计数就可以了解基因组
            ' 之中的基因的总数了
            Call counts.WriteLine(taxonID)

            ' 如果已经存在index了，则不会写入taxo文件之中
            If Not taxonomyIndex.ContainsKey(taxonID) Then
                Call taxon.WriteLine(taxonomy.GetXml)
                Call taxon.WriteLine(blank)
                Call taxonomyIndex.Add(taxonID, "null")
            End If

            Dim KOlist$() = protein.xrefs _
                .TryGetValue("KO") _
                .SafeQuery _
                .Select(Function(KEGG) KEGG.id) _
                .ToArray

            If KOlist.Length > 0 Then
                Call KO.WriteLine(
                    value:=KOlist _
                        .Select(Function(id)
                                    Return taxonID & vbTab & id
                                End Function) _
                        .JoinBy(KO.NewLine)
                )
            End If
        Next
    End Sub
End Class
