// export R# package module type define for javascript/typescript language
//
//    imports "bioseq.fasta" from "seqtoolkit";
//
// ref=seqtoolkit.Fasta@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Fasta sequence toolkit
 * 
 * > This R# package module provides the toolkit for manipulate the biological 
 * >  sequence data in fasta format:
 * >  
 * >  + read the fasta sequence data from a file: ``read.fasta``, ``read.seq``, 
 * >    ``open.fasta``, ``parse.fasta``;
 * >  + save the fasta sequence data to a file: ``write.fasta``, ``open.fasta``;
 * >  + create the fasta sequence object or cast the other sequence data model to 
 * >    the fasta sequence data: ``fasta``, ``as.fasta``;
 * >  + the sequence data analysis tools: ``MSA.of``, ``translate``, ``mass``, 
 * >    ``seq_formula``, ``seq_vector``, ``cut_seq.linear``, etc.
 * >  
 * >  The fasta sequence data object in R# environment is a tuple list that its 
 * >  element type is @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq``, which can be cast to a data frame 
 * >  via the ``as.data.frame`` api, or be printed to the console with a pretty 
 * >  format via the registered console formatter.
*/
declare namespace bioseq.fasta {
   module as {
      /**
       * Create a fasta sequence collection object from any given sequence collection.
       * 
       * 
        * @param x any type of sequence collection, which can be:
        *  
        *  1. a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object or a collection of the 
        *     @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object;
        *  2. a multiple sequence alignment result(@``T:SMRUCC.genomics.Analysis.SequenceAlignment.MSA.MSAOutput``);
        *  3. a set of the @``T:SMRUCC.genomics.SequenceModel.NucleotideModels.SimpleSegment`` sequence segment object;
        *  4. a sequence motif object(@``T:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceMotif``);
        *  5. a ncbi genbank feature object(``Feature``) for extract the nucleotide 
        *     sequence data of the target feature site;
        *  6. a fastq sequence collection or a character vector of the raw sequence 
        *     data.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` sequence collection object that created from the 
        *  given sequence data source;
        *  
        *  this function returns a R# error message object if the input data source 
        *  can not be cast to a fasta sequence collection.
      */
      function fasta(x: any, env?: object): object;
   }
   /**
    * get alphabets represents of the fasta sequence
    * 
    * 
     * @param type the sequence data type.
     * 
     * + default value Is ``null``.
     * @return a character vector of the alphabet letters of the given molecule type: 
     *  the A/C/G/T/U/N letters for the DNA or RNA nucleotide sequence, or the 
     *  20 standard amino acid letters for the protein sequence.
     *  
     *  an error will be thrown if the given sequence type is not a valid 
     *  biological sequence type(DNA/RNA/Protein).
   */
   function chars(type?: object): string;
   module cut_seq {
      /**
       * cut part of the sequence
       * 
       * 
        * @param seq the target sequence data source, which can be a single 
        *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object, a collection of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` 
        *  object, or a character vector of the raw sequence data.
        * @param loci the location region data for make cut of the sequence site, data model could be:
        *  
        *  1. for nucleotide sequence, @``T:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation`` should be used,
        *  2. for general sequence data, @``T:SMRUCC.genomics.ComponentModel.Loci.Location`` should be used.
        * @param nt_auto_reverse make auto reverse of the nucleotide sequence if the given location is on 
        *  the @``F:SMRUCC.genomics.ComponentModel.Loci.Strands.Reverse`` direction.
        * 
        * + default value Is ``false``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a new @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object of the cut sequence fragment if the 
        *  input is a single sequence object, or a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object of 
        *  the cut sequence fragments of each input sequence if the input is a 
        *  sequence collection;
        *  
        *  this function returns a R# error message object if the given location 
        *  information is nothing, or the input sequence data can not be cast to a 
        *  fasta sequence collection.
      */
      function linear(seq: any, loci: any, nt_auto_reverse?: boolean, env?: object): any;
   }
   module fasta {
      /**
       * get/set the fasta headers title
       * 
       * > this api can be used as a property setter in R# environment: the headers 
       * >  data of the given fasta sequence object can be overwritten via the value 
       * >  assign syntax:
       * >  
       * >  ```r
       * >  fasta.headers(seq) <- c("seq_id", "description");
       * >  ```
       * 
        * @param fa a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` sequence object for get or set the headers title 
        *  data.
        * @param headers a character vector of the new fasta headers data for overwrite the headers 
        *  data of the given sequence object. If this parameter is not specified(or is 
        *  an empty vector), then the headers data of the given sequence object will 
        *  not be modified, and the current headers data will be returned.
        * 
        * + default value Is ``null``.
        * @return a character vector of the fasta headers title data of the given sequence 
        *  object.
      */
      function headers(fa: object, headers?: string): string;
      /**
       * get the fasta titles from a collection of fasta sequence
       * 
       * 
        * @param fa a fasta sequence collection, which can be a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, 
        *  a collection of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object, or a character vector of 
        *  the raw sequence data.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a character vector of the fasta title text of each sequence in the given 
        *  fasta sequence collection.
      */
      function titles(fa: any, env?: object): string;
   }
   /**
    * make sequence list index
    * 
    * > the index key of the generated list object is unique: the duplicated key 
    * >  will be renamed automatically by appending an unique numeric suffix.
    * 
     * @param x a fasta sequence collection for make the sequence index, which can be a 
     *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, a collection of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` 
     *  object, or a character vector of the raw sequence data.
     * @param ids a character vector of the index key of each sequence, the length of this 
     *  vector should be equals to the size of the input sequence collection. If 
     *  this parameter is not specified, then the first token of the fasta headers 
     *  title text of each sequence will be used as the index key.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a named list of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` sequence object, the name of the 
     *  list element is the corresponding index key of the sequence, so that we can 
     *  get the target sequence object by the index key directly.
     *  
     *  this function returns a R# error message object if the input sequence data 
     *  can not be cast to a fasta sequence collection.
   */
   function list_index(x: any, ids?: any, env?: object): object;
   /**
    * make the cluster tree of the given sequence fingerprint data
    * 
    * > the cluster tree is built based on the fingerprint similarity: the 
    * >  fingerprint data will be clustered into the same cluster when the 
    * >  similarity between them is greater than or equals to 0.8, and the 
    * >  fingerprints that their similarity is greater than 0.6 will be treated as 
    * >  the neighbours of each other.
    * 
     * @param fingerprints a collection of the @``T:SMRUCC.genomics.Model.OperonMapper.NTCluster`` sequence fingerprint data, which 
     *  can be the output of the ``read.fingerprint_bson`` api, or a pipeline object 
     *  that produces a set of the @``T:SMRUCC.genomics.Model.OperonMapper.NTCluster`` fingerprint data.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the @``T:SMRUCC.genomics.Model.OperonMapper.NTCluster`` fingerprint data that the ``cluster`` 
     *  property of the fingerprint object has been assigned with the cluster id of 
     *  the corresponding cluster: the fingerprints are grouped by the cluster id, 
     *  and the clusters are sorted by the cluster size in descending order;
     *  
     *  this function returns a R# error message object if the input data can not be 
     *  cast to a collection of the @``T:SMRUCC.genomics.Model.OperonMapper.NTCluster`` fingerprint data.
   */
   function make_clusterTree(fingerprints: any, env?: object): object;
   /**
    * evaluate the molecule mass of the given sequence
    * 
    * 
     * @param seqs a fasta sequence collection for evaluate the molecule mass, which can be 
     *  a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, a collection of the 
     *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object, or a character vector of the raw sequence 
     *  data.
     * @param type the molecule type of the input sequence data, if this parameter is not 
     *  specified(@``F:SMRUCC.genomics.SequenceModel.SeqTypes.Generic``), then the molecule type will be 
     *  evaluated from the input sequence data automatically: the most common 
     *  sequence type of the input sequence collection will be used.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a number value of the molecule mass if there is only one sequence in the 
     *  input sequence collection, or a named list of the molecule mass value of 
     *  each sequence if there are multiple sequence in the input sequence 
     *  collection, the name of the list element is the fasta title of the 
     *  corresponding sequence.
   */
   function mass(seqs: any, type?: object, env?: object): any;
   module MSA {
      /**
       * Do multiple sequence alignment
       * 
       * 
        * @param seqs A fasta sequence collection, which can be a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, 
        *  a collection of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object, or a character vector of 
        *  the raw sequence data.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return an @``T:SMRUCC.genomics.Analysis.SequenceAlignment.MSA.MSAOutput`` object that contains the multiple sequence 
        *  alignment result: the aligned sequence data of each input sequence and the 
        *  alignment cost value.
      */
      function of(seqs: any, env?: object): object;
   }
   module open {
      /**
       * open the fasta sequence file
       * 
       * 
        * @param file the file path of the target fasta sequence file for open.
        * @param read load a set of fasta sequence data in lazy mode? default is yes.
        * 
        * + default value Is ``true``.
        * @param line_break the sequence length in one line of the generated fasta document when this 
        *  function is used for open a fasta file in write mode, a negative value 
        *  means that all of the sequence data will be written in a single line.
        * 
        * + default value Is ``-1``.
        * @param delimiter the delimiter character for merge the fasta headers title when this 
        *  function is used for open a fasta file in write mode.
        * 
        * + default value Is ``'|'``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a lazy collection of the fasta sequence data(a pipeline object of the 
        *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` sequence data) when the ``read`` parameter is TRUE, 
        *  or a fasta stream writer(@``T:SMRUCC.genomics.SequenceModel.FASTA.StreamWriter``) 
        *  object for write the sequence data into the target file in a stream manner 
        *  when the ``read`` parameter is FALSE.
      */
      function fasta(file: string, read?: boolean, line_break?: object, delimiter?: string, env?: object): object|object;
      /**
       * open a fingerprint matrix writer for write the sequence fingerprint data 
       *  into a binary BSON file
       * 
       * 
        * @param file the output target: a file path of the generated fingerprint matrix file, or 
        *  a file stream object for write the fingerprint data.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a @``T:SMRUCC.genomics.Model.OperonMapper.FingerprintMatrixWriter`` object for write the sequence 
        *  fingerprint data into the target file in a stream manner, which can be used 
        *  by the ``write_fingerprint`` api;
        *  
        *  this function returns a R# error message object if the target file can not 
        *  be opened for write.
      */
      function fingerprint_writer(file: any, env?: object): object;
   }
   module parse {
      /**
       * parse the fasta sequence object from the given text data
       * 
       * 
        * @param x a character vector of the fasta sequence text data, each element in the 
        *  given character vector is one line of the fasta document text.
        * @return a vector of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` sequence object that parsed from 
        *  the given fasta document text data.
      */
      function fasta(x: any): object;
   }
   module read {
      /**
       * read a fasta sequence collection file
       * 
       * 
        * @param file the file path of the fasta sequence file for read the sequence data.
        * @param lazyStream read the fasta sequence data in a lazy stream mode? if this parameter is 
        *  TRUE, then a pipeline object of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` sequence data 
        *  will be returned, which is helpful for read a huge fasta sequence file 
        *  without loading all of the sequence data into the memory at once.
        * 
        * + default value Is ``false``.
        * @return A collection of the fasta sequence object: a vector of the 
        *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object that contains all of the sequence data in 
        *  the given fasta file, or a lazy pipeline object of the 
        *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` sequence data when the ``lazyStream`` parameter is 
        *  TRUE.
      */
      function fasta(file: string, lazyStream?: boolean): object;
      /**
       * read the sequence fingerprint data from a binary BSON format fingerprint 
       *  matrix file
       * 
       * 
        * @param file the file path of the fingerprint matrix file that is generated by the 
        *  ``write_fingerprint`` api, or a file stream object of the target 
        *  fingerprint matrix file.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a pipeline object of the @``T:SMRUCC.genomics.Model.OperonMapper.NTCluster`` sequence fingerprint data;
        *  
        *  this function returns a R# error message object if the given file can not be 
        *  opened for read.
      */
      function fingerprint_bson(file: any, env?: object): object;
      /**
       * Read a single fasta sequence file
       * 
       * > for input a genbank database file, this function will extract the origin sequence fasta object
       * 
        * @param file the file path of the target sequence file, Just contains one sequence
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object that read from the given sequence file;
        *  
        *  this function returns a R# error message object if the given file is not a 
        *  valid fasta sequence file or a genbank database file.
      */
      function seq(file: string, env?: object): object;
   }
   /**
    * read genome assembly fasta sequence file
    * 
    * > unlike the ``read.fasta`` api, this function reads the whole genome 
    * >  sequence in a chunked manner: the sequence data of each chromosome is 
    * >  stored as a @``T:SMRUCC.genomics.SequenceModel.NucleotideModels.ChunkedNtFasta`` object, so that we can slice a 
    * >  sequence region from a huge chromosome sequence in a memory efficient 
    * >  manner via the ``slicer`` api.
    * 
     * @param file the file path of the genome assembly fasta sequence file, or a file stream 
     *  object of the target sequence file.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a named list of the @``T:SMRUCC.genomics.SequenceModel.NucleotideModels.ChunkedNtFasta`` chunk sequence object, 
     *  the name of the list element is the fasta title of the corresponding 
     *  chromosome or contigs sequence.
     *  
     *  this function returns a R# error message object if the given file can not 
     *  be opened for read.
   */
   function read_assembly(file: any, env?: object): object;
   /**
    * read stockholm MSA file.
    * 
    * 
     * @param file the file path of the stockholm format multiple sequence alignment file.
     * @return a vector of the @``T:SMRUCC.genomics.Analysis.SequenceAlignment.MSA.Tabular.Stockholm`` alignment object that contains the 
     *  aligned sequence data of the target stockholm file.
   */
   function read_stockholm(file: string): object;
   /**
    * evaluate the chemical formula of the given sequence data
    * 
    * 
     * @param seqs a fasta sequence collection for evaluate the chemical formula, which can 
     *  be a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, a collection of the 
     *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object, or a character vector of the raw sequence 
     *  data.
     * @param type the molecule type of the input sequence data, if this parameter is not 
     *  specified(@``F:SMRUCC.genomics.SequenceModel.SeqTypes.Generic``), then the molecule type will be 
     *  evaluated from the input sequence data automatically: the most common 
     *  sequence type of the input sequence collection will be used.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a character value of the chemical formula if there is only one sequence 
     *  in the input sequence collection, or a named list of the chemical formula 
     *  of each sequence if there are multiple sequence in the input sequence 
     *  collection, the name of the list element is the fasta title of the 
     *  corresponding sequence.
   */
   function seq_formula(seqs: any, type?: object, env?: object): any;
   /**
    * Create algorithm for make sequence embedding
    * 
    * 
     * @param moltype the molecule type of the target sequence data for make the sequence 
     *  embedding: protein, DNA or RNA sequence.
     * 
     * + default value Is ``null``.
     * @param kappa the decay factor of the sequence graph transform algorithm, the smaller 
     *  value of this parameter makes the far distance k-mer composition weight 
     *  less.
     * 
     * + default value Is ``1``.
     * @param lengthsensitive is the generated embedding vector sensitive to the sequence length? if 
     *  this parameter is FALSE(the default value), then the embedding vector will 
     *  be normalized by the sequence length, so that two sequences with the same 
     *  k-mer composition but different lengths get the same embedding vector; if 
     *  this parameter is TRUE, then the vector norm value grows with the sequence 
     *  length.
     * 
     * + default value Is ``false``.
     * @return a @``T:SMRUCC.genomics.Model.MotifGraph.ProteinStructure.CreateMatrix`` algorithm object for embedding the given 
     *  sequence data as a numeric vector, which can be applied on a collection 
     *  of the sequence data via the ``seq_vector`` api.
   */
   function seq_sgt(moltype?: object, kappa?: number, lengthsensitive?: boolean): object;
   /**
    * embedding the given fasta sequence as vector
    * 
    * 
     * @param sgt the sequence graph transform algorithm object, which is created by the 
     *  ``seq_sgt`` api in this package module.
     * @param seqs a fasta sequence collection for make the sequence embedding, which can be 
     *  a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, a collection of the 
     *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object, or a character vector of the raw sequence 
     *  data.
     * @param as_dataframe when there are multiple sequence in the input sequence collection: cast 
     *  the embedding matrix as a data frame object? if this parameter is FALSE(the 
     *  default value), then a named list of the embedding vector will be returned 
     *  for each sequence.
     * 
     * + default value Is ``false``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a numeric vector of the embedding result if there is only one sequence in 
     *  the input sequence collection, or a data frame object(each row is the 
     *  embedding vector of one sequence, and the column names are ``v1``, ``v2``, 
     *  ...) when the ``as_dataframe`` parameter is TRUE, or a named list of the 
     *  embedding vector of each sequence.
   */
   function seq_vector(sgt: object, seqs: any, as_dataframe?: boolean, env?: object): number;
   /**
    * get the sequence length
    * 
    * 
     * @param fa a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` sequence object for measure the sequence length.
     * @return the sequence length in chars of the given fasta sequence data, ZERO will 
     *  be returned when the given sequence object is nothing.
   */
   function size(fa: object): object;
   /**
    * create a sequence region slicer for cut a specific sequence region from 
    *  the given sequence data
    * 
    * > the slicer object is used for cut a sequence region from a huge genome 
    * >  sequence in a memory efficient manner, which is very helpful for the 
    * >  sequence data extraction of a specific gene locus site.
    * 
     * @param fa the target sequence data source, which can be:
     *  
     *  1. a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` sequence object, then a 
     *     @``T:SMRUCC.genomics.SequenceModel.Slicer.FastaSlicer`` will be created;
     *  2. a chromosome or contigs sequence object(@``T:SMRUCC.genomics.SequenceModel.NucleotideModels.ChunkedNtFasta``) 
     *     that is read from the genome assembly sequence file via the 
     *     ``read_assembly`` api, then a @``T:SMRUCC.genomics.SequenceModel.Slicer.ChunkSlicer`` will be created;
     *  3. a ncbi genbank database file object(``GBFF.File``), then a 
     *     @``T:SMRUCC.genomics.SequenceModel.Slicer.GenBankSlicer`` will be created.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return an @``T:SMRUCC.genomics.SequenceModel.Slicer.ISlicer`` object for slice the sequence region from the 
     *  given sequence data source;
     *  
     *  this function returns a R# error message object if the given sequence data 
     *  source is not a supported sequence data model.
   */
   function slicer(fa: any, env?: object): object|object|object|object;
   /**
    * takes the sequence subset from the given sequence collection by a set of 
    *  the sequence id
    * 
    * 
     * @param x a fasta sequence collection for make the subset, which can be a 
     *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, a collection of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` 
     *  object, or a character vector of the raw sequence data.
     * @param gene_ids a character vector of the sequence id for takes the sequence subset, the 
     *  sequence id is the first token of the fasta headers title text, which is 
     *  splitted by the space, ``|``, ``(`` or the TAB character.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` sequence object that its sequence id 
     *  is in the given id set;
     *  
     *  this function returns a R# error message object if the input sequence data 
     *  can not be cast to a fasta sequence collection.
   */
   function takes(x: any, gene_ids: any, env?: object): object;
   /**
    * Do translation of the nt sequence to protein sequence
    * 
    * > when the ``bypassStop`` parameter is TRUE and there are some invalid gene 
    * >  sequence that contains the stop codon symbol in the translated protein 
    * >  sequence, a warning message will be pushed into the R# environment message 
    * >  buffer.
    * 
     * @param nt The given fasta collection, which can be a single @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` 
     *  object, a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, a collection of the 
     *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object, or a character vector of the raw nucleotide 
     *  sequence data.
     * @param table The genetic code for translation table.
     * 
     * + default value Is ``null``.
     * @param bypassStop Try ignores of the stop codon.
     * 
     * + default value Is ``true``.
     * @param checkNt check the input nucleotide sequence data is a valid nucleotide sequence? 
     *  if this parameter is TRUE and the input sequence data contains the invalid 
     *  nucleotide letters, then an error will be thrown.
     * 
     * + default value Is ``true``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a protein @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object if the input is a single nucleotide 
     *  sequence, or a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` protein sequence collection if the 
     *  input is a collection of the nucleotide sequence data;
     *  
     *  this function returns a R# error message object if the input sequence data 
     *  can not be cast to a nucleotide fasta sequence collection.
   */
   function translate(nt: any, table?: object, bypassStop?: boolean, checkNt?: boolean, env?: object): any;
   module write {
      /**
       * write a fasta sequence or a collection of fasta sequence object
       * 
       * 
        * @param seq the fasta sequence data for write into the target file, which can be a 
        *  single @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object, a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, a 
        *  collection of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object, a character vector of the 
        *  raw sequence data, a fastq sequence collection, or a pipeline object that 
        *  produces a set of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` sequence data.
        * @param file the output target: a file path of the generated fasta sequence file, a file 
        *  stream object, or a fasta stream writer object that is created by the 
        *  ``open.fasta`` api in write mode.
        * @param lineBreak The sequence length in one line, negative value or ZERo means no line break.
        * 
        * + default value Is ``-1``.
        * @param delimiter the delimiter character for merge the fasta headers title of the sequence 
        *  data.
        * 
        * + default value Is ``' '``.
        * @param filter_empty skip write sequence if the sequence object has no sequence data
        * 
        * + default value Is ``false``.
        * @param encoding The text encoding value of the generated fasta file.
        * 
        * + default value Is ``null``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a boolean value of the file save result: TRUE means the sequence data has 
        *  been written into the target file successfully;
        *  
        *  this function returns a R# error message object if the given sequence data 
        *  can not be cast to a fasta sequence collection, or the target file can not 
        *  be opened for write.
      */
      function fasta(seq: any, file: any, lineBreak?: object, delimiter?: string, filter_empty?: boolean, encoding?: object, env?: object): boolean;
   }
   /**
    * make the sequence fingerprint data of the given nucleotide sequence 
    *  collection, and then write the generated fingerprint data into the target 
    *  fingerprint matrix file
    * 
    * > the fasta headers title of the input sequence data should be formatted as: 
    * >  ``{gb_acc}.{locus_tag} {left} {right} {strand}|{biom_string}``, the 
    * >  ``strand`` token should be ``forward`` or ``reverse``, or the target 
    * >  sequence will be skipped with a warning message.
    * 
     * @param file a @``T:SMRUCC.genomics.Model.OperonMapper.FingerprintMatrixWriter`` object that is created by the 
     *  ``open.fingerprint_writer`` api.
     * @param seqs a nucleotide fasta sequence collection for make the sequence fingerprint 
     *  data, which can be a @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaFile`` object, a collection of the 
     *  @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` object, or a character vector of the raw sequence 
     *  data.
     * @param debug only make the fingerprint data of the first n sequence for debug test? a 
     *  negative value means that all of the input sequence will be processed.
     * 
     * + default value Is ``-1``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the input @``T:SMRUCC.genomics.Model.OperonMapper.FingerprintMatrixWriter`` object, so that this api can 
     *  be used in a pipeline manner;
     *  
     *  this function returns a R# error message object if the input sequence data 
     *  can not be cast to a fasta sequence collection.
   */
   function write_fingerprint(file: object, seqs: any, debug?: object, env?: object): object;
}
