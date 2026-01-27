// export R# package module type define for javascript/typescript language
//
//    imports "background" from "gseakit";
//
// ref=gseakit.GSEABackground@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * tools for handling GSEA background model.
 * 
*/
declare namespace background {
   module append {
      /**
       * Append id terms to a given gsea background
       * 
       * 
        * @param background -
        * @param term_name -
        * @param terms -
        * @param env -
        * 
        * + default value Is ``null``.
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
        * @param omics Create a enrichment background model for run multiple omics data analysis?
        *  this parameter is only works for the kegg pathway model where you are 
        *  speicifc via the **`clusters`** parameter.
        * 
        * + default value Is ``null``.
        * @param filter_compoundId do compound id filtering when target model is **`omics`**?
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
      function background(clusters: any, background_size?: object, name?: string, tax_id?: string, desc?: string, omics?: object, filter_compoundId?: boolean, kegg_code?: string, env?: object): object;
      /**
       * Extract the gene set list from the background model
       * 
       * > the result list could be used for save as json file for 
       * >  parsed in R by ``jsonlite::fromJSON`` function, and used
       * >  for the gsva analysis.
       * 
        * @param background -
        * @param export_alias 
        * + default value Is ``false``.
        * @return a tuple list object that contains the gene set information,
        *  data result in format like:
        *  
        *  ```r
        *  list(
        *      "cluster id 1" = c("gene id", "gene id", ...),
        *      "cluster id 2" = c("gene id", "gene id", ...),
        *      ...
        *  )
        *  ```
      */
      function geneSet(background: object, export_alias?: boolean): object;
   }
   module background {
      /**
       * do id mapping of the members in the background cluster
       * 
       * 
        * @param background -
        * @param mapping do id translation via this id source list
        * @param subset only the cluster which has the member gene id exists in this
        *  collection then the cluster will be keeps from the result
        *  background if this parameter is not null or empty.
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function id_mapping(background: object, mapping: object, subset?: string, env?: object): object;
   }
   /**
    * summary of the background model as dataframe
    * 
    * 
     * @param background -
     * @param gene_names 
     * + default value Is ``true``.
     * @return row item is the cluster object
   */
   function background_summary(background: object, gene_names?: boolean): object;
   /**
    * get all of the cluster id set from the given background model object
    * 
    * 
     * @param background -
     * @return A character vector of the cluster id(or pathway id) that defined inside the given background model
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
   /**
    * make gsea background dynamic cut
    * 
    * 
     * @param background -
     * @param annotated -
   */
   function cut_background(background: object, annotated: any): object;
   module dag {
      /**
       * create gsea background from a given obo ontology file data.
       * 
       * 
        * @param dag -
        * @param flat Flat the ontology tree into cluster via the ``is_a`` relationship?
        *  
        *  default false, required of the ``enrichment.go`` function for run enrichment analysis
        *  value true, will flat the ontology tree into cluster, then the enrichment analysis could be
        *  applied via the ``enrichment`` function.
        * 
        * + default value Is ``false``.
        * @param verbose_progress 
        * + default value Is ``true``.
        * @param env 
        * + default value Is ``null``.
      */
      function background(dag: object, flat?: boolean, verbose_progress?: boolean, env?: object): object;
   }
   /**
    * cast the cluster data as the enrichment background
    * 
    * 
     * @param geneSet a collection of the gene feature clusters, usualy be a tuple list in format of:
     *  cluster id as the list name and the corresponding tuple list value should be
     *  the gene id character vector.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function fromList(geneSet: object, env?: object): any;
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
       * make filter of the background model
       * 
       * 
        * @param background -
        * @param geneSet usually be a character of the gene id set.
        * 
        * + default value Is ``null``.
        * @param min_size the min feature size is required for each cluster. 
        *  all of the cluster that have the feature number less than this cutoff 
        *  will be removed from the background.
        * 
        * + default value Is ``3``.
        * @param max_intersects the max intersect number that each cluster 
        *  intersect with the input geneSet. all of the clusters that greater than 
        *  this value will be removed from the background.
        * 
        * + default value Is ``500``.
        * @param remove_clusters 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
        * @return a new background model that has cluster filtered by the given rule.
      */
      function filter(background: object, geneSet?: any, min_size?: object, max_intersects?: object, remove_clusters?: any, env?: object): object;
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
        * @param term_map 
        * + default value Is ``false``.
        * @param id_map 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
        * @return a character vector of the intersected gene id set or the cluster id set based on the option of parameter **`get_clusterID`**.
      */
      function intersects(cluster: any, geneSet: string, isLocusTag?: boolean, get_clusterID?: boolean, term_map?: boolean, id_map?: boolean, env?: object): string;
   }
   /**
    * Create a cluster for gsea background
    * 
    * > the input dataframe could be a set of database xrefs, example as:
    * >  
    * >  |xref|name|alias|KEGG|uniprot|
    * >  |----|----|-----|----|-------|
    * >  |    |    |     |    |       |
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
   function gsea_cluster(x: object, clusterId: string, clusterName: string, desc?: string, id?: string, name?: string): object;
   module KO {
      /**
       * create kegg background model
       * 
       * 
        * @param genes a set of molecules with the kegg orthology/compound id mapping
        * @param maps the kegg maps model
        * @param size the background gene size, default -1 or zero means auto calculation.
        * 
        * + default value Is ``-1``.
        * @param genomeName the genome name that tagged with this background model.
        * 
        * + default value Is ``'unknown'``.
        * @param id_map 
        * + default value Is ``null``.
        * @param multiple_omics 
        * + default value Is ``false``.
        * @param term_db 
        * + default value Is ``'unknown'``.
        * @param instance_map 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
      */
      function background(genes: any, maps: any, size?: object, genomeName?: string, id_map?: any, multiple_omics?: boolean, term_db?: string, instance_map?: object, env?: object): object;
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
     * @return A reference background of the kegg pathway by parse the internal resource file.
   */
   function KO_reference(): object;
   /**
   */
   function meta_background(enrich: object, graphQuery: object): object;
   module metabolism {
      /**
       * create kegg maps background for the metabolism data analysis
       * 
       * 
        * @param kegg Should be a collection of the kegg map object
        * @param filter 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
      */
      function background(kegg: any, filter?: string, env?: object): object;
   }
   /**
    * Create the gsea background model for metabolism analysis
    * 
    * 
     * @param kegg the kegg @``T:SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway`` model collection of current organism or 
     *  the KEGG @``T:SMRUCC.genomics.Assembly.KEGG.WebServices.XML.Map`` data collection.
     *  andalso could be a tuple list of the idset.
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
     * @return A character vector of the gene id that defined inside the given background model
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
        * @param file -
      */
      function background(background: object, file: string): boolean;
   }
}
