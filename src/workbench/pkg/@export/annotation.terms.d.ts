declare namespace annotation.terms {
   /**
   */
   function geneNames(descriptions:any): any;
   module assign {
      /**
        * @param env default value is ``null``.
      */
      function KO(forward:object, reverse:object, env?:object): any;
      /**
        * @param env default value is ``null``.
      */
      function COG(alignment:any, env?:object): any;
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
      function id_maps(maps:object, file:string): any;
   }
   module read {
      /**
      */
      function MyvaCOG(file:string): any;
      /**
        * @param skip2ndMaps default value is ``false``.
      */
      function id_maps(file:string, skip2ndMaps?:boolean): any;
   }
   /**
     * @param excludeNull default value is ``false``.
   */
   function synonym(idlist:string, idmap:object, excludeNull?:boolean): any;
}
