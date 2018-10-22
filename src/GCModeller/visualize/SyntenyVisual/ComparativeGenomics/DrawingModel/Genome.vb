Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.GFF

Namespace ComparativeGenomics

    Public Class GenomeModel : Implements IEnumerable(Of GeneObject)

        Public Property genes As GeneObject()

        ''' <summary>
        ''' 基因组的nt长度
        ''' </summary>
        ''' <returns></returns>
        Public Property Length As Integer
        ''' <summary>
        ''' 基因组的名称
        ''' </summary>
        ''' <returns></returns>
        Public Property Title As String

        ''' <summary>
        ''' 假若是一个完整的基因组，则这个属性为0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SegmentOffset As Integer

        ''' <summary>
        ''' 在这个基因组之中的基因的数量计数
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Count As Integer
            Get
                Return genes.Length
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Title
        End Function

        Public Function Height(h As Integer) As GenomeModel
            For i As Integer = 0 To genes.Length - 1
                genes(i).Height = h
            Next

            Return Me
        End Function

        Public Function Reverse() As GenomeModel
            Dim genes = Me.genes _
                .Select(Function(g) g.ReverseCopy(genomeSize:=Length)) _
                .ToArray

            Return New GenomeModel With {
                .genes = genes,
                .Length = Length,
                .SegmentOffset = SegmentOffset,
                .Title = Title
            }
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of GeneObject) Implements IEnumerable(Of GeneObject).GetEnumerator
            For Each gene As GeneObject In genes
                Yield gene
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FromGffTable(genome As GFFTable) As GenomeModel
            Return genome.Features _
                .AsGenes _
                .Where(Function(g)
                           ' 因为在gff之中还包含有整个基因组序列的feature
                           ' 要把这些feature删除掉
                           Return Not g.Gene.StringEmpty
                       End Function) _
                .GroupBy(Function(g) g.Gene) _
                .Select(Function(g)
                            ' 20181020 因为gff文件之中可能会出现gene和蛋白的注释feature
                            ' 因为基因和蛋白之间的id可能会发生重复
                            ' 所以需要在这里进行一次去重
                            Return g.First
                        End Function) _
                .ToArray _
                .CreateSyntenyGenome(genome.Size, genome.SeqRegion.AccessId, Function(g) g.Gene)
        End Function
    End Class
End Namespace