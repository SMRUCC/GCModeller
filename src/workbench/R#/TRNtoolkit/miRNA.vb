
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.XmlFile
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("miRNA")>
Module miRNA

    <ExportAPI("psRNATarget")>
    Public Function psRNATarget_clr(Optional version As psRNATarget.Schema = psRNATarget.Schema.V2_2017, Optional max_expectation As Double = 5.0) As psRNATarget
        Return New psRNATarget With {.Version = version, .MaxExpectation = max_expectation}
    End Function

    <ExportAPI("TargetFinder")>
    Public Function TargetFinder(Optional score_cutoff As Double = 5.0) As TargetFinder
        Return New TargetFinder With {.ScoreCutoff = score_cutoff}
    End Function

    <ExportAPI("miRNA_targets")>
    <RApiReturn(GetType(siRNAHit))>
    Public Function miRNA_targets(mapper As miRNAMapper,
                                  <RRawVectorArgument(GetType(FastaSeq))> miRNAs As Object,
                                  <RRawVectorArgument(GetType(FastaSeq))> targets As Object,
                                  Optional env As Environment = Nothing) As Object

        Dim miRNAList = GetFastaSeq(miRNAs, env)
        Dim targetDb = GetFastaSeq(targets, env).SafeQuery.ToArray

        If miRNAList Is Nothing Then
            Return Nothing
        ElseIf targetDb.IsNullOrEmpty Then
            Return Nothing
        End If

        Dim result As New List(Of siRNAHit)

        For Each miRNA As FastaSeq In miRNAList
            Call result.AddRange(mapper.Run(miRNA, targetDb))
        Next

        Return result.ToArray
    End Function

    ''' <summary>
    ''' --- High-confidence intersection (psRNATarget ∩ TargetFinder) ---
    ''' </summary>
    ''' <param name="psRNATarget"></param>
    ''' <param name="TargetFinder"></param>
    ''' <param name="site_tolerance"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("intersect_targets")>
    <RApiReturn(GetType(siRNAHit))>
    Public Function intersect_targets(<RRawVectorArgument(GetType(siRNAHit))> psRNATarget As Object,
                                      <RRawVectorArgument(GetType(siRNAHit))> TargetFinder As Object,
                                      Optional site_tolerance As Integer = 3,
                                      Optional env As Environment = Nothing) As Object
        ' ---- 交集（高置信靶标）----
        Dim merger As New Intersection() With {.SiteTolerance = site_tolerance}
        Dim psrnaHits As PipeIterator(Of siRNAHit) = pipeline.Stream(Of siRNAHit)(psRNATarget, env)
        Dim tfHits As PipeIterator(Of siRNAHit) = pipeline.Stream(Of siRNAHit)(TargetFinder, env)

        If psrnaHits.isError Then
            Return psrnaHits.getError
        ElseIf tfHits.isError Then
            Return tfHits.getError
        End If

        Dim intersect As siRNAHit() = merger.Merge(psrnaHits, tfHits).ToArray

        Return intersect
    End Function
End Module
