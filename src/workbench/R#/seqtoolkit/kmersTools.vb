#Region "Microsoft.VisualBasic::d6e1da6a747e6ff39c51c5b8bd9afc58, R#\seqtoolkit\kmersTools.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 193
    '    Code Lines: 124 (64.25%)
    ' Comment Lines: 40 (20.73%)
    '    - Xml Docs: 92.50%
    ' 
    '   Blank Lines: 29 (15.03%)
    '     File Size: 7.52 KB


    ' Module kmersTools
    ' 
    '     Function: cdhit_clusters, cdhit_nr, kmers_from_seq, kmers_matrix, oneHot_vectorizer
    '               tfidf_vectorizer
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.SequenceAlignment
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.genomics.SequenceModel.Slicer
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SeqMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix

''' <summary>
''' The sequence k-mer tools
''' </summary>
''' 
''' <remarks>
''' This R# package module provides the toolkit for make the k-mer based 
''' sequence data analysis:
''' 
''' + ``kmers``: generate the k-mer sequence fragments from a given sequence 
'''   data in a sliding window manner;
''' + ``kmers_matrix``: generate the k-mer count matrix of a given sequence 
'''   collection;
''' + ``tfidf_vectorizer`` and ``onehot_vectorizer``: make the sequence 
'''   embedding via the bag-of-k-mers model, the TF-IDF weight or the one-hot 
'''   encoding vector;
''' + ``cdhit_nr`` and ``cdhit_clusters``: run the CD-HIT like sequence 
'''   clustering for get the non-redundant sequence set or the cluster 
'''   family table.
''' </remarks>
<Package("kmers")>
Module kmersTools

    ''' <summary>
    ''' Create kmers from a given sequence
    ''' </summary>
    ''' <param name="seq">the raw sequence data text.</param>
    ''' <param name="k">the length of the k-mer sequence fragment.</param>
    ''' <returns>
    ''' a character vector of the k-mer sequence fragments, which is generated 
    ''' from the given sequence data via a sliding window of size ``k``, and 
    ''' the step size of the sliding window is just one char, so that all of the 
    ''' generated k-mer fragments are overlapped with each other.
    ''' 
    ''' an empty character vector will be returned if the value of the ``k`` 
    ''' parameter is greater than the length of the input sequence data.
    ''' </returns>
    ''' 
    ''' <example>
    ''' imports "kmers" from "seqtoolkit";
    ''' 
    ''' # all of the generated k-mers are overlapped:
    ''' # "ATGGC" -&gt; "ATG", "TGG", "GGC"
    ''' print(kmers("ATGGC", k = 3));
    ''' </example>
    <ExportAPI("kmers")>
    Public Function kmers_from_seq(seq As String, k As Integer) As String()
        Return KSeq.KmerSpans(seq, k).ToArray
    End Function

    ''' <summary>
    ''' generate sequence k-mer count data matrix
    ''' </summary>
    ''' <param name="x">
    ''' a collection of the sequence data, which can be a fasta sequence 
    ''' collection(<see cref="FastaSeq"/>, <see cref="FastaFile"/>), a fastq 
    ''' sequence collection(<see cref="FastQFile"/>) or any other 
    ''' <see cref="IFastaProvider"/> sequence data model, or a pipeline object 
    ''' that produces a set of the sequence data.
    ''' </param>
    ''' <param name="k">
    ''' the length of the k-mer sequence fragment for make the count.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a <see cref="SeqMatrix"/> k-mer count matrix object: each row in this 
    ''' matrix is a sequence in the input sequence collection(the row name is 
    ''' the sequence title), and each column is a k-mer feature(the ``sampleID`` 
    ''' property of the generated matrix is the k-mer alphabet sorted in 
    ''' ascending order), the cell value is the count of the corresponding k-mer 
    ''' in the corresponding sequence(ZERO means that the k-mer is not exists in 
    ''' the target sequence).
    ''' 
    ''' this function returns a R# error message object if the input data can not 
    ''' be cast to a collection of the sequence data.
    ''' </returns>
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
    ''' make the sequence embedding via the TF-IDF weight of the bag-of-k-mers 
    ''' model
    ''' </summary>
    ''' <param name="x">should be a collection of the <see cref="FastaSeq"/> sequence collection</param>
    ''' <param name="type">the sequence data type, default is protein sequence</param>
    ''' <param name="k">the length of the k-mers</param>
    ''' <param name="L2_norm">do L2 normalized of the generated matrix data?</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a data frame object: each row is a sequence in the input sequence 
    ''' collection(the row name is the fasta title of the corresponding 
    ''' sequence), and each column is a k-mer term, the cell value is the TF-IDF 
    ''' weight of the corresponding k-mer in the corresponding sequence.
    ''' 
    ''' this function returns NULL if the input data can not be cast to a fasta 
    ''' sequence collection.
    ''' </returns>
    ''' <remarks>
    ''' make sequence embedding via TF-IDF algorithm which is implemented via <see cref="KmerTFIDFVectorizer"/>
    ''' 
    ''' the generated embedding vector of each sequence will be normalized to an 
    ''' unit vector when the ``L2_norm`` parameter is TRUE, which is helpful for 
    ''' the cosine similarity or euclidean distance measurement between the 
    ''' embedding vectors of the different length sequences.
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
        Dim df As dataframe = vec.toDataframe(list.empty, env)
        Return df
    End Function

    ''' <summary>
    ''' make the sequence embedding via the one-hot encoding(Bag-of-n-grams) of 
    ''' the k-mer composition
    ''' </summary>
    ''' <param name="x">
    ''' should be a collection of the <see cref="FastaSeq"/> sequence 
    ''' collection, which can be a <see cref="FastaFile"/> object, a vector of 
    ''' the <see cref="FastaSeq"/> object, or a character vector of the raw 
    ''' sequence data.
    ''' </param>
    ''' <param name="type">
    ''' the sequence data type, default is protein sequence. If the sequence type 
    ''' is not protein, then the input sequence data will be canonicalized as the 
    ''' standard nucleotide letters at first.
    ''' </param>
    ''' <param name="k">the length of the k-mers</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a data frame object: each row is a sequence in the input sequence 
    ''' collection(the row name is the fasta title of the corresponding 
    ''' sequence), and each column is a k-mer term, the cell value is ONE when the 
    ''' k-mer is exists in the corresponding sequence, otherwise ZERO.
    ''' 
    ''' this function returns NULL if the input data can not be cast to a fasta 
    ''' sequence collection.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' unlike the ``tfidf_vectorizer`` api, which evaluates the weight of each 
    ''' k-mer term by the term frequency and the inverse document frequency, this 
    ''' api just encodes the k-mer composition of the sequence data as a binary 
    ''' vector, i.e. the presence or absence of each k-mer term.
    ''' </remarks>
    <ExportAPI("onehot_vectorizer")>
    Public Function oneHot_vectorizer(<RRawVectorArgument> x As Object,
                                      Optional type As SeqTypes = SeqTypes.Protein,
                                      Optional k As Integer = 6,
                                      Optional env As Environment = Nothing) As Object

        Dim seqs As IEnumerable(Of FastaSeq) = GetFastaSeq(x, env)

        If seqs Is Nothing Then
            Return Nothing
        End If

        Dim latent As New KmerTFIDFVectorizer(type, k)
        Call latent.AddRange(seqs)
        Dim vec = latent.OneHotVectorizer
        Dim df As dataframe = vec.toDataframe(list.empty, env)
        Return df
    End Function

    ''' <summary>
    ''' run the CD-HIT like sequence clustering for get the non-redundant 
    ''' sequence set
    ''' </summary>
    ''' <param name="x">
    ''' a collection of the sequence data for run the clustering, which can be a 
    ''' <see cref="FastaFile"/> object, a vector of the <see cref="FastaSeq"/> 
    ''' object, or a character vector of the raw sequence data.
    ''' </param>
    ''' <param name="k">
    ''' the k-mer size for build the min-hash sketch of the sequence data: 
    ''' 
    ''' + protein - k=5aa
    ''' + nucleotide - k=12nt
    ''' + genomics - k=31nt
    ''' </param>
    ''' <param name="identities">
    ''' the sequence identity threshold of the cluster members: the sequences 
    ''' that their identity is greater than or equals to this threshold value will 
    ''' be clustered into the same cluster.
    ''' </param>
    ''' <param name="n_threads">
    ''' the thread number for run the min-hash task in parallel.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a vector of the <see cref="FastaSeq"/> sequence object: the 
    ''' representative sequence of each cluster. For a cluster that contains 
    ''' multiple sequence members, the fasta headers of the representative 
    ''' sequence is formatted as: the representative sequence title, 
    ''' ``{cluster_size} cluster members`` and the json text of the cluster member 
    ''' sequence id list; and the sequence data of a singleton cluster(the unique 
    ''' sequence) is returned as is.
    ''' 
    ''' this function returns NULL if the input data can not be cast to a fasta 
    ''' sequence collection.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' the input sequence data will be sorted by the sequence length in 
    ''' descending order at first, and then the greedy clustering algorithm runs 
    ''' based on the min-hash similarity of the k-mer sketch of each sequence.
    ''' </remarks>
    <ExportAPI("cdhit_nr")>
    <RApiReturn(GetType(FastaSeq))>
    Public Function cdhit_nr(<RRawVectorArgument> x As Object,
                             Optional k As Integer = 12,
                             Optional identities As Double = 0.8,
                             Optional n_threads As Integer? = Nothing,
                             Optional env As Environment = Nothing) As Object

        Dim seqs As IEnumerable(Of FastaSeq) = GetFastaSeq(x, env)

        If seqs Is Nothing Then
            Return Nothing
        End If

        Dim cdhit As CDHit = New CDHit(k, n_threads:=n_threads).Setup(seqs)
        Dim nr As FastaSeq() = cdhit.NrSeqs(threshold:=identities).ToArray

        Return nr
    End Function

    ''' <summary>
    ''' run the CD-HIT like sequence clustering and then export the cluster 
    ''' result as a set of the cluster tables
    ''' </summary>
    ''' <param name="x">
    ''' a collection of the sequence data for run the clustering, which can be a 
    ''' <see cref="FastaFile"/> object, a vector of the <see cref="FastaSeq"/> 
    ''' object, or a character vector of the raw sequence data.
    ''' </param>
    ''' <param name="k">
    ''' the k-mer size for build the min-hash sketch of the sequence data: 
    ''' 
    ''' + protein - k=5aa
    ''' + nucleotide - k=12nt
    ''' + genomics - k=31nt
    ''' </param>
    ''' <param name="identities">
    ''' the sequence identity threshold of the cluster members: the sequences 
    ''' that their identity is greater than or equals to this threshold value will 
    ''' be clustered into the same cluster.
    ''' </param>
    ''' <param name="n_threads">
    ''' the thread number for run the min-hash task in parallel.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a tuple list that contains the data slots:
    ''' 
    ''' - family: a vector of the <see cref="FamilyExports"/> object, each 
    '''   element is the summary data of one cluster: the ``family_id``, the 
    '''   ``members`` cluster size, and the ``representative``/``rep_seq`` data of 
    '''   the representative sequence;
    ''' - sequence: a vector of the <see cref="SequenceCluster"/> object, each 
    '''   element is the data of one cluster member: the ``seq_title``, the 
    '''   ``family_id``, the ``score`` identity to the cluster representative and 
    '''   the ``seq`` sequence data;
    ''' - clusters: a vector of the <see cref="SimilarHit"/> object, which is the 
    '''   raw cluster result of the CD-HIT like clustering: the ``SeqID`` is the 
    '''   representative sequence of the cluster and the ``Similar`` property is 
    '''   the identity score of each cluster member to the representative 
    '''   sequence.
    ''' 
    ''' this function returns NULL if the input data can not be cast to a fasta 
    ''' sequence collection.
    ''' </returns>
    <ExportAPI("cdhit_clusters")>
    Public Function cdhit_clusters(<RRawVectorArgument> x As Object,
                                   Optional k As Integer = 12,
                                   Optional identities As Double = 0.8,
                                   Optional n_threads As Integer? = Nothing,
                                   Optional env As Environment = Nothing) As Object

        Dim seqs As IEnumerable(Of FastaSeq) = GetFastaSeq(x, env)

        If seqs Is Nothing Then
            Return Nothing
        End If

        Dim cdhit As CDHit = New CDHit(k, n_threads:=n_threads).Setup(seqs)
        Dim clusters As SimilarHit() = cdhit.FindSimilar(identities).ToArray
        Dim result = cdhit.GetSequencePool.ExportClusters(clusters)

        Return New list(slot("family") = result.family,
                        slot("sequence") = result.clusters,
                        slot("clusters") = clusters)
    End Function
End Module
