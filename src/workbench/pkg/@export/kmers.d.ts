// export R# package module type define for javascript/typescript language
//
//    imports "kmers" from "metagenomics_kit";
//    imports "kmers" from "seqtoolkit";
//
// ref=metagenomics_kit.KmersTool@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=seqtoolkit.kmersTools@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
 * 
*/
declare namespace kmers {
   module as {
      /**
        * @param normalized default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function abundance_matrix(samples: any, normalized?: boolean, env?: object): object;
   }
   /**
    * quantify of the metagenome community via kmers and bayes method
    * 
    * 
     * @param db -
     * @param bayes -
     * @param reads all reads data in one sample
     * @param rank 
     * + default value Is ``["genus","family","order","class","phylum","species"]``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function bayes_abundance(db: object, bayes: object, reads: any, rank?: any, env?: object): number;
   /**
     * @param rank default value Is ``["species","genus","family","order","class","phylum","superkingdom"]``.
     * @param env default value Is ``null``.
   */
   function bayes_background(kmers_db: any, ncbi_taxonomy: object, seq_id: object, rank?: any, env?: object): object;
   /**
   */
   function bayes_estimate(background: object, taxonomyDB: object, seq_ids: object): object;
   /**
    * generate sequence k-mer count data matrix
    * 
    * 
     * @param x -
     * @param k -
     * 
     * + default value Is ``3``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function kmers_matrix(x: any, k?: object, env?: object): object;
   /**
    * just make reads classify of the fastq reads based on the k-mer distribution
    * 
    * > apply this method for do host sequence filter
    * 
     * @param db -
     * @param reads -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function make_classify(db: object, reads: any, env?: object): object;
   module read {
      /**
      */
      function kmers_background(dirpath: string): object;
   }
   /**
   */
   function read_seqid(file: string): object;
   module write {
      /**
      */
      function kmers_background(bayes: object, dirpath: string): any;
   }
}
