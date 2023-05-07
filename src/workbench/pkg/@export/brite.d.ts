declare namespace brite {
   module brite {
      module as {
         /**
           * @param entryId_pattern default value is ``'[a-z]+\d+'``.
           * @param env default value is ``null``.
         */
         function table(htext:any, entryId_pattern?:string, env?:object): any;
      }
      /**
        * @param env default value is ``null``.
      */
      function parseJSON(file:string, env?:object): any;
   }
   /**
     * @param env default value is ``null``.
   */
   function parse(file:string, env?:object): any;
   module KO {
      /**
      */
      function geneNames(): any;
   }
   /**
     * @param level default value is ``'class'``.
   */
   function briteMaps(htext:object, geneId:string, level?:string): any;
}
