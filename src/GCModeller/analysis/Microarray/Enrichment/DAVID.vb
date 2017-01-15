Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Data.csv
Imports System.Runtime.CompilerServices

Namespace DAVID

    Public Module DAVID

        Public Function Load(path$) As FunctionCluster()
            Dim lines As IEnumerable(Of String()) = path _
                .ReadAllLines _
                .Split("Annotation Cluster \d+.+", True) _
                .Where(Function(line) line.Length > 0) _
                .ToArray
            Dim tsv As New List(Of String)

            tsv += lines(0)(Scan0)

            For Each line In lines
                tsv += line.Skip(1)
            Next

            Dim out = tsv.ImportsTsv(Of FunctionCluster)
            Return out
        End Function

        <Extension>
        Public Function SelectGoTerms(data As IEnumerable(Of FunctionCluster)) As FunctionCluster()
            Return LinqAPI.Exec(Of FunctionCluster) <= From x As FunctionCluster
                                                       In data
                                                       Where x.Category.StartsWith("GOTERM_")
                                                       Select x
                                                       Order By x.Category Ascending
        End Function

        <Extension>
        Public Function SelectKEGGPathway(data As IEnumerable(Of FunctionCluster)) As FunctionCluster()
            Return LinqAPI.Exec(Of FunctionCluster) <= From x As FunctionCluster
                                                       In data
                                                       Where x.Category = "KEGG_PATHWAY"
                                                       Select x
                                                       Order By x.Category Ascending
        End Function
    End Module

    Public Class FunctionCluster

        Public Property Category As String
        Public Property Term As String
        Public Property Count As Integer
        <Column("%")> Public Property Percent As Double
        Public Property PValue As Double
        <Ignored> Public Property Genes As String()
        <Column("List Total")> Public Property ListTotal As Integer
        <Column("Pop Hits")> Public Property PopHits As Integer
        <Column("Pop Total")> Public Property PopTotal As Integer
        <Column("Fold Enrichment")> Public Property FoldEnrichment As Double
        Public Property Bonferroni As Double
        Public Property Benjamini As Double
        Public Property FDR As Double

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace