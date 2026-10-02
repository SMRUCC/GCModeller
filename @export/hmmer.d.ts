// export R# package module type define for javascript/typescript language
//
//    imports "hmmer" from "seqtoolkit";
//
// ref=seqtoolkit.hmmer@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * HMMER profile hidden markov model search tools
 * 
 * > This R# package module provides the api for run the HMMER3 profile HMM 
 * >  search based protein function annotation:
 * >  
 * >  + ``load_interprodb``: load the InterPro database term entries;
 * >  + ``parse_hmmer_model``: parse the HMMER3 profile HMM model text data;
 * >  + ``load_hmmer``: load a collection of the HMMER3 profile HMM model files;
 * >  + ``hmmer_search``: run the HMMER profile HMM search for protein function 
 * >    annotation;
 * >  + ``parse_kofamscan``: parse the kofamscan annotation table output.
*/
declare namespace hmmer {
   /**
    * run the HMMER profile HMM search for protein function annotation
    * 
    * 
     * @param hmmer a [ProteinAnnotator](cref:T:SMRUCC.genomics.Analysis.SequenceTools.HMMER.ProteinAnnotator) object that contains the loaded HMMER3 
     *  profile models, which is created by the ``load_hmmer`` api.
     * @param x a protein fasta sequence collection for run the HMMER search, which can 
     *  be a [FastaFile](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile) object, 
     *  a collection of the 
     *  [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) object, or a 
     *  character vector of the raw sequence data.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a lazy pipeline collection of the [AnnotationResult](cref:T:SMRUCC.genomics.Analysis.SequenceTools.HMMER.AnnotationResult) domain 
     *  annotation result, one result element for each of the profile HMM hit 
     *  that found in the input protein sequence collection;
     *  
     *  this function returns NULL if the input sequence data can not be cast to 
     *  a fasta sequence collection.
   */
   function hmmer_search(hmmer: object, x: any, env?: object): object;
   /**
    * load a collection of the HMMER3 profile HMM model files for protein 
    *  function annotation
    * 
    * 
     * @param x a character vector of the HMMER3 profile model file paths(``*.hmm``) 
     *  for load into the protein annotator. this parameter also can be a 
     *  directory path that contains a set of the HMMER3 profile model files, 
     *  then all of the profile model files inside the given directory will be 
     *  loaded.
     *  
     *  this parameter also can be a file path of the hmmer model package 
     *  archive file(``*.zip``) which is created by the ``save_hmmer`` api.
     * @return a [ProteinAnnotator](cref:T:SMRUCC.genomics.Analysis.SequenceTools.HMMER.ProteinAnnotator) object that contains the loaded HMMER3 
     *  profile models, which can be used for run the protein function 
     *  annotation via the ``hmmer_search`` api;
     *  
     *  this function returns NULL if the given input is an empty character 
     *  vector.
   */
   function load_hmmer(x: any): object;
   /**
    * load the InterPro database term entries from a given interpro database 
    *  xml document file
    * 
    * 
     * @param file the file path of the InterPro database document file(``interpro.xml``) 
     *  for load the term entries.
     * @return a lazy pipeline collection of the [Interpro](cref:T:SMRUCC.genomics.Analysis.SequenceTools.HMMER.InterPro.Xml.Interpro) term entry 
     *  object that loaded from the given InterPro database document file.
   */
   function load_interprodb(file: string): object;
   /**
    * parse the HMMER3 profile HMM model text data
    * 
    * 
     * @param x the HMMER3 profile HMM model text data, or a file path of the HMMER3 
     *  profile model document(``*.hmm``) for parse.
     * @return a [ProfileHMM](cref:T:SMRUCC.genomics.Analysis.SequenceTools.HMMER.ProfileHMM) profile hidden markov model object that 
     *  parsed from the given HMMER3 profile model text data.
   */
   function parse_hmmer_model(x: string): object;
   /**
    * Parse the kofamscan table output
    * 
    * 
     * @param file the input source: a file path of the kofamscan annotation table output 
     *  file, or a file stream object of the target table file.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a lazy pipeline collection of the [KOFamScan](cref:T:SMRUCC.genomics.Analysis.SequenceTools.HMMER.KOFamScan) KEGG orthology 
     *  annotation result record object;
     *  
     *  this function returns a R# error message object if the given file can 
     *  not be opened for read.
   */
   function parse_kofamscan(file: any, env?: object): object;
   /**
    * save the hmmer profile HMM model collection as a model package 
    *  archive file(``*.zip``)
    * 
    * 
     * @param hmmer a [ProteinAnnotator](cref:T:SMRUCC.genomics.Analysis.SequenceTools.HMMER.ProteinAnnotator) object that contains the loaded HMMER3 
     *  profile models.
     * @param file the file path of the target model package archive file(``*.zip``) for 
     *  save the model data. model data will be saved in binary format inside 
     *  the zip archive for get the maximum of the io performance.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the file path of the created model package archive file.
   */
   function save_hmmer(hmmer: object, file: string, env?: object): any;
}
