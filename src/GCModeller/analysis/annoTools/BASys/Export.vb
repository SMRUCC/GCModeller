Imports System.Runtime.CompilerServices
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

    End Function
End Module
