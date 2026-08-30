// export R# package module type define for javascript/typescript language
//
//    imports "kmers" from "metagenomics_kit";
//    imports "kmers" from "seqtoolkit";
//
// ref=metagenomics_kit.KmersTool@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=seqtoolkit.kmersTools@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
 * The sequence k-mer tools
 * 
 * > This R# package module provides the toolkit for make the k-mer based 
 * >  sequence data analysis:
 * >  
 * >  + ``kmers``: generate the k-mer sequence fragments from a given sequence 
 * >    data in a sliding window manner;
 * >  + ``kmers_matrix``: generate the k-mer count matrix of a given sequence 
 * >    collection;
 * >  + ``tfidf_vectorizer`` and ``onehot_vectorizer``: make the sequence 
 * >    embedding via the bag-of-k-mers model, the TF-IDF weight or the one-hot 
 * >    encoding vector;
 * >  + ``cdhit_nr`` and ``cdhit_clusters``: run the CD-HIT like sequence 
 * >    clustering for get the non-redundant sequence set or the cluster 
 * >    family table.
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
     * @param tool_name default value Is ``'Tool_New'``.
     * @param env default value Is ``null``.
   */
   function benchmark(reference: any, test: any, baseline: object, groups: any, tool_name?: string, env?: object): object;
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
     * @param hash_index default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function bloom_vector(x: any, k?: object, hash_index?: boolean, env?: object): any;
   /**
    * run the CD-HIT like sequence clustering and then export the cluster 
    *  result as a set of the cluster tables
    * 
    * 
     * @param x a collection of the sequence data for run the clustering, which can be a 
     *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, a vector of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` 
     *  object, or a character vector of the raw sequence data.
     * @param k the k-mer size for build the min-hash sketch of the sequence data: 
     *  
     *  + protein - k=5aa
     *  + nucleotide - k=12nt
     *  + genomics - k=31nt
     * 
     * + default value Is ``12``.
     * @param identities the sequence identity threshold of the cluster members: the sequences 
     *  that their identity is greater than or equals to this threshold value will 
     *  be clustered into the same cluster.
     * 
     * + default value Is ``0.8``.
     * @param n_threads the thread number for run the min-hash task in parallel.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a tuple list that contains the data slots:
     *  
     *  - family: a vector of the @``T:SMRUCC.genomics.Model.MotifGraph.ProteinStructure.FamilyExports`` object, each 
     *    element is the summary data of one cluster: the ``family_id``, the 
     *    ``members`` cluster size, and the ``representative``/``rep_seq`` data of 
     *    the representative sequence;
     *  - sequence: a vector of the @``T:SMRUCC.genomics.Model.MotifGraph.ProteinStructure.SequenceCluster`` object, each 
     *    element is the data of one cluster member: the ``seq_title``, the 
     *    ``family_id``, the ``score`` identity to the cluster representative and 
     *    the ``seq`` sequence data;
     *  - clusters: a vector of the @``T:SMRUCC.genomics.Analysis.SequenceAlignment.SimilarHit`` object, which is the 
     *    raw cluster result of the CD-HIT like clustering: the ``SeqID`` is the 
     *    representative sequence of the cluster and the ``Similar`` property is 
     *    the identity score of each cluster member to the representative 
     *    sequence.
     *  
     *  this function returns NULL if the input data can not be cast to a fasta 
     *  sequence collection.
   */
   function cdhit_clusters(x: any, k?: object, identities?: number, n_threads?: object, env?: object): any;
   /**
    * run the CD-HIT like sequence clustering for get the non-redundant 
    *  sequence set
    * 
    * > the input sequence data will be sorted by the sequence length in 
    * >  descending order at first, and then the greedy clustering algorithm runs 
    * >  based on the min-hash similarity of the k-mer sketch of each sequence.
    * 
     * @param x a collection of the sequence data for run the clustering, which can be a 
     *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, a vector of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` 
     *  object, or a character vector of the raw sequence data.
     * @param k the k-mer size for build the min-hash sketch of the sequence data: 
     *  
     *  + protein - k=5aa
     *  + nucleotide - k=12nt
     *  + genomics - k=31nt
     * 
     * + default value Is ``12``.
     * @param identities the sequence identity threshold of the cluster members: the sequences 
     *  that their identity is greater than or equals to this threshold value will 
     *  be clustered into the same cluster.
     * 
     * + default value Is ``0.8``.
     * @param n_threads the thread number for run the min-hash task in parallel.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` sequence object: the 
     *  representative sequence of each cluster. For a cluster that contains 
     *  multiple sequence members, the fasta headers of the representative 
     *  sequence is formatted as: the representative sequence title, 
     *  ``{cluster_size} cluster members`` and the json text of the cluster member 
     *  sequence id list; and the sequence data of a singleton cluster(the unique 
     *  sequence) is returned as is.
     *  
     *  this function returns NULL if the input data can not be cast to a fasta 
     *  sequence collection.
   */
   function cdhit_nr(x: any, k?: object, identities?: number, n_threads?: object, env?: object): object;
   /**
    * filter the reads data that has the specific taxonomy id assignment.
    * 
    * 
     * @param kraken_output the kraken2 reads taxonomy assignment result
     * @param taxids a set of the target taxonomy id to make filter
     * @param ncbi_taxonomy 
     * + default value Is ``null``.
     * @param strict 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function filter_classification(kraken_output: any, taxids: any, ncbi_taxonomy?: object, strict?: boolean, env?: object): object;
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
     * @param seq the raw sequence data text.
     * @param k the length of the k-mer sequence fragment.
     * @return a character vector of the k-mer sequence fragments, which is generated 
     *  from the given sequence data via a sliding window of size ``k``, and 
     *  the step size of the sliding window is just one char, so that all of the 
     *  generated k-mer fragments are overlapped with each other.
     *  
     *  an empty character vector will be returned if the value of the ``k`` 
     *  parameter is greater than the length of the input sequence data.
   */
   function kmers(seq: string, k: object): string;
   /**
    * generate sequence k-mer count data matrix
    * 
    * 
     * @param x a collection of the sequence data, which can be a fasta sequence 
     *  collection(@``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq``, @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile``), a fastq 
     *  sequence collection(@``T:SMRUCC.genomics.SequenceModel.FQ.FastQFile``) or any other 
     *  @``T:SMRUCC.genomics.SequenceModel.FASTA.IFastaProvider`` sequence data model, or a pipeline object 
     *  that produces a set of the sequence data.
     * @param k the length of the k-mer sequence fragment for make the count.
     * 
     * + default value Is ``3``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a @``T:SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix`` k-mer count matrix object: each row in this 
     *  matrix is a sequence in the input sequence collection(the row name is 
     *  the sequence title), and each column is a k-mer feature(the ``sampleID`` 
     *  property of the generated matrix is the k-mer alphabet sorted in 
     *  ascending order), the cell value is the count of the corresponding k-mer 
     *  in the corresponding sequence(ZERO means that the k-mer is not exists in 
     *  the target sequence).
     *  
     *  this function returns a R# error message object if the input data can not 
     *  be cast to a collection of the sequence data.
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
     * @param filter_unclassfied default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function MAG_classify(mag: any, MAG_id: string, tax_tree: object, filter_unclassfied?: boolean, env?: object): any;
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
     * @param env default value Is ``null``.
   */
   function make_seq_groups(kraken_output: any, env?: object): any;
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
     * @param parallel 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function make_vector(bloom: object, x: any, file?: any, as_matrix?: boolean, test?: object, parallel?: boolean, env?: object): any;
   /**
    * make the sequence embedding via the one-hot encoding(Bag-of-n-grams) of 
    *  the k-mer composition
    * 
    * > unlike the ``tfidf_vectorizer`` api, which evaluates the weight of each 
    * >  k-mer term by the term frequency and the inverse document frequency, this 
    * >  api just encodes the k-mer composition of the sequence data as a binary 
    * >  vector, i.e. the presence or absence of each k-mer term.
    * 
     * @param x should be a collection of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` sequence 
     *  collection, which can be a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, a vector of 
     *  the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object, or a character vector of the raw 
     *  sequence data.
     * @param type the sequence data type, default is protein sequence. If the sequence type 
     *  is not protein, then the input sequence data will be canonicalized as the 
     *  standard nucleotide letters at first.
     * 
     * + default value Is ``null``.
     * @param k the length of the k-mers
     * 
     * + default value Is ``6``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a data frame object: each row is a sequence in the input sequence 
     *  collection(the row name is the fasta title of the corresponding 
     *  sequence), and each column is a k-mer term, the cell value is ONE when the 
     *  k-mer is exists in the corresponding sequence, otherwise ZERO.
     *  
     *  this function returns NULL if the input data can not be cast to a fasta 
     *  sequence collection.
   */
   function onehot_vectorizer(x: any, type?: object, k?: object, env?: object): any;
   /**
    * Parse the reads annotation result generated from the kraken2
    * 
    * 
     * @param filepath a character vector of the file path of the reads annotation files.
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
       * read reads annotation result generated from the kraken2
       * 
       * 
        * @param file the csv table file path
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
     * @param env default value Is ``null``.
   */
   function taxonomy_expression(id: any, expr: any, taxdata: any, env?: object): object;
   /**
    * make the sequence embedding via the TF-IDF weight of the bag-of-k-mers 
    *  model
    * 
    * > make sequence embedding via TF-IDF algorithm which is implemented via @``T:SMRUCC.genomics.Model.MotifGraph.ProteinStructure.KmerTFIDFVectorizer``
    * >  
    * >  the generated embedding vector of each sequence will be normalized to an 
    * >  unit vector when the ``L2_norm`` parameter is TRUE, which is helpful for 
    * >  the cosine similarity or euclidean distance measurement between the 
    * >  embedding vectors of the different length sequences.
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
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a data frame object: each row is a sequence in the input sequence 
     *  collection(the row name is the fasta title of the corresponding 
     *  sequence), and each column is a k-mer term, the cell value is the TF-IDF 
     *  weight of the corresponding k-mer in the corresponding sequence.
     *  
     *  this function returns NULL if the input data can not be cast to a fasta 
     *  sequence collection.
   */
   function tfidf_vectorizer(x: any, type?: object, k?: object, L2_norm?: boolean, env?: object): any;
   module write {
      /**
      */
      function kmers_background(bayes: object, dirpath: string): any;
   }
}
