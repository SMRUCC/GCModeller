Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace StringDB.Tsv

    ''' <summary>
    ''' interaction types for protein links
    ''' </summary>
    Public Class Actions

        Public Property item_id_a As String
        Public Property item_id_b As String
        Public Property mode As String
        Public Property action As String
        Public Property a_is_acting As String
        Public Property score As String

        Public Shared Iterator Function LoadText(path As String) As IEnumerable(Of Actions)
            For Each line As String In path.IterateAllLines.Skip(1)
                Dim tokens As String() = line.Split(Text.ASCII.TAB)
                Yield New Actions With {
                    .item_id_a = tokens(0),
                    .item_id_b = tokens(1),
                    .mode = tokens(2),
                    .action = tokens(3),
                    .a_is_acting = tokens(4),
                    .score = tokens(5)
                }
            Next
        End Function
    End Class

    ''' <summary>
    ''' protein network data (incl. subscores per channel); commercial entities require a license.	
    ''' </summary>
    Public Class linksDetail

        Public Property protein1 As String
        Public Property protein2 As String
        Public Property neighborhood As String
        Public Property fusion As String
        Public Property cooccurence As String
        Public Property coexpression As String
        Public Property experimental As String
        Public Property database As String
        Public Property textmining As String
        Public Property combined_score As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="maps"></param>
        ''' <returns></returns>
        Public Iterator Function Selects(source As IEnumerable(Of EntityObject), maps As Dictionary(Of String, String), links As IEnumerable(Of linksDetail)) As IEnumerable(Of EntityObject)

        End Function
    End Class

    ''' <summary>
    ''' separate identifier mapping files, for several frequently used name_spaces...
    ''' </summary>
    Public Class entrez_gene_id_vs_string

        <Column("#Entrez_Gene_ID")> Public Property Entrez_Gene_ID As String
        Public Property STRING_Locus_ID As String

        Public Overrides Function ToString() As String
            Return $"{Entrez_Gene_ID} <--> {STRING_Locus_ID}"
        End Function

        Public Shared Function BuildMapsFromFile(path As String, Optional tsv As Boolean = True) As Dictionary(Of String, String)
            Return BuildMaps(path.Imports(Of entrez_gene_id_vs_string)(tsv))
        End Function

        Public Shared Function BuildMaps(source As IEnumerable(Of entrez_gene_id_vs_string)) As Dictionary(Of String, String)
            Return source.ToDictionary(Function(x) x.Entrez_Gene_ID, Function(x) x.STRING_Locus_ID)
        End Function
    End Class
End Namespace
