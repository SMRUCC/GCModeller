Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline

Public Module Annotations

    Public Function LoadSBHMaps(filepath As String) As IEnumerable(Of BestHit)
        Return filepath.LoadCsv(Of BestHit)(skipWhile:=SkipHitNotFound)
    End Function

    Public Function PfamAssign()

    End Function
End Module
