// export R# package module type define for javascript/typescript language
//
//    imports "background" from "gseakit";
//
// ref=gseakit.GSEABackground@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace background {
   module append {
      /**
        * @param env default value Is ``null``.
      */
      function id_terms(background: object, term_name: string, terms: object, env?: object): any;
   }
   module as {
      /**
        * @param background_size default value Is ``-1``.
        * @param name default value Is ``'n/a'``.
        * @param tax_id default value Is ``'n/a'``.
        * @param desc default value Is ``'n/a'``.
        * @param omics default value Is ``null``.
        * @param filter_compoundId default value Is ``true``.
        * @param kegg_code default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function background(clusters: any, background_size?: object, name?: string, tax_id?: string, desc?: string, omics?: object, filter_compoundId?: boolean, kegg_code?: string, env?: object): object;
      /**
        * @param export_alias default value Is ``false``.
      */
      function geneSet(background: object, export_alias?: boolean): object;
   }
   module background {
      /**
        * @param subset default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function id_mapping(background: object, mapping: object, subset?: string, env?: object): object;
   }
   /**
     * @param gene_names default value Is ``true``.
   */
   function background_summary(background: object, gene_names?: boolean): object;
   /**
     * @param env default value Is ``null``.
   */
   function cluster_filter(background: object, id_removes: any, env?: object): any;
   /**
   */
   function cluster_names(background: object, idset: any): string;
   /**
   */
   function clusterIDs(background: object): string;
   /**
   */
   function clusterInfo(background: object, clusterId: string): object;
   /**
   */
   function compoundBrite(): object;
   /**
   */
   function cut_background(background: object, annotated: any): object;
   module dag {
      /**
        * @param flat default value Is ``false``.
        * @param verbose_progress default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function background(dag: object, flat?: boolean, verbose_progress?: boolean, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function fromList(geneSet: object, env?: object): any;
   module geneSet {
      /**
        * @param env default value Is ``null``.
      */
      function annotations(background: object, geneSet: any, env?: object): any;
      /**
        * @param geneSet default value Is ``null``.
        * @param min_size default value Is ``3``.
        * @param max_intersects default value Is ``500``.
        * @param remove_clusters default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function filter(background: object, geneSet?: any, min_size?: object, max_intersects?: object, remove_clusters?: any, env?: object): object;
      /**
        * @param isLocusTag default value Is ``false``.
        * @param get_clusterID default value Is ``false``.
        * @param term_map default value Is ``false``.
        * @param id_map default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function intersects(cluster: any, geneSet: string, isLocusTag?: boolean, get_clusterID?: boolean, term_map?: boolean, id_map?: boolean, env?: object): string;
   }
   /**
     * @param desc default value Is ``'n/a'``.
     * @param id default value Is ``'xref'``.
     * @param name default value Is ``'name'``.
   */
   function gsea_cluster(x: object, clusterId: string, clusterName: string, desc?: string, id?: string, name?: string): object;
   module KO {
      /**
        * @param size default value Is ``-1``.
        * @param genomeName default value Is ``'unknown'``.
        * @param id_map default value Is ``null``.
        * @param multiple_omics default value Is ``false``.
        * @param term_db default value Is ``'unknown'``.
        * @param instance_map default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function background(genes: any, maps: any, size?: object, genomeName?: string, id_map?: any, multiple_omics?: boolean, term_db?: string, instance_map?: object, env?: object): object;
      /**
      */
      function table(background: object): object;
   }
   /**
   */
   function KO_reference(): object;
   /**
   */
   function meta_background(enrich: object, graphQuery: object): object;
   module metabolism {
      /**
        * @param filter default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function background(kegg: any, filter?: string, env?: object): object;
   }
   /**
     * @param org_name default value Is ``null``.
     * @param is_ko_ref default value Is ``false``.
     * @param multipleOmics default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function metpa(kegg: any, reactions: any, org_name?: string, is_ko_ref?: boolean, multipleOmics?: boolean, env?: object): object;
   /**
   */
   function moleculeIDs(background: object): string;
   module read {
      /**
      */
      function background(file: string): object;
   }
   module write {
      /**
      */
      function background(background: object, file: string): boolean;
   }
}
