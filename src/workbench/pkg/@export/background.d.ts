// export R# package module type define for javascript/typescript language
//
// ref=gseakit.GSEABackground

/**
 * tools for handling GSEA background model.
 * 
*/
declare namespace background {
   /**
   */
   function clusterIDs(background:object): any;
   /**
   */
   function moleculeIDs(background:object): any;
   /**
   */
   function meta_background(enrich:object, graphQuery:object): any;
   module dag {
      /**
      */
      function background(dag:object): any;
   }
   module append {
      /**
        * @param env default value is ``null``.
      */
      function id_terms(background:object, term_name:string, terms:object, env?:object): any;
   }
   module background {
      /**
        * @param env default value is ``null``.
      */
      function id_mapping(background:object, mapping:object, env?:object): any;
   }
   module read {
      /**
      */
      function background(file:string): any;
   }
   module write {
      /**
      */
      function background(background:object, file:string): any;
   }
   /**
   */
   function background_summary(background:object): any;
   /**
   */
   function clusterInfo(background:object, clusterId:string): any;
   module geneSet {
      /**
        * @param env default value is ``null``.
      */
      function annotations(background:object, geneSet:any, env?:object): any;
      /**
        * @param isLocusTag default value is ``false``.
        * @param get_clusterID default value is ``false``.
        * @param env default value is ``null``.
      */
      function intersects(cluster:any, geneSet:string, isLocusTag?:boolean, get_clusterID?:boolean, env?:object): any;
   }
   module KO {
      /**
      */
      function table(background:object): any;
      /**
        * @param size default value is ``-1``.
        * @param genomeName default value is ``'unknown'``.
        * @param id_map default value is ``null``.
        * @param env default value is ``null``.
      */
      function background(genes:any, maps:any, size?:object, genomeName?:string, id_map?:any, env?:object): any;
   }
   module gsea {
      /**
        * @param desc default value is ``'n/a'``.
        * @param id default value is ``'xref'``.
        * @param name default value is ``'name'``.
      */
      function cluster(x:object, clusterId:string, clusterName:string, desc?:string, id?:string, name?:string): any;
   }
   /**
     * @param org_name default value is ``null``.
     * @param is_ko_ref default value is ``false``.
     * @param multipleOmics default value is ``false``.
     * @param env default value is ``null``.
   */
   function metpa(kegg:any, reactions:any, org_name?:string, is_ko_ref?:boolean, multipleOmics?:boolean, env?:object): any;
   module as {
      /**
        * @param background_size default value is ``-1``.
        * @param name default value is ``'n/a'``.
        * @param tax_id default value is ``'n/a'``.
        * @param desc default value is ``'n/a'``.
        * @param is_multipleOmics default value is ``false``.
        * @param filter_compoundId default value is ``true``.
        * @param kegg_code default value is ``null``.
        * @param env default value is ``null``.
      */
      function background(clusters:any, background_size?:object, name?:string, tax_id?:string, desc?:string, is_multipleOmics?:boolean, filter_compoundId?:boolean, kegg_code?:string, env?:object): any;
      /**
      */
      function geneSet(background:object): any;
   }
   /**
   */
   function KO_reference(): any;
   module metabolism {
      /**
        * @param filter default value is ``null``.
      */
      function background(kegg:object, filter?:string): any;
   }
   /**
   */
   function compoundBrite(): any;
}
