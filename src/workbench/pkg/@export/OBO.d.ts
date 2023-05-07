// export R# package module type define for javascript/typescript language
//
// ref=annotationKit.OBO_DAG

/**
 * The Open Biological And Biomedical Ontology (OBO) Foundry
 * 
*/
declare namespace OBO {
   module read {
      /**
      */
      function obo(path:string): any;
   }
   module filter {
      /**
      */
      function is_obsolete(obo:object): any;
   }
   /**
   */
   function filter_properties(obo:object, excludes:string): any;
   module write {
      /**
        * @param excludes default value is ``null``.
      */
      function obo(obo:object, path:string, excludes?:string): any;
   }
}
