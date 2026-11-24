// export R# package module type define for javascript/typescript language
//
//    imports "annotation.terms" from "seqtoolkit";
//
// ref=seqtoolkit.terms@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * tools for make ontology term annotation based on the proteins sequence data
 * 
*/
declare namespace annotation.terms {
   module assign {
      /**
       * assign the COG functional category for each query gene by keeping the 
       *  best identity COG hit
       * 
       * 
        * @param alignment a vector of the COG hits([MyvaCOG](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG.MyvaCOG)) alignment result data 
        *  for make the COG category assignment.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a vector of the [MyvaCOG](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG.MyvaCOG) COG assignment result, one best 
        *  hit element for each query gene;
        *  
        *  this function returns a R# error message object if the input alignment 
        *  data is not a collection of the [MyvaCOG](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG.MyvaCOG) object.
      */
      function COG(alignment: any, env?: object): any;
      /**
       * assign the gene ontology(GO) term annotation for the query gene
       * 
       * > this api is not implemented at this moment.
       * 
      */
      function GO(): any;
      /**
       * assign the pfam protein domain annotation for the query protein 
       *  sequence
       * 
       * > this api is not implemented at this moment.
       * 
      */
      function Pfam(): any;
   }
   /**
    * do KO number assign based on the bbh alignment result.
    * 
    * 
     * @param forward a forward blastn best hit([BestHit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit)) result data pipeline.
     * @param reverse a reverse blastn best hit([BestHit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit)) result data pipeline.
     * @param threshold 
     * + default value Is ``0.95``.
     * @param score_cutoff 
     * + default value Is ``60``.
     * @param kaas_rank 
     * + default value Is ``true``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [KOAssignmentCandidate](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.KOAssignmentCandidate) KO assignment result 
     *  when the ``kaas_rank`` parameter is TRUE, or a vector of the BHR result 
     *  when this parameter is FALSE;
     *  
     *  this function returns a R# error message object if the given forward or 
     *  reverse data stream is nothing or its element type is not the 
     *  [BestHit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit) object.
   */
   function assign_ko(forward: object, reverse: object, threshold?: number, score_cutoff?: number, kaas_rank?: boolean, env?: object): any;
   /**
    * assign the top term by score ranking
    * 
    * 
     * @param alignment the ncbi localblast alignment result, it can be the [BestHit](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit) array or 
     *  the [DiamondAnnotation](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.DiamondAnnotation) array.
     * @param term_maps a R# list object of the term id mapping data: the name of each list 
     *  element is the term name, and the element value is a character vector 
     *  of the hit name that belongs to the corresponding term.
     * 
     * + default value Is ``null``.
     * @param top_best 
     * + default value Is ``true``.
     * @param direct_term_maps 
     * + default value Is ``false``.
     * @param filter_unknown 
     * + default value Is ``false``.
     * @param identities_cut 
     * + default value Is ``0``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [RankTerm](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.RankTerm) term annotation result;
     *  
     *  this function returns a R# error message object if the input alignment 
     *  data can not be cast to a best hit or diamond annotation collection.
   */
   function assign_terms(alignment: any, term_maps?: object, top_best?: boolean, direct_term_maps?: boolean, filter_unknown?: boolean, identities_cut?: number, env?: object): object;
   /**
    * try parse gene names from the product description strings
    * 
    * 
     * @param descriptions the gene functional product description strings.
     * @return a vector of the parsed gene name string from each of the given product 
     *  description text.
   */
   function geneNames(descriptions: any): object;
   /**
    * extract the metabolic function term annotation from the diamond m8 
    *  format alignment result data
    * 
    * 
     * @param m8 a collection of the diamond m8 tabular format alignment hits 
     *  ([DiamondAnnotation](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.DiamondAnnotation)) for extract the metabolic function 
     *  term annotation.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a lazy pipeline collection of the [RankTerm](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.RankTerm) metabolic 
     *  function term annotation result;
     *  
     *  this function returns a R# error message object if the input m8 hits 
     *  data can not be cast to a collection of the 
     *  [DiamondAnnotation](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.DiamondAnnotation) object.
   */
   function m8_metabolic_terms(m8: any, env?: object): object;
   /**
    * make the genomics metabolic vector data from the given term annotation 
    *  result collection
    * 
    * 
     * @param terms a collection of the [RankTerm](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.RankTerm) term annotation data for 
     *  make the genomics metabolic vectors.
     * @param stream returns the generated vector data in a lazy pipeline manner? if this 
     *  parameter is FALSE(the default value), then a vector array will be 
     *  returned instead.
     * 
     * + default value Is ``false``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [GenomeVector](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.GenomeVector) genomics metabolic vector 
     *  object, or a lazy pipeline collection of this vector data when the 
     *  ``stream`` parameter is TRUE;
     *  
     *  this function returns a R# error message object if the input term 
     *  annotation data can not be cast to a collection of the 
     *  [RankTerm](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.RankTerm) object.
   */
   function make_vectors(terms: any, stream?: boolean, env?: object): object;
   /**
    * create the rank term annotation objects from the given parallel data 
    *  vectors
    * 
    * 
     * @param id a character vector of the query gene id of the term annotation.
     * @param term a character vector of the term name of the term annotation.
     * @param score a numeric vector of the term annotation score value.
     * @param source a character vector of the term data source name.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [RankTerm](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.RankTerm) term annotation object, each 
     *  element is created from the element-wise combination of the given data 
     *  vectors.
   */
   function rank_term(id: any, term: any, score: any, source: any, env?: object): object;
   module read {
      /**
       * read the id mappings text data as the secondary id mapping solver 
       *  object
       * 
       * 
        * @param file the file path of the id mappings text data file.
        * @param skip2ndMaps set this parameter value to ``true`` for fixed for build the ``kegg2go`` mapping model.
        * 
        * + default value Is ``false``.
        * @return a [SecondaryIDSolver](cref:T:SMRUCC.genomics.ComponentModel.DBLinkBuilder.SecondaryIDSolver) id mapping solver object, which can 
        *  be used for make the gene id synonym query via the ``synonym`` api, or 
        *  be saved as a text file via the ``write.id_maps`` api.
      */
      function id_maps(file: string, skip2ndMaps?: boolean): object;
      /**
       * read the myva COG hits annotation data from a csv table file
       * 
       * 
        * @param file the file path of the csv format myva COG hits annotation table file.
        * @return a vector of the [MyvaCOG](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG.MyvaCOG) COG hits object that loaded from 
        *  the given csv table file.
      */
      function MyvaCOG(file: string): object;
   }
   /**
    * read the given table file as rank term object
    * 
    * 
     * @param file the file path of the csv format rank term annotation table file.
     * @return a vector of the [RankTerm](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.RankTerm) term annotation object that 
     *  loaded from the given csv table file.
   */
   function read_rankterms(file: string): object;
   /**
    * read VFDB fasta sequence database
    * 
    * 
     * @param file the file path of the VFDB virulence factor fasta sequence database 
     *  file, the fasta headers title of this file should be splitted by the 
     *  ``|`` character.
     * @return a vector of the [VFs](cref:T:SMRUCC.genomics.Analysis.Metagenome.MetaFunction.VFDB.VFs) virulence factor object that parsed 
     *  from the given VFDB fasta sequence database file.
   */
   function read_vfdb_seqs(file: string): object;
   /**
    * Removes the numeric suffix (usually representing an exon index) from protein identifiers.
    * 
    * 
     * @param id A vector of protein identifier strings.
     * @param make_unique 
     * + default value Is ``true``.
     * @return An array of unique protein identifiers with the numeric suffix removed.
   */
   function removes_proteinIDSuffix(id: any, make_unique?: boolean): string;
   /**
    * query the id synonyms of the given id list from the id mapping solver 
    *  object
    * 
    * 
     * @param idlist a character vector of the query id for get its synonyms.
     * @param idmap a [SecondaryIDSolver](cref:T:SMRUCC.genomics.ComponentModel.DBLinkBuilder.SecondaryIDSolver) id mapping solver object, which is 
     *  created by the ``read.id_maps`` api.
     * @param excludeNull remove the query id which has no synonym data from the result?
     * 
     * + default value Is ``false``.
     * @return a vector of the [Synonym](cref:T:SMRUCC.genomics.ComponentModel.DBLinkBuilder.Synonym) id synonym query result object.
   */
   function synonym(idlist: string, idmap: object, excludeNull?: boolean): object;
   /**
    * make the term annotation table of each gene from a given set of the 
    *  term annotation result group
    * 
    * 
     * @param annotations a R# list object of the term annotation result group: the name of each 
     *  list element is the term name, and the element value is a collection 
     *  of the [RankTerm](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.RankTerm) term annotation data that belongs to the 
     *  corresponding term.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the [EntityObject](cref:T:Microsoft.VisualBasic.Data.Framework.IO.EntityObject) term annotation table 
     *  record, one record element for each gene, and each record contains the 
     *  term annotation value of the gene in each term group;
     *  
     *  this function returns a R# error message object if any of the term 
     *  annotation group data can not be cast to a collection of the 
     *  [RankTerm](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.RankTerm) object.
   */
   function term_table(annotations: object, env?: object): object;
   /**
    * make embedding of the genomics metabolic model
    * 
    * 
     * @param annotations a collection of the [GenomeVector](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.GenomeVector) genomics metabolic 
     *  vector data for make the tf-idf embedding.
     * @param L2_norm do L2 normalized of the generated embedding matrix data?
     * 
     * + default value Is ``false``.
     * @param union_contigs the minimum number of the contigs in one taxonomy group for merge the 
     *  genome vectors.
     * 
     * + default value Is ``1000``.
     * @param hierarchical build the hierarchical taxonomy embedding model?
     * 
     * + default value Is ``false``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a data frame object of the genomics metabolic model embedding result: 
     *  each row is one genome(the row name is the taxonomy name), and each 
     *  column is a metabolic function term, the cell value is the tf-idf 
     *  weight of the corresponding term in the corresponding genome;
     *  
     *  this function returns a R# error message object if the input vector 
     *  data can not be cast to a collection of the 
     *  [GenomeVector](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.GenomeVector) object.
   */
   function tfidf_vectorizer(annotations: any, L2_norm?: boolean, union_contigs?: object, hierarchical?: boolean, env?: object): any;
   module write {
      /**
       * save the id mapping solver data into a text file
       * 
       * 
        * @param maps a [SecondaryIDSolver](cref:T:SMRUCC.genomics.ComponentModel.DBLinkBuilder.SecondaryIDSolver) id mapping solver object for save.
        * @param file the file path of the generated id mapping data text file.
        * @return a boolean value of the file save result: TRUE means the id mapping 
        *  data has been written into the target file successfully.
      */
      function id_maps(maps: object, file: string): boolean;
   }
   /**
    * write the genomics metabolic vector data into a jsonl text file
    * 
    * 
     * @param genomes a collection of the [GenomeVector](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.GenomeVector) genomics metabolic 
     *  vector data for write.
     * @param file the output target: a file path of the generated jsonl text file, or a 
     *  file stream object for write the jsonl text data.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a boolean value of the file save result: TRUE means the genomics 
     *  metabolic vector data has been written into the target file 
     *  successfully;
     *  
     *  this function returns a R# error message object if the input vector 
     *  data can not be cast to a collection of the 
     *  [GenomeVector](cref:T:SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.GenomeVector) object, or the target file can not be 
     *  opened for write.
   */
   function write_genomes_jsonl(genomes: any, file: any, env?: object): any;
   /**
    * save the VFDB virulence factor sequence data as a simple fasta format 
    *  sequence file
    * 
    * 
     * @param vfdb a vector of the [VFs](cref:T:SMRUCC.genomics.Analysis.Metagenome.MetaFunction.VFDB.VFs) virulence factor object for save as 
     *  the fasta sequence file.
     * @param file the file path of the generated fasta sequence file.
     * @return a boolean value of the file save result: TRUE means the virulence 
     *  factor sequence data has been written into the target file 
     *  successfully.
   */
   function write_simple_vfdb(vfdb: object, file: string): boolean;
}
