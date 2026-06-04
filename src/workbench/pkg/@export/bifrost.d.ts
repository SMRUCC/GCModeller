// export R# package module type define for javascript/typescript language
//
//    imports "bifrost" from "seqtoolkit";
//
// ref=seqtoolkit.bifrost@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace bifrost {
   module as {
      module data {
         /**
         */
         function frame(result: object, args: object, env: object): any;
      }
      /**
       * Extract the gene sequences from the gene prediction result, and return as FASTA format. The sequence ID is in the format of "seqid_geneindex". For example, "contig1_5" means the 5th predicted gene on contig1. The sequence description is in the format of "start-end(strand)". For example, "100-900(+)" means the gene starts at position 100, ends at position 900, and is on the forward strand.
       * 
       * 
        * @param x -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function genes(x: any, env?: object): object;
      /**
       * cast the gene prediction result as GFF3 table format
       * 
       * 
        * @param x the gene prediction result, which can be the output of "prodigal" function, or a pipeline that produces PredictionResult objects. The pipeline can be created by using the "pipeline" function in R#, and the final output of the pipeline should be PredictionResult objects. For example, if you have a pipeline that produces PredictionResult objects, you can pass it directly to this function to get the GFF3 table format output.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function gff3(x: any, env?: object): object;
      /**
       * Extract the protein sequences from the gene prediction result, and return as FASTA format. The sequence ID is in the format of "seqid_geneindex". For example, "contig1_5" means the 5th predicted gene on contig1. The sequence description is in the format of "start-end(strand)". For example, "100-900(+)" means the gene starts at position 100, ends at position 900, and is on the forward strand.
       * 
       * 
        * @param x -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function proteins(x: any, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function metaeuk(x: any, env?: object): object;
   /**
    * Prodigal (PROkaryotic DYnamic programming Gene-finding ALgorithm)
    * 
    * 
     * @param x -
     * @param min_ORF_len -
     * 
     * + default value Is ``90``.
     * @param model -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function prodigal(x: any, min_ORF_len?: object, model?: object, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function prodigal_training(x: any, env?: object): object;
}
