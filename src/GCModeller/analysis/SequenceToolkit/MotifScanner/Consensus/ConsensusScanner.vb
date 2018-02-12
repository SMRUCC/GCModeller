
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract
Imports SMRUCC.genomics.ContextModel.Promoter
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 扫描出相同的KO编号的基因的上游区域的片段的相似片段
''' </summary>
Public Class ConsensusScanner

    Dim KOUpstream As Dictionary(Of String, FastaSeq())
    Dim KO As Dictionary(Of String, String())

    Sub New(genomes As IEnumerable(Of genomic), Optional length As PrefixLength = PrefixLength.L300)
        With genomes.ToArray
            Dim upstreams = .Select(Function(g) g.GetUpstreams(length)) _
                            .ToArray

            KO = .Select(Function(genome)
                             Return genome.organism _
                                 .genome _
                                 .Select(Function(pathway) pathway.genes) _
                                 .IteratesALL _
                                 .Where(Function(g) Not g.text.StringEmpty)
                         End Function) _
                 .IteratesALL _
                 .GroupBy(Function(gene) gene.text.Split.First) _
                 .ToDictionary(Function(id) id.Key,
                               Function(genes)
                                   ' KEGG之中的基因编号都会存在一个物种缩写的前缀
                                   ' 在这里移除掉
                                   Return genes.Keys _
                                       .Select(Function(id) id.Split(":"c).Last) _
                                       .ToArray
                               End Function)
            KOUpstream = KO.ToDictionary(Function(id) id.Key,
                                         Function(consensus)
                                             Dim geneIDs$() = consensus.Value
                                             Dim upstream = geneIDs _
                                                 .Select(Function(ID)
                                                             Return upstreams _
                                                                 .Where(Function(genome) genome.ContainsKey(ID)) _
                                                                 .First()(ID)
                                                         End Function) _
                                                 .ToArray
                                             Return upstream
                                         End Function)
        End With
    End Sub

    ''' <summary>
    ''' Export motifs for all of the KO category
    ''' </summary>
    ''' <returns></returns>
    Public Iterator Function PopulateMotifs() As IEnumerable(Of Probability)
        For Each KO As String In Me.KO.Keys
            For Each motif As Probability In PopulateMotifs(KO)
                Yield motif
            Next
        Next
    End Function

    ''' <summary>
    ''' Export motifs for a specific KO category
    ''' </summary>
    ''' <param name="KO">KEGG orthology ID</param>
    ''' <returns></returns>
    Public Iterator Function PopulateMotifs(KO As String) As IEnumerable(Of Probability)
        Dim upstreams = KOUpstream(KO)
        ' 先进行两两比对，找出最相似的片段
        Dim pairwise As New List(Of Map(Of (query$, subject$), HSP))

        For Each fa As FastaSeq In upstreams
            For Each fb As FastaSeq In upstreams.Where(Function(s) Not s Is fa)

            Next
        Next
    End Function
End Class
