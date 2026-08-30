// export R# package module type define for javascript/typescript language
//
//    imports "bifrost" from "seqtoolkit";
//
// ref=seqtoolkit.bifrost@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Bifrost: the gene prediction toolkit
 * 
 * > This R# package module provides the api for run gene prediction on the 
 * >  genomics contigs assembly sequence:
 * >  
 * >  + ``prodigal``: the ab-initio prokaryotic gene prediction algorithm 
 * >    (PROkaryotic DYnamic programming Gene-finding ALgorithm), works on the 
 * >    prokaryotic MAGs contigs assembly sequence;
 * >  + ``metaeuk``: the homology based eukaryotic gene prediction algorithm, 
 * >    works on the eukaryotic contigs assembly sequence with a given reference 
 * >    protein database;
 * >    
 * >  The gene prediction result of the prodigal algorithm is a collection of 
 * >  the ``PredictionResult`` object, which could be exported as:
 * >  
 * >  + GFF3 table via the ``as.gff3`` api;
 * >  + nucleotide/protein fasta sequence via the ``as.genes``/``as.proteins`` api;
 * >  + a score table data frame via the ``as.data.frame`` api, for save as a csv 
 * >    file by the ``write.csv`` api.
*/
declare namespace bifrost {
   module as {
      /**
       * Extract the gene sequences from the gene prediction result, and return as FASTA format. The sequence ID is in the format of "seqid_geneindex". For example, "contig1_5" means the 5th predicted gene on contig1. The sequence description is in the format of "start-end(strand)". For example, "100-900(+)" means the gene starts at position 100, ends at position 900, and is on the forward strand.
       * 
       * 
        * @param x the gene prediction result, which can be the output of the "prodigal" 
        *  function, or a pipeline that produces @``T:SMRUCC.genomics.Annotation.Prodigal.PredictionResult`` 
        *  objects.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a collection of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` nucleotide sequence data, one 
        *  sequence object for each of the predicted gene, the sequence data is the 
        *  nucleotide sequence of the corresponding predicted gene region on the 
        *  contigs assembly sequence, and the sequence title is formatted as: 
        *  ``{seq_id}_{gene_index} {start-end(strand)} ID=gene_{gene_index};partial={partial_type}``.
        *  
        *  this function returns a R# error message object if the input data can not 
        *  be cast to a collection of the @``T:SMRUCC.genomics.Annotation.Prodigal.PredictionResult`` object.
      */
      function genes(x: any, env?: object): object;
      /**
       * cast the gene prediction result as GFF3 table format
       * 
       * 
        * @param x the gene prediction result, which can be the output of "prodigal" function, or a pipeline that produces PredictionResult objects. The pipeline can be created by using the "pipeline" function in R#, and the final output of the pipeline should be PredictionResult objects. For example, if you have a pipeline that produces PredictionResult objects, you can pass it directly to this function to get the GFF3 table format output.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a @``T:SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF.GFFTable`` object that contains all of the predicted genes 
        *  as the ``CDS`` feature, the score of each feature is the total score of 
        *  the corresponding predicted gene, and the score details are stored in the 
        *  attributes of the feature, example as ``start_codon``, ``rbs_motif``, 
        *  ``cscore``, ``sscore``, ``rscore``, ``tscore``, ``uscore`` and 
        *  ``partial``.
        *  
        *  this function returns a R# error message object if the input data can not 
        *  be cast to a collection of the @``T:SMRUCC.genomics.Annotation.Prodigal.PredictionResult`` object.
      */
      function gff3(x: any, env?: object): object;
      /**
       * Extract the protein sequences from the gene prediction result, and return as FASTA format. The sequence ID is in the format of "seqid_geneindex". For example, "contig1_5" means the 5th predicted gene on contig1. The sequence description is in the format of "start-end(strand)". For example, "100-900(+)" means the gene starts at position 100, ends at position 900, and is on the forward strand.
       * 
       * 
        * @param x the gene prediction result, which can be the output of the "prodigal" 
        *  function, or a pipeline that produces @``T:SMRUCC.genomics.Annotation.Prodigal.PredictionResult`` 
        *  objects.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a collection of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` protein sequence data, one 
        *  sequence object for each of the predicted gene, the protein sequence is 
        *  translated from the corresponding predicted gene nucleotide sequence, 
        *  and the sequence title is formatted as: 
        *  ``{seq_id}_{gene_index} {start-end(strand)} ID=gene_{gene_index};partial={partial_type}``.
        *  
        *  this function returns a R# error message object if the input data can not 
        *  be cast to a collection of the @``T:SMRUCC.genomics.Annotation.Prodigal.PredictionResult`` object.
      */
      function proteins(x: any, env?: object): object;
   }
   /**
    * MetaEuk: the homology based eukaryotic gene prediction
    * 
    * > Unlike the prodigal gene prediction, which is running in an ab-initio 
    * >  manner, the metaeuk algorithm is running in a reference protein database 
    * >  dependent manner: at first the contigs assembly sequence is translated in 
    * >  six reading frames for generate the candidate coding fragments, and then 
    * >  the candidate fragments are aligned to the reference protein database for 
    * >  get the homology hits, at last the optimal exon set of each gene is 
    * >  picked out from the homology hits via dynamic programming.
    * >  
    * >  NOTE: the input argument is evaluated as a fasta sequence collection at 
    * >  first in the current implementation, so that a ``metaeuk_config`` object 
    * >  input will be rejected by the sequence data check with the error message 
    * >  "there is no MAGs contigs assembly sequence input!", please run this 
    * >  metaeuk gene prediction program from the commandline at this moment.
    * 
     * @param x a ``metaeuk_config`` object(@``T:SMRUCC.genomics.Annotation.MetaEuk.MetaEukConfig``) that carries 
     *  all of the required data and parameters for run the metaeuk gene 
     *  prediction: the contigs assembly fasta file path(``ContigsFile``), the 
     *  reference protein fasta file path(``ReferenceFile``), the output file 
     *  prefix(``OutputPrefix``) and the other algorithm parameters, example as 
     *  the E-value threshold, the minimum identity, the maximum intron length, 
     *  etc.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a collection of the gene prediction result: each element in the 
     *  collection(@``T:SMRUCC.genomics.Annotation.MetaEuk.GenePrediction``) is a predicted gene that its 
     *  exons are chained from the homology hits of the reference protein 
     *  database.
     *  
     *  this function returns a R# error message object if the input config 
     *  object is nothing, or the required contigs/reference file is not 
     *  specified in the config object.
   */
   function metaeuk(x: any, env?: object): object;
   /**
    * Prodigal (PROkaryotic DYnamic programming Gene-finding ALgorithm)
    * 
    * > The prodigal gene prediction pipeline is implemented in an ab-initio 
    * >  manner: a training model will be learned from the input contigs assembly 
    * >  sequence at first(when the ``model`` parameter is not specified), and 
    * >  then the gene finding algorithm is running based on the dynamic programming 
    * >  score of the coding/non-coding hexamer and the RBS motif of the trained 
    * >  model.
    * 
     * @param x the target MAGs contigs assembly sequence for run the gene prediction, 
     *  which can be a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, a collection of the 
     *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object, or a file path of the fasta sequence file, 
     *  or even a character vector of the raw sequence data.
     * @param min_ORF_len the minimum ORF length in bp of the predicted gene, any of the candidate 
     *  ORF that its length is less than this threshold value will be ignored in 
     *  the gene prediction.
     * 
     * + default value Is ``90``.
     * @param model the prodigal training model, which is the output of the 
     *  ``prodigal_training`` function. If this parameter is nothing(the default 
     *  value), then the model will be trained from the input contigs assembly 
     *  sequence in an unsupervised manner automatically.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a collection of the gene prediction result: each element in the 
     *  collection(@``T:SMRUCC.genomics.Annotation.Prodigal.PredictionResult``) is the gene prediction result 
     *  of the corresponding contigs sequence in the input fasta sequence data.
     *  
     *  this function returns a R# error message object if the input sequence 
     *  data is nothing or can not be cast to a fasta sequence collection.
   */
   function prodigal(x: any, min_ORF_len?: object, model?: object, env?: object): object;
   /**
    * Train the gene prediction model in an unsupervised manner
    * 
    * 
     * @param x input target fasta sequence collection for make prodigal training, it 
     *  should be a set of the genomics contigs assembly sequence, which can be 
     *  a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, a collection of the 
     *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object, or a file path of the fasta sequence file, 
     *  or even a character vector of the raw sequence data.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a trained ``prodigal`` @``T:SMRUCC.genomics.Annotation.Prodigal.TrainingModel`` object, which can be 
     *  used for the gene prediction of the other genomics contigs assembly 
     *  sequence that come from the same or a close related species, via the 
     *  ``model`` parameter of the ``prodigal`` function.
     *  
     *  this function returns a R# error message object if the input sequence 
     *  data is nothing or can not be cast to a fasta sequence collection.
   */
   function prodigal_training(x: any, env?: object): object;
}
