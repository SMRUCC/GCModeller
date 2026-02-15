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
        * @param env default value Is ``null``.
      */
      function COG(alignment: any, env?: object): any;
      /**
      */
      function GO(): any;
      /**
      */
      function Pfam(): any;
   }
   /**
    * do KO number assign based on the bbh alignment result.
    * 
    * 
     * @param forward -
     * @param reverse -
     * @param threshold 
     * + default value Is ``0.95``.
     * @param score_cutoff 
     * + default value Is ``60``.
     * @param kaas_rank 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function assign_ko(forward: object, reverse: object, threshold?: number, score_cutoff?: number, kaas_rank?: boolean, env?: object): any;
   /**
    * assign the top term by score ranking
    * 
    * 
     * @param alignment the ncbi localblast alignment result, it can be the @``T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit`` array or 
     *  the @``T:SMRUCC.genomics.Interops.NCBI.Extensions.DiamondAnnotation`` array.
     * @param term_maps -
     * 
     * + default value Is ``null``.
     * @param top_best 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function assign_terms(alignment: any, term_maps?: object, top_best?: boolean, env?: object): object;
   /**
    * try parse gene names from the product description strings
    * 
    * 
     * @param descriptions the gene functional product description strings.
   */
   function geneNames(descriptions: any): object;
   /**
     * @param env default value Is ``null``.
   */
   function m8_metabolic_terms(m8: any, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function make_vectors(terms: any, env?: object): object;
   module read {
      /**
       * 
       * 
        * @param file -
        * @param skip2ndMaps set this parameter value to ``true`` for fixed for build the ``kegg2go`` mapping model.
        * 
        * + default value Is ``false``.
      */
      function id_maps(file: string, skip2ndMaps?: boolean): object;
      /**
      */
      function MyvaCOG(file: string): object;
   }
   /**
    * read the given table file as rank term object
    * 
    * 
     * @param file -
   */
   function read_rankterms(file: string): object;
   /**
    * read VFDB fasta sequence database
    * 
    * 
     * @param file -
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
     * @param excludeNull default value Is ``false``.
   */
   function synonym(idlist: string, idmap: object, excludeNull?: boolean): object;
   /**
     * @param env default value Is ``null``.
   */
   function term_table(annotations: object, env?: object): object;
   /**
    * make embedding of the genomics metabolic model
    * 
    * 
     * @param annotations -
     * @param L2_norm -
     * 
     * + default value Is ``false``.
     * @param union_contigs 
     * + default value Is ``1000``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function tfidf_vectorizer(annotations: any, L2_norm?: boolean, union_contigs?: object, env?: object): any;
   module write {
      /**
      */
      function id_maps(maps: object, file: string): boolean;
   }
   /**
   */
   function write_simple_vfdb(vfdb: object, file: string): boolean;
}
