Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Module Extensions

    ''' <summary>
    ''' Is this collection of the besthit data is empty or nothing?
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    <Extension>
    Public Function IsNullOrEmpty(data As IEnumerable(Of BestHit)) As Boolean
        If data Is Nothing Then
            Return True
        Else
            Dim notNull As BestHit = (From bh As BestHit
                                      In data
                                      Where Not bh.isMatched
                                      Select bh).FirstOrDefault
            Return notNull Is Nothing
        End If
    End Function
End Module
