Imports System.Runtime.CompilerServices

Namespace LocalBLAST.BLASTOutput.BlastPlus.BlastX

    ''' <summary>
    ''' Extensions API for blastX output table
    ''' </summary>
    Public Module Extensions

        ''' <summary>
        ''' Get the top best hit from the blastx output table result
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        <Extension> Public Function TopBest(result As IEnumerable(Of BlastXHit)) As BlastXHit()

        End Function
    End Module
End Namespace