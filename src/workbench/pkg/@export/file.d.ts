// export R# package module type define for javascript/typescript language
//
//    imports "file" from "gokit";
//
// ref=gokit.file@gokit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace file {
   /**
   */
   function DAG(goDb: object): object;
   /**
    * parse the term is_a relationship
    * 
    * 
     * @param env 
     * + default value Is ``null``.
   */
   function is_a(x: any, env?: object): object;
   module read {
      /**
       * read the GO database file
       * 
       * 
        * @param goDb -
      */
      function go_obo(goDb: string): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function synonym(x: any, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function term_xrefs(x: any, env?: object): object;
}
