// export R# package module type define for javascript/typescript language
//
// ref=seqtoolkit.terms@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace annotation.terms {
   /**
    * try parse gene names from the product description strings
    * 
    * 
     * @param descriptions the gene functional product description strings.
   */
   function geneNames(descriptions: any): object;
   module assign {
      /**
       * do KO number assign based on the bbh alignment result.
       * 
       * 
        * @param forward -
        * @param reverse -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function KO(forward: object, reverse: object, env?: object): object;
      /**
        * @param env default value Is ``null``.
      */
      function COG(alignment: any, env?: object): any;
      /**
      */
      function Pfam(): any;
      /**
      */
      function GO(): any;
   }
   module write {
      /**
      */
      function id_maps(maps: object, file: string): boolean;
   }
   module read {
      /**
      */
      function MyvaCOG(file: string): object;
      /**
       * 
       * 
        * @param file -
        * @param skip2ndMaps set this parameter value to ``true`` for fixed for build the ``kegg2go`` mapping model.
        * 
        * + default value Is ``false``.
      */
      function id_maps(file: string, skip2ndMaps?: boolean): object;
   }
   /**
     * @param excludeNull default value Is ``false``.
   */
   function synonym(idlist: string, idmap: object, excludeNull?: boolean): object;
}
