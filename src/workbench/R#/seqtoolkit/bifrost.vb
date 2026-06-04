Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Annotation.Prodigal
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

<Package("bifrost")>
<RTypeExport("prodigal", GetType(TrainingModel))>
Module bifrost

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(PredictionResult()), AddressOf scoreTable)
    End Sub

    <ExportAPI("as.data.frame")>
    Public Function scoreTable(result As PredictionResult(), args As list, env As Environment) As Object

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
