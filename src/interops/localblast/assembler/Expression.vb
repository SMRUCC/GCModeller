Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping

''' <summary>
''' 通过blastn结果进行mRNA测序结果的表达量估算
''' </summary>
Public Module Expression

    <Extension>
    Public Function Measure(reads As IEnumerable(Of BlastnMapping), context As GFFTable) As Dictionary(Of String, Integer)

    End Function
End Module
