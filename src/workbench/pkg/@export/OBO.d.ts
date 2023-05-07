// export R# package module type define for javascript/typescript language
//
// ref=annotationKit.OBO_DAG@annotationKit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The Open Biological And Biomedical Ontology (OBO) Foundry
 * 
*/
declare namespace OBO {
   module read {
      /**
       * parse the obo file
       * 
       * 
        * @param path -
      */
      function obo(path:string): object;
   }
   module filter {
      /**
      */
      function is_obsolete(obo:object): object;
   }
   /**
   */
   function filter_properties(obo:object, excludes:string): object;
   module write {
      /**
        * @param excludes default value Is ``null``.
      */
      function obo(obo:object, path:string, excludes?:string): boolean;
   }
}
