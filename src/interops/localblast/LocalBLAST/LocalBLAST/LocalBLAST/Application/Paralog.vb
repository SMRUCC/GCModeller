Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus

Namespace LocalBLAST.Application

    Public Module Paralog

        ''' <summary>
        ''' Exports the blastp paralog hits.
        ''' </summary>
        ''' <param name="blastp"></param>
        ''' <param name="coverage"></param>
        ''' <param name="identities"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ExportParalog(blastp As v228, Optional coverage As Double = 0.5, Optional identities As Double = 0.3) As BestHit()
            Dim source As IEnumerable(Of BestHit) = blastp.ExportAllBestHist(coverage, identities)
            Dim LQuery = (From x As BestHit
                          In source.AsParallel
                          Where Not String.Equals(x.QueryName, x.HitName)
                          Select x).ToArray
            Return LQuery
        End Function
    End Module
End Namespace