#Region "Microsoft.VisualBasic::aab86347bb08ebf6e2d187c47d410fe5, R#\seqtoolkit\bifrost.vb"

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

    '   Total Lines: 182
    '    Code Lines: 105 (57.69%)
    ' Comment Lines: 53 (29.12%)
    '    - Xml Docs: 94.34%
    ' 
    '   Blank Lines: 24 (13.19%)
    '     File Size: 9.45 KB


    ' Module bifrost
    ' 
    '     Function: AsGff, GetGenes, GetProteins, metaeuk, prodigal
    '               scoreTable, training
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

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

''' <summary>
''' Bifrost: the gene prediction toolkit
''' </summary>
''' <remarks>
''' This R# package module provides the api for run gene prediction on the 
''' genomics contigs assembly sequence:
''' 
''' + ``prodigal``: the ab-initio prokaryotic gene prediction algorithm 
'''   (PROkaryotic DYnamic programming Gene-finding ALgorithm), works on the 
'''   prokaryotic MAGs contigs assembly sequence;
''' + ``metaeuk``: the homology based eukaryotic gene prediction algorithm, 
'''   works on the eukaryotic contigs assembly sequence with a given reference 
'''   protein database;
'''   
''' The gene prediction result of the prodigal algorithm is a collection of 
''' the ``PredictionResult`` object, which could be exported as:
''' 
''' + GFF3 table via the ``as.gff3`` api;
''' + nucleotide/protein fasta sequence via the ``as.genes``/``as.proteins`` api;
''' + a score table data frame via the ``as.data.frame`` api, for save as a csv 
'''   file by the ``write.csv`` api.
''' </remarks>
<Package("bifrost")>
<RTypeExport("prodigal", GetType(TrainingModel))>
<RTypeExport("metaeuk_config", GetType(MetaEukConfig))>
Module bifrost

    ''' <summary>
    ''' Register the internal data cast handler of this package module
    ''' </summary>
    ''' 
    ''' <remarks>
    ''' this function is invoked automatically at the start of the R# runtime 
    ''' environment, it just registers a data cast handler for makes the gene 
    ''' prediction result collection(<see cref="PredictionResult"/>) could be 
    ''' converted to a data frame via the ``as.data.frame`` api, so that the 
    ''' prediction score table can be saved as a csv file via the ``write.csv`` 
    ''' api.
    ''' </remarks>
    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(PredictionResult()), AddressOf scoreTable)
    End Sub

    ''' <summary>
    ''' overloads function for cast gene prediction result collection as dataframe for save to file by ``write.csv``. 
    ''' </summary>
    ''' <param name="result">
    ''' a collection of the gene prediction result, which is the output of the 
    ''' ``prodigal`` function in this package module.
    ''' </param>
    ''' <param name="args">
    ''' the additional arguments for the data frame cast, this parameter is not 
    ''' used in this function.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a data frame object that contains the gene prediction score table, each 
    ''' row in the generated data frame is a predicted gene, and the columns are 
    ''' the corresponding gene location and score details: ``seq_id``, 
    ''' ``gene_index``, ``start``, ``end``, ``strand``, ``frame``, 
    ''' ``start_codon``, ``stop_codon``, ``rbs_motif``, ``total_score``, 
    ''' ``coding_score``, ``start_score``, ``rbs_score``, ``type_score``, 
    ''' ``upstream_score``, ``rbs_spacing`` and ``partial_type``.
    ''' </returns>
    <RGenericOverloads("as.data.frame")>
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

    ''' <summary>
    ''' Train the gene prediction model in an unsupervised manner
    ''' </summary>
    ''' <param name="x">
    ''' input target fasta sequence collection for make prodigal training, it 
    ''' should be a set of the genomics contigs assembly sequence, which can be 
    ''' a <see cref="FastaFile"/> object, a collection of the 
    ''' <see cref="FastaSeq"/> object, or a file path of the fasta sequence file, 
    ''' or even a character vector of the raw sequence data.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a trained ``prodigal`` <see cref="TrainingModel"/> object, which can be 
    ''' used for the gene prediction of the other genomics contigs assembly 
    ''' sequence that come from the same or a close related species, via the 
    ''' ``model`` parameter of the ``prodigal`` function.
    ''' 
    ''' this function returns a R# error message object if the input sequence 
    ''' data is nothing or can not be cast to a fasta sequence collection.
    ''' </returns>
    ''' 
    ''' <example>
    ''' imports "bioseq.fasta" from "seqtoolkit";
    ''' imports "bifrost" from "seqtoolkit";
    ''' 
    ''' # train the gene prediction model from a well assembled 
    ''' # genomics contigs sequence, and then apply the trained 
    ''' # model on the other MAGs contigs of the same species
    ''' let model &lt;- prodigal_training(read.fasta("./genome.fasta"));
    ''' let genes &lt;- prodigal(read.fasta("./MAGs_contigs.fasta"), model = model);
    ''' </example>
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
    ''' <param name="x">
    ''' the target MAGs contigs assembly sequence for run the gene prediction, 
    ''' which can be a <see cref="FastaFile"/> object, a collection of the 
    ''' <see cref="FastaSeq"/> object, or a file path of the fasta sequence file, 
    ''' or even a character vector of the raw sequence data.
    ''' </param>
    ''' <param name="min_ORF_len">
    ''' the minimum ORF length in bp of the predicted gene, any of the candidate 
    ''' ORF that its length is less than this threshold value will be ignored in 
    ''' the gene prediction.
    ''' </param>
    ''' <param name="model">
    ''' the prodigal training model, which is the output of the 
    ''' ``prodigal_training`` function. If this parameter is nothing(the default 
    ''' value), then the model will be trained from the input contigs assembly 
    ''' sequence in an unsupervised manner automatically.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a collection of the gene prediction result: each element in the 
    ''' collection(<see cref="PredictionResult"/>) is the gene prediction result 
    ''' of the corresponding contigs sequence in the input fasta sequence data.
    ''' 
    ''' this function returns a R# error message object if the input sequence 
    ''' data is nothing or can not be cast to a fasta sequence collection.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' The prodigal gene prediction pipeline is implemented in an ab-initio 
    ''' manner: a training model will be learned from the input contigs assembly 
    ''' sequence at first(when the ``model`` parameter is not specified), and 
    ''' then the gene finding algorithm is running based on the dynamic programming 
    ''' score of the coding/non-coding hexamer and the RBS motif of the trained 
    ''' model.
    ''' </remarks>
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

    ''' <summary>
    ''' MetaEuk: the homology based eukaryotic gene prediction
    ''' </summary>
    ''' <param name="x">
    ''' a ``metaeuk_config`` object(<see cref="MetaEukConfig"/>) that carries 
    ''' all of the required data and parameters for run the metaeuk gene 
    ''' prediction: the contigs assembly fasta file path(``ContigsFile``), the 
    ''' reference protein fasta file path(``ReferenceFile``), the output file 
    ''' prefix(``OutputPrefix``) and the other algorithm parameters, example as 
    ''' the E-value threshold, the minimum identity, the maximum intron length, 
    ''' etc.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a collection of the gene prediction result: each element in the 
    ''' collection(<see cref="GenePrediction"/>) is a predicted gene that its 
    ''' exons are chained from the homology hits of the reference protein 
    ''' database.
    ''' 
    ''' this function returns a R# error message object if the input config 
    ''' object is nothing, or the required contigs/reference file is not 
    ''' specified in the config object.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' Unlike the prodigal gene prediction, which is running in an ab-initio 
    ''' manner, the metaeuk algorithm is running in a reference protein database 
    ''' dependent manner: at first the contigs assembly sequence is translated in 
    ''' six reading frames for generate the candidate coding fragments, and then 
    ''' the candidate fragments are aligned to the reference protein database for 
    ''' get the homology hits, at last the optimal exon set of each gene is 
    ''' picked out from the homology hits via dynamic programming.
    ''' 
    ''' NOTE: the input argument is evaluated as a fasta sequence collection at 
    ''' first in the current implementation, so that a ``metaeuk_config`` object 
    ''' input will be rejected by the sequence data check with the error message 
    ''' "there is no MAGs contigs assembly sequence input!", please run this 
    ''' metaeuk gene prediction program from the commandline at this moment.
    ''' </remarks>
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
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a <see cref="GFFTable"/> object that contains all of the predicted genes 
    ''' as the ``CDS`` feature, the score of each feature is the total score of 
    ''' the corresponding predicted gene, and the score details are stored in the 
    ''' attributes of the feature, example as ``start_codon``, ``rbs_motif``, 
    ''' ``cscore``, ``sscore``, ``rscore``, ``tscore``, ``uscore`` and 
    ''' ``partial``.
    ''' 
    ''' this function returns a R# error message object if the input data can not 
    ''' be cast to a collection of the <see cref="PredictionResult"/> object.
    ''' </returns>
    <ExportAPI("as.gff3")>
    <RApiReturn(GetType(GFFTable))>
    Public Function AsGff(<RRawVectorArgument()> x As Object, Optional env As Environment = Nothing) As Object
        Dim pull As PipeIterator(Of PredictionResult) = pipeline.Stream(Of PredictionResult)(x, env, suppress:=True)

        If pull.isError Then
            Return pull.getError
        End If

        Return ResultWriter.CastToGff(pull.ToArray)
    End Function

    ''' <summary>
    ''' Extract the protein sequences from the gene prediction result, and return as FASTA format. The sequence ID is in the format of "seqid_geneindex". For example, "contig1_5" means the 5th predicted gene on contig1. The sequence description is in the format of "start-end(strand)". For example, "100-900(+)" means the gene starts at position 100, ends at position 900, and is on the forward strand.
    ''' </summary>
    ''' <param name="x">
    ''' the gene prediction result, which can be the output of the "prodigal" 
    ''' function, or a pipeline that produces <see cref="PredictionResult"/> 
    ''' objects.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a collection of the <see cref="FastaSeq"/> protein sequence data, one 
    ''' sequence object for each of the predicted gene, the protein sequence is 
    ''' translated from the corresponding predicted gene nucleotide sequence, 
    ''' and the sequence title is formatted as: 
    ''' ``{seq_id}_{gene_index} {start-end(strand)} ID=gene_{gene_index};partial={partial_type}``.
    ''' 
    ''' this function returns a R# error message object if the input data can not 
    ''' be cast to a collection of the <see cref="PredictionResult"/> object.
    ''' </returns>
    <ExportAPI("as.proteins")>
    <RApiReturn(GetType(FastaSeq))>
    Public Function GetProteins(<RRawVectorArgument()> x As Object, Optional env As Environment = Nothing) As Object
        Dim pull As PipeIterator(Of PredictionResult) = pipeline.Stream(Of PredictionResult)(x, env, suppress:=True)

        If pull.isError Then
            Return pull.getError
        End If

        Return ResultWriter.GetProteinSequences(pull.ToArray).ToArray
    End Function

    ''' <summary>
    ''' Extract the gene sequences from the gene prediction result, and return as FASTA format. The sequence ID is in the format of "seqid_geneindex". For example, "contig1_5" means the 5th predicted gene on contig1. The sequence description is in the format of "start-end(strand)". For example, "100-900(+)" means the gene starts at position 100, ends at position 900, and is on the forward strand.
    ''' </summary>
    ''' <param name="x">
    ''' the gene prediction result, which can be the output of the "prodigal" 
    ''' function, or a pipeline that produces <see cref="PredictionResult"/> 
    ''' objects.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a collection of the <see cref="FastaSeq"/> nucleotide sequence data, one 
    ''' sequence object for each of the predicted gene, the sequence data is the 
    ''' nucleotide sequence of the corresponding predicted gene region on the 
    ''' contigs assembly sequence, and the sequence title is formatted as: 
    ''' ``{seq_id}_{gene_index} {start-end(strand)} ID=gene_{gene_index};partial={partial_type}``.
    ''' 
    ''' this function returns a R# error message object if the input data can not 
    ''' be cast to a collection of the <see cref="PredictionResult"/> object.
    ''' </returns>
    <ExportAPI("as.genes")>
    <RApiReturn(GetType(FastaSeq))>
    Public Function GetGenes(<RRawVectorArgument()> x As Object, Optional env As Environment = Nothing) As Object
        Dim pull As PipeIterator(Of PredictionResult) = pipeline.Stream(Of PredictionResult)(x, env, suppress:=True)

        If pull.isError Then
            Return pull.getError
        End If

        Return ResultWriter.GetGeneSequences(pull.ToArray).ToArray
    End Function

End Module

