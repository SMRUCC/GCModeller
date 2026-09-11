
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize.Circos
Imports SMRUCC.genomics.Visualize.Circos.Karyotype
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("circos")>
Module circosTool

    <ExportAPI("default_identity_colors")>
    Public Function IdentityColors([default] As String) As IdentityColors
        Return New IdentityLevels([default])
    End Function

    <ExportAPI("identity_colors")>
    Public Function IdentityColors(min#, max#, Optional depth% = 10, Optional default$ = "Brown", Optional mapName$ = "Jet") As IdentityColors
        Return New IdentityGradients(min, max, depth, [default], mapName)
    End Function

    <ExportAPI("blastn_mapping")>
    <RApiReturn(GetType(KaryotypeChromosomes))>
    Public Function FromBlastnMappings(source As Object, chrs As Object, Optional env As Environment = Nothing) As Object
        Dim blastn As PipeIterator(Of BlastnMapping) = pipeline.Stream(Of BlastnMapping)(source, env)
        Dim chrNts As PipeIterator(Of FastaSeq) = pipeline.Stream(Of FastaSeq)(chrs, env)

        If blastn.isError Then
            Return blastn.getError
        ElseIf chrNts.isError Then
            Return chrNts.getError
        End If

        Return DataExtensions.FromBlastnMappings(blastn, chrNts)
    End Function
End Module
