Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.PrimerDesigner
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.Slicer
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

<Package("primers")>
Module primers

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(CandidateRegion), AddressOf CandidateTable)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function CandidateTable(candidate As CandidateRegion, args As list, env As Environment) As Object
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Dim core_id = candidate.GenesInCoreRegion.Select(Function(g) g.ID)
        Dim ext_id = candidate.GenesInExtendedRegion.Select(Function(g) g.ID)
        Dim core_left = candidate.GenesInCoreRegion.Select(Function(g) g.left)
        Dim ext_left = candidate.GenesInExtendedRegion.Select(Function(g) g.left)
        Dim core_right = candidate.GenesInCoreRegion.Select(Function(g) g.right)
        Dim ext_right = candidate.GenesInExtendedRegion.Select(Function(g) g.right)
        Dim core_len = candidate.GenesInCoreRegion.Select(Function(g) g.Length)
        Dim ext_len = candidate.GenesInExtendedRegion.Select(Function(g) g.Length)
        Dim core_strand = candidate.GenesInCoreRegion.Select(Function(g) g.strand.Description)
        Dim ext_strand = candidate.GenesInExtendedRegion.Select(Function(g) g.strand.Description)
        Dim hits As String = candidate.SupportingHits.Select(Function(h) $"{h.QueryID}({h.SubjectStart}|{h.SubjectEnd})").JoinBy("; ")
        Dim core_type = candidate.GenesInCoreRegion.Select(Function(any) "core")
        Dim ext_type = candidate.GenesInExtendedRegion.Select(Function(any) "extended")
        Dim fna As ChunkedNtFasta = args.getBySynonyms("fna")

        Call df.add("chr", scalar:=candidate.Chr)
        Call df.add("primer_start", scalar:=candidate.CoreStart)
        Call df.add("primer_ends", scalar:=candidate.CoreEnd)
        Call df.add("core_span", scalar:=StringFormats.Lanudry(candidate.Span))
        Call df.add("extends_start", scalar:=candidate.ExtendedStart)
        Call df.add("extends_end", scalar:=candidate.ExtendedEnd)
        Call df.add("extends_span", scalar:=StringFormats.Lanudry(candidate.ExtensionLength))
        Call df.add("gene_id", core_id.JoinIterates(ext_id))
        Call df.add("gene_left", core_left.JoinIterates(ext_left))
        Call df.add("gene_right", core_right.JoinIterates(ext_right))
        Call df.add("gene_length", core_len.JoinIterates(ext_len))
        Call df.add("gene_strand", core_strand.JoinIterates(ext_strand))
        Call df.add("type", core_type.JoinIterates(ext_type))
        Call df.add("num_primer_hits", scalar:=candidate.SupportingHits.Count)
        Call df.add("primer_hits", scalar:=hits)

        If fna IsNot Nothing Then
            With New ChunkSlicer(fna)
                Call df.add("gene_seq", candidate.GenesInCoreRegion _
                       .JoinIterates(candidate.GenesInExtendedRegion) _
                       .Select(Function(gene)
                                   Return .SliceRegionSite(gene.left, gene.Length)
                               End Function))
            End With
        End If

        Return df
    End Function

    <ExportAPI("primer_regions")>
    <RApiReturn(GetType(CandidateRegion))>
    Public Function find_primers_region(<RRawVectorArgument>
                                        blastHits As Object,
                                        Optional maxCoreSpan As Integer = ISequenceModel.MB,
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
