Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Karyotype

    <HideModuleName>
    Public Module KaryotypeExtensions

        ''' <summary>
        ''' 缺口的大小，这个仅仅在单个染色体的基因组绘图模型之中有效
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        <Extension>
        Public Function LoopHole(x As SkeletonInfo) As PropertyValue(Of Integer)
            Return PropertyValue(Of Integer).Read(Of SkeletonInfo)(x, NameOf(LoopHole))
        End Function

        ''' <summary>
        ''' nt核苷酸基因组序列拓展属性
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        <Extension>
        Public Function nt(x As Karyotype) As PropertyValue(Of FastaSeq)
            Return PropertyValue(Of FastaSeq).Read(Of Karyotype)(x, NameOf(nt))
        End Function

        <Extension>
        Public Function MapsRaw(x As Band) As PropertyValue(Of BlastnMapping)
            Return PropertyValue(Of BlastnMapping).Read(Of Band)(x, NameOf(MapsRaw))
        End Function
    End Module
End Namespace