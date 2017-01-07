Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.DocumentStream.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Class KEGGOrthology

    Public ReadOnly Property Repository As String

    Sub New(DIR$)
        Repository = DIR
        __locus2KO()
    End Sub

    Dim locus2KO As New Dictionary(Of String, String())

    Private Function __ko_genes_index() As String
        Return (Repository & "/ko00001_genes_index.csv")
    End Function

    Private Sub __locus2KO()
        Dim package As IEnumerable(Of KO_gene) = __ko_genes_index().LoadCsv(Of KO_gene)
        Dim locus_tags = package.GroupBy(Function(kg) kg.sp_code & ":" & kg.gene)

        For Each gene As IGrouping(Of String, KO_gene) In locus_tags
            Dim k$() = gene _
                .Select(Function(g) g.ko) _
                .Distinct _
                .ToArray

            Call locus2KO.Add(gene.Key, k)
        Next
    End Sub

    ''' <summary>
    ''' 重新构建基因编号的索引
    ''' </summary>
    Public Sub BuildLocusIndex()
        Dim source As IEnumerable(Of KO_gene()) = BuildLocus2KOIndex(Repository & "/ko00001/")

        Using writer As New WriteStream(Of KO_gene)(__ko_genes_index)
            For Each block As KO_gene() In source
                Call writer.Flush(block)
            Next
        End Using
    End Sub

    Public Shared Iterator Function BuildLocus2KOIndex(DIR$) As IEnumerable(Of KO_gene())
        For Each xml As String In ls - l - r - "*.xml" <= DIR
            Dim ko As Orthology = xml.LoadXml(Of Orthology)

            If ko.Genes.IsNullOrEmpty Then
                Continue For
            End If

            Dim genes As KO_gene() = LinqAPI.Exec(Of KO_gene) <=
 _
                From gene As QueryEntry
                In ko.Genes
                Let desc = If(gene.Description Is Nothing, "", gene.Description)
                Let og = New KO_gene With {
                    .gene = gene.LocusId,
                    .ko = ko.Entry,
                    .name = desc,
                    .sp_code = gene.SpeciesId,
                    .url = ""
                }
                Select og

            Yield genes
        Next
    End Function

    Public Iterator Function EnumerateKO(locus_tag$) As IEnumerable(Of Orthology)
        Dim KOlist$() = If(
            locus2KO.ContainsKey(locus_tag), locus2KO(locus_tag), New String() {})

        For Each KO$ In KOlist
            Dim xml$ = $"{Repository}/ko00001/{KO}.xml"
            Dim o As Orthology = xml.LoadXml(Of Orthology)

            Yield o
        Next
    End Function
End Class
