Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF

''' <summary>
''' 基因详细信息，增加位置信息以支持共线性分析
''' </summary>
Public Class GeneInfo

    Public Property GeneID As String
    Public Property GenomeName As String
    Public Property Chromosome As String
    Public Property Start As Integer
    Public Property [End] As Integer

    ' 用于排序和距离计算
    Public ReadOnly Property Length As Integer
        Get
            Return [End] - Start + 1
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return GeneID
    End Function

    Public Shared Iterator Function CreateGeneModel(genome As GFFTable) As IEnumerable(Of GeneInfo)
        For Each gene As Feature In genome.features
            Yield New GeneInfo With {
                .Chromosome = gene.seqname,
                .GeneID = gene.ID,
                .GenomeName = If(genome.species, gene.source),
                .Start = gene.left,
                .[End] = gene.right
            }
        Next
    End Function

End Class