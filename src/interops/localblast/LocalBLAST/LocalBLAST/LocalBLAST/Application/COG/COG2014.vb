Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.NCBI.COG.COGs
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace LocalBLAST.Application.RpsBLAST

    Public Module COG2014

        <Extension>
        Public Function COG2014_result(sbh As IEnumerable(Of BestHit), cog2014 As COGTable()) As MyvaCOG()

        End Function
    End Module
End Namespace