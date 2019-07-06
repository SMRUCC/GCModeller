Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace Pipeline

    Public Module KOAssignment

        <Extension>
        Public Function KOAssignmentSBH(result As IEnumerable(Of BestHit)) As BestHit()


        End Function

        ''' <summary>
        ''' KO number assignment in bbh method
        ''' </summary>
        ''' <param name="drf">Besthits in direction forward</param>
        ''' <param name="drr">Besthits in direction reverse</param>
        ''' <returns></returns>
        Public Function KOassignmentBBH(drf As BestHit(), drr As BestHit()) As BiDirectionalBesthit()

        End Function
    End Module
End Namespace