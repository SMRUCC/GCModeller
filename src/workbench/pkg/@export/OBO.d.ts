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
       * removes all terms which is ``is_obsolete``
       * 
       * 
        * @param obo -
      */
      function is_obsolete(obo: object): object;
   }
   /**
    * make data filter of the ``property_value`` in a term
    * 
    * 
     * @param obo -
     * @param excludes the property name list for make excludes
   */
   function filter_properties(obo: object, excludes: string): object;
   /**
   */
   function lineage_term(term: object): object;
   /**
   */
   function obo_terms(obo: object): object;
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
   /**
    * set property_value list to a term or obo file headers
    * 
    * 
     * @param x -
     * @param append 
     * + default value Is ``true``.
     * @param property_value -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function set_propertyValue(x: any, append?: boolean, property_value?: object, env?: object): any;
   /**
    * set remakrs comment text into the header of the obo file object.
    * 
    * 
     * @param obo -
     * @param remarks -
     * @param append -
     * 
     * + default value Is ``true``.
   */
   function set_remarks(obo: object, remarks: string, append?: boolean): object;
   module write {
      /**
       * write ontology file as ascii plant text file
       * 
       * 
        * @param obo -
        * @param path -
        * @param excludes -
        * 
        * + default value Is ``null``.
        * @param strip_namespace_prefix 
        * + default value Is ``null``.
        * @param strip_property_unit 
        * + default value Is ``false``.
      */
      function obo(obo: object, path: string, excludes?: string, strip_namespace_prefix?: string, strip_property_unit?: boolean): boolean;
   }
}
