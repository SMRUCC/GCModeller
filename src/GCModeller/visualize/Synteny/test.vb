Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application

Module test

    Sub Main()
        Dim data = "E:\2018-7-10\高粱对比筛选结果.csv".LoadCsv(Of align)

    End Sub
End Module

Public Class align : Implements IMapping

    Public Property Qseqid As String Implements IMapping.Qname
    Public Property Sseid As String Implements IMapping.Sname
    Public Property indent As Double
    Public Property mismatch As Double
    Public Property qlength As Double
    Public Property length As Double
    Public Property Si As Double
    Public Property qstart As Integer Implements IMapping.Qstart
    Public Property qend As Integer Implements IMapping.Qstop
    Public Property sstart As Integer Implements IMapping.Sstart
    Public Property send As Integer Implements IMapping.Sstop
    Public Property evlaue As Double

    <Column("bit-score")>
    Public Property bit_score As Double

End Class