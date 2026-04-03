Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.ComponentModel.Annotation

''' <summary>
''' 基因详细信息，增加位置信息以支持共线性分析
''' </summary>
Public Class GeneInfo : Implements INamedValue

    Public Property GeneID As String Implements INamedValue.Key
    Public Property GenomeName As String
    Public Property Chromosome As String
    Public Property Start As Integer
    Public Property [End] As Integer

    ''' <summary>
    ''' 用于排序和距离计算
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Length As Integer
        Get
            Return [End] - Start + 1
        End Get
    End Property

    Sub New()
    End Sub

    Sub New(gene As GeneTable)
        GeneID = gene.locus_id
        GenomeName = gene.species
        Chromosome = gene.replicon_accessionID
        Start = gene.left
        [End] = gene.right
    End Sub

    Public Overrides Function ToString() As String
        Return GeneID
    End Function

    Public Shared Iterator Function CreateGeneModel(genome As GFFTable) As IEnumerable(Of GeneInfo)
        For Each gene As Feature In genome.features
            If gene.feature <> "gene" Then
                Continue For
            End If

            Yield New GeneInfo With {
                .Chromosome = gene.seqname,
                .GeneID = gene.ID,
                .GenomeName = If(genome.species, gene.source),
                .Start = gene.left,
                .[End] = gene.right
            }
        Next
    End Function

    Public Shared Function GenomeSet(genomes As IEnumerable(Of GFFTable)) As Dictionary(Of String, GeneInfo())
        Return genomes _
            .ToDictionary(Function(gn) gn.species,
                          Function(gn)
                              Return GeneInfo _
                                  .CreateGeneModel(gn) _
                                  .ToArray
                          End Function)
    End Function

    Public Shared Function CastTable(genomes As Dictionary(Of String, GeneTable())) As Dictionary(Of String, GeneInfo())
        Return genomes _
            .ToDictionary(Function(g) g.Key,
                          Function(g)
                              Return (From gene As GeneTable
                                      In g.Value
                                      Group By gene.locus_id Into Group
                                      Select New GeneInfo(Group.First)).ToArray
                          End Function)
    End Function

End Class