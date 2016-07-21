Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST

Public Module Export

    <Extension>
    Public Function ExportPTT(proj As Project) As PTT

    End Function

    <Extension>
    Public Function ExportGFF(proj As Project) As GFF

    End Function

    <Extension>
    Public Function ExportCOG(proj As Project) As MyvaCOG()
        Return LinqAPI.Exec(Of MyvaCOG) <=
 _
            From x As TableBrief
            In proj.Briefs
            Select New MyvaCOG With {
                .COG = x.COG,
                .QueryName = x.Accession,
                .Description = x.Function,
                .MyvaCOG = x.Gene,
                .QueryLength = Math.Abs(x.End - x.Start),
                .Length = Math.Abs(x.End - x.Start)
            }
    End Function
End Module
