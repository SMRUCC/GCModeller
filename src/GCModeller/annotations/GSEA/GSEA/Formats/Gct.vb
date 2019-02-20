Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports tsv = Microsoft.VisualBasic.Data.csv.IO.File

''' <summary>
''' #### GCT: Gene Cluster Text file format (``*.gct``)
''' 
''' The GCT format is a tab delimited file format that describes an expression dataset.
''' </summary>
Public Class Gct : Implements IEnumerable(Of GeneExpression)

    Public Property version As String

    Public ReadOnly Property numberOfgenes As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return genes.Length
        End Get
    End Property

    Public ReadOnly Property numberOfsamples As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return genes(Scan0).EnumerateKeys.Length
        End Get
    End Property

    Public Property genes As GeneExpression()

    Public Class GeneExpression : Inherits DataSet

        Public Property Description As String

        Public Overrides Function ToString() As String
            Return MyBase.ToString()
        End Function
    End Class

    Public Shared Function LoadFile(path As String) As Gct
        Dim table = tsv.LoadTsv(path)
        Dim version = table.First()(Scan0).Trim("#"c)
        Dim size = table(1)
        Dim geneNumber As Integer = size(0)
        Dim sampleNumber As Integer = size(1)
        Dim sampleNames = table(2).Skip(2).ToArray
        Dim getGenes As GeneExpression() =
            Iterator Function() As IEnumerable(Of GeneExpression)
                For Each row As RowObject In table.Skip(3)
                    Yield New GeneExpression With {
                        .ID = row(Scan0),
                        .Description = row(1),
                        .Properties = row _
                            .Skip(2) _
                            .SeqIterator _
                            .ToDictionary(Function(i) sampleNames(i), Function(x) Val(x.value))
                    }
                Next
            End Function().ToArray

        Return New Gct With {
            .version = version,
            .genes = getGenes
        }
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator(Of GeneExpression) Implements IEnumerable(Of GeneExpression).GetEnumerator
        For Each gene As GeneExpression In genes
            Yield gene
        Next
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function
End Class