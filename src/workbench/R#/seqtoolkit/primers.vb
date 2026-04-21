Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.PrimerDesigner
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("primers")>
Module primers

    <ExportAPI("primer_regions")>
    <RApiReturn(GetType(CandidateRegion))>
    Public Function find_primers_region(<RRawVectorArgument>
                                        blastHits As Object,
                                        Optional maxCoreSpan As Integer = 2 * ISequenceModel.MB,
                                        Optional eval_cutoff As Double = 1,
                                        <RRawVectorArgument>
                                        Optional primerIds As Object = Nothing,
                                        Optional genome As GFFTable = Nothing,
                                        Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of HitRecord)(blastHits, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim siteHits As IEnumerable(Of HitRecord) = pull.populates(Of HitRecord)(env)
        Dim candidates As CandidateRegion() = CandidateRegion.FindCandidateRegions(
            siteHits, maxCoreSpan, eval_cutoff, CLRVector.asCharacter(primerIds))
        Dim context As Dictionary(Of String, GenomeContext(Of Feature)) = genome _
            .GetChromosomes(mRNA:=True) _
            .ToDictionary(Function(c) c.name,
                          Function(c)
                              Return New GenomeContext(Of Feature)(c, c.name)
                          End Function)

        Call CandidateRegion.CalculateExtensions(candidates, context)

        Return candidates
    End Function

End Module
