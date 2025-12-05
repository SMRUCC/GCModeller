Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome.Kmers
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

<Package("kmers")>
<RTypeExport("kmer", GetType(KmerSeed))>
Module KmersTool

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(SequenceHit()), AddressOf hitTable)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function hitTable(hits As SequenceHit(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        Call df.add(NameOf(SequenceHit.reads_title), From h As SequenceHit In hits Select h.reads_title)
        Call df.add(NameOf(SequenceHit.id), From h As SequenceHit In hits Select h.id)
        Call df.add(NameOf(SequenceHit.ncbi_taxid), From h As SequenceHit In hits Select h.ncbi_taxid)
        Call df.add(NameOf(SequenceHit.accession_id), From h As SequenceHit In hits Select h.accession_id)
        Call df.add(NameOf(SequenceHit.name), From h As SequenceHit In hits Select h.name)
        Call df.add(NameOf(SequenceHit.identities), From h As SequenceHit In hits Select h.identities)
        Call df.add(NameOf(SequenceHit.ratio), From h As SequenceHit In hits Select h.ratio)
        Call df.add(NameOf(SequenceHit.total), From h As SequenceHit In hits Select h.total)
        Call df.add(NameOf(SequenceHit.score), From h As SequenceHit In hits Select h.score)

        Return df
    End Function

    <ExportAPI("bayes_background")>
    <RApiReturn(GetType(KmerBackground))>
    Public Function bayes_background(<RRawVectorArgument> kmers_db As Object,
                                     ncbi_taxonomy As NcbiTaxonomyTree,
                                     seq_id As SequenceCollection,
                                     <RRawVectorArgument(TypeCodes.string)>
                                     Optional rank As Object = "species|genus|family|order|class|phylum|superkingdom",
                                     Optional env As Environment = Nothing) As Object

        Dim estimate As New PriorProbabilityBuilder(ncbi_taxonomy)
        Dim pullKmers As pipeline = pipeline.TryCreatePipeline(Of KmerSeed)(kmers_db, env)
        Dim targetRank As String = CLRVector.safeCharacters(rank).ElementAtOrDefault(0, "genus")

        If pullKmers.isError Then
            Return pullKmers.getError
        End If

        Dim kmers As IEnumerable(Of KmerSeed) = pullKmers.populates(Of KmerSeed)(env)
        Dim prior = estimate.BuildDatabase(kmers, seq_id, targetRank)

        Return prior
    End Function

    <ExportAPI("read_seqid")>
    Public Function readSequenceDb(file As String) As SequenceCollection
        Return SequenceCollection.Load(file)
    End Function
End Module
