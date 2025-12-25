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
     * @param alignment -
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
   module write {
      /**
      */
      function id_maps(maps: object, file: string): boolean;
   }
}
