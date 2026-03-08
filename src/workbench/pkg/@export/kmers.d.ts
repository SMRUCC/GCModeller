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
       * cast the genomics sequence as kmer based bloom filter model
       * 
       * 
        * @param genomics the genomics sequence
        * @param ncbi_taxid ncbi tax id of this sequence data
        * 
        * + default value Is ``0``.
        * @param k -
        * 
        * + default value Is ``35``.
        * @param fpr -
        * 
        * + default value Is ``0.001``.
        * @param spanSize 
        * + default value Is ``524288000``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function bloom_filter(genomics: any, ncbi_taxid?: object, k?: object, fpr?: number, spanSize?: object, env?: object): object;
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
    * 
    * 
     * @param repo -
     * @param ncbi_taxonomy -
     * @param min_supports min supports for LCA, recommended 0.35 as threshold
     * 
     * + default value Is ``0.35``.
     * @param coverage -
     * 
     * + default value Is ``0.5``.
     * @param env 
     * + default value Is ``null``.
   */
   function bloom_filters(repo: any, ncbi_taxonomy: object, min_supports?: number, coverage?: number, env?: object): object;
   /**
     * @param k default value Is ``35``.
     * @param env default value Is ``null``.
   */
   function bloom_vector(x: any, k?: object, env?: object): any;
   /**
     * @param k default value Is ``12``.
     * @param identities default value Is ``0.8``.
     * @param env default value Is ``null``.
   */
   function cdhit_nr(x: any, k?: object, identities?: number, env?: object): any;
   /**
    * filter the reads data that has the specific taxonomy id assignment.
    * 
    * 
     * @param kraken_output the kraken2 reads taxonomy assignment result
     * @param taxids a set of the target taxonomy id to make filter
     * @param ncbi_taxonomy 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function filter_classification(kraken_output: any, taxids: any, ncbi_taxonomy?: object, env?: object): object;
   /**
    * 
    * 
     * @param kraken_output quantification table which could be read from file via the function: ``parse_kraken_report``.
     * @param host_id -
     * @param coverage 
     * + default value Is ``0.999``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function filter_hostId(kraken_output: any, host_id: any, coverage?: number, env?: object): object;
   /**
    * usually be apply for host removal
    * 
    * 
     * @param kraken_output host reads information data
     * @param reads the raw reads fastq data
     * @param env -
     * 
     * + default value Is ``null``.
     * @return read result with host reads removals
   */
   function filter_reads(kraken_output: any, reads: any, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function hits_matrix(samples: object, env?: object): object;
   /**
    * Create kmers from a given sequence
    * 
    * 
     * @param seq -
     * @param k -
   */
   function kmers(seq: string, k: object): string;
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
    * extract the kraken2 quantify result data
    * 
    * 
     * @param kraken_output -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function kraken_data(kraken_output: any, env?: object): object;
   /**
    * extract gene/genomics sequences from genbank file for kraken2 sequence classification
    * 
    * 
     * @param gb -
     * @param geneset 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function kraken_seqs(gb: any, geneset?: boolean, env?: object): any;
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
    * make vector embedding
    * 
    * 
     * @param bloom -
     * @param x -
     * @param file 
     * + default value Is ``null``.
     * @param as_matrix 
     * + default value Is ``false``.
     * @param test 
     * + default value Is ``-1``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function make_vector(bloom: object, x: any, file?: any, as_matrix?: boolean, test?: object, env?: object): any;
   /**
     * @param type default value Is ``null``.
     * @param k default value Is ``6``.
     * @param env default value Is ``null``.
   */
   function onehot_vectorizer(x: any, type?: object, k?: object, env?: object): any;
   /**
   */
   function parse_kraken_output(filepath: any): object;
   /**
   */
   function parse_kraken_report(filepath: any): object;
   module read {
      /**
      */
      function kmers_background(dirpath: string): object;
      /**
      */
      function kraken2(file: string): object;
      /**
      */
      function kraken2_reads(file: string): object;
   }
   /**
   */
   function read_brackens(files: any): object;
   /**
   */
   function read_seqid(file: string): object;
   /**
     * @param env default value Is ``null``.
   */
   function seq_info(genbank: any, env?: object): object;
   /**
    * 
    * > make sequence embedding via TF-IDF algorithm which is implemented via @``T:SMRUCC.genomics.Model.MotifGraph.ProteinStructure.KmerTFIDFVectorizer``
    * 
     * @param x should be a collection of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` sequence collection
     * @param type the sequence data type, default is protein sequence
     * 
     * + default value Is ``null``.
     * @param k the length of the k-mers
     * 
     * + default value Is ``6``.
     * @param L2_norm do L2 normalized of the generated matrix data?
     * 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function tfidf_vectorizer(x: any, type?: object, k?: object, L2_norm?: boolean, env?: object): any;
   module write {
      /**
      */
      function kmers_background(bayes: object, dirpath: string): any;
   }
}
