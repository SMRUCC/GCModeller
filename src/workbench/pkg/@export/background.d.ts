// export R# package module type define for javascript/typescript language
//
//    imports "background" from "gseakit"
//
// ref=gseakit.GSEABackground@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * tools for handling GSEA background model.
 * 
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
       * cast the cluster data as the enrichment background
       * 
       * 
        * @param clusters a data cluster or a collection of kegg pathway model
        * @param background_size default value -1 or zero means auto evaluated
        * 
        * + default value Is ``-1``.
        * @param name the background model name
        * 
        * + default value Is ``'n/a'``.
        * @param tax_id ncbi taxonomy id of the target organism
        * 
        * + default value Is ``'n/a'``.
        * @param desc the model description
        * 
        * + default value Is ``'n/a'``.
        * @param is_multipleOmics Create a enrichment background model for run multiple omics data analysis?
        *  this parameter is only works for the kegg pathway model where you are 
        *  speicifc via the **`clusters`** parameter.
        * 
        * + default value Is ``false``.
        * @param filter_compoundId do compound id filtering when target model is **`is_multipleOmics`**?
        *  (all of the KEGG drug id and KEGG glycan id will be removed from the cluster model)
        * 
        * + default value Is ``true``.
        * @param kegg_code the kegg organism code when the given **`clusters`** collection is
        *  a collection of the pathway object.
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function background(clusters: any, background_size?: object, name?: string, tax_id?: string, desc?: string, is_multipleOmics?: boolean, filter_compoundId?: boolean, kegg_code?: string, env?: object): object;
      /**
      */
      function geneSet(background: object): object;
   }
   module background {
      /**
       * do id mapping of the members in the background cluster
       * 
       * 
        * @param background -
        * @param mapping do id translation via this id source list
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function id_mapping(background: object, mapping: object, env?: object): any;
   }
   /**
   */
   function background_summary(background: object): object;
   /**
    * get all of the cluster id set from the given background model object
    * 
    * 
     * @param background -
   */
   function clusterIDs(background: object): string;
   /**
    * get cluster info data table
    * 
    * 
     * @param background -
     * @param clusterId -
   */
   function clusterInfo(background: object, clusterId: string): object;
   /**
    * get kegg compound class brite background model
    * 
    * > this function generates a background model from the internal
    * >  kegg resource database, the generated background model which 
    * >  could be used for the compound class category annotation.
    * 
   */
   function compoundBrite(): object;
   module dag {
      /**
       * create gsea background from a given obo ontology file data.
       * 
       * 
        * @param dag -
      */
      function background(dag: object): object;
   }
   module geneSet {
      /**
       * make gene set annotation via a given gsea background model
       * 
       * 
        * @param background -
        * @param geneSet -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function annotations(background: object, geneSet: any, env?: object): any;
      /**
       * get an intersection id list between the background
       *  model and the given gene id list.
       * 
       * 
        * @param cluster A gene cluster model or gsea background model object.
        * @param geneSet -
        * @param isLocusTag -
        * 
        * + default value Is ``false``.
        * @param get_clusterID this function will returns a set of mapping cluster id if this 
        *  parameter value is set to value TRUE, otherwise this function
        *  returns a set of intersected geneID list by default.
        *  
        *  this parameter only works when the cluster object is a 
        *  gsea background model object.
        * 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
      */
      function intersects(cluster: any, geneSet: string, isLocusTag?: boolean, get_clusterID?: boolean, env?: object): string;
   }
   module gsea {
      /**
       * Create a cluster for gsea background
       * 
       * 
        * @param x id, name data fields should be exists in current dataframe object, 
        *  other data fields will be used as the gene member terms
        * @param clusterId id of the cluster
        * @param clusterName display name of the cluster model
        * @param desc the description of the cluster model
        * 
        * + default value Is ``'n/a'``.
        * @param id the field column name for get gene members id
        * 
        * + default value Is ``'xref'``.
        * @param name the field column name for get gene members name
        * 
        * + default value Is ``'name'``.
      */
      function cluster(x: object, clusterId: string, clusterName: string, desc?: string, id?: string, name?: string): object;
   }
   module KO {
      /**
       * create kegg background model
       * 
       * 
        * @param genes -
        * @param maps -
        * @param size 
        * + default value Is ``-1``.
        * @param genomeName 
        * + default value Is ``'unknown'``.
        * @param id_map 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
      */
      function background(genes: any, maps: any, size?: object, genomeName?: string, id_map?: any, env?: object): object;
      /**
       * convert the background model to a data table
       * 
       * 
        * @param background -
      */
      function table(background: object): object;
   }
   /**
    * gene/protein KO id background
    * 
    * 
   */
   function KO_reference(): object;
   /**
   */
   function meta_background(enrich: object, graphQuery: object): any;
   module metabolism {
      /**
       * create kegg maps background for the metabolism data analysis
       * 
       * 
        * @param kegg -
        * @param filter 
        * + default value Is ``null``.
      */
      function background(kegg: object, filter?: string): object;
   }
   /**
    * Create the gsea background model for metabolism analysis
    * 
    * 
     * @param kegg the kegg @``T:SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway`` model collection of current organism or the KEGG @``T:SMRUCC.genomics.Assembly.KEGG.WebServices.Map`` data collection.
     * @param reactions A collection of the reference @``T:SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork.ReactionTable`` model 
     *  data for build the metabolism network
     * @param org_name -
     * 
     * + default value Is ``null``.
     * @param is_ko_ref -
     * 
     * + default value Is ``false``.
     * @param multipleOmics 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function metpa(kegg: any, reactions: any, org_name?: string, is_ko_ref?: boolean, multipleOmics?: boolean, env?: object): object;
   /**
    * get all of the molecule id set from the given background model object
    * 
    * 
     * @param background -
   */
   function moleculeIDs(background: object): string;
   module read {
      /**
       * Load GSEA background model from a xml file.
       * 
       * 
        * @param file -
      */
      function background(file: string): object;
   }
   module write {
      /**
       * Save GSEA background model as xml file
       * 
       * 
        * @param background -
      */
      function background(background: object, file: string): boolean;
   }
}
