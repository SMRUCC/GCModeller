Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.COG.COGs
Imports SMRUCC.genomics.Assembly.NCBI.SequenceDump
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace LocalBLAST.Application.RpsBLAST

    Public Module COG2014

        <Extension>
        Public Function COG2014_result(sbh As IEnumerable(Of BestHit), cog2014 As COGTable()) As MyvaCOG()
            Return sbh.COG2014_result(COGTable.GI2COGs(cog2014))
        End Function

        <Extension>
        Public Function COG2014_result(sbh As IEnumerable(Of BestHit), gi2cogs As Dictionary(Of String, String())) As MyvaCOG()
            Dim query = sbh.GroupBy(Function(prot) prot.QueryName)
            Dim out As New List(Of MyvaCOG)

            For Each protein As IGrouping(Of String, BestHit) In query
                ' 取最好的
                Dim best As BestHit = protein.TopHit
                Dim header As NTheader = NTheader.ParseNTheader(best.HitName.Split("|"c)).FirstOrDefault
                Dim gi$ = header.gi

                out += New MyvaCOG With {
                    .QueryName = protein.Key,
                    .QueryLength = best.query_length,
                    .LengthQuery = best.length_query,
                    .Identities = best.identities,
                    .Description = best.HitName,
                    .Evalue = best.evalue,
                    .Length = best.hit_length
                }

                If Not gi Is Nothing Then
                    Dim COG$() = gi2cogs(gi)
                    out.Last.COG = COG.JoinBy("; ")
                End If
            Next

            Return out
        End Function
    End Module
End Namespace