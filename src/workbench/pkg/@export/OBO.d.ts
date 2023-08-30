// export R# package module type define for javascript/typescript language
//
//    imports "OBO" from "annotationKit";
//
// ref=annotationKit.OBO_DAG@annotationKit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The Open Biological And Biomedical Ontology (OBO) Foundry
 * 
*/
declare namespace OBO {
   module filter {
      /**
      */
      function is_obsolete(obo: object): object;
   }
   /**
   */
   function filter_properties(obo: object, excludes: string): object;
   /**
   */
   function lineage_term(term: object): object;
   /**
   */
   function ontologyLeafs(tree: object): object;
   /**
   */
   function ontologyNodes(tree: object): object;
   /**
   */
   function ontologyTree(obo: object): object;
   module open {
      /**
       * open the ontology obo file reader
       * 
       * > This obo file reader object could be used as the data source for read 
       * >  other database
       * 
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function obo(file: any, env?: object): object;
   }
   module read {
      /**
       * parse the gene ontology obo file
       * 
       * 
        * @param path -
      */
      function obo(path: string): object;
   }
   module write {
      /**
        * @param excludes default value Is ``null``.
      */
      function obo(obo: object, path: string, excludes?: string): boolean;
   }
}
