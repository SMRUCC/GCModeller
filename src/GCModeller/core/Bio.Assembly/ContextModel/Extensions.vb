Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ContextModel

    Public Module Extensions

        ''' <summary>
        ''' 使用一个范围选出所有落在该范围内的基因
        ''' </summary>
        ''' <param name="genome"></param>
        ''' <param name="region"></param>
        ''' <returns></returns>
        <Extension>
        Public Function RangeSelection(genome As PTT, region As Location, Optional offset As Boolean = False) As PTT
            Dim genes As GeneBrief() = genome _
                .GeneObjects _
                .Where(Function(gene) region.IsOverlapping(gene.Location)) _
                .ToArray
            Dim title$ = genome.Title & " - region of " & region.ToString
            Dim data As GeneBrief() = If(offset, genes.Offsets(region.Left), genes)
            Dim subset As New PTT(data, title, region.Length)
            Return subset
        End Function

        <Extension>
        Public Function Offsets(genes As IEnumerable(Of GeneBrief), left%) As GeneBrief()
            Dim out As GeneBrief() = genes.Select(Function(gene) gene.Clone).ToArray
            Call out.ForEach(Sub(gene, i)
                                 With gene
                                     Dim l = .Location.Left
                                     Dim r = .Location.Right

                                     .Location = New NucleotideLocation(
                                         l - left, 
                                         r - left, 
                                         .Location.Strand)
                                 End With
                             End Sub)
            Return out
        End Function
    End Module
End Namespace