
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SeqMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix

<Package("kmers")>
Module kmersTools

    <ExportAPI("kmers")>
    Public Function kmers(seq As String, k As Integer) As String()
        Return KSeq.KmerSpans(seq, k).ToArray
    End Function

    ''' <summary>
    ''' generate sequence k-mer count data matrix
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="k"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("kmers_matrix")>
    <RApiReturn(GetType(SeqMatrix))>
    Public Function kmers_matrix(<RRawVectorArgument> x As Object, Optional k As Integer = 3, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline

        If TypeOf x Is FastQFile Then
            pull = pipeline.CreateFromPopulator(DirectCast(x, FastQFile).ToArray)
        Else
            pull = pipeline.TryCreatePipeline(Of IFastaProvider)(x, env)
        End If

        If pull.isError Then
            Return pull.getError
        End If

        Dim seqs As New List(Of NamedValue(Of Dictionary(Of String, Double)))

        For Each seq As IFastaProvider In pull.populates(Of IFastaProvider)(env)
            Call seqs.Add(KMers.KMerSample(seq, k))
        Next

        Dim features As String() = seqs.Select(Function(a) a.Value.Keys) _
            .IteratesALL _
            .GroupBy(Function(m) m) _
            .Keys _
            .OrderBy(Function(m) m) _
            .ToArray
        Dim samples As New List(Of DataFrameRow)

        For Each seq As NamedValue(Of Dictionary(Of String, Double)) In seqs
            Dim counts = seq.Value
            Dim v As IEnumerable(Of Double) = From ks As String
                                              In features
                                              Select If(counts.ContainsKey(ks), counts(ks), 0.0)

            Call samples.Add(New DataFrameRow(seq.Name, v))
        Next

        Return New SeqMatrix With {
            .expression = samples.ToArray,
            .sampleID = features,
            .tag = $"k-mer(k={k})"
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="x">should be a collection of the <see cref="FastaSeq"/> sequence collection</param>
    ''' <param name="type">the sequence data type, default is protein sequence</param>
    ''' <param name="k">the length of the k-mers</param>
    ''' <param name="L2_norm">do L2 normalized of the generated matrix data?</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' make sequence embedding via TF-IDF algorithm which is implemented via <see cref="KmerTFIDFVectorizer"/>
    ''' </remarks>
    <ExportAPI("tfidf_vectorizer")>
    Public Function tfidf_vectorizer(<RRawVectorArgument> x As Object,
                                     Optional type As SeqTypes = SeqTypes.Protein,
                                     Optional k As Integer = 6,
                                     Optional L2_norm As Boolean = False,
                                     Optional env As Environment = Nothing) As Object

        Dim seqs As IEnumerable(Of FastaSeq) = GetFastaSeq(x, env)

        If seqs Is Nothing Then
            Return Nothing
        End If

        Dim latent As New KmerTFIDFVectorizer(type, k)
        Call latent.AddRange(seqs)
        Dim vec = latent.TfidfVectorizer(L2_norm)
        Dim df As New dataframe With {
            .rownames = vec.rownames,
            .columns = vec.featureSet _
                .ToDictionary(Function(a) a.name,
                              Function(a)
                                  Return a.vector
                              End Function)
        }

        Return df
    End Function

End Module
