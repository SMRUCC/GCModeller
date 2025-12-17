Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.Metagenome.Kmers
Imports SMRUCC.genomics.Analysis.Metagenome.Kmers.Kraken2
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

<Package("kmers")>
<RTypeExport("kmer", GetType(KmerSeed))>
<RTypeExport("kmer_bloom", GetType(KmerBloomFilter))>
Module KmersTool

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(SequenceHit()), AddressOf hitTable)

        Call RInternal.generic.add("readBin.kmer_bloom", GetType(Stream), AddressOf readKmerBloomFilter)
        Call RInternal.generic.add("writeBin", GetType(KmerBloomFilter), AddressOf writeKmerBloomFilter)
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

    Public Function readKmerBloomFilter(file As System.IO.Stream, args As list, env As Environment) As Object
        Return KmerBloomFilter.LoadFromFile(file)
    End Function

    Public Function writeKmerBloomFilter(filter As KmerBloomFilter, args As list, env As Environment) As Object
        Dim s As System.IO.Stream = args!con
        Call filter.Save(s)
        Call s.Flush()
        Return True
    End Function

    <ExportAPI("write.kmers_background")>
    Public Function write_background(bayes As KmerBackground, dirpath As String) As Object
        Call bayes.Save(dirpath)
        Return True
    End Function

    <ExportAPI("read.kmers_background")>
    Public Function load_background(dirpath As String) As KmerBackground
        Return KmerBackground.Load(dirpath)
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

    <ExportAPI("bloom_filters")>
    Public Function scanBloomDatabase(repo_dir As String, ncbi_taxonomy As NcbiTaxonomyTree,
                                      Optional min_supports As Double = 0.5,
                                      Optional coverage As Double = 0.95) As BloomDatabase

        Dim bloomfiles As String() = repo_dir.EnumerateFiles("*.ksbloom").ToArray
        Dim lca As New LCA(ncbi_taxonomy)
        Dim genomes As IEnumerable(Of KmerBloomFilter) =
            From file As String
            In TqdmWrapper.Wrap(bloomfiles)
            Select KmerBloomFilter.LoadFromFile(file)
        Dim kdb As New BloomDatabase(genomes, lca, min_supports, coverage)
        Return kdb
    End Function

    ''' <summary>
    ''' just make reads classify of the fastq reads based on the k-mer distribution
    ''' </summary>
    ''' <param name="db"></param>
    ''' <param name="reads"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' apply this method for do host sequence filter
    ''' </remarks>
    <ExportAPI("make_classify")>
    <RApiReturn(GetType(SequenceHit), GetType(KrakenOutputRecord))>
    Public Function make_classify(db As Object, <RRawVectorArgument> reads As Object, Optional env As Environment = Nothing) As Object
        Dim readsFile As pipeline = pipeline.TryCreatePipeline(Of FastQ)(reads, env)
        Dim readsData As IEnumerable(Of FastQ)

        If readsFile.isError OrElse TypeOf reads Is FastQFile Then
            If TypeOf reads Is FastQFile Then
                readsData = DirectCast(reads, FastQFile).AsEnumerable
            Else
                Return readsFile.getError
            End If
        Else
            readsData = readsFile.populates(Of FastQ)(env)
        End If

        If TypeOf db Is DatabaseReader Then
            Dim classifier As New Classifier(DirectCast(db, DatabaseReader))
            Dim labels As New List(Of SequenceHit)

            For Each read As FastQ In TqdmWrapper.Wrap(readsData.ToArray)
                Dim hit = classifier.MakeClassify(read.SequenceData)

                hit.reads_title = read.SEQ_ID
                labels.Add(hit)
            Next

            Return labels.ToArray
        ElseIf TypeOf db Is BloomDatabase Then
            Dim classifier As BloomDatabase = DirectCast(db, BloomDatabase)
            Dim rawdata As FastQ() = readsData.ToArray
            Dim labels As KrakenOutputRecord() = New KrakenOutputRecord(rawdata.Length - 1) {}
            Dim opt As New ParallelOptions With {.MaxDegreeOfParallelism = 1}

            Call Parallel.For(0, labels.Length, opt,
                body:=Sub(i)
                          Dim read As FastQ = rawdata(i)
                          Dim label As KrakenOutputRecord = classifier.MakeClassify(read)

                          labels(i) = label
                      End Sub)

            ' For Each read As FastQ In TqdmWrapper.Wrap(readsData.ToArray)
            '     Call labels.Add(classifier.MakeClassify(read))
            ' Next

            Return labels
        Else
            Return Message.InCompatibleType(GetType(DatabaseReader), db.GetType, env)
        End If
    End Function

    <ExportAPI("bayes_estimate")>
    Public Function bayes_estimate(background As KmerBackground, taxonomyDB As NcbiTaxonomyTree, seq_ids As SequenceCollection) As AbundanceEstimate
        Return New AbundanceEstimate(taxonomyDB).SetBackground(background).SetSequenceDb(seq_ids)
    End Function

    ''' <summary>
    ''' quantify of the metagenome community via kmers and bayes method
    ''' </summary>
    ''' <param name="db"></param>
    ''' <param name="bayes"></param>
    ''' <param name="reads">all reads data in one sample</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("bayes_abundance")>
    <RApiReturn(TypeCodes.double)>
    Public Function quantify(db As DatabaseReader, bayes As AbundanceEstimate, <RRawVectorArgument> reads As Object,
                             <RRawVectorArgument(TypeCodes.string)>
                             Optional rank As Object = "genus|family|order|class|phylum|species",
                             Optional env As Environment = Nothing) As Object

        Dim readsFile As pipeline = pipeline.TryCreatePipeline(Of FastQ)(reads, env)
        Dim readsData As IEnumerable(Of FastQ)
        Dim rank_str As String = CLRVector.asScalarCharacter(rank)

        If readsFile.isError OrElse TypeOf reads Is FastQFile Then
            If TypeOf reads Is FastQFile Then
                readsData = DirectCast(reads, FastQFile).AsEnumerable
            Else
                Return readsFile.getError
            End If
        Else
            readsData = readsFile.populates(Of FastQ)(env)
        End If

        Dim all_kmers As KmerSeed() = db.QueryKmers(readsData).ToArray
        Dim all_speciesId = all_kmers.Select(Function(a) a.source) _
            .IteratesALL _
            .Select(Function(a) bayes.LookupTaxonomyId(a.seqid)) _
            .Distinct _
            .Where(Function(taxid) taxid > 0) _
            .Select(Function(taxid) bayes.GetParentTaxIdAtRank(taxid, rank_str)) _
            .Where(Function(taxid) taxid > 0) _
            .Distinct _
            .ToArray

        Dim abundance As New Dictionary(Of Integer, Double)

        ' 4. 循环处理每一个你关心的属
        For Each ncbi_taxid As UInteger In all_speciesId
            ' 5. 调用贝叶斯估算函数，传入完整的k-mer数据和目标属的ID
            ' 函数内部会自己筛选出相关的k-mer并进行计算
            Dim abundanceForThisGenus As Dictionary(Of Integer, Double) = bayes.EstimateAbundanceForGenus(all_kmers, ncbi_taxid)

            ' 6. 将计算出的属内物种丰度合并到该样本的总丰度字典中
            For Each kvp In abundanceForThisGenus
                abundance(kvp.Key) = abundance.TryGetValue(kvp.Key, [default]:=0.0) + kvp.Value
            Next
        Next

        Return abundance
    End Function

    <ExportAPI("as.abundance_matrix")>
    <RApiReturn(GetType(Matrix))>
    Public Function metagenome_matrix(<RRawVectorArgument> samples As Object,
                                      Optional normalized As Boolean = False,
                                      Optional env As Environment = Nothing) As Object

        Dim sampleList As New List(Of NamedValue(Of Dictionary(Of String, Double)))
        Dim input As list = TryCast(samples, list)

        If input Is Nothing Then
            Throw New NotImplementedException
        End If

        For Each name As String In input.getNames
            Dim data As Object = input.getByName(name)
            Dim v As Dictionary(Of String, Double)

            If TypeOf data Is list Then
                v = DirectCast(data, list).AsGeneric(Of Double)(env)
            ElseIf TypeOf data Is Dictionary(Of String, Double) Then
                v = DirectCast(data, Dictionary(Of String, Double))
            ElseIf TypeOf data Is Dictionary(Of String, Integer) Then
                v = DirectCast(data, Dictionary(Of String, Integer)).AsNumeric
            ElseIf TypeOf data Is Dictionary(Of Integer, Double) Then
                v = DirectCast(data, Dictionary(Of Integer, Double)) _
                    .ToDictionary(Function(a) a.ToString,
                                  Function(a)
                                      Return a.Value
                                  End Function)
            Else
                Throw New NotImplementedException
            End If

            Call sampleList.Add(New NamedValue(Of Dictionary(Of String, Double))(name, v))
        Next

        Return AbundanceMatrixBuilder.BuildAndNormalizeAbundanceMatrix(sampleList.ToArray, normalized)
    End Function

    ''' <summary>
    ''' cast the genomics sequence as kmer based bloom filter model
    ''' </summary>
    ''' <param name="genomics"></param>
    ''' <param name="ncbi_taxid"></param>
    ''' <param name="k"></param>
    ''' <param name="fpr"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("as.bloom_filter")>
    <RApiReturn(GetType(KmerBloomFilter))>
    Public Function bloom_filter(<RRawVectorArgument> genomics As Object,
                                 Optional ncbi_taxid As Integer = 0,
                                 Optional k As Integer = 35,
                                 Optional fpr As Double = 0.00001,
                                 Optional env As Environment = Nothing) As Object

        Dim seqs As IEnumerable(Of FastaSeq) = pipHelper.GetFastaSeq(genomics, env)
        Dim kmers As KmerBloomFilter = KmerBloomFilter.Create(seqs, ncbi_taxid, k, fpr)
        Return kmers
    End Function

    <ExportAPI("parse_kraken_output")>
    Public Function parse_kraken_output(filepath As String) As KrakenOutputRecord()
        Return KrakenOutputRecord.ParseDocument(filepath).ToArray
    End Function

    <ExportAPI("parse_kraken_report")>
    Public Function parse_kraken_report(filepath As String) As KrakenReportRecord()
        Return KrakenReportRecord.ParseDocument(filepath).ToArray
    End Function

    <ExportAPI("host_classification")>
    <RApiReturn(TypeCodes.integer)>
    Public Function filter_classification(<RRawVectorArgument> kraken_output As Object,
                                          <RRawVectorArgument> host_taxids As Object,
                                          Optional env As Environment = Nothing) As Object

        Dim taxIndex As Index(Of Long) = CLRVector.asLong(host_taxids)
        Dim kraken2 As pipeline = pipeline.TryCreatePipeline(Of KrakenOutputRecord)(kraken_output, env)

        If kraken2.isError Then
            Return kraken2.getError
        End If

        Return pipeline.CreateFromPopulator(From c As KrakenOutputRecord
                                            In kraken2.populates(Of KrakenOutputRecord)(env)
                                            Where c.TaxID Like taxIndex)
    End Function

    <ExportAPI("filter_reads")>
    <RApiReturn(GetType(FastQ))>
    Public Function filter_reads(<RRawVectorArgument> kraken_output As Object,
                                 <RRawVectorArgument> reads As Object,
                                 Optional env As Environment = Nothing) As Object

        Dim kraken2 As pipeline = pipeline.TryCreatePipeline(Of KrakenOutputRecord)(kraken_output, env)
        Dim readsData As pipeline = pipeline.TryCreatePipeline(Of FastQ)(reads, env)
        Dim reads_data As IEnumerable(Of FastQ)

        If kraken2.isError Then
            Return kraken2.getError
        ElseIf readsData.isError Then
            If TypeOf reads Is FastQFile Then
                reads_data = DirectCast(reads, FastQFile).AsEnumerable
            Else
                Return readsData.getError
            End If
        Else
            reads_data = readsData.populates(Of FastQ)(env)
        End If

        Dim filterIndex As Index(Of String) = kraken2 _
            .populates(Of KrakenOutputRecord)(env) _
            .Select(Iterator Function(k) As IEnumerable(Of String)
                        Yield k.ReadName
                        Yield "@" & k.ReadName
                    End Function) _
            .IteratesALL _
            .Indexing
        Dim result As pipeline = pipeline.CreateFromPopulator(From fq As FastQ
                                                              In reads_data
                                                              Where Not fq.SEQ_ID Like filterIndex)
        Return result
    End Function
End Module
