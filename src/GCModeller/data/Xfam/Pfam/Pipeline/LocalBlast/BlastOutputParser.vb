Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus

Namespace Pipeline.LocalBlast

    Public Module BlastOutputParser

        ''' <summary>
        ''' PfamA as query, alignment with protein sequence as subjects
        ''' </summary>
        ''' <param name="query"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function ParseDomainQuery(query As Query) As IEnumerable(Of PfamHit)

        End Function

        ''' <summary>
        ''' The protein sequence as query input, alignment with pfamA domain sequence as subjects.
        ''' </summary>
        ''' <param name="query"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function ParseProteinQuery(query As Query) As IEnumerable(Of PfamHit)

        End Function
    End Module
End Namespace