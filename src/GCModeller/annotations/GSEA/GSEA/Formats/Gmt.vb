Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports tsv = Microsoft.VisualBasic.Data.csv.IO.File

''' <summary>
''' #### GMT: Gene Matrix Transposed file format (``*.gmt``)
''' 
''' The GMT file format is a tab delimited file format that 
''' describes gene sets. In the GMT format, each row represents 
''' a gene set; in the GMX format, each column represents a 
''' gene set. 
''' 
''' Each gene set is described by a name, a description, and the 
''' genes in the gene set. GSEA uses the description field to 
''' determine what hyperlink to provide in the report for the 
''' gene set description: if the description is ``na``, GSEA provides 
''' a link to the named gene set in MSigDB; if the description is 
''' a URL, GSEA provides a link to that URL.
''' </summary>
Public Class Gmt

    Public ReadOnly Property species As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return attributes.TryGetValue({NameOf(species), "Species"})
        End Get
    End Property

    Public ReadOnly Property database As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return attributes.TryGetValue({NameOf(database), "Database"})
        End Get
    End Property

    Public Property attributes As Dictionary(Of String, String)
    Public Property clusters As Cluster()

    Public Shared Function LoadFile(path As String) As Gmt
        Dim table = tsv.LoadTsv(path)
        Dim attrs As Dictionary(Of String, String) = table _
            .Comments _
            .Select(Function(s) s.GetTagValue(":", trim:=True)) _
            .ToDictionary(Function(t) t.Name.Trim("#"c, " "c),
                          Function(t) t.Value)
        Dim parseClusters As Cluster() =
            Iterator Function() As IEnumerable(Of Cluster)
                ' 假设注释只出现在前面
                For Each row As RowObject In table.Skip(attrs.Count)
                    Yield New Cluster With {
                        .ID = row(0),
                        .description = row(1),
                        .names = .ID,
                        .Members = row.Skip(2).ToArray
                    }
                Next
            End Function().ToArray

        Return New Gmt With {
            .attributes = attrs,
            .clusters = parseClusters
        }
    End Function
End Class
