// export R# package module type define for javascript/typescript language
//
//    imports "uniprot" from "seqtoolkit";
//
// ref=seqtoolkit.uniprotTools@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The Universal Protein Resource (UniProt)
 * 
*/
declare namespace uniprot {
   /**
    * get the protein full name and function description text from the given 
    *  uniprot protein entry object
    * 
    * 
     * @param prot the uniprot protein [entry](cref:T:SMRUCC.genomics.Assembly.Uniprot.XML.entry) object for extract its 
     *  description text.
     * @return a character vector of the protein description text: the protein full 
     *  name and the function description text.
   */
   function get_description(prot: object): string;
   /**
    * get the protein domain region annotation data from the given uniprot 
    *  protein entry object
    * 
    * 
     * @param prot the uniprot protein [entry](cref:T:SMRUCC.genomics.Assembly.Uniprot.XML.entry) object for extract its domain 
     *  region annotation data.
     * @return a vector of the [DomainModel](cref:T:SMRUCC.genomics.ProteinModel.DomainModel) protein domain region 
     *  annotation object.
   */
   function get_domain(prot: object): object;
   /**
    * get keyword dataframe about the given protein data
    * 
    * 
     * @param prot the uniprot protein [entry](cref:T:SMRUCC.genomics.Assembly.Uniprot.XML.entry) object for extract its keyword 
     *  annotation data.
     * @return a dataframe object that with two data fields: `id` - the keyword id and `keyword` - the keyword name.
   */
   function get_keywords(prot: object): object;
   /**
    * get related pathway names of current protein
    * 
    * 
     * @param prot the uniprot protein [entry](cref:T:SMRUCC.genomics.Assembly.Uniprot.XML.entry) object for extract its related 
     *  pathway names.
     * @return a character vector of the pathway name that related to the given 
     *  protein.
   */
   function get_pathways(prot: object): string;
   /**
    * get the catalytic activity reaction data of the given uniprot protein 
    *  entry object
    * 
    * 
     * @param prot the uniprot protein [entry](cref:T:SMRUCC.genomics.Assembly.Uniprot.XML.entry) object for extract its 
     *  catalytic activity reaction data.
     * @return a named list of the reaction data: the name of each list element is 
     *  the reaction text, and the element value is a list that contains the 
     *  ``equation``, ``ec_number`` and ``metabolites`` data slots.
   */
   function get_reactions(prot: object): any;
   /**
    * extract the protein sequence data from the given uniprot protein entry 
    *  object
    * 
    * 
     * @param prot the uniprot protein [entry](cref:T:SMRUCC.genomics.Assembly.Uniprot.XML.entry) object for extract its 
     *  sequence data.
     * @return a [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) protein sequence object.
   */
   function get_sequence(prot: object): object;
   /**
    * get subcellular location of current protein
    * 
    * 
     * @param prot the uniprot protein [entry](cref:T:SMRUCC.genomics.Assembly.Uniprot.XML.entry) object for extract its 
     *  subcellular location annotation data.
     * @return a data frame object of the subcellular location annotation data, the 
     *  columns are ``location`` and ``topology``.
   */
   function get_subcellularlocation(prot: object): any;
   /**
    * get external database reference id set
    * 
    * > the uniprot database name will be named as: ``UniProtKB/Swiss-Prot`` for
    * >  make unify with the genebank feature xrefs.
    * 
     * @param prot target protein object to extract its database corss reference id
     * @param dbname this function will returns a character vector of the db_xrefs for specific database if this db name is specificed.
     * 
     * + default value Is ``null``.
     * @return a named list of the external database cross reference id set when the 
     *  ``dbname`` parameter is not specified, or a character vector of the 
     *  cross reference id set of the specific database.
   */
   function get_xrefs(prot: object, dbname?: string): object|string;
   /**
    * id unify mapping
    * 
    * 
     * @param uniprot a uniprot dataabse pipeline stream
     * @param id a character vector of the raw id for map to the target database id.
     * @param target the database name for map to
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a character vector of the target database id of each given raw id;
     *  
     *  this function returns a R# error message object if the input uniprot 
     *  data can not be cast to a collection of the [entry](cref:T:SMRUCC.genomics.Assembly.Uniprot.XML.entry) 
     *  object.
   */
   function id_unify(uniprot: any, id: any, target?: string, env?: object): any;
   /**
    * build the metabolite set collection from the catalytic activity 
    *  reaction data of the given uniprot protein entries
    * 
    * 
     * @param uniprot a collection of the uniprot protein [entry](cref:T:SMRUCC.genomics.Assembly.Uniprot.XML.entry) data for build 
     *  the metabolite set collection.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a named list of the metabolite id set: the name of each list element 
     *  is the uniprot accession id of the protein, and the element value is a 
     *  character vector of the ChEBI metabolite id;
     *  
     *  this function returns a R# error message object if the input uniprot 
     *  data can not be cast to a collection of the [entry](cref:T:SMRUCC.genomics.Assembly.Uniprot.XML.entry) 
     *  object.
   */
   function metaboliteSet(uniprot: any, env?: object): any;
   module open {
      /**
       * open a uniprot database file
       * 
       * 
        * @param files a character vector of the uniprot xml format database document file 
        *  paths for load the protein entries.
        * @param isUniParc the target uniprot database document is the UniParc database?
        * 
        * + default value Is ``false``.
        * @param ignoreError ignore the parse error message of the invalid uniprot xml entry data?
        * 
        * + default value Is ``true``.
        * @param tqdm show the progress bar of the entries parsing progress?
        * 
        * + default value Is ``true``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return this function returns a pipeline stream of the uniprot protein entries.
      */
      function uniprot(files: any, isUniParc?: boolean, ignoreError?: boolean, tqdm?: boolean, env?: object): object;
   }
   /**
    * Parse the uniprot fasta header text
    * 
    * 
     * @param x a character vector of the uniprot fasta headers title text, or a 
     *  collection of the [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) sequence object.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [FastaHeader](cref:T:SMRUCC.genomics.Assembly.Uniprot.FastaHeader) uniprot fasta headers object.
   */
   function parseHeader(x: any, env?: object): object;
   /**
    * parse the uniprot xml document text data as the protein entry object 
    *  collection
    * 
    * 
     * @param xml the uniprot xml format database document text data.
     * @return a vector of the uniprot protein [entry](cref:T:SMRUCC.genomics.Assembly.Uniprot.XML.entry) object that parsed 
     *  from the given xml document text data.
   */
   function parseUniProt(xml: string): object;
   module protein {
      /**
       * populate all protein fasta sequence from the given uniprot database reader
       * 
       * 
        * @param uniprot a collection of the uniprot protein [entry](cref:T:SMRUCC.genomics.Assembly.Uniprot.XML.entry) data.
        * @param extractAll populate the sequence with all uniprot accession id
        * 
        * + default value Is ``false``.
        * @param title the fasta title header template, data keys for template could be:
        *  
        *  1. uniprot_id
        *  2. fullname
        *  3. name
        *  4. ncbi_taxid
        *  5. organism
        *  6. ec_number
        *  7. go_id
        *  8. gene_name
        *  9. ORF
        *  10. subcellular_location
        *  11. db_xrefs...
        * 
        * + default value Is ``'<uniprot_id> <fullname>'``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a collection of the [FastaSeq](cref:T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq) that export from the given protein set.
        *  
        *  the generated fasta sequence header title in format: ``uniprot_id|db_xref|protein function``.
        *  the db_xref is optional if the parameter "db_xref" is not be omited.
      */
      function seqs(uniprot: any, extractAll?: boolean, title?: string, env?: object): object;
   }
   /**
    * export protein annotation data as data frame.
    * 
    * 
     * @param uniprot a collection of the uniprot protein [entry](cref:T:SMRUCC.genomics.Assembly.Uniprot.XML.entry) data for make 
     *  the annotation table.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a data frame object of the protein annotation data;
     *  
     *  this function returns a R# error message object if the input uniprot 
     *  data can not be cast to a collection of the [entry](cref:T:SMRUCC.genomics.Assembly.Uniprot.XML.entry) 
     *  object.
   */
   function proteinTable(uniprot: any, env?: object): any;
   module read {
      /**
       * read uniprot protein export output tsv file
       * 
       * 
        * @param file the input source: a file path of the uniprot protein export tsv table 
        *  file, or a file stream object of the target tsv table file.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a vector of the [uniprotTools.proteinTable()](cref:M:seqtoolkit.uniprotTools.proteinTable(System.Object,SMRUCC.Rsharp.Runtime.Environment)) protein annotation record 
        *  object that loaded from the given tsv table file;
        *  
        *  this function returns a R# error message object if the given file can 
        *  not be opened for read.
      */
      function proteinTable(file: any, env?: object): object;
   }
}
