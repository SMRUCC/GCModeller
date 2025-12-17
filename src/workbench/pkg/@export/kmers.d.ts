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
      /**
       * cast the genomics sequence as kmer based bloom filter model
       * 
       * 
        * @param genomics -
        * @param ncbi_taxid -
        * 
        * + default value Is ``0``.
        * @param k -
        * 
        * + default value Is ``35``.
        * @param fpr -
        * 
        * + default value Is ``1E-05``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function bloom_filter(genomics: any, ncbi_taxid?: object, k?: object, fpr?: number, env?: object): object;
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
     * @param min_supports default value Is ``0.5``.
     * @param coverage default value Is ``0.85``.
   */
   function bloom_filters(repo_dir: string, ncbi_taxonomy: object, min_supports?: number, coverage?: number): object;
   /**
     * @param env default value Is ``null``.
   */
   function filter_reads(kraken_output: any, reads: any, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function host_classification(kraken_output: any, host_taxids: any, env?: object): object;
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
     * @param n_threads 
     * + default value Is ``16``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function make_classify(db: any, reads: any, n_threads?: object, env?: object): object|object;
   /**
   */
   function parse_kraken_output(filepath: string): object;
   /**
   */
   function parse_kraken_report(filepath: string): object;
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
