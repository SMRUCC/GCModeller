Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome.Kmers
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
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
    <RApiReturn(GetType(SequenceHit))>
    Public Function make_classify(db As DatabaseReader, <RRawVectorArgument> reads As Object, Optional env As Environment = Nothing) As Object
        Dim classifier As New Classifier(db)
        Dim readsFile As pipeline = pipeline.TryCreatePipeline(Of FastQ)(reads, env)
        Dim labels As New List(Of SequenceHit)
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

        For Each read As FastQ In TqdmWrapper.Wrap(readsData.ToArray)
            Dim hit = classifier.MakeClassify(read.SequenceData)

            hit.reads_title = read.SEQ_ID
            labels.Add(hit)
        Next

        Return labels.ToArray
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
                             Optional rank As Object = "genus|family|order|class|phylum|superkingdom",
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
    Public Function metagenome_matrix(<RListObjectArgument> samples As list,
                                      Optional normalized As Boolean = False,
                                      Optional env As Environment = Nothing) As Object

        Dim sampleList As New List(Of NamedValue(Of Dictionary(Of Integer, Double)))

        For Each name As String In samples.getNames
            Dim data As Object = samples.getByName(name)


        Next

        Return AbundanceMatrixBuilder.BuildAndNormalizeAbundanceMatrix(sampleList.ToArray, normalized)
    End Function
End Module
