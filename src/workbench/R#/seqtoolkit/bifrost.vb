Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Annotation.MetaEuk
Imports SMRUCC.genomics.Annotation.Prodigal
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

<Package("bifrost")>
<RTypeExport("prodigal", GetType(TrainingModel))>
<RTypeExport("metaeuk_config", GetType(MetaEukConfig))>
Module bifrost

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(PredictionResult()), AddressOf scoreTable)
    End Sub

    <ExportAPI("as.data.frame")>
    Public Function scoreTable(result As PredictionResult(), args As list, env As Environment) As Object
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Dim table As GeneScore() = GeneScore.ScoreTable(result).ToArray

        Call df.add(NameOf(GeneScore.seq_id), From gene In table Select gene.seq_id)
        Call df.add(NameOf(GeneScore.gene_index), From gene In table Select gene.gene_index)
        Call df.add(NameOf(GeneScore.start), From gene In table Select gene.start)
        Call df.add(NameOf(GeneScore.end), From gene In table Select gene.end)
        Call df.add(NameOf(GeneScore.strand), From gene In table Select gene.strand)
        Call df.add(NameOf(GeneScore.frame), From gene In table Select gene.frame)
        Call df.add(NameOf(GeneScore.start_codon), From gene In table Select gene.start_codon)
        Call df.add(NameOf(GeneScore.stop_codon), From gene In table Select gene.stop_codon)
        Call df.add(NameOf(GeneScore.rbs_motif), From gene In table Select gene.rbs_motif)
        Call df.add(NameOf(GeneScore.total_score), From gene In table Select gene.total_score)
        Call df.add(NameOf(GeneScore.coding_score), From gene In table Select gene.coding_score)
        Call df.add(NameOf(GeneScore.start_score), From gene In table Select gene.start_score)
        Call df.add(NameOf(GeneScore.rbs_score), From gene In table Select gene.rbs_score)
        Call df.add(NameOf(GeneScore.type_score), From gene In table Select gene.type_score)
        Call df.add(NameOf(GeneScore.upstream_score), From gene In table Select gene.upstream_score)
        Call df.add(NameOf(GeneScore.rbs_spacing), From gene In table Select gene.rbs_spacing)
        Call df.add(NameOf(GeneScore.partial_type), From gene In table Select gene.partial_type)

        Return df
    End Function

    <ExportAPI("prodigal_training")>
    <RApiReturn(GetType(TrainingModel))>
    Public Function training(<RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As Object
        Dim contigs As IEnumerable(Of FastaSeq) = GetFastaSeq(x, env)

        If contigs Is Nothing Then
            Return RInternal.debug.stop("there is no genome assembly sequence input!", env)
        Else
            Return ProdigalWorker.ModelTraining(New FastaFile(contigs))
        End If
    End Function

    ''' <summary>
    ''' Prodigal (PROkaryotic DYnamic programming Gene-finding ALgorithm)
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="min_ORF_len"></param>
    ''' <param name="model"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <example>
    ''' imports "bioseq.fasta" from "seqtoolkit";
    ''' imports "bifrost" from "seqtoolkit";
    ''' imports "annotation.genomics" from "seqtoolkit";
    ''' 
    ''' # an example workflow script for run prodigal gene prediction on MAGs contigs assembly sequence, 
    ''' # and export the result to files. The input contigs assembly sequence is in FASTA format, 
    ''' # and the output gene prediction result is in PredictionResult format, which can be further 
    ''' # converted to GFF3 format, or gene/protein FASTA format. The example workflow script is 
    ''' # as follows:
    ''' 
    ''' # read the contigs assembly sequence from a FASTA file
    ''' let MAGs &lt;- "MAGs_contigs.fasta";
    ''' let contigs &lt;- read.fasta(MAGs);
    ''' # predict genes on the contigs assembly sequence 
    ''' let result &lt;- prodigal(contigs, min.ORF.len = 90);
    ''' 
    ''' # export result to files
    ''' write.csv(as.data.frame(result), file = "gene_predicts.csv");
    ''' # export the gene prediction result to GFF3 format
    ''' write.gff3(as.gff3(result), file = "gene_predicts.gff3");
    ''' # export gene/protein fasta sequence to file
    ''' write.fasta(as.genes(result), file = "gene_predicts.fna");
    ''' write.fasta(as.proteins(result), file = "protein_predicts.faa");
    ''' </example>
    <ExportAPI("prodigal")>
    <RApiReturn(GetType(PredictionResult))>
    Public Function prodigal(<RRawVectorArgument> x As Object,
                             Optional min_ORF_len As Integer = 90,
                             Optional model As TrainingModel = Nothing,
                             Optional env As Environment = Nothing) As Object

        Dim contigs As IEnumerable(Of FastaSeq) = GetFastaSeq(x, env)

        If contigs Is Nothing Then
            Return RInternal.debug.stop("there is no MAGs contigs assembly sequence input!", env)
        End If

        Return ProdigalWorker.GenePrediction(
            MAGs:=New FastaFile(contigs),
            MinOrfLength:=min_ORF_len,
            model:=model).ToArray
    End Function

    <ExportAPI("metaeuk")>
    <RApiReturn(GetType(GenePrediction))>
    Public Function metaeuk(<RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As Object
        Dim contigs As IEnumerable(Of FastaSeq) = GetFastaSeq(x, env)
        Dim config As MetaEukConfig = Nothing

        If contigs Is Nothing Then
            Return RInternal.debug.stop("there is no MAGs contigs assembly sequence input!", env)
        ElseIf TypeOf x Is MetaEukConfig Then
            config = x
        End If

        Return MetaEukWorker.Predict(config).ToArray
    End Function

    ''' <summary>
    ''' cast the gene prediction result as GFF3 table format
    ''' </summary>
    ''' <param name="x">
    ''' the gene prediction result, which can be the output of "prodigal" function, or a pipeline that produces PredictionResult objects. The pipeline can be created by using the "pipeline" function in R#, and the final output of the pipeline should be PredictionResult objects. For example, if you have a pipeline that produces PredictionResult objects, you can pass it directly to this function to get the GFF3 table format output.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("as.gff3")>
    <RApiReturn(GetType(GFFTable))>
    Public Function AsGff(<RRawVectorArgument()> x As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of PredictionResult)(x, env, suppress:=True)

        If pull.isError Then
            Return pull.getError
        End If

        Return ResultWriter.CastToGff(pull.populates(Of PredictionResult)(env).ToArray)
    End Function

    ''' <summary>
    ''' Extract the protein sequences from the gene prediction result, and return as FASTA format. The sequence ID is in the format of "seqid_geneindex". For example, "contig1_5" means the 5th predicted gene on contig1. The sequence description is in the format of "start-end(strand)". For example, "100-900(+)" means the gene starts at position 100, ends at position 900, and is on the forward strand.
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("as.proteins")>
    <RApiReturn(GetType(FastaSeq))>
    Public Function GetProteins(<RRawVectorArgument()> x As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of PredictionResult)(x, env, suppress:=True)

        If pull.isError Then
            Return pull.getError
        End If

        Return ResultWriter.GetProteinSequences(pull.populates(Of PredictionResult)(env).ToArray).ToArray
    End Function

    ''' <summary>
    ''' Extract the gene sequences from the gene prediction result, and return as FASTA format. The sequence ID is in the format of "seqid_geneindex". For example, "contig1_5" means the 5th predicted gene on contig1. The sequence description is in the format of "start-end(strand)". For example, "100-900(+)" means the gene starts at position 100, ends at position 900, and is on the forward strand.
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("as.genes")>
    <RApiReturn(GetType(FastaSeq))>
    Public Function GetGenes(<RRawVectorArgument()> x As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of PredictionResult)(x, env, suppress:=True)

        If pull.isError Then
            Return pull.getError
        End If

        Return ResultWriter.GetGeneSequences(pull.populates(Of PredictionResult)(env).ToArray).ToArray
    End Function

End Module
