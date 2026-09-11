
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.Circos
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.genomics.Visualize.Circos.Karyotype
Imports SMRUCC.genomics.Visualize.Circos.Karyotype.GeneObjects
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("circos")>
<RTypeExport("circos", GetType(Circos))>
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

        Return SMRUCC.genomics.Visualize.Circos.DataExtensions.FromBlastnMappings(blastn, chrNts)
    End Function

    <ExportAPI("track_hits")>
    <RApiReturn(GetType(ValueTrackData))>
    Public Function HitsTracks(source As Object,
                            karyotype As SkeletonInfo,
                            Optional steps% = 2048, Optional env As Environment = Nothing) As Object

        Dim blastn As PipeIterator(Of BlastnMapping) = pipeline.Stream(Of BlastnMapping)(source, env)

        If blastn.isError Then
            Return blastn.getError
        End If

        Return blastn.Hits(karyotype, steps)
    End Function

    ''' <summary>
    ''' Creates the circos gene circle from the PTT database which is defined 
    ''' in the ``*.ptt/*.rnt`` file, and you can download this directory from 
    ''' the NCBI FTP website.
    ''' </summary>
    ''' <param name="PTT"></param>
    ''' <param name="COG"></param>
    ''' <param name="defaultColor"></param>
    ''' <returns></returns>
    <ExportAPI("genome_circle")>
    <RApiReturn(GetType(PTTMarks))>
    Public Function genome_circle(PTT As PTTDbLoader, COG As Object, Optional defaultColor As String = "blue", Optional env As Environment = Nothing) As Object
        Dim cogData As PipeIterator(Of MyvaCOG) = pipeline.Stream(Of MyvaCOG)(COG, env)

        If cogData.isError Then
            Return cogData.getError
        End If

        Dim data As New PTTMarks(PTT, cogData.ToArray, defaultColor)
        Return data
    End Function
End Module
