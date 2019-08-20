Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 测序数据的模拟生成模块
''' </summary>
Public Module Simulator

    ''' <summary>
    ''' 模拟基因组测序, 基因组序列上均匀的随机断裂
    ''' </summary>
    ''' <param name="nt"></param>
    ''' <param name="fragmentSize">碎片的平均大小</param>
    ''' <param name="totalFragments">产生的碎片总数量</param>
    ''' <returns></returns>
    Public Iterator Function MakeFragments(nt As FastaSeq, Optional fragmentSize% = 200, Optional totalFragments% = 50000) As IEnumerable(Of FastaSeq)

    End Function

    ''' <summary>
    ''' 模拟mRNA测序结果, 用来测试基因表达量的估算程序
    ''' </summary>
    ''' <param name="nt"></param>
    ''' <param name="context"></param>
    ''' <returns></returns>
    Public Iterator Function MakeReads(nt As FastaSeq, context As GFFTable) As IEnumerable(Of FastaSeq)

    End Function
End Module
